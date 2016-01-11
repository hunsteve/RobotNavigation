using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using NavigationSimulator;

namespace OnlabNeuralis
{
    
    public partial class NavigationSimulatorForm : Form
    {              
        //public NeuralController neuralController;
        public GridNeuralController neuralController;
        private CarModelInput outInput;
        private CarModelState outState;
        private long timerDiv;
        

        SerialComm serialComm;
        CameraObjectPositionProvider cameraCarPosition;        
        SimulationModeItemManager itemManager;
        bool simulation;
        bool carRunning;

        public NavigationSimulatorForm()
        {
            InitializeComponent();
            buttonStopTraining.Enabled = false;
            buttonStopSim.Enabled = false;            

            timerDiv = 0;
            cameraCarPosition = new CameraObjectPositionProvider(pictureBoxCamPreview);
            simulation = true;
            carRunning = false;
            makeNewItemManager();
        
            //add serial port names to list
            comboBoxSerial.Items.AddRange(SerialPort.GetPortNames());
            comboBoxSerial.SelectedIndex = comboBoxSerial.Items.Count - 1;            

            carModelGraphicControl1.Invalidate();
            carModelGraphicControl1.OnRefreshed += new CarModelGraphicControlRefreshedDelegate(refreshTextboxes);
            
            

            checkBoxShowNNOutput_CheckedChanged(this, null);
            trackBarMu_Scroll(this, null);

            pictureBoxCarMarker.Image = Image.FromFile("marker1.png");
            pictureBoxFinishMarker.Image = Image.FromFile("marker5.png");
            pictureBoxBackground.Image = Image.FromFile("background.png");
            checkCamImages();
            timer1.Enabled = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (carRunning)
            {
                if (timerDiv == 0)
                {
                    ICarPositionProvider carPos;
                    IFinishPositionProvider finishPos;
                    if (simulation)
                    {
                        itemManager.TakeSample();
                        carPos = itemManager;
                        finishPos = itemManager;
                    }
                    else
                    {
                        cameraCarPosition.TakeSample();
                        carPos = cameraCarPosition;
                        finishPos = cameraCarPosition;
                    }

                    //leallitas ha beert a celba
                    double errx = carPos.GetCarState().Position.X - finishPos.GetFinishState(0).Position.X;
                    double erry = carPos.GetCarState().Position.Y - finishPos.GetFinishState(0).Position.Y;
                    double errox = carPos.GetCarState().Orientation.X - finishPos.GetFinishState(0).Orientation.X;
                    double erroy = carPos.GetCarState().Orientation.Y - finishPos.GetFinishState(0).Orientation.Y;

                    if ((errx * errx + erry * erry < CarModel.SHAFT_LENGTH * CarModel.SHAFT_LENGTH) && (errox * errox + erroy * erroy < 0.2))
                    {
                        buttonStopSim_Click(this, null);
                    }
                    else
                    {
                        carModelGraphicControl1.SetReceiveCommand();
                        GridCarModelInput oi;
                        GridCarModelState os;
                        neuralController.SimulateOneStep(GridCarModelState.FromCarModelState(carPos.GetCarState()), out oi, out os);
                        outState = GridCarModelState.ToCarModelState(os);
                        outInput = new CarModelInput(oi.Angle);
                            
                        //outInput = new CarModelInput(20, 100);
                        if (checkBoxSerial.Checked)
                        {
                            byte leftspd = (byte)Convert.ToSByte(ComMath.Normal(outInput.LeftSpeed, -180, 180, -128, 127));
                            byte rightspd = (byte)Convert.ToSByte(ComMath.Normal(outInput.RightSpeed, -180, 180, -128, 127)); //-125, 124
                            if (checkBoxCarEnable.Checked) serialComm.Motor_I2C_Forward(1, leftspd, rightspd);
                            //Thread.Sleep(200);
                        }
                    }
                }

                timerDiv = (timerDiv + 1) % (long)(CarModel.SIMULATION_TIME_STEP * 1000.0 / timer1.Interval);
                if (simulation)
                {
                    //itemManager.Simulate(new MathModelSimulator(), outInput, timer1.Interval / 1000.0);
                    itemManager.SimualteGrid(new GridMathModelSimulator(), new GridCarModelInput(outInput.LeftSpeed, outInput.RightSpeed), timer1.Interval / 1000.0);
                }
                else
                {
                    cameraCarPosition.Simulate(new MathModelSimulator(), outInput, timer1.Interval / 1000.0);
                }
            }
            
            carModelGraphicControl1.Invalidate();                               
        }
        

        private void refreshTextboxes()
        {
            ICarPositionProvider carPos;
            if (simulation) carPos = itemManager;                            
            else carPos = cameraCarPosition;                            

            textBoxPosX.Text = carPos.GetCarState().Position.X.ToString();
            textBoxPosY.Text = carPos.GetCarState().Position.Y.ToString();
            textBoxOriAng.Text = carPos.GetCarState().Angle.ToString();
            textBoxOriX.Text = carPos.GetCarState().Orientation.X.ToString();
            textBoxOriY.Text = carPos.GetCarState().Orientation.Y.ToString();
        }

        public void newTrainEpoch()
        {
            lock (carModelGraphicControl1.trainingModels)
            {
                carModelGraphicControl1.trainingModels = new List<CarModel>();
                lock (carModelGraphicControl1.trainingModels)
                {
                    if (neuralController.trainInnerStates != null)
                    {
                        foreach (GridCarModelState s in neuralController.trainInnerStates)
                        {                            
                            carModelGraphicControl1.trainingModels.Add(new CarModel(GridCarModelState.ToCarModelState(s)));
                        }
                    }

                    if (neuralController.trainInnerStatesOrig != null)
                    {
                        foreach (GridCarModelState s in neuralController.trainInnerStatesOrig)
                        {
                            carModelGraphicControl1.trainingModels.Add(new CarModel(GridCarModelState.ToCarModelState(s), Color.Brown));
                        }
                    }
                }
            }
        }


        private void checkBoxSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSerial.Checked)
            {
                try
                {
                    serialComm = new SerialComm(comboBoxSerial.SelectedItem.ToString(), textBoxSerial);
                    textBoxSerial.AppendText("Connected. \r\n");
                    //serialComm.Comm_Set_Acknowledge(1, true);
                }
                catch (Exception ex)
                {
                    serialComm = null;
                    //checkBoxSerial.Checked = false;
                    textBoxSerial.AppendText(ex.Message + "\r\n");
                }
            }
            else
            {
                serialComm.Close();
                serialComm = null;  
                textBoxSerial.AppendText("Disconnected. \r\n");
            }
            comboBoxSerial.Enabled = !checkBoxSerial.Checked;
        }


        private void checkBoxShowCamImage_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowCamImage.Checked) carModelGraphicControl1.cameraCarPos = cameraCarPosition;
            else carModelGraphicControl1.cameraCarPos = null;
            carModelGraphicControl1.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialComm != null)
            {
                serialComm.Close();
                serialComm = null;
            }

            if (neuralController != null)
            {
                neuralController.StopTrain();
            }

        }

        private void resetController()
        {
            if (simulation)
            {
                neuralController = new GridNeuralController(new GridMathModelSimulator(), itemManager, itemManager, itemManager);
            }
            else 
            {
                neuralController = new GridNeuralController(new GridMathModelSimulator(), cameraCarPosition, cameraCarPosition, cameraCarPosition);
            }
            checkBoxShowNNOutput_CheckedChanged(null, null);
            checkBoxShowCamImage_CheckedChanged(null, null);
            carModelGraphicControl1.Invalidate();
        }

        private void makeNewItemManager()
        {
            itemManager = new SimulationModeItemManager(contextMenuStrip1, carModelGraphicControl1);
            carModelGraphicControl1.simManager = itemManager;
            carModelGraphicControl1.cameraCarPos = cameraCarPosition;
            carModelGraphicControl1.SetSimulation(simulation);
            resetController();
        }

        private void trackBarMu_Scroll(object sender, EventArgs e)
        {
            double mu = 0.01 * Math.Pow(10,trackBarMu.Value/100.0*2)/100.0;
            textBoxMu.Text = mu.ToString();
            neuralController.mu = mu;
        }

        private void checkBoxCameraEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCameraEnabled.Checked)
            {
                cameraCarPosition.StartMarkerFinding(pictureBoxBackground.Image, pictureBoxCarMarker.Image, pictureBoxFinishMarker.Image);                
                simulation = false;                
            }
            else
            {
                simulation = true;
            }
            makeNewItemManager();
            carModelGraphicControl1.SetSimulation(simulation);            
        }        

        private void buttonStartSim_Click(object sender, EventArgs e)
        {
            carRunning = true;
            if (checkBoxSerial.Checked)
            {
                serialComm.Motor_I2C_Forward(2, 55, 55);
            }
            buttonStartSim.Enabled = false;
            buttonStopSim.Enabled = true;
        }

        private void buttonStopSim_Click(object sender, EventArgs e)
        {
            carRunning = false;
            if (checkBoxSerial.Checked)
            {
                serialComm.Motor_Stop(1);
                serialComm.Motor_Stop(2);
            }
            buttonStopSim.Enabled = false;
            buttonStartSim.Enabled = true;            
        }

        private void buttonStartTraining_Click(object sender, EventArgs e)
        {
            neuralController.StartTrain(newTrainEpoch);
            if (!simulation) cameraCarPosition.StartTrain();
            else itemManager.StartTrain();

            buttonStartTraining.Enabled = false;            
            buttonStopTraining.Enabled = true;
            buttonReset.Enabled = false;
            buttonProcessSingleFrame.Enabled = false;

        }

        private void buttonStopTraining_Click(object sender, EventArgs e)
        {
            neuralController.StopTrain();
            if (!simulation) cameraCarPosition.StopTrain();
            else itemManager.StopTrain();

            buttonStopTraining.Enabled = false;
            buttonStartTraining.Enabled = true;            
            buttonReset.Enabled = true;
            buttonProcessSingleFrame.Enabled = true;
        }        

        private void buttonReset_Click(object sender, EventArgs e)
        {
            makeNewItemManager();
            lock (carModelGraphicControl1.trainingModels)
            {
                carModelGraphicControl1.trainingModels = new List<CarModel>();
            }
            carModelGraphicControl1.Invalidate();
        }

        private void checkBoxShowNNOutput_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBoxShowNNOutput.Checked) carModelGraphicControl1.neuralController = neuralController;
            //else 
                carModelGraphicControl1.neuralController = null;
            carModelGraphicControl1.Invalidate();
        }

        private void pictureBoxCarMarker_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "PNG (*.png)|*.png";
            fd.CheckFileExists = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Bitmap bm = new Bitmap(fd.FileName);                
                pictureBoxCarMarker.Image = bm;            
            }
            checkCamImages();
        }

        private void pictureBoxFinishMarker_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "PNG (*.png)|*.png";
            fd.CheckFileExists = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Bitmap bm = new Bitmap(fd.FileName);                
                pictureBoxFinishMarker.Image = bm;
            }
            checkCamImages();
        }

        private void pictureBoxBackground_Click(object sender, EventArgs e)
        {
            pictureBoxBackground.Image = null;
            System.GC.Collect();
            Thread.Sleep(10);
            System.IO.File.Delete("background.png");
            pictureBoxBackground.Image = cameraCarPosition.GetImage();
            pictureBoxBackground.Image.Save("background.png");
            checkCamImages();
        }

        private void checkCamImages()
        {
            if ((pictureBoxBackground.Image != null) && (pictureBoxCarMarker.Image != null) && (pictureBoxFinishMarker.Image != null)) checkBoxCameraEnabled.Enabled = true;
            else checkBoxCameraEnabled.Enabled = false;
        }

        private void buttonProcessSingleFrame_Click(object sender, EventArgs e)
        {
            cameraCarPosition.TakeSample(); 
            carModelGraphicControl1.Invalidate();
        }

        




    }
}
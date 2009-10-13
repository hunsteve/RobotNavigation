using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using NavigationSimulator;


namespace OnlabNeuralis
{
    public delegate void CarModelGraphicControlRefreshedDelegate();

    public partial class CarModelGraphicControl : Control
    {        
        private bool recv = false;
        public List<CarModel> trainingModels;

        private ICarPositionProvider carPositionProvider;
        private IFinishPositionProvider finishPositionProvider;
        private IObstaclePositionProvider obstacleProvider;

        public NeuralController neuralController;

        public CameraObjectPositionProvider cameraCarPos;        
        public SimulationModeItemManager simManager;


        public PointF offset;
        double zoom;
        bool mouseDown;
        Point mouseDownPos;
        private bool simulation;               

        public event CarModelGraphicControlRefreshedDelegate OnRefreshed;

        public CarModelGraphicControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            zoom = 1;
            CalcTransform();
            this.MouseWheel += new MouseEventHandler(CarModelGraphicControl_MouseWheel);
            trainingModels = new List<CarModel>();
        }        

        public void SetReceiveCommand()
        {
            recv = true;
        }

        public void SetSimulation(bool b)
        {
            if (b)
            {
                carPositionProvider = simManager;
                finishPositionProvider = simManager;
                obstacleProvider = simManager;
                simulation = true;
            }
            else
            {
                carPositionProvider = cameraCarPos;
                finishPositionProvider = cameraCarPos;
                obstacleProvider = cameraCarPos;
                simulation = false;
            }
        }

        private Matrix transform, itransform;

        private void CalcTransform()
        {
            Matrix m = new Matrix();
            m.Translate(Width / 2, Height / 2);
            m.Scale((float)zoom, (float)zoom);
            m.Translate(offset.X, offset.Y);
            transform = m;
            itransform = m.Clone();
            itransform.Invert();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.Clear(Color.White);
            if (transform == null) CalcTransform();
            g.Transform = transform;


            int w = (int)((CarModelState.MAX_POS_X - CarModelState.MIN_POS_X) / CarModel.MM_PER_PIXEL);
            int h = (int)((CarModelState.MAX_POS_Y - CarModelState.MIN_POS_Y) / CarModel.MM_PER_PIXEL);


            if (neuralController != null)
            {
                double orix = Math.Cos(carPositionProvider.GetCarState().Angle);
                double oriy = Math.Sin(carPositionProvider.GetCarState().Angle);

                PointF[] pp = new PointF[] { new PointF(0, 0), new PointF(Width - 1, Height - 1) };
                itransform.TransformPoints(pp);               

                pp[0].X = (float)ComMath.Normal((pp[0].X - CarModel.OFFSET_X) * CarModel.MM_PER_PIXEL, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE);
                pp[0].Y = (float)ComMath.Normal((pp[0].Y - CarModel.OFFSET_Y) * CarModel.MM_PER_PIXEL, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE);
                pp[1].X = (float)ComMath.Normal((pp[1].X - CarModel.OFFSET_X) * CarModel.MM_PER_PIXEL, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE);
                pp[1].Y = (float)ComMath.Normal((pp[1].Y - CarModel.OFFSET_Y) * CarModel.MM_PER_PIXEL, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE);
                Bitmap background = neuralController.controller.Visualize(20, 20, new RectangleF(pp[0].X, pp[0].Y, pp[1].X - pp[0].X, pp[1].Y - pp[0].Y), 0, 1, new double[] { 0, 0, orix, oriy }, 0, -10, 10);
                

                pp = new PointF[] { new PointF(0, 0), new PointF(Width - 1, Height - 1) };
                itransform.TransformPoints(pp);                
                
                g.DrawImage(background, new RectangleF(pp[0].X, pp[0].Y, pp[1].X - pp[0].X, pp[1].Y - pp[0].Y), new RectangleF(0, 0, background.Width-1, background.Height-1), GraphicsUnit.Pixel);                
            }

            
            if (cameraCarPos != null)
            {
                Image im = cameraCarPos.GetImage();
               
               
                if (im != null) g.DrawImage(im, new Rectangle(0, 0, w, h), new Rectangle(0, 0, im.Width, im.Height), GraphicsUnit.Pixel);
                
            }                           
            g.DrawRectangle(new Pen(Color.Blue, 3), new Rectangle(0, 0, w, h));
            
            
            if (obstacleProvider != null)
            {
                List<ObstacleModel> obstacles = obstacleProvider.GetObstacleModels(0);
                foreach (ObstacleModel om in obstacles)
                {
                    om.Render(g);                    
                }
            }

            if (finishPositionProvider != null)
            {
                finishPositionProvider.GetFinishModel(0).Render(g);
            }


            if (trainingModels != null)
            {
                lock (trainingModels)
                {
                    foreach (CarModel m in trainingModels)
                    {
                        if (m != null) m.Render(g, 120, false);
                    }
                }
            }

            if (carPositionProvider != null)
            {                
                CarModel model = carPositionProvider.GetCarModel();
                model.Render(g, 255, true);
                if (recv)
                {
                    g.FillEllipse(new SolidBrush(Color.Red), new Rectangle((int)(model.state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X) - 2, (int)(model.state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y) - 2, 4, 4));
                    recv = false;
                }
            }            

            // Calling the base class OnPaint
            base.OnPaint(pe);
            if (OnRefreshed != null) OnRefreshed.Invoke();
        }        

        void CarModelGraphicControl_MouseWheel(object sender, MouseEventArgs e)
        {            
            zoom *= Math.Pow(1.1, e.Delta / 120);
            CalcTransform();
        }

        private void CarModelGraphicControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (!simulation || !simManager.MouseDown(e, itransform))
            {
                if (e.Button == MouseButtons.Left)
                {
                    mouseDown = true;
                    mouseDownPos = e.Location;
                }
            }
        }

        private void CarModelGraphicControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!simulation || !simManager.MouseMove(e, itransform))
            {
                if (mouseDown)
                {
                    offset.X += (float)((e.X - mouseDownPos.X) / zoom);
                    offset.Y += (float)((e.Y - mouseDownPos.Y) / zoom);
                    mouseDownPos = e.Location;
                    CalcTransform();
                }
            }
        }

        private void CarModelGraphicControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (!simulation || !simManager.MouseUp(e, itransform))
            {
                if (e.Button == MouseButtons.Left)
                {
                    mouseDown = false;
                }
            }
        }

        private void CarModelGraphicControl_Layout(object sender, LayoutEventArgs e)
        {
            CalcTransform();
        }

        private void CarModelGraphicControl_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}

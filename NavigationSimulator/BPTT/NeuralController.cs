using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.Threading;
using System.Runtime.CompilerServices;
using NavigationSimulator;
using NeuralNetworkLib;


namespace OnlabNeuralis
{
    public enum inputType
    {
        wheelAngle, wheelSpeed
    };

    public class NeuralController
    {
        
  
        public const double MAX_NEURON_VALUE = 10;
        public const double MIN_NEURON_VALUE = -10;
        public const double POSITION_SCALE = 1;
        private const int EPOCH_COUNT = 40;
        public const inputType INPUT_TYPE = inputType.wheelAngle;

        public MLPDll controller;
        private IModelSimulator model;        

        public List<CarModelState> trainInnerStates;
       
        private IObstaclePositionProvider obstacleProvider;
        private ICarPositionProvider carStateProvider;
        private IFinishPositionProvider finishStateProvider;

        bool trainingStopped;
        Thread trainThread;
        NewTrainEpochDelegate callback;

        public double mu;


        public NeuralController(IModelSimulator model, IObstaclePositionProvider obstacle, ICarPositionProvider start, IFinishPositionProvider finish)
        {
            this.model = model;
            if (INPUT_TYPE == inputType.wheelAngle)
            {
                controller = new MLPDll(new int[] { 20, 1 }, 4);//4 bemenet a state, 1 kimenet az input                        
            }
            else if (INPUT_TYPE == inputType.wheelSpeed)
            {
                controller = new MLPDll(new int[] { 75, 2 }, 4);//4 bemenet a state, 2 kimenet az input                        
            }
            obstacleProvider = obstacle;
            carStateProvider = start;
            finishStateProvider = finish;
            trainingStopped = true;
            mu = 0.005;
        }

        //public NeuralController(IModelSimulator model, IObstaclePositionProvider obstacle, ICarPositionProvider start, IFinishPositionProvider finish, string filename)
        //{
        //    this.model = model;
        //    controller = new MLPDll(filename);
        //    obstacleProvider = obstacle;
        //    carStateProvider = start;
        //    finishStateProvider = finish;
        //    trainingStopped = true;
        //    mu = 0.005;
        //}
        
        public Bitmap VisualizeObstacleField(int width, int height)
        {

            lock (this)
            {
                uint[] buf = new uint[width * height];

                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {

                        CarModelState state = new CarModelState();
                        state.Angle = 0;
                        state.Position = new PointD(ComMath.Normal(x, 0, width - 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                    ComMath.Normal(y, 0, height - 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));

                        double err = obstacleFieldError(state);
                        if (err > 255) err = 255;
                        buf[y * width + x] = 0x80000000 + ((uint)(err));

                    }


                Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                BitmapData bmData = bm.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int len = bmData.Width * bmData.Height;
                unsafe
                {
                    uint* cim = (uint*)bmData.Scan0.ToPointer();//direkt bitmap memoriaba rajzolunk, gyors                
                    for (int i = 0; i < len; ++i)
                    {
                        cim[i] = buf[i];
                    }
                }

                bm.UnlockBits(bmData);
                return bm;

            }
        }

        private double[] obstacleFieldErrorGradient(CarModelState state, int time)
        {
            //C = sum((1/d(X) - 1/d(0))^2)
            //dC/dy_x =...
            //dC/dy_y =...
            double ksi = 0.0001;
            double errX = 0;
            double errY = 0;
            List<ObstacleState> obstacles = obstacleProvider.GetObstacleStates(time);
            foreach (ObstacleState obst in obstacles)
            {
                double x = ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y = ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double x0 = ComMath.Normal(obst.pp.position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y0 = ComMath.Normal(obst.pp.position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);


                double r = ComMath.Normal(obst.radius + CarModel.SHAFT_LENGTH/2, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

                double dist = Math.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0)) - r;
                if (dist <= 0.0001) dist = 0.0001;
                double err = (1 / (dist * dist * dist));
                errX += err * (x - x0);
                errY += err * (y - y0);
            }
            if (obstacles.Count > 0)
            {
                errX /= obstacles.Count;
                errY /= obstacles.Count;
            }
            return new double[] { ksi * errX, ksi * errY, 0, 0 };
        }

        private double obstacleFieldError(CarModelState state)
        {
            double err = 0;
            List<ObstacleState> obstacles = obstacleProvider.GetObstacleStates(1);
            foreach (ObstacleState obst in obstacles)
            {
                double x = ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y = ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double x0 = ComMath.Normal(obst.pp.position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y0 = ComMath.Normal(obst.pp.position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

                double dist = Math.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0));
                err += Math.Pow(1 / dist - 1 / obst.radius, 2);
            }

            return err;
        }


        public delegate void NewTrainEpochDelegate();

        public void StartTrain(NewTrainEpochDelegate aCallback)
        {            
            if (trainingStopped)
            {
                callback = aCallback;
                trainThread = new Thread(new ThreadStart(this.Train));
                trainThread.Start();
                trainThread.IsBackground = true;
                trainingStopped = false;
            }
        }

        public void StopTrain()
        {
            trainingStopped = true;//magatol ki fog lepni                        
        }

        private void Train()
        {
            while (!trainingStopped)
            {
                double error = 0, error2 = 0;
                double sumSimCount = 0;
                for (int epoch = 0; epoch < EPOCH_COUNT; ++epoch)
                {
                    double ii;
                    error += TrainOneEpoch(mu, out ii, out trainInnerStates);
                    sumSimCount += ii;
                }
                error /= EPOCH_COUNT;
                sumSimCount /= EPOCH_COUNT;
                //if (error2 <= error ) mu *= 0.75;
                error2 = error;
                //System.Console.WriteLine(error.ToString() + "  " + sumSimCount.ToString());

                callback();//ertesitsuk a callbacken keresztul a fromot hogy veget ert egy epoch
                Thread.Sleep(0);
            }
            trainInnerStates.Clear();
            callback();
            //controller.SaveNN("neuralcontroller.mlp");
        }

      
        private double TrainOneEpoch(double mu, out double SumSimCount, out List<CarModelState> innerStates)
        {

            int maxSimCount = 100;
            double sumSimCount = 0;
            double error = 0;            
            innerStates = new List<CarModelState>();
            List<double> deltaws = new List<double>();

            MLPDll[] controllers = new MLPDll[maxSimCount];
            IModelSimulator[] models = new IModelSimulator[maxSimCount];

            CarModelState state = carStateProvider.GetCarState();
            CarModelInput input = new CarModelInput();


            //kimenet kiszamitasa                    
            int simCount = 0;
            List<double[]> singleErrors = new List<double[]>();
            List<double[]> regularizationErrors = new List<double[]>();                
            CarModelState laststate;
            bool earlyStop;
            do
            {
                controllers[simCount] = new MLPDll(controller);//lemasoljuk
                models[simCount] = model.Clone();//a modellt is

                laststate = state;
                NeuralController.SimulateOneStep(controllers[simCount], models[simCount], state, out input, out state);//vegigszimulaljuk a simCount darab controlleren es modellen
                innerStates.Add(state);

                //kozbulso hibak kiszamitasa, itt csak az akadalyoktol valo tavolsag "hibajat" vesszuk figyelembe, irany nem szamit -> hibaja 0                    
                regularizationErrors.Add(obstacleFieldErrorGradient(state, simCount));
               
                //minden pont celtol vett tavolsaga
                double[] desiredOutput = (double[])finishStateProvider.GetFinishState(simCount);
                singleErrors.Add(new double[] {  1*ComMath.Normal(desiredOutput[0] - state.Position.X,CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE),
                                                 1*ComMath.Normal(desiredOutput[1] - state.Position.Y,CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
                                                 0.1*ComMath.Normal(desiredOutput[2] - state.Orientation.X,CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
                                                 0.1*ComMath.Normal(desiredOutput[3] - state.Orientation.Y,CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE) }
                                );
                
                ++simCount;

                earlyStop = false;
                if (simCount > 3)
                {
                    double[] err1 = singleErrors[simCount-1];
                    double[] err2 = singleErrors[simCount-2];
                    double[] err3 = singleErrors[simCount-3];
                    double error1, error2, error3;
                    error1 = error2 = error3 = 0;
                    for (int i = 0; i < err1.Length; i++)
	                {
                        error1 += err1[i] * err1[i];
                        error2 += err2[i] * err2[i];
                        error3 += err3[i] * err3[i];
	                }
                    earlyStop = ((error1 > error2) && (error3 > error2));

                    if (earlyStop)
                    {
                        //utolso elemet toroljuk
                        singleErrors.RemoveAt(singleErrors.Count - 1);
                        regularizationErrors.RemoveAt(regularizationErrors.Count - 1);
                        innerStates.RemoveAt(innerStates.Count - 1);
                        --simCount;
                    }                        
                }
                


            }
            while ((simCount < maxSimCount) && !earlyStop);
           

            
            double[] errors = singleErrors[singleErrors.Count-1];               
            
            sumSimCount += simCount;

            //hibavisszaterjesztes
            for (int i = simCount - 1; i >= 0; --i)
            {
                double[] sensitibility;
                models[i].CalcErrorSensibility(errors, out sensitibility);

                double[] inputSensitibility;

                if (INPUT_TYPE == inputType.wheelAngle)
                {
                    inputSensitibility = new double[1];
                    inputSensitibility[0] = sensitibility[6];
                }
                else if (INPUT_TYPE == inputType.wheelSpeed)
                {
                    inputSensitibility = new double[2];
                    inputSensitibility[0] = sensitibility[4];
                    inputSensitibility[1] = sensitibility[5];
                }

                double[] sensitibility2;

                controllers[i].SetOutputError(inputSensitibility);
                controllers[i].Backpropagate();
                controllers[i].CalculateDeltaWeights();
                sensitibility2 = controllers[i].SensitibilityD();
                

                
                errors[0] = (sensitibility[0] + sensitibility2[0]);
                errors[1] = (sensitibility[1] + sensitibility2[1]);
                errors[2] = (sensitibility[2] + sensitibility2[2]);
                errors[3] = (sensitibility[3] + sensitibility2[3]);
               
                //regularizaciobol szarmazo hiba hozzaadasa                    
                errors[0] += regularizationErrors[i][0];
                errors[1] += regularizationErrors[i][1];                


               
            }

            controller.ClearDeltaWeights();
            //sulymodositasok osszegzese     
            for (int i2 = 0; i2 < simCount; ++i2)
            {
                controller.AddDeltaWeights(controllers[i2]);                
            }
            float maxdw = controller.MaxDeltaWeight();
            //if (maxdw < 50) maxdw = 50;

            controller.ChangeWeights(mu / maxdw);

            ////sulymodositasok osszegzese                    
            //for (int i2 = 0; i2 < simCount; ++i2) //simCount
            //{

            //    int count = 0;
            //    for (int i = 1; i < controllers[i2]; ++i)
            //    {
            //        foreach (INeuron n in controllers[i2].mlp[i])
            //        {
            //            foreach (NeuronInput ni in ((Neuron)n).inputs)
            //            {
            //                if (deltaws.Count <= count) deltaws.Add(ni.deltaw);
            //                else deltaws[count] += ni.deltaw;
            //                ++count;
            //            }
            //        }
            //    }
            //}
                
            ////legnagyobb sulymodositas ertekenek meghatarozasa, majd ezzel normalas
            //double maxdw = 1;

            //foreach (double dw in deltaws)
            //{
            //    if (Math.Abs(dw) > maxdw) maxdw = Math.Abs(dw);
            //}

            //if (maxdw < 50) maxdw = 50;

            ////sulymodositasok ervenyre juttatasa a controllerben
            //int count2 = 0;
            
            //for (int i = 1; i < controller.mlp.Count; ++i)
            //{
            //    foreach (INeuron n in controller.mlp[i])
            //    {
            //        foreach (NeuronInput ni in ((Neuron)n).inputs)
            //        {
            //            ni.w += mu * deltaws[count2] / maxdw;
                         
            //            ++count2;
            //        }
            //    }
            //}

                      
            SumSimCount = sumSimCount;
            return error;
        }





        public void Simulate(CarModelState initialState, int simCount, out CarModelInput[] inputs, out CarModelState[] states)
        {
            inputs = new CarModelInput[simCount];
            states = new CarModelState[simCount];
            CarModelState state = initialState;
            for (int i = 0; i < simCount; ++i)
            {
                SimulateOneStep(state, out inputs[i], out states[i]);
                state = states[i];
            }            
        }

        public void SimulateOneStep(CarModelState state, out CarModelInput outInput, out CarModelState outState)
        {
            NeuralController.SimulateOneStep(this.controller, this.model, state, out outInput, out outState);
        }

        public static void SimulateOneStep(MLPDll controller, IModelSimulator model, CarModelState state, out CarModelInput outInput, out CarModelState outState)
        {
            double[] inputs = new double[4];            
            inputs[0] = ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, POSITION_SCALE * MIN_NEURON_VALUE, POSITION_SCALE * MAX_NEURON_VALUE);
            inputs[1] = ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, POSITION_SCALE * MIN_NEURON_VALUE, POSITION_SCALE * MAX_NEURON_VALUE);
            inputs[2] = ComMath.Normal(state.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[3] = ComMath.Normal(state.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            double[] controllerOutputs = controller.Output(inputs);            
                        
            if (INPUT_TYPE == inputType.wheelAngle)
            {                
                outInput = new CarModelInput();
                outInput.Angle = ComMath.Normal(controllerOutputs[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
            }
            else if (INPUT_TYPE == inputType.wheelSpeed)
            {
                outInput = new CarModelInput(ComMath.Normal(controllerOutputs[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED),
                                             ComMath.Normal(controllerOutputs[1], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED));
                //********
                //hatrafele tilos mennie
                if (outInput.LeftSpeed < 0) outInput.LeftSpeed = 0;
                if (outInput.RightSpeed < 0) outInput.RightSpeed = 0;
                //********
            }

            model.SimulateModel(state, outInput, out outState);
        }




        /*private bool IsOutOfBounds(CarModelState state)
        {
            bool oob = false;

            double distance = CarModel.SIMULATION_TIME_STEP * CarModelInput.MAX_SPEED;

            if ((state.Position.X < CarModelState.MIN_POS_X + 2 * distance) ||
                (state.Position.X > CarModelState.MAX_POS_X - 2 * distance) ||
                (state.Position.Y < CarModelState.MIN_POS_Y + 2 * distance) ||
                (state.Position.Y > CarModelState.MAX_POS_Y - 2 * distance)) oob = true;


            if ((Math.Abs(state.Position.X - (CarModelState.MIN_POS_X + CarModelState.MAX_POS_X) / 2) < 3 * distance) &&
                (state.Position.Y - (CarModelState.MIN_POS_Y + CarModelState.MAX_POS_Y) / 2 < 2 * distance) &&
                (state.Position.Y - (CarModelState.MIN_POS_Y + CarModelState.MAX_POS_Y) / 2 > -distance) &&
                (state.Orientation.Y > 0.5)) oob = true;
           

            foreach (ObstacleModel obst in obstacles)
            {
                double x = (state.Position.X - obst.state.position.X);
                double y = (state.Position.Y - obst.state.position.Y);
                if (x * x + y * y <= obst.state.radius * obst.state.radius) oob = true;
            }


            return oob;
        }

        public Bitmap VisualizeBounds(int width, int height, double angle)
        {
            uint[] buf = new uint[width * height];

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {

                    CarModelState state = new CarModelState();
                    state.Angle = angle;
                    state.Position = new PointD(ComMath.Normal(x, 0, width - 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                ComMath.Normal(y, 0, height - 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));

                    bool outp = IsOutOfBounds(state);
                    if (outp) buf[y * width + x] = 0x80FF0000;
                    else buf[y * width + x] = 0x00000000;
                }


            Bitmap bm = new Bitmap(width, height,PixelFormat.Format32bppArgb);
            BitmapData bmData = bm.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int len = bmData.Width * bmData.Height;
            unsafe
            {
                uint* cim = (uint*)bmData.Scan0.ToPointer();//direkt bitmap memoriaba rajzolunk, gyors                
                for (int i = 0; i < len; ++i)
                {
                    cim[i] = buf[i];
                }
            }

            bm.UnlockBits(bmData);
            return bm;
        }*/

    }
}

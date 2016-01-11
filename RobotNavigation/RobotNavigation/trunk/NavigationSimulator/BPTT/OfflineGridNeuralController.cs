//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OnlabNeuralis;
//using NeuralNetworkLib;
//using System.Threading;

//namespace NavigationSimulator
//{
//    class OfflineGridNeuralController
//    {
//        public const double MAX_NEURON_VALUE = 10;
//        public const double MIN_NEURON_VALUE = -10;
//        private const int EPOCH_COUNT = 100;

//        public MLPDll controller;
//        public MLPDll controllerOriginal;
//        private IGridModelSimulator model;      

//        private IObstaclePositionProvider obstacleProvider;
//        private ICarPositionProvider carStateProvider;
//        private IFinishPositionProvider finishStateProvider;

//        bool trainingStopped;
//        Thread trainThread;        

//        public double mu;


//        public OfflineGridNeuralController(IGridModelSimulator model, IObstaclePositionProvider obstacle, ICarPositionProvider start, IFinishPositionProvider finish)
//        {
//            this.model = model;
            
            
//            controller = new MLPDll(new int[] { 40, 1 }, 5, false);//bemenet: 5 - car state, 12 - polargrid                      
            
//            obstacleProvider = obstacle;
//            carStateProvider = start;
//            finishStateProvider = finish;
//            trainingStopped = true;
//            mu = 0.005;
//        }

      

//        private static double[] obstacleFieldErrorGradient(IObstaclePositionProvider ops, GridCarModelState state, int time)
//        {
//            //C = sum((1/d(X) - 1/d(0))^2)
//            //dC/dy_x =...
//            //dC/dy_y =...
//            double ksi = 0.1;
//            double disterr = 0;
//            double orxerr = 0;
//            double oryerr = 0;

//            List<ObstacleState> obstacles = ops.GetObstacleStates(0);
//            foreach (ObstacleState obst in obstacles)//cel az origo, tehat az origohoz relativak az akadalyok, origo felfele nez
//            {
//                double d = ComMath.Normal(Math.Sqrt(obst.pp.position.X * obst.pp.position.X + obst.pp.position.Y * obst.pp.position.Y), GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, MAX_NEURON_VALUE);
//                double a = ComMath.Normal(state.TargetDist , GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, MAX_NEURON_VALUE);

//                double ang = Math.PI - (Math.Atan2(obst.pp.position.Y, obst.pp.position.X) + Math.PI) + state.TargetAngle - state.TargetFinishAngle;
//                if (ang > Math.PI) ang -= 2 * Math.PI;
//                if (ang < -Math.PI) ang += 2 * Math.PI;

//                double AA = -2 * d * Math.Cos(ang);
//                double BB = d * d;
//                double obstdist = Math.Sqrt(a * a + BB + AA * a);
//                double obstang = state.TargetAngle + Math.Sign(ang) * Math.Acos((a * a + obstdist * obstdist - d * d) / (2 * a * obstdist));

                
//                double r = ComMath.Normal(obst.radius + CarModel.SHAFT_LENGTH / 2, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, MAX_NEURON_VALUE);
//                double dist = obstdist - r;
//                if (dist <= 0.0001) dist = 0.0001;
//                double err = 1 / (2 * (dist * dist * dist));


//                disterr += -(AA + 2 * a) * err;
//                double angerr = (-2 * a * d * Math.Sin(ang)) * err;

//                orxerr += -Math.Sin(state.TargetAngle) * angerr;
//                oryerr += Math.Cos(state.TargetAngle) * angerr; 

//            }
//            if (obstacles.Count > 0)
//            {
//                disterr /= obstacles.Count;
//                orxerr /= obstacles.Count;
//                oryerr /= obstacles.Count;
//            }

//            return new double[] { -ksi * disterr, -ksi * orxerr, -ksi * oryerr };
//        }

//        //private double obstacleFieldError(GridCarModelState state)
//        //{
//        //    double err = 0;
//        //    List<ObstacleState> obstacles = obstacleProvider.GetObstacleStates(1);
//        //    foreach (ObstacleState obst in obstacles)
//        //    {
//        //        double d = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//        //        double x = Math.Cos(state.TargetAngle - state.TargetFinishAngle) * d;
//        //        double y = Math.Sin(state.TargetAngle - state.TargetFinishAngle) * d;

//        //        double x0 = ComMath.Normal(obst.pp.position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//        //        double y0 = ComMath.Normal(obst.pp.position.Y, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

//        //        double dist = Math.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0));
//        //        err += Math.Pow(1 / dist - 1 / obst.radius, 2);
//        //    }

//        //    return err;
//        //}

//        private static PolarGrid obstaclePolarGrid(IObstaclePositionProvider ops, GridCarModelState state)
//        {
//            List<ObstacleState> obstacles = ops.GetObstacleStates(0);
//            PolarGrid pg = new PolarGrid();
//            foreach (ObstacleState obst in obstacles)
//            {
//                GridObstacleState gos = GridObstacleState.FromObstacleState(obst, state);
//                pg.AddObstacle(gos);                
//            }
//            return pg;
//        }


//        public delegate void NewTrainEpochDelegate(); 

//        public void StartTrain(NewTrainEpochDelegate aCallback)
//        {
//            if (trainingStopped)
//            {
//                callback = aCallback;
//                trainThread = new Thread(new ThreadStart(this.Train));
//                trainThread.Start();
//                trainThread.IsBackground = true;
//                trainingStopped = false;
//            }
//        }

//        public void StopTrain()
//        {
//            trainingStopped = true;//magatol ki fog lepni                        
//        }

//        private void Train()
//        {
//            while (!trainingStopped)
//            {
//                double error = 0, error2 = 0;
//                double sumSimCount = 0;
//                for (int epoch = 0; epoch < EPOCH_COUNT; ++epoch)
//                {
//                    double ii;
//                    double ii2;
//                    //controller.RandomClearWeakness(0, 1);
//                    error += TrainOneEpoch(controller, model, carStateProvider, finishStateProvider, obstacleProvider, mu, Math.Min(epoch * 2 + 1, EPOCH_COUNT), out ii, out trainInnerStates);
//                    TrainOneEpoch(controllerOriginal, model, carStateProvider, finishStateProvider, obstacleProvider, mu, Math.Min(epoch*2+1, EPOCH_COUNT), out ii2, out trainInnerStatesOrig);
//                    sumSimCount += ii;
//                }
//                error /= EPOCH_COUNT;
//                sumSimCount /= EPOCH_COUNT;
//                //if (error2 <= error ) mu *= 0.75;
//                error2 = error;
//                //System.Console.WriteLine(error.ToString() + "  " + sumSimCount.ToString());

//                callback();//ertesitsuk a callbacken keresztul a fromot hogy veget ert egy epoch
//                Thread.Sleep(0);
//            }
//            //trainInnerStates.Clear();
//            callback();
//            //controller.SaveNN("neuralcontroller.mlp");
//        }


//        private static double TrainOneEpoch(MLPDll controller, IGridModelSimulator model, ICarPositionProvider cps, IFinishPositionProvider fps, IObstaclePositionProvider ops, double mu, int maxSimCount, out double SumSimCount, out List<GridCarModelState> innerStates)
//        {    
//            double sumSimCount = 0;
//            double error = 0;
//            innerStates = new List<GridCarModelState>();
//            List<double> deltaws = new List<double>();

//            MLPDll[] controllers = new MLPDll[maxSimCount];
//            IGridModelSimulator[] models = new IGridModelSimulator[maxSimCount];

//            GridCarModelState state = GridCarModelState.FromCarModelState(cps.GetCarState());
//            GridCarModelInput input = new GridCarModelInput();


//            //kimenet kiszamitasa                    
//            int simCount = 0;
//            List<double[]> singleErrors = new List<double[]>();
//            List<double[]> regularizationErrors = new List<double[]>();
//            GridCarModelState laststate;
//            bool earlyStop;
//            do
//            {
//                if (simCount == 0) controllers[simCount] = new MLPDll(controller);//lemasoljuk
//                else controllers[simCount] = new MLPDll(controllers[simCount - 1]);
//                models[simCount] = model.Clone();//a modellt is

//                laststate = state;
//                GridNeuralController.SimulateOneStep(controllers[simCount], models[simCount], state, out input, out state);//vegigszimulaljuk a simCount darab controlleren es modellen
//                innerStates.Add(state);

//                //kozbulso hibak kiszamitasa, itt csak az akadalyoktol valo tavolsag "hibajat" vesszuk figyelembe, irany nem szamit -> hibaja 0                    
//                regularizationErrors.Add(obstacleFieldErrorGradient(ops, state, simCount));

//                //minden pont celtol vett tavolsaga                
//                double disterror = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, 1);
//                double orientationerror = disterror;
//                if (orientationerror < 0.2) orientationerror = 0;
//                double finishorientationerror = disterror;
//                if (finishorientationerror > 0.05) finishorientationerror = 0;
//                else finishorientationerror = 1;
//                double finishX = Math.Cos(Math.PI - fps.GetFinishState(simCount).Angle);
//                double finishY = Math.Sin(Math.PI - fps.GetFinishState(simCount).Angle);

//                singleErrors.Add(new double[] { -disterror * MAX_NEURON_VALUE ,                                                                                                 
//                                                orientationerror*ComMath.Normal(1 - state.TargetOrientation.X,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
//                                                orientationerror*ComMath.Normal(0 - state.TargetOrientation.Y,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE),
//                                                finishorientationerror*ComMath.Normal(finishX - state.TargetFinishOrientation.X,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
//                                                finishorientationerror*ComMath.Normal(finishY - state.TargetFinishOrientation.Y,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE) }
//                                );
//                ++simCount;

//                earlyStop = false;
//                if (simCount > 3)
//                {
//                    double[] err1 = singleErrors[simCount - 1];
//                    double[] err2 = singleErrors[simCount - 2];
//                    double[] err3 = singleErrors[simCount - 3];
//                    double error1, error2, error3;
//                    error1 = error2 = error3 = 0;
//                    for (int i = 0; i < 1; i++)//err1.Length
//                    {
//                        error1 += err1[i] * err1[i];
//                        error2 += err2[i] * err2[i];
//                        error3 += err3[i] * err3[i];
//                    }
//                    earlyStop = ((error1 > error2) && (error3 > error2));

//                    if (earlyStop)
//                    {
//                        //utolso elemet toroljuk
//                        singleErrors.RemoveAt(singleErrors.Count - 1);
//                        regularizationErrors.RemoveAt(regularizationErrors.Count - 1);
//                        innerStates.RemoveAt(innerStates.Count - 1);
//                        --simCount;
//                    }
//                }



//            }
//            while ((simCount < maxSimCount) && !earlyStop);



//            double[] errors = singleErrors[singleErrors.Count - 1];

//            sumSimCount += simCount;

//            //hibavisszaterjesztes
//            for (int i = simCount - 1; i >= 0; --i)
//            {
//                double[] sensitibility;
//                models[i].CalcErrorSensibility(errors, out sensitibility);

//                double[] inputSensitibility;

                
//                inputSensitibility = new double[1];
//                inputSensitibility[0] = sensitibility[5];
                

//                double[] sensitibility2;

//                controllers[i].SetOutputError(inputSensitibility);
//                controllers[i].Backpropagate();
//                controllers[i].CalculateDeltaWeights();
//                sensitibility2 = controllers[i].SensitibilityD();



//                errors[0] = (sensitibility[0] + sensitibility2[0] + 0.1 * singleErrors[i][0]);
//                errors[1] = (sensitibility[1] + sensitibility2[1] + 0 * singleErrors[i][1]);
//                errors[2] = (sensitibility[2] + sensitibility2[2] + 0 * singleErrors[i][2]);
//                errors[3] = (sensitibility[3] + sensitibility2[3] + singleErrors[i][3]);
//                errors[4] = (sensitibility[4] + sensitibility2[4] + singleErrors[i][4]);

//                //regularizaciobol szarmazo hiba hozzaadasa                    
//                errors[0] += regularizationErrors[i][0];
//                errors[1] += regularizationErrors[i][1];
//                errors[2] += regularizationErrors[i][2];



//            }

//            controller.ClearDeltaWeights();
//            //sulymodositasok osszegzese     
//            for (int i2 = 0; i2 < simCount; ++i2)
//            {
//                controller.AddDeltaWeights(controllers[i2]);
//            }
//            float maxdw = controller.MaxDeltaWeight();
//            //if (maxdw < 50) maxdw = 50;

//            controller.ChangeWeights(mu / maxdw);

//            ////sulymodositasok osszegzese                    
//            //for (int i2 = 0; i2 < simCount; ++i2) //simCount
//            //{

//            //    int count = 0;
//            //    for (int i = 1; i < controllers[i2]; ++i)
//            //    {
//            //        foreach (INeuron n in controllers[i2].mlp[i])
//            //        {
//            //            foreach (NeuronInput ni in ((Neuron)n).inputs)
//            //            {
//            //                if (deltaws.Count <= count) deltaws.Add(ni.deltaw);
//            //                else deltaws[count] += ni.deltaw;
//            //                ++count;
//            //            }
//            //        }
//            //    }
//            //}

//            ////legnagyobb sulymodositas ertekenek meghatarozasa, majd ezzel normalas
//            //double maxdw = 1;

//            //foreach (double dw in deltaws)
//            //{
//            //    if (Math.Abs(dw) > maxdw) maxdw = Math.Abs(dw);
//            //}

//            //if (maxdw < 50) maxdw = 50;

//            ////sulymodositasok ervenyre juttatasa a controllerben
//            //int count2 = 0;

//            //for (int i = 1; i < controller.mlp.Count; ++i)
//            //{
//            //    foreach (INeuron n in controller.mlp[i])
//            //    {
//            //        foreach (NeuronInput ni in ((Neuron)n).inputs)
//            //        {
//            //            ni.w += mu * deltaws[count2] / maxdw;

//            //            ++count2;
//            //        }
//            //    }
//            //}


//            SumSimCount = sumSimCount;
//            return error;
//        }





//        public void Simulate(GridCarModelState initialState, int simCount, out GridCarModelInput[] inputs, out GridCarModelState[] states)
//        {
//            inputs = new GridCarModelInput[simCount];
//            states = new GridCarModelState[simCount];
//            GridCarModelState state = initialState;
//            for (int i = 0; i < simCount; ++i)
//            {
//                SimulateOneStep(state, out inputs[i], out states[i]);
//                state = states[i];
//            }
//        }

//        public void SimulateOneStep(GridCarModelState state, out GridCarModelInput outInput, out GridCarModelState outState)
//        {
//            GridNeuralController.SimulateOneStep(this.controller, this.model, state, out outInput, out outState);
//        }

//        public static void SimulateOneStep(MLPDll controller, IGridModelSimulator model, GridCarModelState state, out GridCarModelInput outInput, out GridCarModelState outState)
//        {
//            double[] inputs = new double[5];
//            inputs[0] = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, MAX_NEURON_VALUE);
//            inputs[1] = ComMath.Normal(state.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//            inputs[2] = ComMath.Normal(state.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//            inputs[3] = ComMath.Normal(state.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//            inputs[4] = ComMath.Normal(state.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
//            double[] controllerOutputs = controller.Output(inputs);

            
//            outInput = new GridCarModelInput();
//            outInput.Angle = ComMath.Normal(controllerOutputs[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE);
                       
//            model.SimulateModel(state, outInput, out outState);
//        }
    
//    }
//}

﻿using System;
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
 
    public class GridNeuralController
    {


        public const double MAX_NEURON_VALUE = 10;
        public const double MIN_NEURON_VALUE = -10;
        private const int EPOCH_COUNT = 40;

        public MLPDll controller;
        private IGridModelSimulator model;

        public List<GridCarModelState> trainInnerStates;

        private IObstaclePositionProvider obstacleProvider;
        private ICarPositionProvider carStateProvider;
        private IFinishPositionProvider finishStateProvider;

        bool trainingStopped;
        Thread trainThread;
        NewTrainEpochDelegate callback;

        public double mu;


        public GridNeuralController(IGridModelSimulator model, IObstaclePositionProvider obstacle, ICarPositionProvider start, IFinishPositionProvider finish)
        {
            this.model = model;
            
            
            controller = new MLPDll(new int[] { 40, 1 }, 5);//4 bemenet a state, 1 kimenet az input                        
            
            obstacleProvider = obstacle;
            carStateProvider = start;
            finishStateProvider = finish;
            trainingStopped = true;
            mu = 0.005;
        }

      

        private double[] obstacleFieldErrorGradient(GridCarModelState state, int time)
        {
            //C = sum((1/d(X) - 1/d(0))^2)
            //dC/dy_x =...
            //dC/dy_y =...
            double ksi = 0.0001;
            double errX = 0;
            double errY = 0;
            List<ObstacleState> obstacles = obstacleProvider.GetObstacleStates(time);
            foreach (ObstacleState obst in obstacles)//cel az origo, tehat az origohoz relativak az akadalyok, origo felfele nez
            {
                double d = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double x = Math.Cos(state.TargetAngle - state.TargetFinishAngle) * d;
                double y = Math.Sin(state.TargetAngle - state.TargetFinishAngle) * d;

                double x0 = ComMath.Normal(obst.pp.position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y0 = ComMath.Normal(obst.pp.position.Y, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);


                double r = ComMath.Normal(obst.radius + CarModel.SHAFT_LENGTH / 2, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

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

        private double obstacleFieldError(GridCarModelState state)
        {
            double err = 0;
            List<ObstacleState> obstacles = obstacleProvider.GetObstacleStates(1);
            foreach (ObstacleState obst in obstacles)
            {
                double d = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double x = Math.Cos(state.TargetAngle - state.TargetFinishAngle) * d;
                double y = Math.Sin(state.TargetAngle - state.TargetFinishAngle) * d;

                double x0 = ComMath.Normal(obst.pp.position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                double y0 = ComMath.Normal(obst.pp.position.Y, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

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


        private double TrainOneEpoch(double mu, out double SumSimCount, out List<GridCarModelState> innerStates)
        {

            int maxSimCount = 100;
            double sumSimCount = 0;
            double error = 0;
            innerStates = new List<GridCarModelState>();
            List<double> deltaws = new List<double>();

            MLPDll[] controllers = new MLPDll[maxSimCount];
            IGridModelSimulator[] models = new IGridModelSimulator[maxSimCount];

            GridCarModelState state = new GridCarModelState(1000,new PointD(-1,-1),new PointD(1,0));//carStateProvider.GetCarState();
            GridCarModelInput input = new GridCarModelInput();


            //kimenet kiszamitasa                    
            int simCount = 0;
            List<double[]> singleErrors = new List<double[]>();
            List<double[]> regularizationErrors = new List<double[]>();
            GridCarModelState laststate;
            bool earlyStop;
            do
            {
                controllers[simCount] = new MLPDll(controller);//lemasoljuk
                models[simCount] = model.Clone();//a modellt is

                laststate = state;
                GridNeuralController.SimulateOneStep(controllers[simCount], models[simCount], state, out input, out state);//vegigszimulaljuk a simCount darab controlleren es modellen
                innerStates.Add(state);

                //kozbulso hibak kiszamitasa, itt csak az akadalyoktol valo tavolsag "hibajat" vesszuk figyelembe, irany nem szamit -> hibaja 0                    
                regularizationErrors.Add(obstacleFieldErrorGradient(state, simCount));

                //minden pont celtol vett tavolsaga
                double[] desiredOutput = (double[])finishStateProvider.GetFinishState(simCount);
                double disterror = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, 1);
                double orientationerror = disterror * disterror;
                if (orientationerror < 0.2) orientationerror = 0;
                singleErrors.Add(new double[] { -disterror * MAX_NEURON_VALUE ,                                                                                                 
                                                orientationerror*ComMath.Normal(1 - state.TargetOrientation.X,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
                                                orientationerror*ComMath.Normal(0 - state.TargetOrientation.Y,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE),
                                                (1-disterror)*(1-disterror)*ComMath.Normal(0 - state.TargetFinishOrientation.X,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE), 
                                                (1-disterror)*(1-disterror)*ComMath.Normal(1 - state.TargetFinishOrientation.Y,GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE) }
                                );

                ++simCount;

                earlyStop = false;
                if (simCount > 3)
                {
                    double[] err1 = singleErrors[simCount - 1];
                    double[] err2 = singleErrors[simCount - 2];
                    double[] err3 = singleErrors[simCount - 3];
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



            double[] errors = singleErrors[singleErrors.Count - 1];

            sumSimCount += simCount;

            //hibavisszaterjesztes
            for (int i = simCount - 1; i >= 0; --i)
            {
                double[] sensitibility;
                models[i].CalcErrorSensibility(errors, out sensitibility);

                double[] inputSensitibility;

                
                inputSensitibility = new double[1];
                inputSensitibility[0] = sensitibility[5];
                

                double[] sensitibility2;

                controllers[i].SetOutputError(inputSensitibility);
                controllers[i].Backpropagate();
                controllers[i].CalculateDeltaWeights();
                sensitibility2 = controllers[i].SensitibilityD();



                errors[0] = (sensitibility[0] + sensitibility2[0]);
                errors[1] = (sensitibility[1] + sensitibility2[1] + singleErrors[i][1]);
                errors[2] = (sensitibility[2] + sensitibility2[2] + singleErrors[i][2]);
                errors[3] = (sensitibility[3] + sensitibility2[3] + 0*singleErrors[i][3]);
                errors[4] = (sensitibility[4] + sensitibility2[4] + 0*singleErrors[i][4]);

                //regularizaciobol szarmazo hiba hozzaadasa                    
                //errors[0] += regularizationErrors[i][0];
                //errors[1] += regularizationErrors[i][1];



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





        public void Simulate(GridCarModelState initialState, int simCount, out GridCarModelInput[] inputs, out GridCarModelState[] states)
        {
            inputs = new GridCarModelInput[simCount];
            states = new GridCarModelState[simCount];
            GridCarModelState state = initialState;
            for (int i = 0; i < simCount; ++i)
            {
                SimulateOneStep(state, out inputs[i], out states[i]);
                state = states[i];
            }
        }

        public void SimulateOneStep(GridCarModelState state, out GridCarModelInput outInput, out GridCarModelState outState)
        {
            GridNeuralController.SimulateOneStep(this.controller, this.model, state, out outInput, out outState);
        }

        public static void SimulateOneStep(MLPDll controller, IGridModelSimulator model, GridCarModelState state, out GridCarModelInput outInput, out GridCarModelState outState)
        {
            double[] inputs = new double[5];
            inputs[0] = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, MAX_NEURON_VALUE);
            inputs[1] = ComMath.Normal(state.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[2] = ComMath.Normal(state.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[3] = ComMath.Normal(state.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[4] = ComMath.Normal(state.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            double[] controllerOutputs = controller.Output(inputs);

            
            outInput = new GridCarModelInput();
            outInput.Angle = ComMath.Normal(controllerOutputs[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE);
                       
            model.SimulateModel(state, outInput, out outState);
        }





    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Globalization;
using NeuralNetworkLib;

namespace OnlabNeuralis
{
    public class NeuralModelSimulator : IModelSimulator
    {
        MLPDll mlp;
        public const double MAX_NEURON_VALUE = 1;
        public const double MIN_NEURON_VALUE = -1;
        private const int EPOCH_COUNT = 10000;

        public NeuralModelSimulator()
        {
            mlp = new MLPDll(new int[] { 50, 4 }, 7);
        }

        //public NeuralModelSimulator(String filename)
        //{
        //    mlp = new MLPDll(filename);                        
        //}     

        public void Train(IModelSimulator sourceSimulator, double treshold)
        {
            Random r = new Random();            
            double mu = 0.0001;
            long count = 0;
            double errors = 0, errors2 = double.MaxValue;
            double[] error = new double[4];
            do
            {                               
                for (int i2 = 0; i2 < EPOCH_COUNT; ++i2)
                {                    
                    double angle = r.NextDouble() * 2 * Math.PI;//veletlen szog                    
                    CarModelState carstate = new CarModelState(new PointD(ComMath.Normal(r.NextDouble(), 0, 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                                          ComMath.Normal(r.NextDouble(), 0, 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                               new PointD(ComMath.Normal(Math.Cos(angle), -1, 1, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
                                                                          ComMath.Normal(Math.Sin(angle), -1, 1, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY)));

                    CarModelInput carinput = new CarModelInput();
                    //= new CarModelInput(ComMath.Normal(r.NextDouble(), 0, 1, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED),
                    //                                           ComMath.Normal(r.NextDouble(), 0, 1, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED));                        
                    carinput.Angle = ComMath.Normal(r.NextDouble(), 0, 1, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
                                        
                    
                    CarModelState state, state2;
                    double[] output;
                    sourceSimulator.SimulateModel(carstate, carinput, out state);
                    this.SimulateModel(carstate, carinput, out state2, out output);                  


                    error = new double[4];
                    error[0] = -output[0] + ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                    error[1] = -output[1] + ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                    error[2] = -output[2] + ComMath.Normal(state.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                    error[3] = -output[3] + ComMath.Normal(state.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                    count++;
                    mlp.Train(mu, error);
                    errors += error[0] * error[0] + error[1] * error[1] + error[2] * error[2] + error[3] * error[3];
                }
                errors /= EPOCH_COUNT;
                //if (errors2 < errors) mu *= 0.75;
                errors2 = errors;                                              
                System.Console.WriteLine(errors.ToString());
            } while (errors > treshold);
           // mlp.SaveNN("neuralmodel.mlp");
            
        }

        //dont use, not fully implemented
        /*public void TrainLM(IModelSimulator sourceSimulator, double treshold)
        {
            List<TrainingPoint> tp = NeuralModelSimulator.GenerateTrainingPoints(sourceSimulator, 11);

            double error;
            do
            {
                error = mlp.TrainLM(tp);
                System.Console.WriteLine(error.ToString());
            }
            while (error > treshold);

        }
        */

        //public static List<TrainingPoint> GenerateTrainingPoints(IModelSimulator sourceSimulator, int cc)
        //{            
        //    List<TrainingPoint> tp = new List<TrainingPoint>();
        //    TextWriter tw = new StreamWriter("trainingpoints.dat");
        //    for (int i1 = 0; i1 < cc; ++i1)
        //    {
        //        for (int i2 = 0; i2 < cc; ++i2)
        //        {
        //            for (int i3 = 0; i3 < cc; ++i3)
        //            {
        //                for (int i4 = 0; i4 < cc; ++i4)
        //                {
        //                    for (int i5 = 0; i5 < cc; ++i5)
        //                    {

        //                        double angle = ComMath.Normal(i3, 0, cc - 1, 0, 2 * Math.PI);
        //                        double[] input = new double[6];
        //                        input[0] = ComMath.Normal(i1, 0, cc - 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        input[1] = ComMath.Normal(i2, 0, cc - 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        input[2] = ComMath.Normal(Math.Cos(angle), -1, 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        input[3] = ComMath.Normal(Math.Sin(angle), -1, 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        input[4] = ComMath.Normal(i4, 0, cc - 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        input[5] = ComMath.Normal(i5, 0, cc - 1, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

        //                        CarModelState carstate = new CarModelState(new PointD(ComMath.Normal(input[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
        //                                                                              ComMath.Normal(input[1], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
        //                                                                   new PointD(ComMath.Normal(input[2], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
        //                                                                              ComMath.Normal(input[3], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY)));

        //                        CarModelInput carinput = new CarModelInput(ComMath.Normal(input[4], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED),
        //                                                                   ComMath.Normal(input[5], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED));

        //                        CarModelState state;
        //                        sourceSimulator.SimulateModel(carstate, carinput, out state);

        //                        double[] desiredOutput = new double[4];


        //                        desiredOutput[0] = ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        desiredOutput[1] = ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        desiredOutput[2] = ComMath.Normal(state.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
        //                        desiredOutput[3] = ComMath.Normal(state.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

        //                        tp.Add(new TrainingPoint(input, desiredOutput));

        //                        for (int ii = 0; ii < input.Length; ii++)
        //                        {
        //                            tw.Write(String.Format(CultureInfo.InvariantCulture, "{0:0.###############}", input[ii]) + ",");
        //                        }
        //                        for (int ii = 0; ii < desiredOutput.Length - 1; ii++)
        //                        {
        //                            tw.Write(String.Format(CultureInfo.InvariantCulture, "{0:0.###############}", desiredOutput[ii]) + ",");                                        
        //                        }
        //                        tw.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###############}", desiredOutput[desiredOutput.Length - 1]));
                                
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    tw.Flush();
        //    tw.Close();            
        //    return tp;
        //}


        #region ModelSimulator Members

        public void SimulateModel(CarModelState state, CarModelInput input, out CarModelState output)
        {
            SimulateModel(state, input, CarModel.SIMULATION_TIME_STEP, out output);
        }

        public void SimulateModel(CarModelState state, CarModelInput input, out CarModelState output, out double[] NNOutput)
        {
            SimulateModel(state, input, CarModel.SIMULATION_TIME_STEP, out output, out NNOutput);
        }
        
        public void SimulateModel(CarModelState state, CarModelInput input, double timeStep, out CarModelState output)
        {
            double[] NNout;
            SimulateModel(state, input, timeStep, out output, out NNout);
        }

        public void SimulateModel(CarModelState state, CarModelInput input, double timeStep, out CarModelState output, out double[] NNOutput)
        {
            double[] inputs = new double[7];            
                       
            if (NeuralController.INPUT_TYPE == inputType.wheelAngle)
            {               
                inputs[6] = ComMath.Normal(input.Angle, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE, MIN_NEURON_VALUE, MAX_NEURON_VALUE);                
            }
            else if (NeuralController.INPUT_TYPE == inputType.wheelSpeed)
            {
                inputs[4] = ComMath.Normal(input.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
                inputs[5] = ComMath.Normal(input.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED, MIN_NEURON_VALUE, MAX_NEURON_VALUE);                
            }

            inputs[0] = ComMath.Normal(state.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[1] = ComMath.Normal(state.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[2] = ComMath.Normal(state.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);
            inputs[3] = ComMath.Normal(state.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, MIN_NEURON_VALUE, MAX_NEURON_VALUE);

            NNOutput = mlp.Output(inputs);

            double X = ComMath.Normal(NNOutput[0], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X);
            double Y = ComMath.Normal(NNOutput[1], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y);
            double oX = ComMath.Normal(NNOutput[2], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY);
            double oY = ComMath.Normal(NNOutput[3], MIN_NEURON_VALUE, MAX_NEURON_VALUE, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY);

            output = new CarModelState(new PointD(X, Y), new PointD(oX, oY));
        }

        public void CalcErrorSensibility(double[] errors, out double[] sensitibility)
        {   
            mlp.SetOutputError(errors);
            mlp.Backpropagate();
            sensitibility = mlp.SensitibilityD();
        }

        public IModelSimulator Clone()
        {
            NeuralModelSimulator ret = new NeuralModelSimulator();
            ret.mlp = new MLPDll(this.mlp);
            return ret;
        }

        #endregion
    }
}

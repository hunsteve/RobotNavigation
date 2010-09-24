//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;

//namespace OnlabNeuralis
//{
//    public class GridMathModelSimulator : IGridModelSimulator
//    {

//        GridCarModelState state;
//        GridCarModelInput input;

//        #region ModelSimulator Members

//        //matematikai modell
//        public void SimulateModel(GridCarModelState state, GridCarModelInput input, out GridCarModelState output)
//        {
//            SimulateModel(state, input, CarModel.SIMULATION_TIME_STEP, out output);
//        }

//        public void SimulateModel(GridCarModelState state, GridCarModelInput input, double timeStep, out GridCarModelState output)
//        {
//            this.state = state;
//            this.input = input;
//            GridCarModelState state2 = state;
//            double dAngle = (input.LeftSpeed - input.RightSpeed) * timeStep / CarModel.SHAFT_LENGTH;
//            double lamda = 1;
//            if (dAngle != 0) lamda = 2 / dAngle * Math.Sin(dAngle / 2);
//            double vectLength = (input.RightSpeed + input.LeftSpeed) / 2 * timeStep * lamda;

//            state2.TargetDist = Math.Sqrt(state.TargetDist * state.TargetDist + vectLength * vectLength - 2 * state.TargetDist * vectLength * Math.Cos(state.TargetAngle - dAngle / 2));

//            state2.TargetAngle = state.TargetAngle - dAngle + Math.Acos((state.TargetDist * state.TargetDist + state2.TargetDist * state2.TargetDist - vectLength * vectLength) / (2 * state.TargetDist * state2.TargetDist));

//            output = state2;
//        }
        
//        public void CalcErrorSensibility(double[] errors, out double[] sensitibility)
//        {

//            GridCarModelState output1, output2, state1, state2, origiState = this.state;
//            GridCarModelInput input1, input2, origiInput = this.input;
//            double DIFF_C = 0.001;
//            sensitibility = new double[7];

//            //******
//            //POS X
//            //******

//            state1 = origiState;
//            state1.Position = new PointD(ComMath.Normal(ComMath.Normal(state1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
//                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
//                                         state1.Position.Y);
//            this.SimulateModel(state1, origiInput, out output1);
//            state2 = origiState;
//            state2.Position = new PointD(ComMath.Normal(ComMath.Normal(state2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
//                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
//                                         state2.Position.Y);
//            this.SimulateModel(state2, origiInput, out output2);
//            sensitibility[0] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //POS Y
//            //******       

//            state1 = origiState;
//            state1.Position = new PointD(state1.Position.X,
//                                         ComMath.Normal(ComMath.Normal(state1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
//                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
//            this.SimulateModel(state1, origiInput, out output1);
//            state2 = origiState;
//            state2.Position = new PointD(state2.Position.X,
//                                         ComMath.Normal(ComMath.Normal(state2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
//                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
//            this.SimulateModel(state2, origiInput, out output2);
//            sensitibility[1] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //ORIENTATION X
//            //******

//            state1 = origiState;
//            state1.Orientation = new PointD(ComMath.Normal(ComMath.Normal(state1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
//                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
//                                            state1.Orientation.Y);
//            this.SimulateModel(state1, origiInput, out output1);
//            state2 = origiState;
//            state2.Orientation = new PointD(ComMath.Normal(ComMath.Normal(state2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
//                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                                  NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                  CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
//                                           state2.Orientation.Y);
//            this.SimulateModel(state2, origiInput, out output2);
//            sensitibility[2] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //ORIENTATION Y
//            //******

//            state1 = origiState;
//            state1.Orientation = new PointD(state1.Orientation.X,
//                                            ComMath.Normal(ComMath.Normal(state1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
//                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY));
//            this.SimulateModel(state1, origiInput, out output1);
//            state2 = origiState;
//            state2.Orientation = new PointD(state2.Orientation.X,
//                                            ComMath.Normal(ComMath.Normal(state2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
//                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY));
//            this.SimulateModel(state2, origiInput, out output2);
//            sensitibility[3] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //LEFT SPEED
//            //******

//            input1 = origiInput;
//            input1.LeftSpeed = ComMath.Normal(ComMath.Normal(input1.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
//                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
//            this.SimulateModel(origiState, input1, out output1);
//            input2 = origiInput;
//            input2.LeftSpeed = ComMath.Normal(ComMath.Normal(input2.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
//                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
//            this.SimulateModel(origiState, input2, out output2);
//            sensitibility[4] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //RIGHT SPEED
//            //******

//            input1 = origiInput;
//            input1.RightSpeed = ComMath.Normal(ComMath.Normal(input1.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
//                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
//            this.SimulateModel(origiState, input1, out output1);
//            input2 = origiInput;
//            input2.RightSpeed = ComMath.Normal(ComMath.Normal(input2.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
//                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
//            this.SimulateModel(origiState, input2, out output2);
//            sensitibility[5] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

//            //******
//            //WHEEL ANGLE
//            //******

//            input1 = origiInput;
//            input1.Angle = ComMath.Normal(ComMath.Normal(input1.Angle, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE,
//                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
//                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                       CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
//            this.SimulateModel(origiState, input1, out output1);
//            input2 = origiInput;
//            input2.Angle = ComMath.Normal(ComMath.Normal(input2.Angle, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE,
//                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
//                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
//                                       CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
//            this.SimulateModel(origiState, input2, out output2);
//            sensitibility[6] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
//                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
//                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
//                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
//                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];





//            this.state = origiState;
//            this.input = origiInput;
//        }

//        public IGridModelSimulator Clone()
//        {
//            GridMathModelSimulator ret = new GridMathModelSimulator();
//            ret.input = input;
//            ret.state = state;
//            return ret;
//        }

//        #endregion
//    }
//}

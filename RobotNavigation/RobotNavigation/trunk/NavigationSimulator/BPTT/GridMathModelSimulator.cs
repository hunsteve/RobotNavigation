using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public class GridMathModelSimulator : IGridModelSimulator
    {

        GridCarModelState state;
        GridCarModelInput input;

        #region ModelSimulator Members

        //matematikai modell
        public void SimulateModel(GridCarModelState state, GridCarModelInput input, out GridCarModelState output)
        {
            SimulateModel(state, input, CarModel.SIMULATION_TIME_STEP, out output);
        }

        public void SimulateModel(GridCarModelState state, GridCarModelInput input, double timeStep, out GridCarModelState output)
        {
            this.state = state;
            this.input = input;
            GridCarModelState state2 = state;
            double dAngle = (input.LeftSpeed - input.RightSpeed) * timeStep / CarModel.SHAFT_LENGTH;
            double lamda = 1;
            if (dAngle != 0) lamda = 2 / dAngle * Math.Sin(dAngle / 2);
            double vectLength = (input.RightSpeed + input.LeftSpeed) / 2 * timeStep * lamda;

            state2.TargetDist = Math.Sqrt(state.TargetDist * state.TargetDist + vectLength * vectLength - 2 * state.TargetDist * vectLength * Math.Cos(state.TargetAngle - dAngle / 2));

            state2.TargetAngle = state.TargetAngle - dAngle + Math.Acos((state.TargetDist * state.TargetDist + state2.TargetDist * state2.TargetDist - vectLength * vectLength) / (2 * state.TargetDist * state2.TargetDist));

            state2.TargetFinishAngle -= dAngle;

            output = state2;
        }

        public void CalcErrorSensibility(double[] errors, out double[] sensitibility)//errors: distance error, orientation x,y error, finish orientation x,y error
        {

            GridCarModelState output1, output2, state1, state2, origiState = this.state;
            GridCarModelInput input1, input2, origiInput = this.input;
            double DIFF_C = 0.001;
            sensitibility = new double[6];

            //******
            //DIST
            //******

            state1 = origiState;
            state1.TargetDist = ComMath.Normal(ComMath.Normal(state1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST);
                                        
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.TargetDist = ComMath.Normal(ComMath.Normal(state2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[0] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                               (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];

            
            //******
            //ORIENTATION X
            //******

            state1 = origiState;
            state1.TargetOrientation = new PointD(ComMath.Normal(ComMath.Normal(state1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY),
                                            state1.TargetOrientation.Y);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.TargetOrientation = new PointD(ComMath.Normal(ComMath.Normal(state2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                  NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                  GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY),
                                           state2.TargetOrientation.Y);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[1] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                                (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                                (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                                (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                                (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];
            //******
            //ORIENTATION Y
            //******

            state1 = origiState;
            state1.TargetOrientation = new PointD(state1.TargetOrientation.X,
                                            ComMath.Normal(ComMath.Normal(state1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY));
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.TargetOrientation = new PointD(state2.TargetOrientation.X,
                                            ComMath.Normal(ComMath.Normal(state2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY));
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[2] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                  ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                                 (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                  ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                                 (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                  ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                                 (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                  ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                                 (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                  ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];

            //******
            //FINISH ORIENTATION X
            //******

            state1 = origiState;
            state1.TargetFinishOrientation = new PointD(ComMath.Normal(ComMath.Normal(state1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY),
                                            state1.TargetFinishOrientation.Y);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.TargetFinishOrientation = new PointD(ComMath.Normal(ComMath.Normal(state2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                  NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                  GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY),
                                           state2.TargetFinishOrientation.Y);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[3] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                                (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                                (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                                (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                                (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];
            //******
            //FINISH ORIENTATION Y
            //******

            state1 = origiState;
            state1.TargetFinishOrientation = new PointD(state1.TargetFinishOrientation.X,
                                            ComMath.Normal(ComMath.Normal(state1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY));
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.TargetFinishOrientation = new PointD(state2.TargetFinishOrientation.X,
                                            ComMath.Normal(ComMath.Normal(state2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY));
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[4] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                               (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];            
            
            //******
            //ANGLE
            //******

            input1 = origiInput;
            input1.Angle = ComMath.Normal(ComMath.Normal(input1.Angle, GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.Angle = ComMath.Normal(ComMath.Normal(input2.Angle, GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       GridCarModelInput.MIN_ANGLE, GridCarModelInput.MAX_ANGLE);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[5] = (ComMath.Normal(output2.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                                (ComMath.Normal(output2.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                                (ComMath.Normal(output2.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                                (ComMath.Normal(output2.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.X, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3] +
                                (ComMath.Normal(output2.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                 ComMath.Normal(output1.TargetFinishOrientation.Y, GridCarModelState.MIN_OR_XY, GridCarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[4];
                       

            this.state = origiState;
            this.input = origiInput;
        }

        public IGridModelSimulator Clone()
        {
            GridMathModelSimulator ret = new GridMathModelSimulator();
            ret.input = input;
            ret.state = state;
            return ret;
        }

        #endregion
    }
}

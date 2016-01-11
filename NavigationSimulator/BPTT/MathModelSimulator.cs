using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public class MathModelSimulator : IModelSimulator
    {

        CarModelState state;
        CarModelInput input;        

        #region ModelSimulator Members

        //matematikai modell
        public void SimulateModel(CarModelState state, CarModelInput input, out CarModelState output)
        {
            SimulateModel(state, input, CarModel.SIMULATION_TIME_STEP, out output);
        }

        public void SimulateModel(CarModelState state, CarModelInput input, double timeStep, out CarModelState output)
        {
            this.state = state;
            this.input = input;
            CarModelState state2 = state;
            double dAngle = (input.LeftSpeed - input.RightSpeed) * timeStep / CarModel.SHAFT_LENGTH;
            double lamda = 1;
            if (dAngle != 0) lamda = 2 / dAngle * Math.Sin(dAngle / 2);
            double vectLength = (input.RightSpeed + input.LeftSpeed) / 2 * timeStep * lamda;

            state2.Angle += dAngle;
            PointD p = new PointD((state.Position.X + vectLength * Math.Cos(state2.Angle - dAngle / 2)),
                                  (state.Position.Y + vectLength * Math.Sin(state2.Angle - dAngle / 2)));
            state2.Position = p;

            output = state2;
        }
        
        //nem jo
        /*
        public void CalcErrorSensibility(double[] errors, out double[] sensitibility)
        {            
            double dAngle = (input.RightSpeed - input.LeftSpeed) * CarModel.SIMULATION_TIME_STEP / CarModel.SHAFT_LENGTH;
            double lamda = 1;
            if (dAngle != 0) lamda = 4 / dAngle * Math.Sin(dAngle / 2);
            double vectLength = (input.RightSpeed + input.LeftSpeed) / 2 * CarModel.SIMULATION_TIME_STEP * lamda;

           
            //ez felfoghato egy forgataskent is:
            //vectLength * (state.Orientation.X *Math.Cos(-dAngle / 2) - state.Orientation.Y * Math.Sin(-dAngle / 2))
            //vectLength * (state.Orientation.X *Math.Sin(-dAngle / 2) + state.Orientation.Y * Math.Cos(-dAngle / 2))
            //
            PointD p = new PointD((state.Position.X + vectLength * Math.Cos(state.Angle - dAngle / 2)),
                                  (state.Position.Y + vectLength * Math.Sin(state.Angle - dAngle / 2)));                      

            //kimenetek bemenet szerinti derivaltjai
            double dAngle_rightSpeed = CarModel.SIMULATION_TIME_STEP / CarModel.SHAFT_LENGTH;
            double dAngle_leftSpeed = -CarModel.SIMULATION_TIME_STEP / CarModel.SHAFT_LENGTH;
            double lamda_rightSpeed = 0;
            double lamda_leftSpeed = 0;
            if (dAngle != 0)
            {
                lamda_rightSpeed = dAngle_rightSpeed * (-4 / Math.Pow(dAngle, 2) * Math.Sin(dAngle / 2) + 2 / dAngle * Math.Cos(dAngle / 2));
                lamda_leftSpeed = dAngle_leftSpeed * (-4 / Math.Pow(dAngle, 2) * Math.Sin(dAngle / 2) + 2 / dAngle * Math.Cos(dAngle / 2));
            }
            double vectLength_rightSpeed = lamda_rightSpeed * 1 / 2 * CarModel.SIMULATION_TIME_STEP * lamda;
            double vectLength_leftSpeed = lamda_leftSpeed * 1 / 2 * CarModel.SIMULATION_TIME_STEP * lamda;


            double outposX_inposX = 1;
            double outposX_inposY = 0;
            double outposX_inangX = vectLength * Math.Cos(- dAngle / 2);
            double outposX_inangY = - vectLength * Math.Sin(- dAngle / 2);
            double outposX_inrightSpeed = vectLength_rightSpeed * (state.Orientation.X *Math.Cos(-dAngle / 2) - state.Orientation.Y * Math.Sin(-dAngle / 2)) + dAngle_rightSpeed * vectLength * (-1 / 2) * (state.Orientation.X *(-Math.Sin(-dAngle / 2)) - state.Orientation.Y *Math.Cos(-dAngle / 2));
            double outposX_inleftSpeed = vectLength_leftSpeed * (state.Orientation.X *Math.Cos(-dAngle / 2) - state.Orientation.Y * Math.Sin(-dAngle / 2)) + dAngle_leftSpeed * vectLength * (-1 / 2) * (state.Orientation.X *(-Math.Sin(-dAngle / 2)) + state.Orientation.Y *Math.Cos(-dAngle / 2));

            double outposY_inposX = 0;
            double outposY_inposY = 1;
            double outposY_inangX = vectLength * Math.Sin(- dAngle / 2);
            double outposY_inangY = vectLength * Math.Cos(- dAngle / 2);
            double outposY_inrightSpeed = vectLength_rightSpeed * (state.Orientation.X *Math.Sin(-dAngle / 2) + state.Orientation.Y * Math.Cos(-dAngle / 2)) + dAngle_rightSpeed * vectLength * (-1 / 2) * (state.Orientation.X *Math.Cos(-dAngle / 2) - state.Orientation.Y *Math.Sin(-dAngle / 2));
            double outposY_inleftSpeed = vectLength_leftSpeed * (state.Orientation.X * Math.Sin(-dAngle / 2) + state.Orientation.Y * Math.Cos(-dAngle / 2)) + dAngle_leftSpeed * vectLength * (-1 / 2) * (state.Orientation.X * Math.Cos(-dAngle / 2) - state.Orientation.Y * Math.Sin(-dAngle / 2));


            double outangX_inposX = 0;
            double outangX_inposY = 0;
            double outangX_inangX = Math.Cos(dAngle);
            double outangX_inangY = -Math.Sin(dAngle);
            double outangX_inrightSpeed = dAngle_rightSpeed * (state.Orientation.X * (-Math.Sin(dAngle)) + state.Orientation.Y * (-Math.Cos(dAngle)));
            double outangX_inleftSpeed = dAngle_leftSpeed * (state.Orientation.X * (-Math.Sin(dAngle)) + state.Orientation.Y * (-Math.Cos(dAngle)));

            double outangY_inposX = 0;
            double outangY_inposY = 0;
            double outangY_inangX = Math.Sin(dAngle);
            double outangY_inangY = Math.Cos(dAngle);
            double outangY_inrightSpeed = dAngle_rightSpeed * (state.Orientation.X * Math.Cos(dAngle) - state.Orientation.Y * Math.Sin(dAngle));
            double outangY_inleftSpeed = dAngle_leftSpeed * (state.Orientation.X * Math.Cos(dAngle) - state.Orientation.Y * Math.Sin(dAngle));

            sensitibility = new double[6];
            sensitibility[0] = (outposX_inposX * errors[0] + outposY_inposX * errors[1] + outangX_inposX * errors[2] + outangY_inposX * errors[3]);
            sensitibility[1] = (outposX_inposY * errors[0] + outposY_inposY * errors[1] + outangX_inposY * errors[2] + outangY_inposY * errors[3]);
            sensitibility[2] = (outposX_inangX * errors[0] + outposY_inangX * errors[1] + outangX_inangX * errors[2] + outangY_inangX * errors[3]);
            sensitibility[3] = (outposX_inangY * errors[0] + outposY_inangY * errors[1] + outangX_inangY * errors[2] + outangY_inangY * errors[3]);
            sensitibility[4] = (outposX_inrightSpeed * errors[0] + outposY_inrightSpeed * errors[1] + outangX_inrightSpeed * errors[2] + outangY_inrightSpeed * errors[3]);
            sensitibility[5] = (outposX_inleftSpeed * errors[0] + outposY_inleftSpeed * errors[1] + outangX_inleftSpeed * errors[2] + outangY_inleftSpeed * errors[3]);
        }
        */
        
        
        
        public void CalcErrorSensibility(double[] errors, out double[] sensitibility)
        {

            CarModelState output1, output2, state1, state2, origiState = this.state;
            CarModelInput input1, input2, origiInput = this.input;
            double DIFF_C = 0.001;
            sensitibility = new double[7];

            //******
            //POS X
            //******

            state1 = origiState; 
            state1.Position = new PointD(ComMath.Normal(ComMath.Normal(state1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2, 
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE, 
                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X), 
                                         state1.Position.Y);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Position = new PointD(ComMath.Normal(ComMath.Normal(state2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                         state2.Position.Y);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[0] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //POS Y
            //******       

            state1 = origiState;
            state1.Position = new PointD(state1.Position.X,
                                         ComMath.Normal(ComMath.Normal(state1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Position = new PointD(state2.Position.X,
                                         ComMath.Normal(ComMath.Normal(state2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[1] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //ORIENTATION X
            //******

            state1 = origiState;
            state1.Orientation = new PointD(ComMath.Normal(ComMath.Normal(state1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
                                            state1.Orientation.Y);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Orientation = new PointD(ComMath.Normal(ComMath.Normal(state2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                  NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                  CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY),
                                           state2.Orientation.Y);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[2] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //ORIENTATION Y
            //******

            state1 = origiState;
            state1.Orientation = new PointD(state1.Orientation.X,
                                            ComMath.Normal(ComMath.Normal(state1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY));
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Orientation = new PointD(state2.Orientation.X,
                                            ComMath.Normal(ComMath.Normal(state2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY,
                                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                   NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                   CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY));
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[3] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //LEFT SPEED
            //******
            
            input1 = origiInput; 
            input1.LeftSpeed = ComMath.Normal(ComMath.Normal(input1.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.LeftSpeed = ComMath.Normal(ComMath.Normal(input2.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[4] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //RIGHT SPEED
            //******
            
            input1 = origiInput;
            input1.RightSpeed = ComMath.Normal(ComMath.Normal(input1.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.RightSpeed = ComMath.Normal(ComMath.Normal(input2.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[5] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];

            //******
            //WHEEL ANGLE
            //******

            input1 = origiInput;
            input1.Angle = ComMath.Normal(ComMath.Normal(input1.Angle, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.Angle = ComMath.Normal(ComMath.Normal(input2.Angle, CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_ANGLE, CarModelInput.MAX_ANGLE);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[6] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(output2.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.X, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2] +
                               (ComMath.Normal(output2.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Orientation.Y, CarModelState.MIN_OR_XY, CarModelState.MAX_OR_XY, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[3];





            this.state = origiState;
            this.input = origiInput;
        }


       /* public void CalcErrorSensibility(double[] errors, out double[] sensitibility)
        {

            CarModelState output1, output2, state1, state2, origiState = this.state;
            CarModelInput input1, input2, origiInput = this.input;
            double DIFF_C = 0.001;
            sensitibility = new double[5];

            //******
            //POS X
            //******
            state1 = origiState;
            state1.Position = new PointD(ComMath.Normal(ComMath.Normal(state1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                         state1.Position.Y);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Position = new PointD(ComMath.Normal(ComMath.Normal(state2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                         state2.Position.Y);
            this.SimulateModel(state2, origiInput, out output2);                        
            sensitibility[0] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                               (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                               (ComMath.Normal(ComMath.AngleDiff(output2.Angle,output1.Angle), -Math.PI, Math.PI, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2];

            //******
            //POS Y
            //******

            state1 = origiState;
            state1.Position = new PointD(state1.Position.X,
                                         ComMath.Normal(ComMath.Normal(state1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Position = new PointD(state2.Position.X,
                                         ComMath.Normal(ComMath.Normal(state2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y,
                                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                                NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                                CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[1] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                              (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                              (ComMath.Normal(ComMath.AngleDiff(output2.Angle, output1.Angle), -Math.PI, Math.PI, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2];
            //******
            //ANGLE
            //******
            state1 = origiState;
            state1.Angle = ComMath.Normal(ComMath.Normal(state1.Angle, -Math.PI, Math.PI,
                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                          -Math.PI, Math.PI);
            this.SimulateModel(state1, origiInput, out output1);
            state2 = origiState;
            state2.Angle = ComMath.Normal(ComMath.Normal(state2.Angle, -Math.PI, Math.PI,
                                                         NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                          NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                          -Math.PI, Math.PI);
            this.SimulateModel(state2, origiInput, out output2);
            sensitibility[2] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                           ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                                          (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                                           ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                                          (ComMath.Normal(ComMath.AngleDiff(output2.Angle, output1.Angle), -Math.PI, Math.PI, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2];

            //******
            //LEFT SPEED
            //******
            input1 = origiInput;
            input1.LeftSpeed = ComMath.Normal(ComMath.Normal(input1.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.LeftSpeed = ComMath.Normal(ComMath.Normal(input2.LeftSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                             NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                      NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                      CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[3] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                              (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                              (ComMath.Normal(ComMath.AngleDiff(output2.Angle, output1.Angle), -Math.PI, Math.PI, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2];
            //******
            //RIGHT SPEED
            //******
            input1 = origiInput;
            input1.RightSpeed = ComMath.Normal(ComMath.Normal(input1.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) - DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input1, out output1);
            input2 = origiInput;
            input2.RightSpeed = ComMath.Normal(ComMath.Normal(input2.RightSpeed, CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED,
                                              NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) + DIFF_C / 2,
                                       NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE,
                                       CarModelInput.MIN_SPEED, CarModelInput.MAX_SPEED);
            this.SimulateModel(origiState, input2, out output2);
            sensitibility[4] = (ComMath.Normal(output2.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.X, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[0] +
                              (ComMath.Normal(output2.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE) -
                               ComMath.Normal(output1.Position.Y, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[1] +
                              (ComMath.Normal(ComMath.AngleDiff(output2.Angle, output1.Angle), -Math.PI, Math.PI, NeuralController.MIN_NEURON_VALUE, NeuralController.MAX_NEURON_VALUE)) / DIFF_C * errors[2];
            
            this.state = origiState;
            this.input = origiInput;
        }
        */



        public IModelSimulator Clone()
        {
            MathModelSimulator ret = new MathModelSimulator();
            ret.input = input;
            ret.state = state;
            return ret;
        }

        #endregion
    }
}

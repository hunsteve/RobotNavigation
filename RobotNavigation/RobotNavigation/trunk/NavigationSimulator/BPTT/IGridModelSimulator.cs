using System;
using System.Collections.Generic;
using System.Text;

namespace OnlabNeuralis
{
    public interface IGridModelSimulator
    {
        //input -> state and input
        //output -> output of model
        //gradient -> d output / d input (input)
        void SimulateModel(GridCarModelState state, GridCarModelInput input, out GridCarModelState output);
        void SimulateModel(GridCarModelState state, GridCarModelInput input, double timeStep, out GridCarModelState output);
        void CalcErrorSensibility(double[] errors, out double[] sensitibility);
        IGridModelSimulator Clone();

    }
}


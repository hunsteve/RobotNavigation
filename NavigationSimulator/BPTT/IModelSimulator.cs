using System;
using System.Collections.Generic;
using System.Text;

namespace OnlabNeuralis
{
    public interface IModelSimulator
    {
        //input -> state and input
        //output -> output of model
        //gradient -> d output / d input (input)
        void SimulateModel(CarModelState state, CarModelInput input, out CarModelState output);
        void SimulateModel(CarModelState state, CarModelInput input, double timeStep, out CarModelState output);
        void CalcErrorSensibility(double[] errors, out double[] sensitibility);
        IModelSimulator Clone();

    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace NNImage
{
    public class TrainingPoint
    {
        public double[] input;
        public double[] desiredOutput;
        public TrainingPoint(double[] input, double[] desiredOutput)
        {
            this.input = input;
            this.desiredOutput = desiredOutput;
        }

    }
}

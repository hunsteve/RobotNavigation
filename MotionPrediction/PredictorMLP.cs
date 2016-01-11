using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NNImage;
using OnlabNeuralis;

namespace DiplomaMunka
{
    class PredictorMLP
    {
        private MLP mlp;
        public int dimension, inputLength;
        private Random r;

        private double mu, preverror;
        private long trainCount;
     

        public PredictorMLP(int aDimension, int aInputLength)
        {
            dimension = aDimension;
            inputLength = aInputLength;            
            mlp = new MLP(new int[] { 30, dimension }, dimension * inputLength);
            r = new Random();
            mu = 0.0001;
            trainCount = 0;            
        }



        public double Train(double[] inputValues)
        {
            if (inputValues.Length >= dimension * (inputLength + 1))
            {
                double error = 0;
                for (int i = inputLength; i < inputValues.Length / dimension; ++i)
                {
                    double[] input = new double[dimension * inputLength];
                    int i3 = 0;
                    for (int i2 = (i - inputLength) * dimension; i2 < i * dimension; ++i2)
                    {
                        input[i3] = inputValues[i2];                        
                        i3 ++;
                    }
                    double[] outp = mlp.Output(input);
                    double[] err = new double[dimension];
                    for (int i2 = 0; i2 < dimension; ++i2)
                    {
                        err[i2] = inputValues[i * dimension + i2] - outp[i2];
                        error += err[i2] * err[i2];
                    }
                    mlp.Train(mu, err);
                    trainCount++;
                }
                //if (error > 1.2 * preverror)
                //{
                //    if (trainCount > 20000) mu *= 0.2;
                //}
                //else if (error < 0.8 * preverror)
                //{
                //    mu *= 1.2;
                //}
                preverror = error / inputValues.Length * dimension;
                return preverror;
            }
            else
            {
                throw new Exception("Not enough data!");                
            }            
        }

        public double[] Predict(double[] inputValues)
        {
            if (inputValues.Length >= dimension * inputLength)
            {                
                double[] input = new double[dimension * inputLength];
                int i3 = 0;
                for (int i2 = (inputValues.Length / dimension - inputLength) * dimension; i2 < (inputValues.Length / dimension) * dimension; ++i2)
                {
                    input[i3] = inputValues[i2];
                    i3++;
                }

                double[] outp = mlp.Output(input);

                return outp;
            }
            else
            {
                throw new Exception("Not enough data!");
            } 
        }

        public double[] PredictMore(double[] inputValues, int count)
        {
            List<double> inp = new List<double>(inputValues);
            List<double> ret = new List<double>();
            for (int i = 0; i < count; ++i)
            {
                double[] p = Predict(inp.ToArray());
                inp.AddRange(p);
                ret.AddRange(p);
            }
            return ret.ToArray();            
        }
    }
}

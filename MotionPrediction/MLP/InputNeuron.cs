using System;
using System.Collections.Generic;
using System.Text;
using NNImage;

namespace NNImage
{    
    public class InputNeuron: INeuron
    {
        double[] data;
        int index;
        public double error;

        public InputNeuron()
        {
            data = new double[1];
            index = 0;
            data[0] = 0;
        }

        public InputNeuron(double[] data,int index)
        {
            SetInput(data, index);
        }

        public void SetInput(double[] data,int index)
        {
            this.data = data;
            this.index = index;
        }

        #region INeuron Members
        
        public double  Output()
        {
            return data[index];
        }

        public void  AddError(double error)
        {
            this.error += error;
        }

        public void SetNullError()
        {
            error = 0;
        }

        #endregion
    }

}

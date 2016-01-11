using System;
using System.Collections.Generic;
using System.Text;

namespace NNImage
{
    public interface INeuron
    {
        double Output();
        void AddError(double error);
    }
}

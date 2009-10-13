using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public interface IFinishPositionProvider
    {
        FinishState GetFinishState(int iteration);
        FinishModel GetFinishModel(int iteration);
    }
}

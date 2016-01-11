using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public interface ICarPositionProvider
    {
        CarModelState GetCarState();
        CarModel GetCarModel();        
    }
}

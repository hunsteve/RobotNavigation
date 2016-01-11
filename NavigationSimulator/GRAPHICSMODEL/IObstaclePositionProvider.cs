using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public interface IObstaclePositionProvider
    {        
        List<ObstacleState> GetObstacleStates(int iteration);
        List<ObstacleModel> GetObstacleModels(int iteration);
    }
}

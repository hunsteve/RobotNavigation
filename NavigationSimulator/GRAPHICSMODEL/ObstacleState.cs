using System;
using System.Collections.Generic;
using System.Text;
using NavigationSimulator;

namespace OnlabNeuralis
{
    public class ObstacleState
    {
        public PredictablePosition pp;
        public double radius;

        public ObstacleState()
        {
            pp = new PredictablePosition();
            this.radius = 0;
        }

        public ObstacleState(PointD position, double radius)
        {
            pp = new PredictablePosition(position);
            this.radius = radius;
        }       
    }
}

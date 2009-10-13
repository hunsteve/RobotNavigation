using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OnlabNeuralis;

namespace NavigationSimulator
{
    class PositionAndOrientationPredictor
    {
        PositionPredictor position;
        PositionPredictor orientation;

        public PositionAndOrientationPredictor(int aMaxLength, int aInputLength)
        {
            position = new PositionPredictor(aMaxLength, aInputLength);
            orientation = new PositionPredictor(aMaxLength, aInputLength);
        }

        public void AddPoint(PointD pos, PointD orn)
        {
            position.AddPoint(pos);
            orientation.AddPoint(orn);
        }       

        public double Train()
        {
            double err1 = position.Train();
            double err2 = orientation.Train();
            return err1 + err2;
        }

        public List<PointD> PredictNextPositions(int count)
        {
            return position.PredictNextPoints(count);
        }

        public List<PointD> PredictNextOrientations(int count)
        {
            return orientation.PredictNextPoints(count);
        }
    }
    
}

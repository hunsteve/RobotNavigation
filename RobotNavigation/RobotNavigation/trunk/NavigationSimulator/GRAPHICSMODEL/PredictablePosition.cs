using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public class PredictablePosition
    {
        public PointD position;
        public PositionPredictor positionPredictor; 

        private double partialTimeStepInterval;

        public PredictablePosition()
        {
            this.position = new PointD(0,0);
            partialTimeStepInterval = 0;
        }

        public PredictablePosition(PointD point)
        {
            this.position = point;
            partialTimeStepInterval = 0;
        }

        public void AddNewPosition(PointD point) 
        {
            this.position = point;
            if (positionPredictor == null) positionPredictor = new PositionPredictor(45, 10);
            positionPredictor.AddPoint(point);
        }

        public void SetPosition(PointD point)
        {
            positionPredictor = null;
            this.position = point;
        }

        public List<PointD> PredictNextPositions(int count)
        {
            if (positionPredictor != null) lock (positionPredictor)
            {
                return positionPredictor.PredictNextPoints(count);
            }
            else return null;
        }

        public List<PointD> GetPreviousPositions()
        {
            if (positionPredictor != null) return positionPredictor.GetFullRoute();                            
            else return null;
        }

        public double Train()
        {
            if (positionPredictor != null) lock (positionPredictor) 
            {
                return positionPredictor.Train();
            }
            else return 0;
        }

        public void Simulate(double interval)
        {
            partialTimeStepInterval += interval;
            List<PointD> list = PredictNextPositions(1);

            if (partialTimeStepInterval > 1)
            {
                partialTimeStepInterval -= 1;
                if (list != null)
                {
                    AddNewPosition(list[0]);
                    Train();
                    position = list[0];
                }
            }
            else
            {
                List<PointD> prev = GetPreviousPositions();
                if ((prev != null) && (list != null)) 
                {
                    PointD last = prev[prev.Count - 1];
                    position.X = last.X * (1 - partialTimeStepInterval) + list[0].X * partialTimeStepInterval;
                    position.Y = last.Y * (1 - partialTimeStepInterval) + list[0].Y * partialTimeStepInterval;
                }
            }
        }
    }
}

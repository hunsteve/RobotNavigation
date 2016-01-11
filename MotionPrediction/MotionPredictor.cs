using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OnlabNeuralis;

namespace DiplomaMunka
{
    class MotionPredictor
    {
        private const int NEEDED_COUNT = 2;

        private List<Point> route;
        private List<Point> tempRoute;
        private int maxLength, inputLength;
        private PredictorMLP mlp;

        public MotionPredictor(int aMaxLength, int aInputLength)
        {
            maxLength = aMaxLength;
            inputLength = aInputLength;
            route = new List<Point>();
            tempRoute = new List<Point>();
        }

        public void AddPoint(Point p)
        {
            lock (tempRoute)
            {
                tempRoute.Add(p);
            }
        }

        private double dist(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private double angle(Point a, Point b)
        {
            return Math.Atan2(a.Y - b.Y, a.X - b.X);
        }

        private double[] MakeInput(List<Point> aRoute)
        {            
            double[] input = null;
            if (route.Count > NEEDED_COUNT)
            {
                input = new double[(aRoute.Count - NEEDED_COUNT + 1) * 2];
                for (int i = 0; i < input.Length / 2; i++)
                {

                    input[i * 2 + 0] = aRoute[i + 1].X - aRoute[i + 0].X;
                    input[i * 2 + 1] = aRoute[i + 1].Y - aRoute[i + 0].Y;
                }
            }
            return input;
        }

        private List<Point> MakeOutput(List<Point> aRoute, double[] outp)
        {
            List<Point> ret = new List<Point>();            
            for (int i = aRoute.Count - NEEDED_COUNT + 1; i < aRoute.Count; ++i)
            {
                ret.Add(route[i]);
            }
            for (int i = 0; i < outp.Length / 2; i++ )
            {
              
                double x = ret[i].X + outp[i * 2 + 0];
                double y = ret[i].Y + outp[i * 2 + 1];
                
                ret.Add(new Point((int)x, (int)y));
            }
            ret.RemoveRange(0, NEEDED_COUNT-1);
            return ret;
        }

        internal double Train()
        {
            lock (tempRoute)
            {
                if (tempRoute.Count > 0)
                {
                    route.AddRange(tempRoute);

                    int length = (route.Count - NEEDED_COUNT) / 2;
                    if (length > inputLength) length = inputLength;

                    if ((length > 0) && ((mlp == null) || (mlp.inputLength != length))) mlp = new PredictorMLP(2, length);
                    
                    if (route.Count > maxLength) route.RemoveRange(0, route.Count - maxLength);
                    tempRoute.Clear();
                }
            }
            double[] input = MakeInput(route);
            if ((input != null) && (mlp != null)) return mlp.Train(input);
            else return 0;
        }

        internal List<Point> PredictNextPoints(int count)
        {          
            double[] input = MakeInput(route);
            if ((input != null) && (mlp != null))
            {
                double[] outp = mlp.PredictMore(MakeInput(route), count);
                return MakeOutput(route, outp);
            }
            else return null;
        }
    }
}

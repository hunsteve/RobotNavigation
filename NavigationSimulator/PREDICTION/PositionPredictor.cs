using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public class PositionPredictor
    {
        private const int NEEDED_COUNT = 2;

        private List<PointD> route;
        private List<PointD> tempRoute;
        private int maxLength, inputLength;
        private PredictorMLP mlp;
        private double lasterror;

        public PositionPredictor(int aMaxLength, int aInputLength)
        {
            maxLength = aMaxLength;
            inputLength = aInputLength;
            route = new List<PointD>();
            tempRoute = new List<PointD>();
            lasterror = 1000;
        }

        public void AddPoint(PointD p)
        {
            lock (tempRoute)
            {
                tempRoute.Add(p);
            }
        }

        public List<PointD> GetRoute()
        {
            return new List<PointD>(route);
        }

        public List<PointD> GetFullRoute() //includes tempRoute
        {
            List<PointD> ret = new List<PointD>(route);
            ret.AddRange(tempRoute);
            return ret;
        }

        private double dist(PointD a, PointD b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private double angle(PointD a, PointD b)
        {
            return Math.Atan2(a.Y - b.Y, a.X - b.X);
        }

        private double[] MakeInput(List<PointD> aRoute)
        {            
            double[] input = null;
            if (route.Count > NEEDED_COUNT)
            {
                input = new double[(aRoute.Count - NEEDED_COUNT + 1) * 2];
                for (int i = 0; i < input.Length / 2; i++)
                {

                    input[i * 2 + 0] = (aRoute[i + 1].X - aRoute[i + 0].X) / 4.0;
                    input[i * 2 + 1] = (aRoute[i + 1].Y - aRoute[i + 0].Y) / 4.0;
                }
            }
            return input;
        }

        private List<PointD> MakeOutput(List<PointD> aRoute, double[] outp)
        {
            List<PointD> ret = new List<PointD>();            
            for (int i = aRoute.Count - NEEDED_COUNT + 1; i < aRoute.Count; ++i)
            {
                ret.Add(route[i]);
            }
            for (int i = 0; i < outp.Length / 2; i++ )
            {

                double x = ret[i].X + outp[i * 2 + 0] * 4;
                double y = ret[i].Y + outp[i * 2 + 1] * 4;
                
                ret.Add(new PointD(x, y));
            }
            ret.RemoveRange(0, NEEDED_COUNT-1);
            return ret;
        }

        public double Train()
        {
            
            lock (tempRoute)
            {
                if (tempRoute.Count > 0)
                {
                    route.AddRange(tempRoute);

                    int length = (route.Count - NEEDED_COUNT) / 2;
                    if (length > inputLength) length = inputLength;

                    if ((length > 0) && ((mlp == null) || (mlp.inputLength != length)))
                    {
                        lasterror = 1000;
                        mlp = new PredictorMLP(2, length);
                    }

                    if (route.Count > maxLength) route.RemoveRange(0, route.Count - maxLength);
                    tempRoute.Clear();
                }
            }
            double[] input = MakeInput(route);
            if ((input != null) && (mlp != null))
            {
                if (lasterror > 0.08)
                {
                    lasterror = mlp.Train(input);
                }
                return lasterror;
            }
            else return 0;
          
            return 0;
        }

        internal List<PointD> PredictNextPoints(int count)
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

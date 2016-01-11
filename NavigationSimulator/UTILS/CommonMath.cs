using System;
using System.Collections.Generic;
using System.Text;

namespace OnlabNeuralis
{
    public class ComMath
    {
        public static double Normal(double value, double valueMin, double valueMax, double normalMin, double normalMax)
        {
            double r = ((value - valueMin) / (valueMax - valueMin)) * (normalMax - normalMin) + normalMin;
            return r;
        }

        public static double AngleDiff(double angle1, double angle2)
        {
            double angleError = angle1 - angle2;
            if (angleError < -Math.PI) angleError += 2 * Math.PI;
            else if (angleError > Math.PI) angleError -= 2 * Math.PI;
            return angleError;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OnlabNeuralis
{
    public class FinishState
    {
        private PointD position;
        private PointD orientation;//origo koruli egysegvektor

        public static explicit operator double[](FinishState fs)
        {
            return new double[] { fs.Position.X, fs.Position.Y, fs.Orientation.X, fs.Orientation.Y };
        }

        public FinishState(double[] arg)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = new PointD(arg[0], arg[1]);
            this.Orientation = new PointD(arg[2], arg[3]);
        }


        public FinishState(PointD position, PointD orientation)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = position;
            this.Orientation = orientation;
        }

        public FinishState(PointD position, double angle)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = position;
            this.Angle = angle;
        }
	
        public PointD Position
        {
            get 
            {
                return position;
            }
            set 
            {
                position = value;
            }
        }


        public PointD Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                this.Angle = Math.Atan2(value.Y, value.X);                   
            }
        }

        public double Angle
        {
            get
            {
                return Math.Atan2(orientation.Y, orientation.X);                
            }
            set
            {
                orientation = new PointD(Math.Cos(value), Math.Sin(value));
            }
        }

    }
}

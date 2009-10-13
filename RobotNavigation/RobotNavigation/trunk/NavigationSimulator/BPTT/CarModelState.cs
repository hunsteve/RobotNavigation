using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public struct CarModelState
    {
        public const double MAX_POS_X = 1300;  //millimeter
        public const double MIN_POS_X = -1300; //millimeter
        public const double MAX_POS_Y = 3.0 / 4 * MAX_POS_X;  //millimeter
        public const double MIN_POS_Y = 3.0 / 4 * MIN_POS_X; //millimeter
     
        public const double MAX_OR_XY = 1;
        public const double MIN_OR_XY = -1;
        public const double OR_LENGTH = 1;
        public const double OR_LENGTH_ACC = 0.0001;


        private PointD position;
        private PointD orientation;//origo koruli egysegvektor

        public static explicit operator double[](CarModelState cs)
        {
            return new double[] { cs.Position.X, cs.Position.Y, cs.Orientation.X, cs.Orientation.Y };
        }

        public CarModelState(double[] arg)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = new PointD(arg[0], arg[1]);
            this.Orientation = new PointD(arg[2], arg[3]);
        }

        public CarModelState(PointD position, double angle)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = position;
            this.Angle = angle;
        }

        public CarModelState(PointD position, PointD orientation)
        {
            this.position = new PointD(0, 0);
            this.orientation = new PointD(1, 0);

            this.Position = position;
            this.Orientation = orientation;
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

              /*  if ((value.X <= MAX_POS_X) && (value.X >= MIN_POS_X) &&
                    (value.Y <= MAX_POS_Y) && (value.Y >= MIN_POS_Y))
                {
                    position = value;
                }
                else
                {
                    position = value;
                    if (value.X > MAX_POS_X) position.X = MAX_POS_X;
                    else if (value.X < MIN_POS_X) position.X = MIN_POS_X;

                    if (value.Y > MAX_POS_Y) position.X = MAX_POS_Y;
                    else if (value.Y < MIN_POS_Y) position.X = MIN_POS_Y;
                }
                */
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

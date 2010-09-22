using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public struct GridCarModelInput
    {
        public const double MAX_SPEED = 100;//135;//   mm/s
        public const double MIN_SPEED = -100;//-135;//   mm/s

        public const double MIN_ANGLE = -1;//R:MAX_SPEED,L:0
        public const double MAX_ANGLE = 1;//R:0,L:MAX_SPEED

        private double leftspeed, rightspeed;

        public static explicit operator double[](GridCarModelInput ci)
        {
            return new double[] { ci.LeftSpeed, ci.RightSpeed };
        }

        public GridCarModelInput(double[] arg)
        {
            this.leftspeed = 0;
            this.rightspeed = 0;
            this.LeftSpeed = arg[0];
            this.RightSpeed = arg[1];
        }

        public GridCarModelInput(double leftspeed, double rightspeed)
        {
            this.leftspeed = 0;
            this.rightspeed = 0;
            this.LeftSpeed = leftspeed;
            this.RightSpeed = rightspeed;
        }

        public GridCarModelInput(double angle)
        {
            this.leftspeed = 0;
            this.rightspeed = 0;
            this.Angle = angle;
        }

        public double LeftSpeed
        {
            get
            {
                return leftspeed;
            }
            set
            {
                if ((value <= MAX_SPEED) && (value >= MIN_SPEED))
                {
                    leftspeed = value;
                }
                else
                {
                    if (value > MAX_SPEED) leftspeed = MAX_SPEED;
                    else leftspeed = MIN_SPEED;
                }// throw new Exception("LeftSpeed out of bounds.");
            }
        }

        public double RightSpeed
        {
            get
            {
                return rightspeed;
            }
            set
            {
                if ((value <= MAX_SPEED) && (value >= MIN_SPEED))
                {
                    rightspeed = value;
                }
                else
                {
                    if (value > MAX_SPEED) rightspeed = MAX_SPEED;
                    else rightspeed = MIN_SPEED;
                }// throw new Exception("RightSpeed out of bounds.");
            }
        }

        public double Angle
        {
            get
            {
                double ang = (leftspeed - rightspeed) / MAX_SPEED;
                if (ang < -1) ang = -1;
                if (ang > 1) ang = 1;
                return ang;
            }
            set
            {
                double val = ComMath.Normal(value, MIN_ANGLE, MAX_ANGLE, -1, 1);
                if (val < -1) val = -1;
                if (val > 1) val = 1;

                if (val < 0)
                {
                    rightspeed = MAX_SPEED;
                    leftspeed = (val + 1) * MAX_SPEED;

                }
                else
                {
                    leftspeed = MAX_SPEED;
                    rightspeed = (1 - val) * MAX_SPEED;
                }
            }
        }
    }
}

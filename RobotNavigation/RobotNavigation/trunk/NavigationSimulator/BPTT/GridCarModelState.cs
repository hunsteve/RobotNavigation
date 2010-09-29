using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public struct GridCarModelState
    {        
        public const double MAX_DIST = 1300;  //millimeter
        public const double MIN_DIST = 0; //millimeter

        public const double MAX_OR_XY = 1;
        public const double MIN_OR_XY = -1;
        public const double OR_LENGTH = 1;
        public const double OR_LENGTH_ACC = 0.0001;


        private double targetDist;//robot-cel tavolsag
        private PointD targetOrientation;//origo koruli egysegvektor, robothoz kepest milyen iranyba van a cel
        private PointD targetFinishOrientation;//origo koruli egysegvektor, robothoz kepest hogy all a cel

        public static explicit operator double[](GridCarModelState cs)
        {
            return new double[] { cs.targetDist, cs.targetOrientation.X, cs.targetOrientation.Y };
        }

        public GridCarModelState(double[] arg)
        {
            this.targetDist = 0;
            this.targetOrientation = new PointD(1, 0);
            this.targetFinishOrientation = new PointD(1, 0);

            this.TargetDist = arg[0];
            this.TargetOrientation = new PointD(arg[1], arg[2]);
            this.TargetFinishOrientation = new PointD(arg[3], arg[4]);
        }

        public GridCarModelState(double dist, double angle, double finishAngle)
        {
            this.targetDist = dist;
            this.targetOrientation = new PointD(1, 0);
            this.targetFinishOrientation = new PointD(1, 0);
      
            this.TargetAngle = angle;
            this.TargetFinishAngle = finishAngle;
        }

        public GridCarModelState(double dist, PointD angle, PointD finishAngle)
        {
            this.targetDist = dist;
            this.targetOrientation = new PointD(1, 0);
            this.targetFinishOrientation = new PointD(1, 0);
            
            this.TargetOrientation = angle;
            this.TargetFinishOrientation = finishAngle;
        }

        public double TargetDist
        {
            get
            {
                return targetDist;
            }
            set
            {

                targetDist = value;

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


        public PointD TargetOrientation
        {
            get
            {
                return targetOrientation;
            }
            set
            {
                this.TargetAngle = Math.Atan2(value.Y, value.X);
            }
        }

        public double TargetAngle
        {
            get
            {
                return Math.Atan2(targetOrientation.Y, targetOrientation.X);
            }
            set
            {
                targetOrientation = new PointD(Math.Cos(value), Math.Sin(value));
            }
        }

        public PointD TargetFinishOrientation
        {
            get
            {
                return targetFinishOrientation;
            }
            set
            {
                this.TargetFinishAngle = Math.Atan2(value.Y, value.X);
            }
        }

        public double TargetFinishAngle
        {
            get
            {
                return Math.Atan2(targetFinishOrientation.Y, targetFinishOrientation.X);
            }
            set
            {
                targetFinishOrientation = new PointD(Math.Cos(value), Math.Sin(value));
            }
        }

    }
}

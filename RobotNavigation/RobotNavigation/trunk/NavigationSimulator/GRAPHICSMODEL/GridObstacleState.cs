using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;

namespace NavigationSimulator
{
    public struct GridObstacleState
    {
        private double obstacleDistance;
        private double obstacleAngle;
        private double radius;        

        public GridObstacleState(double distance, double angle, double radius)
        {
            this.obstacleDistance = distance;
            this.obstacleAngle = angle;
            this.radius = radius;
        }

        public static GridObstacleState FromObstacleState(ObstacleState obst, GridCarModelState state)
        {            
            double d = ComMath.Normal(Math.Sqrt(obst.pp.position.X * obst.pp.position.X + obst.pp.position.Y * obst.pp.position.Y), GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, 1);
            double a = ComMath.Normal(state.TargetDist, GridCarModelState.MIN_DIST, GridCarModelState.MAX_DIST, 0, 1);
            double ang = Math.PI - (Math.Atan2(obst.pp.position.Y, obst.pp.position.X) + Math.PI) + state.TargetAngle - state.TargetFinishAngle;
            if (ang > Math.PI) ang -= 2 * Math.PI;
            if (ang < -Math.PI) ang += 2 * Math.PI;

            double AA = -2 * d * Math.Cos(ang);
            double BB = d * d;
            double obstdist = Math.Sqrt(a * a + BB + AA * a);
            double obstang = state.TargetAngle + Math.Sign(ang) * Math.Acos((a * a + obstdist * obstdist - d * d) / (2 * a * obstdist));

            GridObstacleState gos = new GridObstacleState(obstdist, obstang, obst.radius);
            return gos;
        }

        public double ObstacleDistance
        {
            get
            {
                return obstacleDistance;
            }
        }

        public double ObstacleAngle
        {
            get
            {
                return obstacleAngle;
            }
        }

        public double Radius
        {
            get
            {
                return radius;
            }
        }
    }
}

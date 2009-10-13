using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public class CarModel : IDragable
    {
        public const double SHAFT_LENGTH = 110;  // tengelyhossz, mm
        public const double SIMULATION_TIME_STEP = 1;  // szimulacios lepeskoz: sec
        public const double MM_PER_PIXEL = 4;
        public const double OFFSET_X = -CarModelState.MIN_POS_X / MM_PER_PIXEL;
        public const double OFFSET_Y = -CarModelState.MIN_POS_Y / MM_PER_PIXEL;

        public CarModelState state;
        private PointF[] graphicCarModelWheelLeft;
        private PointF[] graphicCarModelWheelRight;
        private PointF[] graphicCarModelBody;

        int selinside, seloutside;
        const int OUT_WIDTH = 15;

        public CarModel(CarModelState state)
        {
            this.state = state;
            int len = (int)(SHAFT_LENGTH / MM_PER_PIXEL / 2);
            graphicCarModelWheelLeft = new PointF[] { 
                new PointF(-5, -len - 3),
                new PointF(5, -len - 3),
                new PointF(5, -len + 2),
                new PointF(-5, -len + 2)
            };
            graphicCarModelWheelRight = new PointF[] { 
                new PointF(-5, len - 2),
                new PointF(5, len - 2),
                new PointF(5, len + 3),
                new PointF(-5, len + 3)
            };

            graphicCarModelBody = new PointF[] { 
                new PointF(-1, -len + 2),
                new PointF(-len*2, -len + 2),
                new PointF(-len*2, len - 2),
                new PointF(-1, len - 2)
            };           
        }


        public void SimulateModel(CarModelInput input, IModelSimulator simulator)
        {            
            simulator.SimulateModel(this.state, input, out this.state);
        }

        public void SimulateModel(CarModelInput input, IModelSimulator simulator, double timeStep)
        {
            simulator.SimulateModel(this.state, input, timeStep, out this.state);
        }

        public void Render(Graphics g,int alpha,bool realCar)
        {
            PointF[] WheelLeftTransformed = new PointF[graphicCarModelWheelLeft.Length];
            for (int i = 0; i < graphicCarModelWheelLeft.Length; ++i)
            {
                WheelLeftTransformed[i].X = (float)(Math.Cos(state.Angle) * graphicCarModelWheelLeft[i].X - Math.Sin(state.Angle) * graphicCarModelWheelLeft[i].Y + state.Position.X / MM_PER_PIXEL + OFFSET_X);
                WheelLeftTransformed[i].Y = (float)(Math.Sin(state.Angle) * graphicCarModelWheelLeft[i].X + Math.Cos(state.Angle) * graphicCarModelWheelLeft[i].Y + state.Position.Y / MM_PER_PIXEL + OFFSET_Y); 
            }

            PointF[] WheelRightTransformed = new PointF[graphicCarModelWheelRight.Length];
            for (int i = 0; i < graphicCarModelWheelRight.Length; ++i)
            {
                WheelRightTransformed[i].X = (float)(Math.Cos(state.Angle) * graphicCarModelWheelRight[i].X - Math.Sin(state.Angle) * graphicCarModelWheelRight[i].Y + state.Position.X / MM_PER_PIXEL + OFFSET_X);
                WheelRightTransformed[i].Y = (float)(Math.Sin(state.Angle) * graphicCarModelWheelRight[i].X + Math.Cos(state.Angle) * graphicCarModelWheelRight[i].Y + state.Position.Y / MM_PER_PIXEL + OFFSET_Y);
            }

            PointF[] BodyTransformed = new PointF[graphicCarModelBody.Length];
            for (int i = 0; i < graphicCarModelBody.Length; ++i)
            {
                BodyTransformed[i].X = (float)(Math.Cos(state.Angle) * graphicCarModelBody[i].X - Math.Sin(state.Angle) * graphicCarModelBody[i].Y + state.Position.X / MM_PER_PIXEL + OFFSET_X);
                BodyTransformed[i].Y = (float)(Math.Sin(state.Angle) * graphicCarModelBody[i].X + Math.Cos(state.Angle) * graphicCarModelBody[i].Y + state.Position.Y / MM_PER_PIXEL + OFFSET_Y);
            }

            SolidBrush b = new SolidBrush(Color.Gray);
            b.Color = Color.FromArgb(alpha, b.Color);
            g.FillPolygon(b, BodyTransformed);
            b = new SolidBrush(Color.Black);
            b.Color = Color.FromArgb(alpha, b.Color);
            g.FillPolygon(b, WheelLeftTransformed);
            g.FillPolygon(b, WheelRightTransformed);

            if (realCar)
            {
                Pen p = new Pen(Color.Black);
                p.Width = 3;
                p.Color = Color.FromArgb(alpha, p.Color);
                g.DrawPolygon(p, BodyTransformed);
            }
           



            if ((selinside != 0) || (seloutside != 0))
            {
                Pen p = new Pen(Color.Black, 1);
                int x = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r = (int)(SHAFT_LENGTH / MM_PER_PIXEL * 1.41);
                int x2 = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y2 = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r2 = (int)(SHAFT_LENGTH / MM_PER_PIXEL * 1.41) + OUT_WIDTH;

                if (selinside == 1)
                {
                    Pen p2 = new Pen(Color.FromArgb(64, 255, 255, 255), r);
                    g.DrawEllipse(p2, x - r / 2, y - r / 2, r, r);
                }
                if (seloutside == 1)
                {
                    int w = r2 - r;
                    Pen p2 = new Pen(Color.FromArgb(64, 255, 255, 255), w);
                    g.DrawEllipse(p2, x2 - r2 + w / 2, y2 - r2 + w / 2, 2 * r2 - w, 2 * r2 - w);
                }

                g.DrawEllipse(p, x - r, y - r, 2 * r, 2 * r);
                g.DrawEllipse(p, x2 - r2, y2 - r2, 2 * r2, 2 * r2);

                if (selinside == 2)
                {
                    Pen p2 = new Pen(Color.FromArgb(64, 255, 255, 255), r);
                    g.DrawEllipse(p2, x - r / 2, y - r / 2, r, r);
                    p = new Pen(Color.White, 4);
                    g.DrawEllipse(p, x - r, y - r, 2 * r, 2 * r);
                }
                if (seloutside == 2)
                {
                    int w = r2 - r;
                    Pen p2 = new Pen(Color.FromArgb(64, 255, 255, 255), w);
                    g.DrawEllipse(p2, x2 - r2 + w / 2, y2 - r2 + w / 2, 2 * r2 - w, 2 * r2 - w);
                    p = new Pen(Color.White, 4);
                    g.DrawEllipse(p, x2 - r2, y2 - r2, 2 * r2, 2 * r2);
                }
            }
        }

        #region IDragable Members

        public bool PointInInsideArea(Point p)
        {
            int x = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
            int d = (int)(SHAFT_LENGTH / MM_PER_PIXEL * 1.41);
            return ((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y) < d * d);
        }

        public bool PointInOutsideArea(Point p)
        {
            int x = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
            int d = (int)(SHAFT_LENGTH / MM_PER_PIXEL * 1.41);
            int d2 = (x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y);
            return ((d2 < (d + OUT_WIDTH) * (d + OUT_WIDTH)) && (d2 > d * d));
        }

        public void SetPosition(Point p, bool addToRoute)
        {
            state.Position = new PointD((p.X - CarModel.OFFSET_X) * CarModel.MM_PER_PIXEL, (p.Y - CarModel.OFFSET_Y) * CarModel.MM_PER_PIXEL);
        }

        public void SetSecondParameterAgainstPosition(Point p)
        {
            state.Orientation = new PointD((p.X - CarModel.OFFSET_X) * CarModel.MM_PER_PIXEL - state.Position.X, (p.Y - CarModel.OFFSET_Y) * CarModel.MM_PER_PIXEL - state.Position.Y);
        }

        public void SetSelectedState(int inside, int outside)
        {
            selinside = inside;
            seloutside = outside;
        }

        #endregion
    }
}

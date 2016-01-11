using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OnlabNeuralis
{
    public class ObstacleModel : IDragable
    {
        public ObstacleState state;
        int selinside, seloutside;
        const int OUT_WIDTH = 15;


        public ObstacleModel(PointD position, double radius)
        {
            state = new ObstacleState(position, radius);
        }

        public ObstacleModel(Point positionInPixel, double radiusInPixel)
        {
            state = new ObstacleState();
            SetPosition(positionInPixel,false);
            state.radius = radiusInPixel * CarModel.MM_PER_PIXEL;
        }

        public void Render(Graphics g)
        {
            g.FillEllipse(new HatchBrush(HatchStyle.Divot,Color.Black,Color.Blue), (float)((state.pp.position.X - state.radius) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X),
                                                     (float)((state.pp.position.Y - state.radius) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y),
                                                     (float)(2 * state.radius / CarModel.MM_PER_PIXEL),
                                                     (float)(2 * state.radius / CarModel.MM_PER_PIXEL));
            g.DrawEllipse(new Pen(Color.Black, 4), (float)((state.pp.position.X - state.radius) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X + 2),
                                                     (float)((state.pp.position.Y - state.radius) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y + 2),
                                                     (float)(2 * state.radius / CarModel.MM_PER_PIXEL - 4),
                                                     (float)(2 * state.radius / CarModel.MM_PER_PIXEL - 4));

            if ((selinside != 0) || (seloutside != 0))
            {
                Pen p = new Pen(Color.Black, 1);
                int x = (int)(state.pp.position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y = (int)(state.pp.position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r = (int)(state.radius / CarModel.MM_PER_PIXEL);
                int x2 = (int)(state.pp.position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y2 = (int)(state.pp.position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r2 = (int)(state.radius / CarModel.MM_PER_PIXEL) + OUT_WIDTH;

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


                List<PointD> next = state.pp.PredictNextPositions(20);
                List<PointD> prev = state.pp.GetPreviousPositions();
                if (prev != null) foreach (PointD pp in prev)
                {
                    g.DrawEllipse(new Pen(Color.Red, 4), (float)((pp.X - 30) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X + 2),
                                                         (float)((pp.Y - 30) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y + 2),
                                                         (float)(2 * 30 / CarModel.MM_PER_PIXEL - 4),
                                                         (float)(2 * 30 / CarModel.MM_PER_PIXEL - 4));
                }

                if (next != null) foreach (PointD pp in next)
                {
                    g.DrawEllipse(new Pen(Color.Green, 4), (float)((pp.X - 30) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X + 2),
                                                         (float)((pp.Y - 30) / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y + 2),
                                                         (float)(2 * 30 / CarModel.MM_PER_PIXEL - 4),
                                                         (float)(2 * 30 / CarModel.MM_PER_PIXEL - 4));
                } 
            }
        }

        #region IDragable Members

        public bool PointInInsideArea(System.Drawing.Point p)
        {
            int x = (int)(state.pp.position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.pp.position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);

            return ((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y) < state.radius * state.radius / CarModel.MM_PER_PIXEL / CarModel.MM_PER_PIXEL);
        }

        public bool PointInOutsideArea(System.Drawing.Point p)
        {
            int x = (int)(state.pp.position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.pp.position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);

            int d2 = (x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y);

            return ((d2 < (OUT_WIDTH + state.radius / CarModel.MM_PER_PIXEL) * (OUT_WIDTH + state.radius / CarModel.MM_PER_PIXEL)) && (d2 > state.radius * state.radius / CarModel.MM_PER_PIXEL / CarModel.MM_PER_PIXEL));
        }

        public void SetPosition(System.Drawing.Point p, bool addToRoute)
        {
            PointD px = new PointD((p.X - CarModel.OFFSET_X) * CarModel.MM_PER_PIXEL, (p.Y - CarModel.OFFSET_Y) * CarModel.MM_PER_PIXEL);

            if (addToRoute) state.pp.AddNewPosition(px);
            else state.pp.SetPosition(px);
        }

        public void SetSecondParameterAgainstPosition(System.Drawing.Point p)
        {
            int x = (int)(state.pp.position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.pp.position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);

            state.radius = Math.Sqrt((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y)) * CarModel.MM_PER_PIXEL;
        }


        public void SetSelectedState(int inside, int outside)
        {
            selinside = inside;
            seloutside = outside;
        }

        #endregion
    }
}

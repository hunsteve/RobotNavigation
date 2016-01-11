using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OnlabNeuralis
{
    public class FinishModel : IDragable
    {
        public FinishState state;
        private Bitmap finish;
        int selinside, seloutside;

        const int OUT_WIDTH = 15;

        public FinishModel(FinishState state)
        {
            this.state = state;
            finish = NavigationSimulator.Properties.Resources.finish;
        }

        public FinishModel(PointD position, double angle)
        {
            state = new FinishState(position, angle);
            finish = NavigationSimulator.Properties.Resources.finish;
        }
       
        public void Render(Graphics g)
        {
            if (finish != null)
            {
                Matrix m = g.Transform;
                g.TranslateTransform((float)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X), (float)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y));
                g.RotateTransform((float)(state.Angle * 180 / Math.PI + 90));               
                g.DrawImage(finish, new Point((int)(-finish.Width / 1.5), (int)(-finish.Height / 1.5)));
                g.Transform = m;
            }


            if ((selinside != 0) || (seloutside != 0))
            {
                Pen p = new Pen(Color.Black, 1);
                int x = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r = finish.Width / 2;
                int x2 = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
                int y2 = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);
                int r2 = finish.Width / 2 + OUT_WIDTH;                               

                if (selinside == 1)
                {
                    Pen p2 = new Pen(Color.FromArgb(64,255,255,255), r);
                    g.DrawEllipse(p2, x - r/2, y - r/2, r, r);
                }
                if (seloutside == 1)
                {
                    int w = r2-r;
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

            return ((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y) < finish.Width * finish.Width / 4);
        }

        public bool PointInOutsideArea(Point p)
        {
            int x = (int)(state.Position.X / CarModel.MM_PER_PIXEL + CarModel.OFFSET_X);
            int y = (int)(state.Position.Y / CarModel.MM_PER_PIXEL + CarModel.OFFSET_Y);

            int d2 = (x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y);

            return ((d2 < (finish.Width / 2 + OUT_WIDTH) * (finish.Width / 2 + OUT_WIDTH)) && (d2 > finish.Width * finish.Width / 4));
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

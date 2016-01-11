using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MarkerFinderTest
{
    public class CustomDrawListBox : ListBox
    {
        public CustomDrawListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable; // We're using custom drawing.     
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Make sure we're not trying to draw something that isn't there.
            if (e.Index >= this.Items.Count || e.Index <= -1)
                return;
            
            // Get the item object.
            object item = this.Items[e.Index];
            if (item == null)
                return;
            e.Graphics.SetClip(e.Bounds);
            // Draw the background color depending on 
            // if the item is selected or not.
            e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {            
                e.Graphics.DrawRectangle(new Pen(Color.DarkBlue,4), e.Bounds);
            }
                        

            if (typeof(Image).IsAssignableFrom(item.GetType()))
            {
                e.Graphics.DrawImageUnscaled((Image)item, e.Bounds);
            }
            else if (typeof(Shape).IsAssignableFrom(item.GetType()))
            {
                Shape shape = (Shape)item;
                Pen pen = new Pen(Color.Black);
                Matrix m = e.Graphics.Transform;
                e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);
                e.Graphics.TranslateTransform(30, 60);
                Matrix m2 = e.Graphics.Transform;
                e.Graphics.ScaleTransform(0.75f, 0.75f);
                e.Graphics.DrawPolygon(pen, shape.contour.ToArray());
                e.Graphics.Transform = m2;
                e.Graphics.TranslateTransform(60, 0);

                int sd = 2;
                int dx = 3;

                e.Graphics.DrawLine(new Pen(Color.Blue), 0, -20, 32 * dx, -20);
                e.Graphics.DrawLine(new Pen(Color.Blue), 0, 20, 32 * dx, 20);             

                Point p = shape.contour[0];
                int x = 0;
                
                foreach (Point p2 in shape.contour)
                {
                    Point pxa = new Point(x, p.X / sd - 20);
                    Point pxb = new Point(x + dx, p2.X / sd - 20);
                    Point pya = new Point(x, p.Y / sd + 20);
                    Point pyb = new Point(x + dx, p2.Y / sd + 20);
                    e.Graphics.DrawLine(pen, pxa, pxb);
                    e.Graphics.DrawLine(pen, pya, pyb);
                    p = p2;
                    x += dx;
                }

                e.Graphics.TranslateTransform(32 * dx + 20, -60);
                e.Graphics.DrawString("Pos: (" + shape.pos.X + ", " + shape.pos.Y + ")", this.Font, new SolidBrush(Color.Black), 0, 10);
                e.Graphics.DrawString("Rot: " + String.Format("{0:0.##}", shape.rot), this.Font, new SolidBrush(Color.Black), 0, 30);
                e.Graphics.DrawString("Scale: " + String.Format("{0:0.##}", shape.scale), this.Font, new SolidBrush(Color.Black), 0, 50);
                e.Graphics.DrawString("Marker: " + shape.index, this.Font, new SolidBrush(Color.Black), 0, 70);


                e.Graphics.Transform = m;
            }
            else
            {
                // Draw the item.
                string text = item.ToString();
                SizeF stringSize = e.Graphics.MeasureString(text, this.Font);
                e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Black),
                    new PointF(5, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MarkerDesigner
{
    public delegate void EventHandlerDiagramChanged(object sender);

    public partial class DrawDiagram : Control
    {
        private Point[] mP;
        public bool mousedown;        

        public event EventHandlerDiagramChanged DiagramChanged;

        public DrawDiagram()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            mP = new Point[32];            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            if (this.Enabled) g.Clear(Color.White);
            else g.Clear(Color.FromArgb(220,220,220));

            g.DrawLine(new Pen(Color.Blue), 0, this.Height * 0.25f, this.Width, this.Height * 0.25f);
            g.DrawLine(new Pen(Color.Blue), 0, this.Height * 0.75f, this.Width, this.Height * 0.75f);
            g.DrawLine(new Pen(Color.Black,3), 0, this.Height * 0.5f, this.Width, this.Height * 0.5f);
            Pen p = new Pen(Color.Black);            
            for (int i = 1; i < mP.Length; i++)
            {
                g.DrawLine(p, (i - 1) * this.Width / (mP.Length - 1), this.Height * 0.25f + mP[i - 1].X * 0.75f, i * this.Width / (mP.Length - 1), this.Height * 0.25f + mP[i].X * 0.75f);
                g.DrawLine(p, (i - 1) * this.Width / (mP.Length - 1), this.Height * 0.75f + mP[i - 1].Y * 0.75f, i * this.Width / (mP.Length - 1), this.Height * 0.75f + mP[i].Y * 0.75f);
            }
        }

        public Point[] PointList { 
            get {
                return mP;
            } 
            set{
                if (value != null)
                {
                    mP = value;
                    this.Invalidate();
                }
                else {
                    mP = new Point[32];
                    this.Invalidate();
                }
            } 
        }

        private void DrawDiagram_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousedown = true;
                AdjustDiagram(e.Location);
            }
        }

        private void DrawDiagram_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mousedown = false;
        }

        private void DrawDiagram_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                AdjustDiagram(e.Location);
            }
        }


        private void AdjustDiagram(Point p)
        {
            int i = (int)(p.X * mP.Length / this.Width);
            if (i < 0) i = 0;
            if (i >= mP.Length) i = mP.Length - 1;

            if (p.Y < this.Height * 0.5f)
            {
                mP[i].X = (int)((p.Y - this.Height * 0.25f) / 0.75f);
            }
            else
            {
                mP[i].Y = (int)((p.Y - this.Height * 0.75f) / 0.75f);
            }
            this.Invalidate();
            DiagramChanged(this);
        }
    }
}

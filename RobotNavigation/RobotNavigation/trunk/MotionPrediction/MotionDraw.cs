using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NNImage;
using System.Threading;

namespace DiplomaMunka
{
    public partial class MotionDraw : Control
    {
        private const int D_MIN = 100;       
        private bool down;
        private List<Point> list;
        private List<Point> predicted;

        MotionPredictor mpm;

        bool running;

        Thread t;

        public MotionDraw()
        {
            InitializeComponent();
            down = false;
            list = new List<Point>();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);           
        }

        public void StartThread()
        {
            t = new Thread(new ThreadStart(trainRun));
            running = true;
            t.Start();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.Clear(Color.White);

            foreach (Point p in list)
            {
                g.FillEllipse(new SolidBrush(Color.Red), p.X - 5, p.Y - 5, 11, 11);
            }
           

            if (predicted != null)
            {               
                foreach (Point p in predicted)
                {
                    g.FillEllipse(new SolidBrush(Color.Blue), p.X - 5, p.Y - 5, 11, 11);
                }               
            }
            
        }


        private void MotionDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                down = true;              
                list.Clear();                
                this.Invalidate();
                mpm = new MotionPredictor(45, 15);
            }
        }

        private void MotionDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                down = false;                
            }
        }

        private void MotionDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                if ((list.Count == 0) || ((e.X - list.Last().X) * (e.X - list.Last().X) + (e.Y - list.Last().Y) * (e.Y - list.Last().Y) > D_MIN))
                {
                    list.Add(e.Location);
                    
                    mpm.AddPoint(e.Location);
                    
                    this.Invalidate();
                }
            }
        }

        void trainRun()
        {
            while (running)
            {
                if (mpm != null)
                {
                    
                    double xx = mpm.Train();    
                                            
                    predicted = mpm.PredictNextPoints(30);
               
                    this.Invalidate();
                }                                        
            }
        }

        internal void StopThread()
        {
            running = false;
        }
    }
}

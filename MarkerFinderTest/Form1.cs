using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using MMCar_Finder;
using System.Threading;

namespace MarkerFinderTest
{

    public partial class Form1 : Form
    {

        Camera camera;
        MyMarkerFinder mf;
        List<Bitmap> markerlist;
        Bitmap back;
        bool docopy;
        System.Windows.Forms.Timer tim;

        public Form1()
        {
            InitializeComponent();
        
            camera = new Camera(pictureBox1);
            pictureBox1.Image = new Bitmap("cam.png"); 
            back = new Bitmap("back.png");
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = back;

         
            markerlist = new List<Bitmap>();
            markerlist.Add(new Bitmap("marker1.png"));
            markerlist.Add(new Bitmap("marker2.png"));
            markerlist.Add(new Bitmap("marker3.png"));
            markerlist.Add(new Bitmap("marker4.png"));
            markerlist.Add(new Bitmap("marker5.png"));
           
            mf = new MyMarkerFinder(markerlist, back);

            docopy = true;
            run();
            
        }

        void t_Tick(object sender, EventArgs e)
        {
            run();
        }

        
        void run()
        {
           
            Bitmap foreground = new Bitmap(back.Width, back.Height);
            Graphics g = Graphics.FromImage(foreground);
            g.Clear(Color.White);

            DateTime x = DateTime.Now;


            Bitmap cam = camera.getBitmap();
            if (cam == null) cam = new Bitmap("cam.png");                         
            List<Shape> shapes = null;

            shapes = mf.ProcessFrame(cam);

           
            if (docopy)
            {
                customDrawListBox1.Items.Clear();
                customDrawListBox1.Items.AddRange(shapes.ToArray());
                docopy = false;
            }

            foreach (Shape shape in shapes)
            {
                Matrix m = g.Transform;
                g.TranslateTransform(shape.pos.X, shape.pos.Y);
                g.RotateTransform((float)(-shape.rot * 180 / Math.PI));
                g.ScaleTransform((float)(shape.scale * 0.75), (float)(shape.scale * 0.75));
                if (shape.index >= 0)
                {
                    Bitmap bm = markerlist[shape.index];
                    g.DrawImage(bm, -bm.Width * 2 / 3, -bm.Height * 2 / 3);
                }

                g.Transform = m;
                g.TranslateTransform(shape.pos.X, shape.pos.Y);
                g.RotateTransform((float)(-shape.rot * 180 / Math.PI));
                g.ScaleTransform((float)(shape.scale * 1.5), (float)(shape.scale * 1.5));

                g.DrawPolygon(new Pen(Color.Red), shape.contour.ToArray());

                g.Transform = m;
            }
            
            
            DateTime y = DateTime.Now;
            Console.Out.WriteLine(y - x);

            pictureBox3.Image = foreground;

            Invalidate();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap back2 = camera.getBitmap();
            if (back2 != null)
            {
                back = back2;
                pictureBox2.Image = back;
                mf = new MyMarkerFinder(markerlist, back);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            docopy = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (tim != null)
            {
                button3.Text = "Start processing cam";
                tim.Enabled = false;
                tim = null;
            }
            else
            {
                button3.Text = "Stop processing cam";
                tim = new System.Windows.Forms.Timer();
                tim.Interval = 50;
                tim.Tick += new EventHandler(t_Tick);
                tim.Start();
            }

             
        }

       

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MarkerFinderTest;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace MarkerFinderClassifier
{
    public partial class Form1 : Form
    {
        MyMarkerFinder mf;
        Bitmap background;
        Bitmap marker1;
        Random rand;
        Bitmap noise;
        FileStream f;
        Bitmap contour;

        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            marker1 = new Bitmap(Image.FromFile("marker1.png"));            
            background = new Bitmap(Image.FromFile("back.png"));
            noise = new Bitmap(Image.FromFile("noise.png"));
            contour = new Bitmap(100, 100);
            pictureBox2.Image = contour;
            mf = new MyMarkerFinder(new List<Bitmap>() { marker1 }, background);

            f = new FileStream("positivesamples.txt",FileMode.Append);            
            timer1.Enabled = true;                     
        }

        private Bitmap GenerateImage()
        {
            Bitmap frame = new Bitmap(320, 240);
            Graphics g = Graphics.FromImage(frame);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(background, new Point(0, 0));
            Matrix matrix = g.Transform;

            //transzformacio
            g.TranslateTransform((float)rand.NextDouble() * 280 + 20, (float)rand.NextDouble() * 200 + 20);
            float scale = 0.2f + (float)rand.NextDouble() * 0.1f;
            g.ScaleTransform(scale, scale + (float)rand.NextDouble() * 0.01f - 0.005f);
            g.RotateTransform((float)rand.NextDouble() * 360);

            g.FillRectangle(new SolidBrush(Color.White), -50, -70, 100, 140);
            g.DrawImage(marker1, new Point(-50, -50));

            g.Transform = matrix;
            //zaj hozzaadasa
            g.DrawImage(noise, new Point(0, 0));            
            g.Flush();
            MemoryStream ms = new MemoryStream();
            frame.Save(ms, ImageFormat.Jpeg);
            frame = new Bitmap(ms);
            return frame;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap frame = GenerateImage();
            pictureBox1.Image = frame;
            List<Shape> shapes = mf.ProcessFrame(frame);

            Graphics g = Graphics.FromImage(frame);
            foreach (Shape shape in shapes)
            {
                Matrix m = g.Transform;
                g.TranslateTransform(shape.pos.X, shape.pos.Y);
                g.RotateTransform((float)(-shape.rot * 180 / Math.PI));
                g.ScaleTransform((float)(shape.scale * 0.75), (float)(shape.scale * 0.75));
                g.Transform = m;
                g.TranslateTransform(shape.pos.X, shape.pos.Y);
                g.RotateTransform((float)(-shape.rot * 180 / Math.PI));
                g.ScaleTransform((float)(shape.scale * 1.5), (float)(shape.scale * 1.5));

                g.DrawPolygon(new Pen(Color.Red), shape.contour.ToArray());

                g.Transform = m;
            }
            g.Flush();
            g = Graphics.FromImage(contour);
            g.TranslateTransform(50, 50);
            g.DrawPolygon(new Pen(Color.Red), shapes[0].contour.ToArray());
            g.Flush();
            pictureBox2.Image = contour;

            this.Invalidate();
            StringBuilder line = new StringBuilder();
            foreach (Point p in shapes[0].contour)
            {
                line.Append(p.X);
                line.Append(",");
                line.Append(p.Y);
                line.Append(",");
            }
            line.Append("\r\n");
            byte[] data = Encoding.ASCII.GetBytes(line.ToString());
            f.Write(data, 0, data.Length);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.Flush();
            f.Close();
        }
    }
}

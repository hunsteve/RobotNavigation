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
using System.Collections;

namespace MarkerDesigner
{

    public class Shape
    {
        public Point pos;
        public double scale;
        public double rot;
        public int index;
        public List<Point> contour;

        public Shape(Point pos, double scale, double rot, int index)
        {
            this.pos = pos;
            this.scale = scale;
            this.rot = rot;
            this.index = index;
            this.contour = new List<Point>();
        }
    }

    public delegate void EventHandlerImageChanged(object sender);

    public partial class DrawBoard : Control
    {
        public Bitmap im;
        public float brushsize;
        public bool pen;

        private Point lastPoint;

        public bool mousedown;
        
        public event EventHandlerImageChanged ImageChanged;

        public DrawBoard()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);                        
            mousedown = false;
            pen = true;
            brushsize = 5;
            im = new Bitmap(100,100);
            Clear();
            lastPoint.X = int.MinValue;            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            if (im != null) g.DrawImage(im, new Rectangle(0, 0, this.Width, this.Height));
        }

        private void DrawBoard_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                mousedown = true;
                pen = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                mousedown = true;
                pen = false;
            }
            DrawBrush(e.Location);                        
        }

        public void Clear()
        {
            Graphics g = Graphics.FromImage(im);
            g.Clear(Color.White);
            this.Invalidate();
            if (ImageChanged != null) ImageChanged(this);
        }

        public void DrawBrush(Point point)
        {            
            Graphics g = Graphics.FromImage(im);
            Color c;
            if (pen) c = Color.Black;
            else c = Color.White;
            g.FillEllipse(new SolidBrush(c), point.X * im.Width / this.Width - brushsize / 2, point.Y * im.Height / this.Height - brushsize / 2, brushsize, brushsize);
            if (lastPoint.X >= 0) g.DrawLine(new Pen(c, brushsize), point.X * im.Width / this.Width, point.Y * im.Height / this.Height,
                                                                   lastPoint.X * im.Width / this.Width, lastPoint.Y * im.Height / this.Height);                
            lastPoint = point;
            this.Invalidate();
            if (ImageChanged != null) ImageChanged(this);
        }

        private void DrawBoard_MouseUp(object sender, MouseEventArgs e)
        {            
            mousedown = false;
            lastPoint.X = int.MinValue;
        }

        private void DrawBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                DrawBrush(e.Location);                
            }
        }


        public void Open(string p)
        {
            Bitmap im2 = new Bitmap(p);
            Clear();
            Graphics g = Graphics.FromImage(im);
            g.DrawImage(im2, new Rectangle(0,0,im.Width,im.Height));
            this.Invalidate();
        }

        public void Save(string p)
        {
            im.Save(p);
        }


        #region(private structs)
        private unsafe struct MyImage
        {
            public uint* img;
            public int w;
            public int h;
            public MyImage(uint* aImg, int aW, int aH)
            {
                img = aImg;
                w = aW;
                h = aH;
            }
        }
        private unsafe struct MyImageListElement
        {
            public MyImage image;
            public MyImageListElement* next;
        }
        private unsafe struct MyContourPoint
        {
            public MyPoint pos;
            public MyContourPoint* next;
        }
        private unsafe struct MyShape
        {
            public MyContourPoint* contour;
            public MyPoint pos;
            public double scale;
            public double rot;
            public int index;
        }
        private unsafe struct MyShapeListElement
        {
            public MyShape shape;
            public MyShapeListElement* next;
        }
        private unsafe struct MyPoint
        {
            public int X;
            public int Y;
        }
        private unsafe struct MyComplexData
        {
            public double* data;
            public int length;
        }
        #endregion



        public Shape GetContour()
        {       
            BitmapData data = im.LockBits(new Rectangle(0, 0, im.Width, im.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Shape shape = null;
            unsafe
            {
                MyImage img = new MyImage((uint*)data.Scan0.ToPointer(), im.Width, im.Height);            
                MyImage img2;
                clone_image_N(img, &img2);
                binarize2(img2, true);
                closing(img2);
                MyShapeListElement* shapeList = null;
                real_contour_N(img2, &shapeList);                
                if ((shapeList != null) && (shapeList->next != null))
                {                    
                    normalize_shape(&(shapeList->shape));
                    MyShape current = shapeList->shape;
                    shape = new Shape(new Point(current.pos.X, current.pos.Y), current.scale, current.rot, current.index);
                    MyContourPoint* curp = current.contour;
                    while (curp->next != null)
                    {
                        shape.contour.Add(new Point(curp->pos.X, curp->pos.Y));
                        curp = curp->next;
                    }
                }

                delete_shape_list_D(shapeList);
                delete_image_D(img2);
            }
            im.UnlockBits(data);

            if (shape != null)
            {                
                return shape;
            }
            else return null;            
        }

        public void DrawPolygon(Point[] pointlist, Point pos, double scale, double rot)
        {
            Graphics g = Graphics.FromImage(im);
            g.Clear(Color.White);
            g.TranslateTransform(pos.X, pos.Y);
            if (scale > 0) g.ScaleTransform((float)scale, (float)scale);
            g.RotateTransform((float)(-rot * 180 / Math.PI));
            g.FillPolygon(new SolidBrush(Color.Black), pointlist);
            this.Invalidate();
        }

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void clone_image_N(MyImage src, MyImage* dest);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void delete_image_D(MyImage im);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void binarize2(MyImage im, bool inverse);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void real_contour_N(MyImage im, MyShapeListElement** shapeList);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void delete_shape_list_D(MyShapeListElement* shapeList);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void normalize_shape(MyShape* shape);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void writeImage(char[] filename, MyImage im);
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void closing(MyImage im);




    }
}

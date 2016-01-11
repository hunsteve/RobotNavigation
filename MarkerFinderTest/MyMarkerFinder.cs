using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace MarkerFinderTest
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


    public class MyMarkerFinder: IDisposable
    {
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
        
        public Bitmap background;

        public MyMarkerFinder(List<Bitmap> markers, Bitmap background)
        {
            unsafe
            {
                createMarkerList(markers);
                this.background = background;                
            }
        }


        private void createMarkerList(List<Bitmap> images)
        {
            unsafe
            {
                MyImageListElement* list;
                create_image_list_N(&list);
                Dictionary<Bitmap, BitmapData> bmdlist = new Dictionary<Bitmap, BitmapData>();
                foreach (Bitmap im in images)
                {
                    BitmapData bmd = im.LockBits(new Rectangle(0, 0, im.Width, im.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    bmdlist.Add(im, bmd);
                    MyImage mimg = new MyImage((uint*)bmd.Scan0.ToPointer(), im.Width, im.Height);
                    add_image_list_item_N(&list, mimg);
                }
                
                generate_marker_shapes_N(list);
            

                foreach (KeyValuePair<Bitmap, BitmapData> pair in bmdlist)
                {
                    pair.Key.UnlockBits(pair.Value);
                }

                delete_image_list_D(list);
            }
        }
        private unsafe MyShapeListElement* processFrame(Bitmap cam, Bitmap back)
        {
            BitmapData backData = back.LockBits(new Rectangle(0, 0, back.Width, back.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData camData = cam.LockBits(new Rectangle(0, 0, cam.Width, cam.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            MyImage afg = new MyImage((uint*)camData.Scan0.ToPointer(), cam.Width, cam.Height);
            MyImage abg = new MyImage((uint*)backData.Scan0.ToPointer(), back.Width, back.Height);
            
            MyShapeListElement* shapeList;
            find_objects_N(afg, abg, &shapeList);

            cam.UnlockBits(camData);
            back.UnlockBits(backData);

            return shapeList;
        }

        public List<Shape> ProcessFrame(Bitmap cam)
        {
            unsafe
            {
                MyShapeListElement* shapes = processFrame(cam, this.background);

                List<Shape> ret = new List<Shape>();
                
                MyShapeListElement* current = shapes;
                while (current->next != null)
                {
                    ret.Add(new Shape(new Point(current->shape.pos.X, current->shape.pos.Y), current->shape.scale, current->shape.rot, current->shape.index));
                    MyContourPoint* curp = current->shape.contour;
                    while (curp->next != null)
                    {
                        ret.Last().contour.Add(new Point(curp->pos.X, curp->pos.Y));
                        curp = curp->next;
                    }
                    current = current->next;
                }

                delete_shape_list_D(shapes);
                
                return ret;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        // Use C# destructor syntax for finalization code.
        ~MyMarkerFinder()
        {           
            Dispose(false);
        }

        #endregion


        #region("p/invokes from dll")

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void delete_shape_list_D(MyShapeListElement* shapeList);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void delete_image_list_D(MyImageListElement* list);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void create_image_list_N(MyImageListElement** element);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void add_image_list_item_N(MyImageListElement** element, MyImage image);
       
        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void find_objects_N(MyImage foreground, MyImage background, MyShapeListElement** shapeList);       

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void generate_marker_shapes_N(MyImageListElement* markers);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void binarize2(MyImage im, bool inverse);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void binarize(MyImage im, uint color0, uint color1);

        [DllImport("MarkerFinderLib.dll")]
        private static extern unsafe void segment_kmeans(MyImage im, int levels);

        #endregion
    }
}

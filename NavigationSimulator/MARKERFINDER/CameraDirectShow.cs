using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace NavigationSimulator
{
    public delegate void OnNewFrameDelegate(Bitmap frame);
    class CameraDirectShow
    {
        Thread t;
        public bool running;
        Bitmap bitmap;

        public event OnNewFrameDelegate OnNewFrame;

        public CameraDirectShow()
        {
        }

        public void Start()
        {
            t = new Thread(new ThreadStart(run));
            t.IsBackground = true;
            running = true;
            t.Start();
        }

        public void run()
        {
            try
            {
                using (CaptureDeviceDirectShow cam = new CaptureDeviceDirectShow())
                {
                        cam.Start();
                        while (running)
                        {
                            
                            IntPtr ip = cam.GetBitMap();
                            Bitmap bm = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, ip);
                            bm.RotateFlip(RotateFlipType.RotateNoneFlipY);

                            lock (bm)
                            {
                                if (OnNewFrame != null) OnNewFrame.Invoke(bm);
                                bitmap = bm;
                            }
                        }   
                }
            }
            catch (Exception ex) { running = false;}
        }

        public void Stop()
        {
            running = false;
            if (!t.Join(150))
            {
                t.Abort();
            }
        }

        public Bitmap GetBitmap()
        {
            if (bitmap != null)
            {
                lock (bitmap)
                {
                    return new Bitmap(bitmap);
                }
            }
            else return null;
        }
    }
}

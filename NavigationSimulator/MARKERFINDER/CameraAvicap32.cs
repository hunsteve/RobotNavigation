﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MMCar_Finder;

namespace MarkerFinderTest
{
    class CameraAvicap32
    {
        List<CaptureDeviceAvicap32> devices;
        CaptureDeviceAvicap32 activeDevice;

        public CameraAvicap32(PictureBox pb)
        {
            devices = CaptureDeviceAvicap32.GetDevices();
            if (devices.Count > 0)
            {
                activeDevice = devices[0];
                activeDevice.Attach(pb);
            }
        }

        public Bitmap getBitmap()
        {
            if (activeDevice != null)
            {
                Image camImage = activeDevice.Capture();
                if (camImage != null)
                {
                    Bitmap cam = new Bitmap(camImage.Width, camImage.Height);
                    Graphics g2 = Graphics.FromImage(cam);
                    g2.DrawImage(camImage, new Rectangle(0, 0, cam.Width - 1, cam.Height - 1), new Rectangle(0, 0, camImage.Width - 1, camImage.Height - 1), GraphicsUnit.Pixel);
                    return cam;
                }
                else return null;    
            }
            else return null;
        }
    }
}

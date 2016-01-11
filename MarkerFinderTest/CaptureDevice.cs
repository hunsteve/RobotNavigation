using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace MMCar_Finder
{
    /// <summary>
    /// This class represents a device that is capable of capturing audio and video
    /// </summary>
    public class CaptureDevice
    {
        private static int MAX_DEVICES = 10;

        private ushort deviceNumber;
        private string name;
        private string description;
        private IntPtr deviceHandle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceNumber">the number</param>
        /// <param name="name">the name</param>
        /// <param name="description">the description</param>
        private CaptureDevice(ushort deviceNumber, string name, string description)
        {
            this.deviceNumber = deviceNumber;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Setter and Getter for the Device Number
        /// </summary>
        public ushort DeviceNumber
        {
            get { return deviceNumber; }
            set { deviceNumber = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Displays a dialog box in which the user can select the video format
        /// </summary>
        /// <param name="control">the control</param>
        public void Dialog() 
        {
            if (deviceHandle != null) 
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_DLG_VIDEOFORMAT, (IntPtr)0, (IntPtr)0);
            }
        }

        /// <summary>
        /// Attaches the preview stream to the given control
        /// </summary>
        /// <param name="control">the control</param>
        public void Attach(System.Windows.Forms.Control control)
        {
            deviceHandle = Avicap32.capCreateCaptureWindow("", Constants.WS_VISIBLE | Constants.WS_CHILD, 0, 0, control.Width, control.Height, control.Handle, 0);

            if (User32.SendMessage(deviceHandle, Constants.WM_CAP_DRIVER_CONNECT, (IntPtr)deviceNumber, (IntPtr)0).ToInt32() > 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_SCALE, (IntPtr)(-1), (IntPtr)0);
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_PREVIEWRATE, (IntPtr)0x42, (IntPtr)0);
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_PREVIEW, (IntPtr)(-1), (IntPtr)0);
                User32.SetWindowPos(deviceHandle, new IntPtr(0), 0, 0, control.Width, control.Height, 6);
            }
        }

        /// <summary>
        /// Detaches from the control
        /// </summary>
        public void Detach()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_DRIVER_DISCONNECT, (IntPtr)deviceNumber, (IntPtr)0);
                User32.DestroyWindow(deviceHandle);
            }
            deviceHandle = new IntPtr(0);

        }

        /// <summary>
        /// Returns a captured image
        /// </summary>
        /// <returns>an image, null if capture failed</returns>
        public Image Capture()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_EDIT_COPY, (IntPtr)0, (IntPtr)0);
                IDataObject ido = Clipboard.GetDataObject();
                if (ido.GetDataPresent(DataFormats.Bitmap))
                {
                    return ((Bitmap)ido.GetData(DataFormats.Bitmap));
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an array with available capture devices
        /// </summary>
        /// <returns>the device names</returns>
        public static List<CaptureDevice> GetDevices()
        {
            List<CaptureDevice> devices = new List<CaptureDevice>();

            for (ushort i = 0; i < MAX_DEVICES; ++i)
            {
                int capacity = 200;
                StringBuilder name = new StringBuilder(capacity);
                StringBuilder description = new StringBuilder(capacity);

                if (Avicap32.capGetDriverDescription(i, name, capacity, description, capacity).ToInt32() > 0)
                {
                    devices.Add(new CaptureDevice(i, name.ToString(), description.ToString()));
                }
            }

            return devices;
        }
    }
}

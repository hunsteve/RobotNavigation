using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace OnlabNeuralis
{
    class SerialComm
    {
        SerialPort serial;
        TextBox textbox;
        public StringBuilder readText;
        bool can_read;
        bool[] acknowledge_mode;        
        byte lastCommandByte;

        public bool lastCommandReturned;
        public bool LastCommandReturned 
        { 
            get
            {
                return lastCommandReturned;
            } 
        }

        public const byte MOTOR_OFORWARD = 0x64; //left speed right speed 
        public const byte MOTOR_CFORWARD = 0x67; //left speed right speed left distance right distance 
        public const byte MOTOR_LFORWARD = 0x66; //speed distance 
        public const byte MOTOR_RFORWARD = 0x65; //speed distance 
        public const byte MOTOR_STOP = 0x63; //
        public const byte MOTOR_LSTOP = 0x62; //
        public const byte MOTOR_RSTOP = 0x61; //- 
        public const byte MOTOR_GET_DIST = 0x71; //left distance right distance 
        public const byte MOTOR_SET_HEAD_ANGLE = 0x68; //angle 
        public const byte MOTOR_GET_HEAD_ANGLE = 0x78;//angle 
        public const byte DISP_SET = 0x41; //display 1 display 2 display 3 4 LED 
        public const byte DISP_DLS = 0x51; //display 1 display 2 display 3 LED & Switch 
        public const byte DISP_BUTTONS = 0x52; //buttons 
        public const byte DISP_TEMP = 0x53; //temperature 
        public const byte SENSOR_COMP_LED_ON = 0x91; //threshold level status of 13 sensors 
        public const byte SENSOR_COMP_LED_OFF = 0x92; //threshold level status of 13 sensors 
        public const byte SENSOR_LEVEL_LED_ON = 0x93; //sensor ID sensor level 
        public const byte SENSOR_LEVEL_LED_OFF = 0x94; //sensor ID sensor level 
        public const byte SENSOR_DIFF_MES = 0x95; //sensor ID sensor level

        public const byte MOTOR_I2C_FORWARD = 0x69; //i2c closed control
        public const byte MOTOR_I2C_SET_PID_PARAMS = 0x6A;
        public const byte MOTOR_I2C_GET_PID_PARAMS = 0x6B;

        public const byte COMM_SET_ACKNOWLEDGE_MODE = 0x20;

        public SerialComm(string portname, TextBox textbox)
        {
            acknowledge_mode = new bool[16];
            for (int i = 0; i < acknowledge_mode.Length; ++i) acknowledge_mode[i] = false;

            serial = new SerialPort(portname,38400,Parity.None,8,StopBits.One);
            serial.Open();
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);

            can_read = true;
            this.textbox = textbox;
            readText = new StringBuilder();            
        }


        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (can_read)
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                string sss = serial.ReadExisting();
                byte[] bytes = encoding.GetBytes(sss);                               
                readText.Append(sss);             
            }
        }

        public void SendCommand(byte[] buf, byte address, bool waitForAnswer)
        {
            SendCommand(buf, address, waitForAnswer, 0);
        }

        public void SendCommand(byte[] buf, byte address, bool waitForAnswer, int bytesToRead)
        {
            lastCommandByte = buf[0];
            lastCommandReturned = false;
            byte header = (byte)(((buf.Length - 1) % 8) + (waitForAnswer ? 1 : 0) * 8 + (address % 16) * 16);
            byte[] buf2 = new byte[buf.Length + 1];
            buf2[0] = header;
            for (int i = 1; i < buf2.Length; ++i)
            {
                buf2[i] = buf[i - 1];
            }
            while (serial.BytesToWrite > 0) { Thread.Sleep(1); }
            can_read = false;
            serial.Write(buf2,0,buf2.Length);
            textbox.AppendText(ByteArrayToHexString(buf2) + "\r\n");

            if (acknowledge_mode[address] && !waitForAnswer)
            {
                int i = 0;
                while ((serial.BytesToRead < 2) && (i < 1000))
                {
                    Thread.Sleep(10);
                    i += 1;
                }
                byte[] bb = new byte[2];
                serial.Read(bb, 0, 2);
                if (bb[1] != lastCommandByte)
                {
                    throw new Exception("Invalid or no acknowledge!");
                }
                else
                {
                    lastCommandReturned = true;
                }
            }

            if (waitForAnswer)
            {
                int i = 0; 
                while ((serial.BytesToRead < bytesToRead) && (i < 100)) 
                { 
                    Thread.Sleep(10); 
                    i += 1; 
                }
                lastCommandReturned = true;
            }
            can_read = true;
        }

        private static string ByteArrayToHexString(byte[] buf)
        {
            string s = "";
            foreach (byte b in buf)
            {
                s += String.Format("{0:x}", b) + " ";
            }
            return s;
        }

        public void Motor_OForward(byte address, byte leftSpeed, byte rightSpeed)
        {
            SendCommand(new byte[] { MOTOR_OFORWARD, leftSpeed, rightSpeed }, address, false);
        }

        public void Motor_CForward(byte address, byte leftSpeed, byte rightSpeed)
        {
            SendCommand(new byte[] { MOTOR_CFORWARD, leftSpeed, rightSpeed }, address, false);
        }

        public void Motor_CForward(byte address, byte leftSpeed, byte rightSpeed, short leftDistance, short rightDistance)
        {
            SendCommand(new byte[] { MOTOR_CFORWARD, leftSpeed, rightSpeed, (byte)(leftDistance/256), (byte)(leftDistance%256), (byte)(rightDistance/256), (byte)(rightDistance%256) }, address, false);
        }

        public void Motor_LForward(byte address, byte leftSpeed, short leftDistance)
        {
            SendCommand(new byte[] { MOTOR_LFORWARD, leftSpeed, (byte)(leftDistance / 256), (byte)(leftDistance % 256) }, address, false);
        }

        public void Motor_LForward(byte address, byte leftSpeed)
        {
            SendCommand(new byte[] { MOTOR_LFORWARD, leftSpeed}, address, false);
        }

        public void Motor_RForward(byte address, byte rightSpeed, short rightDistance)
        {
            SendCommand(new byte[] { MOTOR_RFORWARD, rightSpeed, (byte)(rightDistance / 256), (byte)(rightDistance % 256) }, address, false);
        }

        public void Motor_RForward(byte address, byte rightSpeed)
        {
            SendCommand(new byte[] { MOTOR_RFORWARD, rightSpeed }, address, false);
        }

        public void Motor_Stop(byte address)
        {
            SendCommand(new byte[] { MOTOR_STOP }, address, false);
        }

        public void Motor_I2C_Forward(byte address, byte leftSpeed, byte rightSpeed)
        {
            SendCommand(new byte[] { MOTOR_I2C_FORWARD, leftSpeed, rightSpeed }, address, false);
        }

        public void Motor_I2C_Forward(byte address, byte leftSpeed, byte rightSpeed, short leftDistance, short rightDistance)
        {
            SendCommand(new byte[] { MOTOR_I2C_FORWARD, leftSpeed, rightSpeed, (byte)(leftDistance/256), (byte)(leftDistance%256), (byte)(rightDistance/256), (byte)(rightDistance%256) }, address, false);
        }

        public void Motor_I2C_Set_Kp(byte address, float Kp)
        {
            byte code = 0;
            byte[] bb = BitConverter.GetBytes(Kp);
            SendCommand(new byte[] { MOTOR_I2C_SET_PID_PARAMS, code, bb[0], bb[1], bb[2], bb[3] }, address, false);
        }

        public float Motor_I2C_Get_Kp(byte address)
        {           
            byte code = 0;
            SendCommand(new byte[] { MOTOR_I2C_GET_PID_PARAMS, code}, address, true, 7);                        
            byte[] bb = new byte[7];            
            serial.Read(bb, 0, 7);
            if ((bb[1] == MOTOR_I2C_GET_PID_PARAMS) && (bb[2] == code)) return BitConverter.ToSingle(bb, 3);            
            else return float.NaN;            
        }

        public void Motor_I2C_Set_Ki(byte address, float Ki)
        {
            byte code = 1;
            byte[] bb = BitConverter.GetBytes(Ki);
            SendCommand(new byte[] { MOTOR_I2C_SET_PID_PARAMS, code, bb[0], bb[1], bb[2], bb[3] }, address, false);
        }

        public float Motor_I2C_Get_Ki(byte address)
        {
            byte code = 1;
            SendCommand(new byte[] { MOTOR_I2C_GET_PID_PARAMS, code }, address, true, 7);            
            byte[] bb = new byte[7];            
            serial.Read(bb, 0, 7);
            if ((bb[1] == MOTOR_I2C_GET_PID_PARAMS) && (bb[2] == code)) return BitConverter.ToSingle(bb, 3);
            else return float.NaN;
        }

        public void Motor_I2C_Set_Kd(byte address, float Kd)
        {
            byte code = 2;
            byte[] bb = BitConverter.GetBytes(Kd);
            SendCommand(new byte[] { MOTOR_I2C_SET_PID_PARAMS, code, bb[0], bb[1], bb[2], bb[3] }, address, false);
        }

        public float Motor_I2C_Get_Kd(byte address)
        {
            byte code = 2;
            SendCommand(new byte[] { MOTOR_I2C_GET_PID_PARAMS, code }, address, true, 7);            
            byte[] bb = new byte[7];            
            serial.Read(bb, 0, 7);
            if ((bb[1] == MOTOR_I2C_GET_PID_PARAMS) && (bb[2] == code)) return BitConverter.ToSingle(bb, 3);
            else return float.NaN;
        }

        public void Motor_I2C_Set_Dt(byte address, float Dt)
        {
            byte code = 3;
            byte[] bb = BitConverter.GetBytes(Dt);
            SendCommand(new byte[] { MOTOR_I2C_SET_PID_PARAMS, code, bb[0], bb[1], bb[2], bb[3] }, address, false);
        }

        public float Motor_I2C_Get_Dt(byte address)
        {
            byte code = 3;
            SendCommand(new byte[] { MOTOR_I2C_GET_PID_PARAMS, code }, address, true, 7);
            byte[] bb = new byte[7];
            serial.Read(bb, 0, 7);
            if ((bb[1] == MOTOR_I2C_GET_PID_PARAMS) && (bb[2] == code)) return BitConverter.ToSingle(bb, 3);
            else return float.NaN;
        }

        public void Comm_Set_Acknowledge(byte address, bool ack)
        {
            acknowledge_mode[address] = ack;
            SendCommand(new byte[] { COMM_SET_ACKNOWLEDGE_MODE, (byte)(ack?1:0) }, address, false);            
        }

        public void Close()
        {
            serial.Close();
        }
    }
}

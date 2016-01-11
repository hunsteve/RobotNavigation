using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MarkerDesigner
{
    public partial class Form1 : Form
    {
        string filename;
        public Form1()
        {
            InitializeComponent();
            drawBoard1.ImageChanged += new EventHandlerImageChanged(drawBoard1_ImageChanged);
            drawDiagram1.DiagramChanged += new EventHandlerDiagramChanged(drawDiagram1_DiagramChanged);
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;            
            filename = "untitled.png";
            setTitle();
        }

        void drawDiagram1_DiagramChanged(object sender)
        {
            Point[] c = drawDiagram1.PointList;
            if (c != null)
            {
                drawBoard1.DrawPolygon(c, new Point(drawBoard1.Width/4, drawBoard1.Height/4),0.5,0);
            }
        }

        void drawBoard1_ImageChanged(object sender)
        {
            Shape c = drawBoard1.GetContour();
            if (c != null)
            {
                drawDiagram1.PointList = c.contour.ToArray();
            }
            else
            {
                drawDiagram1.PointList = null;
            }
        }

        private void setTitle()
        {            
            this.Text = "Marker Designer - " + Path.GetFileName(filename);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawBoard1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Shape c = drawBoard1.GetContour();
            if (c != null)
            {
                drawBoard1.DrawPolygon(c.contour.ToArray(),c.pos,c.scale,c.rot);
                drawDiagram1.PointList = c.contour.ToArray();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename != "untitled.png") drawBoard1.Save(filename);
            else saveAsToolStripMenuItem_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.Filter = "PNG (*.png)|*.png";
            fd.FileName = filename;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                filename = fd.FileName;
                setTitle();

                drawBoard1.Save(fd.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "PNG (*.png)|*.png";
            fd.CheckFileExists = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                filename = fd.FileName;
                setTitle();

                drawBoard1.Open(fd.FileName);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawBoard1.Clear();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by StEvE. (c) 2009");
        }
        
    }
}

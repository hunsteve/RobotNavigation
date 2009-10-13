using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using OnlabNeuralis;

namespace NNImage
{
    public class MLP
    {
        public List<List<INeuron>> mlp;
        public MLP(int[] counts, int input)
        {
            mlp = new List<List<INeuron>>();

            //bemeneti reteg
            List<INeuron> inputlayer = new List<INeuron>();
            for (int i = 0; i < input; ++i)
            {
                InputNeuron n = new InputNeuron();
                inputlayer.Add(n);
            }
            mlp.Add(inputlayer);

            //rejtett retegek
            int layernum = 0;
            foreach (int count in counts)
            {
                List<INeuron> layer = new List<INeuron>();
                for (int i = 0; i < count; ++i)
                {
                    Neuron n = new Neuron((layernum < counts.Length - 1));
                    //elozo reteg osszes neuronja bemenet
                    foreach (INeuron n2 in mlp[mlp.Count-1])
                    {
                        n.AddInputNeuron(n2);
                    }
                    layer.Add(n);
                }
                mlp.Add(layer);
                layernum++;
            }
        }

        public MLP(MLP copyInstance)
        {
            mlp = new List<List<INeuron>>();
            List<INeuron> inputlayer = new List<INeuron>();
            for (int i = 0; i < copyInstance.mlp[0].Count; ++i)
            {
                InputNeuron n = new InputNeuron();
                inputlayer.Add(n);                
            }
            mlp.Add(inputlayer);

            int layernum = 0;
            for (int i = 1; i < copyInstance.mlp.Count; ++i)
            {
                List<INeuron> layer = new List<INeuron>();
                for (int i2 = 0; i2 < copyInstance.mlp[i].Count; ++i2)
                {
                    Neuron n = new Neuron(((Neuron)copyInstance.mlp[i][i2]).nonlinear);
                    layer.Add(n);
                    //elozo reteg osszes neuronja bemenet
                    foreach (INeuron n2 in mlp[i - 1])
                    {
                        n.AddInputNeuron(n2);                        
                    }                    
                    //sulyok atmasolasa
                    int i3=0;
                    foreach (NeuronInput ni in n.inputs)
                    {
                        ni.w = ((Neuron)copyInstance.mlp[i][i2]).inputs[i3].w;
                        ++i3;
                    }                    
                }
                mlp.Add(layer);
                layernum++;
            }
        }

        public MLP(string filename)
        {
            this.LoadNN(filename);
        }

        public void CalcOutput(double[] input)
        {
            int i = 0;
            foreach (INeuron n in mlp[0])
            {
                ((InputNeuron)n).SetInput(input, i);
                ++i;
            }

            for (i = 1; i < mlp.Count; ++i)
            {
                foreach (INeuron n in mlp[i])
                {
                    ((Neuron)n).CalcOutput();
                }
            }
        }

        public double[] Output()
        {
            double[] outp = new double[mlp[mlp.Count-1].Count];
            int i = 0;
            foreach (INeuron n in mlp[mlp.Count-1])
            {
                outp[i] = n.Output();
                ++i;
            }
            return outp;
        }

        public double[] Output(double[] input)
        {
            CalcOutput(input);
            return Output();
        }

        public void Train(double mu,double[] error)
        {
            //utolso retegre a deltak kiszamitasa
            int i=0;
            foreach (INeuron n in mlp[mlp.Count-1])
            {
                ((Neuron)n).CalcDelta(error[i]);
                ((Neuron)n).Train(mu);
                ++i;
            }

            //rejtett retegekre visszafele hibavisszaterjesztes
            for (i = mlp.Count - 2; i > 0; --i)//bemeneti retegre nem
            {
                foreach (INeuron n in mlp[i])
                {
                    ((Neuron)n).CalcDelta();
                    ((Neuron)n).Train(mu);
                }
            }
        }

        public void Sensitibility(double[] error, out double[] sensitibility)
        {
            foreach (INeuron n in mlp[0])
            {
                ((InputNeuron)n).SetNullError();//bemeneti reteg hibainak nullazasa             
            }

            int i = 0;
            foreach (INeuron n in mlp[mlp.Count - 1])
            {
                ((Neuron)n).CalcDelta(error[i]);
                ((Neuron)n).BackPropagate();
                ++i;
            }

            //rejtett retegekre visszafele hibavisszaterjesztes
            for (i = mlp.Count - 2; i > 0; --i)//bemeneti retegre nem
            {
                foreach (INeuron n in mlp[i])
                {
                    ((Neuron)n).CalcDelta();
                    ((Neuron)n).BackPropagate();
                }
            }

            sensitibility = new double[mlp[0].Count];
            int i2 = 0;
            foreach (INeuron n in mlp[0])
            {
                sensitibility[i2] = ((InputNeuron)n).error;
                i2++;
            }
        }


        /*public double TrainLM(List<TrainingPoint> points)
        {
            double mu = 0.1;
            int w = 0;
            for (int i = 1; i < mlp.Count; ++i)
            {
                w += mlp[i].Count * (mlp[i - 1].Count + 1); //elozo reteg + bias
            }
            int rows = points.Count * points[0].desiredOutput.Length;
            Matrix J = new Matrix(rows,w);
            Matrix R = new Matrix(rows, 1);
            int rowindex = 0;
            double sumerror = 0;
            foreach (TrainingPoint tp in points)
            {
                double[] outp = this.Output(tp.input);
                double[] error = new double[tp.desiredOutput.Length];
                for (int i = 0; i < tp.desiredOutput.Length; ++i)
                {
                    error[i] = tp.desiredOutput[i] - outp[i];
                    sumerror += Math.Abs(error[i]);
                }

                for (int i = 0; i < outp.Length; ++i)
                {
                    //kimeneti retegre hiba kiszamolas, kulon-kulon a kimenetekre
                    for (int i2 = 0; i2 < mlp[mlp.Count - 1].Count; ++i2)
                    {
                        if (i2 == i) ((Neuron)mlp[mlp.Count - 1][i2]).CalcDelta(error[i]);
                        else ((Neuron)mlp[mlp.Count - 1][i2]).CalcDelta(0);
                        ((Neuron)mlp[mlp.Count - 1][i2]).BackPropagate();
                    }
                    //rejtett retegekre visszafele hibavisszaterjesztes
                    for (int i2 = mlp.Count - 2; i2 > 0; --i2)//bemeneti retegre nem
                    {
                        foreach (INeuron n in mlp[i2])
                        {
                            ((Neuron)n).CalcDelta();
                            ((Neuron)n).BackPropagate();
                        }
                    }

                    // J matrix letrehozasa
                    int colindex = 0;//egy sorba berakjuk az osszes sulyt
                    for (int i2 = 1; i2 < mlp.Count; i2++)
                    {
                        foreach (INeuron n in mlp[i2])
                        {
                            for (int i3 = 0; i3 < ((Neuron)n).inputs.Count; ++i3)
                            {
                                double sens = ((Neuron)n).WeightSensibilityToError(i3, error[i]);
                                J[rowindex, colindex] = sens;
                                colindex++;
                            }
                        }
                    }

                    //R matrix letrehozasa
                    R[rowindex, 0] = error[i];


                    rowindex++;                    
                }
            }

           

            Matrix temp1 = (J.Transpose() * J + mu * Matrix.IdentityMatrix(w, w));
            Matrix temp2 = temp1.Inverse() *J.Transpose() * R; //delta W

            int rowindex2 = 0;
            for (int i2 = 1; i2 < mlp.Count; i2++)
            {
                foreach (INeuron n in mlp[i2])
                {
                    foreach (NeuronInput ni in ((Neuron)n).inputs)
                    {
                        ni.w += temp2[rowindex2, 0];
                        rowindex2++;                  
                    }
                }
            }
            return sumerror / points.Count;
        }*/

        public void AddNoise(double noisefactor)
        {
            for (int i = 1; i < mlp.Count; ++i)
            {
                foreach (INeuron n in mlp[i])
                {
                    ((Neuron)n).AddNoise(noisefactor);
                }
            }
        }

        public void AddNeuron(int layer)
        {
            Neuron n = new Neuron(true);
            foreach (INeuron n2 in mlp[layer - 1])
            {
                n.AddInputNeuron(n2);
            }
            mlp[layer].Add(n);
            foreach (INeuron n2 in mlp[layer + 1])
            {
                ((Neuron)n2).AddInputNeuron(n);
            }
        }

        public void SaveNN(string filename)
        {
            TextWriter tw = new StreamWriter(filename);
            tw.WriteLine("M");
            foreach (List<INeuron> layer in mlp)
            {
                tw.WriteLine("L");
                foreach (INeuron n in layer)
                {                    
                    if (n.GetType() == typeof(Neuron))
                    {
                        if (((Neuron)n).nonlinear) tw.WriteLine("Pn");
                        else tw.WriteLine("Pl");
                        foreach (NeuronInput ni in ((Neuron)n).inputs)
                        {
                            if (ni.n != null) tw.WriteLine("w");
                            else tw.WriteLine("b");
                            tw.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###############}", ni.w));
                        }
                    }
                    else tw.WriteLine("I");
                }
            }
            tw.WriteLine("E");
            tw.Flush();
            tw.Close();
        }

        public void LoadNN(string filename)
        {
            TextReader tr = new StreamReader(filename);
            string line = tr.ReadLine();
            List<INeuron> layer = null;
            Neuron n = null;
            int ni_counter = 0;
            while (line != "E")
            {
                if (line == "M") mlp = new List<List<INeuron>>();
                if (line == "L")
                {
                    layer = new List<INeuron>();
                    mlp.Add(layer);
                }
                if (line == "I") layer.Add(new InputNeuron());
                if ((line == "Pn")||(line == "P"))
                {
                    n = new Neuron(true);
                    ni_counter = 0;
                    layer.Add(n);
                }
                if (line == "Pl")
                {
                    n = new Neuron(false);
                    ni_counter = 0;
                    layer.Add(n);
                }
                if (line == "b")
                {
                    string w = tr.ReadLine();
                    n.inputs[0].w = double.Parse(w, CultureInfo.InvariantCulture);                    
                }
                if (line == "w")
                {
                    string w = tr.ReadLine();
                    NeuronInput ni = new NeuronInput();
                    ni.w = double.Parse(w, CultureInfo.InvariantCulture);
                    ni.n = mlp[mlp.Count - 2][ni_counter];                    
                    n.inputs.Add(ni);
                    ni_counter++;                        
                }
                line = tr.ReadLine();
            }
            tr.Close();
        }

        public Bitmap Visualize(int width, int height, double scaleX, double scaleY, int xindex, int yindex, double[] otherargs, int outpindex)
        {
            lock (this)
            {
                uint[] buf = new uint[width * height];

                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        double[] inp = new double[otherargs.Length];
                        for (int i = 0; i < otherargs.Length; ++i)
                        {
                            inp[i] = otherargs[i];
                        }
                        inp[xindex] = ComMath.Normal(x, 0, width - 1, -scaleX, scaleX);
                        inp[yindex] = ComMath.Normal(y, 0, height - 1, -scaleY, scaleY);
                        double[] outp = this.Output(inp);
                        int blue = 0;
                        int green = 0;
                        int red = 0;
                        blue = (int)ComMath.Normal(outp[outpindex], -1, 1, 255, 0);
                        //if (outp[outpindex] >= -1) 
                        //else { blue = 255; green = 255; }
                        green = (int)ComMath.Normal(outp[outpindex], -1, 1, 0, 255);
                        //if (outp[outpindex] <= 1) 
                        //else { red = 255; green = 255; }
                        if (blue > 255) blue = 255;
                        if (blue < 0) blue = 0;
                        if (red > 255) red = 255;
                        if (red < 0) red = 0;
                        if (green > 255) green = 255;
                        if (green < 0) green = 0;

                        buf[y * width + x] = 0xFF000000 + (uint)(red * 256 * 256 + green * 256 + blue);
                    }





                Bitmap bm = new Bitmap(width, height);
                BitmapData bmData = bm.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

                int len = bmData.Width * bmData.Height;
                unsafe
                {
                    uint* cim = (uint*)bmData.Scan0.ToPointer();//direkt bitmap memoriaba rajzolunk, gyors                
                    for (int i = 0; i < len; ++i)
                    {
                        cim[i] = buf[i];
                    }
                }

                bm.UnlockBits(bmData);
                return bm;
            }
        }

    }
}

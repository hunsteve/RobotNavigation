using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using OnlabNeuralis;
using System.Drawing.Imaging;

namespace NeuralNetworkLib
{


    public class MLPDll
    {
        private unsafe struct Matrix
        {
            public float* data;
            public int width, height;
        }

        private unsafe struct MLP
        {
            //layer, neurons * inputs
            public Matrix* weights;
            public Matrix* deltaWeights;
            public Matrix* sums;
            public Matrix* ends;
            public Matrix* deltas;
            public Matrix sensibility;

            public int* neuronCounts;
            public int layerCount;
        }

        [DllImport("MLPDll.dll")]
        private static extern unsafe Matrix createMatrix(int width, int height);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void deleteMatrix(Matrix m);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void RandomClearWeights(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe MLP createMLP(int inputLength, int* neuronCounts, int layerCount);

        [DllImport("MLPDll.dll")]
        private static extern unsafe MLP copyMLP(MLP copy);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void deleteMLP(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void Sigmoid(Matrix input, Matrix dest);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void SetInput(MLP mlp, Matrix input);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void ForwardPorpagate(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe Matrix Output(MLP mlp, Matrix input);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void SetOutputError(MLP mlp, Matrix errors);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void Backpropagate(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void CalculateDeltaWeights(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void ChangeWeights(MLP mlp, float mu);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void Train(MLP mlp, Matrix errors, float mu);

        [DllImport("MLPDll.dll")]
        private static extern unsafe int AddDeltaWeights(MLP src1, MLP src2, MLP dest);

        [DllImport("MLPDll.dll")]
        private static extern unsafe void ClearDeltaWeights(MLP mlp);

        [DllImport("MLPDll.dll")]
        private static extern unsafe float MaxDeltaWeight(MLP mlp);


        MLP mlp;

        public MLPDll(int[] neuronCount, int inputLength)
        {
            // Fix the pointer to the array. Assume array is the variable and T is
            // the type of the array.
            unsafe
            {
                fixed (int* nc = neuronCount)
                {
                    // pArray now has the pointer to the array. You can get an IntPtr
                    //by casting to void, and passing that in.
                    mlp = createMLP(inputLength, nc, neuronCount.Length);
                }
            }
        }

        public MLPDll(MLPDll copy)
        {
            mlp = copyMLP(copy.mlp);
        }

        ~MLPDll()
        {
            deleteMLP(mlp);        
        }

        public void SetInput(double[] inp)
        {
            float[] input = (float[])inp.Cast<float>();
            SetInput(input);
        }

        public void SetInput(float[] input)
        {         
            unsafe
            {
                fixed (float* inp = input) {
                    Matrix inpv;
                    inpv.data = inp;
                    inpv.width = 1;
                    inpv.height = input.Length;
                    SetInput(mlp, inpv);      
                }
            }
        }

        public void ForwardPorpagate()
        {
            ForwardPorpagate(mlp);
        }

        private static double[] FloatArrToDouble(float[] inp)
        {
            double[] ret = new double[inp.Length];
            for (int i = 0; i < ret.Length; ++i) ret[i] = inp[i];
            return ret;
        }

        private static float[] DoubleArrToFloat(double[] inp)
        {
            float[] ret = new float[inp.Length];
            for (int i = 0; i < ret.Length; ++i) ret[i] = (float)inp[i];
            return ret;
        }

        public double[] Output(double[] inp)
        {                        
            return FloatArrToDouble(Output(DoubleArrToFloat(inp)));
        }

        public float[] Output(float[] input)
        {
            float[] ret;
            unsafe
            {
                fixed (float* inp = input)
                {
                    Matrix inpv;
                    inpv.data = inp;
                    inpv.width = 1;
                    inpv.height = input.Length;
                    Matrix outp = Output(mlp, inpv);
                    ret = new float[outp.height];
                    Marshal.Copy(new IntPtr(outp.data), ret, 0, outp.height);
                }
            }
            return ret;        
        }

        public void SetOutputError(double[] inp)
        {            
            SetOutputError(DoubleArrToFloat(inp));         
        }

        public void SetOutputError(float[] errors)
        {
            unsafe
            {
                fixed (float* inp = errors)
                {
                    Matrix inpv;
                    inpv.data = inp;
                    inpv.width = 1;
                    inpv.height = errors.Length;
                    SetOutputError(mlp, inpv);
                }
            }
        }

        public void Backpropagate()
        {
            Backpropagate(mlp);
        }

        public void CalculateDeltaWeights()
        {
            CalculateDeltaWeights(mlp);
        }

        public void ChangeWeights(double mu)
        {
            ChangeWeights((float)mu);
        }

        public void ChangeWeights(float mu)
        {
            ChangeWeights(mlp, mu);
        }

        public void Train(double mu, double[] errs)
        {            
            Train((float)mu, DoubleArrToFloat(errs));
        }

        public void Train(float mu, float[] errors)
        {
            unsafe
            {
                fixed (float* inp = errors)
                {
                    Matrix inpv;
                    inpv.data = inp;
                    inpv.width = 1;
                    inpv.height = errors.Length;
                    Train(mlp, inpv, mu);
                }
            }
        }

        public double[] SensitibilityD()
        {            
            return FloatArrToDouble(SensitibilityF());
        }

        public float[] SensitibilityF()
        {
            float[] ret = new float[mlp.sensibility.height];
            unsafe
            {
                Marshal.Copy(new IntPtr(mlp.sensibility.data), ret, 0, mlp.sensibility.height);
            }
            return ret;
        }

        public void AddDeltaWeights(MLPDll src1)
        {
            AddDeltaWeights(src1.mlp, this.mlp, this.mlp);
        }

        public void ClearDeltaWeights()
        {
            ClearDeltaWeights(mlp);
        }

        public float MaxDeltaWeight()
        {
            return MaxDeltaWeight(mlp);
        }

        public Bitmap Visualize(int width, int height, RectangleF rect, int xindex, int yindex, double[] otherargs, int outpindex, double minNeuronOutp, double maxNeuronOutp)
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
                        inp[xindex] = ComMath.Normal(x, 0, width - 1, rect.Left, rect.Right);
                        inp[yindex] = ComMath.Normal(y, 0, height - 1, rect.Top, rect.Bottom);
                        double[] outp = this.Output(inp);
                        int blue = 0;
                        int green = 0;
                        int red = 0;
                        blue = (int)ComMath.Normal(outp[outpindex], minNeuronOutp, maxNeuronOutp, 255, 0);
                        //if (outp[outpindex] >= -1) 
                        //else { blue = 255; green = 255; }
                        green = (int)ComMath.Normal(outp[outpindex], minNeuronOutp, maxNeuronOutp, 0, 255);
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

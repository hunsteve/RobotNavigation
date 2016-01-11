using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NNImage
{
    public class NeuronInput
    {
        public INeuron n;
        public double bias;
        public double w;
        public double deltaw;
        public double deltawmomentum;
    }

    public class Neuron : INeuron
    {
        static int randomSeed = (int)DateTime.Now.Ticks%56264563;
        private Random rand;
        public List<NeuronInput> inputs;
        private double output;
        private double summa;
        private double delta;
        private double error;
        public bool nonlinear;
        
        
        public Neuron(bool nonlinear)
        {
            rand = new Random((int)(Neuron.randomSeed++));
            Neuron.randomSeed = Neuron.randomSeed % 56264563;
            inputs = new List<NeuronInput>();
            NeuronInput inp = new NeuronInput();
            inp.n = null;
            inp.w = randomWeight();
            inp.bias = 1;
            inputs.Add(inp);
            this.nonlinear = nonlinear;
        }

        private double randomWeight()
        {
            double r;
            do
            {
                r = (rand.NextDouble() - 0.5)*2;
            }
            while (Math.Abs(r) < 0.00001);
            return r * 0.05;
        }

        public void AddInputNeuron(INeuron n)
        {
            NeuronInput inp = new NeuronInput();
            inp.n = n;
            inp.w = randomWeight();
            inp.bias = 0;
            inputs.Add(inp);            
        }

        public void CalcOutput()
        {
            summa = 0;
            foreach (NeuronInput input in inputs)
            {
                if (input.n != null) summa += input.n.Output() * input.w;
                else summa += input.bias * input.w;
            }
            if (nonlinear) output = Math.Tanh(summa);
            else output = summa;
        }

        public void CalcDelta(double error)
        {
            if (nonlinear) delta = error * (1 - Math.Tanh(summa) * Math.Tanh(summa));
            else delta = error;
        }

        public void CalcDelta()
        {
            if (nonlinear) delta = this.error * (1 - Math.Tanh(summa) * Math.Tanh(summa));
            else delta = this.error;
            this.error = 0;
        }

        public void BackPropagate()
        {
            foreach (NeuronInput input in inputs)
            {                
                if (input.n != null)
                {
                    input.deltaw = 2 * delta * input.n.Output();
                    input.n.AddError(delta * input.w);//elozo neuronra a visszaterjesztett hiba
                }
                else input.deltaw = 2 * delta * input.bias;             
            }
        }

        public void Train(double mu)
        {
            foreach (NeuronInput input in inputs)
            {
                double x;
                if (input.n != null)
                {
                    x = input.n.Output();
                    input.n.AddError(delta * input.w);//elozo neuronra a visszaterjesztett hiba
                }
                else x = input.bias;
                input.deltaw = 2 * mu * delta * x;// +0.5 * input.deltawmomentum;
                input.w += input.deltaw;//delta szabaly, LMS + momentum modszer
                input.deltawmomentum = input.deltaw;                
            }
        }

        public void AddNoise(double noisefactor)
        {
            foreach (NeuronInput input in inputs)
            {
                double d = (rand.Next(1000)+1) / 1000.0;
                input.w += noisefactor * input.w * (rand.Next(2) * 2 - 1) * Math.Sqrt(-Math.Log(d));
            }
        }

        public double WeightSensibilityToError(int index, double error)
        {
            NeuronInput input = inputs[index];
            double x;
            if (input.n != null)
            {
                x = input.n.Output();                
            }
            else x = input.bias;
            return delta * x / error;
        }


        #region INeuron Members

        public double Output()
        {
            return output;
        }

        public void AddError(double error)
        {
            this.error += error;
        }

        #endregion
    }
}


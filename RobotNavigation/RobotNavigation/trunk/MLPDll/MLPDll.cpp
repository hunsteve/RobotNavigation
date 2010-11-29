#include "MLPDll.h"
#include <math.h>
#include <cstdlib>
#include <iostream>
#include <time.h>

using namespace std;

Matrix createMatrix(int width, int height) {
	Matrix m;
	m.width = width;
	m.height = height;
	m.data = new float[width*height];
	return m;
}

float maxMatrix(Matrix m) 
{
	int l = m.width * m.height;
	float max = m.data[0];
	for(int i=1; i<l; ++i) {
		float j = m.data[i];
		if (j > max) max = j;
	}
	return max;
}

float maxAbsMatrix(Matrix m) 
{
	int l = m.width * m.height;
	float max = abs(m.data[0]);
	for(int i=1; i<l; ++i) {
		float j = abs(m.data[i]);
		if (j > max) max = j;
	}
	return max;
}

void deleteMatrix(Matrix m)
{
	delete[] m.data;
}

int addMatrix(Matrix m1, Matrix m2, Matrix dest) 
{
	if ((m1.width != m2.width) || (m2.width != dest.width)	|| (m1.height != m2.height) || (m2.height != dest.height)) {
		return -1;
	}
	int l = m1.width * m1.height;
	for(int i=0; i<l; ++i) 
	{
		dest.data[i] = m1.data[i] + m2.data[i];
	}
	return 0;
}


int addMatrix(Matrix m1, float mult1, Matrix m2, float mult2, Matrix dest) 
{
	if ((m1.width != m2.width) || (m2.width != dest.width)	|| (m1.height != m2.height) || (m2.height != dest.height)) {
		return -1;
	}
	int l = m1.width * m1.height;
	for(int i=0; i<l; ++i) 
	{
		dest.data[i] = m1.data[i] * mult1 + m2.data[i] * mult2;
	}
	return 0;
}

void transposeMatrix(Matrix* m) {
	if ((m->width > 1) && (m->height > 1)) {
		int w = m->width;
		int h = m->height;
		for(int i=0; i<h; ++i) 
		{
			for(int j=0; j<w; ++j) 
			{
				float t = m->data[i * w + j];
				m->data[i * w + j] = m->data[j * h + i];
				m->data[j * h + i] = t;
			}
		}
	}
	int x = m->height;
	m->height = m->width;
	m->width = x;
}

int multMatrix(Matrix m1, Matrix m2, Matrix dest) 
{
	if ((m1.width != m2.height) || (dest.width != m2.width) || (dest.height != m1.height)) {
		return -1;
	}

	int w1 = m1.width;
	int w2 = m2.width;
	int h1 = m1.height;
	int h2 = m2.height;

	for(int j=0; j<h1; ++j)
	{
		for(int k=0; k<w2; ++k) 
		{
			float sum = 0;
			for(int i=0; i<w1; ++i)
			{
				sum += m1.data[j * w1 + i] * m2.data[i * w2 + k];
			}
			dest.data[j * w2 + k] = sum;
		}
	}
	return 0;
}

int elementMult(Matrix m1, Matrix m2, Matrix dest) {
	if ((m1.width != m2.width) || (m1.height != m2.height) || (dest.width != m2.width) || (dest.height != m2.height) || ((m1.height != 1) && (m1.width != 1))) {
		return -1;
	}
	int l = 0;
	if (m1.width == 1) l = m1.height;
	else l = m1.width;

	for (int i=0; i<l; ++i) {
		dest.data[i] = m1.data[i] * m2.data[i];
	}
	return 0;
}

int copyMatrix(Matrix input, Matrix dest) 
{
	if ((input.width > dest.width) || (input.height > dest.height)) {
		return -1;
	}
	memcpy(dest.data,input.data,input.width * input.height * sizeof(float));
	return 0;
}


int concatRight(Matrix m1, Matrix m2, Matrix dest) {
	if ((m1.height != m2.height) || (dest.width != m1.width + m2.width) || (dest.height != m1.height)) {
		return -1;
	}
	for(int i = 0; i < m1.height; ++i) 
	{
		memcpy(dest.data + i*dest.width*sizeof(float), m1.data + i*m1.width*sizeof(float), m1.width*sizeof(float));
		memcpy(dest.data + (i*dest.width + m1.width)*sizeof(float), m2.data + i*m2.width*sizeof(float), m2.width*sizeof(float));
	}
	return 0;
}

int concatDown(Matrix m1, Matrix m2, Matrix dest) {
	if ((m1.width != m2.width) || (dest.height != m1.height + m2.height) || (dest.width != m1.width)) {
		return -1;
	}
	memcpy(dest.data, m1.data, (m1.width*m1.height)*sizeof(float));
	memcpy(dest.data + (m1.width*m1.height)*sizeof(float), m2.data, (m2.width*m2.height)*sizeof(float));
	return 0;
}

float randomWeight()
{
    float r;
    do
    {
        r = (((float)rand()/RAND_MAX) - 0.5f)*2;
    }
    while (abs(r) < 0.00001f);
    return r * 0.05f;
}

void RandomClearWeights(MLP mlp)
{
	long ltime = time(NULL);
	int stime = (unsigned) ltime/2;
	srand(stime);
	for (int l = 0; l < mlp.layerCount; ++l) //layers
	{
		int w = mlp.weights[l].width;
		int h = mlp.weights[l].height;
		for (int i = 0; i < w; ++i) //neurons
		{
			for (int k = 0; k < h; ++k)//inputs
			{
				mlp.weights[l].data[i * h + k]=  randomWeight();//random
			}
		}
	}
}

void ClearWeakness(MLP mlp) 
{
	if (mlp.isWeakening) {
		for (int l = 0; l < mlp.layerCount; ++l) //layers
		{
			for(int n=0; n<mlp.weakness[l].height; ++n) {
				mlp.weakness[l].data[n] = 1;
			}
		}
	}
}


MLP createMLP(int inputLength ,int* neuronCounts, int layerCount, bool isWeakening)
{
	MLP mlp;
	mlp.layerCount = layerCount;
	mlp.neuronCounts = new int[layerCount];
	mlp.isWeakening = isWeakening;
	mlp.weights = new Matrix[layerCount];
	mlp.deltaWeights = new Matrix[layerCount];
	mlp.sums = new Matrix[layerCount];
	mlp.ends = new Matrix[layerCount + 1];
	if (mlp.isWeakening) {
		mlp.weakness = new Matrix[layerCount];
	}
	mlp.ends[0] = createMatrix(1, inputLength + 1);
	mlp.ends[0].data[inputLength] = 1; //bias
	mlp.deltas = new Matrix[layerCount];
	mlp.sensibility = createMatrix(1,inputLength + 1);
	mlp.sensibility.height--;//biasnak hagytunk helyet, de az nem kell
	int tmp = inputLength;
	for (int i = 0; i < layerCount; ++i)//layers
	{
		mlp.weights[i] = createMatrix(tmp + 1, neuronCounts[i]);
		mlp.deltaWeights[i] = createMatrix(tmp + 1, neuronCounts[i]);
		mlp.sums[i] = createMatrix(1, neuronCounts[i]);
		if (mlp.isWeakening) {
			mlp.weakness[i] = createMatrix(1, neuronCounts[i]);
		}
		mlp.ends[i+1] = createMatrix(1, neuronCounts[i] + 1);
		mlp.ends[i+1].data[neuronCounts[i]] = 1; //bias
		mlp.deltas[i] = createMatrix(1, neuronCounts[i] + 1);
		mlp.deltas[i].data[neuronCounts[i]] = 0;
		mlp.neuronCounts[i] = tmp = neuronCounts[i];
	}
	RandomClearWeights(mlp);
	if (mlp.isWeakening) {
		ClearWeakness(mlp);
	}
	return mlp;
}

MLP copyMLP(MLP copy) {
	MLP ret = createMLP(copy.ends[0].height-1, copy.neuronCounts, copy.layerCount, copy.isWeakening);	
	for (int i = 0; i < ret.layerCount; ++i)//layers
	{
		copyMatrix(copy.weights[i], ret.weights[i]);
		if (copy.isWeakening) {
			copyMatrix(copy.weakness[i], ret.weakness[i]);
		}
	}
	return ret;
}

void deleteMLP(MLP mlp) 
{
	for (int i = 0; i < mlp.layerCount; ++i)//layers
	{
		deleteMatrix(mlp.weights[i]); 
		deleteMatrix(mlp.deltaWeights[i]); 
		deleteMatrix(mlp.sums[i]); 
		deleteMatrix(mlp.ends[i+1]); 
		deleteMatrix(mlp.deltas[i]); 
		if (mlp.isWeakening) {
			deleteMatrix(mlp.weakness[i]);
		}
	}

	deleteMatrix(mlp.ends[0]);
	delete[] mlp.weights;
	delete[] mlp.deltaWeights;
	delete[] mlp.sums;
	delete[] mlp.ends;
	delete[] mlp.deltas;
	if (mlp.isWeakening) {
		delete[] mlp.weakness;
	}
	deleteMatrix(mlp.sensibility);
}


int Sigmoid(Matrix input, Matrix dest)
{
	if ((input.width > dest.width) || (input.height > dest.height)) return -1;
	int l = input.width * input.height;
	for (int i = 0; i < l; ++i)
	{
		dest.data[i] = (float)tanh(input.data[i]);
	}
	return 0;
}

int StepWeakness(Matrix wo, Matrix weakness) {
	float a = 0.2f;
	float b = 1;
	for(int i=0; i<weakness.height; ++i) {		
		if (weakness.data[i] < 1) weakness.data[i] += a;
		weakness.data[i] += - (wo.data[i] * wo.data[i]) * b;		
		if (weakness.data[i] < 0) weakness.data[i] = 0;
		if (weakness.data[i] > 1) weakness.data[i] = 1;		
	}
	return 0;
}



void SetInput(MLP mlp, Matrix input)
{
	copyMatrix(input, mlp.ends[0]);
}

void ForwardPorpagate(MLP mlp)
{
	for (int l = 0; l < mlp.layerCount; ++l) //layers
	{
		multMatrix(mlp.weights[l], mlp.ends[l], mlp.sums[l]);
		Sigmoid(mlp.sums[l], mlp.ends[l + 1]);
		if (mlp.isWeakening) {
			mlp.ends[l + 1].height--;//bias miatt
			elementMult(mlp.ends[l + 1], mlp.weakness[l], mlp.ends[l + 1]);			
			StepWeakness(mlp.ends[l + 1], mlp.weakness[l]);
			mlp.ends[l + 1].height++;//bias miatt
		}		
	}
}

Matrix Output(MLP mlp, Matrix input)
{
	SetInput(mlp, input);
	ForwardPorpagate(mlp);
	return mlp.sums[mlp.layerCount - 1];
}

void SetOutputError(MLP mlp, Matrix errors)
{
	copyMatrix(errors, mlp.deltas[mlp.layerCount - 1]);
}

void Backpropagate(MLP mlp)
{
	for (int l = mlp.layerCount - 2; l >= 0; --l)//layer
	{
		mlp.deltas[l+1].height--;//bias miatt
		transposeMatrix(&mlp.deltas[l+1]);
		transposeMatrix(&mlp.deltas[l]);
		multMatrix(mlp.deltas[l+1], mlp.weights[l+1] ,mlp.deltas[l]);
		transposeMatrix(&mlp.deltas[l+1]);
		transposeMatrix(&mlp.deltas[l]);
		mlp.deltas[l+1].height++;//bias miatt
		for (int i = 0; i < mlp.deltas[l].height - 1; ++i)//inputs, bias nem kell, az linearis
		{				
			float t = tanh(mlp.sums[l].data[i]);
			mlp.deltas[l].data[i] *= (float)(1 - t * t);
			if (mlp.isWeakening) {
				 mlp.deltas[l].data[i] *= mlp.weakness[l].data[i];
			}
		}
		mlp.deltas[l].data[mlp.deltas[l].height - 1] = 0;
	}

	//sensibility kiszamitasa
	mlp.deltas[0].height--;//bias miatt
	mlp.sensibility.height++;//bias miatt, de ez felesleges
	transposeMatrix(&mlp.deltas[0]);
	transposeMatrix(&mlp.sensibility);
	multMatrix(mlp.deltas[0], mlp.weights[0] ,mlp.sensibility);
	transposeMatrix(&mlp.deltas[0]);
	transposeMatrix(&mlp.sensibility);
	mlp.deltas[0].height++;//bias miatt
	mlp.sensibility.height--;//bias miatt, de ez felesleges
}

void CalculateDeltaWeights(MLP mlp)
{
	for (int l = 0; l < mlp.layerCount; ++l) //layers
	{
		mlp.deltas[l].height--;
		transposeMatrix(&mlp.ends[l]);
		multMatrix(mlp.deltas[l], mlp.ends[l], mlp.deltaWeights[l]);
		transposeMatrix(&mlp.ends[l]);
		mlp.deltas[l].height++;
	}
}

void ClearDeltaWeights(MLP mlp) 
{
	for(int i=0; i<mlp.layerCount; ++i) {
		for(int j=0; j<mlp.deltaWeights[i].height*mlp.deltaWeights[i].width; ++j) 
		{
			mlp.deltaWeights[i].data[j] = 0;
		}
	}
}

void ChangeWeights(MLP mlp, float mu)
{
	for (int l = 0; l < mlp.layerCount; ++l) //layers
	{
		addMatrix(mlp.weights[l], 1, mlp.deltaWeights[l], mu, mlp.weights[l]);
	}
}

void Train(MLP mlp, Matrix errors, float mu)
{
	SetOutputError(mlp,errors);
	Backpropagate(mlp);
	CalculateDeltaWeights(mlp);
	ChangeWeights(mlp, mu);
}

int AddDeltaWeights(MLP src1, MLP src2, MLP dest) 
{
	if ((src1.layerCount != dest.layerCount) || (src2.layerCount != dest.layerCount)) return -1;
	for(int i=0; i<src1.layerCount; ++i) {
		addMatrix(src1.deltaWeights[i],src2.deltaWeights[i],dest.deltaWeights[i]);
	}
	return 0;
}

float MaxDeltaWeight(MLP mlp) {
	float max = maxAbsMatrix(mlp.deltaWeights[0]);
	for(int i=1; i<mlp.layerCount; ++i) 
	{
		float j = maxAbsMatrix(mlp.deltaWeights[i]);	
		if (j > max) max = j;
	}
	return max;
}
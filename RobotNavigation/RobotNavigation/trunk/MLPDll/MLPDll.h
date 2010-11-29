#define DLLEXPORT extern "C" __declspec(dllexport)

//  bias
//    \
//     \weights[0][0,2]
//      \
// in0--(+)-----------[sigmoid]----------- 
//      /
//     /weights[0][0,1]
//    / deltaWeights[0][0,1]
//  in1
//
//   ^          ^                 ^
//   ends[0][0] sums[0][0]        ends[1]
//              deltas[0][0]

struct Matrix {
	float* data;
	int width, height;
};

struct MLP {
    //layer, neurons * inputs
    Matrix* weights;
	Matrix* deltaWeights;
    Matrix* sums;
    Matrix* ends;
    Matrix* deltas;
	Matrix sensibility;

	Matrix* weakness;
    
    int* neuronCounts;
	int layerCount;

	bool isWeakening;
};

DLLEXPORT Matrix createMatrix(int width, int height);
DLLEXPORT void deleteMatrix(Matrix m);
DLLEXPORT void RandomClearWeights(MLP mlp);
DLLEXPORT MLP createMLP(int inputLength ,int* neuronCounts, int layerCount, bool isWeakening);
DLLEXPORT MLP copyMLP(MLP copy);
DLLEXPORT void deleteMLP(MLP mlp);
DLLEXPORT int Sigmoid(Matrix input, Matrix dest);
DLLEXPORT void SetInput(MLP mlp, Matrix input);
DLLEXPORT void ForwardPorpagate(MLP mlp);
DLLEXPORT Matrix Output(MLP mlp, Matrix input);
DLLEXPORT void SetOutputError(MLP mlp, Matrix errors);
DLLEXPORT void Backpropagate(MLP mlp);
DLLEXPORT void CalculateDeltaWeights(MLP mlp);
DLLEXPORT void ChangeWeights(MLP mlp, float mu);
DLLEXPORT void Train(MLP mlp, Matrix errors, float mu);
DLLEXPORT int AddDeltaWeights(MLP src1, MLP src2, MLP dest);
DLLEXPORT void ClearDeltaWeights(MLP mlp);
DLLEXPORT float MaxDeltaWeight(MLP mlp);



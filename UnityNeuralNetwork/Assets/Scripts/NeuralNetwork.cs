using System;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private Neuron[] _inputLayer;
    private Neuron[][] _hiddenLayers;
    private Neuron[] _outputLayer;
    private float[] _newBiases;
    
    public NeuralNetwork(int inputCount, int outputCount, int hiddenLayerNeuronsCount, int hiddenLayerCount)
    {
        //TODO fill constuctor
        _inputLayer = new Neuron[inputCount];
        _hiddenLayers = new Neuron[hiddenLayerCount][];
        for (int i = 0; i < hiddenLayerCount; i++)
        {
            _hiddenLayers[i] = new Neuron[hiddenLayerNeuronsCount];
        }
        
        _outputLayer = new Neuron[outputCount];
    }
}
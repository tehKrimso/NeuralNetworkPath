using System;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private Neuron[] _inputLayer;
    private Neuron[][] _hiddenLayers;
    private Neuron[] _outputLayer;
    private float[] _newBiases;

    //сделать слои не тремя массивами а List<Neuron[]>?
    //
    private List<Neuron[]> _layers = new List<Neuron[]>();
    
    //
    private float[][][] _weights; //вес для каждого слоя на связи ВЫХОДЯЩЕЙ из нейрона на слое
                                  //индексы слой/нейрон на слое/нейрона на следующем слое
                                //[1][1][1] -> вес связи между 1-м нейроном на 1-ом слое и 1-м нейроном на следующем слое
    //
    //new constructor
    public NeuralNetwork(int inputCount, int outputCount, int hiddenLayerNeuronsCount, int hiddenLayerCount, 
        float initialWeightsValue ,float initialBiasesValue)
    {
        //TODO refactor

        //layers init
        _weights = new float[1 + hiddenLayerCount][][];
        
        //input
        Neuron[] inputLayer = new Neuron[inputCount];
        for (int i = 0; i < inputCount; i++)
        {
            inputLayer[i] = new Neuron();
            //inputLayer[i].SetWeight(1); //1 так как в инпуте нет весов для перемножения
            inputLayer[i].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
        }

        _weights[0] = new float[inputCount][];
        _layers.Add(inputLayer);
        
        //hidden layers
        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Neuron[] hiddenLayer = new Neuron[hiddenLayerNeuronsCount];
            for (int j = 0; j < hiddenLayerNeuronsCount; j++)
            {
                hiddenLayer[j] = new Neuron();
                //hiddenLayer[j].SetWeight(UnityEngine.Random.Range(-initialWeightsValue,initialWeightsValue));
                hiddenLayer[j].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
            }

            _weights[i + 1] = new float[hiddenLayerNeuronsCount][];
            
            _layers.Add(hiddenLayer);
        }
        
        //output
        Neuron[] outputLayer = new Neuron[outputCount];
        for (int i = 0; i < outputCount; i++)
        {
            outputLayer[i] = new Neuron();
            //outputLayer[i].SetWeight(UnityEngine.Random.Range(-initialWeightsValue,initialWeightsValue));
            outputLayer[i].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
        }

        _layers.Add(outputLayer);
        //
        
        
        //links
        for (int i = 0; i < _layers.Count-1; i++)//для кжадого слоя начиная с инпута
        {
            for (int j = 0; j < _layers[i].Length; j++)//для кжадого нейрона на слое
            {
                _weights[i][j] = new float[_layers[i + 1].Length];
                
                for (int k = 0; k < _layers[i + 1].Length; k++)//для каждого нейрона на следующем слое
                {
                    _layers[i][j].AddOutput(_layers[i+1][k]);
                    _layers[i+1][k].AddInput(_layers[i][j]);
                    _weights[i][j][k] = UnityEngine.Random.Range(-initialWeightsValue, initialWeightsValue);
                }
                
                // foreach (Neuron nextLayerNeuron in _layers[i+1])
                // {
                //     _layers[i][j].AddOutput(nextLayerNeuron);
                //     nextLayerNeuron.AddInput(_layers[i][j]);
                // }
            }
        }
        //
    }

    public void TrainingLoop()
    {
        
    }

    public void FeedForward(float[] inputs)
    {
        //!!!!!!!
        //Biases пока не участвуют !!!!!!!
        //!!!!!!!
        
        //выставлять значение напрявую в активацию, чтобы не пропускат ьинпуты через сигмоид???
        for (int i = 0; i < inputs.Length; i++)
        {
            _layers[0][i].SetValue(inputs[i]);
        }
        
        //для каждого нейрона я беру его значение, домножаю на j-ый вес в связи и отправляю в следующий нейрон
        //или каждый нейрон проходит по своим аутпутам и забирает из них value*weight, и полученную сумму пропускает через активацию
        
        //это можно заменить рекурсией?
        for (int i = 1; i < _layers.Count; i++) //каждый слой начиная с 1 скрытого
        {
            for (int j = 0; j < _layers[i].Length; j++) //каждый нейрон в текущем слое
            {
                float feedforwardValue = 0;
                for (int k = 0; k < _layers[i - 1].Length; k++)//каждый нейрон в предыдущем слое
                {
                    feedforwardValue += _layers[i - 1][k].GetActivationValue() * _weights[i - 1][k][j];
                }
                
                _layers[i][j].SetValue(feedforwardValue);
            }
        }
        
    }

    public float[] GetOutputs()
    {
        float[] outputs = new float[_layers[_layers.Count - 1].Length];
        
        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i] = _layers[_layers.Count - 1][i].GetActivationValue();
        }

        return outputs;
    }
}
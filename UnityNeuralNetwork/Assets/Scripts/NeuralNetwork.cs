using System;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork
{
    private Neuron[] _inputLayer;
    private Neuron[][] _hiddenLayers;
    private Neuron[] _outputLayer;
    private float[] _newBiases;
    public float Fitness = 0;

    //
    private List<Neuron[]> _layers = new List<Neuron[]>();
    
    //
    public float[][][] weights; //вес для каждого слоя на связи ВЫХОДЯЩЕЙ из нейрона на слое
                                  //индексы слой/нейрон на слое/нейрона на следующем слое
                                //[1][1][1] -> вес связи между 1-м нейроном на 1-ом слое и 1-м нейроном на следующем слое
    //
    //new constructor
    public NeuralNetwork(int inputCount, int outputCount, int hiddenLayerNeuronsCount, int hiddenLayerCount, 
        float initialWeightsValue ,float initialBiasesValue)
    {
        //TODO refactor

        //layers init
        weights = new float[1 + hiddenLayerCount][][];
        
        //input
        Neuron[] inputLayer = new Neuron[inputCount];
        for (int i = 0; i < inputCount; i++)
        {
            inputLayer[i] = new Neuron();
            inputLayer[i].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
        }

        weights[0] = new float[inputCount][];
        _layers.Add(inputLayer);
        
        //hidden layers
        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Neuron[] hiddenLayer = new Neuron[hiddenLayerNeuronsCount];
            for (int j = 0; j < hiddenLayerNeuronsCount; j++)
            {
                hiddenLayer[j] = new Neuron();
                hiddenLayer[j].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
            }

            weights[i + 1] = new float[hiddenLayerNeuronsCount][];
            
            _layers.Add(hiddenLayer);
        }
        
        //output
        Neuron[] outputLayer = new Neuron[outputCount];
        for (int i = 0; i < outputCount; i++)
        {
            outputLayer[i] = new Neuron();
            outputLayer[i].SetBias(UnityEngine.Random.Range(-initialBiasesValue, initialBiasesValue));
        }

        _layers.Add(outputLayer);
        //
        
        
        //links
        for (int i = 0; i < _layers.Count-1; i++)//для кжадого слоя начиная с инпута
        {
            for (int j = 0; j < _layers[i].Length; j++)//для кжадого нейрона на слое
            {
                weights[i][j] = new float[_layers[i + 1].Length];
                
                for (int k = 0; k < _layers[i + 1].Length; k++)//для каждого нейрона на следующем слое
                {
                    _layers[i][j].AddOutput(_layers[i+1][k]);
                    _layers[i+1][k].AddInput(_layers[i][j]);
                    weights[i][j][k] = UnityEngine.Random.Range(-initialWeightsValue, initialWeightsValue);
                }
            }
        }
        //
    }

    //crossingover constructor
    public NeuralNetwork(NeuralNetwork first, NeuralNetwork second)
    {
        var firstLayers = first.GetLayers();
        var secondLayers = second.GetLayers();

        var firstWeights = first.GetWeights();
        var secondWeights = first.GetWeights();

        var halfValue = firstLayers.Count / 2;
        
        weights = new float[halfValue * 2-1][][];
        
        for(int i = 0; i<halfValue; i++)
        {
            _layers.Add(firstLayers[i]);
            weights[i] = firstWeights[i];
        }
        
        for(int i = halfValue; i<halfValue*2; i++)
        {
            _layers.Add(secondLayers[i]);

            //костыль
            if (i == halfValue*2-1)
                break;
            //
            weights[i] = secondWeights[i];
        }
    }

    public void TrainingLoop()
    {
        
    }

    public List<Neuron[]> GetLayers() => _layers;
    public float[][][] GetWeights() => weights;

    public void FeedForward(float[] inputs)
    {
        //выставлять значение напрявую в активацию, чтобы не пропускат ьинпуты через сигмоид???
        for (int i = 0; i < inputs.Length; i++)
        {
            _layers[0][i].SetValue(inputs[i]);
        }
        
        for (int i = 1; i < _layers.Count; i++) //каждый слой начиная с 1 скрытого
        {
            for (int j = 0; j < _layers[i].Length; j++) //каждый нейрон в текущем слое
            {
                float feedforwardValue = 0;
                for (int k = 0; k < _layers[i - 1].Length; k++)//каждый нейрон в предыдущем слое
                {
                    feedforwardValue += _layers[i - 1][k].GetValue() * weights[i - 1][k][j];
                }

                float activationValue = _layers[i][j].Activation(feedforwardValue + _layers[i][j].GetBias());
                
                _layers[i][j].SetValue(activationValue);
            }
        }
        
    }

    public float[] GetOutputs()
    {
        float[] outputs = new float[_layers[_layers.Count - 1].Length];
        
        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i] = _layers[_layers.Count - 1][i].GetValue();
        }

        return outputs;
    }

    public void Mutate(float mutationChance, float mutationStrength)
    {
        //weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, 1f) <= mutationChance)
                        ? weights[i][j][k] += UnityEngine.Random.Range(-mutationStrength, mutationStrength)
                        : weights[i][j][k];
                }
            }
        }
        
        //biases
        foreach (Neuron[] layer in _layers)
        {
            foreach (Neuron neuron in layer)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= mutationChance)
                {
                    neuron.SetBias(neuron.GetActivationValue() +
                                   UnityEngine.Random.Range(-mutationStrength, mutationStrength));
                }
            }
        }
    }
    
    public void Save(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        writer.WriteLine(Fitness); //количество пройденых чекпоинтов
        writer.WriteLine(_layers.Count); //количество слоев
        writer.WriteLine(_layers[0].Length); //количество входов
        writer.WriteLine(_layers[1].Length); //количество нейронов в скрытом слое
        writer.WriteLine(_layers[_layers.Count-1].Length); //количество выходов
        
        //веса
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }

        //отклонения
        foreach (Neuron[] layer in _layers)
        {
            foreach (Neuron neuron in layer)
            {
                writer.WriteLine(neuron.GetBias());
            }
        }

        writer.Close();
    }
}
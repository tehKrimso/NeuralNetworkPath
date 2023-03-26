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

    //outdated
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

    //outdated
    public void TrainingLoop()
    {
        
    }

    public List<Neuron[]> GetLayers() => _layers;
    public float[][][] GetWeights() => weights;

    public void FeedForward(float[] inputs)
    {
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

        //смещения
        foreach (Neuron[] layer in _layers)
        {
            foreach (Neuron neuron in layer)
            {
                writer.WriteLine(neuron.GetBias());
            }
        }

        writer.Close();
    }
    
    public void Load(string path)
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int) new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }

        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < _biases.Length; i++)
            {
                for (int j = 0; j < _biases[i].Length; j++)
                {
                    _biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < _weightsOld.Length; i++)
            {
                for (int j = 0; j < _weightsOld[i].Length; j++)
                {
                    for (int k = 0; k < _weightsOld[i][j].Length; k++)
                    {
                        _weightsOld[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }
    }
    
    
    //OLD CODE//

    //feed forward, inputs >==> outputs.

    public float[] FeedForwardOld(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            _neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < _layersOld.Length; i++)
        {
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < _neurons[i - 1].Length; k++)
                {
                    value += _weightsOld[i - 1][j][k] * _neurons[i - 1][k];
                }
                _neurons[i][j] = Activate(value + _biases[i][j]);
            }
        }
        return _neurons[_neurons.Length - 1];
    }

    //old constructor
    
    public NeuralNetwork(int[] layers)
    {
        _layersOld = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            _layersOld[i] = layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();
    }


    private float Activate(float value)
    {
        return (float) Math.Tanh(value);
    }

    public void MutateOld(int chance, float val)
    {
        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                _biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5)
                    ? _biases[i][j] += UnityEngine.Random.Range(-val, val)
                    : _biases[i][j];
            }
        }

        for (int i = 0; i < _weightsOld.Length; i++)
        {
            for (int j = 0; j < _weightsOld[i].Length; j++)
            {
                for (int k = 0; k < _weightsOld[i][j].Length; k++)
                {
                    _weightsOld[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5)
                        ? _weightsOld[i][j][k] += UnityEngine.Random.Range(-val, val)
                        : _weightsOld[i][j][k];
                }
            }
        }
    }

    public NeuralNetwork Copy(NeuralNetwork nn) //For creatinga deep copy, to ensure arrays are serialzed.
    {
        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                nn._biases[i][j] = _biases[i][j];
            }
        }

        for (int i = 0; i < _weightsOld.Length; i++)
        {
            for (int j = 0; j < _weightsOld[i].Length; j++)
            {
                for (int k = 0; k < _weightsOld[i][j].Length; k++)
                {
                    nn._weightsOld[i][j][k] = _weightsOld[i][j][k];
                }
            }
        }

        return nn;
    }

    //create empty storage array for the neurons in the network.

    public void LoadOld(string path)
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int) new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }

        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < _biases.Length; i++)
            {
                for (int j = 0; j < _biases[i].Length; j++)
                {
                    _biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < _weightsOld.Length; i++)
            {
                for (int j = 0; j < _weightsOld[i].Length; j++)
                {
                    for (int k = 0; k < _weightsOld[i][j].Length; k++)
                    {
                        _weightsOld[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }
    }

    public void SaveOld(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                writer.WriteLine(_biases[i][j]);
            }
        }

        for (int i = 0; i < _weightsOld.Length; i++)
        {
            for (int j = 0; j < _weightsOld[i].Length; j++)
            {
                for (int k = 0; k < _weightsOld[i][j].Length; k++)
                {
                    writer.WriteLine(_weightsOld[i][j][k]);
                }
            }
        }

        writer.Close();
    }

    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
            return 1;
        if (Fitness > other.Fitness)
            return 1;
        else if (Fitness < other.Fitness)
            return -1;
        else
            return 0;
    }

    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < _layersOld.Length; i++)
        {
            neuronsList.Add(new float[_layersOld[i]]);
        }

        _neurons = neuronsList.ToArray();
    }

    //initializes and populates array for the biases being held within the network.


    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < _layersOld.Length; i++)
        {
            float[] bias = new float[_layersOld[i]];
            for (int j = 0; j < _layersOld[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f); // вынести константой
            }

            biasList.Add(bias);
        }

        _biases = biasList.ToArray();
    }

    //initializes random array for the weights being held in the network.


    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < _layersOld.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = _layersOld[i - 1];
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        _weightsOld = weightsList.ToArray();
    }

    //Comparing For NeuralNetworks performance.


    //this loads the biases and weights from within a file into the neural network.
}
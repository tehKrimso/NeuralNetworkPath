using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    public bool UsePresavedData;//load presaved values flag
    
    [Header("Main Params")] 
    public int LearningAreaCount = 3;
    public int AgentsInGroupCount = 5;
    public float IterationTime = 15f; // time for one learning iteration before restart

    public string SavePath = "Assets/SavedData.txt";

    public GameObject ArenaPrefab; //arena prefab
    public GameObject AgentPrefab; //robotic agent model prefab

    [Header("Genetic Algorithm Params")] 
    [Range(0.001f, 1f)]
    public float MutationChance = 0.01f;
    public float MutationDelta = 0.5f; // force of mutation
    
    
    [Header("Neural Network Configuration")]
    public int HiddenLayersCount = 3;
    public float WeightsInitialValue = 0.5f;
    public float BiasesInitialValue = 0.5f;

    private int _inputNeuronsCount; // количество входов - лучи лазерного дальномера агентов, по 5 на агента
    private int _hiddenLayerNeuronCount; // нейроны в скрытых слоях
    private int _outputNeuronCount; // количество выходов - движение вперед/назад, поворот влево/вправо, по 2 на агента
    
    private List<NeuralNetwork> _neuralNetworks = new List<NeuralNetwork>();

    private List<LearningAreaManager> _areaManagers = new List<LearningAreaManager>();

    private bool _isTimerCounting;
    
    private float _timer;
    private int _iterationCount;
    
    private void Start()
    {
        //_inputNeuronsCount = AgentsInGroupCount * 5;
        _inputNeuronsCount = AgentsInGroupCount * 8;
        _outputNeuronCount = AgentsInGroupCount * 2;

        _hiddenLayerNeuronCount = _inputNeuronsCount - _outputNeuronCount;

        for (int i = 0; i < LearningAreaCount; i++)
        {
            GameObject learningArea = Instantiate(ArenaPrefab, new Vector3(50 * i, 0, 0), Quaternion.Euler(-90,0,0));
            LearningAreaManager areaManager = learningArea.GetComponent<LearningAreaManager>();
            
            NeuralNetwork neuralNetwork = new NeuralNetwork(
                _inputNeuronsCount, _outputNeuronCount, _hiddenLayerNeuronCount, 
                HiddenLayersCount, WeightsInitialValue, BiasesInitialValue
            );
            
            areaManager.Construct(neuralNetwork, AgentPrefab,AgentsInGroupCount);
            
            _neuralNetworks.Add(neuralNetwork);
            _areaManagers.Add(areaManager);
        }
        
        //
        //дополнительные инициализации
        //
        
        StartIteration();
        
        //InvokeRepeating("Test", 0.1f, IterationTime);

    }

    private void Test()
    {
        EndLearningIteration();
        
        _iterationCount++;
        Debug.Log($"Iteration #{_iterationCount} started.");
        
        foreach (LearningAreaManager areaManager in _areaManagers)
        {
            areaManager.SpawnAgents();
            areaManager.StartLearning();
        }
    }

    private void Update()
    {
        if(_isTimerCounting)
            _timer += Time.deltaTime;
        
        if (IterationTime - _timer > 0)
            return;

        _timer = 0;
        
        EndLearningIteration();
        StartIteration();
    }

    private void StartIteration()
    {
        _isTimerCounting = true;
        _iterationCount++;
        Debug.Log($"Iteration #{_iterationCount} started.");
        
        foreach (LearningAreaManager areaManager in _areaManagers)
        {
            areaManager.SpawnAgents();
            areaManager.StartLearning();
        }
    }

    private void EndLearningIteration()
    {
        _isTimerCounting = false;
        //отбор нейронок по максимальной функциии
        //скрещивание путем обмена половинами
        //мутация весов и байосов
        
        for (int i = 0; i < _areaManagers.Count; i++)
        {
            _areaManagers[i].CleanUp();
        }
        
        
        var list = _neuralNetworks.OrderByDescending(x => x.Fitness).ToList();
        
        NeuralNetwork bestNetwork = list[0];
        NeuralNetwork secondBestNetwork = list[1];
        
        bestNetwork.Save(SavePath);

        //скрещиваю две лучших
        for (int i = 0; i < _neuralNetworks.Count; i++)
        {
            //_neuralNetworks[i] = new NeuralNetwork(bestNetwork, secondBestNetwork);
            _neuralNetworks[i] = CrossNetworks(bestNetwork, bestNetwork);
        }

        //мутация весов и отклонений
        foreach (NeuralNetwork neuralNetwork in _neuralNetworks)
        {
            neuralNetwork.Mutate(MutationChance,MutationDelta);
        }

        for (int i = 0; i < _areaManagers.Count; i++)
        {
            _areaManagers[i].Construct(_neuralNetworks[i], AgentPrefab,AgentsInGroupCount);
        }
        
        // foreach (LearningAreaManager areaManager in _areaManagers)
        // {
        //     areaManager.CleanUp();
        // }

        _timer = 0;
        Debug.Log($"Iteration #{_iterationCount} ended.");
    }

    //должно быть здесь
    private void SaveData(NeuralNetwork neuralNetwork)
    {
        
    }

    private NeuralNetwork CrossNetworks(NeuralNetwork first, NeuralNetwork second)
    {
        NeuralNetwork newNetwork = new NeuralNetwork(
            _inputNeuronsCount, _outputNeuronCount, _hiddenLayerNeuronCount, 
            HiddenLayersCount, WeightsInitialValue, BiasesInitialValue
        );


        var halfValue =  newNetwork.weights.Length / 2;
        
        for (int i = 0; i < halfValue + 1; i++)
        {
            for (int j = 0; j < newNetwork.weights[i].Length; j++)
            {
                for (int k = 0; k < newNetwork.weights[i][j].Length; k++)
                {
                    newNetwork.weights[i][j][k] = first.weights[i][j][k];
                }
            }
        }
        
        for (int i = halfValue+1; i < newNetwork.weights.Length; i++)
        {
            for (int j = 0; j < newNetwork.weights[i].Length; j++)
            {
                for (int k = 0; k < newNetwork.weights[i][j].Length; k++)
                {
                    newNetwork.weights[i][j][k] = second.weights[i][j][k];
                }
            }
        }

        var firstLayers = first.GetLayers();
        var secondLayers = second.GetLayers();
        
        var newLayers = newNetwork.GetLayers();

        for (int i = 0; i < newLayers.Count; i++)
        {
            for (int j = 0; j < newLayers[i].Length; j++)
            {
                newLayers[i][j].SetBias(firstLayers[i][j].GetBias());
            }
        }

        return newNetwork;
    }
}
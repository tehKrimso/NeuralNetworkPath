using System;
using System.Collections.Generic;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    public bool UsePresavedData;//load presaved values flag
    
    [Header("Main Params")] 
    public int LearningAreaCount = 3;
    public int AgentsInGroupCount = 5;
    public float IterationTime = 15f; // time for one learning iteration before restart

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

    private float _timer;

    private int _iterationCount;
    
    private void Start()
    {
        _inputNeuronsCount = AgentsInGroupCount * 5;
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

    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (IterationTime - _timer > 0)
            return;


        EndLearningIteration();
        StartIteration();
    }

    private void StartIteration()
    {
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
        
        //отбор нейронок по максимальной функциии
        //скрещивание путем обмена половинами
        //мутация весов и байосов
        
        foreach (LearningAreaManager areaManager in _areaManagers)
        {
            areaManager.CleanUp();
        }

        _timer = 0;
        Debug.Log($"Iteration #{_iterationCount} ended.");
    }
}
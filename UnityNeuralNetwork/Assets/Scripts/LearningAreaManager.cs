using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using UnityEngine;

public class LearningAreaManager : MonoBehaviour
{
    public Transform StartPositionMarker;

    private GameObject _agentPrefab;
    private int _agentsCount;

    private bool _useSavedData;
    
    private bool _isLearning;
    private NeuralNetwork _network;
    private List<Agent> _agents = new List<Agent>();//список агентов в одной системе

    private float[] _outputs;

    public void Construct(NeuralNetwork neuralNetwork, GameObject agentPrefab, int agentsCount, bool useSavedData = false)
    {
        _useSavedData = useSavedData;
        
        _agentPrefab = agentPrefab;
        _agentsCount = agentsCount;
        
        _network = neuralNetwork;
        
        _outputs = new float[agentsCount * 2];//изменить инициализацию
        
        
    }

    private void Update()
    {
        if (_useSavedData)
        {
            NetworkLoop();
            return;
        }
        
        if (_isLearning)
        {
            ExecuteNetworkLoop();//собираем инпуты с агентов и отправляем в нейронку
        }
    }

    private void ExecuteNetworkLoop()
    {
        NetworkLoop();

        //оценка
        int score = 0;
        for (int i = 0; i < _agents.Count; i++)
        {
            int agentScore = _agents[i].GetScore();

            if (agentScore > score)
            {
                score = agentScore;
            }
        }
        
        //проверять, что новая оценка больше текущей?
        _network.Fitness = score;
    }

    private void NetworkLoop()
    {
        //сбор инпутов сделать отдельным методом
        List<float> inputs = new List<float>(); //константу вынести
        foreach (Agent agent in _agents)
        {
            float[] agentSensorData = agent.GetSensorData(); //Прописать метод
            inputs.AddRange(agentSensorData);
        }
        //

        //отправка инпутов в сеть и получение выходных значение
        _network.FeedForward(inputs.ToArray()); //отправляем данные с сенсоров на вход сети
        _outputs = _network.GetOutputs();
        //

        //отправка выходных значений агентам
        for (int i = 0; i < _outputs.Length; i += 2)
        {
            _agents[i / 2].SetMoveValues(_outputs[i], _outputs[i + 1]);
        }
        //
    }

    public void SpawnAgents() // спавн агентов в стартовой позиции
    {
        for (int i = 0; i < _agentsCount; i++)
        {
            GameObject agent = Instantiate(_agentPrefab, StartPositionMarker.position + new Vector3(i*3f,0,0), Quaternion.Euler(0,90,0));
            agent.transform.SetParent(StartPositionMarker);
            
            Agent agentController = agent.GetComponent<Agent>();
            agentController.network = _network;
            _agents.Add(agentController);
        }
    }

    public void StartLearning()//установка флага
    {
        _isLearning = true;
    }

    public void CleanUp()//очистка треков от агентов и ресчет точек интереса
    {
        _isLearning = false;

        for (int i = 0; i < _agents.Count; i++)
        {
            Agent agent = _agents[i];
            Destroy(agent.gameObject);
        }
        
        _agents.Clear();
    }
}
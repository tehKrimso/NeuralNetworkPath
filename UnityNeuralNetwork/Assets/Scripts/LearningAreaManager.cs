using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using UnityEngine;

public class LearningAreaManager : MonoBehaviour
{
    private bool _isLearning;
    private NeuralNetwork _network;
    private List<Agent> _agents = new List<Agent>();//список агентов в одной системе

    private float[] _outputs;

    public void Construct(NeuralNetwork neuralNetwork)
    {
        _network = neuralNetwork;
        
        _outputs = new float[_agents.Count * 2];//изменить инициализацию
    }

    private void Update()
    {
        if (_isLearning)
        {
            ExecuteNetworkLoop();//собираем инпуты с агентов и отправляем в нейронку
        }
    }

    private void ExecuteNetworkLoop()
    {
        //сбор инпутов
        List<float> inputs = new List<float>();//константу вынести
        foreach (Agent agent in _agents)
        {
            float[] agentSensorData = agent.GetSensorData();//Прописать метод
            inputs.AddRange(agentSensorData);
        }
        //
        
        //отправка инпутов в сеть и получение выходных значение
        _network.FeedForward(inputs.ToArray());//отправляем данные с сенсоров на вход сети
        _outputs = _network.GetOutputs();
        //
        
        //отправка выходных значений агентам
        for (int i = 0; i < _outputs.Length; i += 2)
        {
            for (int j = 0; j < _agents.Count; j++)
            {
                _agents[j].SetMoveValues(_outputs[i], _outputs[i + 1]);//движение агентов вызывать отсюда же
            }
        }
        //
    }

    public void CleanUp()//очистка треков от агентов и ресчет точек интереса
    {
        throw new System.NotImplementedException();
        _isLearning = false;
    }

    public void SpawnAgents() // спавн агентов в стартовой позиции
    {
        throw new System.NotImplementedException();
    }

    public void StartLearning()//установка флага
    {
        throw new System.NotImplementedException();
        _isLearning = true;
    }
}
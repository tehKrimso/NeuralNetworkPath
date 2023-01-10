using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class LearningAreaManager : MonoBehaviour
{
    private bool _isLearning;
    private NeuralNetwork _network;
    private List<Agent> _agents = new List<Agent>();//список агентов в одной системе

    public void Construct(NeuralNetwork neuralNetwork)
    {
        _network = neuralNetwork;
    }

    private void Update()
    {
        if (_isLearning)
        {
            foreach (Agent agent in _agents)
            {
                //делать действия агентов
            }
        }
    }

    public void CleanUp()//очистка треков от агентов и ресет точек интереса
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
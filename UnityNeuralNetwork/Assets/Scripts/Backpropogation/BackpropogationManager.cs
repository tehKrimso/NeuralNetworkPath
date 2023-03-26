using System.Collections.Generic;
using System;
using UnityEngine;

public class BackpropogationManager : MonoBehaviour
{
    private BackporpogationNeuralNetwork _net;
    int[] layers = new int[3]{ 3, 5, 1 };
    string[] activation = new string[2] { "leakyrelu", "leakyrelu" };

    void Start()
    {
        _net = new BackporpogationNeuralNetwork(layers, activation);
        for (int i = 0; i < 20000; i++)
        {
            _net.BackPropagate(new float[] { 0, 0, 0 },new float[] { 0 });
            _net.BackPropagate(new float[] { 1, 0, 0 },new float[] { 1 });
            _net.BackPropagate(new float[] { 0, 1, 0 },new float[] { 1 });
            _net.BackPropagate(new float[] { 0, 0, 1 },new float[] { 1 });
            _net.BackPropagate(new float[] { 1, 1, 0 },new float[] { 1 });
            _net.BackPropagate(new float[] { 0, 1, 1 },new float[] { 1 });
            _net.BackPropagate(new float[] { 1, 0, 1 },new float[] { 1 });
            _net.BackPropagate(new float[] { 1, 1, 1 },new float[] { 1 });
        }
        print("cost: "+_net.cost);
        
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 0, 0, 0 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 1, 0, 0 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 0, 1, 0 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 0, 0, 1 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 1, 1, 0 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 0, 1, 1 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 1, 0, 1 })[0]);
        UnityEngine.Debug.Log(_net.FeedForward(new float[] { 1, 1, 1 })[0]);
    }
}
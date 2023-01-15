using System;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    private List<Neuron> _inputs;
    private List<Neuron> _outputs;

    private float _value;
    //private float _weight;
    private float _bias;

    public Neuron()
    {
        _inputs = new List<Neuron>();
        _outputs = new List<Neuron>();
    }
    
    public void AddInput(Neuron formNeuron) => _inputs.Add(formNeuron);
    public void AddOutput(Neuron toNeuron) => _outputs.Add(toNeuron);
    //public void SetWeight(float weight) => _weight = weight;
    public void SetBias(float biasValue) => _bias = biasValue;
    public void SetValue(float value) => _value = value;
    public float GetValue() => _value;
    public float GetActivationValue() => Activation(_value + _bias);
    public float GetBias() => _bias;
    public float Activation(float value) => (float)Math.Tanh(value);
}
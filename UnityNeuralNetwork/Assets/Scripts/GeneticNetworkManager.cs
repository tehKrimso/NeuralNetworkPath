using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneticNetworkManager : MonoBehaviour
{
    public float timeframe;
    public int populationSize;//creates population size
    public GameObject prefab;//holds bot prefab

    public bool UsePresavedData;

    public int[] layers = new int[3] { 5, 3, 2 };

    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Range(0.1f, 10f)] public float Gamespeed = 1f;
    
    public List<NeuralNetwork> Networks;

    private List<Agent> _agents;

    private void Start()// Start is called before the first frame update
    {
        _agents = new List<Agent>();
        
        if (populationSize % 2 != 0)
            populationSize = 50;//if population size is not even, sets it to fifty

        InitNetworks();
        InvokeRepeating("CreateBots", 0.1f, timeframe);
    }

    private void Update()
    {
        SetTimeScale();
    }

    public void InitNetworks()
    {
        Networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            
            if(UsePresavedData)
                net.LoadOld("Assets/Pre-trained.txt");//on start load the network save
            
            Networks.Add(net);
        }
    }

    public void CreateBots()
    {
        
        if (_agents.Count>0)
        {
            for (int i = 0; i < populationSize; i++)
            {
                _agents[i].UpdateFitness();
            }
            
            
            for (int i = 0; i < _agents.Count; i++)
            {
                Destroy(_agents[i].gameObject);
            }
            
            SortNetworks();
            
            _agents.Clear();
        }
        
        for (int i = 0; i < populationSize; i++)
        {
            Agent agent = (Instantiate(prefab, new Vector3(0, 0.6f, -16), new Quaternion(0, 0, 0, 0))).GetComponent<Agent>();//create agents
            agent.network = Networks[i];//deploys network to each learner
            _agents?.Add(agent);
        }
        
    }

    public void SortNetworks()
    {
        Networks.Sort();
        Networks[populationSize - 1].SaveOld("Assets/Save.txt");
        for (int i = 0; i < populationSize / 2; i++)
        {
            Networks[i] = Networks[i + populationSize / 2].Copy(new NeuralNetwork(layers));
            Networks[i].MutateOld((int)(1/MutationChance), MutationStrength);
        }
    }
    
    private void SetTimeScale() => Time.timeScale = Gamespeed;
}
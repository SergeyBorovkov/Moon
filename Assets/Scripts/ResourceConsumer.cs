using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceConsumer : MonoBehaviour
{
    [SerializeField] private ResourceProducer _producer;
    [SerializeField] private List<ResourceCluster> _clusters;

    public event Action ReadyForProduction;           
    public IReadOnlyList<ResourceCluster> Clusters => _clusters;    
    public bool IsEnoughForProduction => _clusters.TrueForAll(c => c.CurrentResources > 0);   

    private void Awake()
    {        
        ShowResourceName();        
    }
    
    private void OnEnable()
    {        
        foreach (var cluster in _clusters)        
            cluster.CurrentResourceChanged += OnCurrentResourceChanged;        
    }

    private void OnDisable()
    {
        foreach (var cluster in _clusters)        
            cluster.CurrentResourceChanged -= OnCurrentResourceChanged;        
    }

    private void OnCurrentResourceChanged()
    {
        _producer.ReportMessages();

        if (IsEnoughForProduction)      
            ReadyForProduction?.Invoke();
    }

    public int GetResourcesForProduction()
    {
        int value = _clusters[0].CurrentResources;
        
        foreach (var cluster in _clusters)
        {
            if (cluster.CurrentResources < value)
                value = cluster.CurrentResources;
        }        

        return value;
    }

    public void RemoveOneResource()
    {
        if (IsEnoughForProduction)
        {
            foreach (var cluster in _clusters)            
                cluster.RemoveOneResource();            
        }
        else
        {
            print("Not enough resources for production");
        }
    }

    public string OutputRequiredResourceNames()
    {
        string outputMessage = "";
        var list = new List<string>();

        foreach (var cluster in _clusters)
        {
            if (cluster.CurrentResources <= 0)
                list.Add(cluster.Resource.Name);
        }

        if (list.Count == 0)
        {
            outputMessage = "All resources are available";
        }
        else
        {
            foreach (var item in list)
            {
                string spacer = ", ";

                outputMessage += list.Count > 0 ? item + spacer : item;
            }
        }

        return outputMessage;        
    }    

    public void ShowResourceName()
    {
        foreach (var cluster in _clusters)        
            cluster.ChangeMessage(cluster.Resource.Name);        
    }
}
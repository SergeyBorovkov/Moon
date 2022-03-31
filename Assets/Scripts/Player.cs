using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{    
    [SerializeField] private float _transferSpeed;
    [SerializeField] private List<ResourceCluster> _clusters;
    [SerializeField] private PlayerController _controller;
    private event Action _transferringStopped;
    private ResourceProducer _producer;
    private ResourceConsumer _consumer;
    private Coroutine _downloadJob;
    private Coroutine _uploadJob;    

    public event Action VisitEnded;
    public event Action VisitStarted;
    public ResourceCluster InteractionCluster;
    
    public ResourceProducer Producer => _producer;
    public ResourceConsumer Consumer => _consumer;  
    
    private void Start()
    {
        ReportMessages();
    }

    private void OnEnable()
    {
        _transferringStopped += OnTransferringStopped;
    }

    private void OnDisable()
    {
        _transferringStopped -= OnTransferringStopped;
    }

    private void OnTransferringStopped()
    {
        if (_consumer != null)
        {
            Destroy(ref _uploadJob);
            UploadResourcesTo(_consumer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ReportMessages();

        if (other.gameObject.TryGetComponent<ResourceProducer>(out ResourceProducer producer))
        {            
            _producer = producer;
            _consumer = null;
            
            VisitStarted?.Invoke();

            Destroy(ref _uploadJob);

            DownloadResourcesFrom(_producer);
        }
        else if (other.gameObject.TryGetComponent<ResourceConsumer>(out ResourceConsumer consumer))
        {
            _consumer = consumer;
            _producer = null;

            VisitStarted?.Invoke();

            Destroy(ref _downloadJob);

            UploadResourcesTo(_consumer);         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ReportMessages();

        _consumer = null;
        _producer = null;
        InteractionCluster = null;
        
        VisitEnded?.Invoke();

        Destroy(ref _downloadJob);
        Destroy(ref _uploadJob);
    }

    private void UploadResourcesTo(ResourceConsumer consumer)
    {
        var consumerClusters = consumer.Clusters;

        for (int i = 0; i < consumerClusters.Count; i++)
        {
            var toConsumerCluster = consumerClusters[i];            

            var fromPlayerCluster = _clusters.FirstOrDefault(c => c.Resource == toConsumerCluster.Resource);

            bool baseCondition = fromPlayerCluster != null && _uploadJob == null && fromPlayerCluster.CurrentResources > 0 && 
                toConsumerCluster.CurrentResources < toConsumerCluster.MaxResources;

            if (baseCondition)
            {
                _uploadJob = StartCoroutine(TransferResources(toConsumerCluster, fromPlayerCluster, _transferSpeed));
                InteractionCluster = toConsumerCluster;

                
            }
        }
    }

    private void DownloadResourcesFrom(ResourceProducer producer)
    {   
        var toPlayerCluster = _clusters.FirstOrDefault(c => c.Resource == producer.Cluster.Resource);
        var fromProducerCluster = producer.Cluster;

        bool baseCondition = _downloadJob == null && fromProducerCluster.CurrentResources > 0 &&
                toPlayerCluster.CurrentResources < toPlayerCluster.MaxResources;

        if (baseCondition)
        {
            _downloadJob = StartCoroutine(TransferResources(toPlayerCluster, fromProducerCluster, _transferSpeed));
            InteractionCluster = fromProducerCluster;
        }
    }

    private IEnumerator TransferResources(ResourceCluster toCluster, ResourceCluster fromCluster, float speed)
    {
        float currentValue = toCluster.CurrentResources;

        float targetValue = Math.Min(toCluster.MaxResources, toCluster.CurrentResources + fromCluster.CurrentResources);

        while (currentValue < targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);

            if (currentValue >= toCluster.CurrentResources + 1)
            {
                fromCluster.RemoveOneResource();
                toCluster.AddOneResource();
            }

            yield return null;
        }

        if (toCluster.CurrentResources == toCluster.MaxResources || fromCluster.CurrentResources == 0)
            _transferringStopped?.Invoke();
    }

    private void Destroy(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private void ReportMessages()
    {
        foreach (var cluster in _clusters)
        {
            if (cluster.CurrentResources == cluster.MaxResources)
                cluster.ChangeMessage("Inventory is full");
            else if (cluster.CurrentResources == 0)
                cluster.ChangeMessage("Inventory is empty");
            else
                cluster.ChangeMessage("...");
        }
    }
}
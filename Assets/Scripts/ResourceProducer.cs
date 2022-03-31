using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private ResourceConsumer _consumer;
    [SerializeField] private ResourceCluster _cluster;
    [SerializeField] private float _productionSpeed;    
    private Coroutine _produceJob;    
    public ResourceCluster Cluster => _cluster;

    private void OnEnable()
    {
        _cluster.CurrentResourceChanged += OnCurrentResourceChanged;
        _player.VisitEnded += OnPlayerVisitEnded;
        _player.VisitStarted += OnPlayerVisitStarted;
        
        if (_consumer != null)        
            _consumer.ReadyForProduction += OnConsumerReadyForProduction;
    }

    private void OnDisable()
    {
        _cluster.CurrentResourceChanged -= OnCurrentResourceChanged;
        _player.VisitEnded -= OnPlayerVisitEnded;
        _player.VisitStarted += OnPlayerVisitStarted;
        
        if (_consumer != null)
            _consumer.ReadyForProduction -= OnConsumerReadyForProduction;
    }

    private void Start()
    {
        ProduceResources();
    }

    private void OnConsumerReadyForProduction()
    {     
        ProduceResources();     
    }

    private void OnPlayerVisitEnded()
    {
        ProduceResources();
    }

    private void OnPlayerVisitStarted()
    {
        if (this == _player.Producer || _consumer == _player.Consumer)
            Destroy(ref _produceJob);
    }

    private void OnCurrentResourceChanged()
    {
        Destroy(ref _produceJob);
        ProduceResources();        
    }

    private void ProduceResources()
    {
        bool baseCondition = _cluster.CurrentResources < _cluster.MaxResources && this != _player.Producer;

        if (_consumer == null)
        {
            if (baseCondition && _produceJob == null)            
                _produceJob = StartCoroutine(ProduceResources(_productionSpeed));            
            else if (!baseCondition)            
                Destroy(ref _produceJob);            
        }
        else if (_consumer != null)
        {
            if (baseCondition && _consumer.IsEnoughForProduction && _produceJob == null && _consumer != _player.Consumer)            
                _produceJob = StartCoroutine(ProduceResources(_productionSpeed));            
            else if (_produceJob != null && (!baseCondition || !_consumer.IsEnoughForProduction || _consumer == _player.Consumer))            
                Destroy(ref _produceJob);                        
        }

        ReportMessages();
    }

    private IEnumerator ProduceResources(float speed)
    {
        float currentValue = _cluster.CurrentResources;
        int targetValue = _consumer == null ? _cluster.MaxResources : Math.Min(_cluster.MaxResources, _cluster.CurrentResources + _consumer.GetResourcesForProduction());
        
        while (currentValue < targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);

            if (currentValue >= _cluster.CurrentResources + 1)
            {
                if (_consumer != null)
                {                
                    _consumer.RemoveOneResource();
                    _cluster.AddOneResource();
                }
                else if (_consumer == null)
                {
                    _cluster.AddOneResource();
                }
            }

            yield return null;            
        }
    }

    private void Destroy(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void ReportMessages()
    {
        string additionMessage = "";
        
        if (_consumer != null && !_consumer.IsEnoughForProduction)        
            additionMessage = "Need resources: " + _consumer.OutputRequiredResourceNames();        

        if (_cluster.CurrentResources == _cluster.MaxResources)
            _cluster.ChangeMessage("Stock is full");
        else if (_cluster.CurrentResources == 0)
            _cluster.ChangeMessage("Stock is empty! " + additionMessage);
        else
            _cluster.ChangeMessage("..." + additionMessage);      
    }    
}
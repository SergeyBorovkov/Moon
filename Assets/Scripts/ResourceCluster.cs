using System;
using UnityEngine;

public class ResourceCluster : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private int _currentResources;
    [SerializeField] private int _maxResources;
    private string _message;
    private int _totalMax = 100; 

    public event Action CurrentResourceChanged;
    public event Action MessageChanged;
    public Resource Resource => _resource;
    public int MaxResources => _maxResources;
    public int CurrentResources => _currentResources;
    public string Message => _message;

    private void Start()
    {
        _maxResources = Mathf.Clamp(_maxResources, 0, _totalMax);
        _currentResources = Mathf.Clamp(_currentResources, 0, _maxResources);
    }

    public void AddOneResource()
    {
        if (_currentResources + 1 <= _maxResources)
        {
            _currentResources++;
            CurrentResourceChanged?.Invoke();
        }
        else
        {
            print("Trying to add too much!");
        }
    }

    public void RemoveOneResource()
    {
        if (_currentResources - 1 >= 0)
        {
            _currentResources--;
            CurrentResourceChanged?.Invoke();            
        }
        else
        {
            print("Trying to remove too much!");
        }
    }      

    public void ChangeMessage (string message)
    {
        _message = message;
        MessageChanged?.Invoke();
    }
}
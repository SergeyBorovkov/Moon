using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceClusterInformer : MonoBehaviour
{
    [SerializeField] private ResourceCluster _cluster;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private TMP_Text _notice;    

    private void Start()
    {
        ShowResourceValues();
        ShowMessage();
    }

    private void OnEnable()
    {
        _cluster.CurrentResourceChanged += OnCurrentResourceChanged;
        _cluster.MessageChanged += OnMessageChanged;
    }


    private void OnDisable()
    {
        _cluster.CurrentResourceChanged -= OnCurrentResourceChanged;
        _cluster.MessageChanged -= OnMessageChanged;
    }

    private void OnMessageChanged()
    {
        ShowMessage();
    }

    private void OnCurrentResourceChanged()
    {
        ShowResourceValues();        
    }

    private void ShowResourceValues()
    {
        string currentResources = _cluster.CurrentResources.ToString();
        string maxResources = _cluster.MaxResources.ToString();
        string totalString = currentResources + "/" + maxResources;

        _status.text = totalString;
    }

    private void ShowMessage()
    {
        _notice.text = _cluster.Message;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private int _weight;

    public int Weight => _weight;

    public string Name => _name;

    private void Awake()
    {
        if (_name == "")
            print("The Resource shoud be named properly!");

        if (_weight <= 0)
            print($"Check the {Name} Resource weight! It shoud be positive value!");
    }
}
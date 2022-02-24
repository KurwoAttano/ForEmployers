using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    private Prefabs Prefabs;

    // STATE
    public bool IsNode;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        Prefabs = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterHandler>().Prefabs;
    }

    private void Update()
    {
        
    }
}

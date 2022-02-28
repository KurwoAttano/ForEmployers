using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private GameObject _character;
    private const string _characterTag = "Player";

    private float _camDistance = 8f;
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _character = GameObject.FindWithTag(_characterTag);
    }

    private void Update()
    {
        transform.position = new Vector3(_camDistance, _character.transform.position.y, _character.transform.position.z);
    }
}

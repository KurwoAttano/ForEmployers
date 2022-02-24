using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : MonoBehaviour
{
    private CharacterHandler _character;
    private List<Transform> _elemList = new List<Transform>();

    private bool _isInitialized = false;
    private bool _isConnectedToWall = false;

    // ROTATION
    //private bool _parentInitialized = false;
    private Transform _parent;
    private Transform _level;

    // LINERENDERER
    private LineRenderer _lr;
    private const float _lrWidth = 0.1f;

    // DESTRUCTION
    private bool _isDestroyed = false;
    private int _destrFrameDelay = 100;
    private float _destrCharge = 1f;
    private float _destrStep = 0.01f;

    public void Initialize(CharacterHandler character, Transform level)
    {
        _character = character;
        _parent = transform.parent;
        _level = level;

        _lr = gameObject.AddComponent<LineRenderer>();
        _lr.startWidth = _lrWidth;
        _lr.endWidth = _lrWidth;
        _lr.SetPosition(0, transform.position);
        _lr.material = _character.Prefabs.RopeMaterial;

        _isInitialized = true;
    }

    private void Update()
    {
        RotateLogic();
        LineRendererLogic();
        DestractionLogic();
    }

    private void RotateLogic()
    {
        if (_isInitialized && _isConnectedToWall)
        {
            //if (!_parentInitialized)
                _parent = transform.parent;
            _parent.position = _level.position;
            _parent.rotation = _level.rotation;
        }
    }

    private void LineRendererLogic()
    {
        _lr.SetPosition(0, transform.position);
        for (int i = 0; i < _elemList.Count; i++)
            _lr.SetPosition(i + 1, _elemList[i].position);
    }
    private void DestractionLogic()
    {
        if (_isDestroyed)
        {
            _destrFrameDelay--;
            if (_destrFrameDelay <= 0)
            {
                _destrCharge -= _destrStep;
                _lr.material.color = new Color(1, 1, 1, _destrCharge);
                if (_destrCharge <= 0)
                    Destroy(transform.parent.gameObject);
            }
        }
    }

    public void AddElem(Transform elem)
    {
        _elemList.Add(elem);
        _lr.positionCount = _elemList.Count + 1;
    }

    private void Connect()
    {
        if (_isInitialized)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            _character.ConnectRope();
            _isConnectedToWall = true;
            Debug.Log("Rope is connected"); //!
        }
        else Debug.LogError("RopeEnd not initialized, can't Connect in the CharacterHandler");
    }
    public void DestroyRope()
    {
        _isDestroyed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isConnectedToWall) Connect();
    }
}

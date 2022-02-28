using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    public Transform Connector;
    public Transform Crown;
    public Transform Pin;

    // STATE
    [Space(10)]
    public bool State = false;
    private bool _currentState = false;

    // GLOBAL
    private bool _isMoving = false;
    private bool _isComplete = true;

    // START POS
    private float _crownStartPos = 0.15f;
    private float _pinStartPos = 0.05f;

    // LENGTH
    [Space(10)]
    public float MaxLength = 1f;
    private float _currentLength = 0f;

    // SPEED
    public float Speed = 0.03f;
    public float Acceleration = 0.1f;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        if (State != _currentState)
        {
            _currentState = State;
            Crown.localPosition = new Vector3(0, _crownStartPos + MaxLength, 0);
            Pin.localPosition = new Vector3(0, _pinStartPos + MaxLength / 2, 0);
            Pin.localScale = new Vector3(Pin.localScale.x, MaxLength / 2, Pin.localScale.z);
        }
    }

    private void FixedUpdate()
    {
        CalcLogic();
        MovementLogic();
    }

    private void CalcLogic()
    {
        if (State != _currentState)
        {
            _currentState = State;
            _isMoving = true;
            _isComplete = false;
        }
        if (_isMoving)
        {
            if (_currentState)
            {
                if (_currentLength < MaxLength)
                    _currentLength += Speed;
                else
                {
                    _currentLength = MaxLength;
                    _isMoving = false;
                }
            }
            else
            {
                if (_currentLength > 0)
                    _currentLength -= Speed;
                else
                {
                    _currentLength = 0;
                    _isMoving = false;
                }
            } 
        }
    }
    private void MovementLogic()
    {
        if (!_isComplete)
        {
            Crown.localPosition = Vector3.Lerp(Crown.localPosition, new Vector3(0, _crownStartPos + _currentLength, 0), Acceleration);
            Pin.localPosition = Vector3.Lerp(Pin.localPosition, new Vector3(0, _pinStartPos + _currentLength / 2, 0), Acceleration);
            Pin.localScale = Vector3.Lerp(Pin.localScale, new Vector3(Pin.localScale.x, _currentLength / 2, Pin.localScale.z), Acceleration);

            if (Crown.localPosition == new Vector3(0, _crownStartPos + _currentLength, 0)) 
                _isComplete = true;
        }
    }

    [ContextMenu("MoveOut")]
    public void MoveOut()
    {
        State = true;
        _currentState = true;

        _isMoving = true;
        _isComplete = false;
        //_dirToOut = true;
    }
    [ContextMenu("MoveIn")]
    public void MoveIn()
    {
        State = false;
        _currentState = false;

        _isMoving = true;
        _isComplete = false;
        //_dirToOut = false;
    }

}

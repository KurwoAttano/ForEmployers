using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Transform Connector;

    // STATE
    [Space(10)]
    public bool Switch = false;
    private bool _currentSwitch = false;
    public bool Direction = true;
    private bool _currentDirection = true;

    // GLOBAL
    private bool _isComplete = true;

    // SPEED
    [Space(10)]
    public float Speed = 1f;
    private float _currentSpeed = 0f;
    public float Acceleration = 0.05f;
    private float _lerp = 0.1f;

    // ROTATION
    private float _currentRotation;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _currentRotation = Connector.localRotation.y;
    }

    private void FixedUpdate()
    {
        CalcLogic();
        RotateLogic();
    }

    private void CalcLogic()
    {
        Connector.rotation = Connector.rotation.normalized;
        if (Switch != _currentSwitch)
        {
            _currentSwitch = Switch;
            _isComplete = false;
        }
        if (Direction != _currentDirection)
            _currentDirection = Direction;
        if (_currentSwitch)
        {
            if (_currentDirection)
            {
                if (_currentSpeed < Speed)
                    _currentSpeed += Acceleration;
                else
                    _currentSpeed = Speed;
            }
            else
            {
                if (_currentSpeed > -Speed)
                    _currentSpeed -= Acceleration;
                else
                    _currentSpeed = -Speed;
            }
        }
        else
        {
            if (_currentDirection)
            {
                if (_currentSpeed > 0)
                    _currentSpeed -= Acceleration;
                else
                    _currentSpeed = 0;
            }
            else
            {
                if (_currentSpeed < 0)
                    _currentSpeed += Acceleration;
                else
                    _currentSpeed = 0;
            }
        }
        _currentRotation += _currentSpeed;
        Debug.Log(_currentSpeed);
    }
    private void RotateLogic() 
    {
        if (!_isComplete)
        {
            Connector.localRotation = Quaternion.Euler(new Vector3(0, _currentRotation, 0));

            // Here _isComplete should turn to true when rotator stoped
        }
    }

    [ContextMenu("TurnON")]
    public void TurnON()
    {
        Switch = true;
        _currentSwitch = true;

        _isComplete = false;
    }
    [ContextMenu("TurnOFF")]
    public void TurnOFF()
    {
        Switch = false;
        _currentSwitch = false;

        _isComplete = false;
    }

    public void SetDirection(bool clockwise)
    {
        Direction = clockwise;
        _currentDirection = clockwise;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    private GameObject _character;
    private const string _characterTag = "Player";

    private bool _rotationPressed = false;

    // ROTATION
    public float _rotateCharge = 0f;
    private const float _rotateChargeMax = 2f;
    private const float _rotateChargeStep = 0.1f;
    private const float _rotateSlowdown = 1.7f;
    private const float _rotateZeroBorder = 0.01f;
    private const float _rotateAdjustment = 10f;

    private const float _lerpValue = 0.1f;

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
        RotationLogic();
        //ReturnPositionLogic();
    }
    private void RotationLogic()
    {
        _rotationPressed = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _rotationPressed = true;
            if (_rotateCharge > -_rotateChargeMax)
                _rotateCharge -= _rotateChargeStep;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _rotationPressed = true;
            if (_rotateCharge < _rotateChargeMax)
                _rotateCharge += _rotateChargeStep;
        }
        if (!_rotationPressed && _rotateCharge != 0)
        {
            if (_rotateCharge != 0)
                _rotateCharge /= _rotateSlowdown;
            if (Mathf.Abs(_rotateCharge) <= _rotateZeroBorder)
                _rotateCharge = 0;
        }
        if (_rotateCharge != 0f)
        {
            transform.RotateAround(
                _character.transform.position, new Vector3(1, 0, 0),
                _rotateCharge * _rotateAdjustment * Time.deltaTime);
        }
    }
    private void ReturnPositionLogic()
    {
        //Vector3 direction = Vector3.Lerp(transform.position, Vector3.zero, _lerpValue * Time.deltaTime);

        /*transform.position = direction;
        _character.transform.position = transform.position + _character.transform.position;*/

        /*transform.Translate(direction);
        _character.transform.Translate(direction);*/
    }
}

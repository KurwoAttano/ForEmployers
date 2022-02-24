using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    public Prefabs Prefabs;
    private Transform _level;

    private Rigidbody _rb;
    private HingeJoint _hj;

    // MOVEMENT
    private float _rotateForce = 1f;
    private float _moveForce = 0.12f;
    private float _ropeSwingForce = 0.01f;

    // JUMPING
    private Vector3 _jumpDirection = new Vector3();
    private bool _isGrounded = false;
    private bool _isCanJump = false;
    private bool _isJumping = false;

    private float _jumpForce = 2f;
    private int _jumpCharge = 5;
    private const int _jumpChargeMax = 5;
    private const int _jumpChargeStep = 1;

    // MAGNET
    private float _magnetForce = 0.1f;
    ///private float _magnetSlowdown = 1.1f;

    // ROPE
    private bool _isHingeJointExist = false;
    private bool _isRopeParentsExist = false;
    private Transform _ropeContainer;
    private Transform _ropeEnd;
    private Transform _ropePrev;

    private bool _isRopeCreated = false;
    private bool _isRopeConnected = false;
    private bool _isRopeCompleted = false;

    private const float _ropeRadius = 0.1f;
    private const float _ropeElemSpawnDist = 0.25f;

    private float _ropeThrowForce = 100f;
    private float _ropeSpring = 10f;
    private float _ropeDamper = 100000f;

    private int _ropeElemsNameI = 0;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _level = GameObject.FindGameObjectWithTag("Level").transform;
    }

    private void Update()
    {
        MovementLogic();
        JumpLogic();
        RopeLogic();

        Debug.DrawRay(transform.position, _rb.velocity, Color.red); //!
    }

    private void MovementLogic()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rb.AddTorque(new Vector3(1, 0, 0) * _rotateForce, ForceMode.Impulse);
            if (_isGrounded)
                _rb.AddForce(new Vector3(0, 0, 1) * _moveForce, ForceMode.Impulse);
            if (_isRopeCompleted && !_isGrounded)
                _rb.AddForce(new Vector3(0, 0, 1) * _ropeSwingForce, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rb.AddTorque(new Vector3(-1, 0, 0) * _rotateForce, ForceMode.Impulse);
            if (_isGrounded)
                _rb.AddForce(new Vector3(0, 0, -1) * _moveForce, ForceMode.Impulse);
            if (_isRopeCompleted && !_isGrounded)
                _rb.AddForce(new Vector3(0, 0, -1) * _ropeSwingForce, ForceMode.Impulse);
        }
    }
    private void JumpLogic()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            if (_isGrounded && _isCanJump)
            {
                _isJumping = true;
                _isCanJump = false;
            }
            if (_isJumping && _jumpCharge > 0)
            {
                _rb.AddForce(_jumpDirection * _jumpForce, ForceMode.Impulse);
                _jumpCharge -= _jumpChargeStep;
            }
            else
            {
                _isJumping = false;
                if (_jumpCharge < _jumpChargeMax)
                    _jumpCharge += _jumpChargeStep;
            }
        }
        else
        {
            if (_isGrounded)
                _isCanJump = true;
            _isJumping = false;
        }
    }
    private void RopeLogic()
    {
        if (Input.GetKey(KeyCode.R))
        {
            DestroyRope();
            Destroy(_hj);
            _isHingeJointExist = false;
        }
        if (_isRopeCreated)
        {
            if (!_isRopeCompleted)
            {
                while (Vector3.Distance(transform.position, _ropePrev.position) >= _ropeElemSpawnDist)
                {
                    _ropeElemsNameI++;

                    // Create RopeElem
                    Transform elem = Instantiate(Prefabs.RopeElemPrefab).transform;
                    elem.name = "RopeElem" + String.Format("{0:000}", _ropeElemsNameI);
                    elem.position = _ropePrev.position + (transform.position - _ropePrev.position).normalized * _ropeElemSpawnDist;
                    elem.parent = _ropeContainer;
                    // Сделать изменение радиуса от _ropeRadius
                    HingeJoint joint = elem.GetComponent<HingeJoint>();
                    joint.connectedBody = _ropePrev.GetComponent<Rigidbody>();

                    // Spring
                    JointSpring jointSpring = joint.spring;
                    jointSpring.spring = _ropeSpring;
                    jointSpring.damper = _ropeDamper;
                    joint.spring = jointSpring;
                    joint.useSpring = true;

                    joint.connectedMassScale = 0.95f;

                    _ropeEnd.GetComponent<RopeEnd>().AddElem(elem);
                    _ropePrev = elem;
                }

                if (_isRopeConnected)
                {
                    if (!_isHingeJointExist)
                    {
                        _hj = gameObject.AddComponent<HingeJoint>();
                        _isHingeJointExist = true;
                    }
                    _hj.connectedBody = _ropePrev.GetComponent<Rigidbody>();
                    _hj.enableCollision = true;
                    _hj.anchor = Vector3.zero;

                    // Spring
                    JointSpring jointSpring = _hj.spring;
                    jointSpring.spring = _ropeSpring;
                    jointSpring.damper = _ropeDamper;
                    _hj.spring = jointSpring;
                    _hj.useSpring = true;

                    Transform elem = _hj.connectedBody.transform;
                    while (elem.GetComponent<HingeJoint>() != null)
                    {
                        elem.GetComponent<Rigidbody>().useGravity = true;
                        elem = elem.GetComponent<HingeJoint>().connectedBody.transform;
                    }

                    _isRopeCompleted = true;
                    Debug.Log("Rope is completed"); //!
                }
            }
            else
            {

            }
        }
    }

    // Веревка при выкидывании замедляется потому что другие элементы замедляют конец
    // Попробовать сделать что бы масса конца была очень большой пока летит к стене
    // Или пропробовать делать большую массу для каждого следующего элемента относительно текущего

    public void ThrowRope(Vector3 direction)
    {
        if (_isRopeCreated) DestroyRope();
        if (!_isRopeParentsExist)
        {
            _ropeContainer = new GameObject().transform;
            _ropeContainer.name = "RopeContainer";
            _isRopeParentsExist = true;
        }

        _isRopeCreated = true;

        _ropeEnd = Instantiate(Prefabs.RopeEndPrefab).transform;
        
        // Сделать изменение радиуса от _ropeRadius
        _ropeEnd.parent = _ropeContainer;
        _ropeEnd.position = transform.position;
        _ropeEnd.name = "RopeEnd";
        _ropeEnd.GetComponent<Rigidbody>().velocity = direction.normalized * _ropeThrowForce;
        _ropeEnd.GetComponent<RopeEnd>().Initialize(this, _level);

        _ropePrev = _ropeEnd;
    }
    public void ConnectRope()
    {
        _isRopeConnected = true;
    }
    private void DestroyRope()
    {
        if (_isRopeCreated)
        {
            _ropeEnd.GetComponent<RopeEnd>().DestroyRope();
            _isRopeParentsExist = false;

            _isRopeCreated = false;
            _isRopeConnected = false;
            _isRopeCompleted = false;

            _ropeElemsNameI = 0;
        }
    }

    public void Kill()
    {
        Debug.Log("Game Over");
    }

    private void OnCollisionStay(Collision collision)
    {
        _isGrounded = true;

        Vector3 jumpDirection = new Vector3();
        foreach (ContactPoint contact in collision.contacts)
        {
            _rb.velocity -= (transform.position - contact.point).normalized * _magnetForce;
            //_rb.velocity /= _magnetSlowdown;
            jumpDirection += (transform.position - contact.point);
        }
        _jumpDirection = jumpDirection.normalized;
    }
    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}

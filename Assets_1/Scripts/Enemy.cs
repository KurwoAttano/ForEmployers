using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _agent;
    public Slider DetectSlider;

    private Transform _pig;

    public Sprite[] Sprites = new Sprite[12];
    private readonly List<Vector2> PatrolPositions = new List<Vector2>();

    private const int _layerMask = (1 << 7) | (1 << 8);
    private const int RotationAnimDelay = 5;

    public float MovementSpeed = 25f;
    public float PatrolRadius = 4f;
    public float DetectIncrease = 0.01f;
    public float DetectDecrease = 0.005f;
    private const int PatrolStayDelay = 150;
    private const int DirtyDelay = 500;

    private Vector3 _targetPos;
    private Vector3 _lastTickPos;

    private bool _isMoves = false;
    private int _currDir = 2;
    private byte _status = 0;
    
    private int _currDelay = PatrolStayDelay;
    private int _currRotationAnimDelay = RotationAnimDelay;

    public void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _pig = GameObject.Find("Pig").transform;

        Vector2 startPos = new Vector2(-8.5f, 3.5f);
        float lineLeftOffset = 0.15f;
        Vector2 offset = new Vector2(1.09f, -1f);
        Vector2Int count = new Vector2Int(17, 9);
        for (int x = 0; x < count.x; x++)
            for (int y = 0; y < count.y; y++)
                if (x % 2 == 0 || (x % 2 != 0 && y % 2 == 0))
                    PatrolPositions.Add(startPos + new Vector2(offset.x * x - lineLeftOffset * (y - 1), offset.y * y));
    }

    private void Update()
    {
        MovementLogic();
        DetectLogic();
        SpriteLogic();
    }

    private void MovementLogic()
    {
        if (_isMoves)
        {
            if (Vector3.Distance(_targetPos, transform.position) < 0.2f)
                StopMovement();
            Vector3 dir = (transform.position - _lastTickPos).normalized;
            _currDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? (dir.x > 0 ? 1 : 3) : (dir.y > 0 ? 0 : 2);
            Debug.DrawRay(transform.position, dir, Color.red); //!
        }
        else if (_currDelay <= 0)
        {
            _targetPos = DefineTargetPos();
            _currDelay = PatrolStayDelay;
            GoToPos(_targetPos);
        }
        else if (!_isMoves) _currDelay--;

        _lastTickPos = transform.position;
    }

    private void DetectLogic()
    {
        if (_status != 2)
        {
            Vector3 dir = _pig.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dir.magnitude * 2, _layerMask);
            Debug.DrawLine(transform.position, hit.point, Color.blue); //!
            if (hit.transform == _pig && _status != 2)
            {
                if (DetectSlider.value == 1f)
                {
                    GoToPos(_pig.position);
                    _targetPos = _pig.position;
                }
                else DetectSlider.value += DetectIncrease;
            }
            else if (DetectSlider.value != 0f) DetectSlider.value -= DetectDecrease;
        }
        else if (DetectSlider.value != 0f) DetectSlider.value -= DetectDecrease;
    }

    private void SpriteLogic()
    {
        if (_currRotationAnimDelay <= 0)
        {
            _currRotationAnimDelay = RotationAnimDelay;
            int index = _currDir + (_status == 1 ? 4 : _status == 2 ? 8 : 0);
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprites[index];
        }
        else _currRotationAnimDelay--;
    }

    public Vector3 DefineTargetPos()
    {
        const int tryCount = 20;
        for (int i = 0; i < tryCount; i++)
        {
            Vector2 pos = PatrolPositions[Random.Range(0, PatrolPositions.Count)];
            if (Vector3.Distance(transform.position, pos) <= PatrolRadius)
                return pos;
        }
        return transform.position;
    }
    public void GoToPos(Vector3 targetPos)
    {
        _isMoves = true;
        _agent.SetDestination(targetPos);
        _agent.isStopped = false;
        _status = 1;
    }
    private void StopMovement()
    {
        _isMoves = false;
        _agent.isStopped = true;
        _status = 0;
    }
    
    public void Hit()
    {
        Debug.Log("Farmer hited");
        StopMovement();
        _status = 2;
        _currDelay = DirtyDelay;
    }
}

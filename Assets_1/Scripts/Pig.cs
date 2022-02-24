using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField]
    protected Sprite[] Sprites = new Sprite[4];

    private const float _movementSpeed = 40f;
    private const float _movementShiftSpeed = 60f;

    [SerializeField]
    protected GameObject BombPrefab;
    private bool _bombPermission = true;
    private int _currBombDelay = BombDelay;
    private const int BombDelay = 250; // 5 seconds

    public bool Up = false;
    public bool Right = false;
    public bool Down = false;
    public bool Left = false;

    public bool Shift = false;

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        MovementLogic();
        BombLogic();
    }
    public void FixedUpdate()
    {
        BombTimerLogic();
    }

    private void MovementLogic()
    {
        Shift = Input.GetKey(KeyCode.LeftShift) ? true : Shift;

        if (Input.GetKey(KeyCode.W) || Up) 
        { 
            _rb.AddForce(new Vector2(0f, 1f) * (Shift ? _movementShiftSpeed : _movementSpeed) * Time.deltaTime, ForceMode2D.Impulse);
            GetComponent<SpriteRenderer>().sprite = Sprites[0];
        }
        if (Input.GetKey(KeyCode.S) || Down) 
        { 
            _rb.AddForce(new Vector2(0f, -1f) * (Shift ? _movementShiftSpeed : _movementSpeed) * Time.deltaTime, ForceMode2D.Impulse);
            GetComponent<SpriteRenderer>().sprite = Sprites[2];
        }
        if (Input.GetKey(KeyCode.A) || Left) 
        { 
            _rb.AddForce(new Vector2(-1f, 0f) * (Shift ? _movementShiftSpeed : _movementSpeed) * Time.deltaTime, ForceMode2D.Impulse);
            GetComponent<SpriteRenderer>().sprite = Sprites[3];
        }
        if (Input.GetKey(KeyCode.D) || Right) 
        {
            _rb.AddForce(new Vector2(1f, 0f) * (Shift ? _movementShiftSpeed : _movementSpeed) * Time.deltaTime, ForceMode2D.Impulse);
            GetComponent<SpriteRenderer>().sprite = Sprites[1];
        }
    }

    private void BombLogic()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PutBomb();
    }
    public void PutBomb()
    {
        if (_bombPermission)
        {
            GameObject bomb = Instantiate(BombPrefab, GameObject.Find("Bombs").transform);
            bomb.transform.position = transform.position;
            _bombPermission = false;
        }
    }

    private void BombTimerLogic()
    {
        if (!_bombPermission)
            if (_currBombDelay <= 0)
            {
                _bombPermission = true;
                _currBombDelay = BombDelay;
            }
            else _currBombDelay--;
    }

}

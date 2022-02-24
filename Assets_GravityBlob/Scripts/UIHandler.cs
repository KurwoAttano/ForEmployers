using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private static bool _isPressedOnUI = false;

    private Vector2 _slideStartPos;
    private Vector2 _slideEndPos;
    private bool _isSlideStarted = false;
    private bool _isSlideActive = false;

    private float _slideDistance = 20f;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {

    }

    private void Update()
    {
        SlideRopeLogic();
    }
    private void SlideRopeLogic() 
    {
        if (!_isPressedOnUI)
        {
            if (Input.GetMouseButton(0) && _isSlideStarted && Vector2.Distance(Input.mousePosition, _slideStartPos) >= _slideDistance)
            {
                _isSlideActive = true;
                Debug.Log("Slide activated"); //!
            }
            if (Input.GetMouseButtonDown(0))
            {
                _slideStartPos = Input.mousePosition;
                _isSlideStarted = true;
            }
        }
        if (!Input.GetMouseButton(0))
        {
            if (_isSlideActive)
            {
                // Throw rope
                _slideEndPos = Input.mousePosition;
                Vector3 dir = new Vector3(0, _slideEndPos.y - _slideStartPos.y, _slideEndPos.x - _slideStartPos.x);
                GameObject character = GameObject.Find("Character");
                character.GetComponent<CharacterHandler>().ThrowRope(dir);
            }
            _isSlideStarted = false;
            _isSlideActive = false;
        }

        if (_isSlideActive)
        {

        }
    }
}

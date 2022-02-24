using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ButtonType
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
    Shift = 4
}
public class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ButtonType direction;
    public Pig pig;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        switch (direction)
        {
            case ButtonType.Up: pig.Up = true; break;
            case ButtonType.Right: pig.Right = true; break;
            case ButtonType.Down: pig.Down = true; break;
            case ButtonType.Left: pig.Left = true; break;
            case ButtonType.Shift: pig.Shift = true; break;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        switch (direction)
        {
            case ButtonType.Up: pig.Up = false; break;
            case ButtonType.Right: pig.Right = false; break;
            case ButtonType.Down: pig.Down = false; break;
            case ButtonType.Left: pig.Left = false; break;
            case ButtonType.Shift: pig.Shift = false; break;
        }
    }
}

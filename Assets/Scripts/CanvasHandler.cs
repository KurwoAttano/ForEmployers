using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    private Transform _farmer;
    private Transform _dog;

    private Slider _farmerDetectSlider;
    private Slider _dogDetectSlider;

    private readonly Vector3 _farmerDetectSliderPosOffset = new Vector3(0, 180, 0);
    private readonly Vector3 _dogDetectSliderPosOffset = new Vector3(0, 150, 0);

    public void Start()
    {
        _farmer = GameObject.Find("Farmer").transform;
        _dog = GameObject.Find("Dog").transform;
        _farmerDetectSlider = GameObject.Find("FarmerDetectSlider").GetComponent<Slider>();
        _dogDetectSlider = GameObject.Find("DogDetectSlider").GetComponent<Slider>();
    }

    public void Update()
    {
        _farmerDetectSlider.transform.position = Camera.main.WorldToScreenPoint(_farmer.position) + _farmerDetectSliderPosOffset;
        _dogDetectSlider.transform.position = Camera.main.WorldToScreenPoint(_dog.position) + _dogDetectSliderPosOffset;
    }
}

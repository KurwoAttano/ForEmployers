using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int _delay = 150; // 3 sec 
    private const float Range = 3f;

    public void FixedUpdate()
    {
        if (_delay <= 0)
        {
            Badabum();
            Destroy(gameObject);
        }
        else _delay--;
    }

    private void Badabum()
    {
        Enemy farmer = GameObject.Find("Farmer").GetComponent<Enemy>();
        if (Vector3.Distance(transform.position, farmer.transform.position) <= Range) 
            farmer.Hit();
        Enemy dog = GameObject.Find("Dog").GetComponent<Enemy>();
        if (Vector3.Distance(transform.position, dog.transform.position) <= Range)
            dog.Hit();
    }
}

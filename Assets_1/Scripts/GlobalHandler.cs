using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHandler : MonoBehaviour
{
    [SerializeField] protected GameObject Stone;

    public void Start()
    {
        
    }
    public void Update()
    {

    }

#if UNITY_EDITOR

    [ContextMenu("Bake stones")]
    public void BakeStones()
    {
        Vector2 startPos = new Vector2(-7.5f, 2.5f);
        float lineLeftOffset = 0.3f;
        Vector2 offset = new Vector2(2.15f, -2f);
        Vector2Int count = new Vector2Int(8, 4);

        Transform parent = GameObject.Find("Stones").transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
            DestroyImmediate(parent.GetChild(i).gameObject);

        for (int x = 0; x < count.x; x++)
            for (int y = 0; y < count.y; y++)
            {
                GameObject s = Instantiate(Stone, parent);
                s.transform.position = startPos + new Vector2(offset.x * x - lineLeftOffset * (y - 1), offset.y * y);
            }
    }

#endif

}

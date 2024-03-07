using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    TextMeshPro minesText;

    [SerializeField, Min(1)]
    int rows = 8, columns = 21;

    Grid grid;
    void OnEnable()
    {
        grid.Initialize(rows, columns);
    }

    void OnDisable()
    {
        grid.Dispose();
    }

    void Update()
    {
        if (grid.Rows != rows || grid.Columns != columns)
        {
            OnDisable();
            OnEnable();
        }
    }
}

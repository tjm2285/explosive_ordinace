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

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    GridVisualizations visualization;

    Grid grid;
    void OnEnable()
    {
        grid.Initialize(rows, columns);
        visualization.Initialize(grid, material, mesh);
    }

    void OnDisable()
    {
        grid.Dispose();
        visualization.Dispose();
    }

    void Update()
    {
        if (grid.Rows != rows || grid.Columns != columns)
        {
            OnDisable();
            OnEnable();
        }

        visualization.Update();
        visualization.Draw();
    }
}

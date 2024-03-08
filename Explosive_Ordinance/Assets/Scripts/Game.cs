using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    TextMeshPro minesText;

    [SerializeField, Min(1)]
    int rows = 8, columns = 21, mines = 30;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    GridVisualizations visualization;

    Grid grid;

    int markedSureCount;

    void OnEnable()
    {
        grid.Initialize(rows, columns);
        visualization.Initialize(grid, material, mesh);
        mines = Mathf.Min(mines, grid.CellCount);
        minesText.SetText("{0}", mines);
        markedSureCount = 0;
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

        if (PerformAction())
        {
            visualization.Update();
        }
        visualization.Draw();
    }

    bool PerformAction()
    {
        bool markAction = Input.GetMouseButtonDown(1);
        Debug.Log(markAction);
        if (
            markAction &&
            visualization.TryGetHitCellIndex(
                Camera.main.ScreenPointToRay(Input.mousePosition), out int cellIndex
            )
        )
        {
            Debug.Log(cellIndex);
            grid[cellIndex] = CellState.MarkedSure;
            return DoMarkAction(cellIndex); 
        }

        return false;
    }

    bool DoMarkAction(int cellIndex)
    {
        CellState state = grid[cellIndex];
        if (state.Is(CellState.Revealed))
        {
            return false;
        }

        if (state.IsNot(CellState.Marked))
        {
            grid[cellIndex] = state.With(CellState.MarkedSure);
            markedSureCount += 1;
        }
        else if (state.Is(CellState.MarkedSure))
        {
            grid[cellIndex] =
                state.Without(CellState.MarkedSure).With(CellState.MarkedUnsure);
            markedSureCount -= 1;
        }
        else
        {
            grid[cellIndex] = state.Without(CellState.MarkedUnsure);
        }

        minesText.SetText("{0}", mines - markedSureCount);
        return true;
    }
}

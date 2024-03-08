using Unity.Collections;
using Unity.Jobs;
using UnityEditorInternal;
using UnityEngine;

public struct Grid
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public int CellCount => states.Length;
    public CellState this[int i]
    {
        get => states[i];
        set => states[i] = value;
    }

    NativeArray<CellState> states;
    public int GetCellIndex(int row, int column) => row * Columns + column;
    public bool TryGetCellIndex(int row, int column, out int index)
    {
        bool valid = 0 <= row && row < Rows && 0 <= column && column < Columns;
        index = valid ? GetCellIndex(row, column) : -1;
        return valid;
    }

    public void GetRowColumn(int index, out int row, out int column)
    {
        row = index / Columns;
        column = index - row * Columns;
    }
    public void Initialize(int rows, int columns)
    {
        Rows = rows; 
        Columns = columns;
        states = new NativeArray<CellState>(Rows * Columns, Allocator.Persistent);
    }

    public void PlaceMines(int mines) => new PlaceMinesJob
    {
        grid = this,
        mines = mines,
        seed = Random.Range(1, int.MaxValue)
    }.Schedule().Complete();

    public void Dispose () => states.Dispose();


}

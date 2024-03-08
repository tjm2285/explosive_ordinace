using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
struct InitializeVisualizationJob : IJobFor
{
    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<float3> positions, colors;

    public int rows, columns;
    
    public void Execute(int i)
    {

        float3 cellPosition = GetCellPosition(i);
        int blockOffset = i * GridVisualizations.blocksPerCell;

        for (int bi = 0; bi < GridVisualizations.blocksPerCell; bi++)
        {
            positions[blockOffset + bi] = cellPosition + GetBlockPosition(bi);
            colors[blockOffset + bi] = 0.5f;
        }
    }
    float3 GetBlockPosition(int i)
    {
        int r = i / GridVisualizations.ColumnsPerCell;
        int c = i - r * GridVisualizations.ColumnsPerCell;
        return float3(c, 0f, r);
    }
    float3 GetCellPosition(int i)
    {
        int r = i / columns;
        int c = i - r * columns;
        return float3(
            c - (columns - 1) * 0.5f,
            0f,
            r - (rows - 1) * 0.5f - (c & 1) * 0.5f + 0.25f
        ) * float3(
                GridVisualizations.ColumnsPerCell + 1,
                0f,
                GridVisualizations.RowsPerCell + 1
            ) - float3(
                GridVisualizations.ColumnsPerCell / 2,
                0f,
                GridVisualizations.RowsPerCell / 2
            );
    }
}
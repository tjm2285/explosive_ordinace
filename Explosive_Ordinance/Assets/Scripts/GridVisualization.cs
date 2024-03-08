using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public struct GridVisualizations
{
    static int
        positionsId = Shader.PropertyToID("_Positions"),
        colorsId = Shader.PropertyToID("_Colors");

    public const int
        RowsPerCell = 7,
        ColumnsPerCell = 5,
        blocksPerCell = RowsPerCell * ColumnsPerCell;

    ComputeBuffer positionsBuffer, colorsBuffer;

    NativeArray<float3> positions, colors;

    Grid grid;

    Material material;

    Mesh mesh;

    public void Initialize(Grid grid, Material material, Mesh mesh)
    {
        this.grid = grid;
        this.material = material;
        this.mesh = mesh;

        int instanceCount = grid.CellCount * blocksPerCell;
        positions = new NativeArray<float3>(instanceCount, Allocator.Persistent);
        colors = new NativeArray<float3>(instanceCount, Allocator.Persistent);

        positionsBuffer = new ComputeBuffer(instanceCount, 3 * 4);
        colorsBuffer = new ComputeBuffer(instanceCount, 3 * 4);
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetBuffer(colorsId, colorsBuffer);

        new InitializeVisualizationJob
        {
            positions = positions,
            colors = colors,
            rows = grid.Rows,
            columns = grid.Columns
        }.ScheduleParallel(grid.CellCount, grid.Columns, default).Complete();
        positionsBuffer.SetData(positions);
        colorsBuffer.SetData(colors);
    }
    public void Update()
    {
        new UpdateVisualizationJob
        {
            positions = positions,
            colors = colors,
            grid = grid
        }.ScheduleParallel(grid.CellCount, grid.Columns, default).Complete();
        positionsBuffer.SetData(positions);
        colorsBuffer.SetData(colors);
    }
    public bool TryGetHitCellIndex(Ray ray, out int cellIndex)
    {
        Vector3 p = ray.origin - ray.direction * (ray.origin.y / ray.direction.y);

        float x = p.x + ColumnsPerCell / 2 + 1.5f;
        x /= ColumnsPerCell + 1;
        x += (grid.Columns - 1) * 0.5f;
        int c = Mathf.FloorToInt(x);

        float z = p.z + RowsPerCell / 2f + 1.5f;
        z /= RowsPerCell + 1;
        z += (grid.Rows - 1) * 0.5f + (c & 1) * 0.5f - 0.25f;
        int r = Mathf.FloorToInt(z);
        Debug.Log(grid.TryGetCellIndex(r, c, out cellIndex));
        Debug.Log(x - c > 1f / (ColumnsPerCell + 1));
        Debug.Log(z - r > 1f / (RowsPerCell + 1));

        return grid.TryGetCellIndex(r, c, out cellIndex) && x - c > 1f / (ColumnsPerCell + 1) && z - r > 1f / (RowsPerCell + 1) ;
    }

    public void Dispose()
    {
        positions.Dispose();
        colors.Dispose(); 
        positionsBuffer.Release();
        colorsBuffer.Release();
    }

    public void Draw() => Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one), positionsBuffer.count);
}

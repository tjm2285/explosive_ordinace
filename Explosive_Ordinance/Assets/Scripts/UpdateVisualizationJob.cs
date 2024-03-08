using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
struct UpdateVisualizationJob : IJobFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> positions, colors;

    [ReadOnly]
    public Grid grid;

    public void Execute(int i)
    {
        int blockOffset = i * GridVisualizations.blocksPerCell;

        for (int bi = 0; bi < GridVisualizations.blocksPerCell; bi++)
        {
            float3 position = positions[blockOffset + bi];
            position.y = bi / (float)GridVisualizations.blocksPerCell;
            positions[blockOffset + bi] = position;
            colors[blockOffset + bi] = position.y;
        }
    }
}
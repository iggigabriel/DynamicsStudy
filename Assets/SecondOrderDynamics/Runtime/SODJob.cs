using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Dynamics
{
    [BurstCompile]
    public struct SODJobFloat : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobFloat2 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float2>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float2>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobFloat3 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float3>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float3>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

    [BurstCompile]
    public struct SODJobFloat4 : IJobParallelFor
    {
        readonly float deltaTime;

        [ReadOnly]
        public NativeArray<SODState<float4>> inStates;

        [WriteOnly]
        public NativeArray<SODState<float4>> outStates;

        public void Execute(int index)
        {
            var state = inStates[index];

            SecondOrderDynamics.UpdateState(ref state, deltaTime);

            outStates[index] = state;
        }
    }

}
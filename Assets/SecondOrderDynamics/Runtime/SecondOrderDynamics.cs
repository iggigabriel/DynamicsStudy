using Unity.Burst;
using Unity.Mathematics;

namespace Dynamics
{
    [BurstCompile]
    public static class SecondOrderDynamics
    {
        // If changed, all SODCurve K-Values  need to be recalculated, so i prefer to keep it as compile constant
        public const float DeltaTime = 1f / 100f;

        [BurstCompile]
        public static void UpdateState(ref SODState<float> state, float deltaTime)
        {
            var k = state.kValues;

            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= 1f)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= 1f;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SODState<float2> state, float deltaTime)
        {
            var k = state.kValues;

            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= 1f)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= 1f;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SODState<float3> state, float deltaTime)
        {
            var k = state.kValues;

            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= 1f)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= 1f;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }

        [BurstCompile]
        public static void UpdateState(ref SODState<float4> state, float deltaTime)
        {
            var k = state.kValues;

            state.timeFraction += deltaTime / DeltaTime;

            while (state.timeFraction >= 1f)
            {
                state.previousValue += state.velocity * DeltaTime;
                state.velocity += (state.target + k.z * (state.target - state.previousTarget) - state.previousValue - k.x * state.velocity) / k.y;

                state.timeFraction -= 1f;
                state.previousTarget = state.target;
            }

            state.value = state.previousValue + state.velocity * state.timeFraction * DeltaTime;
        }
    }
}
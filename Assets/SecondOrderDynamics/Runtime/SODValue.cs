using System;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics
{
    [Serializable]
    public abstract class SODValue<TValue, TState>
        where TValue : struct
        where TState : struct
    {
        [SerializeField]
        protected SODCurve curve = SODCurve.Default;

        [SerializeField]
        protected SODState<TState> state = new(default);

        public float TimeFraction => state.timeFraction;

        public SODCurve Curve { get => curve; set { curve = value; state.kValues = curve.KValues; } }
        public SODState<TState> State { get => state; set => state = value; }

        public abstract TValue PreviousValue { get; set; }
        public abstract TValue Velocity { get; set; }
        public abstract TValue PreviousTarget { get; set; }
        public abstract TValue Target { get; set; }

        public abstract TValue Value { get; }

        public abstract void Update(float deltaTime);
        public abstract void Reset(TValue value, bool resetVelocity = true, bool resetTime = true);
    }

    [Serializable]
    public class SODFloat : SODValue<float, float>
    {
        public override float PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(0f); } }
        public override float Velocity { get => state.velocity; set => state.velocity = value; }
        public override float PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override float Target { get => state.target; set => state.target = value; }
        public override float Value => state.value;

        public override void Update(float deltaTime) => SecondOrderDynamics.UpdateState(ref state, deltaTime);
        public override void Reset(float value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }

    [Serializable]
    public class SODFloat2 : SODValue<float2, float2>
    {
        public override float2 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(0f); } }
        public override float2 Velocity { get => state.velocity; set => state.velocity = value; }
        public override float2 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override float2 Target { get => state.target; set => state.target = value; }
        public override float2 Value => state.value;

        public override void Update(float deltaTime) => SecondOrderDynamics.UpdateState(ref state, deltaTime);
        public override void Reset(float2 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }

    [Serializable]
    public class SODFloat3 : SODValue<float3, float3>
    {
        public override float3 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(0f); } }
        public override float3 Velocity { get => state.velocity; set => state.velocity = value; }
        public override float3 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override float3 Target { get => state.target; set => state.target = value; }
        public override float3 Value => state.value;

        public override void Update(float deltaTime) => SecondOrderDynamics.UpdateState(ref state, deltaTime);
        public override void Reset(float3 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }

    [Serializable]
    public class SODFloat4 : SODValue<float4, float4>
    {
        public override float4 PreviousValue { get => state.previousValue; set { state.previousValue = value; Update(0f); } }
        public override float4 Velocity { get => state.velocity; set => state.velocity = value; }
        public override float4 PreviousTarget { get => state.previousTarget; set => state.previousTarget = value; }
        public override float4 Target { get => state.target; set => state.target = value; }
        public override float4 Value => state.value;

        public override void Update(float deltaTime) => SecondOrderDynamics.UpdateState(ref state, deltaTime);
        public override void Reset(float4 value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value, resetVelocity, resetTime);
    }

    [Serializable]
    public class SODQuaternion : SODValue<quaternion, float4>
    {
        public override quaternion PreviousValue { get => state.previousValue; set { state.previousValue = value.value; Update(0f); } }
        public override quaternion Velocity { get => state.velocity; set => state.velocity = value.value; }
        public override quaternion PreviousTarget { get => state.previousTarget; set => state.previousTarget = value.value; }
        public override quaternion Target { get => state.target; set => state.target = value.value; }
        public override quaternion Value => state.value;

        public override void Update(float deltaTime) => SecondOrderDynamics.UpdateState(ref state, deltaTime);
        public override void Reset(quaternion value, bool resetVelocity = true, bool resetTime = true) => state.Reset(value.value, resetVelocity, resetTime);
    }
}
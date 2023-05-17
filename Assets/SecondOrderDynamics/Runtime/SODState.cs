using System;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics
{
    [Serializable]
    public struct SODState<T> where T : struct
    {
        public T previousValue;

        public T previousTarget;

        public T target;

        public T velocity;

        [NonSerialized]
        public T value;

        public float3 kValues;

        [NonSerialized]
        public float timeFraction;

        public SODState(T value)
        {
            target = previousTarget = previousValue = this.value = value;
            kValues = default;
            velocity = default;
            timeFraction = 1f;
        }

        public void Reset(T value, bool resetVelocity = true, bool resetTime = true)
        {
            target = previousTarget = previousValue = this.value = value;

            if (resetVelocity) velocity = default;
            if (resetTime) timeFraction = 1f;
        }
    }
}
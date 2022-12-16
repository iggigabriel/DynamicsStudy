using System;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics
{
    [Serializable]
    public struct SODCurve
    {
        public static readonly SODCurve Default = new(1f, 1f, 1f);
        public static readonly SODCurve Damped = new(3f, 0.5f, -2f);
        public static readonly SODCurve Elastic = new(4f, 0.3f, 0f);
        public static readonly SODCurve Undershoot = new(4f, 1f, -1.5f);

        [SerializeField, Tooltip("Speed at which the system will respond to changes in the input, measured in cycles per second")]
        private float frequency;

        [SerializeField, Tooltip("0 = never stops vibrating, 0-1 = vibration strenght, >1 = never vibrates")]
        private float damping;

        [SerializeField, Tooltip("<0 = anticipate, 0-1 = response strenght, >1 = overshoot")]
        private float response;

        [SerializeField]
        private float3 kValues;

        /// <summary>
        /// Speed at which the system will respond to changes in the input, measured in cycles per second.
        /// </summary>
        public float Frequency
        {
            get => frequency;
            set
            {
                frequency = value;
                RecalculateKValues();
            }
        }

        /// <summary>
        /// 0 = never stops vibrating, 0-1 = vibration strenght, >1 = never vibrates
        /// </summary>
        public float Damping
        {
            get => damping;
            set
            {
                damping = value;
                RecalculateKValues();
            }
        }

        /// <summary>
        /// <0 = anticipate, 0-1 = response strenght, >1 = overshoot
        /// </summary>
        public float Response
        {
            get => response;
            set
            {
                response = value;
                RecalculateKValues();
            }
        }

        public float3 KValues => kValues;

        public SODCurve(float frequency = 1f, float damping = 1f, float response = 1f)
        {
            this.frequency = frequency;
            this.damping = damping;
            this.response = response;

            kValues = default;

            RecalculateKValues();
        }

        public void Set(float? frequency = default, float? damping = default, float? response = default)
        {
            if (frequency.HasValue) this.frequency = frequency.Value;
            if (damping.HasValue) this.damping = damping.Value;
            if (response.HasValue) this.response = response.Value;

            RecalculateKValues();
        }

        private void RecalculateKValues()
        {
            float f = math.max(0.001f, math.PI * frequency);
            float fdbl = 2f * f;

            kValues.x = damping / f;

            kValues.y = 1f / (fdbl * fdbl);
            kValues.y = math.max(kValues.y, SecondOrderDynamics.DeltaTime * SecondOrderDynamics.DeltaTime / 2f + SecondOrderDynamics.DeltaTime * kValues.x / 2f);
            kValues.y = math.max(kValues.y / SecondOrderDynamics.DeltaTime, kValues.x);

            kValues.z = response * damping / fdbl / SecondOrderDynamics.DeltaTime;
        }
    }
}
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public static class HexMath
    {
        public static readonly float3 Up = new(0f, 1f, 0f);
        public static readonly float3 Forward = new(0f, 0f, 1f);

        const float hexMod1 = 1.7320508f;
        const float hexMod2 = hexMod1 / 2f;
        const float hexMod3 = hexMod1 / 3f;
        const float hexMod4 = 2f / 3f;
        const float hexMod5 = -1f / 3f;
        const float hexMod6 = 3f / 2f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 HexToWorld2D(float2 hexPosition)
        {
            return new float2(hexMod2 * hexPosition.x + hexMod1 * hexPosition.y, hexMod6 * hexPosition.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 HexToWorld3D(float2 hexPosition)
        {
            return new float3(hexMod2 * hexPosition.x + hexMod1 * hexPosition.y, 0f, hexMod6 * hexPosition.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 World2DToHex(float2 worldPosition)
        {
            return new float2(worldPosition.y * hexMod4, hexMod5 * worldPosition.y + hexMod3 * worldPosition.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 World3DToHex(float3 worldPosition)
        {
            return new float2(worldPosition.z * hexMod4, hexMod5 * worldPosition.z + hexMod3 * worldPosition.x);
        }

        public static float3 Flatten(this Vector3 vector)
        {
            return new float3(vector.x, 0f, vector.z);
        }

        public static float3 Flatten(this float3 vector)
        {
            return new float3(vector.x, 0f, vector.z);
        }

        public static float2 To2D(this Vector3 vector)
        {
            return new float2(vector.x, vector.z);
        }
    }
}

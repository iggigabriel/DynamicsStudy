using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class Jiggle : MonoBehaviour
    {
        public Transform source;
        public Transform target;

        public SODFloat2 jiggle;

        public float force = 1f;

        float2 lastPosition;

        private void Start()
        {
            lastPosition = GetPos(source.position);
        }

        private void Update()
        {
            jiggle.Update(Time.deltaTime);

            target.localEulerAngles = GetEuler(jiggle.Value) * 360f;
        }

        private void FixedUpdate()
        {
            var pos = GetPos(source.position);
            var deltaPos = lastPosition - pos;
            lastPosition = pos;

            jiggle.Velocity += 0.5f * force * Time.deltaTime * deltaPos;
        }

        private static float2 GetPos(float3 worldPos) => new(worldPos.x, worldPos.z);
        private static float3 GetEuler(float2 pos) => new(pos.y, 0, -pos.x);
    }
}

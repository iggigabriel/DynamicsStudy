using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class Car : MonoBehaviour
    {
        public SODFloat rotationX;
        public SODFloat rotationY;
        public float2 rotationForce;

        public Transform body;
        public float bodyForce;

        [Range(0.1f, 2f)]
        public float easingModifier = 0.8f;

        public float2 tiresPivot;

        private void Update()
        {
            rotationX.Update(Time.deltaTime);
            rotationY.Update(Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
                {
                    var dir = GetDir(ray.direction + UnityEngine.Random.insideUnitSphere * 0.1f);

                    rotationX.Velocity += dir.y * rotationForce.x * 360f;
                    rotationY.Velocity += -dir.x * rotationForce.y * 360f;
                }
            }

            body.localEulerAngles = new Vector3(rotationY.Value, 0f, rotationX.Value) * bodyForce;

            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            RotateAround(Vector3.left * tiresPivot.x, Vector3.forward, rotationX.Value);
            RotateAround(Vector3.back * tiresPivot.y, Vector3.left, rotationY.Value);
        }

        private void RotateAround(float3 pivot, float3 axis, float force)
        {
            var sign = math.sign(force);
            force = math.pow(math.abs(force), easingModifier) * sign;
            transform.RotateAround((float3)transform.position + pivot * sign, axis, force);
        }

        private static float2 GetDir(float3 vector) => math.normalize(new float2(vector.z, -vector.x));
    }
}

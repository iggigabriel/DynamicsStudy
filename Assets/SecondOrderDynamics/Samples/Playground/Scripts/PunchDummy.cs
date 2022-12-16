using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class PunchDummy : MonoBehaviour
    {
        public Transform stump;
        public SODFloat2 stumpHitRotation;

        public Transform body;
        public SODFloat2 bodyHitRotation;

        public SODFloat bodyHitSpin;

        public float hitBodyForce = 10f;
        public float hitStumpForce = 10f;
        public float spinForce = 10f;

        void Update()
        {
            stumpHitRotation.Update(Time.deltaTime);
            bodyHitRotation.Update(Time.deltaTime);
            bodyHitSpin.Update(Time.deltaTime);

            stump.localEulerAngles = new float3(stumpHitRotation.Value, 0f) * 360f;
            body.localEulerAngles = new float3(bodyHitRotation.Value, bodyHitSpin.Value) * 360f;

            CheckMouseClick();
        }

        private void CheckMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit) && hit.collider.GetComponentInParent<PunchDummy>() == this)
                {
                    var dir = math.normalize(GetDir(ray.direction + UnityEngine.Random.insideUnitSphere * 0.15f));

                    stumpHitRotation.Velocity += dir * hitStumpForce;
                    bodyHitRotation.Velocity += dir * hitBodyForce;

                    var hitDistance = math.length(hit.point.Flatten() - transform.position.Flatten());
                    var hitAngle = Vector2.SignedAngle(ray.direction.To2D(), (hit.point - transform.position).To2D());

                    bodyHitSpin.Velocity -= hitAngle * spinForce / 180f * hitDistance;
                }
            }
        }

        private static float2 GetDir(Vector3 vector) => new(vector.z, -vector.x);
    }
}

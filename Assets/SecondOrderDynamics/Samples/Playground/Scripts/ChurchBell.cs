using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class ChurchBell : MonoBehaviour
    {
        public SODFloat rotation;

        public float rotationForce;

        public float moveForce;
        
        Vector3 lastPosition;

        void Start()
        {
            lastPosition = transform.position;
        }

        void Update()
        {
            rotation.Update(Time.deltaTime);
            transform.localEulerAngles = new Vector3(0f, 0f, rotation.Value) * 360f;

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit) && hit.collider.GetComponentInParent<ChurchBell>() == this)
                {
                    if (math.abs(rotation.Velocity) > 0.01f)
                    {
                        rotation.Velocity += rotationForce * math.sign(rotation.Velocity);
                    }
                    else
                    {
                        rotation.Velocity += rotationForce * (UnityEngine.Random.value > 0.5f ? -1f : 1f);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            var deltaPosition = transform.position - lastPosition;
            lastPosition = transform.position;
            rotation.Velocity += transform.InverseTransformVector(deltaPosition).x * moveForce * Time.fixedDeltaTime;
        }
    }
}

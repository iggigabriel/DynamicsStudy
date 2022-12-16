using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class NewtonSphere : MonoBehaviour
    {
        public SODFloat rotation;

        public float2 forceMinMax;

        public SphereCollider center;

        public float CenterRadius => center.radius * center.transform.lossyScale.x;
        public Vector3 CenterPosition => center.transform.position;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit) && hit.collider.GetComponentInParent<NewtonSphere>() == this)
                {
                    var randomSign = (transform.parent.childCount / 2) > transform.GetSiblingIndex() ? -1f : 1f;
                    rotation.Velocity += UnityEngine.Random.Range(forceMinMax.x, forceMinMax.y) * randomSign;
                }
            }
        }

        public void UpdateRotation(float deltaTime)
        {
            rotation.Update(deltaTime);
            transform.localEulerAngles = Vector3.forward * rotation.Value;
        }

        public void OnCollision(NewtonSphere other, float penetrationDistance)
        {
            var velocityA = rotation.Velocity;
            var velocityB = other.rotation.Velocity;

            rotation.Velocity = velocityB;
            other.rotation.Velocity = velocityA;

            //todo: maybe push them out a bit depending on penetration
        }
    }
}

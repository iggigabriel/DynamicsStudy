using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynamics.Samples
{
    public class FollowSample : MonoBehaviour
    {
        public Transform follower;
        public Transform target;

        public SODFloat3 position;

        public bool useGraphFromUi;
        public GraphRenderer graphDrawer;

        void Start()
        {
            if (follower)
            {
                position.Reset(follower.position);
            }
        }

        void Update()
        {
            if (Input.GetMouseButton(0) &&
                gameObject == CameraController.ActiveObject &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(Vector3.up, target.position);

                if (plane.Raycast(ray, out var hit))
                {
                    target.position = ray.GetPoint(hit);
                }
            }

            if (graphDrawer && useGraphFromUi)
            {
                position.Curve = graphDrawer.curve;
            }

            if (target && follower)
            {
                position.Target = target.position;
                position.Update(Time.deltaTime);
                follower.position = position.Value;
            }
        }
    }
}

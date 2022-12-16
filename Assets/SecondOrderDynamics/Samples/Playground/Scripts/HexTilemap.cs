using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class HexTilemap : MonoBehaviour
    {
        public Camera targetCamera;

        public float dragTreshold = 0.3f;

        public LayerMask hexLayer;

        public Transform tileMarker;

        public RectInt grid;

        readonly HexTileDragEvent activeDragEvent = new();

        HexTile pointerTarget;

        Ray CurrentRay => targetCamera.ScreenPointToRay(Input.mousePosition);

        bool IsDragging => activeDragEvent.target != null;

        private void Start()
        {
            foreach (var tilePos in grid.allPositionsWithin)
            {
                if (tilePos == grid.min || tilePos == grid.max - Vector2Int.one) continue;

                var pos = HexMath.HexToWorld3D(new float2(tilePos.x, tilePos.y));
                Instantiate(tileMarker, (float3)transform.position + pos, Quaternion.identity, transform);
            }
        }

        public static float3 GetRayPoint(Ray ray, float height = 0f)
        {
            var plane = new Plane(HexMath.Up, -height);
            if (plane.Raycast(ray, out var enter)) return ray.GetPoint(enter);
            return default;
        }

        HexTile CurrentObjectUnderPointer()
        {
            if (Physics.Raycast(CurrentRay, out var hitInfo))
            {
                return hitInfo.collider.GetComponentInParent<HexTile>();
            }

            return default;
        }

        private void Update()
        {
            var currentPointerTarget = CurrentObjectUnderPointer();

            if (pointerTarget != currentPointerTarget)
            {
                pointerTarget = currentPointerTarget;
            }

            if (IsDragging)
            {
                if (!Input.GetMouseButton(0))
                {
                    activeDragEvent.target.OnDragEnd(activeDragEvent);
                    activeDragEvent.target = null;
                }
                else
                {
                    activeDragEvent.currentRay = CurrentRay;

                    if (!activeDragEvent.isInitialized)
                    {
                        var pointA = GetRayPoint(activeDragEvent.startRay);
                        var pointB = GetRayPoint(activeDragEvent.currentRay);

                        if (math.distance(pointA, pointB) > dragTreshold)
                        {
                            activeDragEvent.isInitialized = true;
                        }
                    }

                    activeDragEvent.target.OnDrag(activeDragEvent);
                }
            }
            else
            {
                if (pointerTarget && Input.GetMouseButtonDown(0))
                {
                    activeDragEvent.manager = this;
                    activeDragEvent.camera = targetCamera;
                    activeDragEvent.target = pointerTarget;
                    activeDragEvent.startRay = CurrentRay;
                    activeDragEvent.currentRay = activeDragEvent.startRay;
                    activeDragEvent.targetOffset = (pointerTarget.transform.position.Flatten() - activeDragEvent.GetStartPosition()) / 2f;
                    activeDragEvent.isInitialized = false;
                    activeDragEvent.targetDropPosition = null;

                    pointerTarget.OnDragStart(activeDragEvent);
                }
            }
        }
    }

    public class HexTileDragEvent
    {
        public HexTilemap manager;
        public Camera camera;

        public HexTile target;
        public float3 targetOffset;

        public Ray startRay;
        public Ray currentRay;

        public float2? targetDropPosition;

        public bool isInitialized;

        public float3 GetStartPosition(float height = 0f) => HexTilemap.GetRayPoint(startRay, height);
        public float3 GetCurrentPosition(float height = 0f) => HexTilemap.GetRayPoint(currentRay, height) + targetOffset;

        public float3 GetTranslation() => HexTilemap.GetRayPoint(currentRay) - HexTilemap.GetRayPoint(startRay);
    }
}
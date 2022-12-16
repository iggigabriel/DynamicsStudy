using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class HexTile : MonoBehaviour
    {
        public float2 hexPos;

        public SODFloat3 positionMod;
        public SODFloat3 rotationMod;
        public SODFloat dragMod;

        public float dragTorque = 4f;
        public float bounceForce = 5f;

        bool isDragging;
        Vector3 dragStartPosition;

        private void Awake()
        {
            positionMod.Reset(transform.position);
            rotationMod.Reset(HexMath.Up);
        }

        private void Update()
        {
            positionMod.Update(Time.deltaTime);
            rotationMod.Update(Time.deltaTime);
            dragMod.Update(Time.deltaTime);

            if (positionMod.Value.y < 0f && !isDragging)
            {
                positionMod.PreviousValue = positionMod.Value.Flatten();
                positionMod.Velocity *= new float3(1f, -bounceForce, 1f);
            }

            rotationMod.Velocity += dragTorque * Time.deltaTime * positionMod.Velocity.Flatten();

            var rotationOffset = (HexMath.Up - rotationMod.Value).Flatten();

            transform.position = positionMod.Value + (HexMath.Up * math.length(rotationOffset) - rotationOffset / 5f) * (1f - dragMod.Value);

            transform.up = rotationMod.Value;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                transform.localPosition = HexMath.HexToWorld3D(hexPos);
            }
        }

        public void OnDrag(HexTileDragEvent drag)
        {
            if (drag.isInitialized)
            {
                positionMod.Target = drag.GetCurrentPosition(1.2f) + HexMath.Up * 0.6f;
                rotationMod.Target = HexMath.Up;
                dragMod.Target = 1.2f;
            }
            else
            {
                var translation = drag.GetTranslation();

                positionMod.Target = new float3(dragStartPosition.x, 0f, dragStartPosition.z);
                rotationMod.Target = math.normalize(HexMath.Up + translation / 2f);
            }
        }

        public void OnDragStart(HexTileDragEvent drag)
        {
            dragStartPosition = positionMod.Target;
            isDragging = true;
        }

        public void OnDragEnd(HexTileDragEvent drag)
        {
            isDragging = false;
            dragMod.Target = 0f;

            if (drag.isInitialized)
            {
                if (!drag.targetDropPosition.HasValue)
                {
                    var targetPosition = drag.GetCurrentPosition(0.6f).Flatten();
                    targetPosition = HexMath.HexToWorld3D(math.round(HexMath.World3DToHex(targetPosition)));

                    positionMod.Target = targetPosition;
                    rotationMod.Velocity += rotationMod.Value * 2f;
                }

                rotationMod.Target = HexMath.Up;
            }
            else
            {
                positionMod.Target = dragStartPosition;
                rotationMod.Target = HexMath.Up;
            }
        }
    }
}

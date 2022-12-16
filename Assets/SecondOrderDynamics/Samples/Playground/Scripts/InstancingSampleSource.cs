using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class InstancingSampleSource : MonoBehaviour
    {
        public SODFloat3 positionDriver;

        public float interval = 2f;
        public float distance = 1f;

        float3 position;

        IEnumerator Start()
        {
            position = transform.position;
            positionDriver.Reset(position);

            var timer = new WaitForSeconds(interval);

            while (true)
            {
                yield return timer;
                positionDriver.Target = position + (float3)UnityEngine.Random.insideUnitSphere * distance;
            }
        }

        void Update()
        {
            positionDriver.Update(Time.deltaTime);
            transform.position = positionDriver.Value;
        }
    }

}

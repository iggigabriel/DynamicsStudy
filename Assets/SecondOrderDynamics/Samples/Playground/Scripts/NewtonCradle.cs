using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class NewtonCradle : MonoBehaviour
    {
        readonly List<NewtonSphere> spheres = new();

        void Update()
        {
            GetComponentsInChildren(spheres);

            float totalUpdateTime = Time.deltaTime;

            while (totalUpdateTime > 0.0001f)
            {
                //split the update on several iterations because cradle needs enhanced precision, target updates/sec: 300
                var dt = Mathf.Min(1f / 300f, totalUpdateTime); 
                totalUpdateTime -= dt;

                foreach (var sphere in spheres) sphere.UpdateRotation(dt);

                CheckCollision();
            }
        }

        void CheckCollision()
        {
            for (int a = 0; a < spheres.Count; a++)
            {
                for (int b = a + 1; b < spheres.Count; b++)
                {
                    var sphereA = spheres[a];
                    var sphereB = spheres[b];

                    if (sphereB.CenterPosition.x < sphereA.CenterPosition.x)
                    {
                        sphereA = spheres[b];
                        sphereB = spheres[a];
                    }

                    var distance = math.length(sphereA.CenterPosition - sphereB.CenterPosition) - (sphereA.CenterRadius + sphereB.CenterRadius);

                    if (distance > 0f) continue; //not touching

                    var velocityA = sphereA.rotation.Velocity;
                    var velocityB = sphereB.rotation.Velocity;

                    if (velocityA <= 0f && velocityB >= 0f) continue; //traveling in opposing directions
                    if (math.sign(velocityA) == math.sign(velocityB) && velocityA < velocityB) continue; //already collided

                    sphereA.OnCollision(sphereB, distance); //boom!
                }
            }
        }
    }
}

using Dynamics;
using UnityEngine;
using UnityEngine.UI;

namespace Dynamics.Samples
{
    public class GraphRenderer : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public Vector2 graphSize;
        public float graphTime = 2f;

        readonly Vector3[] linePositions = new Vector3[200];

        public Slider fSlider;
        public Slider dSlider;
        public Slider rSlider;

        public SODCurve curve = SODCurve.Default;
        readonly SODFloat iterator = new();

        private void Start()
        {
            lineRenderer.positionCount = 200;
        }

        void Update()
        {
            curve.Set(fSlider.value, dSlider.value, rSlider.value);

            iterator.Curve = curve;
            iterator.Reset(0f);
            iterator.Target = 1f;

            var t = 0f;
            var dt = graphTime / (linePositions.Length - 1);

            for (int i = 0; i < linePositions.Length; i++)
            {
                linePositions[i] = new Vector3(t * graphSize.x / graphTime, iterator.Value * graphSize.y, 0f);
                t += dt;
                iterator.Update(dt);
            }

            lineRenderer.SetPositions(linePositions);
        }
    }
}


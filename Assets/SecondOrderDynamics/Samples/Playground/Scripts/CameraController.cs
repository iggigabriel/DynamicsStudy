using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Dynamics.Samples
{
    public class CameraController : MonoBehaviour
    {
        public static GameObject ActiveObject;

        public SODFloat targetAngle;
        public SODFloat targetPitch;
        public SODFloat3 targetLookFrom;
        public SODFloat3 targetLookAt;

        public float rotationSpeed = 180f;
        public float pitchSpeed = 10f;

        public float lookFromHeightOffset = 10f;
        public float lookFromDistance = 10f;
        public float lookAtHeightOffset = 10f;

        public Playground[] playgrounds;

        int activePlaygroundIndex;

        public PostProcessProfile postProcessProfileHighQuality;
        public PostProcessProfile postProcessProfileLowQuality;

        [Range(0, 200)]
        public int targetFrameRate;

        private void Start()
        {
#if UNITY_WEBGL
            GetComponent<PostProcessVolume>().sharedProfile = postProcessProfileLowQuality;
#else
            GetComponent<PostProcessVolume>().sharedProfile = postProcessProfileHighQuality;
#endif

            targetAngle.Reset(targetAngle.PreviousValue);
            targetPitch.Reset(lookFromHeightOffset);
            targetLookAt.Reset(playgrounds[activePlaygroundIndex].transform.position + Vector3.up * lookAtHeightOffset);
            targetLookFrom.Reset(GetLookFromPosition());
            Update();
        }

        void Update()
        {
            if (Application.isEditor)
            {
                Application.targetFrameRate = targetFrameRate;
            }

            targetPitch.Update(Time.deltaTime);
            targetAngle.Update(Time.deltaTime);
            targetLookFrom.Update(Time.deltaTime);
            targetLookAt.Update(Time.deltaTime);

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                targetAngle.Target -= Time.deltaTime * rotationSpeed;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                targetAngle.Target += Time.deltaTime * rotationSpeed;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                targetPitch.Target += Time.deltaTime * pitchSpeed;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                targetPitch.Target -= Time.deltaTime * pitchSpeed;
            }

            targetPitch.Target = Mathf.Clamp(targetPitch.Target, 2f, 20f);
            lookFromHeightOffset = targetPitch.Value;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                while(true)
                {
                    activePlaygroundIndex = (activePlaygroundIndex + 1) % playgrounds.Length;
                    targetPitch.Target = playgrounds[activePlaygroundIndex].cameraHeightTarget;
#if UNITY_WEBGL
                    if (!playgrounds[activePlaygroundIndex].supportedInWebGL) continue;
#endif
                    break;
                }
            }

            ActiveObject = playgrounds[activePlaygroundIndex].gameObject;

            targetLookAt.Target = playgrounds[activePlaygroundIndex].transform.position + Vector3.up * lookAtHeightOffset;
            targetLookFrom.Target = GetLookFromPosition();

            transform.position = targetLookFrom.Target;
            transform.LookAt(targetLookAt.Value);
        }

        Vector3 GetLookFromPosition()
        {
            return (Vector3)targetLookAt.Value + DegreeToVector(targetAngle.Value) * lookFromDistance + Vector3.up * lookFromHeightOffset;
        }

        static Vector3 RadianToVector(float radian)
        {
            return new Vector3(Mathf.Cos(radian), 0f, Mathf.Sin(radian));
        }

        static Vector3 DegreeToVector(float degree)
        {
            return RadianToVector(degree * Mathf.Deg2Rad);
        }
    }
}

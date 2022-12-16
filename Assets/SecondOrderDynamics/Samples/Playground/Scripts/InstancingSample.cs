#if UNITY_EDITOR || !UNITY_WEBGL
#define INSTANCING_SUPPORTED
#endif

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Dynamics.Samples
{
    public class InstancingSample : MonoBehaviour
    {
        public Transform source;
        public Transform target;

        public Mesh instanceMesh;
        public Material instanceMaterial;

        public int instanceCount = 10;
        public float gridSize = 6f;

        public uint instanceSeed = 1;
        public float instanceScale = 0.5f;

        int InstanceTotalCount => instanceCount * instanceCount;

        ComputeBuffer positionBuffer;
        ComputeBuffer drawArgsBuffer;

        NativeArray<float4> positions;
        NativeArray<SODState<float3>> positionDriversA;
        NativeArray<SODState<float3>> positionDriversB;
        bool positionDriverBuffer;

        uint[] drawArgs;
        Bounds drawBounds;

        float3 lastSourcePos;

#if INSTANCING_SUPPORTED
        void Start()
        {
            positions = new NativeArray<float4>(InstanceTotalCount, Allocator.Persistent);
            positionDriversA = new NativeArray<SODState<float3>>(InstanceTotalCount, Allocator.Persistent);
            positionDriversB = new NativeArray<SODState<float3>>(InstanceTotalCount, Allocator.Persistent);

            drawArgs = new uint[]
            {
                instanceMesh.GetIndexCount(0), 
                (uint)InstanceTotalCount,
                instanceMesh.GetIndexStart(0),
                instanceMesh.GetBaseVertex(0),
                0,
                0,
            };

            drawBounds = new Bounds(target.position, new float3(gridSize, gridSize, gridSize));

            positionBuffer = new ComputeBuffer(InstanceTotalCount, sizeof(float) * 4);
            drawArgsBuffer = new ComputeBuffer(1, drawArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            drawArgsBuffer.SetData(drawArgs);

            InitializeDriverConstants();
        }

        private void OnDestroy()
        {
            positions.Dispose();
            positionDriversA.Dispose();
            positionDriversB.Dispose();

            positionBuffer?.Release();
            positionBuffer = null;

            drawArgsBuffer?.Release();
            drawArgsBuffer = null;
        }

        void InitializeDriverConstants()
        {
            var curve = SODCurve.Default;
            var random = Unity.Mathematics.Random.CreateFromIndex(instanceSeed);

            float3 targetPos = target.position;
            lastSourcePos = source.position;

            for (int i = 0; i < positionDriversA.Length; i++)
            {
                var state = positionDriversA[i];

                state.Reset(targetPos + GetPositionFromIndex(i));

                var k = random.NextFloat3();

                curve.Set(math.lerp(0.1f, 3f, k.x), math.lerp(0.01f, 2f, k.x), math.lerp(-3f, 3f, k.x));

                state.kValues = curve.KValues;

                positionDriversA[i] = state;
            }
        }

        float3 GetPositionFromIndex(int index)
        {
            var x = index % instanceCount;
            var y = index / instanceCount;

            return new float3(gridSize / instanceCount * x * 2f - gridSize, 0f, gridSize / instanceCount * y * 2f - gridSize);
        }

        void Update()
        {
            if (CameraController.ActiveObject != gameObject) return;

            var jobHandle = default(JobHandle);

            var inBuffer = positionDriverBuffer ? positionDriversB : positionDriversA;
            var outBuffer = positionDriverBuffer ? positionDriversA : positionDriversB;
            positionDriverBuffer = !positionDriverBuffer;

            var deltaPos = (float3)source.position - lastSourcePos;
            lastSourcePos = source.position;

            // update drivers
            jobHandle = new UpdateDriversJob(Time.deltaTime, deltaPos, inBuffer, outBuffer).Schedule(InstanceTotalCount, 64, jobHandle);

            // update positions
            jobHandle = new UpdatePositionsJob(instanceScale, outBuffer, positions).Schedule(InstanceTotalCount, 64, jobHandle);

            jobHandle.Complete();

            // update buffer
            positionBuffer.SetData(positions);
            instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

            Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, drawBounds, drawArgsBuffer);
        }

        struct UpdatePositionsJob : IJobParallelFor
        {
            public readonly float scale;

            [ReadOnly]
            public NativeArray<SODState<float3>> positionDrivers;

            [WriteOnly]
            public NativeArray<float4> positions;

            public UpdatePositionsJob(float scale, NativeArray<SODState<float3>> positionDrivers, NativeArray<float4> positions)
            {
                this.scale = scale;
                this.positionDrivers = positionDrivers;
                this.positions = positions;
            }

            public void Execute(int i)
            {
                positions[i] = new float4(positionDrivers[i].value, scale);
            }
        }

        struct UpdateDriversJob : IJobParallelFor
        {
            public readonly float deltaTime;
            public readonly float3 deltaPos;

            [ReadOnly]
            public NativeArray<SODState<float3>> inState;

            [WriteOnly]
            public NativeArray<SODState<float3>> outState;

            public UpdateDriversJob(float deltaTime, float3 deltaPos, NativeArray<SODState<float3>> inState, NativeArray<SODState<float3>> outState)
            {
                this.deltaTime = deltaTime;
                this.deltaPos = deltaPos;
                this.inState = inState;
                this.outState = outState;
            }

            public void Execute(int i)
            {
                var state = inState[i];

                state.target += deltaPos;

                SecondOrderDynamics.UpdateState(ref state, deltaTime);

                outState[i] = state;
            }
        }
#endif

    }
}


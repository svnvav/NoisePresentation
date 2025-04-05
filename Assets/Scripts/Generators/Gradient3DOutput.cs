using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class Gradient3DOutput : MonoBehaviour
    {
        private static int
            hashesId = Shader.PropertyToID("Hashes"),
            gradientsId = Shader.PropertyToID("Gradients"),
            noiseScaleId = Shader.PropertyToID("NoiseScale"),
            octavesId = Shader.PropertyToID("Octaves"),
            lacunarityId = Shader.PropertyToID("Lacunarity"),
            factorsId = Shader.PropertyToID("Factors"),
            thresoldsId = Shader.PropertyToID("Thresholds"),
            zSliceOffsetId = Shader.PropertyToID("ZSliceOffset"),
            configId = Shader.PropertyToID("Config");

        [SerializeField] private Mesh _instanceMesh;

        [SerializeField] private Material _material;

        [SerializeField, Range(1, 256)] private int _resolution = 64;

        [SerializeField, Range(0f, 1f)] private float _alphaMultiplier = 0.0f;

        [SerializeField] private Gradient2DOutput _noise2DOutput;


        private ComputeBuffer _hashBuffer, _gradientsBuffer;

        private MaterialPropertyBlock _propertyBlock;

        public Gradient2DOutput Gradient2DOutput => _noise2DOutput;

        private void OnEnable () {
            _hashBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            _hashBuffer.SetData(NoiseUtility.Hashes);
            _gradientsBuffer = new ComputeBuffer(NoiseUtility.Gradients.Length, sizeof(float) * 3, ComputeBufferType.Default);
            _gradientsBuffer.SetData(NoiseUtility.Gradients);

            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.SetBuffer(hashesId, _hashBuffer);
            _propertyBlock.SetBuffer(gradientsId, _gradientsBuffer);
        }

        private void OnDisable () {
            _hashBuffer.Release();
            _hashBuffer = null;
            _gradientsBuffer.Release();
            _gradientsBuffer = null;
        }

        private void OnValidate () {
            if ((_hashBuffer != null || _gradientsBuffer != null) && enabled) {
                OnDisable();
                OnEnable();
            }
        }
        
        public void ApplyResolution(float value)
        {
            _resolution = Mathf.RoundToInt(value);
        }
        
        public void ApplyAlphaMultiplier(float value)
        {
            _alphaMultiplier = value;
        }

        private void Update () {
            
            _propertyBlock.SetVector(configId, new Vector4(
                _resolution, 
                1f / _resolution,
                _alphaMultiplier,
                _noise2DOutput.Persistence
            ));
            _propertyBlock.SetVector(factorsId, _noise2DOutput.Factors);
            _propertyBlock.SetVector(thresoldsId, _noise2DOutput.Thresholds);
            _propertyBlock.SetInt(noiseScaleId, _noise2DOutput.NoiseScale.x);
            _propertyBlock.SetInt(octavesId, _noise2DOutput.Octaves);
            _propertyBlock.SetInt(lacunarityId, _noise2DOutput.Lacunarity);
            _propertyBlock.SetFloat(zSliceOffsetId, _noise2DOutput.ZSliceOffset);

            Graphics.DrawMeshInstancedProcedural(
                _instanceMesh, 0, _material, new Bounds(Vector3.zero, Vector3.one),
                _resolution * _resolution * _resolution, _propertyBlock
            );
        }
    }
}
using System;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SimplexGradient3DOutput : MonoBehaviour
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

        [SerializeField] private int _noiseScale = 4;
        
        [SerializeField, Range(1, 256)] private int _resolution = 128;

        [SerializeField, Range(0f, 1f)] private float _alphaMultiplier = 0.02f;

        private ComputeBuffer _hashBuffer, _gradientsBuffer;
        private MaterialPropertyBlock _propertyBlock;
        
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;
        private float _skewFactor = 1f;
        private float _continuityFactor = 1f;
        private float _randomFactor = 1f;
        private float _axisColorFactor = 0f;
        private float _xPosThreshold, _yPosThreshold, _zPosThreshold;
        private float _alphaThreshold = 0f;
        private float _zSliceOffset = -1;
        
        public Vector4 Factors => new Vector4(
            _axisColorFactor,
            _randomFactor,
            _continuityFactor,
            _skewFactor
        );

        public Vector4 Config => new Vector4(
            _resolution,
            1f / _resolution,
            _alphaMultiplier,
            _persistence
        );
        
        public int NoiseScale => _noiseScale;

        public int Octaves => _octaves;

        public int Lacunarity => _lacunarity;

        public Vector4 Thresholds => new Vector4(
            _xPosThreshold,
            _yPosThreshold,
            _zPosThreshold,
            _alphaThreshold
        );

        public float ZSliceOffset => _zSliceOffset;

        public event Action OnParameterUpdate;

        private void OnEnable () {
            _hashBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            _hashBuffer.SetData(NoiseUtility.Hashes);
            _gradientsBuffer = new ComputeBuffer(NoiseUtility.SimplexGradients.Length, sizeof(float) * 3, ComputeBufferType.Default);
            _gradientsBuffer.SetData(NoiseUtility.SimplexGradients);

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

        private void Update () {
            _propertyBlock.SetVector(configId, Config);
            _propertyBlock.SetVector(factorsId, Factors);
            _propertyBlock.SetVector(thresoldsId, Thresholds);
            _propertyBlock.SetInt(noiseScaleId, _noiseScale);
            _propertyBlock.SetInt(octavesId, _octaves);
            _propertyBlock.SetInt(lacunarityId, _lacunarity);
            _propertyBlock.SetFloat(zSliceOffsetId, _zSliceOffset);

            Graphics.DrawMeshInstancedProcedural(
                _instanceMesh, 0, _material, new Bounds(Vector3.zero, Vector3.one), 
                _resolution * _resolution * _resolution, _propertyBlock);
        }
        
        public void ApplySkew(float factorValue)
        {
            _skewFactor = factorValue;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyRandom(float factorValue)
        {
            _randomFactor = factorValue;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyContinuity(float factorValue)
        {
            _continuityFactor = factorValue;
            OnParameterUpdate?.Invoke();
        }

        public void ApplyResolution(float value)
        {
            _resolution = Mathf.RoundToInt(value);
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyOctaves(float octaves)
        {
            _octaves = Mathf.RoundToInt(octaves);
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyLacunarity(float lacunarity)
        {
            _lacunarity = Mathf.RoundToInt(lacunarity);
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyPersistence(float factorValue)
        {
            _persistence = factorValue;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAlphaThreshold(float value)
        {
            _alphaThreshold = value;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAlphaMultiplier(float value)
        {
            _alphaMultiplier = value;
        }
        
        public void ApplyAxisColor(float factorValue)
        {
            _axisColorFactor = factorValue;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAxisXThreshold(float value)
        {
            _xPosThreshold = value;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAxisYThreshold(float value)
        {
            _yPosThreshold = value;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAxisZThreshold(float value)
        {
            _zPosThreshold = value;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyZOffset(float value)
        {
            _zSliceOffset = value;
            OnParameterUpdate?.Invoke();
        }
    }
}
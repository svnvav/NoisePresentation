using System;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class Value3DOutput : MonoBehaviour
    {
        private static int
            hashesId = Shader.PropertyToID("Hashes"),
            noiseScaleId = Shader.PropertyToID("NoiseScale"),
            octavesId = Shader.PropertyToID("Octaves"),
            lacunarityId = Shader.PropertyToID("Lacunarity"),
            factorsId = Shader.PropertyToID("Factors"),
            thresoldsId = Shader.PropertyToID("Thresholds"),
            zSliceOffsetId = Shader.PropertyToID("ZSliceOffset"),
            configId = Shader.PropertyToID("Config");

        [SerializeField] private Mesh _instanceMesh;

        [SerializeField] private Material _material;

        [SerializeField, Range(1, 256)] private int _resolution = 128;
        
        [SerializeField] private int _noiseScale = 16;

        [SerializeField, Range(0f, 1f)] private float _multiplier = 1f;
        [SerializeField, Range(0f, 1f)] private float _alphaThreshold;
        
        private float _randomFactor = 1f;
        private float _continuityFactor = 1f;
        private float _easingFactor = 1f;
        private float _axisColorFactor;
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;
        private float _xPosThreshold, _yPosThreshold, _zPosThreshold;
        private float _zSliceOffset = -1;

        private ComputeBuffer _hashBuffer;

        private MaterialPropertyBlock _propertyBlock;
        
        public int NoiseScale => _noiseScale;

        public Vector4 Factors => new Vector4(
            _axisColorFactor, 
            _randomFactor,
            _continuityFactor,
            _easingFactor
        );

        public Vector4 Config => new Vector4(
            _resolution,
            1f / _resolution,
            _multiplier,
            _persistence
        );
        
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

            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.SetBuffer(hashesId, _hashBuffer);
        }

        private void OnDisable () {
            _hashBuffer.Release();
            _hashBuffer = null;
        }

        private void OnValidate () {
            if (_hashBuffer != null && enabled) {
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
                _resolution * _resolution * _resolution, _propertyBlock
            );
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
        
        public void ApplyEasing(float factorValue)
        {
            _easingFactor = factorValue;
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
        
        public void ApplyThreshold(float value)
        {
            _alphaThreshold = value;
            OnParameterUpdate?.Invoke();
        }
        
        public void ApplyAlphaMultiplier(float value)
        {
            _multiplier = value;
            OnParameterUpdate?.Invoke();
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
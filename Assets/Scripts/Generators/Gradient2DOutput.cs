
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Gradient2DOutput : MonoBehaviour
    {
        [SerializeField] private Vector2Int _noiseScale;
        [SerializeField] private RawImage[] _targets;
        [SerializeField] private ComputeShader _computeShader;

        private RenderTexture _outputRt;
        
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;
        private float _axisColorFactor;
        private float _randomFactor = 1f;
        private float _easingFactor = 1f;
        private float _xPosThreshold, _yPosThreshold, _zPosThreshold;
        private float _alphaThreshold;
        private float _zSliceOffset = -1;

        public Vector2Int NoiseScale => _noiseScale;

        public Vector4 Factors => new Vector4(
            _axisColorFactor,
            _randomFactor,
            0f,
            _easingFactor
        );

        public int Octaves => _octaves;

        public int Lacunarity => _lacunarity;

        public float Persistence => _persistence;

        public Vector4 Thresholds => new Vector4(
            _xPosThreshold,
            _yPosThreshold,
            _zPosThreshold,
            _alphaThreshold
        );

        public float ZSliceOffset => _zSliceOffset;

        private void Awake()
        {
            _outputRt = new RenderTexture(512, 512, 24);
            _outputRt.enableRandomWrite = true;
            foreach (var target in _targets)
            {
                target.texture = _outputRt;
            }
        }
        
        public void ApplyRandom(float factorValue)
        {
            _randomFactor = factorValue;
            UpdateTargets();
        }
        
        public void ApplyEasing(float factorValue)
        {
            _easingFactor = factorValue;
            UpdateTargets();
        }

        public void ApplyOctaves(float octaves)
        {
            _octaves = Mathf.RoundToInt(octaves);
            UpdateTargets();
        }
        
        public void ApplyLacunarity(float lacunarity)
        {
            _lacunarity = Mathf.RoundToInt(lacunarity);
            UpdateTargets();
        }
        
        public void ApplyPersistence(float factorValue)
        {
            _persistence = factorValue;
            UpdateTargets();
        }
        
        public void ApplyAlphaThreshold(float value)
        {
            _alphaThreshold = value;
            UpdateTargets();
        }

        public void ApplyAxisColor(float factorValue)
        {
            _axisColorFactor = factorValue;
            UpdateTargets();
        }
        
        public void ApplyAxisXThreshold(float value)
        {
            _xPosThreshold = value;
            UpdateTargets();
        }
        
        public void ApplyAxisYThreshold(float value)
        {
            _yPosThreshold = value;
            UpdateTargets();
        }
        
        public void ApplyAxisZThreshold(float value)
        {
            _zPosThreshold = value;
            UpdateTargets();
        }
        
        public void ApplyZOffset(float value)
        {
            _zSliceOffset = value;
            UpdateTargets();
        }
        
        public void UpdateTargets()
        {
            var kernel = _computeShader.FindKernel("CSMain");

            var hashesBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            hashesBuffer.SetData(NoiseUtility.Hashes);
            _computeShader.SetBuffer(kernel, "Hashes", hashesBuffer);
            
            var gradientsBuffer = new ComputeBuffer(NoiseUtility.Gradients.Length, sizeof(float) * 3, ComputeBufferType.Default);
            gradientsBuffer.SetData(NoiseUtility.Gradients);
            _computeShader.SetBuffer(kernel, "Gradients", gradientsBuffer);
            
            _computeShader.SetInt("NoiseScale", _noiseScale.x);
            _computeShader.SetInt("Octaves", _octaves);
            _computeShader.SetInt("Lacunarity", _lacunarity);
            _computeShader.SetVector("Config", new Vector4(
                0f, 
                0f,
                0f,
                _persistence
            ));
            _computeShader.SetVector("Factors", Factors);
            _computeShader.SetVector("Thresholds", Thresholds);
            _computeShader.SetFloat("ZSliceOffset", _zSliceOffset);
            
            _computeShader.SetTexture(kernel, "Output", _outputRt);

            _computeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _computeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            hashesBuffer.Dispose();
            gradientsBuffer.Dispose();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SimplexValue2DOutput : MonoBehaviour
    {
        [SerializeField] private int _noiseScale = 8;
        [SerializeField] private RawImage _target;
        [SerializeField] private ComputeShader _computeShader;

        private RenderTexture _outputRt;
        private float _barycentricFactor = 0f;
        private float _skewFactor = 0f;
        private float _randomFactor = 0f;
        private float _continuityFactor = 0f;
        private float _easingFactor;
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;

        public int NoiseScale => _noiseScale;

        public float SkewFactor => _skewFactor;

        public float RandomFactor => _randomFactor;

        private void Awake()
        {
            _outputRt = new RenderTexture(512, 512, 24);
            _outputRt.enableRandomWrite = true;
            _target.texture = _outputRt;
        }
        
        public void ApplyBarycentric(float factorValue)
        {
            _barycentricFactor = factorValue;
            UpdateTargets();
        }
        
        public void ApplySkew(float factorValue)
        {
            _skewFactor = factorValue;
            UpdateTargets();
        }
        
        public void ApplyRandom(float factorValue)
        {
            _randomFactor = factorValue;
            UpdateTargets();
        }
        
        public void ApplyContinuity(float factorValue)
        {
            _continuityFactor = factorValue;
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
        
        public void UpdateTargets()
        {
            var kernel = _computeShader.FindKernel("CSMain");

            var hashesBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            hashesBuffer.SetData(NoiseUtility.Hashes);
            _computeShader.SetBuffer(kernel, "Hashes", hashesBuffer);
            _computeShader.SetInt("NoiseScale", _noiseScale);
            _computeShader.SetFloat("BarycentricFactor", _barycentricFactor);
            _computeShader.SetFloat("SkewFactor", _skewFactor);
            _computeShader.SetFloat("RandomFactor", _randomFactor);
            _computeShader.SetFloat("ContinuityFactor", _continuityFactor);
            //_computeShader.SetFloat("EasingFactor", _easingFactor);
            _computeShader.SetInt("Octaves", _octaves);
            _computeShader.SetInt("Lacunarity", _lacunarity);
            _computeShader.SetFloat("Persistence", _persistence);
            
            _computeShader.SetTexture(kernel, "Output", _outputRt);

            _computeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _computeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            hashesBuffer.Dispose();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Value2DOutput : MonoBehaviour
    {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private RawImage[] _targets;
        [SerializeField] private ComputeShader _valueNoise2D;

        private RenderTexture _outputRt;
        private float _randomFactor;
        private float _continuityFactor;
        private float _easingFactor;
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;

        public Vector2Int Size => _size;

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
            var kernel = _valueNoise2D.FindKernel("CSMain");

            var hashesBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            hashesBuffer.SetData(NoiseUtility.Hashes);
            _valueNoise2D.SetBuffer(kernel, "Hashes", hashesBuffer);
            _valueNoise2D.SetInt("NoiseScale", _size.x);
            _valueNoise2D.SetFloat("RandomFactor", _randomFactor);
            _valueNoise2D.SetFloat("ContinuityFactor", _continuityFactor);
            _valueNoise2D.SetFloat("EasingFactor", _easingFactor);
            _valueNoise2D.SetInt("Octaves", _octaves);
            _valueNoise2D.SetInt("Lacunarity", _lacunarity);
            _valueNoise2D.SetFloat("Persistence", _persistence);
            
            _valueNoise2D.SetTexture(kernel, "Output", _outputRt);

            _valueNoise2D.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _valueNoise2D.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            hashesBuffer.Dispose();
        }
    }
}
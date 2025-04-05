
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Cellular2DOutput : MonoBehaviour
    {
        [SerializeField] private int _noiseScale = 8;
        [SerializeField] private RawImage[] _targets;
        [SerializeField] private ComputeShader _computeShader;

        private RenderTexture _outputRt;
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;
        private float _randomFactor = 1f;

        public int NoiseScale => _noiseScale;

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

            var pointsBuffer = new ComputeBuffer(NoiseUtility.PointCount, sizeof(float) * 3, ComputeBufferType.Default);
            pointsBuffer.SetData(NoiseUtility.Points);
            _computeShader.SetBuffer(kernel, "Points", pointsBuffer);

            _computeShader.SetInt("PointCount", NoiseUtility.PointCount);
            
            _computeShader.SetInt("NoiseScale", _noiseScale);
            _computeShader.SetInt("Octaves", _octaves);
            _computeShader.SetInt("Lacunarity", _lacunarity);
            _computeShader.SetFloat("Persistence", _persistence);
            _computeShader.SetFloat("RandomFactor", _randomFactor);

            _computeShader.SetTexture(kernel, "Output", _outputRt);

            _computeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _computeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            pointsBuffer.Dispose();
        }
    }
}
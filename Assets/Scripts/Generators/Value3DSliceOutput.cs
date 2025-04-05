
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Value3DSliceOutput : MonoBehaviour
    {
        [SerializeField] private Value3DOutput _value3DOutput;
        [SerializeField] private RawImage[] _targets;
        [SerializeField] private ComputeShader _computeShader;

        private RenderTexture _outputRt;

        private void Awake()
        {
            _outputRt = new RenderTexture(512, 512, 24);
            _outputRt.enableRandomWrite = true;
            foreach (var target in _targets)
            {
                target.texture = _outputRt;
            }
        }

        private void OnEnable()
        {
            _value3DOutput.OnParameterUpdate += UpdateTargets;
        }

        private void OnDisable()
        {
            _value3DOutput.OnParameterUpdate -= UpdateTargets;
        }

        public void UpdateTargets()
        {
            var kernel = _computeShader.FindKernel("CSMain");

            var hashBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            hashBuffer.SetData(NoiseUtility.Hashes);
            _computeShader.SetBuffer(kernel, "Hashes", hashBuffer);
            _computeShader.SetInt("NoiseScale", _value3DOutput.NoiseScale);
            _computeShader.SetInt("Octaves", _value3DOutput.Octaves);
            _computeShader.SetInt("Lacunarity", _value3DOutput.Lacunarity);
            _computeShader.SetVector("Config", _value3DOutput.Config);
            _computeShader.SetVector("Factors", _value3DOutput.Factors);
            _computeShader.SetVector("Thresholds", _value3DOutput.Thresholds);
            
            _computeShader.SetFloat("ZSliceOffset", _value3DOutput.ZSliceOffset);
            
            _computeShader.SetTexture(kernel, "Output", _outputRt);

            _computeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _computeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            hashBuffer.Dispose();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SimplexValue3DSliceOutput : MonoBehaviour
    {
        [SerializeField] private SimplexValue3DOutput _output3D;
        [SerializeField] private RawImage _target;
        [SerializeField] private ComputeShader _computeShader;

        private RenderTexture _outputRt;

        private void Awake()
        {
            _outputRt = new RenderTexture(512, 512, 24);
            _outputRt.enableRandomWrite = true;
            _target.texture = _outputRt;
        }

        private void OnEnable()
        {
            _output3D.OnParameterUpdate += UpdateTargets;
        }

        private void OnDisable()
        {
            _output3D.OnParameterUpdate -= UpdateTargets;
        }

        public void UpdateTargets()
        {
            var kernel = _computeShader.FindKernel("CSMain");

            var hashBuffer = new ComputeBuffer(NoiseUtility.Hashes.Length, sizeof(uint), ComputeBufferType.Default);
            hashBuffer.SetData(NoiseUtility.Hashes);
            _computeShader.SetBuffer(kernel, "Hashes", hashBuffer);
            _computeShader.SetInt("NoiseScale", _output3D.NoiseScale);
            _computeShader.SetInt("Octaves", _output3D.Octaves);
            _computeShader.SetInt("Lacunarity", _output3D.Lacunarity);
            _computeShader.SetVector("Config", _output3D.Config);
            _computeShader.SetVector("Factors", _output3D.Factors);
            _computeShader.SetVector("Thresholds", _output3D.Thresholds);
            
            _computeShader.SetFloat("ZSliceOffset", _output3D.ZSliceOffset);
            
            _computeShader.SetTexture(kernel, "Output", _outputRt);

            _computeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _computeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            hashBuffer.Dispose();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Value1DOutput : MonoBehaviour
    {
        [SerializeField] private RawImage[] _targets;
        [SerializeField] private ComputeShader _solidColorComputeShader;
        [SerializeField] private ComputeShader _stripesComputeShader;
        
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

        public void SolidColor(float value)
        {
            var kernel = _solidColorComputeShader.FindKernel("CSMain");
            _solidColorComputeShader.SetVector("Color", new Color(value, value, value));
            _solidColorComputeShader.SetTexture(kernel, "Output", _outputRt);

            _solidColorComputeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _solidColorComputeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
        }

        public void Stripes(Color[] key)
        {
            var kernel = _stripesComputeShader.FindKernel("CSMain");

            var colorsBuffer = new ComputeBuffer(key.Length, sizeof(float) * 4, ComputeBufferType.Default);
            colorsBuffer.SetData(key);
            _stripesComputeShader.SetBuffer(kernel, "Colors", colorsBuffer);
            _stripesComputeShader.SetInt("Count", key.Length);
            _stripesComputeShader.SetTexture(kernel, "Output", _outputRt);

            _stripesComputeShader.GetKernelThreadGroupSizes(kernel, out var sizeX, out var sizeY, out var sizeZ);
            _stripesComputeShader.Dispatch(kernel, _outputRt.width / (int) sizeX, _outputRt.height / (int) sizeY, (int) sizeZ);
            
            colorsBuffer.Dispose();
        }
    }
}
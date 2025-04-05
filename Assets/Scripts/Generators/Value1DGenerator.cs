using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Plarium.Tools.NoisePresentation
{
    public class Value1DGenerator : MonoBehaviour
    {
        [SerializeField] private Graph _graph;
        [SerializeField] private Value1DOutput _output;

        private float[] _randomValues;
        private float _randomFactor;
        private float _linearFactor;
        private float _easingFactor;
        private float _closingPointFactor;
        private float _tilingFactor;
        private int _octaves = 1;
        private int _lacunarity = 2;
        private float _persistence = 0.5f;

        public void ApplyRandom(float factorValue)
        {
            _randomFactor = factorValue;
            UpdateOutput();
        }

        public void ApplyLinear(float factorValue)
        {
            _linearFactor = factorValue;
            UpdateOutput();
        }

        public void ApplyEasing(float factorValue)
        {
            _easingFactor = factorValue;
            UpdateOutput();
        }

        public void ApplyClosingPointFactor(float factorValue)
        {
            _closingPointFactor = factorValue;
            UpdateOutput();
        }

        public void ApplyTiling(float factorValue)
        {
            _tilingFactor = factorValue;
            UpdateOutput();
        }

        public void ApplyOctaves(float octaves)
        {
            _octaves = Mathf.RoundToInt(octaves);
            UpdateOutput();
        }

        public void ApplyLacunarity(float lacunarity)
        {
            _lacunarity = Mathf.RoundToInt(lacunarity);
            UpdateOutput();
        }

        public void ApplyPersistence(float factorValue)
        {
            _persistence = factorValue;
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            var keyPoints = _graph.KeyPoints;
            var scale = _graph.IsOpen ? keyPoints.Count : (keyPoints.Count - 1);
            var linePoints = _graph.LinePoints;

            var random = new Random(2);
            _randomValues = new float[256];
            for (int i = 0; i < _randomValues.Length; ++i)
            {
                _randomValues[i] = Mathf.Lerp(0.5f, (float)random.NextDouble(), _randomFactor);
            }

            if (!_graph.IsOpen)
            {
                var lastKeyValue = Mathf.Lerp(_randomValues[keyPoints.Count - 2], _randomValues[keyPoints.Count - 1], _closingPointFactor);
                _randomValues[keyPoints.Count - 1] = lastKeyValue;
            }

            var resultValues = new float[linePoints.Count];

            var keyValues = new float[keyPoints.Count];
            for (int i = 0; i < keyPoints.Count; i++)
            {
                var index = i;
                var indexTiled = i % scale;

                keyValues[i] = Mathf.Lerp(_randomValues[index], _randomValues[indexTiled], _tilingFactor);
                keyPoints[i].Set1DValue(keyValues[i]);
            }

            var h = linePoints.Count / scale;

            var ih = 1f / linePoints.Count;

            var lineValues = new float[linePoints.Count];
            for (int i = 0; i < linePoints.Count; i++)
            {
                var frequency = 1;
                var amplitude = 1f;
                var range = 1f;
                for (int o = 0; o < _octaves; o++)
                {
                    var x = i * ih * frequency * scale;

                    var index = Mathf.FloorToInt(x);
                    var t = x - index;
                    t = Mathf.Lerp(t, QuinticCurve(t), _easingFactor) * _linearFactor;

                    var keyIndex0 = index % scale;
                    var keyIndex1 = keyIndex0 + 1;
                    var keyIndex1Tiled = (keyIndex0 + 1) % scale;
                    if (_graph.IsOpen && keyIndex0 + 1 == keyPoints.Count)
                    {
                        keyIndex1 = keyIndex0;
                    }

                    var value0 = _randomValues[keyIndex0];
                    var value1 = Mathf.Lerp(_randomValues[keyIndex1], _randomValues[keyIndex1Tiled], _tilingFactor);
                    var value = Mathf.Lerp(value0, value1, t);
                    value *= amplitude;
                    lineValues[i] += value;

                    frequency *= _lacunarity;
                    amplitude *= _persistence;
                    range += amplitude;
                }

                range -= amplitude; //compensate
                lineValues[i] /= range;

                resultValues[i] = lineValues[i];
                linePoints[i].SetValue(resultValues[i]);
            }

            var colors = resultValues
                .Select(value => new Color(value, value, value))
                .ToArray();

            _output.Stripes(colors);
        }

        private static float QuinticCurve(float x)
        {
            return x * x * x * (x * (x * 6f - 15f) + 10f);
        }

        private static float HermineCurve(float x)
        {
            return x * x * (3.0f - 2.0f * x);
        }
    }
}
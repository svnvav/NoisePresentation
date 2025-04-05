using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideGraph1DClosingPoint : MonoBehaviour, ISlide
    {
        [SerializeField] private Graph _graph;
        [SerializeField] private Value1DGenerator _value1DGenerator;

        private KeyPoint _closingPoint;
        
        public IEnumerator DoEnter(float time)
        {
            var lastPoint = _graph.KeyPoints[_graph.KeyPoints.Count - 1];
            _closingPoint = _graph.CreateClosingPoint();
            var position = lastPoint.Position;
            position.x = 1f;
            _closingPoint.SetPosition(position, true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value1DGenerator.ApplyClosingPointFactor(t);
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _value1DGenerator.ApplyClosingPointFactor(1f);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value1DGenerator.ApplyClosingPointFactor(1f - t);
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _graph.RemoveClosingPoint(_closingPoint);
            yield return null;
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
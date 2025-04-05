using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideSimplexValue3DSlice : MonoBehaviour, ISlide
    {
        [SerializeField] private SimplexValue3DOutput _output3D;
        [SerializeField] private SimplexValue3DSliceOutput _sliceOutput;
        [SerializeField] private CanvasGroup _sliceCanvasGroup;
        [SerializeField] private CanvasGroup _mainCanvasGroup;

        private float _alphaThresholdCached;
        
        protected void Start()
        {
            _sliceCanvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            _sliceCanvasGroup.gameObject.SetActive(true);
            _sliceOutput.UpdateTargets();
            

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _sliceCanvasGroup.alpha = t;

                t += Time.deltaTime * dt;
                yield return null;
            }

            _sliceCanvasGroup.alpha = 1f;
        }
        
        public IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _sliceCanvasGroup.alpha = 1f - t;

                t += Time.deltaTime * dt;
                yield return null;
            }

            _sliceCanvasGroup.alpha = 0f;
            _output3D.ApplyZOffset(-1f);

            _sliceCanvasGroup.gameObject.SetActive(false);
        }
        
        public IEnumerator DoEnterFromBack(float time)
        {
            _sliceCanvasGroup.gameObject.SetActive(true);
            _mainCanvasGroup.gameObject.SetActive(true);
            _output3D.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _sliceCanvasGroup.alpha = t;
                _mainCanvasGroup.alpha = t;

                _output3D.ApplyAlphaThreshold(Mathf.Lerp(0f, _alphaThresholdCached, t));
                
                t += Time.deltaTime * dt;
                yield return null;
            }

            _sliceCanvasGroup.alpha = 1f;
            _mainCanvasGroup.alpha = 1f;

            _output3D.ApplyAlphaThreshold(_alphaThresholdCached);
        }

        public IEnumerator DoExit(float time)
        {
            _alphaThresholdCached = _output3D.Thresholds.w;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _sliceCanvasGroup.alpha = 1f - t;
                _mainCanvasGroup.alpha = 1f - t;

                _output3D.ApplyAlphaThreshold(Mathf.Lerp(_alphaThresholdCached, 0f, t));
                
                t += Time.deltaTime * dt;
                yield return null;
            }

            _sliceCanvasGroup.alpha = 0f;
            _mainCanvasGroup.alpha = 0f;

            _output3D.ApplyAlphaThreshold(0f);
            
            _sliceCanvasGroup.gameObject.SetActive(false);
            _mainCanvasGroup.gameObject.SetActive(false);
            _output3D.gameObject.SetActive(false);
        }
    }
}
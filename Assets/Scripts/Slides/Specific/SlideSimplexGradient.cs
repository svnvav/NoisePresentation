using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideSimplexGradient : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private SimplexGradient3DOutput _output3D;
        [SerializeField] private SimplexGradient3DSliceOutput _sliceOutput;
        [SerializeField] private Slider _alphaThresholdSlider;
        [SerializeField] private TitleChanger _titleChanger;
        [SerializeField] private CanvasGroup _links;

        private float _alphaThresholdCached;
        
        private void Start()
        {
            _mainCanvasGroup.gameObject.SetActive(false);
            _output3D.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Gradient 3D", time));
            _mainCanvasGroup.gameObject.SetActive(true);
            _output3D.gameObject.SetActive(true);
            _sliceOutput.UpdateTargets();

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _mainCanvasGroup.alpha = t;
                _alphaThresholdSlider.value = Mathf.Lerp(1f, _alphaThresholdCached, t);;

                t += Time.deltaTime * dt;
                yield return null;
            }

            _mainCanvasGroup.alpha = 1f;
            _alphaThresholdSlider.value = 0f;
        }

        public IEnumerator DoExit(float time)
        {
            _alphaThresholdCached = _output3D.Thresholds.w;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _mainCanvasGroup.alpha = 1f - t;
                _links.alpha = 1f - t;
                _alphaThresholdSlider.value = Mathf.Lerp(_alphaThresholdCached, 1f, t);

                t += Time.deltaTime * dt;
                yield return null;
            }

            _mainCanvasGroup.alpha = 0f;
            _alphaThresholdSlider.value = 1f;
            _links.alpha = 0f;
            
            _links.gameObject.SetActive(false);
            _mainCanvasGroup.gameObject.SetActive(false);
            _output3D.gameObject.SetActive(false);
        }

        public IEnumerator DoBack(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Value 3D", time));
            _alphaThresholdCached = _output3D.Thresholds.w;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _mainCanvasGroup.alpha = 1f - t;
                _alphaThresholdSlider.value = Mathf.Lerp(_alphaThresholdCached, 1f, t);

                t += Time.deltaTime * dt;
                yield return null;
            }

            _mainCanvasGroup.alpha = 0f;
            _alphaThresholdSlider.value = 1f;
            
            _mainCanvasGroup.gameObject.SetActive(false);
            _output3D.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Gradient 3D", time));
            _mainCanvasGroup.gameObject.SetActive(true);
            _output3D.gameObject.SetActive(true);
            _links.gameObject.SetActive(true);
            _sliceOutput.UpdateTargets();

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _mainCanvasGroup.alpha = t;
                _alphaThresholdSlider.value = Mathf.Lerp(1f, _alphaThresholdCached, t);
                _links.alpha = t;

                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _links.alpha = 1f;
            _mainCanvasGroup.alpha = 1f;
            _alphaThresholdSlider.value = 0f;
        }
    }
}
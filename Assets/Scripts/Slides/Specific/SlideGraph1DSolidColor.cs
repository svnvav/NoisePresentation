using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideGraph1DSolidColor : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _outputCanvasGroup;
        [SerializeField] private Slider _slider;
        [SerializeField] [Range(0f, 1f)] private float _defaultSliderValue;

        public float SliderValue => _slider.value;

        private void Awake()
        {
            _canvasGroup.gameObject.SetActive(false);
            _outputCanvasGroup.gameObject.SetActive(false);
            _slider.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            _canvasGroup.gameObject.SetActive(true);
            _outputCanvasGroup.gameObject.SetActive(true);
            
            _slider.gameObject.SetActive(true);
            _slider.value = _defaultSliderValue;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = t;
                _outputCanvasGroup.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 1f;
            _outputCanvasGroup.alpha = 1f;
        }
        
        public IEnumerator DoEnterFromBack(float time)
        {
            _canvasGroup.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
        }

        public IEnumerator DoExit(float time)
        {
            var oldSliderValue = _slider.value;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _slider.value = Mathf.Lerp(oldSliderValue, _defaultSliderValue, t);
                _canvasGroup.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);

            _slider.value = _defaultSliderValue;
        }

        public IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = 1f - t;
                _outputCanvasGroup.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _outputCanvasGroup.alpha = 0f;

            _canvasGroup.gameObject.SetActive(false);
            _outputCanvasGroup.gameObject.SetActive(false);
        }
        
        

    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideSlider : MonoBehaviour, ISlide
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private CanvasGroup _sliderCanvasGroup;
        [SerializeField] private float _defaultSliderValue;

        private void Start()
        {
            _sliderCanvasGroup.gameObject.SetActive(false);
            _slider.gameObject.SetActive(false);
        }

        public virtual IEnumerator DoEnter(float time)
        {
            _sliderCanvasGroup.gameObject.SetActive(true);
            _slider.gameObject.SetActive(true);
            _slider.value = 0f;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _sliderCanvasGroup.alpha = t;
                _slider.value = Mathf.Lerp(0f, _defaultSliderValue, t);
                t += Time.deltaTime * dt;
                yield return null;
            }

            _sliderCanvasGroup.alpha = 1f;
            _slider.value = _defaultSliderValue;
        }
        
        public virtual IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }

        public virtual IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public virtual IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            var sliderValue = _slider.value;
            while (t < 1.0f)
            {
                _sliderCanvasGroup.alpha = 1f - t;
                _slider.value = Mathf.Lerp(sliderValue, 0f, t);
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _sliderCanvasGroup.gameObject.SetActive(false);
            _slider.value = 0f;
            _slider.gameObject.SetActive(false);
        }
        
    }
}
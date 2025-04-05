using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideManySliders : MonoBehaviour, ISlide
    {
        [Serializable]
        private class SliderSetup
        {
            [SerializeField] private Slider _slider;
            [SerializeField] private float _targetSliderValue;
            
            private CanvasGroup _canvasGroup;
            private float _savedSliderValue;
            private float _savedAlphaValue;
            private bool _isActiveBefore;

            public Slider Slider => _slider;

            public float TargetSliderValue => _targetSliderValue;

            public CanvasGroup CanvasGroup => _canvasGroup;
            
            public float SavedSliderValue => _savedSliderValue;

            public bool IsActiveBefore => _isActiveBefore;

            public float SavedAlphaValue => _savedAlphaValue;

            public void Initialize()
            {
                _canvasGroup = _slider.GetComponent<CanvasGroup>();
                _canvasGroup.alpha = 0f;
                _slider.gameObject.SetActive(false);
            }

            public void SaveState()
            {
                _savedSliderValue = _slider.value;
                _isActiveBefore = _slider.gameObject.activeSelf;
                _savedAlphaValue = _canvasGroup.alpha;
            }
        }

        [SerializeField] private SliderSetup[] _sliders;

        protected virtual void Start()
        {
            foreach (var sliderSetup in _sliders)
            {
                sliderSetup.Initialize();
            }
        }

        public virtual IEnumerator DoEnter(float time)
        {
            foreach (var sliderSetup in _sliders)
            {
                sliderSetup.SaveState();
                sliderSetup.Slider.gameObject.SetActive(true);
            }

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                foreach (var sliderSetup in _sliders)
                {
                    sliderSetup.CanvasGroup.alpha = Mathf.Lerp(sliderSetup.SavedAlphaValue, 1f, t);
                    sliderSetup.Slider.value = Mathf.Lerp(sliderSetup.SavedSliderValue, sliderSetup.TargetSliderValue, t);
                }
                t += Time.deltaTime * dt;
                yield return null;
            }

            foreach (var sliderSetup in _sliders)
            {
                sliderSetup.CanvasGroup.alpha = 1;
                sliderSetup.Slider.value = sliderSetup.TargetSliderValue;
            }
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
            var currentValues = _sliders
                .Select(slider => slider.Slider.value)
                .ToArray();
            while (t < 1.0f)
            {
                for (var i = 0; i < _sliders.Length; i++)
                {
                    var sliderSetup = _sliders[i];
                    sliderSetup.CanvasGroup.alpha = Mathf.Lerp(1f, sliderSetup.SavedAlphaValue, t);
                    sliderSetup.Slider.value = Mathf.Lerp(currentValues[i], sliderSetup.SavedSliderValue, t);
                }

                t += Time.deltaTime * dt;
                yield return null;
            }
            
            foreach (var sliderSetup in _sliders)
            {
                sliderSetup.CanvasGroup.alpha = sliderSetup.SavedAlphaValue;
                sliderSetup.Slider.value = sliderSetup.SavedSliderValue;
                sliderSetup.Slider.gameObject.SetActive(sliderSetup.IsActiveBefore);
            }
        }
        
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideSimplexValue2DTo3D : SlideManySliders
    {
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private GameObject _output3D;
        [SerializeField] private CanvasGroup _target2DCanvasGroup;
        [SerializeField] private Slider _gizmosSlider;
        [SerializeField] private CanvasGroup _gizmosSliderCanvasGroup;
        [SerializeField] private CanvasGroup _barycentricSliderCanvasGroup;
        [SerializeField] private TitleChanger _titleChanger;

        private float _gizmosValueCached;
        private float _bgSavedAlpha;

        protected override void Start()
        {
            base.Start();
            _output3D.SetActive(false);
            _bgSavedAlpha = _background.color.a;
        }

        public override IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Value 3D", time));
            
            Color bgColor;
            
            _gizmosValueCached = _gizmosSlider.value;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _barycentricSliderCanvasGroup.alpha = 1f - t;
                _gizmosSliderCanvasGroup.alpha = 1f - t;
                _target2DCanvasGroup.alpha = 1f - t;
                _gizmosSlider.value = Mathf.Lerp(_gizmosValueCached, 0f, t);

                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(_bgSavedAlpha, 0f, t);
                _background.color = bgColor;
                
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            bgColor = _background.color;
            bgColor.a = 0f;
            _background.color = bgColor;
            
            _barycentricSliderCanvasGroup.alpha = 0f;
            _gizmosSliderCanvasGroup.alpha = 0f;
            _target2DCanvasGroup.alpha = 0f;
            _gizmosSlider.value = 0f;
            
            _gizmosSlider.gameObject.SetActive(false);
            _barycentricSliderCanvasGroup.gameObject.SetActive(false);
            
            _output3D.SetActive(true);
            yield return base.DoEnter(time);
        }
        

        public override IEnumerator DoBack(float time)
        {
            yield return base.DoBack(time);
            
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Value 2D", time));
            
            Color bgColor;
            
            _barycentricSliderCanvasGroup.gameObject.SetActive(true);
            _gizmosSlider.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _barycentricSliderCanvasGroup.alpha = t;
                _gizmosSliderCanvasGroup.alpha = t;
                _target2DCanvasGroup.alpha = t;
                _gizmosSlider.value = Mathf.Lerp(0f, _gizmosValueCached, t);
                
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _background.color = bgColor;

                t += Time.deltaTime * dt;
                yield return null;
            }
            
            bgColor = _background.color;
            bgColor.a = _bgSavedAlpha;
            _background.color = bgColor;
            
            _barycentricSliderCanvasGroup.alpha = 1f;
            _gizmosSliderCanvasGroup.alpha = 1f;
            _target2DCanvasGroup.alpha = 1f;
            _gizmosSlider.value = _gizmosValueCached;
            
            _output3D.SetActive(false);
        }
        
    }
}
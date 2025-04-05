using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideValue2DTo3D : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _value2D;
        [SerializeField] private Value3DOutput _value3DOutput;
        [SerializeField] private SpriteRenderer _backgruond;
        [SerializeField] private TitleChanger _titleChanger;
        
        [SerializeField] private CanvasGroup _linksOld;
        [SerializeField] private CanvasGroup _linksNew;

        private float _bgSavedAlpha;

        private void Start()
        {
            _value3DOutput.gameObject.SetActive(false);
            _linksNew.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Value 3D", time));
            _bgSavedAlpha = _backgruond.color.a;
            Color bgColor;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value2D.alpha = 1f - t;
                bgColor = _backgruond.color;
                bgColor.a = Mathf.Lerp(_bgSavedAlpha, 0f, t);
                _backgruond.color = bgColor;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _value2D.alpha = 0f;
            bgColor = _backgruond.color;
            bgColor.a = 0f;
            _backgruond.color = bgColor;
            _value2D.gameObject.SetActive(false);
            
            _value3DOutput.gameObject.SetActive(true);
            _linksNew.gameObject.SetActive(true);
            t = 0f;
            while (t < 1.0f)
            {
                _value3DOutput.ApplyThreshold(1f - t);
                _linksOld.alpha = 1f - t;
                _linksNew.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 0f;
            _linksNew.alpha = 1f;
            
            _value3DOutput.ApplyThreshold(0f);
            _linksOld.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Value 2D", time));
            _value2D.gameObject.SetActive(true);
            _linksOld.gameObject.SetActive(true);
            
            
            var thresholdOld = _value3DOutput.Thresholds.w;
            Color bgColor;

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value2D.alpha = t;
                _value3DOutput.ApplyThreshold(Mathf.Lerp(thresholdOld, 1f, t));
                bgColor = _backgruond.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _backgruond.color = bgColor;
                
                _linksOld.alpha = t;
                _linksNew.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 1f;
            _linksNew.alpha = 0f;
            
            _value2D.alpha = 1f;
            bgColor = _backgruond.color;
            bgColor.a = _bgSavedAlpha;
            _backgruond.color = bgColor;
            _value3DOutput.ApplyThreshold(1f);
            _value3DOutput.gameObject.SetActive(false);
            _linksNew.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
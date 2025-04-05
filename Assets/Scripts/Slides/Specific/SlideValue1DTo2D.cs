using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideValue1DTo2D : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _value1D;
        [SerializeField] private CanvasGroup _value2D;
        [SerializeField] private Value2DOutput _value2DOutput;
        [SerializeField] private TitleChanger _titleChanger;
        
        private void Start()
        {
            _value2D.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Value 2D", time));
            _value2D.gameObject.SetActive(true);
            _value2DOutput.UpdateTargets();
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value1D.alpha = 1f - t;
                _value2D.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _value1D.alpha = 0f;
            _value2D.alpha = 1f;
            _value1D.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Value 1D", time));
            _value1D.gameObject.SetActive(true);
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value2D.alpha = 1f - t;
                _value1D.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _value2D.alpha = 0f;
            _value1D.alpha = 1f;
            _value2D.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
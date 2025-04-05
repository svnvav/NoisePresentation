using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideGraph1DTileTextures : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _outputFrame;

        private void Awake()
        {
            _canvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            _canvasGroup.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = t;
                var color = _outputFrame.color;
                color.a = 1f - t;
                _outputFrame.color = color;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 1f;
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
                _canvasGroup.alpha = 1f - t;
                
                var color = _outputFrame.color;
                color.a = t;
                _outputFrame.color = color;
                    
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            
            var col = _outputFrame.color;
            col.a = 1f;
            _outputFrame.color = col;
            
            _canvasGroup.gameObject.SetActive(false);
        }
        
        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }

    }
}
using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideCanvasGroupShow : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup.gameObject.SetActive(false);
        }

        public virtual IEnumerator DoEnter(float time)
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

        public virtual IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public virtual IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            
            _canvasGroup.gameObject.SetActive(false);
        }
        
        public virtual IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }

    }
}
using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideValue1DStart : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _links;
        [SerializeField] private TitleChanger _titleChanger;

        private void Start()
        {
            _canvasGroup.gameObject.SetActive(false);
            _links.gameObject.SetActive(false);
        }

        public virtual IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Value 1D", time));
            _canvasGroup.gameObject.SetActive(true);
            _links.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _links.alpha = t;
                _canvasGroup.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _links.alpha = 1f;
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
                _links.alpha = 1f - t;
                _canvasGroup.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _links.alpha = 0f;
            _canvasGroup.alpha = 0f;
            
            _canvasGroup.gameObject.SetActive(false);
            _links.gameObject.SetActive(false);
        }
        
        public virtual IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }

    }
}
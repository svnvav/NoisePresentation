using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideContents : MonoBehaviour, ISlide
    {
        [SerializeField] private TitleChanger _titleChanger;

        [SerializeField] private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup.gameObject.SetActive(false);
            _titleChanger.SetAlpha(0);
        }

        public IEnumerator DoEnter(float time)
        {
            _canvasGroup.gameObject.SetActive(true);
            StartCoroutine(_titleChanger.ChangeTitle("Contents", time));
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _titleChanger.SetAlpha(t);
                _canvasGroup.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _titleChanger.SetAlpha(1f);
            _canvasGroup.alpha = 1f;
        }

        public IEnumerator DoBack(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("", time));
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _titleChanger.SetAlpha(1f - t);
                _canvasGroup.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _titleChanger.SetAlpha(0f);

            _canvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
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


        public IEnumerator DoEnterFromBack(float time)
        {
            yield return DoEnter(time);
        }

    }
}
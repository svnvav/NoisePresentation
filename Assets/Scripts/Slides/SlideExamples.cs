using System.Collections;
using TMPro;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideExamples : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _self;
        [SerializeField] private TitleChanger _titleChanger;

        private void Start()
        {
            _self.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Examples", time));
            _self.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                _self.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _self.alpha = 1f;
        }

        public IEnumerator DoExit(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("", time));

            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                _self.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _self.alpha = 0f;

            _self.gameObject.SetActive(false);
        }

        public IEnumerator DoBack(float time)
        {
            yield return DoExit(time);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return DoEnter(time);
        }
    }
}
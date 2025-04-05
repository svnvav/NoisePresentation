using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideNoise3DSlice : MonoBehaviour, ISlide
    {
        [SerializeField] private Gradient2DOutput _noise2DOutput;
        [SerializeField] private CanvasGroup _canvasGroup;

        protected void Start()
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

                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }
        

        public IEnumerator DoBack(float time)
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
            _noise2DOutput.ApplyZOffset(-1f);

            _canvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class Slide3DToGradient2D : MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _value3D;
        [SerializeField] private CanvasGroup _slice2D;
        [SerializeField] private CanvasGroup _gradient;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Value3DOutput _value3DOutput;
        [SerializeField] private Gradient2DGizmos _gizmos;
        [SerializeField] private Gradient2DOutput _noise2DOutput;
        [SerializeField] private TitleChanger _titleChanger;
        [SerializeField] private CanvasGroup _linksOld;
        [SerializeField] private CanvasGroup _linksNew;


        private float _bgSavedAlpha;
        private float _savedThreshold;

        private void Start()
        {
            _linksNew.gameObject.SetActive(false);
            _gradient.gameObject.SetActive(false);
            _bgSavedAlpha = _background.color.a;
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Gradient 2D", time));
            _gradient.gameObject.SetActive(true);
            
            Color bgColor;
            _noise2DOutput.UpdateTargets();

            _savedThreshold = _value3DOutput.Thresholds.w;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _value3D.alpha = 1f - t;
                _slice2D.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _background.color = bgColor;
                _value3DOutput.ApplyThreshold(Mathf.Lerp(_savedThreshold, 1f, t));
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _value3D.alpha = 0f;
            _slice2D.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = _bgSavedAlpha;
            _background.color = bgColor;
            _value3DOutput.ApplyThreshold(1f);

            _value3D.gameObject.SetActive(false);
            _slice2D.gameObject.SetActive(false);
            _value3DOutput.gameObject.SetActive(false);
            
            _linksNew.gameObject.SetActive(true);
            _gradient.gameObject.SetActive(true);
            _gizmos.GenerateNewKeys();
            _gizmos.GenerateNewLines();
            _gizmos.SetRandom(1f);
            
            t = 0f;
            while (t < 1.0f)
            {
                _gradient.alpha = t;
                _linksOld.alpha = 1f - t;
                _linksNew.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 0f;
            _linksNew.alpha = 1f;
            
            _gradient.alpha = 1f;
            _linksOld.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            Color bgColor;
            
            StartCoroutine(_titleChanger.ChangeTitle("Value3D", time));
            
            _value3D.gameObject.SetActive(true);
            _slice2D.gameObject.SetActive(true);
            _value3DOutput.gameObject.SetActive(true);
            _linksOld.gameObject.SetActive(true);

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _linksOld.alpha = t;
                _linksNew.alpha = 1f - t;
                _value3D.alpha = t;
                _slice2D.alpha = t;
                _value3DOutput.ApplyThreshold(Mathf.Lerp(1f, _savedThreshold, t));
                _gradient.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(_bgSavedAlpha, 0f, t);
                _background.color = bgColor;
                t += Time.deltaTime * dt;
                yield return null;
                yield return null;
            }
            
            _linksOld.alpha = 1f;
            _linksNew.alpha = 0f;
            _value3D.alpha = 1f;
            _slice2D.alpha = 1f;
            _value3DOutput.ApplyThreshold(_savedThreshold);
            _gradient.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = 0f;
            _background.color = bgColor;
            _gizmos.FlushKeys();
            _gizmos.FlushLines();
            _gradient.gameObject.SetActive(false);
            _linksNew.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
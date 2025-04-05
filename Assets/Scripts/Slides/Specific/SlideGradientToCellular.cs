using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideGradientToCellular: MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _gradientSlidersGroup;
        [SerializeField] private CanvasGroup _gradientSlice2DGroup;
        [SerializeField] private CanvasGroup _cellularGroup;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Gradient2DOutput _gradient2DOutput;
        [SerializeField] private Gradient3DOutput _gradient3DOutput;
        [SerializeField] private Cellular2DGizmos _gizmos;
        [SerializeField] private Cellular2DOutput _cellular2DOutput;
        [SerializeField] private TitleChanger _titleChanger;
        [SerializeField] private CanvasGroup _linksOld;
        [SerializeField] private CanvasGroup _linksNew;

        private float _bgSavedAlpha;
        private float _savedThreshold;

        private void Start()
        {
            _linksNew.gameObject.SetActive(false);
            _cellularGroup.gameObject.SetActive(false);
            _bgSavedAlpha = _background.color.a;
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Cellular 2D", time));
            _cellularGroup.gameObject.SetActive(true);
            
            Color bgColor;
            _cellular2DOutput.UpdateTargets();

            _savedThreshold = _gradient2DOutput.Thresholds.w;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _gradientSlidersGroup.alpha = 1f - t;
                _gradientSlice2DGroup.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _background.color = bgColor;
                _gradient2DOutput.ApplyAlphaThreshold(Mathf.Lerp(_savedThreshold, 1f, t));
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _gradientSlidersGroup.alpha = 0f;
            _gradientSlice2DGroup.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = _bgSavedAlpha;
            _background.color = bgColor;
            _gradient2DOutput.ApplyAlphaThreshold(0f);

            _gradientSlidersGroup.gameObject.SetActive(false);
            _gradientSlice2DGroup.gameObject.SetActive(false);
            _gradient3DOutput.gameObject.SetActive(false);
            
            _linksNew.gameObject.SetActive(true);
            _cellularGroup.gameObject.SetActive(true);
            _gizmos.GenerateNewKeys();
            _gizmos.GenerateNewLines();
            _gizmos.SetRandom(1f);
            
            t = 0f;
            while (t < 1.0f)
            {
                _cellularGroup.alpha = t;
                _linksOld.alpha = 1f - t;
                _linksNew.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 0f;
            _linksNew.alpha = 1f;
            
            _cellularGroup.alpha = 1f;
            _linksOld.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            Color bgColor;
            
            StartCoroutine(_titleChanger.ChangeTitle("Gradient 3D", time));
            
            _gradientSlidersGroup.gameObject.SetActive(true);
            _gradientSlice2DGroup.gameObject.SetActive(true);
            _gradient3DOutput.gameObject.SetActive(true);
            _linksOld.gameObject.SetActive(true);

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _linksOld.alpha = t;
                _linksNew.alpha = 1f - t;
                _gradientSlidersGroup.alpha = t;
                _gradientSlice2DGroup.alpha = t;
                _gradient2DOutput.ApplyAlphaThreshold(Mathf.Lerp(1f, _savedThreshold, t));
                _cellularGroup.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(_bgSavedAlpha, 0f, t);
                _background.color = bgColor;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 1f;
            _linksNew.alpha = 0f;
            _gradientSlidersGroup.alpha = 1f;
            _gradientSlice2DGroup.alpha = 1f;
            _gradient2DOutput.ApplyAlphaThreshold(_savedThreshold);
            _cellularGroup.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = 0f;
            _background.color = bgColor;
            _gizmos.FlushKeys();
            _gizmos.FlushLines();
            _cellularGroup.gameObject.SetActive(false);
            _linksNew.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
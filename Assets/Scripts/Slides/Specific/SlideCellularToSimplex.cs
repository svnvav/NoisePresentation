using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideCellularToSimplex: MonoBehaviour, ISlide
    {
        [SerializeField] private CanvasGroup _cellularSlidersGroup;
        [SerializeField] private CanvasGroup _cellularSlice2DGroup;
        [SerializeField] private CanvasGroup _simplexGroup;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Cellular3DOutput _cellular3DOutput;
        [SerializeField] private SimplexValue2DGizmos _gizmos;
        [SerializeField] private SimplexValue2DOutput _simplex2DOutput;
        [SerializeField] private TitleChanger _titleChanger;
        [SerializeField] private CanvasGroup _linksOld;
        [SerializeField] private CanvasGroup _linksNew;

        private float _bgSavedAlpha;
        private float _savedThreshold;

        private void Start()
        {
            _linksNew.gameObject.SetActive(false);
            _simplexGroup.gameObject.SetActive(false);
            _bgSavedAlpha = _background.color.a;
        }

        public IEnumerator DoEnter(float time)
        {
            StartCoroutine(_titleChanger.ChangeTitle("Simplex Value 2D", time));
            
            _simplexGroup.gameObject.SetActive(true);
            
            Color bgColor;
            _simplex2DOutput.UpdateTargets();

            _savedThreshold = _cellular3DOutput.Thresholds.w;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _cellularSlidersGroup.alpha = 1f - t;
                _cellularSlice2DGroup.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _background.color = bgColor;
                _cellular3DOutput.ApplyAlphaThreshold(Mathf.Lerp(_savedThreshold, 1f, t));
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _cellularSlidersGroup.alpha = 0f;
            _cellularSlice2DGroup.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = _bgSavedAlpha;
            _background.color = bgColor;
            _cellular3DOutput.ApplyAlphaThreshold(0f);

            _cellularSlidersGroup.gameObject.SetActive(false);
            _cellularSlice2DGroup.gameObject.SetActive(false);
            _cellular3DOutput.gameObject.SetActive(false);
            
            _linksNew.gameObject.SetActive(true);
            _simplexGroup.gameObject.SetActive(true);
            _gizmos.GenerateNewKeys();
            _gizmos.GenerateNewLines();
            _gizmos.Resolve();
            _simplex2DOutput.UpdateTargets();
            
            t = 0f;
            while (t < 1.0f)
            {
                _linksOld.alpha = 1f - t;
                _linksNew.alpha = t;
                _simplexGroup.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _linksOld.alpha = 0f;
            _linksNew.alpha = 1f;
            
            _simplexGroup.alpha = 1f;
            _linksOld.gameObject.SetActive(false);
        }

        public IEnumerator DoExit(float time)
        {
            yield return null;
        }

        public IEnumerator DoBack(float time)
        {
            Color bgColor;
            
            StartCoroutine(_titleChanger.ChangeTitle("Cellular 3D", time));
            
            _cellularSlidersGroup.gameObject.SetActive(true);
            _cellularSlice2DGroup.gameObject.SetActive(true);
            _cellular3DOutput.gameObject.SetActive(true);
            _linksOld.gameObject.SetActive(true);

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _linksOld.alpha = t;
                _linksNew.alpha = 1f - t;
                _cellularSlidersGroup.alpha = t;
                _cellularSlice2DGroup.alpha = t;
                _cellular3DOutput.ApplyAlphaThreshold(Mathf.Lerp(1f, _savedThreshold, t));
                _simplexGroup.alpha = 1f - t;
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(_bgSavedAlpha, 0f, t);
                _background.color = bgColor;
                t += Time.deltaTime * dt;
                yield return null;
            }
            _linksOld.alpha = 1f;
            _linksNew.alpha = 0f;
            _cellularSlidersGroup.alpha = 1f;
            _cellularSlice2DGroup.alpha = 1f;
            _cellular3DOutput.ApplyAlphaThreshold(_savedThreshold);
            _simplexGroup.alpha = 0f;
            bgColor = _background.color;
            bgColor.a = 0f;
            _background.color = bgColor;
            _gizmos.FlushKeys();
            _gizmos.FlushLines();
            _simplexGroup.gameObject.SetActive(false);
            _linksNew.gameObject.SetActive(false);
        }

        public IEnumerator DoEnterFromBack(float time)
        {
            yield return null;
        }
    }
}
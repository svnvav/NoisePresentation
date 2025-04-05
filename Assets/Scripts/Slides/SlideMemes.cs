
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideMemes : MonoBehaviour, ISlide
    {
        private static readonly int AlphaThreshold = Shader.PropertyToID("_AlphaThreshold");
        
        [SerializeField] private Image _image;
        
        [SerializeField] private CanvasGroup _memes;

        [SerializeField] private float _targetAlphaThreshold = 0.32f;
        
        private Material _material;

        private void Awake()
        {
            _material = _image.material;
            _image.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            _image.gameObject.SetActive(true);
            _memes.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                var threshold = Mathf.Lerp(1f, _targetAlphaThreshold, t);
                _material.SetFloat(AlphaThreshold, threshold);
                //_memes.alpha = t;
                
                t += Time.deltaTime * dt;
                yield return null;
            }

            _memes.alpha = 1f;
            _material.SetFloat(AlphaThreshold, _targetAlphaThreshold);
        }

        public IEnumerator DoExit(float time)
        {
            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                var threshold = Mathf.Lerp(_targetAlphaThreshold, 1f, t);
                _material.SetFloat(AlphaThreshold, threshold);
                _memes.alpha = 1f - t;
                t += Time.deltaTime * dt;
                yield return null;
            }

            _memes.alpha = 0f;
            _memes.gameObject.SetActive(false);
            _image.gameObject.SetActive(false);
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
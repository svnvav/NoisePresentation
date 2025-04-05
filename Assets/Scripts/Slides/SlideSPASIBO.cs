
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideSPASIBO : MonoBehaviour, ISlide
    {
        private static readonly int AlphaThreshold = Shader.PropertyToID("_AlphaThreshold");
        
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Image _image;
        [SerializeField] private float _targetAlphaThreshold = 0.5f;
        
        private Material _material;
        private float _bgSavedAlpha;

        private void Awake()
        {
            _material = _image.material;
            _image.gameObject.SetActive(false);
            _bgSavedAlpha = _background.color.a;
        }

        public IEnumerator DoEnter(float time)
        {
            _image.gameObject.SetActive(true);

            Color bgColor;
            
            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                var threshold = Mathf.Lerp(1f, _targetAlphaThreshold, t);
                _material.SetFloat(AlphaThreshold, threshold);

                bgColor = _background.color;
                bgColor.a = Mathf.Lerp(0f, _bgSavedAlpha, t);
                _background.color = bgColor;
                
                t += Time.deltaTime * dt;
                yield return null;
            }

            _material.SetFloat(AlphaThreshold, _targetAlphaThreshold);
            bgColor = _background.color;
            bgColor.a = _bgSavedAlpha;
            _background.color = bgColor;
        }

        public IEnumerator DoExit(float time)
        {
            Color bgColor;
            
            var t = 0f;
            var dt = 1 / time;
            while (t < 1f)
            {
                var threshold = Mathf.Lerp(_targetAlphaThreshold, 1f, t);
                _material.SetFloat(AlphaThreshold, threshold);
                
                bgColor = _background.color;
                bgColor.a = Mathf.Lerp( _bgSavedAlpha, 0f, t);
                _background.color = bgColor;
                
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _image.gameObject.SetActive(false);
            
            bgColor = _background.color;
            bgColor.a = 0f;
            _background.color = bgColor;
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
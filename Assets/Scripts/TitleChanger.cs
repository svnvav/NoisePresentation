using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class TitleChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text0;
        [SerializeField] private TMP_Text _text1;
        [SerializeField] private Image _panel;

        private bool _isState0 = true;

        private float _startAlpha;

        private void Start()
        {
            _startAlpha = _panel.color.a;
        }

        public IEnumerator ChangeTitle(string title, float time)
        {
            var outText = _isState0 ? _text0 : _text1;
            var inText = _isState0 ? _text1 : _text0;

            _isState0 = !_isState0;

            inText.text = title;
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                outText.alpha = 1f - t;
                inText.alpha = t;
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            outText.alpha = 0f;
            inText.alpha = 1f;
        }

        public void SetAlpha(float t)
        {
            var color = _panel.color;
            color.a = Mathf.Lerp(0, _startAlpha, t);
            _panel.color = color;
        }
    }
}
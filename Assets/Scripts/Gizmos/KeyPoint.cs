using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class KeyPoint : MonoBehaviour
    {
        [SerializeField] private Image _underlay;
        [SerializeField] private Image _valueIndicator;

        private Vector2 _position;

        private RectTransform _rectTransform;

        public Vector2 Position => _position;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetUnderlayColor(Color color)
        {
            _underlay.color = color;
        }

        public void SetPosition(Vector2 position, bool withColor)
        {
            _position = position;
            
            _rectTransform.anchorMin = _position;
            _rectTransform.anchorMax = _position;

            if (withColor)
            {
                var value = position.y;
                _valueIndicator.color = new Color(value, value, value);
            }
        }

        public void Set1DValue(float value)
        {
            var anchorMin = _rectTransform.anchorMin;
            anchorMin.y = value;
            _rectTransform.anchorMin = anchorMin;
            
            var anchorMax = _rectTransform.anchorMax;
            anchorMax.y = value;
            _rectTransform.anchorMax = anchorMax;

            _position.y = value;
            
            _valueIndicator.color = new Color(value, value, value);
        }

        public void SetAlpha(float alpha)
        {
            var color = _underlay.color;
            color.a = alpha;
            _underlay.color = color;

            color = _valueIndicator.color;
            color.a = alpha;
            _valueIndicator.color = color;
        }

        public void SetColor(float value)
        {
            _valueIndicator.color = new Color(value, value, value);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class GraphLinePoint : MonoBehaviour
    {
        private Image _image;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchorMin = position;
            _rectTransform.anchorMax = position;

            var value = position.y;
            _image.color = new Color(value, value, value);
        }
        
        public void SetValue(float value)
        {
            var anchorMin = _rectTransform.anchorMin;
            anchorMin.y = value;
            _rectTransform.anchorMin = anchorMin;
            
            var anchorMax = _rectTransform.anchorMax;
            anchorMax.y = value;
            _rectTransform.anchorMax = anchorMax;
            
            _image.color = new Color(value, value, value);
        }
    }
}
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class ValueLine : MonoBehaviour
    {
        
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetValue(float value)
        {
            var anchorMin = _rectTransform.anchorMin;
            anchorMin.y = value;
            _rectTransform.anchorMin = anchorMin;
            
            var anchorMax = _rectTransform.anchorMax;
            anchorMax.y = value;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
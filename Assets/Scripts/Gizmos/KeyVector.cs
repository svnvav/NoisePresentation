using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class KeyVector : MonoBehaviour
    {
        [SerializeField] private Vector2 _initialDirection = Vector2.right;
        [SerializeField] private Image _arrow;
        [SerializeField] private Image _tip;

        private Vector2 _position;
        private float _length;
        private Vector2 _direction;

        private RectTransform _rectTransform;

        public Vector2 Position => _position;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetUnderlayColor(Color color)
        {
            _arrow.color = color;
        }

        public void Initialize(float length)
        {
            _length = length;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
            
            _rectTransform.anchorMin = _position;
            var anchorMax = _position;
            anchorMax.x += _length;
            _rectTransform.anchorMax = anchorMax;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(_direction, _initialDirection));
        }
        
        public void SetAlpha(float alpha)
        {
            var color = _arrow.color;
            color.a = alpha;
            _arrow.color = color;

            color = _tip.color;
            color.a = alpha;
            _tip.color = color;
        }

        /*public void SetColor(Color color)
        {
            
            _tip.color = color;
        }*/
    }
}
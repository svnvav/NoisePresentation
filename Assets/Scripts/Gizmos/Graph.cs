using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Color _color;
        [SerializeField] private RectTransform _notchesRoot;
        [SerializeField] private GameObject _xNotchPrefab;
        [SerializeField] private RectTransform _keyPointsRoot;
        [SerializeField] private KeyPoint _keyPointPrefab;
        [SerializeField] private RectTransform _linePointsRoot;
        [SerializeField] private GraphLinePoint _linePointPrefab;

        private List<GameObject> _notches;
        private List<KeyPoint> _keyPoints;
        private List<GraphLinePoint> _linePoints;
        private bool _isOpen = true;

        public Color Color => _color;

        public List<KeyPoint> KeyPoints => _keyPoints;

        public List<GraphLinePoint> LinePoints => _linePoints;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            _notches = new List<GameObject>();
            _keyPoints = new List<KeyPoint>();
            _linePoints = new List<GraphLinePoint>();
        }

        public GameObject[] CreateNotches(int count)
        {
            var addedNotches = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                var notch = Instantiate(_xNotchPrefab, _notchesRoot);
                var image = notch.GetComponent<Image>();
                image.color = _color;
                _notches.Add(notch);
                addedNotches[i] = notch;
            }

            return addedNotches;
        }

        public void FlushNotches()
        {
            for (int i = 0; i < _notches.Count; i++)
            {
                Destroy(_notches[i]);
            }
            _notches.Clear();
        }

        public KeyPoint CreateClosingPoint()
        {
            var point = CreateKeyPoints(1)[0];
            _isOpen = false;
            return point;
        }

        public void RemoveClosingPoint(KeyPoint closingPoint)
        {
            _keyPoints.Remove(closingPoint);
            Destroy(closingPoint.gameObject);
            _isOpen = true;
        }
        
        public KeyPoint[] CreateKeyPoints(int count)
        {
            var addedPoints = new KeyPoint[count];
            for (int i = 0; i < count; i++)
            {
                var point = Instantiate(_keyPointPrefab, _keyPointsRoot);
                point.SetUnderlayColor(_color);
                _keyPoints.Add(point);
                addedPoints[i] = point;
            }

            return addedPoints;
        }
        
        public void FlushKeyPoints()
        {
            for (int i = 0; i < _keyPoints.Count; i++)
            {
                Destroy(_keyPoints[i].gameObject);
            }
            _keyPoints.Clear();
        }
        
        public GraphLinePoint[] CreateLinePoints(int count)
        {
            var addedPoints = new GraphLinePoint[count];
            for (int i = 0; i < count; i++)
            {
                var point = Instantiate(_linePointPrefab, _linePointsRoot);
                _linePoints.Add(point);
                addedPoints[i] = point;
            }

            return addedPoints;
        }
        
        public void FlushLinePoints()
        {
            for (int i = 0; i < _linePoints.Count; i++)
            {
                Destroy(_linePoints[i].gameObject);
            }
            _linePoints.Clear();
        }
    }
}
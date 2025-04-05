using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideGraph1DPoints : MonoBehaviour, ISlide
    {
        [SerializeField] private SlideGraph1DSolidColor _prevSlide;
        [SerializeField] private Graph _graph;
        [SerializeField] private int _keyPointsCount = 8;
        [SerializeField] private int _linePointsCount = 256;
        [SerializeField] private CanvasGroup _pointsCanvasGroup;
        [SerializeField] private CanvasGroup _outputLinesCanvasGroup;
        [SerializeField] private GameObject _outputLinePrefab;

        private Image[] _notchImages;
        private KeyPoint[] _graphKeyPoints;
        private GraphLinePoint[] _graphLinePoints;
        private GameObject[] _outputLines;

        private void Awake()
        {
            _outputLinesCanvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoEnter(float time)
        {
            _outputLinesCanvasGroup.gameObject.SetActive(true);

            _outputLines = new GameObject[_keyPointsCount];
            for (int i = 0; i < _keyPointsCount; i++)
            {
                _outputLines[i] = Instantiate(_outputLinePrefab, _outputLinesCanvasGroup.transform);
                var image = _outputLines[i].GetComponent<Image>();
                image.color = _graph.Color;
            }
            
            _notchImages = _graph.CreateNotches(_keyPointsCount)
                .Select(notch => notch.GetComponent<Image>())
                .ToArray();

            _graphKeyPoints = _graph.CreateKeyPoints(_keyPointsCount);
            for (int i = 0; i < _graphKeyPoints.Length; i++)
            {
                _graphKeyPoints[i].SetPosition(new Vector2((float)i / _keyPointsCount, _prevSlide.SliderValue), true);
            }
            
            _graphLinePoints = _graph.CreateLinePoints(_linePointsCount);
            for (int i = 0; i < _graphLinePoints.Length; i++)
            {
                _graphLinePoints[i].SetPosition(new Vector2((float)i / _linePointsCount, _prevSlide.SliderValue));
            }

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                foreach (var notchImage in _notchImages)
                {
                    var color = notchImage.color;
                    color.a = t;
                    notchImage.color = color;
                }

                _pointsCanvasGroup.alpha = t;
                _outputLinesCanvasGroup.alpha = t;

                t += Time.deltaTime * dt;
                yield return null;
            }

            foreach (var notchImage in _notchImages)
            {
                var color = notchImage.color;
                color.a = 1;
                notchImage.color = color;
            }

            _pointsCanvasGroup.alpha = 1f;
            _outputLinesCanvasGroup.alpha = 1f;
        }

        public IEnumerator DoExit(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _outputLinesCanvasGroup.alpha = 1f - t;
                
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _outputLinesCanvasGroup.alpha = 0f;
            
            _outputLinesCanvasGroup.gameObject.SetActive(false);
        }

        public IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                foreach (var notchImage in _notchImages)
                {
                    var color = notchImage.color;
                    color.a = 1f - t;
                    notchImage.color = color;
                }

                _pointsCanvasGroup.alpha = 1f - t;
                _outputLinesCanvasGroup.alpha = 1f - t;
                
                t += Time.deltaTime * dt;
                yield return null;
            }

            foreach (var notchImage in _notchImages)
            {
                var color = notchImage.color;
                color.a = 0f;
                notchImage.color = color;
            }

            _pointsCanvasGroup.alpha = 0f;
            _outputLinesCanvasGroup.alpha = 0f;

            _graph.FlushNotches();
            _graph.FlushKeyPoints();
            _graph.FlushLinePoints();
            for (int i = 0; i < _outputLines.Length; i++)
            {
                Destroy(_outputLines[i]);
            }
            _notchImages = null;
            
            _outputLinesCanvasGroup.gameObject.SetActive(false);
        }
        
        public IEnumerator DoEnterFromBack(float time)
        {
            _outputLinesCanvasGroup.gameObject.SetActive(true);
            
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _outputLinesCanvasGroup.alpha = t;
                
                t += Time.deltaTime * dt;
                yield return null;
            }
            
            _outputLinesCanvasGroup.alpha = 1f;
        }

    }
}
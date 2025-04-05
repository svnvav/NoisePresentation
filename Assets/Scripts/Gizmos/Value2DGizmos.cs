using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Value2DGizmos : MonoBehaviour
    {
        [SerializeField] private Value2DOutput _value2DOutput;
        
        [SerializeField] private KeyPoint _keyPointPrefab;
        [SerializeField] private RectTransform _keyPointsRoot;

        [SerializeField] private Image _verticalLinePrefab;
        [SerializeField] private RectTransform _verticalLinesRoot;
        
        [SerializeField] private Image _horizontalLinePrefab;
        [SerializeField] private RectTransform _horizontalLinesRoot;

        private CanvasGroup _canvasGroup;
        
        private KeyPoint[,] _keys;
        private Image[] _verticalLines;
        private Image[] _horizontalLines;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void GenerateNewKeyPoints()
        {
            FlushKeyPoints();
            
            var size = _value2DOutput.Size;
            _keys = new KeyPoint[size.x + 1, size.y + 1];

            var xh = 1f / size.x;
            var yh = 1f / size.y;

            for (int x = 0, xlen = size.x + 1; x < xlen; x++)
            {
                for (int y = 0, ylen = size.y + 1; y < ylen; y++)
                {
                    var point = Instantiate(_keyPointPrefab, _keyPointsRoot);
                    point.SetPosition(new Vector2(x * xh, y * yh), false);
                    _keys[x, y] = point;
                }
            }
        }

        public void FlushKeyPoints()
        {
            if(_keys == null) return;
            
            for (int x = 0; x < _keys.GetLength(0); x++)
            {
                for (int y = 0; y < _keys.GetLength(1); y++)
                {
                    Destroy(_keys[x, y].gameObject);
                }
            }
            
            _keys = null;
        }

        public void GenerateNewLines()
        {
            FlushLines();
            
            var size = _value2DOutput.Size;

            _verticalLines = new Image[size.x - 1];

            var xh = 1f / size.x;

            for (int i = 0; i < _verticalLines.Length; i++)
            {
                var line = Instantiate(_verticalLinePrefab, _verticalLinesRoot);
                
                var anchorMin = line.rectTransform.anchorMin;
                anchorMin.x = xh + xh * i;
                anchorMin.y = 0f;
                line.rectTransform.anchorMin = anchorMin;
                
                var anchorMax = line.rectTransform.anchorMax;
                anchorMax.x = anchorMin.x;
                anchorMax.y = 1;
                line.rectTransform.anchorMax = anchorMax;

                _verticalLines[i] = line;
            }
            
            _horizontalLines = new Image[size.y - 1];

            var yh = 1f / size.y;

            for (int i = 0; i < _horizontalLines.Length; i++)
            {
                var line = Instantiate(_horizontalLinePrefab, _horizontalLinesRoot);
                
                var anchorMin = line.rectTransform.anchorMin;
                anchorMin.x = 0f;
                anchorMin.y = yh + yh * i;
                line.rectTransform.anchorMin = anchorMin;
                
                var anchorMax = line.rectTransform.anchorMax;
                anchorMax.x = 1;
                anchorMax.y = anchorMin.y;
                line.rectTransform.anchorMax = anchorMax;
                
                _horizontalLines[i] = line;
            }
            
        }

        public void FlushLines()
        {
            if (_verticalLines != null)
            {
                for (int i = 0; i < _verticalLines.Length; i++)
                {
                    Destroy(_verticalLines[i].gameObject);
                }

                _verticalLines = null;
            }

            if (_horizontalLines != null)
            {
                for (int i = 0; i < _horizontalLines.Length; i++)
                {
                    Destroy(_horizontalLines[i].gameObject);
                }

                _horizontalLines = null;
            }
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public void UpdateColors(float factor)
        {
            var size = _value2DOutput.Size;
            for (int x = 0, xlen = size.x + 1; x < xlen; x++)
            {
                for (int y = 0, ylen = size.y + 1; y < ylen; y++)
                {
                    var xIndex = x % size.x;
                    var yIndex = y % size.y;
                    var hash = NoiseUtility.Hashes[NoiseUtility.Hashes[xIndex] + yIndex];
                    var value = Mathf.Lerp(0.5f, (float)hash / 255, factor);
                    _keys[x, y].SetColor(value);
                }
            }
        }
    }
}
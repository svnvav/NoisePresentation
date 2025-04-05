
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Plarium.Tools.NoisePresentation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Cellular2DGizmos : MonoBehaviour
    {
        [SerializeField] private Cellular2DOutput _noise2DOutput;
        
        [SerializeField] private KeyPoint _keyPrefab;
        [SerializeField] private RectTransform _keysRoot;

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

        public void GenerateNewKeys()
        {
            FlushKeys();
            
            var size = _noise2DOutput.NoiseScale;
            _keys = new KeyPoint[size, size];

            var xh = 1f / size;
            var yh = 1f / size;

            for (int x = 0, xlen = size; x < xlen; x++)
            {
                for (int y = 0, ylen = size; y < ylen; y++)
                {
                    var key = Instantiate(_keyPrefab, _keysRoot);
                    key.SetPosition(new Vector2(x * xh, y * yh), false);
                    _keys[x, y] = key;
                }
            }
        }

        public void FlushKeys()
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
            
            var size = _noise2DOutput.NoiseScale;

            _verticalLines = new Image[size - 1];

            var xh = 1f / size;

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
            
            _horizontalLines = new Image[size - 1];

            var yh = 1f / size;

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

        public void SetRandom(float factor)
        {
            var size = _noise2DOutput.NoiseScale;

            var scaler = new Vector2(1f / size, 1f / size);

            for (int x = 0, xlen = size; x < xlen; x++)
            {
                for (int y = 0, ylen = size; y < ylen; y++)
                {
                    var xIndex = x % size;
                    var yIndex = y % size;

                    var cellOrigin = new Vector2(x, y);
                    
                    var randomPoint = GetCellRandomPoint(xIndex, yIndex);
                    randomPoint = Vector2.Lerp(Vector2.one * 0.5f, randomPoint, factor);
                    
                    var pos = (cellOrigin + randomPoint) * scaler;
                    
                    _keys[x, y].SetPosition(pos, false);
                }
            }
        }
        
        private static Vector2 GetCellRandomPoint(int x, int y)
        {
            int i = x * 31 + y * 33;
            i = i % NoiseUtility.PointCount;

            return NoiseUtility.Points[i];
        }
    }
}
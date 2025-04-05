using System;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Plarium.Tools.NoisePresentation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimplexValue2DGizmos : MonoBehaviour
    {
        [SerializeField] private SimplexValue2DOutput _noise2DOutput;

        [SerializeField] private KeyPoint _keyPrefab;
        [SerializeField] private RectTransform _keysRoot;

        [SerializeField] private Image _verticalLinePrefab;
        [SerializeField] private RectTransform _verticalLinesRoot;
        
        [SerializeField] private Image _horizontalLinePrefab;
        [SerializeField] private RectTransform _horizontalLinesRoot;
        
        [SerializeField] private Image _diagonalLinePrefab;
        [SerializeField] private RectTransform _diagonalLinesRoot;

        private CanvasGroup _canvasGroup;

        private KeyPoint[,] _keys;
        private Image[] _verticalLines;
        private Image[] _horizontalLines;
        private Image[] _diagonalLines;

        private int count => Mathf.CeilToInt(_noise2DOutput.NoiseScale * 1.8f);
        private int diagonalCount => _noise2DOutput.NoiseScale * 2 - 1;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Resolve()
        {
            ResolveLinesTransform();
            ResolvePointsPosition();
            ResolvePointsColor();
        }
        
        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public void GenerateNewKeys()
        {
            FlushKeys();

            _keys = new KeyPoint[count, count];

            for (int x = 0, xlen = count; x < xlen; x++)
            {
                for (int y = 0, ylen = count; y < ylen; y++)
                {
                    var key = Instantiate(_keyPrefab, _keysRoot);
                    _keys[x, y] = key;
                }
            }
        }

        public void FlushKeys()
        {
            if (_keys == null) return;

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

            GenerateDiagonals();
            GenerateHorizontals();
            GenerateVerticals();
        }

        public void FlushLines()
        {
            FlushLinesInternal(ref _diagonalLines);
            FlushLinesInternal(ref _horizontalLines);
            FlushLinesInternal(ref _verticalLines);
        }
        
        private void FlushLinesInternal(ref Image[] linesArray)
        {
            if (linesArray != null)
            {
                for (int i = 0; i < linesArray.Length; i++)
                {
                    Destroy(linesArray[i].gameObject);
                }

                linesArray = null;
            }
        }

        private void GenerateHorizontals()
        {
            _horizontalLines = new Image[count];
            for (int i = 0; i < _horizontalLines.Length; i++)
            {
                _horizontalLines[i] = Instantiate(_horizontalLinePrefab, _horizontalLinesRoot);
            }
        }

        private void GenerateVerticals()
        {
            _verticalLines = new Image[count];
            for (int i = 0; i < _verticalLines.Length; i++)
            {
                _verticalLines[i] = Instantiate(_verticalLinePrefab, _verticalLinesRoot);
            }
        }
        
        private void GenerateDiagonals()
        {
            _diagonalLines = new Image[diagonalCount];
            for (int i = 0; i < diagonalCount; i++)
            {
                _diagonalLines[i] = Instantiate(_diagonalLinePrefab, _diagonalLinesRoot);
            }
        }

        private void ResolveLinesTransform()
        {
            var noiseScale = _noise2DOutput.NoiseScale;
            var skewFactor = _noise2DOutput.SkewFactor;

            var coef = -(float)(3.0 - Math.Sqrt(3.0)) / 6.0f;
            var h = 1f / noiseScale;

            for (int i = 0; i < _verticalLines.Length; i++)
            {
                var line = _verticalLines[i];

                var origin = new Vector2(h + h * i, 0f);
                var end = new Vector2(origin.x, 1f);

                var originSkewer = (origin.x + origin.y) * coef;
                origin += new Vector2(originSkewer, originSkewer) * skewFactor;
                
                var endSkewer = (end.x + end.y) * coef;
                end += new Vector2(endSkewer, endSkewer) * skewFactor;
                
                line.rectTransform.anchorMin = origin;

                var anchorMax = new Vector2(origin.x, 1.5f);
                line.rectTransform.anchorMax = anchorMax;

                var angle = Vector2.SignedAngle(Vector2.up, end - origin);
                line.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            }
            
            for (int i = 0; i < _horizontalLines.Length; i++)
            {
                var line = _horizontalLines[i];

                var origin = new Vector2(0f, h + h * i);
                var end = new Vector2(1f, origin.y);

                var originSkewer = (origin.x + origin.y) * coef;
                origin += new Vector2(originSkewer, originSkewer) * skewFactor;
                
                var endSkewer = (end.x + end.y) * coef;
                end += new Vector2(endSkewer, endSkewer) * skewFactor;
                
                line.rectTransform.anchorMin = origin;

                var anchorMax = new Vector2( 1.5f, origin.y);
                line.rectTransform.anchorMax = anchorMax;

                var angle = Vector2.SignedAngle(Vector2.right, end - origin);
                line.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            }

            
            for (int i = 0; i < _diagonalLines.Length; i++)
            {
                var point = new Vector2((i + 1) * h * 0.5f, 1 - (i + 1) * h * 0.5f);
                _diagonalLines[i].rectTransform.anchorMin = point;
                _diagonalLines[i].rectTransform.anchorMax = point;
            }
        }

        private void ResolvePointsPosition()
        {
            var noiseScale = _noise2DOutput.NoiseScale;
            var skewFactor = _noise2DOutput.SkewFactor;

            var coef = -(float)(3.0 - Math.Sqrt(3.0)) / 6.0f;
            var h = 1f / noiseScale;

            for (int x = 0, xlen = count; x < xlen; x++)
            {
                for (int y = 0, ylen = count; y < ylen; y++)
                {
                    var position = new Vector2(x * h, y * h);
                    var skew = (position.x + position.y) * coef;
                    position += new Vector2(skew, skew) * skewFactor;
                    _keys[x, y].SetPosition(position, false);
                }
            }
        }

        private void ResolvePointsColor()
        {
            var randomFactor = _noise2DOutput.RandomFactor;

            for (int x = 0, xlen = count; x < xlen; x++)
            {
                for (int y = 0, ylen = count; y < ylen; y++)
                {
                    var hash = NoiseUtility.Hashes[NoiseUtility.Hashes[x] + y];
                    var value = Mathf.Lerp(0.5f, (float)hash / 255, randomFactor);
                    _keys[x, y].SetColor(value);
                }
            }
        }
    }
}
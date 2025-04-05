using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class SliderAnimator : MonoBehaviour
    {
        private const string PLAY_TEXT = "Play";
        private const string PAUSE_TEXT = "Pause";
        
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _buttonText;

        [SerializeField] private float _speed = 1f;

        private bool _isPlaying;

        private void Start()
        {
            ResolveButtonText();
        }

        private void Update()
        {
            if(!_isPlaying) return;

            var value = _slider.value;
            value += _speed * Time.deltaTime;
            while (value > _slider.maxValue)
            {
                value -= _slider.maxValue;
            }

            _slider.value = value;
        }

        public void Switch()
        {
            _isPlaying = !_isPlaying;
            ResolveButtonText();
        }

        private void ResolveButtonText()
        {
            _buttonText.text = _isPlaying ? PAUSE_TEXT : PLAY_TEXT;
        }
    }
}
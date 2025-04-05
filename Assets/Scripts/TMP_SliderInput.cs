using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TMP_SliderInput : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.text = _slider.value.ToString();
        }

        public void OnValueChanged(string newValue)
        {
            if (float.TryParse(newValue, out var floatValue))
            {
                _slider.value = floatValue;
            }
        }

        public void SetFloatValue(float value)
        {
            _inputField.text = value.ToString();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class ValueIndicator : MonoBehaviour
    {
        [SerializeField] private Image _indicator;
        
        public void SetValue(float value)
        {
            _indicator.color = new Color(value, value, value);
        }
    }
}
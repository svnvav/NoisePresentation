using TMPro;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class TMP_TextOutput : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetValue(float value)
        {
            _text.text = value.ToString();
        }
    }
}
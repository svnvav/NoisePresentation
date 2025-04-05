using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class LinkButton : MonoBehaviour
    {
        [SerializeField] private string _link;

        public void OpenInBrowser()
        {
            Application.OpenURL(_link);
        }

        
    }
}
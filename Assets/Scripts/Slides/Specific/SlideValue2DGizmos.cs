using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideValue2DGizmos : SlideSlider
    {
        [SerializeField] private Value2DGizmos _value2DGizmos;

        public override IEnumerator DoEnter(float time)
        {
            _value2DGizmos.GenerateNewLines();
            _value2DGizmos.GenerateNewKeyPoints();
            _value2DGizmos.UpdateColors(0);

            yield return base.DoEnter(time);
        }
        

        public override IEnumerator DoBack(float time)
        {
            yield return base.DoBack(time);
            
            _value2DGizmos.FlushKeyPoints();
            _value2DGizmos.FlushLines();
        }
        
    }
}
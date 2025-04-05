using System.Collections;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class SlideCellular3DSlice : SlideManySliders
    {
        [SerializeField] private Cellular3DOutput _output3D;
        [SerializeField] private Cellular3DSliceOutput _sliceOutput;
        [SerializeField] private CanvasGroup _canvasGroup;

        protected override void Start()
        {
            base.Start();
            _canvasGroup.gameObject.SetActive(false);
        }

        public override IEnumerator DoEnter(float time)
        {
            yield return base.DoEnter(time);
            
            _canvasGroup.gameObject.SetActive(true);
            _sliceOutput.UpdateTargets();
            

            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = t;

                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }
        

        public override IEnumerator DoBack(float time)
        {
            var t = 0f;
            var dt = 1f / time;
            while (t < 1.0f)
            {
                _canvasGroup.alpha = 1f - t;

                t += Time.deltaTime * dt;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _output3D.ApplyZOffset(-1f);

            _canvasGroup.gameObject.SetActive(false);

            yield return base.DoBack(time);
        }
        
    }
}
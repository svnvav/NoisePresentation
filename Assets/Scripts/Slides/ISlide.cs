
using System.Collections;

public interface ISlide
{
    IEnumerator DoEnter(float time);
    
    IEnumerator DoExit(float time);

    IEnumerator DoBack(float time);
    
    IEnumerator DoEnterFromBack(float time);
}

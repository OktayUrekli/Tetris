using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    public float startAlpha = 1f;
    public float endAlpha = 0f;

    public float waitingDuration = 0f;
    public float fadeDuration = 1f;

    void Start()
    {
        GetComponent<CanvasGroup>().alpha=startAlpha;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(waitingDuration);
        GetComponent<CanvasGroup>().DOFade(endAlpha, fadeDuration);
    }

    
}

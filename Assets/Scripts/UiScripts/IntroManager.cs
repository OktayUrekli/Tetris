using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class IntroManager : MonoBehaviour
{
    [SerializeField] GameObject[] countdownTexts;
    [SerializeField] Transform numbersImageTransform;

    

    void Start()
    {
        AssingFirstValuesOfCountdonwTexts();
        StartCoroutine(AnimateCountdown());
    }


    void AssingFirstValuesOfCountdonwTexts()
    {
        for (int i = 0; i < countdownTexts.Length; i++)
        {
            countdownTexts[i].GetComponent<RectTransform>().localPosition=new Vector3(0,-250,0);
            countdownTexts[i].GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    IEnumerator AnimateCountdown()
    {
        yield return new WaitForSeconds(.3f);
        numbersImageTransform.GetComponent<RectTransform>().DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
        numbersImageTransform.GetComponent<CanvasGroup>().DOFade(1, 0.3f);

        yield return new WaitForSeconds(.1f);

        int counter = 0;

        while (counter < countdownTexts.Length) 
        {
            countdownTexts[counter].GetComponent<RectTransform>().DOLocalMoveY(0, 0.7f);
            countdownTexts[counter].GetComponent<CanvasGroup>().DOFade(1, 0.7f);
            countdownTexts[counter].GetComponent<RectTransform>().DOScale(2f, 0.3f).SetEase(Ease.InBounce).OnComplete(() =>
            { countdownTexts[counter].GetComponent<RectTransform>().DOScale(1.5f, 0.3f).SetEase(Ease.InBack); });
            yield return new WaitForSeconds(0.5f);

            countdownTexts[counter].GetComponent<RectTransform>().DOLocalMoveY(250, 0.7f);
            countdownTexts[counter].GetComponent<CanvasGroup>().DOFade(0, 0.7f);

            counter++;
        }

        numbersImageTransform.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => 
        {
            gameObject.SetActive(false);
            FindFirstObjectByType<GameManager>().StartGame();
        });
    }
    
}

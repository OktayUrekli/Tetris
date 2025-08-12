using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int score = 0;
    int line = 5;
    int level = 1;

    public int scoreMul = 13;

    UiManager uiManager;

    private void Awake()
    {
        uiManager=FindAnyObjectByType<UiManager>();
    }

    void Start()
    {
        uiManager.UpdateTexts(score,line,level);
    }

    public void ScoreCounter(int _line)
    {
        line -= _line;
        if (line <= 0) // next level
        {
            SoundManager.instance.PlaySfx(2); // level up sfx
            score += scoreMul * level*_line;
            level++;
            line = level * 3;
        }
        else 
        {
            score += scoreMul * level;
        }

        uiManager.UpdateTexts(score, line, level);
    }
}

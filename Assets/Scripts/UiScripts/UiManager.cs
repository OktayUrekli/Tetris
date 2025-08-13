using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class UiManager : MonoBehaviour
{
    bool isGamePaused = false;

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;

    GameManager gameManager;

    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI lineTxt;
    [SerializeField] TextMeshProUGUI scoreTxt;

    private void Awake()
    {
        gameManager=FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        pausePanel.SetActive(isGamePaused);
        GameOverPanelOnOff(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanelOnOff();
        }
    }

    public void PausePanelOnOff()
    {
        if (gameManager.isGameOver)
            return;


        if (pausePanel)
        {
            PlayButtonClickSfx();

            isGamePaused = !isGamePaused;
            pausePanel.SetActive(isGamePaused);

            Time.timeScale = (isGamePaused)? 0 : 1 ;
        }
    }

    public void GameOverPanelOnOff(bool onOffState)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(onOffState);
            Time.timeScale = (onOffState) ? 0 : 1;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1 ;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateTexts(int score, int line, int level)
    {
        if (!scoreTxt || !lineTxt || !levelTxt)
            return;

        scoreTxt.text = score.ToString();
        lineTxt.text = line.ToString();
        levelTxt.text = level.ToString();

    }


    void PlayButtonClickSfx()
    {
        if (SoundManager.instance)
        {
            SoundManager.instance.PlaySfx(0);
        }
    }
}

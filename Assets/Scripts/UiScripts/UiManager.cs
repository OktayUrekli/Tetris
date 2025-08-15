using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UiManager : MonoBehaviour
{
    bool isGamePaused = false;

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;

    GameManager gameManager;

    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI lineTxt;
    [SerializeField] TextMeshProUGUI scoreTxt;

    bool isMusicOn = true;
    bool isSoundOn = true;
    [SerializeField] IconController musicOnOfIcon;
    [SerializeField] IconController sfxOnOffIcon;

    private void Awake()
    {
        gameManager=FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        pausePanel.SetActive(isGamePaused);
        GameOverPanelOnOff(false);

        if (FindAnyObjectByType<SoundManager>())
        {
            FindAnyObjectByType<SoundManager>().SetMusicLevels();
            FindAnyObjectByType<SoundManager>().SetSfxLevels();
        }
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

    public void ReturnMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void UpdateTexts(int score, int line, int level)
    {
        if (!scoreTxt || !lineTxt || !levelTxt)
            return;

        scoreTxt.text = score.ToString();
        lineTxt.text = line.ToString();
        levelTxt.text = level.ToString();

    }

    public void MusicOnOffButton()
    {
        isMusicOn = !isMusicOn;
        UpdateMusicIcons(isMusicOn);

        if (FindFirstObjectByType<SoundManager>())
        {
            FindFirstObjectByType<SoundManager>().isMusicOn=isMusicOn;
            FindFirstObjectByType<SoundManager>().UpdateMusicState();
        }
    }

    public void SfxOnOffButton()
    {
        isSoundOn = !isSoundOn;
        UpdateSfxIcons(isSoundOn);
        if (FindFirstObjectByType<SoundManager>())
        {
            FindFirstObjectByType<SoundManager>().isSoundOn=isSoundOn;
        }
        
    }

    public void UpdateMusicIcons(bool musicOnOffState)
    {
        musicOnOfIcon.UpdateIconState(musicOnOffState);
    }

    public void UpdateSfxIcons(bool sfxOnOffState)
    {
        sfxOnOffIcon.UpdateIconState(sfxOnOffState);
    }

    void PlayButtonClickSfx()
    {
        if (SoundManager.instance)
        {
            SoundManager.instance.PlaySfx(0);
        }
    }
}

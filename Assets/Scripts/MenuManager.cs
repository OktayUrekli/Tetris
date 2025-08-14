using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform menuPanel, settingsPanel;

    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] Slider musicSlider, sfxSlider;

    void Start()
    {
        SetFirstPosOfPanels();
        SetSoundLevels();
    }

    private void SetFirstPosOfPanels()
    {
        if (menuPanel && settingsPanel) // panellerin ilk konumlarý atanýyor
        {
            menuPanel.gameObject.SetActive(true);
            settingsPanel.gameObject.SetActive(true);
            menuPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            settingsPanel.GetComponent<RectTransform>().localPosition = new Vector3(1200, 0, 0);
        }
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    void SetSoundLevels()
    {
        if (PlayerPrefs.HasKey("MusicLevel"))
        {
            musicSlider.value= PlayerPrefs.GetFloat("MusicLevel");
            //musicAudioSource.volume = PlayerPrefs.GetFloat("MusicLevel");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicLevel", 1);
            musicSlider.value = PlayerPrefs.GetFloat("MusicLevel");
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicLevel");
        }

        if (PlayerPrefs.HasKey("SfxLevel"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SfxLevel");
            //musicAudioSource.volume = PlayerPrefs.GetFloat("SfxLevel");
        }
        else
        {
            PlayerPrefs.SetFloat("SfxLevel", 1);
            sfxSlider.value = PlayerPrefs.GetFloat("SfxLevel");
            //musicAudioSource.volume = PlayerPrefs.GetFloat("SfxLevel");
        }
    }

    
    public void OpenSettingsPanel()
    {
        menuPanel.GetComponent<RectTransform>().DOLocalMoveX(-1200, 1f);
        settingsPanel.GetComponent<RectTransform>().DOLocalMoveX(0, 1f);
    }

    public void CloseSettingsPanel()
    {
        menuPanel.GetComponent<RectTransform>().DOLocalMoveX(0, 1f);
        settingsPanel.GetComponent<RectTransform>().DOLocalMoveX(1200, 1f);
    }

    public void UpdateMusicLevel()
    {
        PlayerPrefs.SetFloat("MusicLevel", musicSlider.value);
        FindAnyObjectByType<SoundManager>().SetMusicLevels();
    }

    public void UpdateSfxLevel()
    {
        PlayerPrefs.SetFloat("SfxLevel", sfxSlider.value);
        FindAnyObjectByType<SoundManager>().SetSfxLevels();
    }
}

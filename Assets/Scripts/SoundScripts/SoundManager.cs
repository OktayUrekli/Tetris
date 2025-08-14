using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioClip[] musics;
    bool isMusicOn=true;

    [SerializeField] AudioSource vocalsAudioSource;
    [SerializeField] AudioClip[] vocals;

    [SerializeField] AudioSource[] sfxAudioSources;
    bool isSoundOn=true;


    [SerializeField] IconController musicOnOfIcon;
    [SerializeField] IconController sfxOnOffIcon;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UpdateMusicState();
        SetSfxLevels();
        SetMusicLevels();
    }


    AudioClip RandomMusicClip(AudioClip[] musicClips)
    {
        return musicClips[Random.Range(0, musicClips.Length)];
    }

    AudioClip RandomVocalClip(AudioClip[] vocals)
    {
        return vocals[Random.Range(0, vocals.Length)];
    }

    public void MusicOnOffButton()
    {
        isMusicOn = !isMusicOn;
        UpdateMusicState();
        musicOnOfIcon.UpdateIconState(isMusicOn);
    }

    void UpdateMusicState()
    {
        if (musicAudioSource)
        {
            if (isMusicOn)
            {
                musicAudioSource.clip = RandomMusicClip(musics);
                musicAudioSource.Play();
            }
            else
            {
                musicAudioSource.Stop();
            }
        }
    }

    public void SoundOnOffButton()
    {
        isSoundOn = !isSoundOn;
        sfxOnOffIcon.UpdateIconState(isSoundOn);
    }

    public void PlaySfx(int sfxIndex)
    {
        if (isSoundOn)
        {
            sfxAudioSources[sfxIndex].Stop();
            sfxAudioSources[sfxIndex].Play();
        }
    }

    public void PlayVocalSound()
    {
        if (isSoundOn)
        {
            vocalsAudioSource.Stop();
            vocalsAudioSource.clip=RandomVocalClip(vocals);
            vocalsAudioSource.Play();
        }
    }

    public void SetMusicLevels()
    {
        if (PlayerPrefs.HasKey("MusicLevel"))
        {
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicLevel");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicLevel", 1);
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicLevel");
        }
    }

    public void SetSfxLevels()
    {

        if (PlayerPrefs.HasKey("SfxLevel"))
        {
            vocalsAudioSource.volume = PlayerPrefs.GetFloat("SfxLevel");

            foreach (AudioSource source in sfxAudioSources)
            {
                source.volume = PlayerPrefs.GetFloat("SfxLevel");
            }
        }
        else
        {
            PlayerPrefs.SetFloat("SfxLevel", 1);
            vocalsAudioSource.volume = PlayerPrefs.GetFloat("SfxLevel");

            foreach (AudioSource source in sfxAudioSources)
            {
                source.volume = PlayerPrefs.GetFloat("SfxLevel");
            }
        }
    }


}   

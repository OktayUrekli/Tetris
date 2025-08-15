using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioClip[] musics;
    public bool isMusicOn=true;

    [SerializeField] AudioSource vocalsAudioSource;
    [SerializeField] AudioClip[] vocals;

    [SerializeField] AudioSource[] sfxAudioSources;
    public bool isSoundOn=true;

 
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
        SetSfxLevels();
        SetMusicLevels();
        UpdateMusicState();
    }


    AudioClip RandomMusicClip(AudioClip[] musicClips)
    {
        return musicClips[Random.Range(0, musicClips.Length)];
    }

    AudioClip RandomVocalClip(AudioClip[] vocals)
    {
        return vocals[Random.Range(0, vocals.Length)];
    }


    public void UpdateMusicState()
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
            if (PlayerPrefs.GetFloat("MusicLevel")==0)
            {
                isMusicOn = false;
                if (FindAnyObjectByType<UiManager>())
                {
                    FindAnyObjectByType<UiManager>().UpdateMusicIcons(isMusicOn);
                }
            }
            else
            {
                isMusicOn = true;
            }

                
        }
        else
        {
            PlayerPrefs.SetFloat("MusicLevel", 1);
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicLevel");
            isMusicOn= true;
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

            if (PlayerPrefs.GetFloat("SfxLevel") == 0)
            {
                isSoundOn = false;
                if (FindAnyObjectByType<UiManager>())
                {
                    FindAnyObjectByType<UiManager>().UpdateSfxIcons(isSoundOn);
                }
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

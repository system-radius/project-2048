using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    [SerializeField]
    private TouchManager touchManager;

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip mergeClip;

    [SerializeField]
    private AudioClip moveClip;

    private Coroutine bgmCoroutine;

    [SerializeField]
    private List<AudioClip> defaultBgms;

    private float mainVolume;
    private float bgmVolume;
    private float sfxVolume;

    private void Awake()
    {
        PlayDefaultBGM();
    }

    private void OnEnable()
    {
        touchManager.OnCancel += TriggerCancel;
        mainVolume = PlayerPrefs.GetFloat("mainVol", 1f);
        bgmVolume = PlayerPrefs.GetFloat("bgmVol", 1f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVol", 1f);

        RefreshVolume();
    }

    private void OnDisable()
    {
        touchManager.OnCancel -= TriggerCancel;
    }

    public void PlayMerge()
    {
        PlaySFX(mergeClip);
    }

    public void PlayMove()
    {
        PlaySFX(moveClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(params AudioClip[] clips)
    {
        bgmSource.Stop();
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
        }
        bgmCoroutine = StartCoroutine(PlayAllBGM(clips));
    }

    private IEnumerator PlayAllBGM(AudioClip[] clips)
    {
        for (int i = 0; ; i++)
        {
            if (i >= clips.Length)
            {
                i = 0;
            }

            var clip = clips[i];
            bgmSource.clip = clip;
            bgmSource.Play();

            //SongDisplay.Instance.AddMessage(clip.name, clip.length);
            yield return new WaitForSeconds(clip.length);
        }
    }

    private void TriggerCancel()
    {
        PlayDefaultBGM();
    }

    private void PlayDefaultBGM()
    {
        defaultBgms.Shuffle();
        PlayBGM(defaultBgms.ToArray());
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        RefreshVolume();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        RefreshVolume();
    }

    public void SetMainVolume(float value)
    {
        mainVolume = value;
        RefreshVolume();
    }

    private void RefreshVolume()
    {
        bgmSource.volume = mainVolume * bgmVolume;
        sfxSource.volume = mainVolume * sfxVolume;
    }
}
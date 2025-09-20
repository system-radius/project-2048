using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    private AudioController controller;

    [SerializeField]
    private Scrollbar bgmScrollBar;

    [SerializeField]
    private Scrollbar sfxScrollBar;

    private void Awake()
    {
        controller = AudioController.Instance;

        bgmScrollBar.value = PlayerPrefs.GetFloat("bgmVol", 1f);
        sfxScrollBar.value = PlayerPrefs.GetFloat("sfxVol", 1f);
    }

    private void OnEnable()
    {
        bgmScrollBar.onValueChanged.AddListener(OnBGMChange);
        sfxScrollBar.onValueChanged.AddListener(OnSFXChange);
    }

    private void OnDisable()
    {
        bgmScrollBar.onValueChanged.RemoveListener(OnBGMChange);
        sfxScrollBar.onValueChanged.RemoveListener(OnSFXChange);
    }

    private void OnBGMChange(float value)
    {
        controller.SetBGMVolume(value);
        PlayerPrefs.SetFloat("bgmVol", value);
        PlayerPrefs.Save();
    }

    private void OnSFXChange(float value)
    {
        controller.SetSFXVolume(value);
        PlayerPrefs.SetFloat("sfxVol", value);
        PlayerPrefs.Save();
    }

}
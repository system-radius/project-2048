using System;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip sfx;

    public event Action OnButtonPress;

    public void TriggerButtonPress()
    {
        OnButtonPress?.Invoke();
        if (sfx != null)
        {
            AudioController.Instance.PlaySFX(sfx);
        }
    }
}
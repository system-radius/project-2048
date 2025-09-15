using System;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class ButtonTrigger : MonoBehaviour
{
    public event Action OnButtonPress;

    public void TriggerButtonPress()
    {
        OnButtonPress?.Invoke();
    }
}
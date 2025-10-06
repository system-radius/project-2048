using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class TouchManager : Singleton<TouchManager>, IRestartTrigger, ITouchContactTrigger
{

    private PlayerControls playerControls;

    public event Action OnRestart;
    public event Action OnPlay;
    public event Action OnUndo;
    public event Action OnCancel;
    public event Action OnAuto;
    public event Action<Vector3, float> OnStartTouch;
    public event Action<Vector3, float> OnEndTouch;

    private bool firstTouch = true;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        playerControls = new PlayerControls();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Touch.Restart.performed += TriggerRestart;
        playerControls.Touch.Play.performed += TriggerPlay;
        playerControls.Touch.Undo.performed += TriggerUndo;
        playerControls.Touch.Cancel.performed += TriggerCancel;
        playerControls.Touch.Auto.performed += TriggerAuto;
        playerControls.Touch.PrimaryTouchContact.started += TriggerTouchStart;
        playerControls.Touch.PrimaryTouchContact.canceled += TriggerTouchEnd;
    }

    private void OnDisable()
    {
        playerControls.Touch.Restart.performed -= TriggerRestart;
        playerControls.Touch.Play.performed -= TriggerPlay;
        playerControls.Touch.Undo.performed -= TriggerUndo;
        playerControls.Touch.Cancel.performed -= TriggerCancel;
        playerControls.Touch.Auto.performed -= TriggerAuto;
        playerControls.Touch.PrimaryTouchContact.started -= TriggerTouchStart;
        playerControls.Touch.PrimaryTouchContact.canceled -= TriggerTouchEnd;

        playerControls.Disable();
    }

    private void TriggerRestart(InputAction.CallbackContext context)
    {
        OnRestart?.Invoke();
    }

    private void TriggerPlay(InputAction.CallbackContext context)
    {
        OnPlay?.Invoke();
    }

    private void TriggerUndo(InputAction.CallbackContext context)
    {
        OnUndo?.Invoke();
    }

    private void TriggerCancel(InputAction.CallbackContext context)
    {
        OnCancel?.Invoke(); 
    }

    private void TriggerAuto(InputAction.CallbackContext context)
    {
        OnAuto?.Invoke();
    }

    private IEnumerator HandleFirstTouchNextFrame(float time)
    {
        yield return null; // wait 1 frame
        TriggerTouchEvent(time, OnStartTouch);
        firstTouch = false;
    }

    private void TriggerTouchStart(InputAction.CallbackContext context) {
        if (!firstTouch) { 
            TriggerTouchEvent((float) context.time, OnStartTouch);
            return;
        }

        // There exists a weird behavior where the very first touch
        // after the game has been started does not register properly,
        // allowing a frame skip fixes the issue.
        StartCoroutine(HandleFirstTouchNextFrame((float) context.time));
    }

    private void TriggerTouchEnd(InputAction.CallbackContext context)
    {
        TriggerTouchEvent((float)context.time, OnEndTouch);
    }

    private void TriggerTouchEvent(float time, Action<Vector3, float> action)
    {
        Vector3 position = Utils.Instance.ScreenToWorldPoint(playerControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>());
        action?.Invoke(position, time);
    }
}

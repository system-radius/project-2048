using System;
using UnityEngine;

public class SwipeDetection : MonoBehaviour, ISwipeTrigger
{
    [SerializeField]
    private float minimumDistance = 0.2f;

    [SerializeField]
    private float maximumTime = 1;


    private TouchManager touchManager;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private float startTime;
    private float endTime;

    public event Action<Vector2Int> OnSwipe;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnStartTouch += TriggerStartTouch;
        touchManager.OnEndTouch += TriggerEndTouch;
    }

    private void OnDisable()
    {
        touchManager.OnStartTouch -= TriggerStartTouch;
        touchManager.OnEndTouch -= TriggerEndTouch;
    }

    private void TriggerStartTouch(Vector3 position, float time)
    {
        //Debug.Log("Touch started!");
        startPosition = position;
        startTime = time;
    }

    private void TriggerEndTouch(Vector3 position, float time)
    {
        //Debug.Log("Touch ended!");
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        //Debug.Log("Start: " + startPosition + ", end: " + endPosition + ", distance: " + Vector3.Distance(startPosition, endPosition));
        if (Vector3.Distance(startPosition, endPosition) < minimumDistance || (endTime - startTime) > maximumTime) return;

        float deltaX = endPosition.x - startPosition.x;
        float deltaY = endPosition.y - startPosition.y;

        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
        {
            // Horizontal swipe
            /*
            if (deltaX > 0)
            {
                Debug.Log("Horzontal swipe - right!");
            } else
            {
                Debug.Log("Horzontal swipe - left!");
            }/**/
            direction.x = deltaX > 0 ? -1 : 1;
        } else
        {
            // Vertical swipe
            /*
            if (deltaY > 0)
            {
                Debug.Log("Vertical swipe - up!");
            }
            else
            {
                Debug.Log("Vertical swipe - down!");
            }/**/
            direction.y = deltaY > 0 ? -1 : 1;
        }

        OnSwipe?.Invoke(direction);
    }
}
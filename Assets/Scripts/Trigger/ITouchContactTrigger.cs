using System;
using UnityEngine;

public interface ITouchContactTrigger
{
    event Action<Vector3, float> OnStartTouch;
    event Action<Vector3, float> OnEndTouch;
}
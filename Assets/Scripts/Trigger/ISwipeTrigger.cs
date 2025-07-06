using System;
using UnityEngine;

public interface ISwipeTrigger
{
    event Action<Vector2Int> OnSwipe;
}
using System;
using UnityEngine;

public interface IBoardTrigger
{
    event Action<Vector2Int, Vector2Int, int, int> OnMergeTile;
    event Action<Vector2Int, Vector2Int> OnMoveTile;
    event Action<Vector2Int, int, int> OnAddTile;
    event Action<Vector2Int, int, int> OnUpdateTile;
    event Action<Vector2Int> OnRemoveTile;
}
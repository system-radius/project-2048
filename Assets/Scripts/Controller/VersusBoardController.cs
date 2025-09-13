using System;
using System.Collections;
using UnityEngine;

public class VersusBoardController : BoardController
{
    private int currentPlayerId = 0;
    private int nextPlayerId = 1;

    public event Action<int> OnChangePlayer;

    public event Action<int, int> OnPlayerScore;

    protected override void Awake()
    {
        base.Awake();
        saveKey = "versus_";
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override IEnumerator StartBoard()
    {
        yield return null;
        Restart();
    }

    protected override void Restart()
    {
        base.Restart();
        currentPlayerId = 0;
        nextPlayerId = 1;
        NextPlayer();
    }

    protected override void PrepareMovementData(Vector2Int direction)
    {
        int moveScore = ProcessMovement(direction, currentPlayerId, nextPlayerId);
        if (moveScore >= 0)
        {
            if (moveScore > 0)
            {
                OnPlayerScore?.Invoke(moveScore, currentPlayerId);
            }
            NextPlayer();
        }
    }

    private void NextPlayer()
    {
        currentPlayerId = nextPlayerId;
        nextPlayerId = nextPlayerId + 1 > config.players ? 1 : nextPlayerId + 1;
        OnChangePlayer?.Invoke(currentPlayerId);
    }

    private void PrevPlayer()
    {
        nextPlayerId = currentPlayerId;
        currentPlayerId = currentPlayerId - 1 == 0 ? config.players : currentPlayerId - 1;
        OnChangePlayer?.Invoke(currentPlayerId);
    }

    protected override void Undo()
    {
        int score = board.Undo();
        if (score >= 0)
        {
            PrevPlayer();
            if (score > 0)
            {
                OnPlayerScore?.Invoke(-score, currentPlayerId);
            }
        }
    }
}
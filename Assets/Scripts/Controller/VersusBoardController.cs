using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusBoardController : BoardController
{
    private int currentPlayerId = 0;
    private int nextPlayerId = 1;

    public event Action<int> OnChangePlayer;

    public event Action<int, int> OnPlayerScore;

    private Dictionary<int, Brain> brainMapping = new();

    private bool hasHuman = false;

    private Brain brain = null;

    protected override void Awake()
    {
        base.Awake();
        saveKey = "versus_";
    }

    protected override void OnEnable()
    {
        SetupPlayers();
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
        //brain = new Brain(nextPlayerId, config.players);
    }

    private void SetupPlayers() {
        brainMapping.Clear();
        int index = 1;
        foreach (var level in config.playerLevels)
        {
            switch(level)
            {
                case Level.Human:
                    brainMapping.Add(index, null);
                    hasHuman = true;
                    break;
                case Level.Basic:
                    brainMapping.Add(index, new RandomBrain(index, config.players));
                    break;
                case Level.Average:
                    brainMapping.Add(index, new Brain(index, config.players));
                    break;
                case Level.Advanced:
                    brainMapping.Add(index, new Brain(index, config.players, 2));
                    break;
            }
            index++;
        }
    }

    protected override void PrepareMovementData(Vector2Int direction)
    {
        if (!hasHuman)
        {
            ExecuteArtificialMove();
            return;
        }

        ExecuteMove(direction);
    }

    private void ExecuteMove(Vector2Int direction)
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

        brain = brainMapping[currentPlayerId];
        if (hasHuman)
        {
            ExecuteArtificialMove();
        }
    }

    private void ExecuteArtificialMove()
    {
        if (brain != null)
        {
            if (currentPlayerId == brain.GetId())
            {
                StartCoroutine(Act(brain));
            }
        }
    }

    private void PrevPlayer()
    {
        nextPlayerId = currentPlayerId;
        currentPlayerId = currentPlayerId - 1 == 0 ? config.players : currentPlayerId - 1;
        OnChangePlayer?.Invoke(currentPlayerId);
    }

    private IEnumerator Act(Brain brain)
    {
        yield return new WaitForSeconds(0.3f);
        HypotheticalState state = new HypotheticalState(config.size.x, config.size.y, brain.GetId(), board.GetState());
        Vector2Int direction = brain.Think(state);
        //Debug.Log("Performing AI movement: " + direction);
        ExecuteMove(direction);
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

    protected override void TriggerGameOver()
    {
        base.TriggerGameOver();
        brain = null;
    }
}
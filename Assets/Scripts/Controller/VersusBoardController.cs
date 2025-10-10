using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusBoardController : BoardController, IPlayerChange, IVersusScoreTrigger
{
    [SerializeField]
    private Button autoButton;

    [SerializeField]
    private VersusScoreController scoreController;

    private int currentPlayerId = 0;
    private int nextPlayerId = 1;

    public event Action<int> OnChangePlayer;

    public event Action<int, int> OnPlayerScore;

    private Dictionary<int, Brain> brainMapping = new();

    private bool hasHuman = false, auto = false;

    private Brain brain = null;

    private Coroutine actCoroutine;

    protected override void OnEnable()
    {
        saveKey = "versus_";
        SetupPlayers();
        base.OnEnable();
        touchManager.OnAuto += TriggerAuto;
        scoreController.Initialize((IVersusScoreTrigger)this);
        scoreController.Initialize((IGameOverTrigger)this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        touchManager.OnAuto -= TriggerAuto;
        scoreController.Terminate();
    }

    protected override IEnumerator StartBoard()
    {
        yield return null;
        Restart();
    }

    protected override void Restart()
    {
        base.Restart();
        if (actCoroutine != null)
        {
            StopCoroutine(actCoroutine);
            actCoroutine = null;
        }

        auto = false;
        currentPlayerId = 0;
        nextPlayerId = 1;
        NextPlayer();
        //brain = new Brain(nextPlayerId, config.players);
    }

    protected virtual void SetupPlayers() {
        brainMapping.Clear();
        hasHuman = auto = false;
        int index = 1;
        List<AudioClip> clips = new List<AudioClip>();
        foreach (var playerType in config.playerTypes)
        {
            switch(playerType.level)
            {
                case Level.Human:
                    brainMapping.Add(index, null);
                    clips.AddRange(playerType.audioClips);
                    hasHuman = true;
                    break;
                case Level.Basic:
                    brainMapping.Add(index, new RandomBrain(index, config.players));
                    clips.AddRange(playerType.audioClips);
                    break;
                case Level.Average:
                    brainMapping.Add(index, new Brain(index, config.players));
                    clips.AddRange(playerType.audioClips);
                    break;
                case Level.Advanced:
                    brainMapping.Add(index, new Brain(index, config.players, 2));
                    clips.AddRange(playerType.audioClips);
                    break;
            }
            index++;
        }

        autoButton.gameObject.SetActive(!hasHuman);
        clips.Shuffle();
        AudioController.Instance.PlayBGM(clips.ToArray());
    }

    private void TriggerAuto()
    {
        auto = !auto;
        if (auto)
        {
            PrepareMovementData(new Vector2Int(0, 0));
        }
    }

    protected override void PrepareMovementData(Vector2Int direction)
    {
        if (settingsActive) return;

        if (!hasHuman || brain != null)
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
        if (hasHuman || auto)
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
                actCoroutine = StartCoroutine(Act(brain));
            }
        }
    }

    private void PrevPlayer()
    {
        nextPlayerId = currentPlayerId;
        currentPlayerId = currentPlayerId - 1 == 0 ? config.players : currentPlayerId - 1;

        brain = brainMapping[currentPlayerId];
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
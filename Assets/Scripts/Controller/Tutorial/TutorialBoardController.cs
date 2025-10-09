using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoardController : BoardController, IPlayerChange, IVersusScoreTrigger
{
    [SerializeField]
    private VersusScoreController scoreController;

    [SerializeField]
    private List<TutorialStepController> steps = new();

    [SerializeField]
    private GameObject tutorialStepsContainer;

    private int stepIndex = -1;

    private TutorialStepController step;

    public event Action<int> OnChangePlayer;
    public event Action<int, int> OnPlayerScore;

    private bool swiped = false;

    private int currentPlayerId = 0, nextPlayerId = 1;

    private Brain brain;

    private void Awake()
    {
        steps = new(tutorialStepsContainer.GetComponentsInChildren<TutorialStepController>());
        Debug.Log("Steps: " + steps.Count);
    }

    protected override Board InitializeBoard()
    {
        return new TutorialBoard(sizeX, sizeY);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        touchManager.OnStartTouch += ResetTouch;
        touchManager.OnEndTouch += DoTouch;

        scoreController.Initialize((IVersusScoreTrigger)this);
        scoreController.Initialize((IGameOverTrigger)this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        touchManager.OnStartTouch -= ResetTouch;
        touchManager.OnEndTouch -= DoTouch;
        scoreController.Terminate();
    }

    protected override IEnumerator StartBoard()
    {
        yield return null;

        Restart();

        Advance(true);
    }

    protected override void Restart()
    {
        base.Restart();
        currentPlayerId = 0;
        nextPlayerId = 1;
        stepIndex = -1;
        brain = new RandomBrain(2, 2);
    }

    private void ResetTouch(Vector3 vector, float time)
    {
        swiped = false;
    }

    private void DoTouch(Vector3 vector, float time)
    {
        if (settingsActive || swiped) return;
        Advance(step.Complete(new CompletionData(StepRequirement.Tap)));
    }

    protected override void PrepareMovementData(Vector2Int direction)
    {
        if (settingsActive) return;

        int moveScore = 0;
        swiped = true;
        if (step != null)
        {
            bool status = step.Complete(new CompletionData(step.requirement, direction));
            if (!status) return;
            moveScore = ProcessMovement(direction, step.currentPlayer, step.nextPlayer);

            if (moveScore > 0)
            {
                OnPlayerScore?.Invoke(moveScore, step.currentPlayer);
            }

            // Fix the scoring mechanism next!
            // Then proceed to refining the tutorial.
            if (step.spawnTile)
            {
                Vector2Int location = step.location;
                board.SpawnTile(location.x, location.y, step.tileValue, step.playerOwner);
            }
            Advance(status);
            return;
        }

        moveScore = ProcessMovement(direction, currentPlayerId, nextPlayerId);
        if (moveScore >= 0)
        {
            if (moveScore > 0)
            {
                OnPlayerScore?.Invoke(moveScore, currentPlayerId);
            }
            board.AddTile(nextPlayerId);
            NextPlayer();
        }

    }
    private void NextPlayer()
    {
        currentPlayerId = nextPlayerId;
        nextPlayerId = nextPlayerId + 1 > config.players ? 1 : nextPlayerId + 1;
        OnChangePlayer?.Invoke(currentPlayerId);

        if (step == null && currentPlayerId == brain.GetId())
        {
            StartCoroutine(Act(brain));
        }
    }

    private IEnumerator Act(Brain brain)
    {
        yield return new WaitForSeconds(0.3f);
        HypotheticalState state = new HypotheticalState(config.size.x, config.size.y, brain.GetId(), board.GetState());
        Vector2Int direction = brain.Think(state);
        //Debug.Log("Performing AI movement: " + direction);
        PrepareMovementData(direction);
    }

    private IEnumerator DelayNextMove(Vector2Int direction)
    {
        yield return new WaitForSeconds(0.3f);
        PrepareMovementData(direction);
    }

    private void Advance(bool status)
    {
        if (!status) return;
        if (step != null)
        {
            step.Deactivate();
        }
        stepIndex++;
        if (stepIndex == steps.Count)
        {
            //PlayerPrefs.SetInt("tutorialDone", 1);
            //PlayerPrefs.Save();
            Debug.Log("Tutorial complete!");
            currentPlayerId = step.currentPlayer;
            nextPlayerId = step.nextPlayer;
            step = null;
            NextPlayer();
            return;
        }

        step = steps[stepIndex];
        step.Activate();

        if (step.requirement == StepRequirement.AI)
        {
            StartCoroutine(DelayNextMove(step.direction));
        }
    }
}
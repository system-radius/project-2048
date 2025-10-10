using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoardController : BoardController, IPlayerChange, IVersusScoreTrigger
{
    [SerializeField]
    private VersusScoreController scoreController;

    [SerializeField]
    private GameObject tutorialStepsContainer;

    [SerializeField]
    private GameObject cancelButton;

    [SerializeField]
    private ButtonTrigger skipButton;

    [SerializeField]
    private GameObject undoButton;

    [SerializeField]
    private TutorialCheck checker;

    private List<TutorialStepController> steps;

    private int stepIndex = -1;

    private TutorialStepController step;

    public event Action<int> OnChangePlayer;
    public event Action<int, int> OnPlayerScore;

    private bool cancelTap = false;
    private bool complete = false;

    private int currentPlayerId = 0, nextPlayerId = 1;

    private Brain brain;

    private void Awake()
    {
        steps = new(tutorialStepsContainer.GetComponentsInChildren<TutorialStepController>());
    }

    protected override Board InitializeBoard()
    {
        return new TutorialBoard(sizeX, sizeY);
    }

    protected override void OnEnable()
    {
        saveKey = "tutorial_";
        base.OnEnable();
        touchManager.OnStartTouch += ResetTouch;
        touchManager.OnEndTouch += DoTouch;

        scoreController.Initialize((IVersusScoreTrigger)this);
        scoreController.Initialize((IGameOverTrigger)this);

        skipButton.OnButtonPress += SkipTutorial;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        touchManager.OnStartTouch -= ResetTouch;
        touchManager.OnEndTouch -= DoTouch;
        scoreController.Terminate();
        skipButton.OnButtonPress -= SkipTutorial;
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
        board.PrepareState();

        cancelButton.SetActive(false);
        skipButton.gameObject.SetActive(true);
        currentPlayerId = 0;
        nextPlayerId = 1;

        stepIndex = -1;
        brain = new RandomBrain(2, 2);
        NextPlayer();

        complete = false;
    }

    private void ResetTouch(Vector3 vector, float time)
    {
        cancelTap = false;
    }

    private void DoTouch(Vector3 vector, float time)
    {
        if (settingsActive || cancelTap || step == null) return;
        Advance(step.Complete(new CompletionData(StepRequirement.Tap)));
    }

    protected override void PrepareMovementData(Vector2Int direction)
    {
        if (settingsActive || (step != null && step.requirement != StepRequirement.Swipe)) return;

        ExecuteMove(direction);
    }

    private void ExecuteMove(Vector2Int direction)
    {
        int moveScore = 0;
        cancelTap = true;
        if (step != null)
        {
            bool status = step.Complete(new CompletionData(step.requirement, direction));
            if (!status) return;
            moveScore = ProcessMovement(direction, step.currentPlayer, step.nextPlayer);

            if (moveScore > 0)
            {
                OnPlayerScore?.Invoke(moveScore, step.currentPlayer);
            }

            OnChangePlayer?.Invoke(step.nextPlayer);

            // Fix the scoring mechanism next!
            // Then proceed to refining the tutorial.
            if (step.spawnTile)
            {
                Vector2Int location = step.location;
                board.SpawnTile(location.x, location.y, step.tileValue, step.playerOwner);
                board.PrepareState();
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
            board.AddTile(nextPlayerId, true);
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

    private IEnumerator DelayNextMove(Vector2Int direction)
    {
        yield return new WaitForSeconds(0.3f);
        ExecuteMove(direction);
    }

    private void SkipTutorial()
    {
        step.Deactivate();
        for (int i = stepIndex; i < steps.Count; i++)
        {
            var currentStep = steps[i];
            if (currentStep.requirement == StepRequirement.AI || currentStep.requirement == StepRequirement.Swipe)
            {
                int score = ProcessMovement(currentStep.direction, currentStep.currentPlayer, currentStep.nextPlayer);
                if (score > 0)
                {
                    OnPlayerScore?.Invoke(score, currentStep.currentPlayer);
                }
                if (currentStep.spawnTile)
                {
                    Vector2Int location = currentStep.location;
                    board.SpawnTile(location.x, location.y, currentStep.tileValue, currentStep.playerOwner);
                }
            }
        }

        stepIndex = steps.Count - 1;
        step = steps[stepIndex];
        undoButton.SetActive(false);
        Advance(true);
    }

    private void Advance(bool status)
    {
        if (!status) return;
        if (step != null)
        {
            step.Deactivate();
        }
        stepIndex++;
        if (stepIndex >= steps.Count)
        {
            PlayerPrefs.SetInt("tutorialDone", 1);
            PlayerPrefs.Save();
            //Debug.Log("Tutorial complete!");
            currentPlayerId = step.currentPlayer;
            nextPlayerId = step.nextPlayer;
            step = null;
            NextPlayer();
            checker.ReloadButton();
            complete = true;
            cancelButton.SetActive(true);
            skipButton.gameObject.SetActive(false);
            undoButton.SetActive(false);
            return;
        }

        step = steps[stepIndex];
        step.Activate();
        StartCoroutine(DelayDisableUndo());

        if (step.requirement == StepRequirement.AI)
        {
            StartCoroutine(DelayNextMove(step.direction));
        }
    }

    protected override void Undo()
    {
        cancelTap = true;
        if (complete)
        {
            int score = board.Undo();
            PrevPlayer();
            if (score > 0)
            {
                OnPlayerScore?.Invoke(-score, currentPlayerId);
            }
            return;
        }

        if (step != null)
        {
            step.Deactivate();
        }
        DecreaseStepIndex();
        step = steps[stepIndex];

        if (step == null || (step != null && (step.requirement == StepRequirement.AI || step.requirement == StepRequirement.Swipe)))
        {
            int score = board.Undo();
            OnChangePlayer?.Invoke(step.currentPlayer);
            if (score > 0)
            {
                OnPlayerScore?.Invoke(-score, step.currentPlayer);
            }

            if (step.requirement == StepRequirement.AI)
            {
                DecreaseStepIndex();
                step = steps[stepIndex];
            }
        }

        step.Activate();
    }

    private void DecreaseStepIndex()
    {
        stepIndex--;
        if (stepIndex <= 0)
        {
            stepIndex = 0;
        }

        StartCoroutine(DelayDisableUndo());
    }

    private IEnumerator DelayDisableUndo()
    {
        yield return null;
        undoButton.SetActive(stepIndex > 0 && !complete);
    }
}
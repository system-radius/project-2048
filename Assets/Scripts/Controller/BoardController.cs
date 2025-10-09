using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour, IScoreIncrement, IScoreUpdate, IGameOverTrigger
{
    [SerializeField]
    protected SwipeDetection swipeDetection;

    [SerializeField]
    protected Configuration config;

    [SerializeField]
    protected ButtonTrigger settingsTrigger;

    [SerializeField]
    protected ButtonTrigger settingsCancel;

    [SerializeField]
    protected TileViewController tileViewController;

    [SerializeField]
    protected TouchManager touchManager;

    protected Board board;

    protected AxisData mainAxis;
    protected AxisData staticAxis;

    protected int sizeX;
    protected int sizeY;

    public event Action<int> OnIncrementScore;
    public event Action<int> OnUpdateScore;
    public event Action OnGameOver;
    public event Action OnWin;

    protected bool winState = false;
    protected bool gameOver = false;

    protected bool settingsActive = false;

    protected string saveKey = "normal_";

    protected virtual Board InitializeBoard()
    {
        return new Board(sizeX, sizeY, config.players);
    }

    protected virtual void OnEnable()
    {
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;

        sizeX = config.size.x;
        sizeY = config.size.y;

        board = InitializeBoard();
        tileViewController.Initialize(board);
        touchManager.OnUndo += Undo;
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += SaveState;
        swipeDetection.OnSwipe += PrepareMovementData;

        board.OnMerge += TriggerMerge;
        board.OnMove += TriggerMove;

        board.OnGameOver += TriggerGameOver;

        settingsTrigger.OnButtonPress += TriggerSettings;
        settingsCancel.OnButtonPress += CancelSettings;

        StartCoroutine(StartBoard());
    }

    protected virtual void OnDisable()
    {
        tileViewController.Terminate();
        touchManager.OnUndo -= Undo;
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;
        swipeDetection.OnSwipe -= PrepareMovementData;

        board.OnMerge -= TriggerMerge;
        board.OnMove -= TriggerMove;

        board.OnGameOver -= TriggerGameOver;

        settingsTrigger.OnButtonPress -= TriggerSettings;
        settingsCancel.OnButtonPress -= CancelSettings;
    }

    protected void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
        }
    }

    protected virtual IEnumerator StartBoard()
    {
        List<AudioClip> audioClips = new(config.bgm);
        audioClips.Shuffle();
        AudioController.Instance.PlayBGM(audioClips.ToArray());
        yield return null;
        LoadState();
    }

    protected virtual void PrepareMovementData(Vector2Int direction)
    {
        if (settingsActive) return;
        ProcessMovement(direction, 0, 0);
    }

    protected int ProcessMovement(Vector2Int direction, int playerId, int nextPlayerId)
    {

        // Do the actual movement for the tiles according to the direction.
        int moveScore = -1;
        if (direction.x != 0)
        {
            moveScore = MoveAlongX(direction.x, playerId, nextPlayerId);
        }
        else if (direction.y != 0)
        {
            moveScore = MoveAlongY(direction.y, playerId, nextPlayerId);
        }

        if (moveScore > 0)
        {
            OnIncrementScore?.Invoke(moveScore);
            if (moveScore >= config.winCondition && !winState)
            {
                OnWin?.Invoke();
                winState = true;
            }
        }

        return moveScore;
    }

    private int MoveAlongX(int directionality, int playerId, int nextPlayerId)
    {
        int axisStart = 0;
        int axisEnd = sizeX;
        if (directionality < 0)
        {
            axisStart = sizeX - 1;
            axisEnd = -1;
        }

        return Move(axisStart, axisEnd, directionality, sizeY, true, playerId, nextPlayerId);
    }

    private int MoveAlongY(int directionality, int playerId, int nextPlayerId)
    {
        int axisStart = 0;
        int axisEnd = sizeY;
        if (directionality < 0)
        {
            axisStart = sizeY - 1;
            axisEnd = -1;
        }

        return Move(axisStart, axisEnd, directionality, sizeX, false, playerId, nextPlayerId);
    }

    private int Move(int mainAxisStart, int mainAxisEnd, int mainAxisUpdate, int staticAxisEnd, bool horizontal, int playerId, int nextPlayerId)
    {
        mainAxis.start = mainAxisStart;
        mainAxis.end = mainAxisEnd;
        mainAxis.update = mainAxisUpdate;
        staticAxis.end = staticAxisEnd;

        return board.DetectTileMovement(mainAxis, staticAxis, horizontal, playerId, nextPlayerId);
    }

    protected virtual void Restart()
    {
        OnUpdateScore?.Invoke(0);
        board.Restart();
        winState = false;
    }

    protected virtual void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        gameOver = true;
        SaveState();
    }

    protected void TriggerMerge()
    {
        AudioController.Instance.PlayMerge();
    }

    protected void TriggerMove()
    {
        AudioController.Instance.PlayMove();
    }

    protected void TriggerSettings()
    {
        settingsActive = true;
    }

    protected void CancelSettings()
    {
        settingsActive = false;
    }

    protected void SaveState()
    {
        if (gameOver)
        {
            // Force invalid size to trigger restart on the next load state.
            PlayerPrefs.SetInt("sizeX", -1);
            PlayerPrefs.SetInt("sizeY", -1);
            winState = false;
        } else
        {
            board.SaveState(saveKey);
        }
        
        PlayerPrefs.SetInt("winState", winState ? 1 : 0);
        PlayerPrefs.Save();
    }

    protected void LoadState()
    {
        if (!board.LoadState(saveKey))
        {
            Debug.Log("Restarting!");
            Restart();
            return;
        }

        winState = PlayerPrefs.GetInt("winState", 0) == 1;
    }

    protected virtual void Undo()
    {
        int score = board.Undo();
        if (score > 0)
        {
            OnIncrementScore?.Invoke(-score);
        }
    }
}
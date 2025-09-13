using System;
using System.Collections;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    protected SwipeDetection swipeDetection;

    [SerializeField]
    protected Configuration config;

    protected TouchManager touchManager;

    protected Board board;

    protected AxisData mainAxis;
    protected AxisData staticAxis;

    protected int sizeX;
    protected int sizeY;

    public event Action<Vector2Int, Vector2Int, int, int> OnMergeTile;
    public event Action<Vector2Int, Vector2Int> OnMoveTile;
    public event Action<Vector2Int, int, int> OnAddTile;
    public event Action<Vector2Int, int, int> OnUpdateTile;
    public event Action<Vector2Int> OnRemoveTile;

    public event Action<int> OnIncrementScore;
    public event Action<int> OnUpdateScore;
    public event Action OnGameOver;
    public event Action OnWin;

    protected bool winState = false;
    protected bool gameOver = false;

    protected string saveKey = "normal_";

    protected virtual void Awake()
    {
        touchManager = TouchManager.Instance;
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;
    }

    protected virtual void OnEnable()
    {
        sizeX = config.size.x;
        sizeY = config.size.y;

        board = new Board(sizeX, sizeY, config.players);
        touchManager.OnUndo += Undo;
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += SaveState;
        swipeDetection.OnSwipe += PrepareMovementData;

        board.OnMergeTile += MergeTile;
        board.OnMoveTile += MoveTile;
        board.OnAddTile += AddTile;
        board.OnUpdateTile += UpdateTile;
        board.OnRemoveTile += RemoveTile;

        board.OnGameOver += TriggerGameOver;

        StartCoroutine(StartBoard());
    }

    protected virtual void OnDisable()
    {
        touchManager.OnUndo -= Undo;
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;
        swipeDetection.OnSwipe -= PrepareMovementData;

        board.OnMergeTile -= MergeTile;
        board.OnMoveTile -= MoveTile;
        board.OnAddTile -= AddTile;
        board.OnUpdateTile -= UpdateTile;
        board.OnRemoveTile -= RemoveTile;

        board.OnGameOver -= TriggerGameOver;
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
        yield return null;
        LoadState();
    }

    protected virtual void PrepareMovementData(Vector2Int direction)
    {
        ProcessMovement(direction, 0, 0);
    }

    protected int ProcessMovement(Vector2Int direction, int playerId, int nextPlayerId)
    {

        // Do the actual movement for the tiles according to the direction.
        int moveScore = -1;
        if (direction.x != 0)
        {
            if (direction.x < 0)
            {
                mainAxis.start = sizeX - 1;
                mainAxis.end = -1;
            }
            else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeX;
            }
            mainAxis.update = direction.x;
            staticAxis.end = sizeY;
            moveScore = board.DetectTileMovement(mainAxis, staticAxis, true, playerId, nextPlayerId);
        }

        else if (direction.y != 0)
        {
            if (direction.y < 0)
            {
                mainAxis.start = sizeY - 1;
                mainAxis.end = -1;
            }
            else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeY;
            }
            mainAxis.update = direction.y;
            staticAxis.end = sizeX;
            moveScore = board.DetectTileMovement(mainAxis, staticAxis, false, playerId, nextPlayerId);
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

    protected void MergeTile(int x, int y, int dx, int dy, int value, int playerId)
    {
        OnMergeTile?.Invoke(new Vector2Int(x, y), new Vector2Int(dx, dy), value, playerId);
    }

    protected void MoveTile(int x, int y, int dx, int dy)
    {
        OnMoveTile?.Invoke(new Vector2Int(x, y), new Vector2Int(dx, dy));
    }

    protected void AddTile(int x, int y, int value, int playerId)
    {
        OnAddTile?.Invoke(new Vector2Int(x, y), value, playerId);
    }

    protected void UpdateTile(int x, int y, int value, int playerId)
    {
        OnUpdateTile?.Invoke(new Vector2Int(x, y), value, playerId);
    }

    protected void RemoveTile(int x, int y)
    {
        OnRemoveTile?.Invoke(new Vector2Int(x, y));
    }

    protected virtual void Restart()
    {
        OnUpdateScore?.Invoke(0);
        board.Restart();
        winState = false;
    }

    protected void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        gameOver = true;
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
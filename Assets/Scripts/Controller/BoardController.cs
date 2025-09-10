using System;
using System.Collections;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private SwipeDetection swipeDetection;

    [SerializeField]
    private Configuration config;

    private TouchManager touchManager;

    private Board board;

    private AxisData mainAxis;
    private AxisData staticAxis;

    private int sizeX;
    private int sizeY;

    public event Action<Vector2Int, Vector2Int, int> OnMergeTile;
    public event Action<Vector2Int, Vector2Int> OnMoveTile;
    public event Action<Vector2Int, int> OnAddTile;
    public event Action<Vector2Int, int> OnUpdateTile;
    public event Action<Vector2Int> OnRemoveTile;

    public event Action<int> OnIncrementScore;
    public event Action<int> OnUpdateScore;
    public event Action OnGameOver;
    public event Action OnWin;

    private bool winState = false;
    private bool gameOver = false;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;
    }

    private void OnEnable()
    {
        sizeX = config.size.x;
        sizeY = config.size.y;

        board = new Board(sizeX, sizeY);
        touchManager.OnUndo += Undo;
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += SaveState;
        swipeDetection.OnSwipe += ProcessMovement;

        board.OnMergeTile += MergeTile;
        board.OnMoveTile += MoveTile;
        board.OnAddTile += AddTile;
        board.OnUpdateTile += UpdateTile;
        board.OnRemoveTile += RemoveTile;

        board.OnGameOver += TriggerGameOver;

        StartCoroutine(StartBoard());
    }

    private void OnDisable()
    {
        touchManager.OnUndo -= Undo;
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;
        swipeDetection.OnSwipe -= ProcessMovement;

        board.OnMergeTile -= MergeTile;
        board.OnMoveTile -= MoveTile;
        board.OnAddTile -= AddTile;
        board.OnUpdateTile -= UpdateTile;
        board.OnRemoveTile -= RemoveTile;

        board.OnGameOver -= TriggerGameOver;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
        }
    }

    private IEnumerator StartBoard()
    {
        yield return null;
        LoadState();
    }

    private void ProcessMovement(Vector2Int direction)
    {

        // Do the actual movement for the tiles according to the direction.
        int moveScore = 0;
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
            moveScore = board.DetectTileMovement(mainAxis, staticAxis, true);
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
            moveScore = board.DetectTileMovement(mainAxis, staticAxis, false);
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
    }

    private void MergeTile(int x, int y, int dx, int dy, int value)
    {
        OnMergeTile?.Invoke(new Vector2Int(x, y), new Vector2Int(dx, dy), value);
    }

    private void MoveTile(int x, int y, int dx, int dy)
    {
        OnMoveTile?.Invoke(new Vector2Int(x, y), new Vector2Int(dx, dy));
    }

    private void AddTile(int x, int y, int value)
    {
        OnAddTile?.Invoke(new Vector2Int(x, y), value);
    }

    private void UpdateTile(int x, int y, int value)
    {
        OnUpdateTile?.Invoke(new Vector2Int(x, y), value);
    }

    private void RemoveTile(int x, int y)
    {
        OnRemoveTile?.Invoke(new Vector2Int(x, y));
    }

    private void Restart()
    {
        OnUpdateScore?.Invoke(0);
        board.Restart();
        winState = false;
    }

    private void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        gameOver = true;
    }

    private void SaveState()
    {
        if (gameOver)
        {
            // Force invalid size to trigger restart on the next load state.
            PlayerPrefs.SetInt("sizeX", -1);
            PlayerPrefs.SetInt("sizeY", -1);
            winState = false;
        } else
        {
            board.SaveState();
        }
        
        PlayerPrefs.SetInt("winState", winState ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        if (!board.LoadState())
        {
            Restart();
            return;
        }

        winState = PlayerPrefs.GetInt("winState", 0) == 1;
    }

    private void Undo()
    {
        int score = board.Undo();
        if (score > 0)
        {
            OnIncrementScore?.Invoke(-score);
        }
    }
}
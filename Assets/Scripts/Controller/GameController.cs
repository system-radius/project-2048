using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[DefaultExecutionOrder(1)]
public class GameController : MonoBehaviour
{
    [SerializeField]
    private int winCondition = 2048;

    [SerializeField]
    private SwipeDetection swipeDetection;

    [SerializeField]
    private TileViewController tileViewController;

    [SerializeField]
    private BackgroundController backgroundController;

    [SerializeField]
    private ScoreDataController scoreDataController;

    [SerializeField]
    private WinView winView;

    [SerializeField]
    private GameOverView gameOverView;

    private TouchManager touchManager;

    private MergeController mergeController;

    private Board board;

    private List<IInitializable> initializables = new List<IInitializable>();
    private List<IDisposable> disposables = new List<IDisposable>();
    private List<IDimensions> dimensionSetter = new List<IDimensions>();
    private List<IResetable> resetables = new List<IResetable>();
    private List<IPersistable> persistables = new List<IPersistable>();

    public event Action OnRestart;
    public event Action OnWin;

    private bool winFlag = false;

    private void Awake()
    {
        touchManager = TouchManager.Instance;

        board = new Board(4, 4);
        initializables.Add(board);

        mergeController = new MergeController(board, swipeDetection);
        initializables.Add(mergeController);
        disposables.Add(mergeController);

        dimensionSetter.Add(tileViewController);
        dimensionSetter.Add(backgroundController);

        resetables.Add(tileViewController);
        resetables.Add(scoreDataController);
        resetables.Add(board);

        persistables.Add(scoreDataController);
        persistables.Add(board);
    }

    private void OnEnable()
    {
        Initialize();
        touchManager.OnRestart += Restart;
        touchManager.OnUndo += board.Undo;
        board.OnIncrementScore += scoreDataController.ShowIncrement;
        board.OnUpdateScore += scoreDataController.UpdateScore;
        board.OnMergeTile += tileViewController.MergeTiles;
        board.OnMoveTile += tileViewController.MoveTile;
        board.OnAddTile += tileViewController.SpawnTile;
        board.OnRemoveTile += tileViewController.RemoveTile;
        board.OnUpdateTile += tileViewController.UpdateTile;
        board.OnMergeTile += DetectMerge;

        touchManager.OnUndo += winView.HideWinPanel;
        OnRestart += winView.HideWinPanel;
        OnWin += winView.ShowWinPanel;

        touchManager.OnUndo += gameOverView.HideGameOver;
        OnRestart += gameOverView.HideGameOver;
        board.OnGameOver += gameOverView.ShowGameOver;
    }

    private void OnDisable()
    {
        Dispose();
        touchManager.OnRestart -= Restart;
        touchManager.OnUndo -= board.Undo;
        board.OnIncrementScore -= scoreDataController.ShowIncrement;
        board.OnUpdateScore -= scoreDataController.UpdateScore;
        board.OnMergeTile -= tileViewController.MergeTiles;
        board.OnMoveTile -= tileViewController.MoveTile;
        board.OnAddTile -= tileViewController.SpawnTile;
        board.OnRemoveTile -= tileViewController.RemoveTile;
        board.OnUpdateTile -= tileViewController.UpdateTile;
        board.OnMergeTile -= DetectMerge;

        touchManager.OnUndo -= winView.HideWinPanel;
        OnRestart -= winView.HideWinPanel;
        OnWin -= winView.ShowWinPanel;

        touchManager.OnUndo -= gameOverView.HideGameOver;
        OnRestart -= gameOverView.HideGameOver;
        board.OnGameOver -= gameOverView.ShowGameOver;
    }

    private IEnumerator Start()
    {
        yield return null;
        SetDimensions();

        yield return null;
        Restart();
        LoadState();
    }

    private void Initialize()
    {
        foreach (var initializable in initializables)
        {
            initializable.Initialize();
        }
    }

    private void Dispose()
    {
        foreach(var disposable in disposables)
        {
            disposable.Dispose();
        }
    }

    private void SetDimensions()
    {
        Vector2Int dimensions = board.GetDimensions();
        foreach (var dimension in dimensionSetter) { 
            dimension.SetDimensions(dimensions.x, dimensions.y);
        }
    }

    private void Restart()
    {
        winFlag = false;
        foreach (var resetable in resetables)
        {
            resetable.Restart();
        }
        OnRestart?.Invoke();
    }

    private void DetectMerge(int x, int y, int dx, int dy, int value)
    {
        if (value == winCondition && !winFlag)
        {
            OnWin?.Invoke();
            winFlag = true;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("hasWon", winFlag ? 1 : 0);
        foreach (var persistable in persistables)
        {
            persistable.SaveState();
        }

        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        winFlag = PlayerPrefs.GetInt("hasWon", 0) == 1;
        foreach (var persistable in persistables)
        {
            persistable.LoadState();
        }
    }
}
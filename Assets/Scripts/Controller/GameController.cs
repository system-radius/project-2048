using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[DefaultExecutionOrder(1)]
public class GameController : MonoBehaviour
{

    [SerializeField]
    private int winCondition = 2048;

    private TouchManager touchManager;

    public event Action OnRestart;
    public event Action OnWin;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnRestart += Restart;
        /*touchManager.OnUndo += board.Undo;
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

        touchManager.OnCancel += startController.DisplayStart;*/
        touchManager.OnCancel += SaveState;

        //StartCoroutine(StartGameController());
    }

    private void OnDisable()
    {
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;
    }
/*
    private IEnumerator StartGameController()
    {
        yield return null;
        SetDimensions();

        yield return null;
        Restart();
        LoadState();
    }*/

    private void Restart()
    {
        /*winFlag = false;
        foreach (var resetable in resetables)
        {
            resetable.Restart();
        }*/
        OnRestart?.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //SaveState();
        }
    }

    private void SaveState()
    {
        /*
        PlayerPrefs.SetInt("hasWon", winFlag ? 1 : 0);
        foreach (var persistable in persistables)
        {
            persistable.SaveState();
        }

        PlayerPrefs.Save();
        */
    }

    /*
    private void LoadState()
    {
        winFlag = PlayerPrefs.GetInt("hasWon", 0) == 1;
        foreach (var persistable in persistables)
        {
            persistable.LoadState();
        }
    }*/
}
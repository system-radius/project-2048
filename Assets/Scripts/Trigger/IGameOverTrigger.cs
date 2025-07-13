using System;

public interface IGameOverTrigger
{
    event Action OnWin;
    event Action OnGameOver;
    event Action OnGameRestart;
}
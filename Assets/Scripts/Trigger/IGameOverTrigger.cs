using System;

public interface IGameOverTrigger
{
    event Action OnGameOver;
    event Action OnGameRestart;
}
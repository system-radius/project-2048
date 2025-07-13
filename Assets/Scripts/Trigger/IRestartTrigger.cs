using System;

public interface IRestartTrigger
{
    event Action OnRestart;
    event Action OnPlay;
}
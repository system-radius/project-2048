using System;

public interface IScoreUpdate
{
    event Action<int> OnUpdateScore;
}
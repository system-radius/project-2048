using System;

public interface IScoreChangeTrigger
{
    event Action<int> OnIncrementScore;
    event Action<int> OnUpdateScore;
}
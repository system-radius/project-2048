using System;

public interface IScoreIncrement
{
    event Action<int> OnIncrementScore;
}
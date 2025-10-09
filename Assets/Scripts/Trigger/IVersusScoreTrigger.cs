using System;

public interface IVersusScoreTrigger
{
    event Action<int, int> OnPlayerScore;
}
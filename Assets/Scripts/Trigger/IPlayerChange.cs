using System;

public interface IPlayerChange
{
    event Action<int> OnChangePlayer;
}
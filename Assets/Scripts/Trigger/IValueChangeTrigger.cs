using System;

public interface IValueChangeTrigger
{
    event Action<int> OnValueChange;
}
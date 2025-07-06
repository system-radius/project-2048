using Unity.VisualScripting;
using UnityEngine;

public class ValueTile : Tile
{
    public ValueTile() : base() {
        value = 2;
    }

    public void IncreaseValue()
    {
        value *= 2;
    }
}
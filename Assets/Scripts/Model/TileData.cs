public struct TileData
{
    public int value;
    public int playerId;

    public TileData(int value, int playerId)
    {
        this.value = value;
        this.playerId = playerId;
    }

    private int IncreaseValue()
    {
        value *= 2;
        return value;
    }

    public int Merge(TileData that, int playerId = 0)
    {
        if (!CanMerge(that, playerId))
        {
            return 0;
        }

        return IncreaseValue();
    }

    public bool CanMerge(TileData that, int playerId = 0)
    {
        return this.value == that.value && this.playerId == playerId;
    }
}
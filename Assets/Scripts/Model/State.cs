public class State
{
    private int[,] grid;
    private int score;

    private int sizeX, sizeY;

    public State(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        grid = new int[sizeX, sizeY];
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void Ready(int[,] tiles)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                grid[x, y] = tiles[x, y];
            }
        }
    }

    public int[,] GetGrid()
    {
        return grid;
    }

    public int GetScore()
    {
        return score;
    }
}
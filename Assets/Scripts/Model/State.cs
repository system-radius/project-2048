public class State
{
    private TileData[,] grid;
    private int score;

    private int sizeX, sizeY;

    private int playerMove;

    public State(int sizeX, int sizeY, int player = 0)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        grid = new TileData[this.sizeX, this.sizeY];
        playerMove = player;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void Ready(TileData[,] tiles)
    {
        /*
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                grid[x, y] = tiles[x, y];
            }
        }/**/

        grid = tiles;
    }

    public TileData[,] GetGrid()
    {
        return grid;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetPlayer()
    {
        return playerMove;
    }
}
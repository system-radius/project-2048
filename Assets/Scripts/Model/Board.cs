using System.Text;
using UnityEngine;

public class Board : IInitializable, IResetable, IPersistable
{
    private int sizeX = 4;
    private int sizeY = 4;

    private int stateMergeScore;
    private int[,] values;

    public event System.Action<int, int, int, int, int> OnMergeTile;
    public event System.Action<int, int, int, int> OnMoveTile;
    public event System.Action<int, int, int> OnAddTile;
    public event System.Action<int, int, int> OnUpdateTile;
    public event System.Action<int, int> OnRemoveTile;

    public event System.Action<int> OnIncrementScore;
    public event System.Action<int> OnUpdateScore;
    public event System.Action OnGameOver;

    private System.Collections.Generic.List<State> states = new System.Collections.Generic.List<State>();
    private State state;

    public Board(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public void Initialize()
    {
        values = new int[sizeX, sizeY];
    }

    public void Restart()
    {
        ClearBoard();

        AddTile();
        AddTile(true);
        /**/
    }

    public Vector2Int GetDimensions()
    {
        return new Vector2Int(sizeX, sizeY);
    }

    public void DetectTileMovement(AxisData mainAxis, AxisData staticAxis, bool horizontal)
    {
        bool hasMovement = MoveTiles(mainAxis, staticAxis, horizontal);
        if (hasMovement)
        {
            AddTile(true);
        }
    }

    private void AddTile(bool prepareState = false)
    {
        bool tileAdded = false;
        do
        {
            int x = Random.Range(0, sizeX);
            int y = Random.Range(0, sizeY);

            if (!HasNullTile())
            {
                break;
            }

            if (values[x, y] != 0) continue;

            SpawnTile(x, y, (Random.Range(0, 100) > 75) ? 4 : 2);
            //SpawnTile(x, y, (Random.Range(0, 100) > 75) ? 1024 : 512);
            //SpawnTile(x, y, 2);

            tileAdded = true;
        } while (!tileAdded);

        if (tileAdded && prepareState)
        {
            PrepareState();
        }

        if (!HasNullTile() && !CheckMergePossibilities())
        {
            OnGameOver?.Invoke();
        }
    }

    private void SpawnTile(int x, int y, int value) {
        values[x, y] = value;
        OnAddTile?.Invoke(x, y, value);
    }

    private bool HasNullTile()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                //if (tiles[x, y] == null) return true;
                if (values[x, y] == 0) return true;
            }
        }

        return false;
    }

    private void PrepareState()
    {
        if (state != null)
        {
            PushState();
        }
        state = new State(sizeX, sizeY);
        state.Ready(values);
    }

    private void PushState()
    {
        state.SetScore(stateMergeScore);
        states.Insert(0, state);
    }

    private bool PopState()
    {
        if (states.Count == 0) return false;
        state = states[0];
        states.RemoveAt(0);

        return true;
    }

    private bool MoveTiles(AxisData mainAxis, AxisData staticAxis, bool horizontal)
    {
        int moveMergeScore = 0;
        bool hasMovement = false;
        bool lookForMerge = true;
        for (int i = staticAxis.start; i != staticAxis.end; i += staticAxis.update)
        {
            for (int j = mainAxis.start; j != mainAxis.end; j += mainAxis.update)
            {
                int x = horizontal ? j : i;
                int y = horizontal ? i : j;

                var tile = values[x, y];
                lookForMerge = tile > 0;

                for (int k = j + mainAxis.update; k != mainAxis.end; k += mainAxis.update)
                {
                    int nextX = horizontal ? k : i;
                    int nextY = horizontal ? i : k;

                    var nextTile = values[nextX, nextY];
                    if (nextTile != 0)
                    {
                        if (lookForMerge)
                        {
                            int mergeScore = nextTile == tile ? nextTile + tile : 0;
                            if (mergeScore != 0)
                            {
                                moveMergeScore += mergeScore;
                                values[x, y] = mergeScore;
                                OnMergeTile?.Invoke(nextX, nextY, x, y, mergeScore);
                                values[nextX, nextY] = 0;
                                hasMovement = true;
                            }
                            // Break regardless of the merge succeeding or not, since this means that there is a block
                            // between the current tile for merging and any of the succeeding tiles.
                            break;
                        }
                        else
                        {
                            MoveTo(nextX, nextY, x, y, nextTile);
                            // Reload the tile value.
                            tile = values[x, y];
                            lookForMerge = true;
                        }
                        hasMovement = true;
                    }
                }
            }
        }

        if (moveMergeScore > 0)
        {
            OnIncrementScore?.Invoke(moveMergeScore);
        }
        stateMergeScore = moveMergeScore;
        return hasMovement;
    }

    private void MoveTo(int x, int y, int dx, int dy, int tile)
    {
        values[x, y] = 0;
        values[dx, dy] = tile;
        OnMoveTile?.Invoke(x, y, dx, dy);
    }

    private bool CheckMergePossibilities()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int tile = values[x, y];
                if (tile == 0 || CheckTileMergePossibility(x, y, tile)) return true;
            }
        }

        return false;
    }

    private bool CheckTileMergePossibility(int tileX, int tileY, int value)
    {
        for (int x = -1; x <= 1; x++)
        {
            int checkX = tileX + x;
            if (checkX < 0 || checkX >= sizeX) continue;
            for (int y = -1; y <= 1; y++)
            {
                int checkY = tileY + y;
                if (checkY < 0 || checkY >= sizeY) continue;
                if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

                var checkTile = values[checkX, checkY];
                if (checkTile == 0) continue;
                if (value == checkTile) return true;
            }
        }

        return false;
    }

    public void Undo()
    {
        if (!PopState()) return;
        int[,] grid = state.GetGrid();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int tile = values[x, y];
                if (tile == 0 && grid[x, y] == 0) continue;

                if (grid[x, y] == 0 && tile != 0)
                {
                    values[x, y] = 0;
                    OnRemoveTile?.Invoke(x, y);
                } else if (grid[x, y] > 0 && tile == 0)
                {
                    SpawnTile(x, y, grid[x, y]);
                } else if (grid[x, y] != tile)
                {
                    values[x, y] = grid[x, y];
                    OnUpdateTile?.Invoke(x, y, values[x, y]);
                }
            }
        }

        int stateScore = state.GetScore();
        if (stateScore > 0) OnIncrementScore?.Invoke(-stateScore);
    }

    public void SaveState()
    {
        PlayerPrefs.SetInt("sizeX", sizeX);
        PlayerPrefs.SetInt("sizeY", sizeY);

        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int value = values[x, y];
                PlayerPrefs.SetInt(x + "_" + y, value);
                sb.Append("[");
                sb.Append(value);
                sb.Append("] ");
            }
        }
    }

    public void LoadState()
    {
        int sizeX = PlayerPrefs.GetInt("sizeX", -1);
        int sizeY = PlayerPrefs.GetInt("sizeY", -1);
        if (this.sizeX != sizeX || this.sizeY != sizeY)
        {
            OnUpdateScore?.Invoke(0);
            Restart();
            return;
        }

        ClearBoard();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int value = PlayerPrefs.GetInt((x + "_" + y), 0);
                if (value > 0)
                {
                    SpawnTile(x, y, value);
                }
            }
        }

        PrepareState();
    }

    private void ClearBoard()
    {
        states.Clear();
        state = null;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (values[i, j] != 0)
                {
                    OnRemoveTile?.Invoke(i, j);
                }
                values[i, j] = 0;
            }
        }
    }

    private void PrintBoard()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = sizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < sizeX; x++)
            {
                sb.Append("[");
                sb.Append(values[x, y]);
                sb.Append("] ");
            }
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
    }
}
using System;
using System.Text;
using UnityEngine;

public class Board : IBoardTrigger
{
    private int sizeX = 4;
    private int sizeY = 4;
    private int players = 1;
    private int tile4Requirement = 75;
    private int tile4ReqDefault = 75;

    protected int stateMergeScore;
    //private int[,] values;
    private Tile[,] tiles;

    public event Action<Vector2Int, Vector2Int, int, int> OnMergeTile;
    public event Action<Vector2Int, Vector2Int> OnMoveTile;
    public event Action<Vector2Int, int, int> OnAddTile;
    public event Action<Vector2Int, int, int> OnUpdateTile;
    public event Action<Vector2Int> OnRemoveTile;

    public event System.Action OnGameOver;
    public event System.Action OnMerge;
    public event System.Action OnMove;

    private System.Collections.Generic.List<State> states = new System.Collections.Generic.List<State>();
    private State state;

    private Vector2Int[][] startingPoints;

    public Board(int sizeX, int sizeY, int players)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        this.players = players;

        tiles = new Tile[sizeX, sizeY];

        if (players > 0)
        {
            startingPoints = new Vector2Int[players][];
            startingPoints[0] = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(sizeX - 1, sizeY - 1),
            };

            startingPoints[1] = new Vector2Int[]
            {
                new Vector2Int(0, sizeY - 1),
                new Vector2Int(sizeX - 1, 0),
            };
        }
    }

    public Board(int sizeX, int sizeY) : this(sizeX, sizeY, 0)
    {
    }

    public void Restart()
    {
        ClearBoard();

        if (players <= 1)
        {
            AddTile(0);
            AddTile(0, true);
        }
        else
        {
            tile4ReqDefault = tile4Requirement = 100;
            for (int i = 0; i < players; i++)
            {
                Vector2Int[] startingPoint = startingPoints[i];
                foreach (var coord in startingPoint)
                {
                    SpawnTile(coord.x, coord.y, 2, i + 1);
                }
            }

            PrepareState();
        }
        /**/

        //PrintBoard();
    }

    public Vector2Int GetDimensions()
    {
        return new Vector2Int(sizeX, sizeY);
    }

    public virtual int DetectTileMovement(AxisData mainAxis, AxisData staticAxis, bool horizontal, int playerId = 0, int nextPlayerId = 0)
    {
        bool hasMovement = MoveTiles(mainAxis, staticAxis, horizontal, playerId);
        if (hasMovement)
        {
            FireMovementEvents();
            AddTile(nextPlayerId, true);
            //PrintBoard();
            return stateMergeScore;
        }

        return -1;
    }

    protected void FireMovementEvents()
    {
        if (stateMergeScore > 0)
        {
            OnMerge?.Invoke();
        }
        else
        {
            OnMove?.Invoke();
        }
    }

    public void AddTile(int playerId, bool prepareState = false)
    {
        bool tileAdded = false;
        do
        {
            int x = UnityEngine.Random.Range(0, sizeX);
            int y = UnityEngine.Random.Range(0, sizeY);

            if (!HasNullTile())
            {
                break;
            }

            if (tiles[x, y] != null) continue;

            SpawnTile(x, y, (UnityEngine.Random.Range(0, 100) > tile4Requirement) ? 4 : 2, playerId);
            //SpawnTile(x, y, (Random.Range(0, 100) > 75) ? 1024 : 512);
            //SpawnTile(x, y, 2);

            tileAdded = true;
        } while (!tileAdded);

        if (tileAdded && prepareState)
        {
            PrepareState();
        }

        if (!HasNullTile() && !CheckMergePossibilities(playerId))
        {
            OnGameOver?.Invoke();
        }
    }

    public void SpawnTile(int x, int y, int value, int playerId = 0) {
        tiles[x, y] = new Tile(value, playerId);
        OnAddTile?.Invoke(new Vector2Int(x, y), value, playerId);
    }

    private bool HasNullTile()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                //if (tiles[x, y] == null) return true;
                if (tiles[x, y] == null) return true;
            }
        }

        return false;
    }

    public void PrepareState()
    {
        if (state != null)
        {
            PushState();
        }
        state = new State(sizeX, sizeY);
        state.Ready(ConvertBoard());
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

    protected bool MoveTiles(AxisData mainAxis, AxisData staticAxis, bool horizontal, int playerId)
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

                var tile = tiles[x, y];
                lookForMerge = tile != null;

                for (int k = j + mainAxis.update; k != mainAxis.end; k += mainAxis.update)
                {
                    int nextX = horizontal ? k : i;
                    int nextY = horizontal ? i : k;

                    var nextTile = tiles[nextX, nextY];
                    if (nextTile != null)
                    {
                        if (lookForMerge)
                        {
                            int mergeScore = nextTile.Merge(tile, playerId);
                            if (mergeScore != 0)
                            {
                                moveMergeScore += mergeScore;
                                tiles[x, y] = nextTile;
                                OnMergeTile?.Invoke(new Vector2Int(nextX, nextY), new Vector2Int(x, y), mergeScore, playerId);
                                tiles[nextX, nextY] = null;
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
                            tile = tiles[x, y];
                            lookForMerge = true;
                        }
                        hasMovement = true;
                    }
                }
            }
        }

        stateMergeScore = moveMergeScore;
        return hasMovement;
    }

    private void MoveTo(int x, int y, int dx, int dy, Tile tile)
    {
        tiles[x, y] = null;
        tiles[dx, dy] = tile;
        OnMoveTile?.Invoke(new Vector2Int(x, y), new Vector2Int(dx, dy));
    }

    private bool CheckMergePossibilities(int playerId = 0)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Tile tile = tiles[x, y];
                if (tile == null || CheckTileMergePossibility(x, y, tile, playerId)) return true;
            }
        }

        return false;
    }

    private bool CheckTileMergePossibility(int tileX, int tileY, Tile tile, int playerId = 0)
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

                var checkTile = tiles[checkX, checkY];
                if (checkTile == null) continue;
                bool mergeStatus = tile.CanMerge(checkTile);
                //Debug.Log("[" + tileX + "," + tileY + "] -> [" + checkX + ", " + checkY + "]: " + mergeStatus);
                if (tile.CanMerge(checkTile, playerId)) return true;
            }
        }

        return false;
    }

    public int Undo()
    {
        if (!PopState()) return -1;
        TileData[,] grid = state.GetGrid();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Tile tile = tiles[x, y];
                TileData data = grid[x, y];
                if (tile == null && data.value == 0) continue;

                if (data.value == 0 && tile != null)
                {
                    tiles[x, y] = null;
                    OnRemoveTile?.Invoke(new Vector2Int(x, y));
                } else if (data.value > 0 && tile == null)
                {
                    SpawnTile(x, y, data.value, data.playerId);
                } else if (data.value != tile.value || data.playerId != tile.playerId)
                {
                    tiles[x, y].value = data.value;
                    tiles[x, y].playerId = data.playerId;
                    OnUpdateTile?.Invoke(new Vector2Int(x, y), tiles[x, y].value, tiles[x, y].playerId);
                }
            }
        }

        return state.GetScore();
    }

    public void SaveState(string key)
    {
        PlayerPrefs.SetInt(key + "sizeX", sizeX);
        PlayerPrefs.SetInt(key + "sizeY", sizeY);

        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int value = 0;
                int player = -1;
                if (tiles[x, y] != null)
                {
                    value = tiles[x, y].value;
                    player = tiles[x, y].playerId;
                }
                PlayerPrefs.SetInt(key + x + "_" + y, value);
                PlayerPrefs.SetInt(key + x + "_" + y + "@p", player);
                sb.Append("[");
                sb.Append(value);
                sb.Append("] ");
            }
        }
    }

    public bool LoadState(string key)
    {
        int sizeX = PlayerPrefs.GetInt(key + "sizeX", -1);
        int sizeY = PlayerPrefs.GetInt(key + "sizeY", -1);
        if (this.sizeX != sizeX || this.sizeY != sizeY) return false;

        ClearBoard();
        bool spawnedTile = false;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int value = PlayerPrefs.GetInt(key + x + "_" + y, 0);
                int player = PlayerPrefs.GetInt(key + x + "_" + y + "@p", -12);
                if (value > 0)
                {
                    SpawnTile(x, y, value, player);
                    spawnedTile = true;
                }
            }
        }

        if (!spawnedTile)
        {
            return false;
        }

        PrepareState();
        Utils.PrintBoard(state.GetGrid(), sizeX, sizeY);
        return true;
    }

    private void ClearBoard()
    {
        states.Clear();
        state = null;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    OnRemoveTile?.Invoke(new Vector2Int(i, j));
                }
                tiles[i, j] = null;
            }
        }
    }

    private TileData[,] ConvertBoard()
    {
        TileData[,] converted = new TileData[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0;j < sizeY; j++)
            {
                converted[i, j] = new TileData(0, 0);
                var tile = tiles[i, j];
                if (tile != null)
                {
                    converted[i, j] = new TileData(tile.value, tile.playerId);
                }
            }
        }

        return converted;
    }

    public State GetState()
    {
        State currentState = new State(sizeX, sizeY, players);
        currentState.Ready(ConvertBoard());
        return currentState;
    }

    private class Tile
    {
        public int value;
        public int playerId;

        public Tile(int value)
        {
            this.value = value;
        }

        public Tile(int value, int playerId)
        {
            this.value = value;
            this.playerId = playerId;
        }

        private int IncreaseValue()
        {
            value *= 2;
            return value;
        }

        public int Merge(Tile that, int playerId = 0)
        {
            if (!CanMerge(that, playerId))
            {
                return 0;
            }

            return IncreaseValue();
        }

        public bool CanMerge(Tile that, int playerId = 0)
        {
            return this.value == that.value && this.playerId == playerId;
        }
    }
}
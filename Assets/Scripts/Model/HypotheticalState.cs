using UnityEngine;

public class HypotheticalState : State
{
    private AxisData mainAxis;
    private AxisData staticAxis;
    private bool horizontal;

    public HypotheticalState(int sizeX, int sizeY, int player = 0, State prevState = null) : base(sizeX, sizeY, player)
    {
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;
        if (prevState != null)
        {
            CopyState(prevState);
        }
    }

    public HypotheticalState ApplyMove(Vector2Int move, int playerId)
    {
        HypotheticalState nextState = new HypotheticalState(sizeX, sizeY, playerMove, this);
        nextState.PrepareAxis(move);
        //Utils.PrintBoard(nextState.grid, sizeX, sizeY);
        if (nextState.MoveTiles(playerId, score))
        {
            //Utils.PrintBoard(nextState.grid, sizeX, sizeY);
            return nextState;
        }

        return null;
    }

    private bool MoveTiles(int playerId, int prevScore)
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

                var tile = grid[x, y];
                lookForMerge = tile.value != 0;

                for (int k = j + mainAxis.update; k != mainAxis.end; k += mainAxis.update)
                {
                    int nextX = horizontal ? k : i;
                    int nextY = horizontal ? i : k;

                    var nextTile = grid[nextX, nextY];
                    if (nextTile.value != 0)
                    {
                        if (lookForMerge)
                        {
                            int mergeScore = nextTile.Merge(tile, playerId);
                            if (mergeScore != 0)
                            {
                                moveMergeScore += mergeScore;
                                grid[x, y] = nextTile;
                                ResetGridTile(nextX, nextY);
                                hasMovement = true;
                            }
                            // Break regardless of the merge succeeding or not, since this means that there is a block
                            // between the current tile for merging and any of the succeeding tiles.
                            break;
                        }
                        else
                        {
                            grid[x, y] = nextTile;
                            ResetGridTile(nextX, nextY);
                            // Reload the tile value.
                            tile = grid[x, y];
                            lookForMerge = true;
                        }
                        hasMovement = true;
                    }
                }
            }
        }
        moveMergeScore = playerId == playerMove ? moveMergeScore : -moveMergeScore;
        score = prevScore + moveMergeScore;

        return hasMovement;
    }

    private void CopyState(State prevState) {
        var prevStateGrid = prevState.GetGrid();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                grid[i, j] = prevStateGrid[i, j];
            }
        }
    }

    private void PrepareAxis(Vector2Int move)
    {
        if (move.x != 0)
        {
            if (move.x < 0)
            {
                mainAxis.start = sizeX - 1;
                mainAxis.end = -1;
            } else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeX;
            }
            mainAxis.update = move.x;
            staticAxis.end = sizeY;
            horizontal = true;
        }

        else if (move.y != 0) {
            if (move.y < 0)
            {
                mainAxis.start = sizeY - 1;
                mainAxis.end = -1;
            }
            else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeY;
            }
            mainAxis.update = move.y;
            staticAxis.end = sizeX;
            horizontal = false;
        }
    }

    private void ResetGridTile(int x, int y)
    {
        grid[x, y].value = 0;
        grid[x, y].playerId = -1;
    }
}
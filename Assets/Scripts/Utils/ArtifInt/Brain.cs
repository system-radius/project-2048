using UnityEngine;

public class Brain
{
    protected static Vector2Int W = new Vector2Int(0, -1);
    protected static Vector2Int S = new Vector2Int(0, 1);
    protected static Vector2Int A = new Vector2Int(-1, 0);
    protected static Vector2Int D = new Vector2Int(1, 0);

    private int id;

    private int playerCount;

    private int depthLevel;

    protected Vector2Int[] moveList =
    {
        W, S, A, D
    };

    public Brain(int id, int playerCount, int depthLevel = 0)
    {
        this.id = id;
        this.playerCount = playerCount;
        this.depthLevel = depthLevel;
    }

    public int GetId()
    {
        return id;
    }

    public virtual Vector2Int Think(HypotheticalState state)
    {
        Vector2Int direction = new Vector2Int();

        int alpha = -int.MaxValue;
        int maxMoveValue = alpha;
        foreach (var move in moveList)
        {
            //Debug.Log("[" + move + "/max] Checking move!");
            int beta = int.MaxValue;
            int minMoveValue = beta;
            var nextState = state.ApplyMove(move, id);
            if (nextState == null)
            {
                //Debug.Log("[" + move + "/max] No next state found!");
                continue;
            }
            minMoveValue = Mathf.Min(minMoveValue, Minimax(id + 1, depthLevel, alpha, beta, false, nextState));
            beta = minMoveValue;
            //Debug.Log("[" + move + "/max] Back to highest level! Move value: " + minMoveValue + ", alpha: " + alpha + ", beta: " + beta);
            if (minMoveValue >= maxMoveValue)
            {
                direction = move;
                maxMoveValue = minMoveValue;
                alpha = maxMoveValue;
            }
        }

        return direction;
    }

    private int Minimax(int playerId, int depth, int alpha, int beta, bool maximizing, HypotheticalState state)
    {
        playerId = playerId > playerCount ? 1 : playerId;

        if (depth == 0)
        {
            //return ComputeStateScore(state);
            int score = ComputeStateScore(state);
            //Debug.Log("Reached max depth! Score: " + score);
            return score;
        }

        int multiplier = maximizing ? 1 : -1;
        int best = -int.MaxValue;
        foreach (var move in moveList)
        {
            //Debug.Log("[" + move + "/" + (maximizing?"max":"min") + "] Checking move!");
            var nextState = state.ApplyMove(move, playerId);
            if (nextState == null)
            {
                //Debug.Log("[" + move + "/" + (maximizing ? "max" : "min") + "] No next state found!");
                continue;
            }
            int stateValue = Minimax(playerId + 1, depth - 1, alpha, beta, !maximizing, nextState);
            best = Mathf.Max(best, stateValue * multiplier);
            if (maximizing)
            {
                alpha = Mathf.Max(alpha, best);
            } else
            {
                beta = Mathf.Max(beta, -best);
            }

            if (alpha >= beta) break;
        }

        return best * multiplier;
    }

    private int ComputeStateScore(HypotheticalState state)
    {
        return state.GetScore();
    }

}
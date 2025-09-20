using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomBrain : Brain
{
    public RandomBrain(int id, int playerCount, int depthLevel = 0) : base(id, playerCount, depthLevel)
    {
    }

    public override Vector2Int Think(HypotheticalState state)
    {
        List<Vector2Int> availableMoves = moveList.ToList();
        availableMoves.Shuffle();
        Vector2Int move = new Vector2Int(0, 0);
        foreach (var checkMove in availableMoves)
        {
            if (state.ApplyMove(checkMove, GetId()) != null)
            {
                move = checkMove;
                break;
            }
        }

        return move;
    }
}
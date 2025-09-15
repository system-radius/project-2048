using System.Collections.Generic;
using UnityEngine;

public class RandomBrain : Brain
{
    public RandomBrain(int id, int playerCount, int depthLevel = 0) : base(id, playerCount, depthLevel)
    {
    }

    public override Vector2Int Think(HypotheticalState state)
    {
        List<Vector2Int> checkedMoves = new();
        Vector2Int move = new Vector2Int(0, 0);
        do
        {
            if (checkedMoves.Count >= 4)
            {
                Debug.Log("Break!");
                break;
            }

            var checkMove = moveList[Random.Range(0, moveList.Length)];
            if (checkedMoves.Contains(checkMove)) continue;
            if (state.ApplyMove(checkMove, GetId()) != null)
            {
                move = checkMove;
            }
        } while (move.magnitude == 0);

        return move;
    }
}
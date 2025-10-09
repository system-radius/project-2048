public class TutorialBoard : Board
{
    public TutorialBoard(int sizeX, int sizeY) : base(sizeX, sizeY, 2)
    {
    }

    public override int DetectTileMovement(AxisData mainAxis, AxisData staticAxis, bool horizontal, int playerId = 0, int nextPlayerId = 0)
    {
        bool hasMovement = MoveTiles(mainAxis, staticAxis, horizontal, playerId);
        if (hasMovement)
        {
            FireMovementEvents();
            return stateMergeScore;
        }

        return -1;
    }
}
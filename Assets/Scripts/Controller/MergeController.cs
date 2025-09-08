using UnityEngine;

public class MergeController : IInitializable, IDisposable
{
    private SwipeDetection swipeDetection;

    private Board board;

    private AxisData mainAxis;
    private AxisData staticAxis;

    private int sizeX;
    private int sizeY;

    public MergeController(Board board, SwipeDetection swipeDetection)
    {
        this.swipeDetection = swipeDetection;
        this.board = board;
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;
    }

    public void Initialize()
    {
        var dimensions = board.GetDimensions();
        sizeX = dimensions.x;
        sizeY = dimensions.y;
        swipeDetection.OnSwipe += ProcessMovement;
    }

    public void Dispose()
    {
        swipeDetection.OnSwipe -= ProcessMovement;
    }

    private void ProcessMovement(Vector2Int direction)
    {

        // Do the actual movement for the tiles according to the direction.
        if (direction.x != 0)
        {
            if (direction.x < 0)
            {
                mainAxis.start = sizeX - 1;
                mainAxis.end = -1;
            }
            else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeX;
            }
            mainAxis.update = direction.x;
            staticAxis.end = sizeY;
            board.DetectTileMovement(mainAxis, staticAxis, true);
        }

        else if (direction.y != 0)
        {
            if (direction.y < 0)
            {
                mainAxis.start = sizeY - 1;
                mainAxis.end = -1;
            }
            else
            {
                mainAxis.start = 0;
                mainAxis.end = sizeY;
            }
            mainAxis.update = direction.y;
            staticAxis.end = sizeX;
            board.DetectTileMovement(mainAxis, staticAxis, false);
        }
    }
}
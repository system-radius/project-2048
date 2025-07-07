using UnityEngine;

public class FieldController : MonoBehaviour, IScoreChangeTrigger
{
    [SerializeField]
    private CameraScaleController worldCamera;

    [SerializeField]
    private SwipeDetection swipeDetection;

    private TouchManager touchManager;

    [SerializeField]
    private int sizeX = 4;

    [SerializeField]
    private int sizeY = 4;

    [SerializeField]
    private GameObject blankPrefab;

    [SerializeField]
    private ValueTileController tilePrefab;

    [SerializeField]
    private GameObject backgroundPrefab;

    private GameObject container;

    private ValueTileController[,] valueTiles;
    private AxisData mainAxis;
    private AxisData staticAxis;

    private int totalScore;

    public event System.Action<int> OnIncrementScore;
    public event System.Action<int> OnUpdateScore;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;
    }

    private void OnEnable()
    {
        touchManager.OnRestart += Restart;
        swipeDetection.OnSwipe += ProcessMovement;
    }

    private void OnDisable()
    {
        touchManager.OnRestart -= Restart;
        swipeDetection.OnSwipe -= ProcessMovement;
    }

    private void Start()
    {
        container = new GameObject("Container");
        GameObject background = Instantiate(backgroundPrefab, container.transform);
        float multiplier = 0.5f;
        background.transform.position = new Vector3((sizeX - 1) * multiplier, (sizeY - 1) * multiplier, 0);
        multiplier = 1.6f;
        background.transform.localScale = new Vector3(sizeX * multiplier, sizeY * multiplier, 0);
        for (int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                GameObject tile = Instantiate(blankPrefab, container.transform);
                tile.transform.position = new Vector3(i, j, -1);
            }
        }

        worldCamera.AdjustCameraSize(container.transform);
        container.transform.position = new Vector3(0, 0, 10);

        Restart();
    }

    private void Restart()
    {
        totalScore = 0;
        OnUpdateScore?.Invoke(totalScore);

        if (valueTiles != null)
        {
            foreach (ValueTileController valueTile in valueTiles)
            {
                if (valueTile != null) Destroy(valueTile.gameObject);
            }
        }

        valueTiles = new ValueTileController[sizeX, sizeY];
        AddTile();
        AddTile();
    }

    private void ProcessMovement(Vector2Int direction)
    {
        foreach (ValueTileController valueTile in valueTiles)
        {
            if (valueTile != null)
            {
                valueTile.ClearMergeStatus();
            }
        }

        // Do the actual movement for the tiles according to the direction.
        bool hasMovement = false;
        if (direction.x != 0)
        {
            if (direction.x < 0) {
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
            hasMovement = MoveTiles(mainAxis, staticAxis, true);
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
            hasMovement = MoveTiles(mainAxis, staticAxis, false);
        }

        if (hasMovement) AddTile();
    }

    private bool MoveTiles(AxisData mainAxis, AxisData staticAxis, bool horizontal)
    {
        int moveMergeScore = 0;
        bool hasMovement = false;
        for (int i = staticAxis.start; i != staticAxis.end; i += staticAxis.update)
        {
            for (int j = mainAxis.start; j != mainAxis.end; j += mainAxis.update)
            {
                int x = horizontal ? j : i;
                int y = horizontal ? i : j;

                var tile = valueTiles[x, y];
                if (tile != null)
                {
                    if (j == mainAxis.start) continue;

                    int lastEmptyIndex = -1;
                    for (int k = j - mainAxis.update; k != mainAxis.start - mainAxis.update; k -= mainAxis.update)
                    {
                        int tempX = horizontal ? k : i;
                        int tempY = horizontal ? i : k;

                        var tempTile = valueTiles[tempX, tempY];
                        if (tempTile != null)
                        {
                            int mergeScore = tempTile.AttemptMerge(tile);
                            if (mergeScore != 0)
                            {
                                Destroy(tile.gameObject);
                                valueTiles[x, y] = null;
                                lastEmptyIndex = -1;
                                hasMovement = true;
                            }
                            moveMergeScore += mergeScore;
                            break;
                        }
                        else 
                        {
                            lastEmptyIndex = k;
                        }
                    }

                    if (lastEmptyIndex == -1) continue;

                    int xUpdate = horizontal ? lastEmptyIndex : i;
                    int yUpdate = horizontal ? i : lastEmptyIndex;
                    tile.MoveTo(xUpdate, yUpdate);
                    valueTiles[x, y] = null;
                    valueTiles[xUpdate, yUpdate] = tile;
                    hasMovement = true;
                }
            }
        }

        if (moveMergeScore > 0)
        {
            // Fire an event to show the moveMergeScore as increment.
            OnIncrementScore?.Invoke(moveMergeScore);

            // Then fire another event to actually update the score view.
            totalScore += moveMergeScore;
            OnUpdateScore?.Invoke(totalScore);
        }
        return hasMovement;
    }

    private void AddTile()
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

            if (valueTiles[x, y] != null) continue;

            ValueTileController valueTile = Instantiate(tilePrefab);
            valueTile.transform.position = new Vector3(x, y, 0);
            valueTiles[x, y] = valueTile;

            valueTile.Initialize((Random.Range(0, 100) > 75) ? 4 : 2);

            tileAdded = true;
        } while (!tileAdded);

        // Additional checking for if the new tile was not added due to the board being full.
    }

    private bool HasNullTile()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {
                if (valueTiles[x, y] == null) return true;
            }
        }

        return false;
    }

}

public class AxisData
{
    public int start;
    public int end;
    public int update;
    public int min;
    public int max;
}
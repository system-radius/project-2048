using System.Collections;
using UnityEngine;

public class FieldController : MonoBehaviour, IScoreChangeTrigger, IGameOverTrigger
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

    private bool hasWon = false;
    private int winCondition = 2048;

    private int totalScore;

    public event System.Action<int> OnIncrementScore;
    public event System.Action<int> OnUpdateScore;
    public event System.Action OnGameOver;
    public event System.Action OnGameRestart;
    public event System.Action OnWin;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
        mainAxis = new AxisData();
        staticAxis = new AxisData();
        staticAxis.start = 0;
        staticAxis.update = 1;

        //LoadState();
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

    private IEnumerator Start()
    {
        yield return null;
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

        Restart(false);
        LoadState();

        yield return null;
        hasWon = PlayerPrefs.GetInt("hasWon", 0) == 1;
        totalScore = PlayerPrefs.GetInt("currentScore", 0);
        OnUpdateScore?.Invoke(totalScore);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
            PlayerPrefs.Save();
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("hasWon", hasWon ? 1 : 0);
        PlayerPrefs.SetInt("sizeX", sizeX);
        PlayerPrefs.SetInt("sizeY", sizeY);
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int value = 0;
                var tile = valueTiles[x, y];
                if (tile != null)
                {
                    value = tile.GetValue();
                }
                PlayerPrefs.SetInt(x + "_" + y, value);
            }
        }
    }

    private void LoadState()
    {
        int sizeX = PlayerPrefs.GetInt("sizeX", -1);
        int sizeY = PlayerPrefs.GetInt("sizeY", -1);
        if (this.sizeX != sizeX || this.sizeY != sizeY) {
            AddTile();
            AddTile();
            return;
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0;y < sizeY; y++)
            {
                int value = PlayerPrefs.GetInt((x + "_" + y), 0);
                if (value > 0)
                {
                    LoadTile(x, y, value);
                }
            }
        }
    }

    private void Restart()
    {
        Restart(true);
    }

    private void Restart(bool fromButton)
    {
        OnGameRestart?.Invoke();

        if (valueTiles != null)
        {
            foreach (ValueTileController valueTile in valueTiles)
            {
                if (valueTile != null) Destroy(valueTile.gameObject);
            }
        }

        valueTiles = new ValueTileController[sizeX, sizeY];
        if (fromButton)
        {
            hasWon = false;
            totalScore = 0;
            OnUpdateScore?.Invoke(totalScore);
            AddTile();
            AddTile();
        }
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

        if (hasMovement)
        {
            AddTile();
        }

        // Additional checking for if the new tile was not added due to the board being full.
        if (!HasNullTile() && !CheckMergePossibilities())
        {
            OnGameOver?.Invoke();
        }
    }

    private bool MoveTiles(AxisData mainAxis, AxisData staticAxis, bool horizontal)
    {
        int moveMergeScore = 0;
        bool hasMovement = false;
        bool win = hasWon;
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
                            if (!win)
                            {
                                win = winCondition == mergeScore;
                            }
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

        if (win && !hasWon)
        {
            hasWon = true;
            OnWin?.Invoke();
            SaveState();
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

    private void LoadTile(int x, int y, int value)
    {
        ValueTileController valueTile = Instantiate(tilePrefab, container.transform);
        valueTile.transform.position = new Vector3(x, y, 0);
        valueTiles[x, y] = valueTile;

        valueTile.Initialize(value);
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

            LoadTile(x, y, (Random.Range(0, 100) > 75) ? 4 : 2);

            tileAdded = true;
        } while (!tileAdded);
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

    private bool CheckMergePossibilities()
    {
        foreach (ValueTileController tile in valueTiles)
        {
            if (tile == null) continue;
            if (CheckTileMergePossibility(tile)) return true;
        }

        return false;
    }

    private bool CheckTileMergePossibility(ValueTileController tile)
    {
        int tileX = (int) tile.transform.position.x;
        int tileY = (int) tile.transform.position.y;
        int value = tile.GetValue();

        for (int x = -1; x <= 1; x++)
        {
            int checkX = tileX + x;
            if (checkX < 0 || checkX >= sizeX) continue;
            for (int y = -1; y <= 1; y++)
            {
                int checkY = tileY + y;
                if (checkY < 0 || checkY >= sizeY) continue;
                if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

                var checkTile = valueTiles[checkX, checkY];
                if (checkTile == null) continue;
                if (value == checkTile.GetValue()) return true;
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
using UnityEngine;

[DefaultExecutionOrder(3)]
public class TileViewController : MonoBehaviour, IDimensions, IResetable
{
    [SerializeField]
    private TileView tileViewPrefab;

    private TileView[,] tileViews;

    private int sizeX, sizeY;

    private GameObject container;

    /*
    public void SetBoard(Board board)
    {
        container = new GameObject("Tiles_Container");

        if (this.board != null)
        {
            foreach (var tile in tileViews)
            {
                Destroy(tile.gameObject);
            }
        }

        this.board = board;
        var dim = board.GetDimensions();
        sizeX = dim.x;
        sizeY = dim.y;

        tileViews = new TileView[sizeX, sizeY];
        EnableEvents();
    }/**/

    private void OnEnable()
    {
        //EnableEvents();
    }

    private void OnDisable()
    {
        //DisableEvents();
        Destroy(container);
        container = null;
    }

    /*
    private void EnableEvents()
    {
        if (board == null || enableFlag) return;
        board.OnAddTile += SpawnTile;
        board.OnMoveTile += MoveTile;
        board.OnMergeTile += MergeTiles;
        enableFlag = true;
        Debug.Log("Events registration complete!");
    }

    private void DisableEvents()
    {
        if (board == null) return;
        board.OnAddTile -= SpawnTile;
        board.OnMoveTile -= MoveTile;
        board.OnMergeTile -= MergeTiles;
        enableFlag = false;
    }/**/

    public void SpawnTile(int x, int y, int value)
    {
        TileView tileView = Instantiate(tileViewPrefab, container.transform);
        tileView.transform.position = new Vector3(x, y, 0);
        tileView.transform.localScale = Vector3.zero;
        tileView.ChangeValue(value);

        tileViews[x, y] = tileView;
    }

    public void MoveTile(int x, int y, int dx, int dy)
    {
        TileView tileView = tileViews[x, y];
        if (tileView == null) return;
        tileView.MoveTo(dx, dy);
        tileViews[x, y] = null;
        tileViews[dx, dy] = tileView;
    }

    public void MergeTiles(int x, int y, int dx, int dy, int value)
    {
        TileView source = tileViews[x, y];
        TileView target = tileViews[dx, dy];

        if (source == null || target == null) return;

        StartCoroutine(source.Merge(dx, dy));
        target.ChangeValue(value, 1.2f, true);
    }

    public void UpdateTile(int x, int y, int value)
    {
        TileView tileView = tileViews[x, y];
        tileView.ChangeValue(value, 0.8f, true);
    }

    public void RemoveTile(int x, int y)
    {
        TileView tileView = tileViews[x, y];
        if (tileView == null) return;
        StartCoroutine(tileView.Delete());
    }

    public void SetDimensions(float x, float y)
    {
        sizeX = (int)x;
        sizeY = (int)y;
    }

    public void Restart()
    {
        if (container != null)
        {
            Destroy(container);
        }

        container = new GameObject("Tiles Container");

        if (tileViews != null)
        {
            foreach (var tileView in tileViews)
            {
                if (tileView != null)
                {
                    Destroy(tileView.gameObject);
                }
            }
        }

        tileViews = new TileView[sizeX, sizeY];
    }
}
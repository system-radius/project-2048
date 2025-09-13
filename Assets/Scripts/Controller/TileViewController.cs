using UnityEngine;

[DefaultExecutionOrder(3)]
public class TileViewController : MonoBehaviour
{
    [SerializeField]
    private Configuration config;

    [SerializeField]
    private BoardController boardController;

    [SerializeField]
    private TileView tileViewPrefab;

    private TileView[,] tileViews;

    private int sizeX, sizeY;

    private GameObject container;

    private void OnEnable()
    {
        sizeX = config.size.x;
        sizeY = config.size.y;
        Restart();

        boardController.OnMergeTile += MergeTiles;
        boardController.OnMoveTile += MoveTile;
        boardController.OnAddTile += SpawnTile;
        boardController.OnUpdateTile += UpdateTile;
        boardController.OnRemoveTile += RemoveTile;
    }

    private void OnDisable()
    {
        Destroy(container);
        container = null;

        boardController.OnMergeTile -= MergeTiles;
        boardController.OnMoveTile -= MoveTile;
        boardController.OnAddTile -= SpawnTile;
        boardController.OnUpdateTile -= UpdateTile;
        boardController.OnRemoveTile -= RemoveTile;
    }

    private void Start()
    {
    }

    public void SpawnTile(Vector2Int coord, int value, int playerId)
    {
        TileView tileView = Instantiate(tileViewPrefab, container.transform);
        tileView.transform.position = new Vector3(coord.x, coord.y, 0);
        tileView.transform.localScale = Vector3.zero;
        tileView.ChangeValue(value, playerId);

        tileViews[coord.x, coord.y] = tileView;
    }

    public void MoveTile(Vector2Int coord, Vector2Int delta)
    {
        TileView tileView = tileViews[coord.x, coord.y];
        if (tileView == null) return;
        tileView.MoveTo(delta.x, delta.y);
        tileViews[coord.x, coord.y] = null;
        tileViews[delta.x, delta.y] = tileView;
    }

    public void MergeTiles(Vector2Int coord, Vector2Int delta, int value, int playerId)
    {
        TileView source = tileViews[coord.x, coord.y];
        TileView target = tileViews[delta.x, delta.y];

        if (source == null || target == null) return;

        StartCoroutine(source.Merge(delta.x, delta.y));
        target.ChangeValue(value, playerId, 1.2f, true);
    }

    public void UpdateTile(Vector2Int coord, int value, int playerId)
    {
        TileView tileView = tileViews[coord.x, coord.y];
        tileView.ChangeValue(value, playerId, 0.8f, true);
    }

    public void RemoveTile(Vector2Int coord)
    {
        TileView tileView = tileViews[coord.x, coord.y];
        if (tileView == null) return;
        StartCoroutine(tileView.Delete());
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
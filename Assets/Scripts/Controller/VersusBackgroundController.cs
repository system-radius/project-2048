using UnityEngine;

public class VersusBackgroundController : BackgroundController
{
    [SerializeField]
    private GameObject versusBoardController;
    private IPlayerChange playerChange;

    private Color32[] bgColors =
    {
        Utils.HexToColor32("#610000"),
        Utils.HexToColor32("#000061"),
    };

    private void Awake()
    {
        playerChange = versusBoardController.GetComponent<IPlayerChange>();
        if (playerChange == null)
        {
            Debug.LogError("Player change object is null!");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerChange.OnChangePlayer += ChangeBackground;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerChange.OnChangePlayer -= ChangeBackground;
        worldCamera.Reset();
    }

    private void ChangeBackground(int playerId)
    {
        worldCamera.ChangeSkyBox(bgColors[playerId - 1]);
    }
}
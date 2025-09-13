using UnityEngine;

public class VersusBackgroundController : BackgroundController
{
    [SerializeField]
    private VersusBoardController versusBoardController;

    private Color32[] bgColors =
    {
        Utils.HexToColor32("#610000"),
        Utils.HexToColor32("#000061"),
    };

    protected override void OnEnable()
    {
        base.OnEnable();
        versusBoardController.OnChangePlayer += ChangeBackground;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        versusBoardController.OnChangePlayer -= ChangeBackground;
        worldCamera.Reset();
    }

    private void ChangeBackground(int playerId)
    {
        worldCamera.ChangeSkyBox(bgColors[playerId - 1]);
    }
}
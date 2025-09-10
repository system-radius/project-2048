using UnityEngine;

public class GameStateViewController : MonoBehaviour
{
    private TouchManager touchManager;

    [SerializeField]
    private BoardController boardController;

    [SerializeField]
    private WinView winView;

    [SerializeField]
    private GameOverView gameOverView;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        boardController.OnWin += winView.ShowWinPanel;
        boardController.OnGameOver += gameOverView.ShowGameOver;

        touchManager.OnRestart += Restart;
        touchManager.OnUndo += Restart;
        touchManager.OnCancel += Restart;
    }

    private void OnDisable()
    {
        boardController.OnWin -= winView.ShowWinPanel;
        boardController.OnGameOver -= gameOverView.ShowGameOver;

        touchManager.OnRestart -= Restart;
        touchManager.OnUndo -= Restart;
        touchManager.OnCancel -= Restart;
    }

    private void Restart()
    {
        winView.HideWinPanel();
        gameOverView.HideGameOver();
    }
}
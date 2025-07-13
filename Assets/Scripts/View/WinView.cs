using UnityEngine;

public class WinView : MonoBehaviour
{
    [SerializeField]
    private GameObject winGameObject;
    private IGameOverTrigger winTrigger;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private GameObject scoreRestartButton;

    private TouchManager touchManager;

    private void Awake()
    {
        winTrigger = winGameObject.GetComponent<IGameOverTrigger>();
        if (winTrigger == null)
        {
            Debug.LogError("Game Over Trigger not found!");
        }

        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        winTrigger.OnWin += ShowWinPanel;
        winTrigger.OnGameRestart += HideWinPanel;
        touchManager.OnPlay += HideWinPanel;
    }

    private void OnDisable()
    {
        winTrigger.OnWin -= ShowWinPanel;
        winTrigger.OnGameRestart -= HideWinPanel;
        touchManager.OnPlay -= HideWinPanel;
    }

    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
        scoreRestartButton.SetActive(false);
    }

    private void HideWinPanel()
    {
        winPanel?.SetActive(false);
        scoreRestartButton?.SetActive(true);
    }
}
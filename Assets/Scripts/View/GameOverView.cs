using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverPanel;

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
    }
}

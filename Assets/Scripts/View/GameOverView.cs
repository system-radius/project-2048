using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    private string defaultText;
    private void Awake()
    {
        defaultText = gameOverText.text;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowGameOverText(string text)
    {
        ShowGameOver();
        gameOverText.text = text;
    }

    public void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
        gameOverText.text = defaultText;
    }
}

using System.Runtime.CompilerServices;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverObject;
    private IGameOverTrigger gameOverTrigger;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject scoreRestartButton;

    private void Awake()
    {
        gameOverTrigger = gameOverObject.GetComponent<IGameOverTrigger>();
        if ( gameOverTrigger == null )
        {
            Debug.LogError("Game Over Trigger not found!");
        }
    }

    private void OnEnable()
    {
        gameOverTrigger.OnGameOver += ShowGameOver;
        gameOverTrigger.OnGameRestart += HideGameOver;
    }

    private void OnDisable()
    {
        gameOverTrigger.OnGameOver -= ShowGameOver;
        gameOverTrigger.OnGameRestart -= HideGameOver;
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        scoreRestartButton.SetActive(false);
    }

    private void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
        scoreRestartButton?.SetActive(true);
    }
}

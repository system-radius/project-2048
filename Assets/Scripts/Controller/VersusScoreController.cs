using System.Collections.Generic;
using UnityEngine;

public class VersusScoreController : MonoBehaviour
{
    private TouchManager touchManager;

    [SerializeField]
    private VersusBoardController versusBoardController;

    [SerializeField]
    private List<ScoreView> scoreBoard = new List<ScoreView>();

    [SerializeField]
    private GameOverView gameOverView;

    private int[] scores;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        versusBoardController.OnGameOver += TriggerGameOver;
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += Restart;
        versusBoardController.OnPlayerScore += ShowPlayerScoreIncrement;

        Restart();
    }

    private void OnDisable()
    {
        versusBoardController.OnGameOver -= TriggerGameOver;
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= Restart;
        versusBoardController.OnPlayerScore -= ShowPlayerScoreIncrement;
    }

    private void ShowPlayerScoreIncrement(int increment, int playerId)
    {
        playerId -= 1;
        var scoreView = scoreBoard[playerId];
        scoreView.ShowIncrement(increment);
        scores[playerId] += increment;
        scoreView.UpdateScore(scores[playerId]);
    }

    private void Restart()
    {
        scores = new int[scoreBoard.Count];
        for (int i = 0; i < scores.Length; i++)
        {
            var scoreView = scoreBoard[i];
            scoreView.UpdateScore(scores[i]);
        }

        gameOverView.HideGameOver();
    }

    private void TriggerGameOver()
    {
        Debug.Log("[VersusScoreController] Triggered game over!");
        int highestScoreIndex = -1;
        int highestScore = -1;
        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] > highestScore)
            {
                highestScore = scores[i];
                highestScoreIndex = i;
            }
        }

        gameOverView.ShowGameOverText("Player " + (highestScoreIndex + 1) + " has won!");
    }
}
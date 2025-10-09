using System.Collections.Generic;
using UnityEngine;

public class VersusScoreController : MonoBehaviour, IInitializable<IVersusScoreTrigger>, IInitializable<IGameOverTrigger>, ITerminable
{
    [SerializeField]
    private TouchManager touchManager;

    [SerializeField]
    private List<ScoreView> scoreBoard = new List<ScoreView>();

    [SerializeField]
    private GameOverView gameOverView;

    private int[] scores;

    private IGameOverTrigger gameOverTrigger;
    private IVersusScoreTrigger scoreTrigger;

    public void Initialize(IVersusScoreTrigger value)
    {
        value.OnPlayerScore += ShowPlayerScoreIncrement;
        scoreTrigger = value;
    }

    public void Initialize(IGameOverTrigger value)
    {
        value.OnGameOver += TriggerGameOver;
        gameOverTrigger = value;

        touchManager.OnRestart += Restart;
        touchManager.OnCancel += Restart;

        Restart();
    }

    public void Terminate()
    {
        gameOverTrigger.OnGameOver -= TriggerGameOver;
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= Restart;
        scoreTrigger.OnPlayerScore -= ShowPlayerScoreIncrement;
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
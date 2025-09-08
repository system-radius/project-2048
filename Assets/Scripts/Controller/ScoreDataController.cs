using UnityEngine;

public class ScoreDataController : MonoBehaviour, IResetable, IPersistable
{
    [SerializeField]
    private ScoreView currentScoreView;
    [SerializeField]
    private ScoreView bestScoreView;

    private int currentScore;
    private int bestScore;

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        bestScoreView.UpdateScore(bestScore);
    }

    public void UpdateScore(int score)
    {
        currentScore = score;
        currentScoreView.UpdateScore(score);

        if (currentScore >= bestScore)
        {
            bestScore = currentScore;
            bestScoreView.UpdateScore(score);
        }
    }

    public void ShowIncrement(int incrementScore)
    {
        currentScoreView.ShowIncrement(incrementScore);
        currentScore += incrementScore;
        currentScoreView.UpdateScore(currentScore);

        if (currentScore >= bestScore)
        {
            bestScore = currentScore;
            bestScoreView.UpdateScore(currentScore);
        }
    }

    public void SaveState()
    {
        PlayerPrefs.SetInt("bestScore", bestScore);
        PlayerPrefs.SetInt("currentScore", currentScore);
    }

    public void LoadState()
    {
        currentScore = PlayerPrefs.GetInt("currentScore", 0);
        currentScoreView.UpdateScore(currentScore);
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        bestScoreView.UpdateScore(bestScore);
    }

    public void Restart()
    {
        currentScore = 0;
        currentScoreView.UpdateScore(currentScore);
    }
}

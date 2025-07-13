using UnityEngine;

public class ScoreDataController : MonoBehaviour
{
    [SerializeField]
    private ScoreView currentScoreView;
    [SerializeField]
    private ScoreView bestScoreView;

    [SerializeField]
    private GameObject scoreControllerObject;
    private IScoreChangeTrigger scoreChangeTrigger;

    private int currentScore;
    private int bestScore;

    private void Awake()
    {
        scoreChangeTrigger = scoreControllerObject.GetComponent<IScoreChangeTrigger>();
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        bestScoreView.UpdateScore(bestScore);
    }

    private void OnEnable()
    {
        scoreChangeTrigger.OnIncrementScore += ShowIncrement;
        scoreChangeTrigger.OnUpdateScore += UpdateScore;
    }

    private void OnDisable()
    {
        scoreChangeTrigger.OnIncrementScore -= ShowIncrement;
        scoreChangeTrigger.OnUpdateScore -= UpdateScore;
    }

    private void UpdateScore(int score)
    {
        currentScore = score;
        currentScoreView.UpdateScore(score);

        if (currentScore >= bestScore)
        {
            bestScore = currentScore;
            bestScoreView.UpdateScore(score);
        }
    }

    private void ShowIncrement(int incrementScore)
    {
        currentScoreView.ShowIncrement(incrementScore);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetInt("bestScore", bestScore);
            PlayerPrefs.SetInt("currentScore", currentScore);
            PlayerPrefs.Save();
        }
    }
}

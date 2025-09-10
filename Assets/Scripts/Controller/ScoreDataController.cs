using UnityEngine;

public class ScoreDataController : MonoBehaviour
{
    private TouchManager touchManager;

    [SerializeField]
    private BoardController boardController;

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

        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += SaveState;

        boardController.OnIncrementScore += ShowIncrement;
        boardController.OnUpdateScore += UpdateScore;
    }

    private void OnDisable()
    {
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;

        boardController.OnIncrementScore -= ShowIncrement;
        boardController.OnUpdateScore -= UpdateScore;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
        }
    }

    private void Start()
    {
        LoadState();
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
        PlayerPrefs.Save();
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

using UnityEngine;

public class ScoreDataController : MonoBehaviour
{
    protected TouchManager touchManager;

    [SerializeField]
    private BoardController boardController;

    [SerializeField]
    private ScoreView currentScoreView;
    [SerializeField]
    private ScoreView bestScoreView;

    private int currentScore;
    private int bestScore;

    protected virtual void Awake()
    {
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        bestScoreView.UpdateScore(bestScore);

        touchManager = TouchManager.Instance;
    }

    protected virtual void OnEnable()
    {
        touchManager.OnRestart += Restart;
        touchManager.OnCancel += SaveState;

        boardController.OnIncrementScore += ShowIncrement;
        boardController.OnUpdateScore += UpdateScore;
    }

    protected virtual void OnDisable()
    {
        touchManager.OnRestart -= Restart;
        touchManager.OnCancel -= SaveState;

        boardController.OnIncrementScore -= ShowIncrement;
        boardController.OnUpdateScore -= UpdateScore;
    }

    protected virtual void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveState();
        }
    }

    protected virtual void Start()
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

    protected virtual void Restart()
    {
        currentScore = 0;
        currentScoreView.UpdateScore(currentScore);
    }
}

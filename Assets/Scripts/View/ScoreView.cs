using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreTextPanel;

    [SerializeField]
    private GameObject incrementTextPanelPrefab;

    [SerializeField]
    private GameObject scoreControllerObject;
    private IScoreChangeTrigger scoreChangeTrigger;

    private void Awake()
    {
        scoreChangeTrigger = scoreControllerObject.GetComponent<IScoreChangeTrigger>();
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

    private void ShowIncrement(int score)
    {
        GameObject incrementTextPanel = Instantiate(incrementTextPanelPrefab, transform);
        var textObject = incrementTextPanel.GetComponent<TextMeshProUGUI>();
        textObject.text = "+" + score;
        StartCoroutine(MoveFadeOut(textObject, new Vector3(0, 20, 0), 1f));
    }

    private void UpdateScore(int score)
    {
        string scoreText = score.ToString();
        if (score > 10_000)
        {
            float value = score / 1000f;
            scoreText = value.ToString("F2") + "K";
        }
        scoreTextPanel.text = scoreText;
    }

    private IEnumerator MoveFadeOut(TextMeshProUGUI textObject, Vector3 target, float duration)
    {
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        Vector3 start = rectTransform.anchoredPosition;
        Color color = textObject.color;
        float alpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector3.Lerp(start, target, t);
            color.a = Mathf.Lerp(alpha, 0, t);
            textObject.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = target;
        textObject.color = new Color(color.r, color.g, color.b, 0);

        yield return new WaitForSeconds(0.5f);

        Destroy(textObject.gameObject);
    }
}

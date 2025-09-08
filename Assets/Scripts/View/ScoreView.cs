using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreTextPanel;

    [SerializeField]
    private GameObject incrementTextPanelPrefab;

    

    public void ShowIncrement(int score)
    {
        GameObject incrementTextPanel = Instantiate(incrementTextPanelPrefab, transform);
        var textObject = incrementTextPanel.GetComponent<TextMeshProUGUI>();
        textObject.text = (score > 0 ? "+" : "") + score;
        //StartCoroutine(MoveFadeOut(textObject, new Vector3(0, 20, 0), 1f));
        Vector3 position = incrementTextPanel.transform.position;
        Vector3 target = new Vector3(position.x, position.y + 20, position.z);
        StartCoroutine(Utils.Instance.LerpPosition(incrementTextPanel.transform, target, 1f));
        StartCoroutine(Utils.Instance.ChainDestroy(Utils.Instance.FadeTextOut(textObject, 1f), incrementTextPanel));
    }

    public void UpdateScore(int score)
    {
        string scoreText = score.ToString();
        if (score > 10_000)
        {
            float value = score / 1000f;
            scoreText = value.ToString("F2") + "K";
        } else if (score > 1_000_000)
        {
            float value = score / 1000000;
            scoreText = value.ToString("F2") + "M";
        }
        scoreTextPanel.text = scoreText;
    }
}

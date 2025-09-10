using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinView : MonoBehaviour
{
    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private TextMeshProUGUI winPanelText;

    Coroutine textMoveFadeCoroutine = null;

    Vector3 textOriginalPosition;

    private void Awake()
    {
        textOriginalPosition = winPanelText.transform.position;
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        if (textMoveFadeCoroutine != null)
        {
            StopCoroutine(textMoveFadeCoroutine);
        }
        textMoveFadeCoroutine = StartCoroutine(FadeOutWinText());
    }

    public void HideWinPanel()
    {
        if (textMoveFadeCoroutine != null)
        {
            StopCoroutine(textMoveFadeCoroutine);
            textMoveFadeCoroutine = null;
        }
        winPanel?.SetActive(false);
    }

    private IEnumerator FadeOutWinText()
    {
        winPanelText.transform.position = textOriginalPosition;
        Color color = winPanelText.color;
        color.a = 1f;
        winPanelText.color = color;
        Vector3 target = new Vector3(textOriginalPosition.x, textOriginalPosition.y + 100, textOriginalPosition.z);
        yield return StartCoroutine(Utils.Instance.LerpPosition(winPanelText.transform, target, 3f, Utils.Instance.FadeTextOut(winPanelText, 5f)));
        winPanel.SetActive(false);
        textMoveFadeCoroutine = null;
    }
}
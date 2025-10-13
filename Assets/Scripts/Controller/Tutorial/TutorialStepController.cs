using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepController : MonoBehaviour
{
    public GameObject panel;
    public StepRequirement requirement;

    public Vector2Int direction;
    
    public int currentPlayer;
    public int nextPlayer;

    public bool spawnTile;
    public Vector2Int location;
    public int tileValue;
    public int playerOwner;

    public bool highlightRegion;
    public Vector2 highlightStart;
    public Vector2 highlightEnd;

    public Material maskPanel;

    public void Activate()
    {
        panel.SetActive(true);
        if (highlightRegion)
        {
            StartCoroutine(Highlight());
        }
    }

    private IEnumerator Highlight()
    {
        yield return null;
        //maskPanel = GetComponentInChildren<RectMask2D>().gameObject.GetComponent<RectTransform>();

        /*
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskPanel.parent as RectTransform,
            Utils.Instance.WorldToScreenPoint(highlightStart),
            null,
            out localPos
        );
        Debug.Log(localPos);
        maskPanel.anchoredPosition = localPos;
        */

        Vector2 localPos = Utils.Instance.WorldToUIPoint((highlightStart + highlightEnd) / 2);
        maskPanel.SetVector("_MaskPos", new Vector4(localPos.x, localPos.y, 0, 0));
        float size = 100f / Screen.width;
        float x = 1 + (highlightEnd.x - highlightStart.x);
        float y = 1 + (highlightEnd.y - highlightStart.y);
        maskPanel.SetVector("_MaskSize", new Vector4(size * x, size / 1.75f * y, 0, 0));
    }

    public void Deactivate()
    {
        panel.SetActive(false);
    }

    public bool Complete(CompletionData completionData)
    {
        if (requirement != completionData.stepRequirement) return false;
        if (requirement == StepRequirement.None || requirement == StepRequirement.Tap) return true;
        if (completionData.direction == direction) return true;

        return false;
    }
}

public struct CompletionData
{
    public StepRequirement stepRequirement;
    public Vector2Int direction;

    public CompletionData(StepRequirement stepRequirement, Vector2Int direction)
    {
        this.stepRequirement = stepRequirement;
        this.direction = direction;
    }

    public CompletionData(StepRequirement step)
    {
        stepRequirement = step;
        direction = new Vector2Int(0, 0);
    }
}

public enum StepRequirement
{
    None,
    Tap,
    Swipe,
    AI
}
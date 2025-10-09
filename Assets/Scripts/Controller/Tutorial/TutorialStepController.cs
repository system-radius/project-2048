using UnityEngine;

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

    public void Activate()
    {
        panel.SetActive(true);
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
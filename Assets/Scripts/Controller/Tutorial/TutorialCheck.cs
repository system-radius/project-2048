using UnityEngine;

public class TutorialCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject versusButton;

    [SerializeField]
    private GameObject tutorialButton;

    private void Awake()
    {
        bool tutorialDone = PlayerPrefs.GetInt("tutorialDone", 0) == 1;
        versusButton.SetActive(tutorialDone);
        tutorialButton.SetActive(!tutorialDone);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDisplay : MonoBehaviour
{
    [SerializeField]
    private ButtonTrigger triggerButton;

    [SerializeField]
    private ButtonTrigger backButton;

    [SerializeField]
    private List<GameObject> showObjects = new List<GameObject>();

    [SerializeField]
    private List<GameObject> hideObjects = new List<GameObject>();

    private void OnEnable()
    {
        Initialize();
        if (triggerButton != null)
        {
            triggerButton.OnButtonPress += ShowPanel;
        }
        if (backButton != null)
        {
            backButton.OnButtonPress += HidePanel;
        }
    }

    private void OnDisable()
    {
        Deactivate();
        if (triggerButton != null)
        {
            triggerButton.OnButtonPress -= ShowPanel;
        }
        if (backButton != null)
        {
            backButton.OnButtonPress -= HidePanel;
        }
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Deactivate()
    {

    }

    protected void ShowPanel()
    {
        StartCoroutine(Toggle(true));
    }

    protected void HidePanel()
    {
        StartCoroutine(Toggle(false));
    }

    private IEnumerator Toggle(bool toggle)
    {
        yield return null;
        foreach (GameObject gameObject in showObjects)
        {
            gameObject.SetActive(toggle);
        }
        foreach (GameObject gameObject in hideObjects)
        {
            gameObject.SetActive(!toggle);
        }
    }

}
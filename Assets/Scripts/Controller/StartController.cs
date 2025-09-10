using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{

    [SerializeField]
    private GameObject startPanel;

    [SerializeField]
    private List<GameObject> activateOnNormal = new List<GameObject>();

    private TouchManager touchManager;

    private bool ongoingGame = false;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnPlay += StartNormal;
        touchManager.OnRestart += StartVersus;
        touchManager.OnCancel += DisplayStart;
    }

    private void OnDisable()
    {
        touchManager.OnPlay -= StartNormal;
        touchManager.OnRestart -= StartVersus;
        touchManager.OnCancel -= DisplayStart;
    }

    private void StartNormal()
    {
        //scoreDataController.gameObject.SetActive(true);
        //gameController.gameObject.SetActive(true);
        if (ongoingGame) return;
        StartCoroutine(StartGame(activateOnNormal));
    }

    private void StartVersus()
    {
        if (ongoingGame) return;
        Debug.Log("Starting versus game!");
    }

    public void DisplayStart()
    {
        StartCoroutine(EndGame(activateOnNormal));
    }

    private IEnumerator ActivateObjects(List<GameObject> objectsList, bool status = true)
    {
        yield return null;
        foreach (GameObject go in objectsList)
        {
            go.SetActive(status);
        }
    }

    private IEnumerator StartGame(List<GameObject> objectsList)
    {
        yield return StartCoroutine(ActivateObjects(objectsList));
        startPanel.SetActive(false);
        ongoingGame = true;
    }

    private IEnumerator EndGame(List<GameObject> objectsList)
    {
        yield return StartCoroutine(ActivateObjects(objectsList, false));
        startPanel.SetActive(true);
        ongoingGame = false;
    }

}

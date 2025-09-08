using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{
    /*
    [SerializeField]
    private ScoreDataController scoreDataController;

    [SerializeField]
    private GameController gameController;
    /**/

    [SerializeField]
    private GameObject startPanel;

    [SerializeField]
    private List<GameObject> activateOnNormal = new List<GameObject>();

    private TouchManager touchManager;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnPlay += StartNormal;
        touchManager.OnRestart += StartVersus;
    }

    private void OnDisable()
    {
        touchManager.OnPlay -= StartNormal;
        touchManager.OnRestart -= StartVersus;
    }

    private void StartNormal()
    {
        //scoreDataController.gameObject.SetActive(true);
        //gameController.gameObject.SetActive(true);

        StartCoroutine(StartGame(activateOnNormal));
    }

    private void StartVersus()
    {
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
    }

    private IEnumerator EndGame(List<GameObject> objectsList)
    {
        yield return StartCoroutine(ActivateObjects(objectsList, false));
        startPanel.SetActive(true);
    }

}

using UnityEngine;

public class StartController : MonoBehaviour
{
    [SerializeField]
    private ScoreDataController scoreDataController;

    [SerializeField]
    private GameController gameController;

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

    }

    private void StartVersus()
    {

    }

}

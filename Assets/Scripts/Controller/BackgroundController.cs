using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    protected Configuration config;

    [SerializeField]
    protected CameraScaleController worldCamera;

    [SerializeField]
    private GameObject blankPrefab;

    [SerializeField]
    private GameObject backgroundPrefab;

    private GameObject container;

    private int sizeX = 4;
    private int sizeY = 4;

    protected virtual void OnEnable()
    {
        sizeX = config.size.x;
        sizeY = config.size.y;
        StartCoroutine(StartBackground());
    }

    protected virtual void OnDisable()
    {
        Destroy(container);
        container = null;
    }

    private IEnumerator StartBackground()
    {
        yield return null;
        container = new GameObject("Background");
        GameObject background = Instantiate(backgroundPrefab, container.transform);
        float multiplier = 0.5f;
        background.transform.position = new Vector3((sizeX - 1) * multiplier, (sizeY - 1) * multiplier, 2);
        multiplier = 1.6f;
        background.transform.localScale = new Vector3(sizeX * multiplier, sizeY * multiplier, 0);
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GameObject tile = Instantiate(blankPrefab, container.transform);
                tile.transform.position = new Vector3(i, j, 1);
            }
        }

        worldCamera.AdjustCameraSize(container.transform);
    }
}
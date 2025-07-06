using UnityEngine;

[DefaultExecutionOrder(-10)]
public class Utils : Singleton<Utils>
{
    [SerializeField]
    private Camera worldCamera;

    public Vector3 ScreenToWorldPoint(Vector3 point)
    {
        point.z = worldCamera.nearClipPlane;
        return worldCamera.ScreenToWorldPoint(point);
    }
}
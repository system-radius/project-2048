using UnityEngine;

public class CameraScaleController : MonoBehaviour
{
    [SerializeField]
    private Camera worldCamera;

    [SerializeField]
    private float targetAspectX = 9f;
    [SerializeField]
    private float targetAspectY = 16f;

    [SerializeField]
    private float baseOrthographicSize = 5f;
    private float maxOrthoSize = 5f;

    [SerializeField]
    private float padding = 2f;

    private Vector2 minBounds = Vector2.zero;
    private Vector2 maxBounds = Vector2.zero;

    private float halfHeight = 5f;
    private float halfWidth = 5f;

    public void AdjustCameraSize(Transform parentTransform)
    {
        float targetRatio = targetAspectX / targetAspectY;
        float screenAspectRatio = (float)Screen.width / Screen.height;

        Bounds bounds = new Bounds(parentTransform.GetChild(0).position, Vector3.zero);
        foreach (Transform t in parentTransform)
        {
            bounds.Encapsulate(t.position);
        }
        bounds.Expand(padding);

        transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
        float width = bounds.size.x / (2f * screenAspectRatio);
        float height = bounds.size.y / 2f;

        worldCamera.orthographicSize = Mathf.Max(width, height);
        worldCamera.orthographicSize += padding;
        maxOrthoSize = worldCamera.orthographicSize;

        minBounds = bounds.min;
        maxBounds = bounds.max;

        halfHeight = (maxOrthoSize - padding);
        halfWidth = worldCamera.aspect * (maxOrthoSize - padding);
        //halfWidth = worldCamera.aspect * halfHeight;

        //Debug.LogError("Center: " + bounds.center + ", min: " + minBounds.ToString() + ", max: " + maxBounds + ", halfHeight: " + halfHeight + ", halfWidth: " + halfWidth);
    }
}

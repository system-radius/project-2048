using System.Collections;
using TMPro;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private TextMeshPro tmpPro;

    private Utils utils;

    private static Color32[] colors =
    {
        HexToColor32("#CBC2B3"), // 0
        HexToColor32("#EEEBDB"), // 2
        HexToColor32("#ECE0C8"), // 4
        HexToColor32("#EFB27E"), // 8
        HexToColor32("#F39768"), // 16
        HexToColor32("#F37D63"), // 32
        HexToColor32("#F46042"), // 64
        HexToColor32("#EACF76"), // 128
        HexToColor32("#EDCB67"), // 256
        HexToColor32("#ECC85A"), // 512
        HexToColor32("#E7C257"), // 1024
        HexToColor32("#E8BE4E"), // 2048
        HexToColor32("#3C3A33"), // 2048+
    };

    private static float[] fontSizes =
    {
        5, 5, 5, 5, 5, 5, 5, 3.5f, 3.5f, 3.5f, 2.5f, 2.5f, 2.5f
    };

    private static Color32 HexToColor32(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return (Color32)color;
        }

        return (Color32)Color.magenta;
    }

    private void Awake()
    {
        utils = Utils.Instance;
    }

    public void ChangeValue(int value, float multiplier = 1.2f, bool pulseSize = false)
    {
        int index = Mathf.RoundToInt(Mathf.Log(value) / Mathf.Log(2));

        if (index >= colors.Length) index = colors.Length - 1;
        spriteRenderer.color = colors[index];

        tmpPro.fontSize = fontSizes[index];
        tmpPro.text = value.ToString();

        if (pulseSize)
        {
            Vector3 scale = new Vector3(multiplier, multiplier, 1f);
            // First lerp size modification of the tile, then lerp back to the normal size.
            StartCoroutine(utils.LerpScale(transform, scale, 0.1f, utils.LerpScale(transform, Vector3.one, 0.1f)));
        } else
        {
            StartCoroutine(utils.LerpScale(transform, Vector3.one, 0.1f));
        }
    }

    public void MoveTo(int x, int y)
    {
        StartCoroutine(utils.LerpPosition(transform, new Vector3(x, y, transform.position.z), 0.1f));
    }

    public IEnumerator Merge(int x, int y)
    {
        tmpPro.text = string.Empty;
        yield return StartCoroutine(utils.LerpPosition(transform, new Vector3(x, y, transform.position.z), 0.05f));
        Destroy(gameObject);
    }

    public IEnumerator Delete()
    {
        yield return StartCoroutine(utils.LerpScale(transform, new Vector3(0.2f, 0.2f, 1f), 0.1f));
        Destroy(gameObject);
    }
}
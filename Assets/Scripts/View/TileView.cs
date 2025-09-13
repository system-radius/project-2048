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

    private Color32[][] colors = new Color32[][]
    {
        new Color32[] {
            Utils.HexToColor32("#CBC2B3"), // 0
            Utils.HexToColor32("#EEEBDB"), // 2
            Utils.HexToColor32("#ECE0C8"), // 4
            Utils.HexToColor32("#EFB27E"), // 8
            Utils.HexToColor32("#F39768"), // 16
            Utils.HexToColor32("#F37D63"), // 32
            Utils.HexToColor32("#F46042"), // 64
            Utils.HexToColor32("#EACF76"), // 128
            Utils.HexToColor32("#EDCB67"), // 256
            Utils.HexToColor32("#ECC85A"), // 512
            Utils.HexToColor32("#E7C257"), // 1024
            Utils.HexToColor32("#E8BE4E"), // 2048
            Utils.HexToColor32("#3C3A33") // 2048+
        },
        new Color32[] {
            Utils.HexToColor32("#000000"), // 0
            Utils.HexToColor32("#FFD3D3"), // 2
            Utils.HexToColor32("#FF6C6C"), // 4
            Utils.HexToColor32("#FF0000"), // 8
            Utils.HexToColor32("#FF0000"), // 16
            Utils.HexToColor32("#FF0000"), // 32
            Utils.HexToColor32("#BF0000"), // 64 +
        },
        new Color32[] {
            Utils.HexToColor32("#000000"), // 0
            Utils.HexToColor32("#D3D3FF"), // 2
            Utils.HexToColor32("#6C6CFF"), // 4
            Utils.HexToColor32("#0000FF"), // 8
            Utils.HexToColor32("#0000FF"), // 16
            Utils.HexToColor32("#0000FF"), // 32
            Utils.HexToColor32("#0000BF"), // 64 +
        }
    };

    private static float[] fontSizes =
    {
        5, 5, 5, 5, 5, 5, 5, 3.5f, 3.5f, 3.5f, 2.5f, 2.5f, 2.5f
    };

    private void Awake()
    {
        utils = Utils.Instance;
    }

    public void ChangeValue(int value, int playerId, float multiplier = 1.2f, bool pulseSize = false)
    {
        int index = Mathf.RoundToInt(Mathf.Log(value) / Mathf.Log(2));
        int colorIndex = index;
        Color32[] array = colors[playerId];
        if (colorIndex >= array.Length) colorIndex = array.Length - 1;
        //Debug.Log("[" + playerId + "] Index: " + index);
        spriteRenderer.color = array[colorIndex];

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
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ValueTileController))]
public class ValueTileView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private TextMeshPro tmpPro;

    private ValueTileController tileController;

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

    private void Awake()
    {
        tileController = GetComponent<ValueTileController>();
    }

    private void OnEnable()
    {
        tileController.OnValueChange += ChangeValue;
    }

    private void OnDisable()
    {
        tileController.OnValueChange -= ChangeValue;
    }

    private void ChangeValue(int value)
    {
        int index = Mathf.RoundToInt(Mathf.Log(value) / Mathf.Log(2));

        if (index >= colors.Length) index = colors.Length - 1;
        spriteRenderer.color = colors[index];

        tmpPro.fontSize = fontSizes[index];
        tmpPro.text = value.ToString();
    }

    private static Color32 HexToColor32(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return (Color32) color;
        }

        return (Color32) Color.magenta;
    }
}
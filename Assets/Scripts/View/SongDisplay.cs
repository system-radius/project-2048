using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class SongDisplay : Singleton<SongDisplay>
{
    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private TextMeshProUGUI contentPrefab;

    private string RetrieveTimeText(float time)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append((int)(time / 60));
        sb.Append(':');
        sb.Append((int)(time % 60));

        return sb.ToString();
    }

    public void AddMessage(string text, float duration = 0f)
    {
        TextMeshProUGUI msg = Instantiate(contentPrefab, content);
        //msg.transform.SetAsFirstSibling();
        msg.name = text;
        msg.text = "[" + RetrieveTimeText(duration) + "]" + text;
        //timerLabel.SetValue(duration);

        duration = 3f;
        StartCoroutine(Utils.Instance.FadeTextOut(msg, duration));
        //StartCoroutine(Utils.Instance.FadeTextOut(msg, 3f));
        StartCoroutine(DestroyObject(msg.gameObject, duration));
    }

    private IEnumerator DestroyObject(GameObject gameObject, float duration)
    {
        yield return new WaitForSeconds(duration + 1);
        Destroy(gameObject);
    }
}
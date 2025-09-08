using System;
using System.Collections;
using UnityEngine;
using TMPro;

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

    public IEnumerator LerpTransform(Transform transform, Vector3 target, float duration, Func<Transform, Vector3> getter, Action<Transform, Vector3> setter, IEnumerator chainedCoroutine = null)
    {
        Vector3 start = getter(transform);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            setter(transform, Vector3.Lerp(start, target, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        setter(transform, target);
        if (chainedCoroutine != null) yield return StartCoroutine(chainedCoroutine);
    }

    public IEnumerator LerpPosition(Transform transform, Vector3 target, float duration, IEnumerator chainedCoroutine = null)
    {
        return LerpTransform(transform, target, duration, t => t.position, (t, value) => t.position = value, chainedCoroutine);
    }

    public IEnumerator LerpScale(Transform transform, Vector3 target, float duration, IEnumerator chainedCoroutine = null)
    {
        return LerpTransform(transform, target, duration, t => t.localScale, (t, value) => t.localScale = value, chainedCoroutine);
    }

    public IEnumerator LerpTextFade(TextMeshProUGUI textObject, float duration, float fadeTarget, IEnumerator chainedCoroutine = null)
    {
        Color color = textObject.color;
        float alpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            color.a = Mathf.Lerp(alpha, fadeTarget, t);
            textObject.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (chainedCoroutine != null)
        {
            yield return StartCoroutine(chainedCoroutine);
        }
    }

    public IEnumerator FadeTextIn(TextMeshProUGUI textObject, float duration)
    {
        Color color = textObject.color;
        color.a = 0f;
        return (LerpTextFade(textObject, duration, 1f));
    }

    public IEnumerator FadeTextOut(TextMeshProUGUI textObject, float duration)
    {
        Color color = textObject.color;
        color.a = 1f;
        return (LerpTextFade(textObject, duration, 0f));
    }

    public IEnumerator ChainDestroy(IEnumerator coroutine, GameObject gameObject)
    {
        yield return StartCoroutine(coroutine);
        Destroy(gameObject);
    }
}
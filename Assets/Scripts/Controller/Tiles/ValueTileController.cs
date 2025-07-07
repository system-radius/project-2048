using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

public class ValueTileController : TileController, IValueChangeTrigger
{
    public event Action<int> OnValueChange;

    public void Initialize(int value)
    {
        if (tile == null)
        {
            tile = new ValueTile();
        }

        OnValueChange?.Invoke(value);
        if (value > 2)
        {
            IncrementValue();
        }

        transform.localScale = Vector3.zero;
        StartCoroutine(LerpData(Vector3.one, 0.1f, t => t.localScale, (t, value) => t.localScale = value));
    }

    public void IncrementValue()
    {
        StartCoroutine(LerpData(new Vector3(1.2f, 1.2f, 1f), 0.05f, t => t.localScale, (t, value) => t.localScale = value,
            LerpData(Vector3.one, 0.05f, t => t.localScale, (t, value) => t.localScale = value)));

        ((ValueTile) tile).IncreaseValue();
        OnValueChange?.Invoke(tile.Value);
    }

    public void MoveTo(int x, int y)
    {
        StartCoroutine(LerpData(new Vector3(x, y, transform.position.z), 0.1f, t => t.position, (t, value) => t.position = value));
    }

    private IEnumerator LerpData(Vector3 target, float duration, Func<Transform, Vector3> getter, Action<Transform, Vector3> setter, IEnumerator chainedCoroutine = null)
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
        if (chainedCoroutine != null)
        {
            yield return StartCoroutine(chainedCoroutine);
        }
    }

    public bool AttemptMerge(ValueTileController that)
    {
        if (this.tile.Value != that.tile.Value) return false;

        Destroy(that.gameObject);
        this.IncrementValue();
        return true;
    }
}
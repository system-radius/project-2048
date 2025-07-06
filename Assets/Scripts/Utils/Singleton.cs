using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance { get
        {
            if (_instance == null)
            {
                var objs = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
                if (objs == null)
                {
                    // No instances found for the specified type.
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
                else
                {
                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                    }

                    if (objs.Length > 1)
                    {
                        Debug.LogError("Single instance required, found multiple instances for type: " + typeof(T).Name + "!");
                    }
                }

            }

            return _instance;
        }
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Settings/Config")]
public class Configuration : ScriptableObject
{
    public Vector2Int size;

    public int winCondition;

    public int players;
}
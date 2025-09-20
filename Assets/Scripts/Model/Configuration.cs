using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Settings/Config")]
public class Configuration : ScriptableObject
{
    public AudioClip bgm;

    public Vector2Int size;

    public int winCondition;

    public int players;

    public List<PlayerType> playerTypes = new();
}
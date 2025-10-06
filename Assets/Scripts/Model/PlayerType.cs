using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerType", menuName = "Player/Type")]
public class PlayerType : ScriptableObject
{
    public List<AudioClip> audioClips;

    public Level level;
}
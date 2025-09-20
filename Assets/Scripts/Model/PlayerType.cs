using UnityEngine;

[CreateAssetMenu(fileName = "PlayerType", menuName = "Player/Type")]
public class PlayerType : ScriptableObject
{
    public AudioClip clip;

    public Level level;
}
using UnityEngine;

[CreateAssetMenu(fileName = "SFXEntry", menuName = "Audio/SFX Entry")]
public class SFXEntry : ScriptableObject
{
    public SFXID id = SFXID.ButtonClick;
    public AudioClip clip;
    public SFXGroup group = SFXGroup.UI;

    [Range(0f, 1f)]
    [Tooltip("Volume multiplier for balancing this SFX (0 = mute, 1 = full volume).")]
    public float volumeBalance = 1f;

    [Tooltip("Number of overlap instances can be play at a time (0 = no limit).")]
    public int overlapAllowed = 0;
}
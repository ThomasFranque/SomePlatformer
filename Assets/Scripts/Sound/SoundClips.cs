using UnityEngine;

[CreateAssetMenu(menuName = "Sound/Sound Clips")]
public class SoundClips : ScriptableObject
{
    [SerializeField] private AudioClip[] _clips = null;

    public AudioClip this[int index]
    {
        get
        {
            if (index < 0 && index >= _clips.Length)
                Debug.LogError("The clip index is out of bounds.");

            return _clips[index];
        }
    }
}

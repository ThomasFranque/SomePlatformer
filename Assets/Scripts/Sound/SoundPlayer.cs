using UnityEngine;
using Sound;

public class SoundPlayer
{
    [SerializeField] private SoundClips _clips;

    private SoundMngr _soundMngr;

    public SoundPlayer()
    {
        _soundMngr = SoundMngr.Instance;
    }

    public void PlayOneShotLocalized(AudioClip clip, Vector3 sourcePos)
    {
        SoundMngr.Instance?.PlayOneShotLocalized(clip, sourcePos);
    }

    public void PlayOneShotLocalized(AudioClip clip, AudioSource audioSrc)
    {
        SoundMngr.Instance?.PlayOneShotLocalized(clip, audioSrc);
    }

    public void PlayOneShotGeneral(AudioClip clip)
    {
        SoundMngr.Instance?.PlayOneShotGeneral(clip);
    }
}
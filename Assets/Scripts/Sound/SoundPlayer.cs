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

    public void PlayGeneralSound(AudioClip clip, float volume = 1.0f, bool looped = false)
    {
        SoundMngr.Instance?.PlaySoundGeneral(clip, volume, looped);
    }

    public void StopGeneralSound()
    {
        SoundMngr.Instance?.StopGeneralSound();
    }
    public void PauseGeneralSound()
    {
        SoundMngr.Instance?.PauseGeneralSound();
    }
}
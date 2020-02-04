using System.Collections;
using System.Collections.Generic;
using Sound;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private const float _STEP_SOUND_DELAY = 0.4f;
    [SerializeField] private AudioClip _walkSound = null;
    [SerializeField] private AudioClip _jumpSound = null;
    [SerializeField] private AudioClip _wallJumpSound = null;
    [SerializeField] private AudioClip _dashSound = null;

    private float _timeOfLastStepSound;

    private AudioSource _audioSource;

    public float ElapsedTime(float timeToCheck) => Time.time - timeToCheck;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _timeOfLastStepSound = -900;
    }

    private void PlayOneShot(AudioClip clip)
    {
        SoundMngr.Instance?.PlayOneShotLocalized(clip, _audioSource);
    }

    public void WhileWalkInputPressed()
    {
        if (ElapsedTime(_timeOfLastStepSound) >= _STEP_SOUND_DELAY)
        {
            PlayOneShot(_walkSound);
            _timeOfLastStepSound = Time.time;
        }
    }

    public void WhileNotWalkInputPressed()
    {
        _timeOfLastStepSound = Time.time - _STEP_SOUND_DELAY;
    }

    public void PlayJumpSound()
    {
        SoundMngr.Instance?.PlayOneShotLocalized(_jumpSound, _audioSource);
    }

    public void PlayWallJumpSound()
    {
        SoundMngr.Instance?.PlayOneShotLocalized(_wallJumpSound, _audioSource);
    }

    public void PlayDashSound()
    {
        SoundMngr.Instance?.PlayOneShotLocalized(_dashSound, _audioSource);
    }
}

using UnityEngine;

namespace Sound
{
    public class SoundMngr : MonoBehaviour
    {
        [SerializeField] private GameObject _audioMngrPrefab;
        public const string AUDIO_MNGR_OBJ_NAME = "Audio Manager";

        public static SoundMngr Instance { get; private set; }

        [SerializeField]
        private AudioSource _localizedSource = null;
        [SerializeField]
        private AudioSource _generalAudioSource = null;
        private float _volume;

        private void Awake()
        {
            Instance = this;
            name = AUDIO_MNGR_OBJ_NAME;
            _volume = 1.0f;
        }

        public void PlayOneShotLocalized(AudioClip clip, Vector3 sourcePos)
        {
            transform.position = sourcePos;
            PlayOneShotLocalized(clip, _localizedSource);
        }

        public void PlayOneShotLocalized(AudioClip clip, AudioSource audioSrc)
        {
            audioSrc.PlayOneShot(clip, _volume);
        }

        public void PlayOneShotGeneral(AudioClip clip)
        {
            _generalAudioSource.PlayOneShot(clip, _volume);
        }

        public void PlaySoundGeneral(AudioClip clip, float volume = 1.0f, bool loop = false)
        {
            StopGeneralSound();
            _generalAudioSource.clip = clip;
            _generalAudioSource.loop = loop;
            _generalAudioSource.volume = volume;
            _generalAudioSource.Play();
        }

        public void StopGeneralSound()
        {
            _generalAudioSource.Stop();
        }

        public void PauseGeneralSound()
        {
            _generalAudioSource.Pause();
        }
    }
}

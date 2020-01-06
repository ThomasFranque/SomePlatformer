using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraActions : MonoBehaviour
{
	public static CameraActions ActiveCamera { get; private set; }

	private CinemachineVirtualCamera _activeCam = null;
	private CinemachineBrain _cineBrain = null;

	public bool ActiveCamExists => _activeCam != null;
	// Shake 
	private CinemachineBasicMultiChannelPerlin _virtualCameraNoise = null;
	private float _shakeElapsedTime = 0.0f;
	private float _shakeAmplitude = 0.0f;
	private float _shakeFrequency = 0.0f;

	public bool IsShaking => _shakeElapsedTime > 0;
	//

	private void Awake()
	{
		ActiveCamera = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		_cineBrain = GetComponent<CinemachineBrain>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateShake();
	}

	private void UpdateActiveCam()
	{
		_activeCam = _cineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
	}

	private void UpdateShake()
	{
		if (_virtualCameraNoise != null)
			{
				// If Camera Shake effect is still playing
				if (IsShaking)
				{
					// Set Cinemachine Camera Noise parameters
					_virtualCameraNoise.m_AmplitudeGain = _shakeAmplitude;
					_virtualCameraNoise.m_FrequencyGain = _shakeFrequency;

					// Update Shake Timer
					_shakeElapsedTime -= Time.deltaTime;
				}
				else
				{
					// If Camera Shake effect is over, reset variables
					_virtualCameraNoise.m_AmplitudeGain = 0f;
					_shakeElapsedTime = 0f;
				} 
			}
	}


	public void Shake(float amplitude = 80.0f, float frequency = 60.0f, float duration = 0.2f)
	{
		UpdateActiveCam();

		if (ActiveCamExists)
		{
			_virtualCameraNoise = _activeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			_shakeElapsedTime = duration;
			_shakeAmplitude = amplitude;
			_shakeFrequency = frequency;
		}
	}
}

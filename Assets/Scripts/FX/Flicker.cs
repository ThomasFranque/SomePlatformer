using UnityEngine;

public class Flicker : MonoBehaviour
{
	private const float _DEFAULT_FREQUENCY = 0.1f;
	private const float _DEFAULT_STRENGTH = 12.0f;

	[SerializeField] private float _frequency = _DEFAULT_FREQUENCY;
	[SerializeField] private float _strength = _DEFAULT_STRENGTH;

	[SerializeField] private bool _active = false;

	Vector3 startPos;

	// Update is called once per frame
	void LateUpdate()
	{
		if (_active)
		{
			if (startPos == default)
				startPos = transform.localPosition;

			float zInc = -Mathf.Sign(Vector3.Dot(transform.forward, Vector3.forward)) * 2;
			transform.localPosition = startPos + _strength * Vector3.up * (Mathf.PerlinNoise(Time.time * _frequency, 0.0f) * 2 - 1) +
												 _strength * Vector3.right * (Mathf.PerlinNoise(0.0f, Time.time * _frequency * 1.1234567f) * 2 - 1) +
												 zInc * Vector3.forward;
		}
		else
			startPos = default;
	}

	public void SetActive(bool active)
	{
		_active = active;
	}
}
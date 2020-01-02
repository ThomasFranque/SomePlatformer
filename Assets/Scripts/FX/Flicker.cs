using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
	public float frequency = 0.1f;
	public float strength = 5.0f;

	public bool active;
	public bool forceActive = false;

	Vector3 startPos;

	// Start is called before the first frame update
	void Start()
	{
		if (!forceActive)
			active = false;
		else
			active = true;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (active)
		{
			if (startPos == new Vector3 (0,0,0))
				startPos = transform.localPosition;

			float zInc = -Mathf.Sign(Vector3.Dot(transform.forward, Vector3.forward)) * 2;
			transform.localPosition = startPos + strength * Vector3.up * (Mathf.PerlinNoise(Time.time * frequency, 0.0f) * 2 - 1) +
												 strength * Vector3.right * (Mathf.PerlinNoise(0.0f, Time.time * frequency * 1.1234567f) * 2 - 1) +
												 zInc * Vector3.forward;
		}
		else
			startPos = new Vector3 (0,0,0);
	}
}
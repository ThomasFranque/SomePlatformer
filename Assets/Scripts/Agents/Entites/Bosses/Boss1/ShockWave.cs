using UnityEngine;

public class ShockWave : Hazard
{
	[SerializeField] private float _speed = 2;
	[SerializeField] private float _speedIncreaseFactor = 2;
	[SerializeField] private float _sizeIncreaseFactor = 1.025f;

    // Update is called once per frame
    void Update()
    {
		MoveForward();
		DecreaseScale();
		if (transform.localScale.x <= 0) Destroy(gameObject);
	}

	private void DecreaseScale()
	{

		transform.localScale /= _sizeIncreaseFactor;
	}

	private void MoveForward()
	{
		transform.position += transform.right * _speed * Time.deltaTime;
		_speed += _speedIncreaseFactor * Time.deltaTime;

	}
}

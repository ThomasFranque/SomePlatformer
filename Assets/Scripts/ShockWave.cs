using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : Hazard
{
	[SerializeField] private float _speed = 2;
	[SerializeField] private float _speedIncreaseFactor = 2;

    // Update is called once per frame
    void Update()
    {
		transform.position += transform.right * _speed * Time.deltaTime;
		_speed += _speedIncreaseFactor * Time.deltaTime;
	}
}

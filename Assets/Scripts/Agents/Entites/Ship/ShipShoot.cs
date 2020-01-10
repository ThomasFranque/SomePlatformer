using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShoot : MonoBehaviour
{
	[SerializeField] private KeyCode _attackInput = KeyCode.L;
	[SerializeField] private KeyCode _specialAttackInput = KeyCode.Space;

	[SerializeField] private GameObject _bulletPrefab = null;
	[SerializeField] private Transform _shootPointRight = null;
	[SerializeField] private Transform _shootPointLeft = null;
	[SerializeField] private float _shotCooldown = 0.2f;

	private float _timeOfLastShot;
	private Vector3 _shootOffset;
	private bool trigger;
	private float LastShotElapsedTime => Time.time - _timeOfLastShot;

    // Start is called before the first frame update
    void Start()
    {
		_timeOfLastShot = -900;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(_attackInput) && LastShotElapsedTime > _shotCooldown)
			Shoot();
    }

	private void Shoot()
	{
		Instantiate(_bulletPrefab, trigger ? _shootPointRight.position : _shootPointLeft.position, transform.rotation);
		_timeOfLastShot = Time.time;
		trigger = !trigger;
	}
}

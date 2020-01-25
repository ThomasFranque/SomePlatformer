using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShoot : MonoBehaviour
{
	private const byte _MIN_SHOOTING_PARTICLES = 20;
	private const byte _MAX_SHOOTING_PARTICLES = 35;

	[SerializeField] private KeyCode _attackInput = KeyCode.L;
	[SerializeField] private KeyCode _specialAttackInput = KeyCode.Space;

	[SerializeField] private GameObject _bulletPrefab = null;
	[SerializeField] private Transform _shootPointRight = null;
	[SerializeField] private Transform _shootPointLeft = null;
	[SerializeField] private float _shotCooldown = 0.2f;
	[SerializeField] private ParticleSystem _shootingParticles;

	public bool IsShooting => LastShotElapsedTime < _shotCooldown;
	private ControledShip _thisCtrledShip; 

	private float _timeOfLastShot;
	private Vector3 _shootOffset;
	private bool trigger;
	private float LastShotElapsedTime => Time.time - _timeOfLastShot;
	
    private void Awake() 
	{
		_timeOfLastShot = -900;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(_attackInput) && !IsShooting)
			Shoot();
    }

	private void Shoot()
	{
		Vector3 shootingPos = trigger ? _shootPointRight.position : _shootPointLeft.position;
		Instantiate(_bulletPrefab, shootingPos, transform.rotation);
		CastShotParticles(shootingPos);
		_timeOfLastShot = Time.time;
		trigger = !trigger;
	}

	private void CastShotParticles(Vector3 shootingPos)
	{
		_shootingParticles.transform.position = shootingPos;
		_shootingParticles.Emit(Random.Range(_MIN_SHOOTING_PARTICLES, _MAX_SHOOTING_PARTICLES));
	}

	public void SetControlledShip(ControledShip ctrldShip)
	{
		_thisCtrledShip = ctrldShip;
	}
}

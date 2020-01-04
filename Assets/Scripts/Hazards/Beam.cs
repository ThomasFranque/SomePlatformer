using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
	private const float _RANGE = 66.0f;
	private const float _MIN_RANGE = 12.0f;

	private const float _LOOK_AT_PLAYER_TIME = 4.0f;
	private const float _CHARGE_TIME = 2.0f;
	private const float _BEAM_DURATION = 0.2f;
	private const float _TIME_WAITING_AFTER_SHOOTING = 2.0f;

	private const float _Y_PLAYER_OFFSET = 3.5f;

	[Header("Beam References")]
	[SerializeField] private GameObject _beamPrefab = null;
	[SerializeField] private ParticleSystem _beamChargingParticles = null;
	[SerializeField] private ParticleSystem _beamShootingParticles = null;
	[Header("Beam Variables")]
	[SerializeField] private float _followSpeed = 7.5f;
	[SerializeField] private float _slowLookAtSpeed = 5.0f;

	private Vector3 randomExitPoint;
	private Vector3 _playerOffset = new Vector3(0,_Y_PLAYER_OFFSET,0);

	private GameObject _beam = null;

	bool spotted;
	bool moveToPlayer = true;
	bool lookAt;
	bool charge;
	bool exit;

	private void Start()
	{
		randomExitPoint = new Vector3(transform.position.x + Random.Range(-390, 390), transform.position.y + 580, 0);
	}

	private void Update()
	{
		if (!spotted)
		{
			if (Vector3.Distance(Player.Instance.transform.position, transform.position) < _RANGE)
			{
				spotted = true;
				StartCoroutine(CLookAt());
			}
		} 
		else if (spotted && moveToPlayer && Vector3.Distance(Player.Instance.transform.position, transform.position) > _MIN_RANGE)
			MoveTowards(Player.Instance.transform.position + _playerOffset, _followSpeed);
		else if (exit)
		{
			MoveTowards(randomExitPoint, _followSpeed * 10);
			SoftLookAt(transform.position + new Vector3(0, -1,0), 1.4f);
		}

		if (charge)
			transform.localScale += new Vector3(0.1f * Time.deltaTime, 0.1f * Time.deltaTime, 0);

		if (lookAt)
			SoftLookAt(Player.Instance.transform.position + _playerOffset);
	}

	private IEnumerator CLookAt()
	{
		yield return new WaitForSeconds(_LOOK_AT_PLAYER_TIME);
		StartCoroutine(CCharge());
	}

	private IEnumerator CCharge()
	{
		_beamChargingParticles.enableEmission = true;
		lookAt = true;
		charge = true;
		moveToPlayer = false;
		yield return new WaitForSeconds(_CHARGE_TIME);
		charge = false;
		_beamChargingParticles.enableEmission = false;
		StartCoroutine(CBeamSpawn());
	}

	private IEnumerator CBeamSpawn()
	{
		lookAt = false;
		_beamShootingParticles.enableEmission = true;
		transform.localScale = new Vector3(1, 1, 1);
		_beam = Instantiate(_beamPrefab, transform.position, transform.rotation, transform);
		yield return new WaitForSeconds(_BEAM_DURATION);
		Destroy(_beam);
		_beamShootingParticles.enableEmission = false;
		StartCoroutine(CTimeAfterDone());
	}

	private IEnumerator CTimeAfterDone()
	{
		yield return new WaitForSeconds(_TIME_WAITING_AFTER_SHOOTING);
		exit = true;
	}

	private void MoveTowards(Vector3 p, float speed)
	{
		float step = speed * Time.deltaTime;

		// move sprite towards the target location
		transform.position = Vector2.MoveTowards(transform.position, p, step);
	}

	private void HardLookAt(Vector3 target)
	{
		Vector3 dir = target - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
	}

	private void SoftLookAt(Vector3 target, float lookMultiplier = 1.0f)
	{
		Vector3 dir = target - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		// The step size is equal to speed times frame time.
		float step = (_slowLookAtSpeed * lookMultiplier)* Time.deltaTime;

		// Rotate our transform a step closer to the target's.
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angle + 90, Vector3.forward), step);
	}
}

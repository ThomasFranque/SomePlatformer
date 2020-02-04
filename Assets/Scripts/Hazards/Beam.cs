using System.Collections;
using UnityEngine;

#pragma warning disable 0618 
public class Beam : Enemy
{
	private const float _RANGE = 54.0f;
	private const float _MIN_RANGE = 12.0f;
	private const float _GETTING_CLOSE_ADDED_RANGE = 20.0f;

	private const float _LOOK_AT_PLAYER_TIME = 4.0f;
	private const float _CHARGE_TIME = 2.0f;
	private const float _BEAM_DURATION = 0.2f;
	private const float _TIME_WAITING_AFTER_SHOOTING = 2.0f;

	private const float _Y_PLAYER_OFFSET = 3.5f;

	[Header("Beam References")]
	[SerializeField] private GameObject _beamPrefab = null;
	[SerializeField] private ParticleSystem _beamChargingParticles = null;
	[SerializeField] private ParticleSystem _beamShootingParticles = null;
	[SerializeField] private Animator _proximityIndAnim = null;
	[Header("Beam Variables")]
	[SerializeField] private float _followSpeed = 7.5f;
	[SerializeField] private float _slowLookAtSpeed = 5.0f;
	[SerializeField] private float _stompDisableTime = 2.5f;
	[SerializeField] private byte _maxInterruptedTimes = 5;

	private Vector3 randomExitPoint;
	private Vector3 _playerOffset = new Vector3(0,_Y_PLAYER_OFFSET,0);

	private GameObject _beam = null;

	private byte _timesInterrupted = 0;

	bool stomped;

	bool moveToPlayer = true;
	bool spotted;
	bool lookAt;
	bool charge;
	bool beamShot;
	bool exit;

	private Coroutine _behaviorCor = null;
	private Coroutine _stompCor = null;

	private Vector3 smoothPosition;
	private Vector3 desiredPosition;

	private bool CanBeInterrupted => _timesInterrupted < 5;

	private bool PlayerInRange => Vector3.Distance(Player.Instance.transform.position, transform.position) < _RANGE;
	private bool PlayerAlmostInRange => Vector3.Distance(Player.Instance.transform.position, transform.position) < _RANGE + _GETTING_CLOSE_ADDED_RANGE && !spotted;
	private bool PlayerTooClose => Vector3.Distance(Player.Instance.transform.position, transform.position) > _MIN_RANGE;

	protected override void Start()
	{
		base.Start();
		randomExitPoint = new Vector3(transform.position.x + Random.Range(-390, 390), transform.position.y + 580, 0);
	}

	protected override void Update()
	{
		base.Update();
		PerformBehavior();
	}

	#region Behavior
	private void PerformBehavior()
	{
		if (!stomped)
		{
			if (!spotted && PlayerInRange)
				_behaviorCor = StartCoroutine(CBehavior());
			else if (spotted && moveToPlayer && PlayerTooClose)
				SmoothMoveTowards(Player.Instance.transform.position + _playerOffset, _followSpeed);
			else if (exit)
			{
				MoveTowards(randomExitPoint, _followSpeed * 100);
				SoftLookAt(transform.position + new Vector3(0, -1, 0), 3.2f);
			}
			else if (PlayerAlmostInRange)
			{
				_proximityIndAnim.SetTrigger("Question");
			}
			else
			{
				_proximityIndAnim.SetTrigger("None");
			}

			if (charge)
				transform.localScale += new Vector3(0.1f * Time.deltaTime, 0.15f * Time.deltaTime, 0);

			if (lookAt)
				SoftLookAt(Player.Instance.transform.position + _playerOffset);

		}
		else
			WhileStomped();
	}

	private IEnumerator CBehavior()
	{
		OnSpotted();
		yield return new WaitForSeconds(_LOOK_AT_PLAYER_TIME);
		OnCharge();
		yield return new WaitForSeconds(_CHARGE_TIME);
		OnStartBeam();
		yield return new WaitForSeconds(_BEAM_DURATION);
		OnEndBeam();
		yield return new WaitForSeconds(_TIME_WAITING_AFTER_SHOOTING);
		OnExit();

		_behaviorCor = null;
	}

	private void OnSpotted()
	{
		spotted = true;
		_anim.SetTrigger("Spotted");
		_proximityIndAnim.SetTrigger("Warning");
	}

	private void OnCharge()
	{
		_proximityIndAnim.SetTrigger("None");
		_beamChargingParticles.enableEmission = true;
		lookAt = true;
		charge = true;
		moveToPlayer = false;
	}

	private void OnStartBeam()
	{
		beamShot = true;
		charge = false;
		_beamChargingParticles.enableEmission = false;

		lookAt = false;
		_beamShootingParticles.enableEmission = true;
		transform.localScale = new Vector3(1, 1, 1);
		_beam = Instantiate(_beamPrefab, transform.position, transform.rotation, transform);
		CameraActions.ActiveCamera.Shake(15, 10, 0.1f);

	}

	private void OnEndBeam()
	{
		Destroy(_beam);
		_beamShootingParticles.enableEmission = false;
	}

	private void OnExit()
	{
		exit = true;
	}

	private void ResetBehavior()
	{
		if (_stompCor != null)
		{
			StopCoroutine(_stompCor);
			_stompCor = null;
		}
		if (_behaviorCor != null)
		{
			StopCoroutine(_behaviorCor);
			_behaviorCor = null;
			_timesInterrupted++;
			transform.localScale = new Vector3(1, 1, 1);
		}


		_beamChargingParticles.enableEmission = false;
		_beamShootingParticles.enableEmission = false;
		moveToPlayer = true;
		spotted = false;
		lookAt = false;
		charge = false;
		exit = false;
	}

	private void WhileStomped()
	{
		SoftLookAt(transform.position + new Vector3(0, -1, 0), 3.2f);
	}

	private void MoveTowards(Vector3 p, float speed)
	{
		float step = speed * Time.deltaTime;

		// move sprite towards the target location
		transform.position = Vector2.MoveTowards(transform.position, p, step);

	}

	private void SmoothMoveTowards(Vector3 p, float speed)
	{
		// Preparing variable for Lerp
		desiredPosition = new Vector3(
			p.x,
			p.y + 13.5f,
			transform.position.z);

		// Using Lerp to pan the camera 
		smoothPosition =
			Vector3.Lerp(transform.position, desiredPosition,
			speed * Time.deltaTime);

		transform.position = smoothPosition;
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
	#endregion

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg) {Flash();}

	protected override void OnPlayerStomp(Player p)
	{
		if (!exit)
		{
			base.OnPlayerStomp(p);
			if (!beamShot && CanBeInterrupted)
			{
				ResetBehavior();
				_stompCor = StartCoroutine(CStomped());
			}
		}
	}

	private IEnumerator CStomped()
	{
		_proximityIndAnim.SetTrigger("Dots");
		_anim.SetTrigger("Stomped");
		SetInvulnerability(true);
		stomped = true;
		yield return new WaitForSeconds(_stompDisableTime);
		stomped = false;
		if (Vector3.Distance(Player.Instance.transform.position, transform.position) < _RANGE)
		{
			_behaviorCor = StartCoroutine(CBehavior());
		}
		else
		{
			_anim.SetTrigger("Idle");
		}

		_stompCor = null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _RANGE + _GETTING_CLOSE_ADDED_RANGE);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _RANGE);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _MIN_RANGE);
	}
}

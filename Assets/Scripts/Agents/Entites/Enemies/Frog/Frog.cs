using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
	[SerializeField] private Vector2 _jumpSpeed = new Vector2( 5, 15 );

	private bool _facingRight = true;
	private Vector2 InvertedJumpSpeed => new Vector2(-_jumpSpeed.x, _jumpSpeed.y);

	private Coroutine _actionCor = null;

	protected override void Start()
	{
		base.Start();

		_anim = GetComponent<Animator>();

		_actionCor = StartCoroutine(CAction());
	}

	protected override void Update()
	{
		base.Update();

		_anim.SetFloat("YVeloc", rb.velocity[1]);
	}

	// Returns true if new rotation is turned right
	private bool Rotate()
	{
		// Rotate object
		if (transform.rotation == Quaternion.identity)
		{
			transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
			return false;
		}

		transform.rotation = Quaternion.identity;
		return true;
	}

	private IEnumerator CAction()
	{
		yield return new WaitForSeconds(Random.Range(2.5f, 6.5f));
		_soundPlayer.PlayOneShotLocalized(_soundClips[0], transform.position);

		if (OnGround && Random.Range(0.0f, 1.0f) > 0.5f)
			Jump();
		else if (OnGround)
			Walk();

		_actionCor = StartCoroutine(CAction());
	}

	private void Jump()
	{
		MaybeRotate();
		rb.velocity = _facingRight ? _jumpSpeed : InvertedJumpSpeed;
	}

	private void Walk()
	{
		MaybeRotate();
		Vector2 newVel = _jumpSpeed;
		newVel.x *= _facingRight ? 2 : -2;
		newVel.y = 0;
		rb.velocity = newVel;
	}

	private void MaybeRotate()
	{
		_facingRight = Rotate();
	}

	protected override void OnPlayerStomp(Player p)
	{
		base.OnPlayerStomp(p);
		InterruptAction();
	}

	private void InterruptAction()
	{
		if (gameObject.activeSelf)
		{
			StopCoroutine(_actionCor);
			_actionCor = StartCoroutine(CAction());
		}
	}
}

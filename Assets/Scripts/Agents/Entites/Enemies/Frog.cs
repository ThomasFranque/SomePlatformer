using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Entity
{
	[SerializeField] private Vector2 _jumpSpeed = new Vector2( 5, 15 );

	private bool _facingRight = true;
	private Vector2 InvertedJumpSpeed => new Vector2(-_jumpSpeed.x, _jumpSpeed.y);

	private Animator anim;

	protected override void Start()
	{
		base.Start();

		anim = GetComponent<Animator>();

		StartCoroutine(CAction());
	}

	protected override void Update()
	{
		base.Update();

		anim.SetFloat("YVeloc", rb.velocity[1]);
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

		if (OnGround && Random.Range(0.0f, 1.0f) > 0.5f)
			Jump();
		else if (OnGround)
			Walk();

		StartCoroutine(CAction());
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

	protected override void OnHit(Vector2 hitDirection, float knockSpeed)
	{
		base.OnHit(hitDirection, knockSpeed);
		SetInvunerability(true);
	}

	protected override void OnPlayerCollision(Collider2D col)
	{
		base.OnPlayerCollision(col); 
		Vector3 hitDirection =
				 (col.transform.position - transform.position).normalized;
		hitDirection.y = 2.5f;
		Player p = col.GetComponent<Player>();

		p.Hit(hitDirection, 50.0f);
	}
}

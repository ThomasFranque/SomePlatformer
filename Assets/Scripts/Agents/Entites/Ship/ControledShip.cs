using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControledShip : Entity
{
	[SerializeField] private ParticleSystem _thursterParticles;

	private KeyCode _leftInput = KeyCode.A;
	private KeyCode _rightInput = KeyCode.D;
	private KeyCode _upInput = KeyCode.W;
	private KeyCode _downInput = KeyCode.S;

	private bool AnyMovementKeyPressed => 
		Input.GetKey(_leftInput) || Input.GetKey(_rightInput) || 
		Input.GetKey(_upInput) || Input.GetKey(_downInput);

	protected override void Start()
	{
		base.Start();

	}

	protected override void Update()
	{
		base.Update();

		// PROTOTYPING
		if (Input.GetKey(_leftInput))
			rb.velocity = (new Vector2(-100, rb.velocity.y));
		else if (Input.GetKey(_rightInput))
			rb.velocity = (new Vector2(100, rb.velocity.y));
		if (Input.GetKey(_upInput))
			rb.velocity = new Vector2(rb.velocity.x, 100);
		else if (Input.GetKey(_downInput))
			rb.velocity = (new Vector2(rb.velocity.x, -100));

		if (rb.velocity != Vector2.zero)
		{
			Vector2 v = rb.velocity;
			float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		}

		if (AnyMovementKeyPressed)
			_thursterParticles.Play();
		else
			_thursterParticles.Stop();
	}
}

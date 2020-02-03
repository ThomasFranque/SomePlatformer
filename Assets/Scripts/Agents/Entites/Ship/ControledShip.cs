using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipShoot))]
public class ControledShip : Entity
{
    [SerializeField] private ParticleSystem _thursterParticles = null;
    //TODO: Maybe create a scriptable object
    [SerializeField] private float _acceleration = 10.0f;

	//! Linear drag only allows for a max velocity of 750 with a 
	//! 10.0f acceleration (just make the math)
    [SerializeField] private float _maxVelocity = 120.0f;

    private KeyCode _leftInput = KeyCode.A;
    private KeyCode _rightInput = KeyCode.D;
    private KeyCode _upInput = KeyCode.W;
    private KeyCode _downInput = KeyCode.S;

	private float _slowWhileShootingAmmount;

    private ShipShoot _thisShipShoot;

    private bool AnyMovementKeyPressed =>
        Input.GetKey(_leftInput) || Input.GetKey(_rightInput) ||
        Input.GetKey(_upInput) || Input.GetKey(_downInput);


    private void Awake()
    {
		_thisShipShoot = GetComponent<ShipShoot>();
		_thisShipShoot.SetControlledShip(this);

		_slowWhileShootingAmmount = 5.0f;
    }

    private void FixedUpdate()
    {
        Vector2 newVelocity = _rb.velocity;

		float _calculatedAcceleration = _thisShipShoot.IsShooting ? 
		_acceleration / _slowWhileShootingAmmount : 
		_acceleration;

        // PROTOTYPING
        if (Input.GetKey(_leftInput))
            newVelocity.x -= _calculatedAcceleration;
        else if (Input.GetKey(_rightInput))
            newVelocity.x += _calculatedAcceleration;

        if (Input.GetKey(_upInput))
            newVelocity.y += _calculatedAcceleration;
        else if (Input.GetKey(_downInput))
            newVelocity.y -= _calculatedAcceleration;

        CapVector(ref newVelocity);

        _rb.velocity = newVelocity;

        if (_rb.velocity != Vector2.zero)
        {
            Vector2 v = _rb.velocity;
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        if (AnyMovementKeyPressed)
            _thursterParticles.Play();
        else
            _thursterParticles.Stop();
    }

    private void CapVector(ref Vector2 velocity)
    {
        velocity.x = Mathf.Clamp(velocity.x, -_maxVelocity, _maxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -_maxVelocity, _maxVelocity);

        // // Cap x
        // if (velocity.x > _maxVelocity)
        // 	velocity.x = _maxVelocity;
        // else if (rb.velocity.x < -_maxVelocity)
        // 	velocity.x = -_maxVelocity;

        // // Cap y
        // if (velocity.y > _maxVelocity)
        // 	velocity.y = _maxVelocity;
        // else if (velocity.y < -_maxVelocity)
        // 	velocity.y = -_maxVelocity;
    }
}

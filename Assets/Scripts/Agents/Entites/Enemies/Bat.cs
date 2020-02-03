using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    private const float _MOVE_TOWARDS_Y_OFFSET = 8.0f;
    private const float _STOMP_SELF_Y_VELOCITY = -80.0f;

    [Header("Bat Properties")]
    [SerializeField] private float _flightSpeed = 40.0f;
    [SerializeField] private float _attackCooldown = .5f;
    [SerializeField] private float _range = 32.0f;

    private float _timeOfPlayerHit;

    private bool PlayerInRange => Vector3.Distance(Player.Instance.transform.position, transform.position) < _range;
    private bool ShouldFlyTowardsPlayer => Time.time - _timeOfPlayerHit > _attackCooldown && !KnockedBack && PlayerInRange;


    private void Awake()
    {
        _timeOfPlayerHit = -900.0f;
    }

    protected override void Update()
    {
        base.Update();

        if (ShouldFlyTowardsPlayer)
            MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        // Preparing variable for Lerp
		Vector3 _desiredPosition = Player.Instance.transform.position;
        LookAtPlayer();
        _desiredPosition.y += _MOVE_TOWARDS_Y_OFFSET;

        transform.position = Vector2.MoveTowards(transform.position, _desiredPosition, _flightSpeed * Time.deltaTime);
    }

    protected override void OnPlayerStomp(Player p)
    {
        base.OnPlayerStomp(p);
        _rb.velocity = new Vector2(0, _STOMP_SELF_Y_VELOCITY);
    }

    protected override void OnHitPlayer()
    {
        _timeOfPlayerHit = Time.time;
    }

    protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
    {
        _timeOfPlayerHit = Time.time;
        base.OnHit(cameFromRight, knockSpeed, dmg);
    }

    protected override void OnDeath(byte dmg = 1)
    {
        SetGravityScale(9);
        base.OnDeath(dmg);
    }

    private void OnDrawGizmos() 
    {        
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _range);
    }
}

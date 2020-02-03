using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private const float _HIT_FLASH_DURATION = 0.06f;
    private const byte _FLASHES_WHEN_HIT = 1;
    private const float _STOMP_CENTER_Y_OFFSET = 0.0f;
    private const float _TIME_BEFORE_FINAL_DEATH = .15f;

    [Header("Enemy")]
    [SerializeField] protected EnemyProperties _selfProperties;

    [SerializeField] private Color _hitColor = Color.white;
    [SerializeField] protected bool _hurtsPlayer = true;
    [SerializeField] protected bool _canBeStomped = true;
    [SerializeField] protected bool _useColliderAsTrigger = false;
    [SerializeField] private float _stompYSpeed = 250.0f;
    [SerializeField] private float _knockIntensity = 50.0f;
    [SerializeField] private byte _damage = 1;

    protected bool _colliderTouchingSolidGround;

    protected override void Start()
    {
        base.Start();
        GetEnemyProperties();
    }

    protected void GetEnemyProperties()
    {
        if (_selfProperties != null)
        {
            _hurtsPlayer = _selfProperties.HurtsPlayer;
            _canBeStomped = _selfProperties.CanBeStomped;
            _hitColor = _selfProperties.HitColor;
            _useColliderAsTrigger = _selfProperties.UseColliderAsTrigger;
            _stompYSpeed = _selfProperties.StompYSpeed;
            _knockIntensity = _selfProperties.KnockBackIntensity;
            _damage = _selfProperties.Damage;
        }
        else
            Debug.LogWarning($"Enemy Properties on {name.ToUpper()} not assigned. Using default values.\n Please create one from the asset menu.");
    }

    protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
    {
        if (_invulnerable) return;
        CameraActions.ActiveCamera.Shake(10 * dmg, 20 * dmg, 0.06f);
        deathParticle?.Emit(Random.Range(3 * dmg, 5 * dmg));
        StartCoroutine(Flash());

        base.OnHit(cameFromRight, knockSpeed, dmg);
        SetInvulnerability(true);
    }

    protected override void OnDeath(byte dmg = 1)
    {
        StartCoroutine(CDeathSequence(dmg));
    }

    protected override void OnPlayerCollision(Collision2D col)
    {
        base.OnPlayerCollision(col);
        if (!_useColliderAsTrigger && _canBeStomped)
        {
            Vector3 contactPoint = col.GetContact(0).point;
            Vector3 center = selfCol.bounds.center;

            bool right = contactPoint.x < center.x;
            bool top = Player.Instance.transform.position.y > center.y + _STOMP_CENTER_Y_OFFSET;

            if (top)
            {
                OnPlayerStomp(col.gameObject.GetComponent<Player>());
                return;
            }
        }

        HitPlayer(col.collider);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_useColliderAsTrigger && col.tag == "Player")
        {
            Vector3 center = selfCol.bounds.center;
            bool top = Player.Instance.transform.position.y > center.y + _STOMP_CENTER_Y_OFFSET;

            if (top && _canBeStomped)
            {
                OnPlayerStomp(col.gameObject.GetComponent<Player>());
                return;
            }

            if (_hurtsPlayer) HitPlayer(col);

        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {

    }

    private void HitPlayer(Collider2D col)
    {
        if (_hurtsPlayer)
        {
            Player p = col.GetComponent<Player>();
            bool cameFromRight = p.transform.position.x < transform.position.x;
            OnHitPlayer();

            p.Hit(cameFromRight, _knockIntensity, _damage);
        }
    }

    protected virtual void OnHitPlayer()
    {

    }

    protected virtual void OnPlayerStomp(Player p)
    {
        p.DoStomp(_stompYSpeed);
        Hit(true, 0.0f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
            OnPlayerCollision(collision);
    }

    protected override void UpdateInvulnerabilityEffect() { }

    private IEnumerator CDeathSequence(byte deathDmg)
    {
        _hurtsPlayer = false;
        _sr.color = Color.white;
        yield return new WaitForSeconds(_TIME_BEFORE_FINAL_DEATH);
        deathParticle?.Emit(Random.Range(35, 55));
        base.OnDeath(deathDmg);
    }

    //https://www.reddit.com/r/Unity2D/comments/8xcw8g/how_can_i_make_a_sprite_blink_in_white_when/
    protected IEnumerator Flash()
    {
        StoreCurrentSRColor();
        for (int i = 0; i < _FLASHES_WHEN_HIT; i++)
        {
            _anim.enabled = false;
            SetSpriteColor(_hitColor);
            yield return new WaitForSeconds(_HIT_FLASH_DURATION);
            if (!Dead) SetSpriteColor(_srBaseColor);
            _anim.enabled = true;
            yield return new WaitForSeconds(_HIT_FLASH_DURATION);
        }
    }

    protected void LookAtPlayer()
    {

        bool playerOnTheRight = Player.Instance.transform.position.x > transform.position.x;


        if (playerOnTheRight)
            transform.rotation = Quaternion.identity;
        else
            transform.rotation = Quaternion.Euler(0, 180.0f, 0);
    }
}

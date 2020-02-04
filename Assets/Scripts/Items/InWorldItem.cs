using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0618
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(TrailRenderer))]
public abstract class InWorldItem : MonoBehaviour
{
    private const byte _MAX_BOUNCES = 3;
    private const float _BLINK_PREWARN_SECONDS = 2.5f;

    protected abstract InWorldResourceProperties _BaseProperties { get; }
    protected Rigidbody2D _rb;
    protected SpriteRenderer _sr;
    protected TrailRenderer _tr;
    protected ParticleSystem _ps;


    private GFXBlink _blink;

    private float _timeOfSpawn;
    private byte _bounces;

    protected virtual void Awake()
    {
        _blink = new GFXBlink();
        _timeOfSpawn = Time.time;
        _bounces = 0;

        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _tr = GetComponent<TrailRenderer>();
        _ps = GetComponentInChildren<ParticleSystem>();

        _sr.sprite = _BaseProperties.Sprite;
        _sr.color = _BaseProperties.SpriteColor;
        _tr.startColor = _BaseProperties.SpriteColor;
        _ps.startColor = _BaseProperties.SpriteColor;

        if (_BaseProperties.LockRotation)
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            _rb.rotation = _BaseProperties.InitialRotationAmount;

        Burst();
    }

    private void Burst()
    {
        Vector2 newVelocity = _rb.velocity;
        newVelocity.x = Random.Range(_BaseProperties.InitialBurstSpeedXRange.x, _BaseProperties.InitialBurstSpeedXRange.y);
        newVelocity.y = Random.Range(_BaseProperties.InitialBurstSpeedYRange.x, _BaseProperties.InitialBurstSpeedYRange.y);
        _rb.velocity = newVelocity;
    }

    protected virtual void Update()
    {
        if (Time.time - _timeOfSpawn > _BaseProperties.Lifetime)
            Destroy(gameObject);
        else if (Time.time - _timeOfSpawn > _BaseProperties.Lifetime - _BLINK_PREWARN_SECONDS)
            _blink.DoBlink(_sr, 1 - (Time.time / (_timeOfSpawn + _BaseProperties.Lifetime)));
    }

    protected abstract void OnPlayerCollision(Player p);

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
            OnPlayerCollision(other.collider.GetComponent<Player>());

        else if (other.collider.tag == "Tilemap")
            OnBounce();

    }

    private void OnBounce()
    {
        _bounces++;
        if (_bounces >= _MAX_BOUNCES)
        {
            _rb.sharedMaterial = null;
            _tr.emitting = false;
        }
    }

    protected virtual void PickupEffect()
    {
        _ps.transform.parent = null;
        _ps.enableEmission = false;
        _ps.Emit(Random.Range(5,15));
        DestroyParticleSystemObj();
    }

    protected void DestroyParticleSystemObj()
    {
        Destroy(_ps.gameObject, _ps.startLifetime + 0.5f);
    }
}

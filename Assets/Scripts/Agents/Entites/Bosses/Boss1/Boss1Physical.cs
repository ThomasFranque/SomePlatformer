using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Physical : Enemy
{
    [Header("Boss Properties")]
    [SerializeField] private int _stage2MinHP = 20;
    [SerializeField] private int _stage3MinHP = 10;

    private Animator _bossAnimator;
    private float _timeOfLastHit;

    private byte _currentStage = 1;
    private bool Stage2Reached => HP <= _stage2MinHP;
    private bool Stage3Reached => HP <= _stage3MinHP;

    private bool _isDead;

    private void Awake()
    {
    }

    protected override void Start()
    {
        GetEnemyProperties();
        _selfCol = GetComponent<Collider2D>();
		_selfCol.enabled = false;
        _sr = GetComponentInChildren<SpriteRenderer>();
        StoreCurrentSRColor();
        _bossAnimator = GetComponentInParent<Animator>();
        _anim = _bossAnimator;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
    {
        base.OnHit(cameFromRight, knockSpeed, dmg);

        if (_currentStage == 3)
            return;
        else if (Stage3Reached && _currentStage != 3)
        {
            _bossAnimator.SetTrigger("Final Stage");
            _currentStage++;
            Debug.Log("Boss Reached Stage 3");
            return;
        }
        else if (Stage2Reached && _currentStage != 2)
        {
            _bossAnimator.SetTrigger("Next Stage");
            _currentStage++;
            Debug.Log("Boss Reached Stage 2");
        }
    }

    protected override void OnDeath(byte dmg = 1)
    {
        if (!_isDead)
        {
            _bossAnimator.SetTrigger("Dead");
            _isDead = true;
        }
    }

    public void SetHurtsPlayer(bool active)
    {
        _hurtsPlayer = active;
    }
    public void SetCanBeStomped(bool active)
    {
        _canBeStomped = active;
    }

    public void ShootDeathParticles()
    {
        deathParticle.Emit(200);
        deathParticle.transform.parent = null;
        _sr.gameObject.SetActive(false);
    }
}

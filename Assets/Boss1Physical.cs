using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Physical : Enemy
{
	[SerializeField] private int _stage2MinHP = 20;
	[SerializeField] private int _stage3MinHP = 10;

	private Animator _bossAnimator;
	private float _timeOfLastHit;
	private bool _invulnerable;

	private byte _currentStage = 1;
	private bool Stage2Reached => HP < _stage2MinHP;
	private bool Stage3Reached => HP < _stage3MinHP;


	protected override void Start()
	{
		GetEnemyProperties();
		_blink = new GFXBlink();
		selfCol = GetComponent<Collider2D>();
		sr = GetComponentInChildren<SpriteRenderer>();
		_bossAnimator = GetComponentInParent<Animator>();
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
			Debug.Log(_currentStage);
			return;
		}
		else if (Stage2Reached && _currentStage != 2)
		{
			_bossAnimator.SetTrigger("Next Stage");
			_currentStage++;
			Debug.Log("Boss Reached Stage 2");
		}

	}
}

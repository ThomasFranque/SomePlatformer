using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Hazard
{
	[SerializeField] private float _cycleSpeedMultiplier = 1.0f;
	[SerializeField] private bool _stayActive = false;

	private const float _ACTIVE_DURATION = 1.0f;
	private const float _PRE_WARN_BEFORE = 1.0f;
	private const float _IDLE_DURATION = 3.0f;

	private Animator _anim;
	private Collider2D _selfColl;

	private Coroutine cycleActivityCor;

	// Start is called before the first frame update
	void Start()
	{
		_anim = GetComponent<Animator>();
		_selfColl = GetComponent<Collider2D>();

		_selfColl.enabled = false;

		if (_stayActive)
			OnActivate();
		else
			cycleActivityCor = StartCoroutine(CCycleActitvity());
	}

	private void OnActivate()
	{
		_anim.SetTrigger("Activate");
		_selfColl.enabled = true;
	}

	private void OnWithdraw()
	{
		_anim.SetTrigger("Withdraw");
		_selfColl.enabled = false;
	}

	private void OnPreWarn()
	{
		_anim.SetTrigger("PreWarn");
	}

	private IEnumerator CCycleActitvity()
	{
		yield return new WaitForSeconds((_IDLE_DURATION * _cycleSpeedMultiplier) - (_PRE_WARN_BEFORE * _cycleSpeedMultiplier));
		OnPreWarn();
		yield return new WaitForSeconds(_PRE_WARN_BEFORE * _cycleSpeedMultiplier);
		OnActivate();
		yield return new WaitForSeconds(_ACTIVE_DURATION * _cycleSpeedMultiplier);
		OnWithdraw();

		cycleActivityCor = StartCoroutine(CCycleActitvity());
	}
}

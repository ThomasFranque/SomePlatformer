using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerIdleBehavior : MonoBehaviour
{
	[SerializeField] private MonoBehaviour _nextBehavior;
	private ChargerController _chargerController;
	private Action PlayerSighted;

    // Start is called before the first frame update
    void Start()
    {
		_chargerController = GetComponentInParent<ChargerController>();

		PlayerSighted += BehaviorOver;
    }

    // Update is called once per frame
    void Update()
    {
		//if (_chargerController.PlayerInRange)
		//	OnPlayerSighted();
    }

	private void OnPlayerSighted()
	{
		PlayerSighted?.Invoke();
	}

	private void BehaviorOver()
	{
		_chargerController.ActivateBehaviour(this, _nextBehavior);
	}
}

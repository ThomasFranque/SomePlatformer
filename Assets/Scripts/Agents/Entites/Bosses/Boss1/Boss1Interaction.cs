using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Interaction : Interactible
{
	[SerializeField] private Boss1InitialInteraction _bossInitInteraction;
	protected override void Start()
	{
		base.Start();
		AddActionToInteraction(TriggerInteractionSequence);
	}

	private void TriggerInteractionSequence()
	{
		_bossInitInteraction.InteractionStarted();
		Destroy(this);
	}
}

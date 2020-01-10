using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashedShipInteraction : InteractionListener
{
	protected override void InteractionFinishedAction()
	{
		GameObject.Find("CANVAS").GetComponent<MainCanvas>().FadeIn();
	}

	protected override void InteractionStartedAction()
	{
		Debug.Log("I listen!");
	}
}

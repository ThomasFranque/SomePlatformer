using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerInputBehaviour : StateMachineBehaviour
{
	[SerializeField] private bool _playerInputOnEnter = false;
	[SerializeField] private bool _playerInputOnExit = true;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player.Instance.SetInputReading(_playerInputOnEnter);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player.Instance.SetInputReading(_playerInputOnExit);
	}
}

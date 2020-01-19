using System;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayerBehaviour : StateMachineBehaviour
{
	[SerializeField] private bool _lookAtOnStart = false;
	[SerializeField] private bool _lookAtOnWhile = false;
	[SerializeField] private bool _lookAtOnEnd = false;

	private Action<Animator> WhileAction;
	private Action<Animator> EndAction;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_lookAtOnStart)
			LookTowardsPlayer(animator);

		if (_lookAtOnWhile)
			WhileAction = LookTowardsPlayer;

		if (_lookAtOnEnd)
			EndAction = LookTowardsPlayer;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		WhileAction?.Invoke(animator);
	}

	//OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EndAction?.Invoke(animator);
	}

	private void LookTowardsPlayer(Animator anim)
	{
		bool playerOnTheRight = Player.Instance.transform.position.x > anim.transform.position.x;


		if (playerOnTheRight)
			anim.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
		else
			anim.transform.rotation = Quaternion.identity;
	}

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBehavior : StateMachineBehaviour
{
	[SerializeField] private float _moveTowardsSpeed = 1;
	[SerializeField] private string _nextTriggerName = "Wait";
	private Vector3 _bounceFinalPos;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_bounceFinalPos = animator.GetComponent<Boss1Final>().GetBounceXPos();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Mathf.Abs(_bounceFinalPos.x - animator.transform.position.x) < 20)
			animator.SetTrigger(_nextTriggerName);
		
		SmoothMoveTowardsPlayerX(animator);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}

	private void SmoothMoveTowardsPlayerX(Animator animator)
	{
		// Preparing variable for Lerp
		Vector3 _desiredPosition = new Vector3(
			_bounceFinalPos.x,
			animator.transform.position.y,
			animator.transform.position.z);


		// Using Lerp to pan the camera 
		Vector3 smoothPosition =
			Vector3.Lerp(animator.transform.position, _desiredPosition,
			_moveTowardsSpeed * Time.deltaTime);

		animator.transform.position = smoothPosition;
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

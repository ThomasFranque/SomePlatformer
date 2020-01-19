using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToMiddleBehaviour : StateMachineBehaviour
{
	[SerializeField] private float _speed = 2;

	private Boss1Final _boss;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_boss = animator.GetComponent<Boss1Final>();
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SmoothMoveTowardsMiddle(animator);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
	private void SmoothMoveTowardsMiddle(Animator animator)
	{
		// Preparing variable for Lerp
		Vector3 _desiredPosition = new Vector3(
			_boss.RoomCenterPos.x,
			animator.transform.position.y,
			animator.transform.position.z);


		// Using Lerp to pan the camera 
		Vector3 smoothPosition =
			Vector3.Lerp(animator.transform.position, _desiredPosition,
			_speed * Time.deltaTime);

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

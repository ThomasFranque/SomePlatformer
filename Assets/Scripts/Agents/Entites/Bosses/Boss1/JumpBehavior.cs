using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehavior : StateMachineBehaviour
{
	[SerializeField] private float _moveTowardsSpeed = 1.0f;
	[SerializeField] private bool _doShockWave = false;
	[SerializeField] private GameObject _shockWavePrefab = null;

	private float _targetX = 0;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<Boss1Final>().SetJumpBehavior(this);
		GetPlayerX();
	}


	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SmoothMoveTowardsPlayerX(animator);
	}


	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_doShockWave)
		{
			Instantiate(_shockWavePrefab, animator.transform.position, Quaternion.Euler(0,180,0));
			Instantiate(_shockWavePrefab, animator.transform.position, Quaternion.identity);
		}
	}

	private void SmoothMoveTowardsPlayerX(Animator animator)
	{
		// Preparing variable for Lerp
		Vector3 _desiredPosition = new Vector3(
			_targetX,
			animator.transform.position.y,
			animator.transform.position.z);


		// Using Lerp to pan the camera 
		Vector3 smoothPosition =
			Vector3.Lerp(animator.transform.position, _desiredPosition,
			_moveTowardsSpeed * Time.deltaTime);

		animator.transform.position = smoothPosition;
	}

	public void GetPlayerX()
	{
		_targetX = Player.Instance.transform.position.x;
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

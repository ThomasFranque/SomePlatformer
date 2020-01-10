using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourController
{
	void ActivateBehaviour(MonoBehaviour sender, MonoBehaviour nextBehavior);
}

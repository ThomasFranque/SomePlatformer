using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeHit
{
	int HP { get; set; }

	void Hit(bool cameFromRight, float knockSpeed, byte damage = 1);
}

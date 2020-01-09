using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "LP2/Level Spec")]
public class LevelSpec : ScriptableObject
{
	[SerializeField] private Vector3[] enemySpawnpoints = null;
		
	public IEnumerable<Vector3> EnemySpawnPoints => enemySpawnpoints;
}

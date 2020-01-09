using UnityEngine;

public class Level : MonoBehaviour
{
	[SerializeField] private GameObject _enemyPrefab = null;
	[SerializeField] private LevelSpec _levelSpec = null;

	private void Awake()
	{
		foreach (Vector3 pos in _levelSpec.EnemySpawnPoints)
		{
			GameObject newEnemy =
				Instantiate(_enemyPrefab, pos, Quaternion.identity);
		}
	}
}

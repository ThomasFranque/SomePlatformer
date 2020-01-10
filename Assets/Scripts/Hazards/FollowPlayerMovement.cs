using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : MonoBehaviour
{
	[SerializeField] private int _followDistance;
	private Player _player;
	private List<Vector3> _positions;

	// Start is called before the first frame update
	void Start()
    {
		_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		_positions = new List<Vector3>(480);
    }

    // Update is called once per frame
    void Update()
    {
		if (_positions.Count == 0)
		{
			_positions.Add(_player.transform.position); //store the players current position
			return;
		}
		_positions.Add(_player.transform.position);
		//else if (_positions[_positions.Count - 1] != _player.transform.position)
		//{
		//	//Debug.Log("Add to list");
		//	_positions.Add(_player.transform.position); //store the position every frame
		//}

		if (_positions.Count > _followDistance)
		{
			transform.position = _positions[0]; //move
			_positions.RemoveAt(0); //delete the position that player just moved to
		}
	}
}

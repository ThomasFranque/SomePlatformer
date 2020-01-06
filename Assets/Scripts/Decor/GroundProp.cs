using UnityEngine;

public class GroundProp : MonoBehaviour
{
	private float _MIN_ANIMATOR_SPEED = 0.2f;
	private float _MAX_ANIMATOR_SPEED = 1.5f;

	private Animator _anim;

	private Vector3 _offset;
	private Vector2 _boxColSize;

	private bool _IsPlayerStepping { get
		{
			Collider2D collider = Physics2D.OverlapBox(
				transform.position + _offset,
				_boxColSize,
				0,
				LayerMask.GetMask("Player"));
			Collider2D collider2 = Physics2D.OverlapBox(
				transform.position + _offset,
				_boxColSize,
				0,
				LayerMask.GetMask("Enemy"));

			if (collider != null || collider2 != null) return true;
			return false;
		} 
 }

    // Start is called before the first frame update
    void Start()
    {
		_offset = new Vector3(0.0f, 1.0f);
		_boxColSize = new Vector2(5f, 0.5f);

		_anim = GetComponent<Animator>();
		_anim.speed = Random.Range(_MIN_ANIMATOR_SPEED, _MAX_ANIMATOR_SPEED);
    }

    // Update is called once per frame
    void Update()
    {
		_anim.SetBool("Stepped", _IsPlayerStepping);
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position + _offset, new Vector3(_boxColSize.x, _boxColSize.y, 1));
	}
}

using System.Collections;
using UnityEngine;

public class Boss1Final : MonoBehaviour
{
	private Player _pScript => Player.Instance;
	private Vector3 _pPos => Player.Instance.transform.position;

	[SerializeField]
	private Transform _roomCenter;
	[Tooltip("Distance from the center to the border.")]
	[SerializeField]
	private float _roomSizeFromCenter = 40.0f;
	[SerializeField]
	private Transform _physicalTransform = null;

	[SerializeField]
	private bool _autoGroundOnStart = false;

	private JumpBehavior _jumpBehaviour = null;

	public bool InsideRoom => transform.position.x > RoomLeftEdge && transform.position.x < RoomRightEdge;

	private float RoomRightEdge => _roomCenter.position.x + _roomSizeFromCenter;
	private float RoomLeftEdge => _roomCenter.position.x - _roomSizeFromCenter;

	// Start is called before the first frame update
	private void Start()
    {
		if (_autoGroundOnStart)
			transform.position = new Vector3(transform.position.x, GetGroundPosition().y, 0);
    }

	// Update is called once per frame
	private void Update()
    {

	}
	// Called on jump animation Event
	private void OnJumpEnd()
	{
		_jumpBehaviour.GetPlayerX();
		CameraActions.ActiveCamera.Shake(duration:0.1f);
	}

	public void SetJumpBehavior(JumpBehavior jumpBehavior)
	{
		_jumpBehaviour = jumpBehavior;
	}

	public Vector3 GetBounceXPos()
	{
		// closer to right edge
		if (Mathf.Abs(RoomRightEdge - transform.position.x) > Mathf.Abs(RoomLeftEdge - transform.position.x))
			return new Vector3(RoomRightEdge, _physicalTransform.position.y, 0);
		else
			return new Vector3(RoomLeftEdge, _physicalTransform.position.y, 0);
	}

	private Vector3 GetGroundPosition()
	{
		RaycastHit2D hit =
			Physics2D.Raycast(transform.position, Vector3.down, 600.0f, LayerMask.GetMask("Ground"));

		Vector3 hitPoint = hit.point;
		return hitPoint;
	}


}

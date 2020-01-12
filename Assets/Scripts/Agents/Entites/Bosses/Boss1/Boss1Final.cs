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
	private Animator _rightDoorAnimator = null;

	[SerializeField]
	private bool _autoGroundOnStart = false;

	[SerializeField]
	private Cinemachine.CinemachineVirtualCamera _cutsceneVCam;
	[SerializeField] private Cinemachine.CinemachineVirtualCamera _bossRoomCam;

	private JumpBehavior _jumpBehaviour = null;
	private Boss1Physical _bossPhysical;

	public bool InsideRoom => transform.position.x > RoomLeftEdge && transform.position.x < RoomRightEdge;
	public Vector3 RoomCenterPos => _roomCenter.position;

	private float RoomRightEdge => _roomCenter.position.x + _roomSizeFromCenter;
	private float RoomLeftEdge => _roomCenter.position.x - _roomSizeFromCenter;

	// Start is called before the first frame update
	private void Start()
	{
		_bossPhysical = GetComponentInChildren<Boss1Physical>();
		_rightDoorAnimator.SetTrigger("Close");

		if (_autoGroundOnStart)
			transform.position = new Vector3(transform.position.x, GetGroundPosition().y, 0);

		_bossPhysical.SetHurtsPlayer(false);
		_bossPhysical.SetCanBeStomped(false);
	}

	// Update is called once per frame
	private void Update()
	{

	}
	// Called on jump animation Event
	private void OnJumpEnd()
	{
		_jumpBehaviour?.GetPlayerX();
		CameraActions.ActiveCamera.Shake(duration: 0.1f);
	}

	public void TriggerCutsceneCam()
	{
		_cutsceneVCam.enabled = !_cutsceneVCam.enabled;
	}

	// Called on cutscene animation Event
	public void TriggerCutsceneScreechCamShake()
	{
		CameraActions.ActiveCamera.Shake(duration: 2);
	}
	// Called on initial cutscene animation Event
	public void TriggerCutsceneLandCamShake()
	{
		CameraActions.ActiveCamera.Shake(amplitude: 40, frequency: 30);
		_bossPhysical.SetHurtsPlayer(true);
		_bossPhysical.SetCanBeStomped(true);
	}

	public void ShootDeathParticles()
	{
		_bossPhysical.ShootDeathParticles();
	}

	// Called when death animation ended
	public void DeathAnimationEnded()
	{
		TriggerCutsceneCam();
		_cutsceneVCam.transform.parent = null;
		_rightDoorAnimator.SetTrigger("Open");
		_bossRoomCam.enabled = false;
		Destroy(gameObject);
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

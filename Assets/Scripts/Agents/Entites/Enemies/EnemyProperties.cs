using UnityEngine;

public class EnemyProperties : ScriptableObject, IEnemyProperties
{
	[Header("General Properties")]
	[SerializeField] private bool _hurtsPlayer = true;
	[SerializeField] private bool _canBeStomped = true;
	[Tooltip("Using as trigger will disable stomp.")]
	[SerializeField] private bool _useColliderAsTrigger = false;
	[SerializeField] private float _stompYSpeed = 250.0f;
	[SerializeField] private float _knockIntensity = 50.0f;

	public bool HurtsPlayer { get => _hurtsPlayer; }
	public bool CanBeStomped { get => _canBeStomped; }
	public bool UseColliderAsTrigger { get => _useColliderAsTrigger; }
	public float StompYSpeed { get => _stompYSpeed; }
	public float KnockBackIntesity { get => _knockIntensity; }
}

using UnityEngine;

public class EnemyProperties : ScriptableObject, IEnemyProperties
{
	[Header("General Properties")]
	[SerializeField] private bool _hurtsPlayer = true;
	[SerializeField] private bool _canBeStomped = true;
	[SerializeField] private Color _hitColor = Color.white;
	[SerializeField] private bool _useColliderAsTrigger = false;
	[SerializeField] private float _stompYSpeed = 250.0f;
	[SerializeField] private float _knockIntensity = 50.0f;
	[SerializeField] private byte _damage = 1;

	public bool HurtsPlayer { get => _hurtsPlayer; }
	public bool CanBeStomped { get => _canBeStomped; }
	public Color HitColor { get => _hitColor; }
	public bool UseColliderAsTrigger { get => _useColliderAsTrigger; }
	public float StompYSpeed { get => _stompYSpeed; }
	public float KnockBackIntensity { get => _knockIntensity; }
	public byte Damage => _damage;
}

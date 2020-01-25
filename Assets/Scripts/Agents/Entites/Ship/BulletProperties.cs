using UnityEngine;

[CreateAssetMenu(menuName = "Player/Components/Ship Bullet Properties")]
public class BulletProperties : ScriptableObject
{
	[SerializeField] private Sprite[] _possibleSprites = null;

	[SerializeField] private int _damage = 1;
	[SerializeField] private float _speed = 5.0f;
	[SerializeField] private float _lifetime = 2.0f;

	public Sprite RandomSprite => _possibleSprites[Random.Range(0, _possibleSprites.Length)];
	public int Damage => _damage;
	public float Speed => _speed;
	public float Lifetime => _lifetime;
}

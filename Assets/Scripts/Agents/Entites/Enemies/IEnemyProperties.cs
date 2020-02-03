public interface IEnemyProperties
{
	bool HurtsPlayer { get; }
	bool CanBeStomped { get; }
	bool UseColliderAsTrigger { get; }
	float StompYSpeed { get; }
	float KnockBackIntensity { get; }
}


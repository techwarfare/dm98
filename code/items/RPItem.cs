using Sandbox;

partial class RPItem : BaseItem, IRespawnableEntity
{
    public override void Spawn()
    {
        base.Spawn();

        CollisionGroup = CollisionGroup.Weapon;
        SetInteractsAs(CollisionLayer.Debris);
    }

    public override void OnPlayerControlTick(Player owner)
    {
        base.OnPlayerControlTick(owner);
    }
}
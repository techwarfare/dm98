using Sandbox;

partial class RPItem : BaseCarriable, IPlayerControllable
{
    public override void Spawn()
    {
        base.Spawn();

        CollisionGroup = CollisionGroup.Weapon;
        SetInteractsAs(CollisionLayer.Debris);
    }

    public virtual void OnPlayerControlTick(Player owner)
    {
        
    }
}
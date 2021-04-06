using Sandbox;

partial class BaseItem : BaseCarriable, IPlayerControllable
{
    public int ItemAmount => 0;
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
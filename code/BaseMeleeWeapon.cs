using Sandbox;

partial class BaseMeleeWeapon : BaseCarriable, IPlayerControllable
{
    public virtual float PrimaryRate => 5.0f;
    public virtual float SecondaryRate => 15.0f;
    public override void Spawn()
    {
        base.Spawn();

        CollisionGroup = CollisionGroup.Weapon;
        SetInteractsAs(CollisionLayer.Debris);
    }

    [NetPredicted]
    public TimeSince TimeSincePrimaryAttack{get; set;}
    [NetPredicted]
    public TimeSince TimeSinceSecondaryAttack{get; set;}
    public virtual void OnPlayerControlTick(Player owner)
    {
        if (owner.Input.Down(InputButton.Reload))
        {
            Reload(owner);
        }

        if ( CanPrimaryAttack(owner) )
        {
            TimeSincePrimaryAttack = 0;
            AttackPrimary(owner);
        }

        if ( CanPrimaryAttack(owner) )
        {
            TimeSinceSecondaryAttack = 0;
            AttackSecondary(owner);
        }
    }
    public virtual void Reload(Player owner)
    {

    }
    public virtual bool CanPrimaryAttack(Player owner)
    {
        if ( !ownner.Input.Down(InputButton.Attack1)) return false;

        var rate = PrimaryRate;
        if (rate <= 0) return true;

        return TimeSincePrimaryAttack > (1/rate);
    }
    public virtual void AttackPrimary()
    {

    }
    public virtual bool CanSecondaryAttack(Player owner)
    {
        if ( !owner.Input.Down(InputButton.Attack2)) return false;

        var rate = SecondaryRate;
        if (rate <= 0) return true;

        return TimeSinceSecondaryAttack > 1(1/rate);
    }
    public virtual void AttackSecondary()
    {

    }
    public virtual IEnumerable<TraceResult> TraceBlade(Vector3 start, Vector3 end, float radius=1.0f)
    {
        bool HitWorld = Physics.TestPointContents(start, CollisionLayer.World);

        var tr = Trace.Ray(start,end).UseHitboxes().HitLayer(CollisionLayer.World, !HitWorld).Ignore(!owner).Ignore(this).Size(radius).Run();

        yield return tr;
    }
}
using Sandbox;

partial class BaseDmMeleeWeapon : BaseMeleeWeapon, IRespawnableEntity
{
    [NetPredicted]
    public TimeSince TimeSinceDeployed {get; set;}

    public PickupTrigger PickupTrigger {get; protected set;}
    [Net]
    public int SwingDirection = -1;
    public bool CarryingBlade = false;
    public override void ActiveStart(Entity ent)
    {
        base.ActiveStart(ent);

        TimeSinceDeployed = 0;
    }

    public override string ViewModelPath => "weapons/hl_crowbar/v_hl_crowbar.vmdl";

    public override void Spawn()
    {
        base.Spawn();

        SetModel("weapons/hl_crowbar/v_hl_crowbar.vmdl");

        PickupTrigger = new PickupTrigger();
        PickupTrigger.Parent = this;
        PickupTrigger.WorldPos = WorldPos;
    }

    public override void Reload(Player owner)
    {
        
    }
    public override void OnPlayerControlTick(Player owner)
    {
        if (TimeSinceDeployed < 0.6f)
            return;
        
        if (CarryingBlade)
            //Need to check which way the player has looked
            //Set to a default swing direction so we can swing asap
            BladePosition = owner.EyeRot;
            SwingDirection = 0;
            var directionLooked = owner.EyeRot;
            //Try to use the eye rotation of the player to determine 
            //which way we have looked
            if (directionLooked >= directionLooked + Vector3(0,0,1))
            {
                //We have looked left/right?
                SwingDirection = 1;
            } else if(directionLooked >= directionLooked + Vector3(0,1,0)){
                //We have looked up/down?
                SwingDirection = 2;
            }
        base.OnPlayerControlTick(owner);
    }
    public override void AttackPrimary()
    {
        //If swinging without actually having the blade out?
        if (SwingDirection == -1) return;
        //For a blade tracing type result
        //We need to play an animation and trace where the 'blade' is
        //Through-out the animation and see if it collides
        TimeSincePrimaryAttack = 0;
        TimeSinceSecondaryAttack = 0;
        //Or it traces from left to right, need to trace where the player was looking
        //before starting the attack and find out which way they want to swing
        switch(SwingDirection)
        {
            case 0:
                //Left to right swing
                //Simulate swinging by tracing from left of the eye pos to the right?
                foreach(var tr in TraceBlade(owner.EyePos - Vector3(1, 1, 2), owner.EyePos + Vector3(1,1,2), 20.0f))
                {
                    tr.Surface.DoBulletImpact(tr);
                    if( !IsServer) continue;
                    if (!tr.Entity.IsValid()) continue;

                    using (Prediction.Off())
                    {
                        var damage = DamageInfo.FromBullet(tr.EndPos, owner.EyeRot.Forward * 100, 15)
                            .UsingTraceResult(tr)
                            .WithAttack(owner)
                            .WithWeapon(this);

                        tr.Entity.TakeDamage(damage);
                    }
                }
                break;
            case 1:
                //Right to left swing
                foreach(var tr in TraceBlade(owner.EyePos + Vector3(1, 1, 2), owner.EyePos - Vector3(1,1,2), 20.0f))
                {
                    tr.Surface.DoBulletImpact(tr);
                    if( !IsServer) continue;
                    if (!tr.Entity.IsValid()) continue;

                    using (Prediction.Off())
                    {
                        var damage = DamageInfo.FromBullet(tr.EndPos, owner.EyeRot.Forward * 100, 15)
                            .UsingTraceResult(tr)
                            .WithAttack(owner)
                            .WithWeapon(this);

                        tr.Entity.TakeDamage(damage);
                    }
                }
                break;
            case 2:
                //Up to down swing
                foreach(var tr in TraceBlade(owner.EyePos + Vector3(0, 1, 2), owner.EyePos - Vector3(0,1,3), 20.0f))
                {
                    tr.Surface.DoBulletImpact(tr);
                    if( !IsServer) continue;
                    if (!tr.Entity.IsValid()) continue;

                    using (Prediction.Off())
                    {
                        var damage = DamageInfo.FromBullet(tr.EndPos, owner.EyeRot.Forward * 100, 15)
                            .UsingTraceResult(tr)
                            .WithAttack(owner)
                            .WithWeapon(this);

                        tr.Entity.TakeDamage(damage);
                    }
                }
                break;
            case 3:
                //Thrust
                break;
            default:
                break;
        }

        foreach(var tr in TraceBlade(owner.EyePos, owner.EyePos + owner.EyeRot.Forward))
        {
            tr.Surface.DoBulletImpact(tr);

            if (!IsServer) continue;
            if (!tr.Entity.IsValid()) continue;

            using (Prediction.Off())
            {
                var damage = DamageInfo.FromBullet(tr.EndPos, owner.EyeRot.Forward,15)
                .UsingTraceResult(tr)
                .WithAttack(owner)
                .WithWeapon(this);

                tr.Entity.TakeDamage(damage);
            }
        }
    }

    public virtual void SwingBlade(float speed, float force, float damage, float bladesize)
    {
        //Maybe we get the position of the owners hand bone instead of using eye pos
    }
    public override void OnCarryStart(Entity carrier)
    {
        base.OnCarryStart(carrier);

        if (PickupTrigger.IsValid())
        {
            PickupTrigger.EnableTouch = false;
        }

        CarryingBlade = true;
    }
}
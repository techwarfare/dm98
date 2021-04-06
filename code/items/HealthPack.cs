using Sandbox;
partial class HealthPack : RPItem
{
    [NetPredicted]
    public virtual int HealAmount => 25;
    public PickupTrigger PickupTrigger {get; protected set;}
    public virtual RoleplayPlayer Owner {get; protected set;}
    public override void OnPlayerControlTick(Player entOwner)
    {
        base.OnPlayerControlTick(entOwner);
        
        if (entOwner.Input.Down(InputButton.Attack1))
        {
            HealPlayer(entOwner);
        }
    }
    public override void Spawn()
    {
        base.Spawn();

        SetModel("");

        PickupTrigger = new PickupTrigger();
        PickupTrigger.parent = this;
        PickupTrigger.WorldPos = WorldPos;
    }
    public override OnCarryStart(Entity carrier)
    {
        base.OnCarryStart(carrier);

        if (PickupTrigger.IsValid())
        {
            PickupTrigger.EnableTouch = false;
        }

        Owner = carrier as RoleplayPlayer;
    }
    public override OnCarryDrop(Entity dropper)
    {
        base.OnCarryDrop(dropper);

        if (PickupTrigger.IsValid())
        {
            PickupTrigger.EnableTouch = true;
        }

        Owner = null;
    }

    [ClientRPC]
    public void HealPlayer(int healAmount)
    {
        Owner.Health += healAmount;
    }
}
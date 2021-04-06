
using Sandbox;

partial class RoleplayPlayer
{
    [Net]
    public NetList<int> Items {get; set;} = new ();

    public bool GiveItem(RPItem item, int amount)
    {
        if (!Host.IsServer) return false;
        if (item == null) return false;

        var currentAmount = Items.Get(item);
        return Items.Set(item, currentAmount + amount);
    }
}
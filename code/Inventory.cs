using Sandbox;
using System;
using System.Linq;

partial class RoleplayInventory : BaseInventory
{


	public RoleplayInventory( Player player ) : base ( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		var player = Owner as RoleplayPlayer;
		var notices = !player.SupressPickupNotices;
		//
		// We don't want to pick up the same weapon twice
		// But we'll take the ammo from it Winky Face
		//
		if ( ent != null && IsCarryingType( ent.GetType() ) )
		{
			var weapon = ent as BaseDmWeapon;
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;

			if ( ammo > 0 )
			{
				player.GiveAmmo( ammoType, ammo );

				if ( notices )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.WorldPos );
					PickupFeed.OnPickup( player, $"+{ammo} {ammoType}" );
				}
			}

			ItemRespawn.Taken( ent );

			// Despawn it
			ent.Delete();
			return false;
		}

		if (ent != null && IsCarryingType(ent.GetType()))
		{
			var item = ent as RPItem;
			var itemAmount = item.Amount;

			if (itemAmount > 0)
			{
				player.GiveItem(item, itemAmount);
			}

			ItemRespawn.Taken(ent);

			ent.Delete();
			return false;
		}

		if ( ent != null && notices )
		{
			Sound.FromWorld( "dm.pickup_weapon", ent.WorldPos );
			PickupFeed.OnPickup( player, $"{ent.ClassInfo.Title}" ); 
		}


		ItemRespawn.Taken( ent );
		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}
}

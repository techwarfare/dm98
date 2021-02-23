using Sandbox;
using System;
using System.Linq;

partial class DmInventory : BaseInventory
{
	public DmInventory( Player player ) : base ( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{

		(Owner as DeathmatchPlayer).GiveAmmo( AmmoType.Pistol, 2 );

		//
		// We don't want to pick up the same weapon twice
		// But we'll take the ammo from it Winky Face
		//
		if ( ent is BaseDmWeapon dmw && IsCarryingType( ent.GetType() ) )
		{
			var ammo = dmw.AmmoClip;
			var ammoType = dmw.AmmoType;

			Log.Info( $"{ammo} = {ammoType}" );

			if ( ammo > 0 )
			{
				(Owner as DeathmatchPlayer).GiveAmmo( ammoType, ammo );
				Log.Info( $"{Owner} GiveAmmo {ammo} = {ammoType}" );
			}

			// Despawn it
			ent.Delete();
			return false;
		}

		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}
}

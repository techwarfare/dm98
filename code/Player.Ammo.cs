using Sandbox;
using System;
using System.Linq;

partial class DeathmatchPlayer
{
	[Net]
	public int[] Ammo;

	public void ClearAmmo()
	{
		Ammo = new int[16];
	}

	public int AmmoCount( AmmoType type )
	{
		if ( Ammo == null ) return -5;

		return Ammo[(int)type];
	}

	public void GiveAmmo( AmmoType type, int amount )
	{
		if ( Ammo == null ) return;

		Ammo[(int)type] += amount;
	}

	public int TakeAmmo( AmmoType type, int amount )
	{
		//if ( Ammo == null ) return 0;

		amount = Math.Min( Ammo[(int)type], amount );

		Ammo[(int)type] -= amount;
		return amount;
	}
}

public enum AmmoType
{
	Pistol,
	Buckshot,
	Crossbow
}
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

partial class BaseDmWeapon : BaseWeapon
{

	[Net]
	public int AmmoClip { get; set; }

	public virtual AmmoType AmmoType => AmmoType.Pistol;
	public virtual int ClipSize => 16;
	public virtual float ReloadTime => 3.0f;

	[Net]
	public TimeSince TimeSinceReload { get; set; }

	[Net]
	public bool IsReloading { get; set; }


	public int AvailableAmmo()
	{
		var owner = Owner as DeathmatchPlayer;
		return owner.AmmoCount( AmmoType );
	}

	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
	}

	public override void Reload( Player owner )
	{
		if ( IsClient || IsReloading )
			return;

		TimeSinceReload = 0;

		using ( Prediction.Off() )
		{
			if ( Owner  is DeathmatchPlayer player )
			{
				if ( player.AmmoCount( AmmoType ) <= 0 )
					return;

				StartReloadEffects();
			}

			IsReloading = true;
			StartReloadEffects();
		}
	}

	public override void OnPlayerControlTick( Player owner )
	{
		if ( !IsReloading )
		{
			base.OnPlayerControlTick( owner );
		}

		if ( IsServer && IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( Owner is DeathmatchPlayer player )
		{
			var ammo = player.TakeAmmo( AmmoType, ClipSize - AmmoClip );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;
		}
	}

	[Client]
	public virtual void StartReloadEffects()
	{
		Log.Info( $"START RELOAD {ViewModelEntity}" );
		ViewModelEntity?.SetAnimParam( "reload", true );

		// TODO - player third person model reload
	}

	public override void AttackPrimary( Player owner )
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 5000 ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so aany exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damage = DamageInfo.FromBullet( tr.EndPos, owner.EyeRot.Forward * 100, 15 )
					.UsingTraceResult( tr )
					.WithAttacker( owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damage );
			}
		}
	}

	[Client]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		PlaySound( "rust_pistol.shoot" );
		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParam( "fire", true );
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
	{
		var forward = Owner.EyeRot.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	[Client]
	public virtual void DryFire()
	{
		// CLICK
	}

}

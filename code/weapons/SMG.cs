using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

partial class SMG : BaseDmWeapon
{ 
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 30;
	public override float ReloadTime => 4.0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
		AmmoClip = 20;
	}

	public override void AttackPrimary( Player owner )
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

	//	if ( !TakeAmmo( 1 ) )
		{
		//	DryFire();
		//	return;
		}

		Owner.SetAnimParam( "b_attack", true );
		//Owner.SetAnimParam( "b_jump", true );

		//Owner.SetAnimParam( "b_attack" );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );

	}

	public override void AttackSecondary( Player owner )
	{
		// Grenade lob
	}

	[Client]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		PlaySound( "rust_smg.shoot" );
		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParam( "fire", true ); 
	}

	public override void TickPlayerAnimator( PlayerAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}

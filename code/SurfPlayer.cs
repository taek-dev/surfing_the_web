using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace surfgame
{
	partial class SurfPlayer : Player
	{
		public ModelEntity Board { get; set; }
		[Net] public float Vel { get; set; }
		[Net] public float FadeAlpha { get; set; }
		public bool MusicActive { get; set; }
		public SurfTunes Tunes { get; set; }

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			if ( Board == null )
			{
				Board = new ModelEntity();
				Board.SetModel( "models/surfboard.vmdl" );
				Board.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
				Board.PhysicsBody.GravityEnabled = false;
				Board.Owner = this;
			}

			Dress();

			//Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new SurfCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			WaterLevel.Clear();

			SurfGame.Current?.MoveToSpawnpoint( this );
			ResetInterpolation();

			Board.Position = Position;
			Board.Rotation = Rotation;
			SetParent( Board );
			Position += Vector3.Up * 1f;
			Board.Velocity = Rotation.Forward * 5f;
		}

		float TargetMultiplyer;
		float Multiplyer;
		Rotation OldInputRot;

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			FadeAlpha = FadeAlpha.LerpTo( 0f, Time.Delta * 1f );

			Multiplyer = Multiplyer.LerpTo( TargetMultiplyer, Time.Delta * 5f );

			if ( IsServer && Board != null )
			{
				if ( Input.Pressed( InputButton.Attack2 ) )
				{
					OldInputRot = Input.Rotation;
				}

				Vector3 vel = Vector3.Zero;

				if ( !Input.Down( InputButton.Attack2 ) )
				{
					Board.Rotation = Rotation.Lerp( Board.Rotation, Rotation.From( Input.Rotation.Pitch(), Input.Rotation.Yaw(), Board.Rotation.Roll() ), Time.Delta * 5f );
					vel = (Input.Rotation.Forward * Input.Forward) + (Input.Rotation.Left * Input.Left);
				}
				else
				{
					vel = (OldInputRot.Forward * Input.Forward) + (OldInputRot.Left * Input.Left);
				}

				Board.Rotation = Rotation.Lerp( Board.Rotation, Rotation.From( Rotation.Pitch(), Rotation.Yaw(), 25f * -Input.Left ), Time.Delta * 1f );
				Vel = Board.Velocity.Length;

				vel = vel.Normal * 600;

				if ( Input.Down( InputButton.Jump ) )
				{
					vel += Vector3.Up * 200f;
				}

				if ( Input.Down( InputButton.Duck ) )
				{
					vel += Vector3.Up * -200f;
				}

				if ( Input.Down( InputButton.Run ) )
				{
					TargetMultiplyer = 5f;
				}
				else
				{
					TargetMultiplyer = 1f;
				}

				Board.Velocity += vel * Multiplyer * Time.Delta;

				// slow the player down
				Board.Velocity = Vector3.Lerp( Velocity, 0, Time.Delta * 0.6f );
				Board.PhysicsBody.AngularVelocity = Board.PhysicsBody.AngularVelocity.LerpTo( 0f, Time.Delta * 3f );
			}
		}

		[ClientRpc]
		public void PlayTunes()
		{
			_ = PlayTunesAsync( 1f );
		}

		private async Task PlayTunesAsync( float delay )
		{
			await GameTask.DelaySeconds( delay );
			if ( !MusicActive )
			{
				Tunes = SurfTunes.FromScreen( "lowearthorbit" );
				MusicActive = true;
			}
		}

		public override void FrameSimulate( Client cl )
		{
			var cam = Camera as SurfCamera;

			var t = MathX.LerpInverse( Vel, 1000, 4000 );
			var fov = MathX.LerpTo( 70, 130, t );
			cam.FieldOfView = cam.FieldOfView.LerpTo( fov, Time.Delta );

			base.FrameSimulate( cl );
		}

		public void MovePlayer()
		{
			Board.Position = new Vector3( 14696f, -15000f, -7000f );

			FadeAlpha = 1f;
		}

		public override void OnKilled()
		{
			base.OnKilled();
			Parent = null;
			Board?.Delete();
			Board = null;

			EnableDrawing = false;
		}

		ModelEntity pants;
		ModelEntity jacket;
		ModelEntity shoes;
		ModelEntity hat;

		bool dressed = false;

		public void Dress()
		{
			if ( dressed ) return;
			dressed = true;

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				//"models/citizen/clothes/trousers_tracksuit.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
			} );

				pants = new ModelEntity();
				pants.SetModel( model );
				pants.SetParent( this, true );
				pants.EnableShadowInFirstPerson = true;
				pants.EnableHideInFirstPerson = true;

				SetBodyGroup( "Legs", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			} );

				jacket = new ModelEntity();
				jacket.SetModel( model );
				jacket.SetParent( this, true );
				jacket.EnableShadowInFirstPerson = true;
				jacket.EnableHideInFirstPerson = true;

				var propInfo = jacket.GetModel().GetPropData();
				if ( propInfo.ParentBodyGroupName != null )
				{
					SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );
				}
				else
				{
					SetBodyGroup( "Chest", 0 );
				}
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			} );

				shoes = new ModelEntity();
				shoes.SetModel( model );
				shoes.SetParent( this, true );
				shoes.EnableShadowInFirstPerson = true;
				shoes.EnableHideInFirstPerson = true;

				SetBodyGroup( "Feet", 1 );
			}

			if ( true )
			{
				hat = new ModelEntity();
				hat.SetModel( "models/hd_glasses.vmdl" );
				hat.Position = ((Transform)GetAttachment( "hat" )).Position + new Vector3( 7.2f, 0.2f, -8.5f );
				hat.Rotation += Rotation.FromYaw( 90f );
				hat.SetParent( this, "hat" );

				SetBodyGroup( "Hat", 1 );
			}
		}
	}
}

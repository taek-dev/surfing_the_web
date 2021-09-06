using Sandbox;
using System;
using System.Linq;

namespace surfgame
{
	partial class SurfCamera : Camera
	{
		public SurfCamera()
		{
			FieldOfView = 40f;
		}

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Pos = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + Vector3.Up * 64;

			Pos = center;
			Rot = Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

			float distance = 170.0f * pawn.Scale;
			targetPos = Pos + Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 35) * pawn.Scale);
			targetPos += Input.Rotation.Forward * -distance;

			var tr = Trace.Ray( Pos, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					.Run();

			Pos = tr.EndPos;

			Viewer = null;
		}
	}
}

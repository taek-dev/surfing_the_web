
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace surfgame
{
	public partial class SurfGame : Game
	{	
		public static new SurfGame Current => Game.Current as SurfGame;

		public SurfGame()
		{
			if ( IsServer )
				new SurfHud();
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new SurfPlayer();
			client.Pawn = player;

			player.Respawn();
			player.PlayTunes(To.Single(player));
		}

		public override void MoveToSpawnpoint( Entity pawn )
		{
			var spawnpoint = Entity.All
									.OfType<SurfSpawn>()               // get all SpawnPoint entities
									.OrderBy( x => Guid.NewGuid() )     // order them by random
									.FirstOrDefault();                  // take the first one

			if ( spawnpoint == null )
			{
				Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
				return;
			}

			pawn.Transform = spawnpoint.Transform;
		}
	}

}

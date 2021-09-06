using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace surfgame
{
	[Library( "trigger_surf_teleport" )]
	public partial class Teleporter : BaseTrigger
	{
		public override void StartTouch( Entity other )
		{
			if ( other is ModelEntity board )
			{
				(board.Owner as SurfPlayer)?.MovePlayer();
			}

			base.StartTouch( other );
		}
	}
}

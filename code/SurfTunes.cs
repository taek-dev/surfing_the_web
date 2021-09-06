
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
	public partial class SurfTunes
	{
		private Sound song;
		private float Volume = 1;

		public static SurfTunes FromScreen( string song )
		{
			return new( Sound.FromScreen( song ) );
		}

		private SurfTunes( Sound song )
		{
			this.song = song;
		}
	}
}

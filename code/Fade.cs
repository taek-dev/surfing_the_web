using Sandbox.UI;
using Sandbox;

namespace surfgame
{
	public partial class Fade : Panel
	{
		public Fade()
		{
			StyleSheet.Load( "fade.scss" );
		}

		public override void Tick()
		{
			var pawn = Local.Pawn as SurfPlayer;

			Style.Set( "background-color", $"rgba(4, 255, 255, {(pawn)?.FadeAlpha})" );
		}
	}

}

using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace surfgame
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class SurfHud : Sandbox.HudEntity<RootPanel>
	{
		public SurfHud()
		{
			if ( IsClient )
			{
				RootPanel.AddChild<Fade>();
				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<NameTags>();
				RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();


				RootPanel.AddChild<VoiceList>();
			}
		}
	}

}

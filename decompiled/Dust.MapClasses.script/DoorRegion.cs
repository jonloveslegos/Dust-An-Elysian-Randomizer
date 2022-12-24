using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses.script
{
	public class DoorRegion
	{
		private int doorType;

		private Rectangle region;

		private Vector2 destLocation;

		private string destPath;

		public int DoorType => this.doorType;

		public Rectangle Region => this.region;

		public Vector2 DestLocation => this.destLocation;

		public string DestPath => this.destPath;

		public DoorRegion(int _doorType, Rectangle _region, Vector2 _destLoc, string _destPath)
		{
			this.doorType = _doorType;
			this.region = _region;
			this.destLocation = _destLoc;
			this.destPath = _destPath;
		}

		public bool EnterDoor(Character[] c, Map map, ParticleManager pMan, bool entering)
		{
			if (c[0].State == CharState.Grounded && !entering && this.region.Contains((int)c[0].Location.X, (int)c[0].Location.Y) && !Game1.events.anyEvent && Game1.stats.playerLifeState == 0)
			{
				Game1.hud.InitFidgetPrompt(FidgetPrompt.EnterDoor);
				if (Game1.hud.KeyUp && !c[0].AnimName.StartsWith("crouch"))
				{
					Game1.hud.KeyUp = false;
					map.InitDoor(this.destLocation, this.destPath, c);
					switch (this.doorType)
					{
					case 1:
						Sound.PlayCue("key_insert");
						Sound.PlayCue("chest_open");
						VibrationManager.Rumble(Game1.currentGamePad, 0.7f);
						break;
					case 2:
						Sound.PlayCue("door_crawl");
						VibrationManager.Rumble(Game1.currentGamePad, 0.7f);
						break;
					}
					return true;
				}
			}
			return false;
		}
	}
}

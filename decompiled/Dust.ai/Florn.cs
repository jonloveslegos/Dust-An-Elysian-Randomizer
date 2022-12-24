using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Florn : AI
	{
		public Florn(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 200;
			c.Speed = 120f;
			c.JumpVelocity = 1400f;
			c.Strength = 60;
			c.DefaultHeight = 200;
			c.DefaultWidth = 160;
			c.RandomTextures = 2;
			c.FlyType = FlyingType.CanFlySwaying;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f, 1f);
			base.jobType = JobType.FlyShoot;
			switch (Game1.map.regionName)
			{
			case "cave":
			case "farm":
				base.PrepareStats(c, 2f);
				break;
			default:
				base.PrepareStats(c, 1f);
				break;
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (c[ID].AnimName.Contains("attack"))
			{
				c[ID].MaskGlow += (1f - c[ID].MaskGlow) * Game1.FrameTime;
			}
			else
			{
				c[ID].MaskGlow += (0f - c[ID].MaskGlow) * Game1.FrameTime;
			}
			if (c[ID].DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
			{
				Sound.PlayCue("florn_chirp", c[ID].Location, (c[0].Location - c[ID].Location).Length());
			}
			base.Update(c, ID, map);
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			pMan.AddElectricBolt(loc, -1, Rand.GetRandomFloat(0.1f, 0.3f), 600, 60, 0.3f, 6);
		}
	}
}

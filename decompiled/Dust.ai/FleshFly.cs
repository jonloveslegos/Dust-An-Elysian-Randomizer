using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	public class FleshFly : AI
	{
		public FleshFly(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 600;
			c.Strength = 50;
			c.Speed = 400f;
			c.JumpVelocity = 1400f;
			c.DefaultHeight = 180;
			c.DefaultWidth = 180;
			c.RandomTextures = 1;
			c.FlyType = FlyingType.CanFlySwaying;
			c.DieType = DyingTypes.BodyVanish;
			c.DamageType = DamageTypes.NotTeam;
			c.SpawnType = SpawnTypes.NoEffect;
			c.maxAnimFrames = 2;
			c.MaskGlow = 0.5f;
			c.defaultColor = new Color(1f, Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
			base.PrepareStats(c, 1f);
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
			if (me.DyingFrame > -1f || me.AnimName.StartsWith("hurt"))
			{
				base.MapCollision(map, c, me);
			}
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			Sound.PlayCue("fleshfly_die", base.me.Location, (Game1.character[0].Location - base.me.Location).Length());
			pMan.AddPlayerDebris(base.me.Location + new Vector2(0f, -80f), Rand.GetRandomVector2(-400f, 400f, -1200f, -600f), texture, new Rectangle(60, 54, 50, 56), 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			for (int i = 0; i < 2; i++)
			{
				pMan.AddPlayerDebris(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), Rand.GetRandomVector2(-400f, 400f, -1200f, -600f), texture, new Rectangle(180, 44, 58, 60), 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			}
			for (int j = 0; j < 3; j++)
			{
				pMan.AddPlayerDebris(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), Rand.GetRandomVector2(-400f, 400f, -1200f, -600f), texture, new Rectangle(902, 2, 30, 30), 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			}
			for (int k = 0; k < 3; k++)
			{
				pMan.AddPlayerDebris(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), Rand.GetRandomVector2(-400f, 400f, -1200f, -600f), texture, new Rectangle(906, 38, 19, 19), 1f, Rand.GetRandomFloat(0.75f, 1.5f), 1, base.me.defaultColor.R, base.me.defaultColor.G, base.me.defaultColor.B, 5);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 50) == 0)
			{
				Sound.PlayCue("fleshfly_idle", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (map.path == "grave13")
			{
				if (base.me.Location.X < 2100f)
				{
					base.me.Trajectory.X = 400f;
				}
				if (base.me.Location.Y < 2700f)
				{
					base.me.Trajectory.Y = 400f;
				}
			}
			if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.2f)
				{
					base.randomInt = Rand.GetRandomInt(100, 500);
					base.jobType = JobType.FlyShoot;
					base.jobFrame = Rand.GetRandomFloat(1f, 2f);
				}
				else
				{
					base.jobType = JobType.DartAround;
					base.FaceToTarg(c);
				}
			}
			base.Update(c, ID, map);
		}
	}
}

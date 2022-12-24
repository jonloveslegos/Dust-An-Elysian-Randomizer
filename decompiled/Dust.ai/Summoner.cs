using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Summoner : AI
	{
		public Summoner(Character c)
		{
			Sound.PlayFollowCharCue("summoner_idle", c.ID);
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 2400;
			c.Strength = 60;
			c.Speed = 240f;
			c.JumpVelocity = 1400f;
			c.Aggression = 10;
			c.DefaultHeight = 400;
			c.DefaultWidth = 250;
			c.RandomTextures = 1;
			c.maxAnimFrames = 20;
			c.FlyType = FlyingType.CanFly;
			c.LiftType = CanLiftType.SmallLift;
			c.defaultColor = Color.White;
			base.jobType = JobType.FlyShoot;
			c.MaskGlow = 1.5f;
			string regionName;
			if ((regionName = Game1.map.regionName) != null && regionName == "trial")
			{
				base.PrepareStats(c, 0.2f);
			}
			else
			{
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
			{
				Sound.PlayCue("florn_chirp", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			base.jobType = JobType.FlyKeepDistance;
			base.randomInt = 500;
			base.FaceToTarg(c);
			base.Update(c, ID, map);
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
		}

		public override bool CanAttackAI()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 1; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists == CharExists.Exists)
				{
					num++;
					if (Game1.character[i].renderable)
					{
						num2++;
					}
					if (num2 > 10)
					{
						return false;
					}
				}
			}
			if (num2 < 10 && num < Game1.character.Length)
			{
				return true;
			}
			return false;
		}

		public override bool CheckHit(Particle p)
		{
			if (base.me.DyingFrame > -1f)
			{
				return false;
			}
			if (p != null && p is Hit)
			{
				Vector2 vector = base.me.Location + new Vector2(0f, -200f);
				VibrationManager.SetScreenShake(1f);
				VibrationManager.SetBlast(0.5f, vector);
				for (int i = 0; i < 15; i++)
				{
					Game1.pManager.AddSmoke(base.me.Location + Rand.GetRandomVector2(-100f, 100f, -300f, 100f), Rand.GetRandomVector2(-100f, 100f, -200f, -20f), 0f, 0f, 0f, 0.5f, Rand.GetRandomFloat(1f, 2f), 1f, 6);
				}
				Character[] character = Game1.character;
				float num = base.me.Location.X - character[0].Location.X;
				base.me.Location.X = character[0].Location.X - num;
				base.me.Location.X = ((base.me.Location.X < character[0].Location.X) ? (character[0].Location.X - 500f) : (character[0].Location.X + 500f));
				base.me.Location.Y += 100f;
				base.me.ResetRotation();
				base.FaceToTarg(character);
				base.me.SetAnim("warp", 0, 0);
				for (int j = 0; j < 8; j++)
				{
					Game1.pManager.AddVerticleBeam(base.me.Location - new Vector2(Rand.GetRandomInt(-80, 80), 300f), Rand.GetRandomVector2(-100f, 100f, -10f, 10f), 1f, 0.2f, 1f, 1f, Rand.GetRandomInt(50, 150), 1500, Rand.GetRandomFloat(0.2f, 1f), base.me.ID, 5);
				}
				Game1.pManager.AddShockRing(vector, 0.6f, 5);
				return false;
			}
			return true;
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			switch (trig)
			{
			case Trigger.Special0:
				pMan.AddStarburst(loc, Rand.GetRandomFloat(0.05f, 0.2f), 1f, 1f, 0.5f, 0.2f, Rand.GetRandomFloat(0.2f, 0.8f), base.me.ID, background: false, 5);
				break;
			case Trigger.Special1:
			{
				for (int l = 0; l < 3; l++)
				{
					pMan.AddStarburst(loc, Rand.GetRandomFloat(0.5f, 1.5f), 1f, 0.5f, 1f, 0.2f, Rand.GetRandomFloat(0.1f, 0.6f), base.me.ID, background: false, 5);
				}
				break;
			}
			case Trigger.Special2:
			{
				Vector2 location = Game1.character[base.targ].Location;
				int num = 200;
				int num2 = Rand.GetRandomInt(0, 10);
				CharacterType type;
				if (num2 < 2)
				{
					type = CharacterType.RemainsBomb;
					num = 400;
				}
				else
				{
					type = ((Rand.GetRandomInt(0, 2) == 0) ? CharacterType.Remains : CharacterType.RemainsHalf);
				}
				Vector2 vector = location + new Vector2(num * ((loc.X < location.X) ? 1 : (-1)), -100f);
				Game1.events.SpawnCharacter(vector, "enemy", type, Team.Enemy, ground: true);
				bool flag = false;
				do
				{
					Vector2 vector2 = vector;
					vector.Y += 16f;
					if (Game1.map.CheckPCol(vector + new Vector2(0f, 40f), vector2 + new Vector2(0f, 40f), canFallThrough: false, init: true) > 0f)
					{
						flag = true;
					}
				}
				while (!flag && vector.Y < Game1.map.bottomEdge);
				pMan.AddSpawnBeam(vector, 300f, 6);
				pMan.AddShockRing(vector, 0.4f, 6);
				VibrationManager.SetScreenShake(1f);
				VibrationManager.SetBlast(0.5f, vector);
				break;
			}
			case Trigger.Special3:
			{
				pMan.AddShockRing(loc, 0.4f, 6);
				VibrationManager.SetBlast(0.5f, loc);
				for (int i = 0; i < 5; i++)
				{
					Game1.pManager.AddSmoke(loc + Rand.GetRandomVector2(-100f, 100f, -200f, 300f), Rand.GetRandomVector2(-100f, 100f, -200f, -20f), 1f, 1f, 0f, 1f, Rand.GetRandomFloat(1f, 4f), 1f, 6);
				}
				for (int j = 0; j < 5; j++)
				{
					Game1.pManager.AddSmoke(loc + Rand.GetRandomVector2(-100f, 100f, -200f, 300f), Rand.GetRandomVector2(-100f, 100f, -200f, -20f), 1f, 0f, 1f, 1f, Rand.GetRandomFloat(1f, 4f), 1f, 6);
				}
				for (int k = 0; k < 5; k++)
				{
					Game1.pManager.AddSmoke(loc + Rand.GetRandomVector2(-100f, 100f, -200f, 300f), Rand.GetRandomVector2(-100f, 100f, -200f, -20f), 0f, 1f, 1f, 1f, Rand.GetRandomFloat(1f, 4f), 1f, 6);
				}
				break;
			}
			}
		}
	}
}

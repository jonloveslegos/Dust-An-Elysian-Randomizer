using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	public class FleshFlyHive : AI
	{
		public FleshFlyHive(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 2400;
			c.Speed = 20f;
			c.DefaultHeight = 300;
			c.DefaultWidth = 300;
			c.FlyType = FlyingType.CanFlySwaying;
			c.LiftType = CanLiftType.Immovable;
			c.DieType = DyingTypes.BodyVanish;
			c.MaskGlow = 1.5f;
			base.jobFrame = 1f;
			base.PrepareStats(c, 1f);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.renderable && base.jobFrame < 0f)
			{
				if (!base.me.AnimName.StartsWith("spawn"))
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
						}
					}
					if (num2 < 20 && num < c.Length - 1)
					{
						base.me.SetAnim("spawn0" + Rand.GetRandomInt(0, 3), 0, 1);
					}
				}
				base.jobFrame = Rand.GetRandomFloat(0.1f, 3f);
			}
			base.Update(c, ID, map);
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			Vector2 vector = base.me.Location + new Vector2(0f, -100f);
			pMan.AddShockRing(vector, 1f, 5);
			VibrationManager.SetScreenShake(0.75f);
			VibrationManager.SetBlast(0.75f, vector);
			Game1.map.MapSegFrameSpeed = 0.4f;
			Sound.PlayCue("destruct_die");
			int num = ((!(base.me.Location.X > Game1.character[0].Location.X)) ? 1 : (-1));
			for (int i = 0; i < 20; i++)
			{
				pMan.AddBlood(vector + Rand.GetRandomVector2(-150f, 150f, -200f, 200f), new Vector2((float)num * Rand.GetRandomFloat(-400f, -40f), Rand.GetRandomFloat(-600f, -200f)), 1f, 1f, 1f, 1f, 2.2f, (CharacterType)1004, 0, Rand.GetRandomInt(5, 7));
			}
			for (int j = 0; j < 4; j++)
			{
				Game1.events.SpawnCharacterAppend(vector, "enemy", CharacterType.FleshFly, Team.Enemy, ground: false);
			}
		}

		public override bool CheckHit(Particle p)
		{
			base.me.SetAnim("hurt0" + Rand.GetRandomInt(0, 2), 0, 0);
			return true;
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			if (trig == Trigger.Special0)
			{
				Game1.events.SpawnCharacterAppend(loc, "enemy", CharacterType.FleshFly, Team.Enemy, ground: false);
			}
		}
	}
}

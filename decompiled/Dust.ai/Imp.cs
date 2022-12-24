using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Imp : AI
	{
		public Imp(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.Speed = 300f;
			c.JumpVelocity = 1400f;
			c.DefaultHeight = 160;
			c.DefaultWidth = 160;
			c.MaxDownTime = 10f;
			c.DamageType = DamageTypes.Everyone;
			if (c.Definition.charType == CharacterType.Imp)
			{
				c.MaxHP = 30;
				c.Strength = 6;
				c.Aggression = 5;
				c.RandomTextures = 2;
				c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
				switch (Game1.map.regionName)
				{
				case "forest":
				case "smith":
					base.PrepareStats(c, 2f);
					break;
				default:
					base.PrepareStats(c, 1f);
					break;
				}
			}
			else
			{
				c.MaxHP = 2000;
				c.Strength = 200;
				c.Aggression = 50;
				c.defaultColor = new Color(1f, Rand.GetRandomFloat(0.8f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 300) == 0)
			{
				Sound.PlayCue("impsnort", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.jobFrame < 0f)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 1f);
				if (randomFloat < 0.4f)
				{
					base.jobType = JobType.MeleeChaseLunging;
					base.jobFrame = Rand.GetRandomFloat(2f, 4f);
				}
				else if (randomFloat < 0.7f)
				{
					base.FaceToTarg(c);
					base.jobType = JobType.Idle;
					base.jobFrame = Rand.GetRandomFloat(0.5f, 2f);
				}
				else
				{
					base.jobType = JobType.Avoid;
					base.jobFrame = Rand.GetRandomFloat(0.2f, 1f);
					base.FaceToTarg(c);
				}
			}
			base.Update(c, ID, map);
		}
	}
}

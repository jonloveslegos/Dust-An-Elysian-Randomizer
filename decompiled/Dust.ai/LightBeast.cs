using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class LightBeast : AI
	{
		public LightBeast(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.JumpVelocity = 2000f;
			c.DefaultHeight = 220;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1f;
			c.DamageType = DamageTypes.Everyone;
			c.RandomWeapons = 3;
			if (c.Definition.charType == CharacterType.LightBeast)
			{
				c.RandomTextures = 3;
				c.MaxHP = 40;
				c.Strength = 10;
				c.Speed = 500f;
				c.Aggression = 40;
				c.defaultColor = new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f);
				switch (Game1.map.regionName)
				{
				case "trial":
					base.PrepareStats(c, 4f);
					break;
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
				c.RandomTextures = 2;
				c.MaxHP = 6000;
				c.Strength = 250;
				c.Speed = 620f;
				c.Aggression = 60;
				c.defaultColor = new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, 1f);
				base.PrepareStats(c, 1f);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.DyingFrame == -1f && Rand.GetRandomInt(0, 200) == 0)
			{
				Sound.PlayCue("lightchirp", base.me.Location, (c[0].Location - base.me.Location).Length());
			}
			if (base.me.Definition.charType == CharacterType.LightBeastSnow)
			{
				base.DodgeAttack(c, base.targ, 300f, base.me.Speed * 2f, base.me.JumpVelocity / 3f, faceTarg: true, base.me.Aggression / 2);
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

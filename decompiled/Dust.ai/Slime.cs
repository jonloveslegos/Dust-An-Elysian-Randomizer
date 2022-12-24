using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	public class Slime : AI
	{
		public Slime(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 300;
			c.Speed = 300f;
			c.JumpVelocity = 1400f;
			c.Strength = 32;
			c.DefaultHeight = 300;
			c.DefaultWidth = 300;
			c.ShadowWidth = 1.5f;
			c.Aggression = 40;
			c.LiftType = CanLiftType.SmallLift;
			c.RandomTextures = 2;
			c.MaxDownTime = 3f;
			c.defaultColor = new Color(Rand.GetRandomFloat(0.25f, 1f), Rand.GetRandomFloat(0.25f, 1f), 1f, 1f);
			switch (Game1.map.regionName)
			{
			case "grave":
				c.Strength = 60;
				base.PrepareStats(c, 4f);
				break;
			case "cave":
			case "farm":
				base.PrepareStats(c, 3f);
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

		public override void Update(Character[] c, int ID, Map map)
		{
			base.me = c[ID];
			if (Game1.longSkipFrame > 3)
			{
				base.me.DefaultWidth = 300;
				if (base.me.AnimName == "attackup")
				{
					base.me.DefaultHeight = 600;
					base.me.DefaultWidth = 2;
				}
				else if (base.me.DownTime > 0f)
				{
					base.me.DefaultHeight = 160;
				}
				else
				{
					base.me.DefaultHeight = 300;
				}
				if (base.me.DyingFrame == -1f)
				{
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						Sound.PlayCue("slimegrowl", c[ID].Location, (c[0].Location - c[ID].Location).Length());
					}
				}
				else
				{
					Game1.pManager.MakeBlood(base.me.Location + Rand.GetRandomVector2(-70f, 70f, -200f, -100f), Rand.GetRandomVector2(-40f, 40f, -60f, -20f), base.me.Definition.charType, 0, base.me.defaultColor, 5);
				}
			}
			if (base.targ > -1)
			{
				if (c[base.targ].Location.Y < base.me.Location.Y - 100f && base.me.Location.X < c[base.targ].Location.X + 100f && base.me.Location.X > c[base.targ].Location.X - 100f && base.me.State == CharState.Grounded)
				{
					base.jobType = JobType.AttackUp;
					base.jobFrame = 0.1f;
				}
			}
			else
			{
				base.targ = base.FindTarg(c);
			}
			if (base.jobFrame < 0f)
			{
				base.jobType = JobType.MeleeChaseLunging;
				base.jobFrame = Rand.GetRandomFloat(2f, 4f);
			}
			base.Update(c, ID, map);
		}
	}
}

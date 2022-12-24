using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Strings;
using Dust.NavClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Upgrade : Particle
	{
		private bool stopped;

		private float growTime;

		private float secGrowTime = 0.5f;

		private float moveY;

		private float frame;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public Upgrade(Vector2 loc, int id, int audioID)
		{
			this.Reset(loc, id, audioID);
		}

		public void Reset(Vector2 loc, int id, int audioID)
		{
			base.exists = Exists.Init;
			this.stopped = false;
			base.location = (this.ploc = loc);
			base.owner = id - 2000;
			this.frame = 10f;
			base.renderState = RenderState.AdditiveOnly;
			this.renderRect = new Rectangle((int)(-200f * Game1.hiDefScaleOffset), (int)(-200f * Game1.hiDefScaleOffset), Game1.screenWidth + (int)(400f * Game1.hiDefScaleOffset), Game1.screenHeight + (int)(600f * Game1.hiDefScaleOffset));
			Sound.PlayFollowParticleCue("upgrade_hum", audioID, 5);
			base.exists = Exists.Exists;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 newLocation, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = newLocation - vector;
			float num = Math.Abs(vector2.X + vector2.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, 1f - num);
			sprite.End();
			sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector3 = vector - vector2 * ((float)i / 2f - 1.75f);
				Rectangle value;
				Vector2 origin;
				Vector2 vector4;
				float rotation;
				switch (i)
				{
				case 1:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				case 2:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				case 3:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.25f, 0.25f);
					rotation = 0f;
					break;
				case 5:
					value = new Rectangle(3437, 1860, 560, 150);
					origin = new Vector2(280f, 100f);
					vector4 = new Vector2(1f, 2f) * (num + 0.75f);
					rotation = GlobalFunctions.GetAngle(vector3, vector) + 1.57f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				default:
					value = new Rectangle(3978, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], vector3, value, color, rotation, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], newLocation, new Rectangle(2940, 1860, 497, 150), Color.White, 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		private void UpdateEvent(Character[] c)
		{
			Game1.awardsManager.EarnAchievement(Achievement.Upgraded, forceCheck: false);
			switch (base.owner)
			{
			case 0:
				Game1.events.InitEvent(22, isSideEvent: false);
				break;
			case 1:
				Game1.events.InitEvent(102, isSideEvent: false);
				break;
			case 2:
				break;
			case 10:
				Game1.events.InitEvent(6, isSideEvent: false);
				break;
			case 12:
				break;
			case 14:
				break;
			case 15:
				break;
			case 16:
				Game1.events.InitEvent(345, isSideEvent: false);
				break;
			case 17:
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 11:
			case 13:
				break;
			}
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (c[0].InHitBounds(new Vector2(base.location.X, base.location.Y - 300f)) && Game1.stats.playerLifeState == 0)
			{
				this.frame = 0f;
				Game1.stats.GetChestFromFile("Ability "+base.owner.ToString(),pMan);
				foreach (RevealMap value in Game1.navManager.RevealMap.Values)
				{
					int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, base.owner + 2000, null);
					if (itemArrayID > -1)
					{
						value.GameItemList[itemArrayID].Stage = 1;
					}
				}
				this.UpdateEvent(c);
				Game1.hud.LevelUpEffect(pMan);
				Game1.stats.curCharge = 100f;
			}
			if (this.frame <= 0f)
			{
				base.Reset();
				Game1.navManager.CheckRegionTreasure(pMan);
			}
			if (!this.stopped)
			{
				if (map.CheckCol(base.location) > 0)
				{
					base.location.X = this.ploc.X;
					base.trajectory.X = 0f - base.trajectory.X;
				}
				if (Math.Abs(base.trajectory.X) > 2f)
				{
					base.trajectory.X /= 1.04f;
				}
				else
				{
					base.trajectory.X = 0f;
				}
				if (base.trajectory.Y < 1500f)
				{
					base.trajectory.Y += Game1.FrameTime * 1000f;
				}
				if (base.trajectory.Y < 0f)
				{
					if (map.CheckCol(base.location + new Vector2(0f, -32f)) > 0 || base.location.Y < map.topEdge)
					{
						base.trajectory.Y = 0f;
						base.location.Y = this.ploc.Y;
					}
				}
				else
				{
					float num = map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) + map.CheckSpecialLedge(base.location, this.ploc, LedgeFlags.SpecialPath);
					if (num > 0f)
					{
						base.location.Y = num;
						if (base.trajectory.Y > 400f)
						{
							if (this.renderRect.Contains((int)vector.X, (int)vector.Y))
							{
								Sound.PlayDropCue("coin_bounce", base.location, base.trajectory.Y);
							}
							base.trajectory.Y = (0f - base.trajectory.Y) / 2f;
						}
						else
						{
							base.trajectory.Y = 0f;
							this.stopped = true;
						}
					}
				}
			}
			else
			{
				base.trajectory.X = 0f;
			}
			this.moveY = (float)Math.Sin((double)Game1.map.MapSegFrameLocked * 30.0) * 10f * Game1.worldScale - 300f;
			this.growTime += gameTime;
			if (this.growTime > 1f)
			{
				this.growTime = 0f;
			}
			this.secGrowTime += gameTime * 0.5f;
			if (this.secGrowTime > 1f)
			{
				this.secGrowTime = 0f;
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			if (this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				pMan.AddUpgradeBurn(base.location + new Vector2(0f, this.moveY), 0.8f, l);
				if (Rand.GetRandomInt(0, 20) == 0)
				{
					Vector2 loc = base.location + new Vector2(0f, this.moveY) + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
					int randomInt = Rand.GetRandomInt(12, 48);
					float randomFloat = Rand.GetRandomFloat(0.1f, 1f);
					pMan.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, randomFloat, randomInt, l);
				}
				if (Rand.GetRandomInt(0, 4) == 0)
				{
					pMan.AddVerticleBeam(base.location - new Vector2(0f, 300f), Rand.GetRandomVector2(-100f, 100f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 0.05f, Rand.GetRandomInt(50, 150), Rand.GetRandomInt(500, 800), Rand.GetRandomFloat(0.2f, 1f), -1, 6);
				}
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l) + new Vector2(0f, this.moveY) * worldScale;
			this.DrawCorona(sprite, particlesTex, vector, l);
			sprite.Draw(particlesTex[2], vector, new Rectangle(base.owner * 125, 2796, 125, 125), Color.Cyan, 0f, new Vector2(62f, 62f), worldScale, SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[2], vector, new Rectangle(base.owner * 125, 2796, 125, 125), new Color(1f, 1f, 1f, 1f - this.growTime), 0f, new Vector2(62f, 62f), worldScale + this.growTime / 4f * worldScale, SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[2], vector, new Rectangle(base.owner * 125, 2796, 125, 125), new Color(1f, 1f, 1f, 1f - this.secGrowTime), 0f, new Vector2(62f, 62f), worldScale + this.secGrowTime / 4f * worldScale, SpriteEffects.None, 1f);
			if (Game1.debugging)
				sprite.DrawString(Game1.font, "Ability " + base.owner.ToString(), vector - new Vector2(0, 200), Color.White);
		}
	}
}

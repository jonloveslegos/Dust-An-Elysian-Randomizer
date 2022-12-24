using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Chest : Particle
	{
		private float animFrameTime;

		private float glowFrame;

		private byte animFrame;

		private byte openMode;

		private int lidOffset;

		private SpriteEffects flip;

		private Rectangle sRectLid;

		public Chest(Vector2 loc, int id, bool faceRight)
		{
			this.Reset(loc, id, faceRight);
		}

		public void Reset(Vector2 loc, int id, bool faceRight)
		{
			base.exists = Exists.Init;
			this.animFrameTime = 0f;
			this.glowFrame = 0f;
			this.animFrame = 0;
			this.openMode = 0;
			this.lidOffset = 0;
			this.sRectLid = new Rectangle(316, 2716, 316, 80);
			base.maskGlow = 1.5f;
			base.location = loc;
			base.owner = id;
			base.background = true;
			if (faceRight)
			{
				this.flip = SpriteEffects.FlipHorizontally;
			}
			else
			{
				this.flip = SpriteEffects.None;
			}
			if (Game1.navManager.RevealMap[Game1.navManager.NavPath].ChestList[base.owner].Stage > 0)
			{
				base.flag = 100;
				this.openMode = 3;
				this.sRectLid = new Rectangle(316, 2596, 316, 120);
				if (this.flip == SpriteEffects.FlipHorizontally)
				{
					this.lidOffset = -50;
				}
				else
				{
					this.lidOffset = 50;
				}
			}
			bool flag = false;
			do
			{
				Vector2 vector = base.location;
				base.location.Y += 16f;
				if (Game1.map.CheckPCol(base.location + new Vector2(0f, 40f), vector + new Vector2(0f, 40f), canFallThrough: false, init: true) > 0f)
				{
					flag = true;
				}
			}
			while (!flag && base.location.Y < Game1.map.bottomEdge);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.maskGlow = 2f;
			if (this.openMode == 0)
			{
				Vector2 vector = base.GameLocation(l);
				if (new Rectangle(-400, -400, Game1.screenWidth + 800, Game1.screenHeight + 800).Contains((int)vector.X, (int)vector.Y))
				{
					if (Rand.GetRandomInt(0, 10) < 2)
					{
						Vector2 loc = base.location + Rand.GetRandomVector2(-180f, 180f, -240f, 40f);
						int randomInt = Rand.GetRandomInt(12, 48);
						float randomFloat = Rand.GetRandomFloat(0.2f, 0.75f);
						Game1.pManager.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, randomFloat, randomInt, 5);
					}
					if (c[0].Location.Y < base.location.Y + 200f && c[0].Location.Y > base.location.Y - 300f)
					{
						if (c[0].Location.X < base.location.X + 400f && c[0].Location.X > base.location.X - 400f)
						{
							Game1.events.InitEvent(0, isSideEvent: true);
						}
						if (c[0].State == CharState.Grounded && c[0].Location.X < base.location.X + 150f && c[0].Location.X > base.location.X - 150f)
						{
							Game1.hud.InitFidgetPrompt(FidgetPrompt.OpenTreasure);
							if (Game1.hud.unlockState > 0)
							{
								if (Game1.hud.UpdateUnlocking(pMan, updating: true))
								{
									this.openMode = 1;
									this.animFrameTime = 0f;
									base.flag = 100;
									Game1.stats.Equipment[305]--;
									Game1.navManager.RevealMap[Game1.navManager.NavPath].ChestList[base.owner].Stage = 1;
									Game1.navManager.CheckRegionTreasure(pMan);
									Game1.awardsManager.EarnAchievement(Achievement.OpenChest, forceCheck: false);
								}
							}
							else if (Game1.hud.KeyUpOpen && !Game1.events.anyEvent)
							{
								Game1.hud.KeyUpOpen = false;
								if (Game1.stats.Equipment[305] > 0)
								{
									Sound.PlayCue("key_insert");
									VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
									Game1.hud.InitUnlocking(0, base.location.X);
								}
								else
								{
									Sound.PlayCue("shop_fail");
								}
							}
						}
					}
				}
			}
			else if (this.openMode == 1)
			{
				if (c[0].State == CharState.Grounded && c[0].Location.X < base.location.X + 150f && c[0].Location.X > base.location.X - 150f && c[0].Location.Y < base.location.Y + 200f && c[0].Location.Y > base.location.Y - 200f)
				{
					Game1.hud.InitFidgetPrompt(FidgetPrompt.NoLookUp);
				}
				this.animFrameTime -= Game1.FrameTime;
				if (this.animFrameTime < 0f)
				{
					Sound.PlayCue("chest_open");
					VibrationManager.SetScreenShake(0.5f);
					this.openMode = 2;
					this.animFrame = 0;
					this.animFrameTime = 0f;
					this.sRectLid = new Rectangle(0, 1560, 158, 100);
					Game1.stats.OpenChest(new Vector2(base.location.X, base.location.Y - 220f), Game1.pManager, Game1.navManager.RevealMap[Game1.navManager.NavPath].ChestList[base.owner].UniqueID);
					pMan.AddGroundDust(new Vector2(base.location.X, base.location.Y - 120f), Vector2.Zero, 0.5f, 2f, 0, 5);
					pMan.AddGroundDust(new Vector2(base.location.X, base.location.Y - 120f), Vector2.Zero, 1f, 2f, 0, 5);
				}
			}
			else if (this.openMode == 2)
			{
				if (this.animFrame > 4)
				{
					if (this.flip == SpriteEffects.FlipHorizontally)
					{
						this.lidOffset = -50;
					}
					else
					{
						this.lidOffset = 50;
					}
				}
				this.sRectLid = new Rectangle(158 * this.animFrame, 1560, 158, 100);
				this.animFrameTime += Game1.FrameTime * 24f;
				if (this.animFrameTime > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 8)
					{
						this.openMode = 3;
						this.sRectLid = new Rectangle(316, 2596, 316, 120);
						if (this.flip == SpriteEffects.FlipHorizontally)
						{
							this.lidOffset = -50;
						}
						else
						{
							this.lidOffset = 50;
						}
					}
					this.animFrameTime = 0f;
				}
			}
			if (this.openMode < 2)
			{
				this.glowFrame += Game1.HudTime * 2f;
				if (this.glowFrame > 6.28f)
				{
					this.glowFrame -= 6.28f;
				}
			}
			else
			{
				this.glowFrame = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (new Rectangle(-400, -400, Game1.screenWidth + 800, Game1.screenHeight + 800).Contains((int)vector.X, (int)vector.Y))
			{
				if (this.openMode == 3)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(this.lidOffset, -230f) * worldScale, this.sRectLid, Color.White, 0f, new Vector2(158f, 0f), worldScale, this.flip, 1f);
				}
				sprite.Draw(particlesTex[2], vector, new Rectangle(0, 2596, 316, 200), Color.White, 0f, new Vector2(158f, 170f), worldScale, this.flip, 1f);
				if (this.openMode == 2)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(this.lidOffset, -305f) * worldScale, this.sRectLid, Color.White, 0f, new Vector2(79f, 0f), 2f * worldScale, this.flip, 1f);
				}
				else if (this.openMode < 2)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(this.lidOffset, -185f) * worldScale, this.sRectLid, Color.White, 0f, new Vector2(158f, 0f), worldScale, this.flip, 1f);
				}
				if (this.glowFrame > 0f)
				{
					sprite.End();
					sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
					Color color = new Color(1f, 1f, 1f, Math.Abs((float)Math.Sin(this.glowFrame)));
					sprite.Draw(particlesTex[2], vector, new Rectangle(0, 2596, 316, 200), color, 0f, new Vector2(158f, 170f), worldScale, this.flip, 1f);
					sprite.Draw(particlesTex[2], vector + new Vector2(this.lidOffset, -185f) * worldScale, this.sRectLid, color, 0f, new Vector2(158f, 0f), worldScale, this.flip, 1f);
					sprite.End();
					sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				}
				if (Game1.debugging)
					sprite.DrawString(Game1.font, Game1.navManager.RevealMap[Game1.navManager.NavPath].ChestList[base.owner].UniqueID, vector-new Vector2(0,200), Color.White);
			}
		}
	}
}

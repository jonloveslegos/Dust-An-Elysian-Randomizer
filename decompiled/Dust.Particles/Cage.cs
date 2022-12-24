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
	internal class Cage : Particle
	{
		private float animFrameTime;

		private float glowFrame;

		private float bumpScale;

		private byte animFrame;

		private byte openMode;

		public Cage(Vector2 loc, int id)
		{
			this.Reset(loc, id);
		}

		public void Reset(Vector2 loc, int id)
		{
			base.exists = Exists.Init;
			this.animFrameTime = 0f;
			this.glowFrame = 0f;
			this.bumpScale = 1f;
			this.animFrame = 0;
			this.openMode = 0;
			base.maskGlow = 1.5f;
			base.location = loc;
			base.owner = id;
			base.background = true;
			base.renderState = RenderState.Additive;
			if (Game1.navManager.RevealMap[Game1.navManager.NavPath].CageList[base.owner].Stage > 0)
			{
				base.flag = 100;
				this.openMode = 3;
				base.renderState = RenderState.Normal;
			}
			bool flag = false;
			do
			{
				Vector2 vector = base.location;
				base.location.Y += 16f;
				if (Game1.map.CheckPCol(base.location + new Vector2(0f, 60f), vector + new Vector2(0f, 60f), canFallThrough: false, init: true) > 0f)
				{
					flag = true;
				}
			}
			while (!flag && base.location.Y < Game1.map.bottomEdge);
			base.exists = Exists.Exists;
		}

		private void Spawn(CharacterType type)
		{
			Game1.events.InitEvent(41, isSideEvent: true);
			Game1.hud.InitMiniPrompt(MiniPromptType.FriendRescued, 0, blueprint: false);
			Vector2 newLoc = new Vector2(base.location.X, base.location.Y - 100f);
			Character[] character = Game1.character;
			for (int i = 0; i < character.Length; i++)
			{
				if (character[i].Exists == CharExists.Dead)
				{
					character[i].NewCharacter(newLoc, Game1.charDef[(int)type], i, type.ToString(), Team.Friendly, ground: false);
					character[i].SetJump(1500f, jumped: true);
					break;
				}
			}
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
							Game1.events.InitEvent(2, isSideEvent: true);
						}
						if (c[0].State == CharState.Grounded && c[0].Location.X < base.location.X + 200f && c[0].Location.X > base.location.X - 200f)
						{
							Game1.hud.InitFidgetPrompt(FidgetPrompt.OpenCage);
							if (Game1.hud.unlockState > 0)
							{
								if (Game1.hud.UpdateUnlocking(pMan, updating: true))
								{
									this.openMode++;
									this.animFrameTime = 1f;
									this.bumpScale = 0f;
									base.flag = 100;
									Game1.stats.Equipment[305] -= 4;
									Game1.navManager.RevealMap[Game1.navManager.NavPath].CageList[base.owner].Stage = 1;
									Game1.navManager.CheckRegionTreasure(pMan);
									Game1.stats.GetWorldExplored();
									Game1.awardsManager.EarnAchievement(Achievement.OpenCage, forceCheck: false);
									Sound.PlayCue("cage_open_buildup_part01");
								}
							}
							else if (Game1.hud.KeyUpOpen && !Game1.events.anyEvent)
							{
								Game1.hud.KeyUpOpen = false;
								if (Game1.stats.Equipment[305] >= 4)
								{
									Sound.PlayCue("key_insert");
									VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
									Game1.hud.InitUnlocking(1, base.location.X);
								}
								else
								{
									Sound.PlayCue("shop_fail");
								}
							}
						}
					}
				}
				this.animFrameTime -= Game1.FrameTime;
				if (this.animFrameTime < 0f)
				{
					this.animFrameTime += Rand.GetRandomInt(0, 4);
					this.bumpScale = 1.1f;
					Sound.PlayCue("cage_bump", base.location, (base.location - c[0].Location).Length() / 2f);
					pMan.AddGroundDust(new Vector2(base.location.X, base.location.Y - (float)Rand.GetRandomInt(0, 160)), Vector2.Zero, 0.25f, 0.5f, 0, 5);
				}
				if (this.bumpScale > 0f)
				{
					this.bumpScale = Math.Max(this.bumpScale - Game1.FrameTime * 2f, 0f);
				}
			}
			else if (this.openMode < 3)
			{
				Game1.hud.InitFidgetPrompt(FidgetPrompt.NoLookUp);
				this.animFrameTime -= Game1.FrameTime;
				if (this.openMode == 1 && this.animFrameTime < 0.1f)
				{
					this.bumpScale = 1f;
				}
				if (this.animFrameTime < 0f)
				{
					if (this.openMode == 1)
					{
						this.animFrameTime = 2f;
						VibrationManager.SetScreenShake(0.5f);
						VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
						Sound.PlayCue("cage_unchain", base.location, (base.location - c[0].Location).Length() / 2f);
						for (int i = 0; i < 40; i++)
						{
							pMan.AddChainLink(base.location + Rand.GetRandomVector2(-50f, 50f, -200f, 0f), Rand.GetRandomVector2(-500f, 500f, -800f, -50f), l);
						}
					}
					else
					{
						Sound.PlayCue("cage_open", base.location, (base.location - c[0].Location).Length() / 2f);
						Sound.PlayCue("cage_open_buildup_part02");
						this.Spawn(Game1.stats.GetFriend(Game1.navManager.RevealMap[Game1.navManager.NavPath].CageList[base.owner].UniqueID));
						pMan.AddGroundDust(base.location + new Vector2(0f, 30f), Vector2.Zero, 0.5f, 1f, 0, 5);
						Game1.navManager.RevealCell("sanc01");
					}
					this.bumpScale = 0f;
					this.animFrame = 0;
					this.openMode++;
				}
				if (this.animFrame < 1 && this.openMode == 2 && c[0].State == CharState.Grounded && c[0].Location.X < base.location.X + 200f && c[0].Location.X > base.location.X - 200f)
				{
					CharDir face = c[0].Face;
					if (c[0].Location.X < base.location.X)
					{
						c[0].Face = CharDir.Right;
					}
					else
					{
						c[0].Face = CharDir.Left;
					}
					c[0].Slide(-300f);
					if (c[0].Face != face)
					{
						c[0].SetAnim("idle00", 0, 0);
						c[0].SetAnim("runend", 0, 2);
					}
				}
			}
			else if (this.openMode == 3)
			{
				this.animFrameTime += Game1.FrameTime * 24f;
				if (this.animFrameTime > 1f)
				{
					this.animFrameTime = 0f;
					this.animFrame++;
					if (this.animFrame > 8)
					{
						this.openMode++;
					}
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
			Vector2 vector = (base.location - new Vector2(185f, 0f)) * Game1.worldScale - Game1.Scroll;
			if (new Rectangle(-600, -600, Game1.screenWidth + 1200, Game1.screenHeight + 1200).Contains((int)vector.X, (int)vector.Y) && (!Game1.pManager.renderingAdditive || this.glowFrame > 0f))
			{
				Color color = ((!Game1.pManager.renderingAdditive) ? Color.White : (Color.White * ((float)Math.Sin(this.glowFrame) / 2f)));
				sprite.Draw(particlesTex[2], vector, new Rectangle(1150, 1000, 371, 240), color, 0f, new Vector2(0f, 206f), new Vector2(1f, Math.Max(this.bumpScale, 1f)) * worldScale, SpriteEffects.None, 1f);
				if (this.openMode < 2)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(76f, 0f) * worldScale, new Rectangle(1521 + ((this.bumpScale > 0.9f) ? 205 : 0), 1000, 205, 240), color, 0f, new Vector2(0f, 206f), new Vector2(1f, Math.Max(this.bumpScale, 1f)) * worldScale, SpriteEffects.None, 1f);
				}
				else if (this.openMode == 2)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(110f, 0f) * worldScale, new Rectangle(1931, 1000, 128, 240), color, 0f, new Vector2(0f, 206f), worldScale, SpriteEffects.None, 1f);
				}
				else if (this.openMode == 3)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(66f, 44f) * worldScale, new Rectangle(800 + 90 * this.animFrame, 2970, 90, 96), color, 0f, new Vector2(0f, 103f), 2f * worldScale, SpriteEffects.None, 1f);
				}
				else if (this.openMode == 4)
				{
					sprite.Draw(particlesTex[2], vector + new Vector2(60f, 0f) * worldScale, new Rectangle(2059, 1000, 73, 240), color, 0f, new Vector2(0f, 206f), worldScale, SpriteEffects.None, 1f);
				}
			}
		}
	}
}

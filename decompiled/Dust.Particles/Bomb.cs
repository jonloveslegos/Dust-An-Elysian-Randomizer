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
	internal class Bomb : Particle
	{
		private int timer;

		private int bombStage;

		private int animFrame;

		private int audioID;

		private int boosting;

		private float animFrameTime;

		private float length;

		private float targetRot;

		private float frame;

		private float size;

		private float rotation;

		private float glowFrame;

		private Vector2 pLoc;

		private Rectangle renderRect;

		public Bomb(Vector2 loc, int ID, int _audioID, int _timer, int stage)
		{
			this.Reset(loc, ID, _audioID, _timer, stage);
		}

		public void Reset(Vector2 loc, int ID, int _audioID, int _timer, int stage)
		{
			base.exists = Exists.Init;
			this.pLoc = (base.location = loc);
			base.trajectory = Vector2.Zero;
			base.flag = ID;
			base.owner = -1;
			base.strength = 1000;
			this.timer = _timer;
			this.audioID = _audioID;
			this.frame = 0f;
			this.rotation = (this.targetRot = 0f);
			base.isSpun = 0f;
			base.maskGlow = 0f;
			this.glowFrame = 0f;
			this.size = 0f;
			base.renderState = RenderState.Additive;
			base.background = true;
			this.bombStage = stage;
			if (stage == 3)
			{
				this.pLoc = (base.location = Game1.character[0].Location + new Vector2(0f, (Game1.character[0].State == CharState.Grounded) ? (-320) : 0));
				base.trajectory = base.location + new Vector2((Game1.character[0].State == CharState.Grounded) ? ((Game1.character[0].Face == CharDir.Left) ? (-500) : 500) : 0, 0f);
				Sound.PlayFollowParticleCue("bomb_hover", this.audioID, 5);
				this.length = (this.size = 1f);
				base.maskGlow = 3f;
				this.frame = (this.timer = _timer + 1);
			}
			base.exists = Exists.Exists;
		}

		private bool CheckDestructs()
		{
			if (Game1.dManager.CheckBombCol(base.location))
			{
				return true;
			}
			return false;
		}

		private bool CheckEnemies(Character[] c, Map map, Vector2 gameLoc, int l)
		{
			Rectangle bounds = new Rectangle((int)base.location.X - 75, (int)base.location.Y - 10, 150, 140);
			for (int i = 1; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Team == Team.Enemy && c[i].Ethereal != EtherealState.Ethereal && c[i].DyingFrame == -1f && c[i].InRectBounds(bounds) && c[i].GrabbedBy == -1)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateRegions(Map map, float gameTime)
		{
			this.boosting = -1;
			for (int i = 0; i < map.boostRegions.Count; i++)
			{
				if (map.boostRegions[i] != null && base.location.X > (float)map.boostRegions[i].Region.X && base.location.X < (float)(map.boostRegions[i].Region.X + map.boostRegions[i].Region.Width) && base.location.Y > (float)map.boostRegions[i].Region.Y && base.location.Y < (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height))
				{
					this.boosting = map.boostRegions[i].Direction;
					switch (this.boosting)
					{
					case 0:
					{
						float num4 = MathHelper.Clamp((float)map.boostRegions[i].Region.Height - Math.Abs(base.location.Y - (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height)), 10f, 500f);
						base.trajectory.Y -= num4 * 20f * gameTime;
						break;
					}
					case 3:
					{
						float num3 = ((float)map.boostRegions[i].Region.Width - (base.location.X - (float)map.boostRegions[i].Region.X)) / (float)map.boostRegions[i].Region.Width * 1000f;
						base.trajectory.X += num3 * 20f * gameTime;
						break;
					}
					case 6:
					{
						float num2 = ((float)map.boostRegions[i].Region.Height - (base.location.Y - (float)map.boostRegions[i].Region.Y)) / (float)map.boostRegions[i].Region.Height * 1000f;
						base.trajectory.Y += num2 * 20f * gameTime;
						break;
					}
					case 9:
					{
						float num = (base.location.X - (float)map.boostRegions[i].Region.X) / (float)map.boostRegions[i].Region.Width * 1000f;
						base.trajectory.X -= num * 20f * gameTime;
						break;
					}
					}
				}
			}
		}

		private void UpdateCollision(Map map)
		{
			int num = 0;
			float num2 = map.CheckPCol(base.location + new Vector2(0f, 50f), this.pLoc, canFallThrough: true, init: false);
			if (num2 > 0f)
			{
				base.location.Y = num2 - 50f;
				base.trajectory.Y = this.pLoc.Y - 150f;
				num = map.CheckCol(base.location + new Vector2(0f, 50f));
			}
			if (base.location.Y < this.pLoc.Y)
			{
				int num3 = map.CheckCol(base.location);
				if (num3 > 0 || base.location.Y < map.topEdge)
				{
					base.location.Y = this.pLoc.Y;
					base.trajectory.Y = this.pLoc.Y + 150f;
					num = num3;
				}
			}
			if (base.location.X < this.pLoc.X)
			{
				int num4 = map.CheckCol(base.location - new Vector2(20f, 0f));
				if (num4 > 0)
				{
					base.location.X = this.pLoc.X;
					base.trajectory.X = this.pLoc.X + 150f;
					num = num4;
				}
			}
			else if (base.location.X > this.pLoc.X)
			{
				int num5 = map.CheckCol(base.location + new Vector2(20f, 0f));
				if (num5 > 0)
				{
					base.location.X = this.pLoc.X;
					base.trajectory.X = this.pLoc.X - 150f;
					num = num5;
				}
			}
			if (num == 3)
			{
				this.bombStage = 3;
				this.frame = 0f;
			}
		}

		public override void GetInfo(ref int intVar, ref float floatVar)
		{
			intVar = this.bombStage;
			floatVar = this.frame;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 gameLoc = base.GameLocation(l);
			if (this.bombStage >= 2)
			{
				this.UpdateRegions(map, gameTime);
				if (!Game1.events.anyEvent && Game1.hud.unlockState == 0)
				{
					float num = this.frame;
					this.frame -= gameTime;
					if (this.frame < 11f)
					{
						int num2 = (int)MathHelper.Clamp(6f - this.frame, 1f, 6f);
						if (this.frame * (float)num2 % 1f < 0.9f && num * (float)num2 % 1f >= 0.9f && this.bombStage > 1 && this.bombStage < 4)
						{
							Sound.PlayCue("bomb_beep", base.location, (base.location - c[0].Location).Length() / 4f);
						}
					}
				}
			}
			if (this.bombStage > 3)
			{
				base.maskGlow = this.frame;
			}
			if (this.bombStage < 2)
			{
				this.glowFrame += gameTime;
			}
			else
			{
				this.glowFrame += gameTime * Math.Max(2f, (float)this.timer / this.frame * 2f);
			}
			if (this.glowFrame > 6.28f)
			{
				this.glowFrame -= 6.28f;
			}
			if (this.bombStage < 4)
			{
				bool flag = c[0].AnimName == "attack01" || c[0].AnimName == "airspin";
				float x = (float)Math.Sin((double)map.MapSegFrame * 2.0 + (double)base.flag) * 150f;
				float y = (float)Math.Sin((double)map.MapSegFrame * 4.0 + (double)base.flag) * 150f;
				this.animFrameTime += gameTime * 24f;
				if (this.animFrameTime > 1f)
				{
					this.animFrame++;
					if (this.animFrame > 2)
					{
						this.animFrame = 0;
					}
					this.animFrameTime -= 1f;
				}
				if (this.bombStage == 0)
				{
					if (this.frame == 0f && map.GetTransVal() <= 0f)
					{
						Sound.PlayCue("bomb_grow", base.location, (base.location - c[0].Location).Length() / 2f);
					}
					float num3 = (float)Math.Cos(Game1.hud.pulse * 8f) + 2.5f;
					this.length = (this.size = num3 * 0.5f * (1f - this.frame) + 1f * this.frame);
					this.frame += gameTime;
					base.maskGlow = MathHelper.Clamp(base.maskGlow + gameTime * 10f, 0f, 4f);
					if (this.frame > 1f || map.GetTransVal() > 0f)
					{
						this.frame = 0f;
						base.maskGlow = 4f;
						this.length = (this.size = 1f);
						this.bombStage++;
					}
				}
				else if (this.bombStage == 1)
				{
					this.length += (this.size + this.frame - this.length) * gameTime * 2f;
					this.renderRect = new Rectangle(-200, -200, Game1.screenWidth + 400, Game1.screenHeight + 220);
					if (flag && (this.renderRect.Contains((int)gameLoc.X, (int)gameLoc.Y) || (base.location - c[0].Location).Length() < 1200f))
					{
						if (Game1.longSkipFrame > 3)
						{
							Sound.PlayCue("bomb_pull", base.location, (base.location - c[0].Location).Length());
						}
						Vector2 scroll = Game1.Scroll;
						scroll += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(l)) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
						scroll /= Game1.worldScale;
						scroll += ((c[0].Location + new Vector2((c[0].Face == CharDir.Left) ? (-240) : 240, -240f)) * Game1.worldScale - Game1.Scroll) / Game1.worldScale;
						if (scroll.X < base.location.X && scroll.Y < base.location.Y)
						{
							this.targetRot = GlobalFunctions.GetAngle(base.location, scroll) + 1.57f;
						}
						else
						{
							this.targetRot = GlobalFunctions.GetAngle(base.location, scroll) - 4.71f;
						}
						base.isSpun += gameTime * 20f;
						if (base.isSpun > 6.28f)
						{
							base.isSpun -= 6.28f;
						}
						this.targetRot += (float)Math.Cos(base.isSpun) * 1f;
						this.frame += gameTime;
						if (this.frame > 0.75f)
						{
							Sound.PlayFollowParticleCue("bomb_hover", this.audioID, l);
							this.bombStage++;
							this.frame = 100f;
							base.background = false;
							base.trajectory = base.location;
							Sound.PlayCue("bomb_pluck", base.location, (base.location - c[0].Location).Length());
							for (int i = 0; i < 6; i++)
							{
								pMan.AddBounceSpark(base.location, Rand.GetRandomVector2(-400f, 400f, -200f, 0f), 0.2f, 6);
							}
						}
					}
					else
					{
						this.frame = MathHelper.Clamp(this.frame - gameTime * 1f, 0f, 1f);
						base.isSpun += gameTime;
						if (base.isSpun > 6.28f)
						{
							base.isSpun -= 6.28f;
						}
						this.targetRot += (float)(Math.Cos(base.isSpun + base.location.X) * 0.10000000149011612 - (double)this.rotation) * gameTime * 3f;
					}
					this.rotation += (this.targetRot - this.rotation) * gameTime * 3f;
				}
				else
				{
					if (flag)
					{
						Vector2 scroll2 = Game1.Scroll;
						scroll2 += new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - 1f / base.LayerScale(l)) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
						scroll2 /= Game1.worldScale;
						float num4 = 200f;
						if ((base.location.X < c[0].Location.X + 200f && c[0].Face == CharDir.Right) || (base.location.X > c[0].Location.X - 200f && c[0].Face == CharDir.Left))
						{
							num4 = 460f;
						}
						scroll2 += ((c[0].Location + new Vector2((c[0].Face == CharDir.Left) ? (0f - num4) : num4, -240f)) * Game1.worldScale - Game1.Scroll) / Game1.worldScale;
						scroll2.X += Rand.GetRandomInt(-200, 200);
						if (this.boosting != 3 && this.boosting != 9)
						{
							base.trajectory.X += (scroll2.X - base.trajectory.X) * gameTime * 20f;
						}
						if (this.boosting != 0 && this.boosting != 6)
						{
							base.trajectory.Y += (scroll2.Y - base.trajectory.Y) * gameTime * 20f;
						}
						if (this.bombStage == 2)
						{
							this.bombStage = 3;
							this.frame = this.timer;
						}
					}
					this.pLoc = base.location;
					base.location += (base.trajectory + new Vector2(x, y) - base.location) * gameTime * 2f;
					this.UpdateCollision(map);
					this.targetRot = MathHelper.Clamp((base.location.X - this.pLoc.X) / 2f, -1f, 1f);
					this.rotation += (this.targetRot - this.rotation) * gameTime * 2f;
					if (this.frame < 1f)
					{
						this.size = MathHelper.Clamp(this.size + Rand.GetRandomFloat(-0.1f, 0.1f), 0.8f, 1.2f);
					}
					this.length += (this.size - this.length) * gameTime * 4f;
				}
				if (this.bombStage != 3 || ((Game1.skipFrame <= 1 || !(map.GetTransVal() < 1f) || (!this.CheckDestructs() && !this.CheckEnemies(c, map, gameLoc, l))) && !(this.frame < 0f)))
				{
					return;
				}
				Vector2 vector = new Vector2(base.location.X, base.location.Y + 70f);
				bool flag2 = new Rectangle(-400, -400, Game1.screenWidth + 800, Game1.screenHeight + 800).Contains((int)gameLoc.X, (int)gameLoc.Y);
				if (flag2)
				{
					Game1.pManager.AddShockRing(vector, 1f, 5);
					VibrationManager.SetBlast(1.5f, vector);
					for (int j = 0; j < 20; j++)
					{
						pMan.AddExplosion(vector + Rand.GetRandomVector2(-200f, 200f, -200f, 250f), Rand.GetRandomFloat(1.5f, 2.5f), (Rand.GetRandomInt(0, 3) != 0) ? true : false, 6);
					}
				}
				CharDir dir = ((!(base.location.X < this.pLoc.X)) ? CharDir.Right : CharDir.Left);
				Game1.dManager.CheckBombKill(new Vector2(base.location.X - 150f, base.location.Y - 80f), dir);
				Game1.dManager.CheckBombKill(new Vector2(base.location.X + 150f, base.location.Y - 80f), dir);
				Game1.dManager.CheckBombKill(new Vector2(base.location.X - 150f, base.location.Y + 210f), dir);
				Game1.dManager.CheckBombKill(new Vector2(base.location.X + 150f, base.location.Y + 210f), dir);
				if (flag2)
				{
					for (int k = 0; k < 20; k++)
					{
						Game1.pManager.AddLavaDrip(vector, Rand.GetRandomVector2(-800f, 800f, -700f, 10f), Rand.GetRandomFloat(0.1f, 0.8f), 6);
						if (k < 5)
						{
							pMan.AddBlood(vector, Rand.GetRandomVector2(-400f, 400f, -800f, -400f), 1f, 1f, 1f, 1f, 0.4f, (CharacterType)1000, 0, 5);
						}
					}
				}
				float num5 = (base.location - c[0].Location).Length() / 300f;
				Game1.worldDark = MathHelper.Max(Game1.worldDark - 2f / num5, 0f);
				VibrationManager.SetScreenShake(MathHelper.Min(6f / num5, 2f));
				Game1.map.MapSegFrameSpeed = 0.4f;
				Sound.PlayCue("bomb_explode", base.location, (base.location - c[0].Location).Length() / 4f);
				int enemiesDefeated = Game1.stats.enemiesDefeated;
				int num6 = base.flag;
				base.flag = -1;
				for (int m = 1; m < c.Length; m++)
				{
					if (c[m].Team != Team.Enemy)
					{
						continue;
					}
					c[m].CanHurtFrame = (c[m].CanHurtProjectileFrame = 0f);
					if (HitManager.CheckHitPossible(this, c, m, new Rectangle((int)base.location.X - 360, (int)base.location.Y - 330, 720, 800)))
					{
						base.owner = -1;
						if (HitManager.CheckIDHit(this, c, pMan, m) && c[m].LiftType == CanLiftType.Normal)
						{
							c[m].SetJump(Rand.GetRandomInt(1200, 3000), jumped: false);
						}
					}
				}
				base.flag = num6;
				if (Game1.stats.enemiesDefeated - enemiesDefeated >= 6)
				{
					Game1.awardsManager.EarnAchievement(Achievement.BombKills, forceCheck: false);
				}
				this.bombStage = 4;
				this.frame = 3f;
			}
			else if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = base.location + (base.location - Game1.character[0].Location) / 4f;
			_exists = this.bombStage < 4;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, float alpha, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = new Vector2((float)(0.0 - Math.Sin(this.rotation)) * 90f, (float)Math.Cos(this.rotation) * 90f) + base.location;
			Vector2 vector3 = vector2 * Game1.worldScale - Game1.Scroll - vector;
			float num = Math.Abs(vector3.X + vector3.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, (1f - num) * alpha);
			for (int i = 0; i < 5; i++)
			{
				Vector2 position = vector - vector3 * ((float)i / 2f - 1.75f);
				Rectangle value;
				Vector2 origin;
				Vector2 vector4;
				float num2;
				switch (i)
				{
				case 1:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					num2 = 0f;
					break;
				case 2:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, (0.5f - num) * alpha);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					num2 = 0f;
					break;
				case 3:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.25f, 0.25f);
					num2 = 0f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, (0.5f - num) * alpha);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					num2 = 0f;
					break;
				default:
					value = new Rectangle(3978, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					num2 = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], position, value, color, num2, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (this.bombStage >= 4)
			{
				return;
			}
			if (!Game1.pManager.renderingAdditive)
			{
				sprite.Draw(particlesTex[2], vector, new Rectangle(3648, 2328, 95, 128), Color.White, this.rotation, new Vector2(47f, 7f), worldScale * new Vector2(this.size, this.length), SpriteEffects.None, 1f);
				if (this.bombStage > 1)
				{
					sprite.Draw(particlesTex[2], vector, new Rectangle(3893, 2328 + this.animFrame * 32, 112, 32), Color.White, this.rotation / 3f, new Vector2(56f, 18f), worldScale, SpriteEffects.None, 1f);
					if (this.frame < 11f)
					{
						float alpha = Math.Min((11f - this.frame) * 5f, 0.75f);
						Vector2 vector2 = vector - new Vector2(20f, 80f);
						Game1.hud.DrawTextBubble(vector2, 100f, 0f, alpha);
						Game1.hud.scoreDraw.Draw((long)this.frame, vector2 + new Vector2(50f, 15f), 1f, Color.White, ScoreDraw.Justify.Center, 0);
					}
				}
			}
			else
			{
				float num = 1f - Math.Abs((float)Math.Sin(this.glowFrame));
				if (this.bombStage < 2)
				{
					num *= 0.5f;
				}
				sprite.Draw(particlesTex[2], vector, new Rectangle(3743, 2328, 150, 128), new Color(1f, 1f, 1f, num), this.rotation, new Vector2(73f, -23f), worldScale * new Vector2(this.size, this.length), SpriteEffects.None, 1f);
				if (this.bombStage > 1)
				{
					this.DrawCorona(sprite, particlesTex, num * (this.frame / (float)this.timer) * 2f, l);
				}
			}
		}
	}
}

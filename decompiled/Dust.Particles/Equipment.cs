using System;
using System.Diagnostics;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.NavClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Equipment : Particle
	{
		private int groundOffset;

		private byte itemCategory;

		private bool bluePrint;

		private bool stopped;

		private float pulseTime;

		private float animFrameTime;

		private int animFrame;

		private float frame;

		private float rotation;

		private string name;

		private Rectangle sRect;

		private Vector2 ploc = Vector2.Zero;

		public Equipment(Vector2 loc, Vector2 traj, int _flag, bool _bluePrint, int _treasureID, string _name)
		{
			this.Reset(loc, traj, _flag, _bluePrint, _treasureID, _name);
		}

		public void Reset(Vector2 loc, Vector2 traj, int _flag, bool _bluePrint, int _treasureID, string _uniqueName)
		{
			base.exists = Exists.Init;
			this.bluePrint = _bluePrint;
			this.frame = 20f;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			base.flag = _flag;
			if (Game1.inventoryManager.equipItem[base.flag] == null)
			{
				base.exists = Exists.Dead;
				base.Reset();
				return;
			}
			this.itemCategory = (byte)(base.flag / (int)Game1.inventoryManager.invSelMax);
			this.sRect = new Rectangle(this.itemCategory * 60, (base.flag - this.itemCategory * Game1.inventoryManager.invSelMax) * 60, 60, 60);
			this.stopped = false;
			this.name = _uniqueName;
			base.owner = _treasureID;
			if (base.owner > -1)
			{
				this.stopped = true;
			}
			if (base.owner > -1 || this.bluePrint)
			{
				this.groundOffset = 170;
			}
			else
			{
				this.groundOffset = 30;
			}
			this.pulseTime = Rand.GetRandomFloat(0f, 6.28f);
			this.animFrame = Rand.GetRandomInt(0, 100);
			this.animFrameTime = 0f;
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		private void Sparkle()
		{
			if (Rand.GetRandomInt(0, 5) == 0)
			{
				Vector2 loc = new Vector2(base.location.X + (float)Rand.GetRandomInt(-40, 40), base.location.Y + (float)Rand.GetRandomInt(-100, 60));
				int randomInt = Rand.GetRandomInt(12, 48);
				float randomFloat = Rand.GetRandomFloat(0.1f, 0.5f);
				Game1.pManager.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, randomFloat, randomInt, 5);
			}
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 gameLoc, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = gameLoc - vector;
			float num = Math.Abs(vector2.X + vector2.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, 1f - num);
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector3 = vector - vector2 * ((float)i / 2f - 1.75f);
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
					color = new Color(1f, 1f, 1f, 0.5f - num);
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
				case 5:
					value = new Rectangle(3437, 1860, 560, 150);
					origin = new Vector2(280f, 100f);
					vector4 = new Vector2(1f, 2f) * (num + 0.75f);
					num2 = GlobalFunctions.GetAngle(vector3, vector) + 1.57f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
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
				sprite.Draw(particlesTex[2], vector3, value, color, num2, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], gameLoc, new Rectangle(2940, 1860, 497, 150), Color.Gray, 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.GameLocation(l);
			bool flag = true;
			if (base.owner < 0 && !this.bluePrint)
			{
				if (pMan.Seek(c, this, 0, 60f, 1000f))
				{
					flag = false;
					this.stopped = false;
					if (base.isSpun < 0.02f)
					{
						base.trajectory.Y = -1000f;
					}
					base.trajectory.X = MathHelper.Clamp(base.trajectory.X, -3000f, 3000f);
					base.trajectory.Y = MathHelper.Clamp(base.trajectory.Y, -3000f, 3000f);
				}
				if (base.flag < Game1.inventoryManager.invSelMax && !Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None)
				{
					this.frame -= gameTime;
				}
			}
			if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c) && Game1.stats.playerLifeState == 0)
			{
				Sound.PlayCue("item_pickup");
				this.frame = 0f;
				if (Game1.debugging)
				{
					Debug.WriteLine(base.flag);
				}
				if (base.owner > -1)
				{
					Game1.stats.GetChestFromFile(this.name + "equip", pMan);
					if (base.owner >= 4000)
					{
						if (Game1.cManager.currentChallenge > -1)
						{
							Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].CompleteChallenge(base.owner, this.name);
						}
					}
					else
					{
						foreach (RevealMap value in Game1.navManager.RevealMap.Values)
						{
							int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, base.owner, this.name);
							if (itemArrayID > -1 && value.GameItemList[itemArrayID].UniqueID == this.name)
							{
								value.GameItemList[itemArrayID].Stage = 1;
								Game1.questManager.UpdateQuests(320);
								break;
							}
						}
					}
					Game1.navManager.CheckRegionTreasure(pMan);
				}
                else
                {
					Game1.hud.InitMiniPrompt((MiniPromptType)this.itemCategory, base.flag, this.bluePrint);
					if (!this.bluePrint)
					{
						Game1.stats.Equipment[base.flag]++;
						if (base.flag < Game1.inventoryManager.invSelMax && Game1.stats.currentItem == -1)
						{
							Game1.stats.currentItem = base.flag;
							Game1.events.InitEvent(20, isSideEvent: true);
						}
					}
					else if (base.flag >= Game1.inventoryManager.invSelMax)
					{
						Game1.stats.EquipBluePrint[base.flag - Game1.inventoryManager.invSelMax]++;
					}
				}
				Game1.questManager.UpdateQuests(0);
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
					base.trajectory.Y += Game1.FrameTime * 2000f;
				}
				if (base.trajectory.Y < 0f)
				{
					if (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge)
					{
						base.trajectory.Y = 0f;
						base.location.Y = this.ploc.Y;
					}
				}
				else
				{
					float num = map.CheckPCol(base.location + new Vector2(0f, this.groundOffset), this.ploc + new Vector2(0f, this.groundOffset), canFallThrough: false, init: false);
					if (num > 0f)
					{
						base.location.Y = num - (float)this.groundOffset;
						if (base.trajectory.Y > 250f)
						{
							if (flag)
							{
								Sound.PlayDropCue("item_bounce", base.location, base.trajectory.Y);
							}
							if (base.isSpun == 0f)
							{
								base.trajectory.Y = (0f - base.trajectory.Y) / 2f;
							}
							else
							{
								base.trajectory.Y = -1000f;
								base.isSpun = 0f;
							}
						}
						else
						{
							base.trajectory = Vector2.Zero;
							this.stopped = true;
						}
					}
				}
			}
			if (this.frame <= 0f)
			{
				base.Reset();
				if (this.frame == 0f)
				{
					pMan.AddLenseFlare(base.location, 0.4f, 1, 5);
				}
			}
			this.pulseTime += Game1.FrameTime;
			if (this.pulseTime > 6.28f)
			{
				this.pulseTime -= 6.28f;
			}
			this.animFrameTime += Game1.FrameTime * 48f;
			if (this.animFrameTime > 1f)
			{
				this.animFrameTime = 0f;
				this.animFrame++;
				if (this.animFrame > 100)
				{
					this.animFrame = 0;
				}
			}
			this.ploc = base.location;
			this.rotation = (float)Math.Sin(this.pulseTime) / 8f;
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			base.maskGlow = MathHelper.Clamp(this.frame, 0f, 1f);
			float num = MathHelper.Clamp(this.frame, 0f, 1f);
			Vector2 vector = (base.location + new Vector2(0f, (float)Math.Sin(this.pulseTime * 2f) * 20f)) * Game1.worldScale - Game1.Scroll;
			if (Game1.pManager.renderingAdditive)
			{
				if (base.owner > -1)
				{
					if (new Rectangle(-2000, -2000, Game1.screenWidth + 4000, Game1.screenHeight + 3000).Contains((int)vector.X, (int)vector.Y))
					{
						this.Sparkle();
					}
					this.DrawCorona(sprite, particlesTex, vector, l);
				}
				if (this.animFrame < 21)
				{
					sprite.Draw(particlesTex[2], vector, new Rectangle(1600 + this.animFrame * 60, 2970, 60, 80), Color.White * num, 0f, new Vector2(30f, 40f), 2f, SpriteEffects.None, 0f);
				}
			}
			if (!Game1.pManager.renderingAdditive || (this.pulseTime < 0.5f && (int)(this.pulseTime * 20f) % 2 == 0))
			{
				if (this.bluePrint)
				{
					sprite.Draw(particlesTex[1], vector, new Rectangle(0, 0, 102, 128), new Color(1f, 1f, 1f, num), this.rotation, new Vector2(52f, 60f), 1.2f * worldScale, SpriteEffects.None, 1f);
				}
				sprite.Draw(particlesTex[4], vector, this.sRect, new Color(1f, 1f, 1f, num), this.rotation, new Vector2(30f, 30f), 1.2f * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}

using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.NavClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Note : Particle
	{
		private bool stopped;

		private Rectangle renderRect;

		private float frame;

		private string name;

		public Note(Vector2 loc, Vector2 traj, int id, string _name)
		{
			this.Reset(loc, traj, id, _name);
		}

		public void Reset(Vector2 loc, Vector2 traj, int id, string _name)
		{
			base.exists = Exists.Init;
			this.stopped = false;
			base.maskGlow = 2f;
			this.frame = -1f;
			this.name = _name;
			base.location = loc;
			base.trajectory = traj;
			base.owner = id - 3000;
			base.renderState = RenderState.Additive;
			this.renderRect = new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight);
			bool flag = false;
			do
			{
				Vector2 vector = base.location;
				base.location.Y += 16f;
				if (Game1.map.CheckPCol(base.location + new Vector2(0f, 100f), vector + new Vector2(0f, 100f), canFallThrough: false, init: true) > 0f)
				{
					flag = true;
				}
			}
			while (!flag && base.location.Y < Game1.map.bottomEdge);
			base.exists = Exists.Exists;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 gameLoc, float alpha, int l)
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
				sprite.Draw(particlesTex[2], vector3, value, color * alpha, rotation, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], gameLoc, new Rectangle(2940, 1860, 497, 150), Color.White * alpha, 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (this.frame < 0f)
			{
				if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c) && Game1.stats.playerLifeState == 0)
				{
					Sound.PlayCue("note_pickup");
					Game1.questManager.AddNote(base.owner, loading: false);
					foreach (RevealMap value in Game1.navManager.RevealMap.Values)
					{
						int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, base.owner + 3000, this.name);
						if (itemArrayID > -1 && value.GameItemList[itemArrayID].UniqueID == this.name)
						{
							value.GameItemList[itemArrayID].Stage = 1;
							break;
						}
					}
					if (Game1.worldDark == 0f)
					{
						base.Reset();
						Game1.navManager.CheckRegionTreasure(pMan);
					}
					else
					{
						this.frame = 2f;
					}
					for (int i = 0; i < 20; i++)
					{
						Vector2 loc = new Vector2(base.location.X + (float)Rand.GetRandomInt(-80, 80), base.location.Y + (float)Rand.GetRandomInt(-100, 100));
						int randomInt = Rand.GetRandomInt(12, 48);
						float randomFloat = Rand.GetRandomFloat(0.5f, 1.25f);
						pMan.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, randomFloat, randomInt, 5);
					}
					Game1.questManager.UpdateQuests(0);
				}
			}
			else
			{
				base.maskGlow = this.frame;
				this.frame -= gameTime;
				if (this.frame <= 0f)
				{
					this.frame = 0f;
					base.Reset();
					Game1.navManager.CheckRegionTreasure(pMan);
				}
			}
			if (this.renderRect.Contains((int)vector.X, (int)vector.Y) && Rand.GetRandomInt(0, 5) == 0)
			{
				Vector2 loc2 = base.location + Rand.GetRandomVector2(-40f, 40f, -100f, 60f);
				int randomInt2 = Rand.GetRandomInt(12, 48);
				float randomFloat2 = Rand.GetRandomFloat(0.1f, 0.5f);
				pMan.AddSparkle(loc2, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, randomFloat2, randomInt2, 5);
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.frame < 0f)
			{
				Vector2 vector = (base.location + new Vector2(0f, (float)Math.Sin(Game1.map.MapSegFrameLocked * 16f) * 20f)) * Game1.worldScale - Game1.Scroll;
				float num = 1f;
				if (Game1.pManager.renderingAdditive)
				{
					num = (float)Math.Sin(Game1.map.MapSegFrameLocked * 24f);
					this.DrawCorona(sprite, particlesTex, vector, Math.Max(num, 0.4f), l);
				}
				if (!Game1.pManager.renderingAdditive || num > 0f)
				{
					sprite.Draw(particlesTex[4], vector, new Rectangle(360, 120, 60, 60), Color.White * num, 0f, new Vector2(30f, 30f), 1.5f * worldScale, SpriteEffects.None, 1f);
				}
			}
		}
	}
}

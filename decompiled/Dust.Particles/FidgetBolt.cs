using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FidgetBolt : Particle
	{
		private Vector2[] boltNodes;

		private Vector2 spawnLoc;

		private float frame;

		private byte nodeCount;

		private int targ;

		private byte drawingMode;

		private float bendOffset;

		private float bendOffsetTarget;

		public FidgetBolt(Vector2 loc, CharDir dir, int _owner, int spun, int l)
		{
			this.Reset(loc, dir, _owner, spun, l);
		}

		public void Reset(Vector2 loc, CharDir dir, int _owner, int spun, int l)
		{
			base.exists = Exists.Init;
			base.isSpun = spun;
			base.maskGlow = 0.05f;
			base.strength = 2;
			if (_owner == 0)
			{
				base.location = Game1.character[0].Location;
				if (base.isSpun == 0f)
				{
					base.location += Rand.GetRandomVector2(-400f, 400f, -400f, 400f);
				}
				base.owner = 0;
			}
			else
			{
				base.location = loc;
			}
			this.targ = Game1.pManager.FindTarg(Game1.character, this, 1200f);
			this.spawnLoc = loc;
			base.owner = _owner;
			if (base.isSpun == 0f)
			{
				if (this.targ == -1)
				{
					base.location = new Vector2(this.spawnLoc.X + 1400f / Game1.worldScale * Game1.hiDefScaleOffset * (float)((dir != 0) ? 1 : (-1)), this.spawnLoc.Y + Rand.GetRandomFloat(-400f, 100f));
					this.frame = Rand.GetRandomFloat(0.3f, 0.6f);
				}
				else
				{
					this.frame = Rand.GetRandomFloat(0.8f, 1.2f);
				}
				if (!(loc != Vector2.Zero))
				{
					base.exists = Exists.Dead;
					base.Reset();
					return;
				}
				Sound.PlayCue("fidgetproj_03", (Game1.pManager.GetFidgetLoc(accomodateScroll: false) + base.location) / 2f, (Game1.pManager.GetFidgetLoc(accomodateScroll: false) - Game1.character[0].Location).Length() / 2f);
			}
			else
			{
				if (this.targ == -1)
				{
					this.FailStorm(Game1.character);
					base.exists = Exists.Dead;
					base.Reset();
					return;
				}
				this.frame = Rand.GetRandomFloat(0.4f, 2f);
				base.strength = 10;
			}
			Game1.map.projectileCount++;
			this.bendOffset = (this.bendOffsetTarget = 0f);
			this.drawingMode = 0;
			this.nodeCount = 0;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		private void InitStorm(Map map, ParticleManager pMan, Character[] c, int l)
		{
			List<int> list = new List<int>();
			for (int i = 1; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Team == Team.Enemy && c[i].Ethereal != EtherealState.Ethereal && c[i].DyingFrame == -1f && c[i].renderable)
				{
					list.Add(i);
				}
			}
			if (list.Count < 2)
			{
				for (int j = 0; j < 6; j++)
				{
					if (map.projectileCount < 20)
					{
						pMan.AddFidgetBolt(pMan.GetFidgetLoc(accomodateScroll: false), CharDir.Right, 0, 10, l);
					}
				}
				Sound.PlayCue("fidgetproj_03_fail", c[0].Location, 0f);
			}
			else
			{
				int randomInt = Rand.GetRandomInt(0, 2);
				Game1.pManager.AddElectricBolt(c[list[randomInt]].Location - new Vector2(0f, c[list[randomInt]].Height / 2), 101, 0.6f, 1000, 150, 3f, 5);
				for (int k = 0; k < list.Count; k++)
				{
					for (int m = 0; m < list.Count; m++)
					{
						if (list[k] != list[m] && list[k] > -1 && list[m] > -1)
						{
							if (map.projectileCount < 20)
							{
								pMan.AddFidgetBolt(c[list[m]].Location, CharDir.Right, list[k], 10, l);
							}
							list[m] = -1;
							break;
						}
					}
				}
				Sound.PlayCue("fidgetproj_03_strong");
			}
			pMan.InitFidgetThrow();
			this.KillProjectile(map, pMan);
			VibrationManager.SetScreenShake(1f);
			if (map.shockRingCount <= 0)
			{
				map.shockRingCount++;
				pMan.AddShockRing(base.location, 1f, l);
			}
		}

		private void FailStorm(Character[] c)
		{
			Sound.PlayCue("fidgetproj_03_fail");
			Vector2 loc = c[0].Location + new Vector2((c[0].Face == CharDir.Left) ? (-220) : 200, -280f) + Rand.GetRandomVector2(-400f, 400f, -400f, 600f);
			for (int i = 0; i < 6; i++)
			{
				Game1.pManager.AddElectricBolt(loc, -1, Rand.GetRandomFloat(0.1f, 0.5f), 1000, 100, 0.6f, 5);
				Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-400f, 400f, -800f, 10f), 0.4f, 6);
			}
		}

		private void KillProjectile(Map map, ParticleManager pMan)
		{
			if (map.projectileCount > 0)
			{
				map.projectileCount--;
			}
			base.Reset();
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				this.KillProjectile(map, pMan);
			}
			if (Game1.skipFrame != 1 || this.drawingMode != 0)
			{
				return;
			}
			this.drawingMode = 1;
			int num = (int)(140f * Game1.worldScale);
			if (base.isSpun < 10f)
			{
				if (base.isSpun > 0.75f)
				{
					this.InitStorm(map, pMan, c, l);
				}
				if (c[0].AnimName == "attack01" || c[0].AnimName == "airspin" || c[0].AnimName == "attackairdown")
				{
					if (c[0].State == CharState.Air)
					{
						base.isSpun += gameTime * 4f;
					}
					else
					{
						base.isSpun += gameTime * 2f;
					}
					this.frame = 0.2f;
					Vector2 vector = c[0].Location + new Vector2((c[0].Face == CharDir.Left) ? (-220) : 200, -280f);
					base.trajectory = Rand.GetRandomVector2(-400f, 400f, -400f, 600f);
					base.location += (base.trajectory + vector - base.location) * gameTime * 14f;
					this.spawnLoc += (-base.trajectory + vector - this.spawnLoc) * gameTime * 10f;
					num = (int)(60f * Game1.worldScale);
					if (Rand.GetRandomInt(0, 4) == 0)
					{
						pMan.AddElectricBolt(vector, -1, Rand.GetRandomFloat(0.2f, 0.6f), 40, 500, 0.4f, 5);
					}
				}
			}
			if (base.isSpun == 0f || base.isSpun == 10f)
			{
				if (base.owner == 0)
				{
					this.spawnLoc = Game1.pManager.GetFidgetLoc(accomodateScroll: false);
				}
				else
				{
					this.spawnLoc = c[base.owner].Location - new Vector2(0f, c[base.owner].Height / 2) + Rand.GetRandomVector2(-50f, 50f, -50f, 50f);
					Vector2 vector2 = base.location;
					int i = base.owner;
					base.owner = 0;
					base.location = this.spawnLoc;
					HitManager.CheckIDHit(this, c, pMan, i);
					base.owner = i;
					base.location = vector2;
				}
				if (this.targ > -1)
				{
					base.location = c[this.targ].Location - new Vector2(0f, c[this.targ].Height / 2) + Rand.GetRandomVector2(-50f, 50f, -50f, 50f);
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						Sound.PlayCue("fidgetproj_03_contact", base.location, (base.location - c[0].Location).Length() / 8f);
					}
					int num2 = base.owner;
					base.owner = 0;
					HitManager.CheckIDHit(this, c, pMan, this.targ);
					base.owner = num2;
				}
				else
				{
					base.location += Rand.GetRandomVector2(-50f, 50f, -50f, 50f);
				}
			}
			float num3 = (base.location - this.spawnLoc).Length();
			int num4 = (int)Math.Max(50f, 10f * num3 / 300f);
			byte b = this.nodeCount;
			this.nodeCount = (byte)Math.Min(num3 / Math.Min(num, num3), 250f);
			if (this.nodeCount != b)
			{
				this.boltNodes = new Vector2[this.nodeCount];
			}
			float num5 = GlobalFunctions.GetAngle(this.spawnLoc, base.location) + 1.57f;
			for (int j = 0; j < this.boltNodes.Length; j++)
			{
				float num6 = (float)j / (float)this.boltNodes.Length;
				float num7 = (float)Math.Sin(6.28 * (double)(num6 / 2f / 1f));
				ref Vector2 reference = ref this.boltNodes[j];
				reference = base.location * num6 + this.spawnLoc * (1f - num6) + new Vector2((Rand.GetRandomFloat(-num4, num4) + this.bendOffset) * (float)Math.Cos(num5), (Rand.GetRandomFloat(-num4, num4) + this.bendOffset * Game1.worldScale) * (float)Math.Sin(num5)) * num7;
			}
			if (Rand.GetRandomInt(0, 10) == 0)
			{
				this.bendOffset = 0f;
				this.bendOffsetTarget = Rand.GetRandomFloat(-80f, 80f) * (float)(int)this.nodeCount;
			}
			this.bendOffset += (this.bendOffsetTarget - this.bendOffset) * gameTime * 4f;
			this.drawingMode = 0;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.drawingMode != 0)
			{
				return;
			}
			this.drawingMode = 2;
			Rectangle value = new Rectangle(2940, 1860, 497, 150);
			float angle = GlobalFunctions.GetAngle(base.location, this.spawnLoc);
			if (this.boltNodes != null)
			{
				for (int i = 0; i < this.nodeCount; i++)
				{
					float num;
					float rotation;
					if (i < this.boltNodes.Length - 1)
					{
						num = (this.boltNodes[i + 1] - this.boltNodes[i]).Length();
						rotation = GlobalFunctions.GetAngle(this.boltNodes[i], this.boltNodes[i + 1]) + 1.57f;
					}
					else
					{
						num = (base.location - this.boltNodes[i]).Length();
						rotation = GlobalFunctions.GetAngle(this.boltNodes[i], base.location) + 1.57f;
					}
					Vector2 position = this.boltNodes[i] * worldScale - Game1.Scroll;
					sprite.Draw(particlesTex[2], position, new Rectangle(2132 + Rand.GetRandomInt(0, 2) * 40, 1000 + Rand.GetRandomInt(0, 2) * 120, 40, 120), Color.White * this.frame * 4f, rotation, new Vector2(20f, 10f), new Vector2(Rand.GetRandomFloat(0.6f, 2f), num / 110f) * worldScale, (Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
					if (i > 0 && Rand.GetRandomInt(0, 3) == 0)
					{
						sprite.Draw(particlesTex[2], position, value, Color.White * this.frame * 4f, angle, new Vector2(248f, 75f), worldScale, SpriteEffects.None, 1f);
					}
				}
			}
			sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(3920, 1240, 176, 140), new Color(1f, 1f, 1f, this.frame * 4f), Rand.GetRandomFloat(0f, 6.28f), new Vector2(85f, 70f), Rand.GetRandomFloat(0.5f, 1f) * worldScale, SpriteEffects.None, 1f);
			this.drawingMode = 0;
		}
	}
}

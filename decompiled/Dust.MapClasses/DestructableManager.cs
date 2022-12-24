using System;
using System.Collections.Generic;
using Dust.CharClasses;
using Dust.HUD;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class DestructableManager
	{
		private SpriteBatch sprite;

		private Texture2D destTex;

		private Character[] c;

		private Map map;

		public List<DestructWall> destructWalls = new List<DestructWall>();

		public List<DestructPlatform> destructPlatforms = new List<DestructPlatform>();

		public List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();

		public List<DestructLamp> destructLamps = new List<DestructLamp>();

		public List<Gate> gates = new List<Gate>();

		public DestructableManager(SpriteBatch _sprite, Character[] _c, Map _map)
		{
			this.sprite = _sprite;
			this.c = _c;
			this.map = _map;
		}

		public void Reset()
		{
			this.destructWalls.Clear();
			this.destructPlatforms.Clear();
			this.movingPlatforms.Clear();
			this.gates.Clear();
			Game1.GetLargeContent().Unload();
			this.destTex = null;
		}

		private bool CheckInBounds(Vector2 loc, Vector2 destLoc)
		{
			if (loc.X > destLoc.X - 200f && loc.X < destLoc.X + 200f && loc.Y > destLoc.Y - 800f && loc.Y < destLoc.Y + 150f)
			{
				return true;
			}
			return false;
		}

		public bool CheckPlatformBounds(Vector2 loc, Vector2 destLoc, int width)
		{
			if (loc.X > destLoc.X && loc.X < destLoc.X + (float)width && loc.Y > destLoc.Y && loc.Y < destLoc.Y + 64f)
			{
				return true;
			}
			return false;
		}

		public float CheckCol(Character c, int offset, ref bool sideCollision)
		{
			for (int i = 0; i < this.gates.Count; i++)
			{
				if (this.gates[i].openStage < 2 || this.gates[i].openStage > 3)
				{
					Vector2 location = this.gates[i].location;
					if (this.CheckInBounds(c.Location + new Vector2(0f, offset), location))
					{
						return location.Y - 800f;
					}
				}
			}
			for (int j = 0; j < this.destructWalls.Count; j++)
			{
				if (this.destructWalls[j].Exists)
				{
					Vector2 location = this.destructWalls[j].location;
					if (this.CheckInBounds(c.Location + new Vector2(0f, offset), location))
					{
						return location.Y - 800f;
					}
				}
			}
			sideCollision = false;
			if (c.Trajectory.Y >= 0f)
			{
				for (int k = 0; k < this.destructPlatforms.Count; k++)
				{
					for (int l = 0; l < this.destructPlatforms[k].Exists.Length; l++)
					{
						if (!this.destructPlatforms[k].Exists[l])
						{
							continue;
						}
						Vector2 location = this.destructPlatforms[k].location + new Vector2(l * 240 - 30, 0f);
						if (!this.CheckPlatformBounds(c.Location + new Vector2(0f, offset), location, 300) && (!(c.Trajectory.Y > 3000f) || !this.CheckPlatformBounds(c.Location + new Vector2(0f, offset - 64), location, 300)))
						{
							continue;
						}
						if (c.State == CharState.Grounded)
						{
							if (c.Location.Y >= c.PLoc.Y || c.Trajectory.Y > 0f)
							{
								c.ledgeAttach = -1;
								c.CanFallThrough = false;
							}
							c.Trajectory.Y = 0f;
							c.PLoc.Y = (c.Location.Y = location.Y);
						}
						return location.Y;
					}
				}
				for (int m = 0; m < this.movingPlatforms.Count; m++)
				{
					Vector2 location = this.movingPlatforms[m].GetLoc();
					if ((this.CheckPlatformBounds(c.Location + new Vector2(0f, offset), location, this.movingPlatforms[m].segments * 240) || (c.Trajectory.Y > 3000f && this.CheckPlatformBounds(c.Location + new Vector2(0f, offset - 64), location, this.movingPlatforms[m].segments * 240))) && (!c.IsFalling || c.Trajectory.Y > 200f))
					{
						if (c.ledgeAttach == -1 || location.Y < this.movingPlatforms[m].prevLoc.Y)
						{
							c.CanFallThrough = true;
						}
						return location.Y;
					}
				}
			}
			sideCollision = false;
			return -1f;
		}

		public bool CheckColUpper(int i, int offset)
		{
			for (int j = 0; j < this.destructPlatforms.Count; j++)
			{
				for (int k = 0; k < this.destructPlatforms[j].Exists.Length; k++)
				{
					if (this.destructPlatforms[j].Exists[k])
					{
						Vector2 destLoc = this.destructPlatforms[j].location + new Vector2(k * 240 - 30, 0f);
						if (this.CheckPlatformBounds(this.c[i].Location - new Vector2(0f, offset + 50), destLoc, 300))
						{
							return true;
						}
						if (this.c[i].Trajectory.Y < -2000f && (this.CheckPlatformBounds(this.c[i].Location + new Vector2(0f, offset - 300), destLoc, 300) || this.CheckPlatformBounds(this.c[i].Location + new Vector2(0f, offset - 364), destLoc, 300)))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CheckBombCol(Vector2 bombLoc)
		{
			for (int i = 0; i < this.destructWalls.Count; i++)
			{
				if (this.destructWalls[i].Exists)
				{
					Vector2 location = this.destructWalls[i].location;
					if (bombLoc.X > location.X - 200f && bombLoc.X < location.X + 200f && bombLoc.Y > location.Y - 900f && bombLoc.Y < location.Y + 150f)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CheckBombKill(Vector2 bombLoc, CharDir dir)
		{
			for (int i = 0; i < this.destructWalls.Count; i++)
			{
				if (this.destructWalls[i].Exists)
				{
					Vector2 location = this.destructWalls[i].location;
					if (this.CheckInBounds(bombLoc, location))
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						Game1.pManager.MakeSlash(CharacterType.Dust, (CharacterType)this.destructWalls[i].debrisType, bombLoc, 1.2f, "KO", 0, Color.White, dir);
						this.destructWalls[i].HP = 0f;
						this.destructWalls[i].KillMe(Game1.pManager, dir);
						Game1.stats.comboTimer = 2f;
						Game1.stats.comboMeter++;
						Game1.stats.damageMeter += 1000;
						Game1.hud.comboTextSize = 3.5f;
					}
				}
			}
		}

		public void AddDestructable(int Id, Vector2 loc, int bombType, int flip)
		{
			this.SetDestTextures();
			this.destructWalls.Add(new DestructWall(Id, loc, bombType, (flip != 0) ? true : false));
		}

		public void AddDestructPlatform(Vector2 loc, int width, int ID)
		{
			this.SetDestTextures();
			this.destructPlatforms.Add(new DestructPlatform(loc, width, ID));
		}

		public void AddMovingPlatform(Vector2 loc, int width, int pathPercent, int speed, int loopType)
		{
			this.movingPlatforms.Add(new MovingPlatform(loc, width, pathPercent, speed, loopType));
		}

		public void AddDestructLamps(List<DestructLamp> lampList)
		{
			this.destructLamps.Clear();
			for (int i = 0; i < lampList.Count; i++)
			{
				if (lampList[i].Exists && lampList[i].GetMapName == Game1.map.path)
				{
					this.destructLamps.Add(new DestructLamp(string.Empty, lampList[i].GetID, lampList[i].GetLocation));
				}
			}
		}

		public void AddGate(int Id, Vector2 loc, int lockType)
		{
			this.gates.Add(new Gate(Id, loc, lockType));
		}

		public void UpdateDestructables(float frameTime, Map map, ChallengeManager cManager, Character[] c)
		{
			for (int i = 0; i < this.destructWalls.Count; i++)
			{
				if (this.destructWalls[i] != null)
				{
					if (this.destructWalls[i].Exists)
					{
						this.destructWalls[i].Update(frameTime, map, this, c);
					}
					else
					{
						this.destructWalls.Remove(this.destructWalls[i]);
					}
				}
			}
			for (int j = 0; j < this.destructPlatforms.Count; j++)
			{
				if (this.destructPlatforms[j] != null)
				{
					this.destructPlatforms[j].Update(frameTime, map, this, c);
				}
			}
			for (int k = 0; k < this.movingPlatforms.Count; k++)
			{
				if (this.movingPlatforms[k] != null)
				{
					this.movingPlatforms[k].Update(frameTime, map);
				}
			}
			for (int l = 0; l < this.gates.Count; l++)
			{
				if (this.gates[l] != null)
				{
					this.gates[l].Update(frameTime, map, this, c);
				}
			}
		}

		public void UpdateMovingLedges(Character c)
		{
			for (int i = 0; i < this.movingPlatforms.Count; i++)
			{
				this.movingPlatforms[i].UpdateCollision(Game1.FrameTime, this.map, this, c);
			}
		}

		public Vector2 PlatformTrajectory(Character c, Vector2 charLoc)
		{
			if (c.State == CharState.Grounded)
			{
				for (int i = 0; i < this.movingPlatforms.Count; i++)
				{
					Vector2 loc = this.movingPlatforms[i].GetLoc();
					if (this.CheckPlatformBounds(charLoc + new Vector2(0f, 32f), loc, this.movingPlatforms[i].segments * 240))
					{
						return loc - this.movingPlatforms[i].prevLoc;
					}
				}
			}
			return Vector2.Zero;
		}

		public void DrawLightSources(Texture2D[] particlesTex, float worldScale, float rot)
		{
			for (int i = 0; i < this.destructWalls.Count; i++)
			{
				if (this.destructWalls[i].Exists && this.destructWalls[i].bombType == 1)
				{
					for (int j = 0; j < 8; j++)
					{
						this.sprite.Draw(particlesTex[2], ((this.destructWalls[i].location + Rand.GetRandomVector2(-100f, 100f, -400f, -100f)) * worldScale - Game1.Scroll) / 4f, new Rectangle(4000, 2796, 95, 95), new Color(1f, 1f, 1f, 0.2f), rot + (float)i, new Vector2(48f, 48f), worldScale * 0.7f, SpriteEffects.None, 1f);
					}
				}
			}
		}

		public void DrawDestructables(Texture2D[] particlesTex)
		{
			for (int i = 0; i < this.destructWalls.Count; i++)
			{
				if (this.destructWalls[i].Exists)
				{
					this.destructWalls[i].Draw(this.sprite, this.destTex);
				}
			}
			for (int j = 0; j < this.destructPlatforms.Count; j++)
			{
				this.destructPlatforms[j].Draw(this.sprite, this.destTex);
			}
			for (int k = 0; k < this.movingPlatforms.Count; k++)
			{
				this.movingPlatforms[k].Draw(this.sprite, particlesTex);
			}
			for (int l = 0; l < this.destructLamps.Count; l++)
			{
				if (this.destructLamps[l].Exists)
				{
					this.destructLamps[l].Draw(this.sprite, this.map, particlesTex);
				}
			}
			for (int m = 0; m < this.gates.Count; m++)
			{
				this.gates[m].Draw(this.sprite, particlesTex);
			}
		}

		public int TextureType(string path)
		{
			if (path.StartsWith("challenge"))
			{
				return 1;
			}
			if (path.StartsWith("cave") || path.StartsWith("farm"))
			{
				return 2;
			}
			if (path.StartsWith("grave"))
			{
				return 3;
			}
			if (path.StartsWith("trial"))
			{
				return 3;
			}
			if (path.StartsWith("mansion"))
			{
				return 4;
			}
			if (path.StartsWith("snow"))
			{
				return 5;
			}
			if (path.StartsWith("lava"))
			{
				return 6;
			}
			return 0;
		}

		public void SetDestTextures()
		{
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadDestTextures)));
		}

		public void LoadDestTextures()
		{
			int num = 0;
			while (num < 10)
			{
				try
				{
					this.destTex = Game1.GetDestructContent().Load<Texture2D>("gfx/backgrounds/dest_" + $"{this.TextureType(this.map.path):D2}");
					num = 20;
				}
				catch (Exception)
				{
					num++;
				}
			}
		}
	}
}

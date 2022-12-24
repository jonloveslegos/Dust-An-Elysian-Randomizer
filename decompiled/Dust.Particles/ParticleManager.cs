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
	public class ParticleManager
	{
		private SpriteBatch sprite;

		private Particle[,] particle;

		private Character[] character;

		private int particleMax = 300;

		private int layer = 10;

		public int startLayer = 2;

		private int nLayer;

		private int rState;

		private int maxHits = 20;

		public bool renderingAdditive;

		private List<BombRegion> prevBombs = new List<BombRegion>();

		public ParticleManager(SpriteBatch _sprite, Character[] _character)
		{
			this.particle = new Particle[this.layer - this.startLayer, this.particleMax];
			this.sprite = _sprite;
			this.character = _character;
		}

		public int ParticleMax(int l)
		{
			switch (l)
			{
				case 4: return 160;
				case 5: return this.particleMax;
				case 6: return 200;
				case 8: return 200;
			}
			return 100;
		}

		public void PopulateParticles()
		{
			for (int i = this.startLayer; i < this.layer; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					this.particle[i - this.startLayer, j] = new Particle();
					this.particle[i - this.startLayer, j].Reset();
				}
			}
			for (int k = 1; k < this.maxHits; k++)
			{
				this.particle[5 - this.startLayer, k] = new Hit(Vector2.Zero, Vector2.Zero, -1, 0);
				this.particle[5 - this.startLayer, k].exists = Exists.Dead;
			}
		}

		public void Reset(bool removeWeather, bool removeBombs)
		{
			int num = 2;
			if (removeWeather)
			{
				num = 0;
			}
			this.prevBombs.Clear();
			int intVar = 0;
			float floatVar = 0f;
			Character character = this.character[0];
			for (int i = this.startLayer; i < this.layer - num; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					if (!removeBombs && this.particle[i - this.startLayer, j] is Bomb && this.particle[i - this.startLayer, j].exists == Exists.Exists)
					{
						this.particle[i - this.startLayer, j].GetInfo(ref intVar, ref floatVar);
						if (intVar > 2 && intVar < 4 && floatVar >= 1f)
						{
							float num2 = 1400f / Game1.worldScale;
							float num3 = 1000f / Game1.worldScale;
							if (this.particle[i - this.startLayer, j].location.X > character.Location.X - num2 && this.particle[i - this.startLayer, j].location.X < character.Location.X + num2 && this.particle[i - this.startLayer, j].location.Y > character.Location.Y - num3 && this.particle[i - this.startLayer, j].location.Y < character.Location.Y + num3)
							{
								this.prevBombs.Add(new BombRegion(new Vector2(-10000f, -10000f), this.particle[i - this.startLayer, j].flag, (int)floatVar, _forceSpawn: false));
							}
						}
					}
					this.particle[i - this.startLayer, j].Reset();
				}
			}
			this.ResetHud();
			GC.Collect();
		}

		public void ResetHud()
		{
			for (int i = 0; i < this.ParticleMax(9); i++)
			{
				this.particle[9 - this.startLayer, i].Reset();
			}
		}

		public void ResetWeather()
		{
			Vector2 vector = Game1.Scroll / Game1.worldScale * 1.25f;
			for (int i = 0; i < this.ParticleMax(8 - this.startLayer); i++)
			{
				if (this.particle[8 - this.startLayer, i].exists == Exists.Exists)
				{
					this.particle[8 - this.startLayer, i].location = vector;
					this.particle[8 - this.startLayer, i].Relocate(vector + Rand.GetRandomVector2(-100f, Game1.screenWidth + 100, -100f, Game1.screenHeight) / Game1.worldScale);
				}
			}
		}

		public bool ResetFidget(Character[] c)
		{
			if (this.particle[5 - this.startLayer, 0].exists == Exists.Dead)
			{
				return false;
			}
			this.particle[5 - this.startLayer, 0].location = c[0].Location + new Vector2(0f, -200f);
			return true;
		}

		public void AddFidget(Vector2 loc)
		{
			this.particle[5 - this.startLayer, 0] = new Fidget(loc);
		}

		public Vector2 GetFidgetLoc(bool accomodateScroll)
		{
			if (this.particle[5 - this.startLayer, 0].exists == Exists.Dead)
			{
				return Vector2.Zero;
			}
			if (accomodateScroll)
			{
				return this.particle[5 - this.startLayer, 0].location * Game1.worldScale - Game1.Scroll;
			}
			return this.particle[5 - this.startLayer, 0].location + new Vector2((this.character[0].Face == CharDir.Right) ? 50 : (-50), -20f);
		}

		public void InitFidgetThrow()
		{
			if (this.particle[5 - this.startLayer, 0].exists != 0)
			{
				this.particle[5 - this.startLayer, 0].InitAction(0);
			}
		}

		public bool DirectParticle(Particle p, int command)
		{
			for (int i = this.startLayer; i < this.layer; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					if (this.particle[i - this.startLayer, j].GetType() == p.GetType())
					{
						return this.particle[i - this.startLayer, j].InitAction(command);
					}
				}
			}
			return false;
		}

		public bool DirectParticle(int layer, int id, int command)
		{
			if (this.particle[layer - this.startLayer, id].exists == Exists.Exists)
			{
				return this.particle[layer - this.startLayer, id].InitAction(command);
			}
			return false;
		}

		public void RepopBombs()
		{
			for (int i = 0; i < this.prevBombs.Count; i++)
			{
				if (this.prevBombs[i] != null)
				{
					Game1.map.bombRegions.Add(new BombRegion(new Vector2(-10000f, -10000f), this.prevBombs[i].ID, this.prevBombs[i].Timer, _forceSpawn: true));
				}
			}
			this.prevBombs.Clear();
		}

		public void ReposBombs()
		{
			int num = 5;
			for (int i = 0; i < this.ParticleMax(num); i++)
			{
				if (this.particle[num - this.startLayer, i] is Bomb && this.particle[num - this.startLayer, i].exists == Exists.Exists)
				{
					int intVar = 0;
					float floatVar = 0f;
					this.particle[num - this.startLayer, i].GetInfo(ref intVar, ref floatVar);
					if (intVar > 2 && intVar < 4)
					{
						this.particle[num - this.startLayer, i].location = this.character[0].Location;
					}
					Game1.map.bombRegions.Add(new BombRegion(new Vector2(-10000f, -10000f), this.particle[num - this.startLayer, i].flag, (int)floatVar, _forceSpawn: true));
					this.particle[num - this.startLayer, i].Reset();
				}
			}
		}

		public bool CheckMapTreasureList()
		{
			for (int i = this.startLayer; i < this.layer; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					if (this.particle[i - this.startLayer, j].exists == Exists.Exists)
					{
						if ((this.particle[i - this.startLayer, j] is Chest || this.particle[i - this.startLayer, j] is Cage) && this.particle[i - this.startLayer, j].flag != 100)
						{
							return true;
						}
						if (this.particle[i - this.startLayer, j] is Key || this.particle[i - this.startLayer, j] is Note || this.particle[i - this.startLayer, j] is Upgrade)
						{
							return true;
						}
					}
				}
			}
			for (int k = 1; k < this.character.Length; k++)
			{
				if (this.character[k].Exists == CharExists.Exists && this.character[k].CollectEquipID > -1)
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckBomb(int ID)
		{
			int num = 5;
			for (int i = 0; i < this.ParticleMax(num); i++)
			{
				if (this.particle[num - this.startLayer, i].exists == Exists.Exists && this.particle[num - this.startLayer, i] is Bomb && this.particle[num - this.startLayer, i].flag == ID)
				{
					return true;
				}
			}
			return false;
		}

		public void GetParticle(int l, int id, ref Vector2 loc, ref Vector2 traj, ref int flag)
		{
			if (id > -1 && id < this.ParticleMax(l) && this.particle[l - this.startLayer, id].exists == Exists.Exists)
			{
				loc = this.particle[l - this.startLayer, id].location;
				traj = this.particle[l - this.startLayer, id].trajectory;
				flag = this.particle[l - this.startLayer, id].flag;
			}
		}

		public void GetParticleAudio(int l, int id, ref Vector2 loc, ref Vector2 traj, ref bool exists)
		{
			if (id < this.ParticleMax(l) && this.particle[l - this.startLayer, id].exists == Exists.Exists)
			{
				this.particle[l - this.startLayer, id].GetAudio(ref loc, ref traj, ref exists);
			}
		}

		public void GetParticleInfo(int l, int id, ref int intVar, ref float floatVar)
		{
			if (id > -1 && id < this.ParticleMax(l) && this.particle[l - this.startLayer, id].exists == Exists.Exists)
			{
				this.particle[l - this.startLayer, id].GetInfo(ref intVar, ref floatVar);
			}
		}

		public Vector2 CheckExistingParticle(Particle p)
		{
			for (int i = this.startLayer; i < this.layer; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					if (this.particle[i - this.startLayer, j].GetType() == p.GetType() && this.particle[i - this.startLayer, j].exists == Exists.Exists)
					{
						return new Vector2(i, j);
					}
				}
			}
			return Vector2.Zero;
		}

		public void RemoveParticle(Particle p)
		{
			for (int i = this.startLayer; i < this.layer; i++)
			{
				for (int j = 0; j < this.ParticleMax(i); j++)
				{
					if (this.particle[i - this.startLayer, j].GetType() == p.GetType())
					{
						this.particle[i - this.startLayer, j].Reset();
					}
				}
			}
		}

		private P GetSuitableParticle<P>(int l) where P : Particle
		{
			int num = this.ParticleMax(l);
			int num2 = 0;
			if (l == 5)
			{
				num2 = this.maxHits;
			}
			int num3 = l - this.startLayer;
			for (int i = num2; i < num; i++)
			{
				if (this.particle[num3, i] is P val && val.exists == Exists.Dead)
				{
					return val;
				}
			}
			return null;
		}

		private P GetSuitableAudioParticle<P>(int l, ref int audioID) where P : Particle
		{
			int num = this.ParticleMax(l);
			int num2 = 0;
			if (l == 5)
			{
				num2 = this.maxHits;
			}
			for (int i = num2; i < num; i++)
			{
				if (this.particle[l - this.startLayer, i] is P val && val.exists == Exists.Dead)
				{
					audioID = i;
					return val;
				}
			}
			return null;
		}

		private P GetSuitableHit<P>(int l) where P : Particle
		{
			for (int i = 1; i < this.maxHits; i++)
			{
				P val = this.particle[l - this.startLayer, i] as P;
				if (val.exists == Exists.Dead)
				{
					return val;
				}
			}
			return null;
		}

		private int Empty(int l)
		{
			int num = this.ParticleMax(l);
			int randomInt = Rand.GetRandomInt(20, num);
			for (int i = randomInt; i < num; i++)
			{
				if (this.particle[l - this.startLayer, i].exists == Exists.Dead)
				{
					return i;
				}
			}
			int num2 = 0;
			if (l == 5)
			{
				num2 = this.maxHits;
			}
			for (int j = num2; j < randomInt; j++)
			{
				if (this.particle[l - this.startLayer, j].exists == Exists.Dead)
				{
					return j;
				}
			}
			return -1;
		}

		public void UpdateCritical(float frameTime, Map map, Character[] c)
		{
			for (int i = 0; i < this.maxHits; i++)
			{
				if (this.particle[3, i].exists == Exists.Exists)
				{
					this.particle[3, i].Update(frameTime, map, this, c, 5);
				}
			}
		}

		public void AddAvalanche(Vector2 loc, CharDir direction, int width, int segments, int l)
		{
			int audioID = 0;
			Avalanche suitableAudioParticle = this.GetSuitableAudioParticle<Avalanche>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, direction, width, segments, audioID, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Avalanche(loc, direction, width, segments, num, l);
			}
		}

		public void AddBrickWall(Vector2 loc, Vector2 traj, float size, int l)
		{
			BrickWall suitableParticle = this.GetSuitableParticle<BrickWall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new BrickWall(loc, traj, size);
			}
		}

		public void AddBubbleDrip(Vector2 loc, float size, int l)
		{
			BubbleDrip suitableParticle = this.GetSuitableParticle<BubbleDrip>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new BubbleDrip(loc, size);
			}
		}

		public void AddBubbleSquirt(Vector2 loc, Vector2 traj, float size, int owner, int type, int l)
		{
			BubbleSquirt suitableParticle = this.GetSuitableParticle<BubbleSquirt>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, owner, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new BubbleSquirt(loc, traj, size, owner, type);
			}
		}

		public void AddButterfly(Vector2 loc, Vector2 traj, float size, int flag, int owner, int l)
		{
			Butterfly suitableParticle = this.GetSuitableParticle<Butterfly>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, flag, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Butterfly(loc, traj, size, flag, owner);
			}
		}

		public void AddCannonShell(Vector2 loc, Vector2 traj, float size, int l)
		{
			CannonShell suitableParticle = this.GetSuitableParticle<CannonShell>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new CannonShell(loc, traj, size);
			}
		}

		public void AddCaveLeaf(Vector2 loc, Vector2 traj, float size, int l)
		{
			CaveLeaf suitableParticle = this.GetSuitableParticle<CaveLeaf>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new CaveLeaf(loc, traj, size);
			}
		}

		public void AddCavePollen(int layer, int l)
		{
			PollenCave suitableParticle = this.GetSuitableParticle<PollenCave>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(layer);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new PollenCave(layer);
			}
		}

		public void AddChainLink(Vector2 loc, Vector2 traj, int l)
		{
			ChainLink suitableParticle = this.GetSuitableParticle<ChainLink>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new ChainLink(loc, traj);
			}
		}

		public void AddCheckPointGlow(Vector2 loc, float size, int l)
		{
			CheckPoint suitableParticle = this.GetSuitableParticle<CheckPoint>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new CheckPoint(loc, size);
			}
		}

		public void AddEmitFlame(Vector2 loc, Vector2 traj, float rot, float size, bool colliding, int type, int l)
		{
			EmitFlame suitableParticle = this.GetSuitableParticle<EmitFlame>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, size, colliding, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new EmitFlame(loc, traj, rot, size, colliding, type);
			}
		}

		public void AddEmitLava(Vector2 loc, Vector2 traj, float size, bool colliding, int type, int l)
		{
			EmitLava suitableParticle = this.GetSuitableParticle<EmitLava>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, colliding, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1 && num < this.particleMax - 10)
			{
				this.particle[l - this.startLayer, num] = new EmitLava(loc, traj, size, colliding, type);
			}
		}

		public void AddEmitRocks(Vector2 loc, Vector2 traj, float rot, float size, int type, int l)
		{
			EmitRocks suitableParticle = this.GetSuitableParticle<EmitRocks>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new EmitRocks(loc, traj, rot, size, type);
			}
		}

		public void AddFish(Vector2 loc, Vector2 traj, float size, int id, int l)
		{
			Fish suitableParticle = this.GetSuitableParticle<Fish>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, id);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Fish(loc, traj, size, id);
			}
		}

		public void AddFlameSpark(Vector2 loc, Vector2 traj, float rot, float size, int type, int l)
		{
			FlameSpark suitableParticle = this.GetSuitableParticle<FlameSpark>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FlameSpark(loc, traj, rot, size, type);
			}
		}

		public void AddFlamePuff(Vector2 loc, Vector2 traj, float size, int l)
		{
			FlamePuff suitableParticle = this.GetSuitableParticle<FlamePuff>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FlamePuff(loc, traj, size);
			}
		}

		public void AddFlySwarm(Vector2 loc, int l)
		{
			int audioID = 0;
			FlySwarm suitableAudioParticle = this.GetSuitableAudioParticle<FlySwarm>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, audioID, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FlySwarm(loc, num, l);
			}
		}

		public void AddFog(int layer, int l)
		{
			Fog suitableParticle = this.GetSuitableParticle<Fog>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(layer);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Fog(layer);
			}
		}

		public void AddGraveLeaf(Vector2 loc, Vector2 traj, float size, int l)
		{
			GraveLeaf suitableParticle = this.GetSuitableParticle<GraveLeaf>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new GraveLeaf(loc, traj, size);
			}
		}

		public void AddHeatWave(Vector2 loc, Vector2 traj, float size, float lifeSpan, int l)
		{
			HeatWave suitableParticle = this.GetSuitableParticle<HeatWave>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HeatWave(loc, traj, size, lifeSpan);
			}
		}

		public void AddIcicle(Vector2 loc, Vector2 traj, float size, int type, int l)
		{
			Icicle suitableParticle = this.GetSuitableParticle<Icicle>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Icicle(loc, traj, size, type);
			}
		}

		public void AddLampDebris(Vector2 loc, Vector2 traj, float size, int type, int l)
		{
			LampDebris suitableParticle = this.GetSuitableParticle<LampDebris>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new LampDebris(loc, traj, size, type);
			}
		}

		public void AddLavaDrip(Vector2 loc, Vector2 traj, float size, int l)
		{
			LavaDrip suitableParticle = this.GetSuitableParticle<LavaDrip>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new LavaDrip(loc, traj, size);
			}
		}

		public void AddLavaSpray(Vector2 loc, Vector2 traj, float size, int type, int l)
		{
			LavaSpray suitableParticle = this.GetSuitableParticle<LavaSpray>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new LavaSpray(loc, traj, size, type);
			}
		}

		public void AddLeaf(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, int l)
		{
			Leaf suitableParticle = this.GetSuitableParticle<Leaf>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Leaf(loc, traj, r, g, b, a, size);
			}
		}

		public void AddLightning(Vector2 loc, bool master, float lifeSpan, int boltCount, int boltLength, float boltWidthMult, int l)
		{
			Lightning suitableParticle = this.GetSuitableParticle<Lightning>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, master, lifeSpan, boltCount, boltLength, boltWidthMult, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Lightning(loc, master, lifeSpan, boltCount, boltLength, boltWidthMult, l);
			}
		}

		public void AddPollen(int layer, int l)
		{
			Pollen suitableParticle = this.GetSuitableParticle<Pollen>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(layer);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Pollen(layer);
			}
		}

		public void AddPondRipple(Vector2 loc, float size, int l)
		{
			PondRipple suitableParticle = this.GetSuitableParticle<PondRipple>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new PondRipple(loc, size);
			}
		}

		public void AddRain(Vector2 loc, Vector2 traj, float size, int rainType, int l)
		{
			Rain suitableParticle = this.GetSuitableParticle<Rain>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, rainType);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Rain(loc, traj, size, rainType);
			}
		}

		public void AddSmoke(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, float lifeSpan, int l)
		{
			Smoke suitableParticle = this.GetSuitableParticle<Smoke>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, size, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Smoke(loc, traj, r, g, b, a, size, lifeSpan);
			}
		}

		public void AddSnow(Vector2 traj, float size, int snowType, int l)
		{
			Snow suitableParticle = this.GetSuitableParticle<Snow>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(traj, size, snowType, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Snow(traj, size, snowType, l);
			}
		}

		public void AddSnowTime(Vector2 traj, float size, int l)
		{
			SnowTime suitableParticle = this.GetSuitableParticle<SnowTime>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(traj, size, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new SnowTime(traj, size, l);
			}
		}

		public void AddSpray(Vector2 loc, Vector2 traj, float size, int type, byte count, int l)
		{
			Spray suitableParticle = this.GetSuitableParticle<Spray>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, type, count);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Spray(loc, traj, size, type, count);
			}
		}

		public void AddVolcanicAsh(Vector2 traj, float size, int ashType, int l)
		{
			VolcanicAsh suitableParticle = this.GetSuitableParticle<VolcanicAsh>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(traj, size, ashType, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new VolcanicAsh(traj, size, ashType, l);
			}
		}

		public void AddWaterDrip(Vector2 loc, float size, int l)
		{
			WaterDrip suitableParticle = this.GetSuitableParticle<WaterDrip>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new WaterDrip(loc, size);
			}
		}

		public void AddWaterFall(Vector2 loc, float traj, float size, int stopPoint, int l)
		{
			WaterFall suitableParticle = this.GetSuitableParticle<WaterFall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, stopPoint);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new WaterFall(loc, traj, size, stopPoint);
			}
		}

		public void AddWaterSplash(Vector2 loc, float size, int flag, int l)
		{
			WaterSplash suitableParticle = this.GetSuitableParticle<WaterSplash>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, flag);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new WaterSplash(loc, size, flag);
			}
		}

		public void AddHudSpark(Vector2 loc, Vector2 traj, float size, int l)
		{
			HudSpark suitableParticle = this.GetSuitableParticle<HudSpark>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudSpark(loc, traj, size);
			}
		}

		public void AddCaption(Vector2 loc, string text, float size, Color color, float lifeSpan, int l)
		{
			int num = this.ParticleMax(l);
			for (int i = 0; i < num; i++)
			{
				if (this.particle[l - this.startLayer, i] is Caption)
				{
					this.particle[l - this.startLayer, i].Reset();
				}
			}
			Caption suitableParticle = this.GetSuitableParticle<Caption>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, text, size, color, lifeSpan);
				return;
			}
			int num2 = this.Empty(l);
			if (num2 > -1)
			{
				this.particle[l - this.startLayer, num2] = new Caption(loc, text, size, color, lifeSpan);
			}
		}

		public void AddComboBreak(Vector2 loc, float size, int flag, int l)
		{
			ComboBreak suitableParticle = this.GetSuitableParticle<ComboBreak>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, flag);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new ComboBreak(loc, size, flag);
			}
		}

		public void AddMenuLeaf(Vector2 loc, Vector2 traj, float rot, float size, int l)
		{
			MenuLeaf suitableParticle = this.GetSuitableParticle<MenuLeaf>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new MenuLeaf(loc, traj, rot, size);
			}
		}

		public void AddMenuPuff(Vector2 loc, float size, int l)
		{
			MenuPuff suitableParticle = this.GetSuitableParticle<MenuPuff>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new MenuPuff(loc, size);
			}
		}

		public void AddMenuDebris(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, int debrisType, int l)
		{
			MenuDebris suitableParticle = this.GetSuitableParticle<MenuDebris>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, size, debrisType);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new MenuDebris(loc, traj, r, g, b, a, size, debrisType);
			}
		}

		public void AddHudCoin(Vector2 loc, Vector2 traj)
		{
			HudCoin suitableParticle = this.GetSuitableParticle<HudCoin>(9);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj);
				return;
			}
			int num = this.Empty(9);
			if (num > -1)
			{
				this.particle[9 - this.startLayer, num] = new HudCoin(loc, traj);
			}
		}

		public void AddHudDust(Vector2 loc, Vector2 traj, float size, float rot, Color color, float alpha, int type, int l)
		{
			HudDust suitableParticle = this.GetSuitableParticle<HudDust>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, rot, color, alpha, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudDust(loc, traj, size, rot, color, alpha, type);
			}
		}

		public void AddHudFire(Vector2 loc, Vector2 traj, float rot, float size, int type, int l)
		{
			HudFire suitableParticle = this.GetSuitableParticle<HudFire>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, size, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudFire(loc, traj, rot, size, type);
			}
		}

		public void AddHudLightning(Vector2 loc, bool master, float lifeSpan, int boltCount, int boltLength, float boltWidthMult, int l)
		{
			HudLightning suitableParticle = this.GetSuitableParticle<HudLightning>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, master, lifeSpan, boltCount, boltLength, boltWidthMult, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudLightning(loc, master, lifeSpan, boltCount, boltLength, boltWidthMult, l);
			}
		}

		public void AddHudRain(Vector2 loc, Vector2 traj, float size, int l)
		{
			HudRain suitableParticle = this.GetSuitableParticle<HudRain>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudRain(loc, traj, size);
			}
		}

		public void AddHudSmoke(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, float lifeSpan, int l)
		{
			HudSmoke suitableParticle = this.GetSuitableParticle<HudSmoke>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, size, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HudSmoke(loc, traj, r, g, b, a, size, lifeSpan);
			}
		}

		public void AddBlood(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, CharacterType bloodType, int skin, int l)
		{
			Blood suitableParticle = this.GetSuitableParticle<Blood>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, size, bloodType, skin);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Blood(loc, traj, r, g, b, a, size, bloodType, skin);
			}
		}

		public void AddBubble(Vector2 loc, Vector2 traj, float size, float r, float g, float b, int l)
		{
			Bubble suitableParticle = this.GetSuitableParticle<Bubble>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, r, g, b);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Bubble(loc, traj, size, r, g, b);
			}
		}

		public void AddHP(Vector2 loc, int amount, bool critical, StatusEffects statusEffect, int l)
		{
			HP suitableParticle = this.GetSuitableParticle<HP>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, amount, critical, statusEffect);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HP(loc, amount, critical, statusEffect);
			}
		}

		public void AddXP(Vector2 loc, int amount, bool bonus, int l)
		{
			XP suitableParticle = this.GetSuitableParticle<XP>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, amount, bonus);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new XP(loc, amount, bonus);
			}
		}

		public void AddScore(Vector2 loc, int amount, int l)
		{
			Score suitableParticle = this.GetSuitableParticle<Score>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, amount);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Score(loc, amount);
			}
		}

		public void AddChest(Vector2 loc, int id, bool faceRight, int l)
		{
			Chest suitableParticle = this.GetSuitableParticle<Chest>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, id, faceRight);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Chest(loc, id, faceRight);
			}
		}

		public void AddCage(Vector2 loc, int id, int l)
		{
			Cage suitableParticle = this.GetSuitableParticle<Cage>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, id);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Cage(loc, id);
			}
		}

		public void AddCoin(Vector2 loc, Vector2 traj, int flag, int l)
		{
			Coin suitableParticle = this.GetSuitableParticle<Coin>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, flag);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Coin(loc, traj, flag);
			}
		}

		public int AddBomb(Vector2 loc, int ID, int timer, int stage, int l)
		{
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Bomb(loc, ID, num, timer, stage);
				return num;
			}
			return -1;
		}

		public void AddDeathFlame(Vector2 loc, Vector2 traj, float r, float g, float b, float size, int id, bool audio, int l)
		{
			DeathFlame suitableParticle = this.GetSuitableParticle<DeathFlame>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, size, id, audio);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new DeathFlame(loc, traj, r, g, b, size, id, audio);
			}
		}

		public void AddDustHat(Vector2 loc, Vector2 traj, int l)
		{
			DustHat suitableParticle = this.GetSuitableParticle<DustHat>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new DustHat(loc, traj);
			}
		}

		public void AddDustSword(Vector2 loc, Vector2 traj, int l)
		{
			DustSword suitableParticle = this.GetSuitableParticle<DustSword>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new DustSword(loc, traj);
			}
		}

		public void AddDustSwordFall(Vector2 loc, Vector2 traj, int l)
		{
			DustSwordFall suitableParticle = this.GetSuitableParticle<DustSwordFall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new DustSwordFall(loc, traj);
			}
		}

		public void AddFootStep(Vector2 loc, float size, float a, int l)
		{
			FootStep suitableParticle = this.GetSuitableParticle<FootStep>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, a);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FootStep(loc, size, a);
			}
		}

		public void AddFootStepSnow(Vector2 loc, float size, float rot, int l)
		{
			FootStepSnow suitableParticle = this.GetSuitableParticle<FootStepSnow>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, rot);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FootStepSnow(loc, size, rot);
			}
		}

		private void AddFootStepWater(Vector2 loc, float size, int l)
		{
			FootStepWater suitableParticle = this.GetSuitableParticle<FootStepWater>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FootStepWater(loc, size);
			}
		}

		public void AddHit(Vector2 loc, Vector2 traj, int owner, int flag, int l)
		{
			this.GetSuitableHit<Hit>(l)?.Reset(loc, traj, owner, flag);
		}

		public void AddEquipment(Vector2 loc, Vector2 traj, int flag, bool bluePrint, int treasureID, int l)
		{
			this.AddEquipment(loc, traj, flag, bluePrint, treasureID, string.Empty, l);
		}

		public void AddEquipment(Vector2 loc, Vector2 traj, int flag, bool bluePrint, int treasureID, string name, int l)
		{
			Equipment suitableParticle = this.GetSuitableParticle<Equipment>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, flag, bluePrint, treasureID, name);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Equipment(loc, traj, flag, bluePrint, treasureID, name);
			}
		}

		public void AddMaterial(Vector2 loc, Vector2 traj, int flag, int l)
		{
			Material suitableParticle = this.GetSuitableParticle<Material>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, flag);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Material(loc, traj, flag);
			}
		}

		public void AddKey(Vector2 loc, Vector2 traj, int id, int l)
		{
			Key suitableParticle = this.GetSuitableParticle<Key>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, id);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Key(loc, traj, id);
			}
		}

		public void AddNote(Vector2 loc, Vector2 traj, int id, string name, int l)
		{
			Note suitableParticle = this.GetSuitableParticle<Note>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, id, name);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Note(loc, traj, id, name);
			}
		}
		public void AddUpgrade(Vector2 loc, int id, int l)
		{
			int audioID = 0;
			Upgrade suitableAudioParticle = this.GetSuitableAudioParticle<Upgrade>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, id, audioID);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Upgrade(loc, id, num);
			}
		}

		public void AddPlayerDebris(Vector2 loc, Vector2 traj, Texture2D texture, Rectangle sRect, float size, float lifeSpan, int dieType, byte r, byte g, byte b, int l)
		{
			PlayerDebris suitableParticle = this.GetSuitableParticle<PlayerDebris>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, texture, sRect, size, lifeSpan, dieType, r, g, b);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new PlayerDebris(loc, traj, texture, sRect, size, lifeSpan, dieType, r, g, b);
			}
		}

		public void AddPuff(Vector2 loc, Vector2 traj, float rot, float r, float g, float b, float a, float size, int l)
		{
			Puff suitableParticle = this.GetSuitableParticle<Puff>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, rot, r, g, b, a, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Puff(loc, traj, rot, r, g, b, a, size);
			}
		}

		public void AddShockRing(Vector2 loc, float lifeSpan, int l)
		{
			ShockRing suitableParticle = this.GetSuitableParticle<ShockRing>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new ShockRing(loc, lifeSpan);
			}
		}

		public void AddSpinBlade(Vector2 loc, CharDir dir, int l)
		{
			SpinBlade suitableParticle = this.GetSuitableParticle<SpinBlade>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, dir);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new SpinBlade(loc, dir);
			}
		}

		public void AddStone(Vector2 loc, Vector2 traj, int flag, int l)
		{
			Stone suitableParticle = this.GetSuitableParticle<Stone>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, flag);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Stone(loc, traj, flag);
			}
		}

		public void AddWhirlwind(Vector2 loc, Vector2 traj, int l)
		{
			int audioID = 0;
			Whirlwind suitableAudioParticle = this.GetSuitableAudioParticle<Whirlwind>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, traj, audioID);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Whirlwind(loc, traj, num);
			}
		}

		public void AddCannonFireBall(Vector2 loc, Vector2 traj, float scale, int strength, int owner, int l)
		{
			CannonFireBall suitableParticle = this.GetSuitableParticle<CannonFireBall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, scale, strength, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new CannonFireBall(loc, traj, scale, strength, owner);
			}
		}

		public void AddCoraProjectile(Vector2 loc, Vector2 traj, int l)
		{
			CoraProjectile suitableParticle = this.GetSuitableParticle<CoraProjectile>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new CoraProjectile(loc, traj, l);
			}
		}

		public void AddFidgetPuff(Vector2 loc, int l)
		{
			FidgetPuff suitableParticle = this.GetSuitableParticle<FidgetPuff>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FidgetPuff(loc);
			}
		}

		public void AddFidgetTriple(Vector2 loc, Vector2 traj, int owner, int spun, int l)
		{
			int audioID = 0;
			FidgetTriple suitableAudioParticle = this.GetSuitableAudioParticle<FidgetTriple>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, traj, owner, spun, audioID, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FidgetTriple(loc, traj, owner, spun, num, l);
			}
		}

		public void AddFidgetPillar(Vector2 loc, Vector2 traj, int owner, int spun, int l)
		{
			int audioID = 0;
			FidgetPillar suitableAudioParticle = this.GetSuitableAudioParticle<FidgetPillar>(l, ref audioID);
			if (suitableAudioParticle != null)
			{
				suitableAudioParticle.Reset(loc, traj, owner, spun, audioID, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FidgetPillar(loc, traj, owner, spun, num, l);
			}
		}

		public void AddFidgetBolt(Vector2 loc, CharDir dir, int owner, int spun, int l)
		{
			FidgetBolt suitableParticle = this.GetSuitableParticle<FidgetBolt>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, dir, owner, spun, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FidgetBolt(loc, dir, owner, spun, l);
			}
		}

		public void AddFlornProj(Vector2 loc, int owner, int l)
		{
			FlornProj suitableParticle = this.GetSuitableParticle<FlornProj>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FlornProj(loc, owner);
			}
		}

		public void AddFuseFireBall(Vector2 loc, Vector2 traj, int owner, int l)
		{
			FuseFireBall suitableParticle = this.GetSuitableParticle<FuseFireBall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FuseFireBall(loc, traj, owner);
			}
		}

		public void AddFusePillar(Vector2 loc, int owner, int l)
		{
			FusePillar suitableParticle = this.GetSuitableParticle<FusePillar>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FusePillar(loc, owner);
			}
		}

		public void AddFuseSeekBall(Vector2 loc, Vector2 traj, int owner, int spun, int l)
		{
			FuseSeekBall suitableParticle = this.GetSuitableParticle<FuseSeekBall>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, owner, spun);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new FuseSeekBall(loc, traj, owner, spun);
			}
		}

		public void AddGaiusSphere(Vector2 loc, Vector2 targLoc, float scale, int strength, int owner, int l)
		{
			GaiusSphere suitableParticle = this.GetSuitableParticle<GaiusSphere>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, targLoc, scale, strength, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new GaiusSphere(loc, targLoc, scale, strength, owner);
			}
		}

		public void AddLadySphere(Vector2 loc, float scale, int strength, int owner, int l)
		{
			LadySphere suitableParticle = this.GetSuitableParticle<LadySphere>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, scale, strength, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new LadySphere(loc, scale, strength, owner);
			}
		}

		public void AddSquirtFlame(Vector2 loc, Vector2 traj, int owner, int l)
		{
			SquirtFlame suitableParticle = this.GetSuitableParticle<SquirtFlame>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, owner);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new SquirtFlame(loc, traj, owner);
			}
		}

		public void AddBounceSpark(Vector2 loc, Vector2 traj, float size, int l)
		{
			BounceSpark suitableParticle = this.GetSuitableParticle<BounceSpark>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new BounceSpark(loc, traj, size);
			}
		}

		public void AddElectricBolt(Vector2 loc, int owner, float lifeSpan, int range, int boltLength, float boltWidthMult, int l)
		{
			ElectricBolt suitableParticle = this.GetSuitableParticle<ElectricBolt>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, owner, lifeSpan, range, boltLength, boltWidthMult, l);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new ElectricBolt(loc, owner, lifeSpan, range, boltLength, boltWidthMult, l);
			}
		}

		public void AddExplosion(Vector2 loc, float size, bool makeSmoke, int l)
		{
			Explosion suitableParticle = this.GetSuitableParticle<Explosion>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, makeSmoke);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Explosion(loc, size, makeSmoke);
			}
		}

		public void AddFeather(Vector2 loc, Vector2 traj, float size, float lifeSpan, int l)
		{
			Feather suitableParticle = this.GetSuitableParticle<Feather>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Feather(loc, traj, size, lifeSpan);
			}
		}

		public void AddGlowSpark(Vector2 loc, Vector2 traj, float lifeSpan, float a, float size, int l)
		{
			GlowSpark suitableParticle = this.GetSuitableParticle<GlowSpark>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, lifeSpan, a, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new GlowSpark(loc, traj, lifeSpan, a, size);
			}
		}

		public void AddGroundDust(Vector2 loc, Vector2 traj, float size, float a, int type, int l)
		{
			GroundDust suitableParticle = this.GetSuitableParticle<GroundDust>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, size, a, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new GroundDust(loc, traj, size, a, type);
			}
		}

		public void AddHitSpark(Vector2 loc, float size, float rot, CharDir dir, int l)
		{
			HitSpark suitableParticle = this.GetSuitableParticle<HitSpark>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, rot, dir);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new HitSpark(loc, size, rot, dir);
			}
		}

		public void AddLenseFlare(Vector2 loc, float lifeSpan, int type, int l)
		{
			LensFlare suitableParticle = this.GetSuitableParticle<LensFlare>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, lifeSpan, type);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new LensFlare(loc, lifeSpan, type);
			}
		}

		public void AddRaveBeam(Vector2 loc, float lifeSpan, int l)
		{
			RaveBeam suitableParticle = this.GetSuitableParticle<RaveBeam>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, lifeSpan);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new RaveBeam(loc, lifeSpan);
			}
		}

		public void AddSlash(Vector2 loc, float size, float rot, CharDir dir, int l)
		{
			Slash suitableParticle = this.GetSuitableParticle<Slash>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, rot, dir);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Slash(loc, size, rot, dir);
			}
		}

		public void AddSparkle(Vector2 loc, float r, float g, float b, float a, float size, int _animSpeed, int l)
		{
			Sparkle1 suitableParticle = this.GetSuitableParticle<Sparkle1>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, r, g, b, a, size, _animSpeed);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Sparkle1(loc, r, g, b, a, size, _animSpeed);
			}
		}

		public void AddSpawnBeam(Vector2 loc, float width, int l)
		{
			SpawnBeam suitableParticle = this.GetSuitableParticle<SpawnBeam>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, width);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new SpawnBeam(loc, width);
			}
		}

		public void AddStarburst(Vector2 loc, float size, float r, float g, float b, float a, float lifeSpan, int ownerID, bool background, int l)
		{
			Starburst suitableParticle = this.GetSuitableParticle<Starburst>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size, r, g, b, a, lifeSpan, ownerID, background);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new Starburst(loc, size, r, g, b, a, lifeSpan, ownerID, background);
			}
		}

		public void AddUpgradeBurn(Vector2 loc, float size, int l)
		{
			UpgradeBurn suitableParticle = this.GetSuitableParticle<UpgradeBurn>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, size);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new UpgradeBurn(loc, size);
			}
		}

		public void AddVerticleBeam(Vector2 loc, Vector2 traj, float r, float g, float b, float a, int width, int height, float lifeSpan, int ownerID, int l)
		{
			VerticleBeam suitableParticle = this.GetSuitableParticle<VerticleBeam>(l);
			if (suitableParticle != null)
			{
				suitableParticle.Reset(loc, traj, r, g, b, a, width, height, lifeSpan, ownerID);
				return;
			}
			int num = this.Empty(l);
			if (num > -1)
			{
				this.particle[l - this.startLayer, num] = new VerticleBeam(loc, traj, r, g, b, a, width, height, lifeSpan, ownerID);
			}
		}

		public void UpdateParticles(float frameTime, Map map, Character[] c, int l)
		{
			int num = this.ParticleMax(l);
			int num2 = 0;
			int num3 = l - this.startLayer;
			if (l == 5)
			{
				num2 = this.maxHits;
			}
			for (int i = num2; i < num; i++)
			{
				if (this.particle[num3, i].exists == Exists.Exists)
				{
					this.particle[num3, i].Update(frameTime, map, this, c, l);
				}
			}
		}

		public void DrawParticles(Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.particle[this.nLayer, 0].exists == Exists.Exists)
			{
				this.particle[this.nLayer, 0].Draw(this.sprite, particlesTex, worldScale, l);
			}
			int num = this.ParticleMax(l);
			this.nLayer = l - this.startLayer;
			for (int i = this.maxHits; i < num; i++)
			{
				if (this.particle[this.nLayer, i].exists == Exists.Exists && !this.particle[this.nLayer, i].background)
				{
					this.rState = (int)this.particle[this.nLayer, i].renderState;
					if (this.rState != 2 && this.rState != 4)
					{
						this.particle[this.nLayer, i].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			this.renderingAdditive = true;
			for (int j = this.maxHits; j < num; j++)
			{
				if (this.particle[this.nLayer, j].exists == Exists.Exists && !this.particle[this.nLayer, j].background)
				{
					this.rState = (int)this.particle[this.nLayer, j].renderState;
					if (this.rState == 1 || this.rState == 2 || this.rState == 5)
					{
						this.particle[this.nLayer, j].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.renderingAdditive = false;
		}

		public void DrawRefractParticles(Texture2D[] particlesTex, float worldScale, int l)
		{
			int num = this.ParticleMax(l);
			this.nLayer = l - this.startLayer;
			for (int i = this.maxHits; i < num; i++)
			{
				if (this.particle[this.nLayer, i].exists == Exists.Exists && !this.particle[this.nLayer, i].background && this.particle[this.nLayer, i].renderState > RenderState.AdditiveOnly)
				{
					this.particle[this.nLayer, i].Draw(this.sprite, particlesTex, worldScale, l);
				}
			}
		}

		public void DrawMapParticles(Texture2D[] particlesTex, float worldScale, int l)
		{
			if (l < this.startLayer || (l == 8 && !Game1.settings.WeatherOn))
			{
				return;
			}
			int num = this.ParticleMax(l);
			this.nLayer = l - this.startLayer;
			if (l == 5)
			{
				for (int i = this.maxHits; i < num; i++)
				{
					if (this.particle[this.nLayer, i].exists == Exists.Exists && this.particle[this.nLayer, i].background)
					{
						this.rState = (int)this.particle[this.nLayer, i].renderState;
						if (this.rState != 2 && this.rState != 4)
						{
							this.particle[this.nLayer, i].Draw(this.sprite, particlesTex, worldScale, l);
						}
					}
				}
				Game1.wManager.DrawRegions(this.sprite, Game1.map, particlesTex, worldScale, refractive: false, additive: false);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				this.renderingAdditive = true;
				for (int j = this.maxHits; j < num; j++)
				{
					if (this.particle[this.nLayer, j].exists == Exists.Exists && this.particle[this.nLayer, j].background)
					{
						this.rState = (int)this.particle[this.nLayer, j].renderState;
						if (this.rState == 1 || this.rState == 2 || this.rState == 5)
						{
							this.particle[this.nLayer, j].Draw(this.sprite, particlesTex, worldScale, l);
						}
					}
				}
				Game1.wManager.DrawRegions(this.sprite, Game1.map, particlesTex, worldScale, refractive: false, additive: true);
				this.renderingAdditive = false;
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				return;
			}
			for (int k = 0; k < num; k++)
			{
				if (this.particle[this.nLayer, k].exists == Exists.Exists)
				{
					this.rState = (int)this.particle[this.nLayer, k].renderState;
					if (this.rState != 2 && this.rState != 4)
					{
						this.particle[this.nLayer, k].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			this.renderingAdditive = true;
			for (int m = 0; m < num; m++)
			{
				if (this.particle[this.nLayer, m].exists == Exists.Exists)
				{
					this.rState = (int)this.particle[this.nLayer, m].renderState;
					if (this.rState == 1 || this.rState == 2 || this.rState == 5)
					{
						this.particle[this.nLayer, m].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.renderingAdditive = false;
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawMapRefractParticles(Texture2D[] particlesTex, float worldScale, int l)
		{
			if (l < this.startLayer)
			{
				return;
			}
			int num = this.ParticleMax(l);
			this.nLayer = l - this.startLayer;
			if (l == 5)
			{
				for (int i = 0; i < num; i++)
				{
					if (this.particle[this.nLayer, i].exists == Exists.Exists && this.particle[this.nLayer, i].background && this.particle[this.nLayer, i].renderState > RenderState.AdditiveOnly)
					{
						this.particle[this.nLayer, i].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
				return;
			}
			for (int j = 0; j < num; j++)
			{
				if (this.particle[this.nLayer, j].exists == Exists.Exists && this.particle[this.nLayer, j].renderState > RenderState.AdditiveOnly)
				{
					this.particle[this.nLayer, j].Draw(this.sprite, particlesTex, worldScale, l);
				}
			}
		}

		public void DrawHudParticles(Texture2D[] particlesTex, float worldScale, int l)
		{
			if (l < this.startLayer)
			{
				return;
			}
			int num = this.ParticleMax(l);
			this.nLayer = l - this.startLayer;
			for (int i = 0; i < num; i++)
			{
				if (this.particle[this.nLayer, i].exists == Exists.Exists)
				{
					this.rState = (int)this.particle[this.nLayer, i].renderState;
					if (this.rState != 2 && this.rState != 4)
					{
						this.particle[this.nLayer, i].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			this.renderingAdditive = true;
			for (int j = 0; j < num; j++)
			{
				if (this.particle[this.nLayer, j].exists == Exists.Exists)
				{
					this.rState = (int)this.particle[this.nLayer, j].renderState;
					if (this.rState == 1 || this.rState == 2)
					{
						this.particle[this.nLayer, j].Draw(this.sprite, particlesTex, worldScale, l);
					}
				}
			}
			this.renderingAdditive = false;
		}

		public void DrawLightSources(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, float rot)
		{
			int num = this.ParticleMax(5);
			this.nLayer = 5 - this.startLayer;
			for (int i = 0; i < num; i++)
			{
				if (this.particle[this.nLayer, i].exists == Exists.Exists && this.particle[this.nLayer, i].maskGlow > 0f)
				{
					float num2 = this.particle[this.nLayer, i].maskGlow + 0.75f;
					sprite.Draw(particlesTex[2], (this.particle[this.nLayer, i].location * worldScale - Game1.Scroll) / 4f, new Rectangle(4000, 2796, 95, 95), new Color(1f, 1f, 1f, num2 / 4f), rot + (float)i, new Vector2(48f, 48f), worldScale * MathHelper.Max(num2, 0.75f), SpriteEffects.None, 1f);
				}
			}
			for (int j = 0; j < Game1.map.brightRegions.Count; j++)
			{
				if (Game1.map.brightRegions[j] != null)
				{
					Game1.map.brightRegions[j].Draw(sprite, particlesTex[2]);
				}
			}
			for (int k = 0; k < Game1.map.warpRegions.Count; k++)
			{
				if (Game1.map.warpRegions[k] != null)
				{
					sprite.Draw(particlesTex[2], (new Vector2(Game1.map.warpRegions[k].Region.X + 75, Game1.map.warpRegions[k].Region.Y) * worldScale - Game1.Scroll) / 4f, new Rectangle(4000, 2796, 95, 95), Color.White, rot + (float)k, new Vector2(48f, 48f), worldScale * 2f, SpriteEffects.None, 1f);
				}
			}
		}

		public int FindTarg(Character[] c, Particle p, float range)
		{
			range /= Game1.worldScale;
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < c.Length; i++)
			{
				if (i != p.owner && c[i].Exists == CharExists.Exists && c[p.owner].Exists == CharExists.Exists && c[i].NPC == NPCType.None && c[i].Team != c[p.owner].Team)
				{
					float num3 = (p.location - c[i].Location).Length();
					if ((num == -1 || num3 < num2) && num3 < range)
					{
						num2 = num3;
						num = i;
					}
				}
			}
			return num;
		}

		protected void ChaseTarg(Character[] c, Particle p, int targ, float speed)
		{
			if (c[targ].Exists == CharExists.Exists)
			{
				if (p.location.X < c[targ].Location.X)
				{
					p.trajectory += new Vector2(speed, 0f);
				}
				else
				{
					p.trajectory -= new Vector2(speed, 0f);
				}
				if (p.location.Y < c[targ].Location.Y - (float)(c[targ].Height / 2))
				{
					p.trajectory += new Vector2(0f, speed);
				}
				else
				{
					p.trajectory -= new Vector2(0f, speed);
				}
			}
		}

		public void ChaseTargDirect(Character[] c, Particle p, int targ, float offset)
		{
			if (c[targ].Exists == CharExists.Exists)
			{
				p.trajectory += -(p.location - (c[targ].Location - new Vector2(0f, offset))) / 1.5f;
			}
		}

		protected void SpinVacuum(Character[] c, Particle p)
		{
			if (p.isSpun < 1.5f)
			{
				if (c[0].State == CharState.Air)
				{
					p.isSpun += Game1.FrameTime * 2f;
				}
				else
				{
					p.isSpun += Game1.FrameTime;
				}
				if (p.isSpun > 1.5f)
				{
					p.isSpun = 1.5f;
				}
			}
			float x = Math.Abs(p.location.X - c[0].Location.X) * 1.5f;
			float num = Math.Abs(p.location.Y - c[0].Location.Y) * 1.5f;
			int num2 = 220;
			if (c[0].Face == CharDir.Left)
			{
				num2 = -num2;
			}
			if (p.location.X < c[0].Location.X + (float)num2)
			{
				p.trajectory += new Vector2(x, 0f);
			}
			else
			{
				p.trajectory -= new Vector2(x, 0f);
			}
			if (p.location.Y < c[0].Location.Y - 280f)
			{
				p.trajectory += new Vector2(0f, num * 0.75f);
			}
			else
			{
				p.trajectory -= new Vector2(0f, num);
			}
		}

		protected void SpinClear(Character[] c, Particle p, float speed)
		{
			p.isSpun += Game1.FrameTime;
			float x = Math.Abs(p.location.X - c[0].Location.X) / 20f * (0f - speed);
			float y = Math.Abs(p.location.Y - c[0].Location.Y) / 20f * (0f - speed);
			int num = 220;
			if (c[0].Face == CharDir.Left)
			{
				num = -num;
			}
			if (p.location.X < c[0].Location.X + (float)num)
			{
				p.trajectory += new Vector2(x, 0f);
			}
			else
			{
				p.trajectory -= new Vector2(x, 0f);
			}
			if (p.location.Y < c[0].Location.Y - 280f)
			{
				p.trajectory += new Vector2(0f, y);
			}
			else
			{
				p.trajectory -= new Vector2(0f, y);
			}
		}

		public bool Seek(Character[] c, Particle p, int targ, float speed, float range)
		{
			if (p.owner < 1)
			{
				if (p.isSpun < 10f)
				{
					if (p is Coin || p is Equipment || p is Material || p is Stone)
					{
						if (c[0].AnimName == "attack01" || c[0].AnimName == "airspin")
						{
							this.SpinVacuum(c, p);
							return true;
						}
					}
					else if (c[0].AnimName == "attack01" || c[0].AnimName == "airspin" || c[0].AnimName == "attackairdown")
					{
						this.SpinVacuum(c, p);
						return true;
					}
				}
				else if (p.isSpun == 10f)
				{
					if (targ < 0)
					{
						targ = this.FindTarg(c, p, range);
					}
					if (targ > -1)
					{
						this.ChaseTarg(c, p, targ, speed);
					}
					return true;
				}
				return false;
			}
			if (c[0].AnimName == "attack01" || c[0].AnimName == "airspin" || c[0].AnimName == "attackairdown")
			{
				this.SpinClear(c, p, speed);
				return false;
			}
			if (targ < 0)
			{
				targ = this.FindTarg(c, p, range);
			}
			if (targ > -1)
			{
				this.ChaseTarg(c, p, targ, speed);
			}
			return true;
		}

		public Vector2 TargetLocation(Character[] c, Particle p, float range)
		{
			int num = this.FindTarg(c, p, range);
			if (num > -1)
			{
				return c[num].Location;
			}
			return p.location;
		}

		public void MakeProjectile(Vector2 loc, Vector2 traj, float rot, float scale, CharacterType charType, CharDir face, int owner, int l)
		{
			int num = 1;
			if (face == CharDir.Left)
			{
				num = -1;
			}
			switch (charType)
			{
			case CharacterType.Dust:
				switch (Game1.stats.projectileType)
				{
				case 0:
				{
					for (int n = 0; n < 3; n++)
					{
						this.AddFidgetTriple(loc, new Vector2(traj.X * (float)num, traj.Y - 600f + (float)(n * 400)) + Rand.GetRandomVector2(-200f, 200f, -200f, 200f), owner, 0, l);
					}
					this.AddFidgetPuff(loc, l);
					break;
				}
				case 1:
				{
					for (int num2 = 0; num2 < 3; num2++)
					{
						this.AddFidgetPillar(loc, new Vector2(traj.X * (float)num, traj.Y), owner, 0, l);
					}
					this.AddExplosion(loc, 1f, makeSmoke: false, 6);
					break;
				}
				case 2:
				{
					for (int m = 0; m < 2; m++)
					{
						this.AddFidgetBolt(loc, face, owner, 0, l);
						this.AddElectricBolt(loc, 100, Rand.GetRandomFloat(0.2f, 0.6f), 800, 40, 0.4f, 5);
					}
					break;
				}
				}
				this.AddLenseFlare(loc, 0.5f, 0, 5);
				break;
			case CharacterType.Fuse:
				switch (this.character[owner].AnimName)
				{
				case "attack01":
				{
					for (int j = 0; j < 6; j++)
					{
						this.AddFuseSeekBall(loc, Rand.GetRandomVector2(-300f, 300f, -300f, 0f), owner, 0, l);
					}
					for (int k = 0; k < 4; k++)
					{
						this.AddExplosion(loc + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(0.75f, 1f), makeSmoke: false, 6);
					}
					VibrationManager.SetScreenShake(0.5f);
					break;
				}
				case "attack00":
					this.AddFuseFireBall(loc, new Vector2(100 * num, 0f), owner, 5);
					this.AddExplosion(loc, 1f, makeSmoke: false, 6);
					VibrationManager.SetScreenShake(0.2f);
					Sound.PlayCue("fuse_fire_ball", loc, (this.character[0].Location - loc).Length() / 4f);
					break;
				case "attack02":
				{
					for (int i = 0; i < 3; i++)
					{
						this.AddFusePillar(new Vector2(this.character[0].Location.X - 900f + (float)(900 * i), this.character[0].Location.Y), owner, 6);
					}
					VibrationManager.SetScreenShake(0.5f);
					break;
				}
				}
				break;
			case CharacterType.Florn:
				this.AddFlornProj(loc, owner, l);
				break;
			case CharacterType.SquirtBug:
				this.AddBubbleSquirt(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 1000f * scale, 0.5f * scale, owner, 0, l);
				break;
			case CharacterType.StoneCutter:
				this.AddBubbleSquirt(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 1000f * scale, 0.5f * scale, owner, 0, l);
				break;
			}
		}

		public void MakeBlood(Vector2 loc, Vector2 traj, CharacterType bloodType, int skin, Color color, int l)
		{
			if (Game1.map.bloodCount < 12)
			{
				for (int i = 0; i < 5; i++)
				{
					Game1.map.bloodCount++;
					this.AddBlood(loc, new Vector2(traj.X * Rand.GetRandomFloat(1f, 10f), traj.Y * Rand.GetRandomFloat(10f, 20f)), (int)color.R, (int)color.G, (int)color.B, 1f, 1f, bloodType, skin, l);
				}
			}
		}

		public void MakeSlash(CharacterType ownerType, CharacterType bloodType, Vector2 loc, float size, string attackType, int skin, Color color, CharDir dir)
		{
			int num = 1;
			int num2 = ((dir != CharDir.Right) ? 1 : (-1));
			Vector2 vector = new Vector2(100f * (float)(-num2), -20f);
			float rot = 0f;
			Vector2 vector2 = new Vector2(loc.X + Rand.GetRandomFloat(-80f, 80f), loc.Y + Rand.GetRandomFloat(-80f, 80f));
			switch (attackType)
			{
			case "up":
				rot = 1.3f * (float)num2;
				vector = new Vector2(50f * (float)(-num2), -60f);
				break;
			case "uphigh":
				num = 2;
				size *= 1.2f;
				rot = 1.3f * (float)num2;
				vector = new Vector2(50f * (float)(-num2), -60f);
				break;
			case "down":
				rot = -1.3f * (float)num2;
				vector = new Vector2(75f * (float)(-num2), 30f);
				break;
			case "KO":
				num = 2;
				size *= 1.2f;
				break;
			case "spin":
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					num = 0;
				}
				rot = 1.3f * (float)num2;
				vector = new Vector2(50f * (float)(-num2), -60f);
				break;
			case "hazard":
				num = 2;
				size *= 1.2f;
				Sound.PlayCue("slash_body", vector2, (vector2 - this.character[0].Location).Length());
				this.MakeBlood(vector2, vector, bloodType, skin, color, 5);
				if (Game1.map.slashCount < 6)
				{
					this.AddSlash(vector2, size, rot, dir, 5);
					this.AddHitSpark(vector2, size * 2f, rot, dir, 5);
				}
				return;
			}
			if (ownerType == CharacterType.Dust)
			{
				this.MakeBlood(vector2, vector, bloodType, skin, color, 5);
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					this.AddBounceSpark(vector2 + new Vector2(0f, -100f), vector * Rand.GetRandomFloat(5f, 10f), 0.25f, 5);
				}
				for (int i = 0; i < num; i++)
				{
					if (Game1.map.slashCount < 6)
					{
						this.AddSlash(vector2, size, rot, dir, 5);
						this.AddHitSpark(vector2, size * 2f, rot, dir, 5);
					}
				}
				Sound.PlayCue("slash_body");
				if (bloodType >= (CharacterType)1000)
				{
					Sound.PlayCue("destruct_hit");
				}
			}
			else if (bloodType == CharacterType.Dust)
			{
				Sound.PlayCue("dusthurthit");
				this.MakeBlood(vector2, vector, bloodType, skin, color, 5);
			}
			else
			{
				Sound.PlayCue("slash_body", vector2, (vector2 - this.character[0].Location).Length() / 1.5f);
				this.MakeBlood(vector2, vector, bloodType, skin, color, 5);
				if (Game1.map.slashCount < 6)
				{
					this.AddSlash(vector2, size, rot, dir, 5);
					this.AddHitSpark(vector2, size * 2f, rot, dir, 5);
				}
			}
		}

		public void MakeHazard(int flag, Vector2 pos, Vector2 traj, float rot, float scale, int type, int l)
		{
			switch (flag)
			{
			case 55:
				this.AddEmitFlame(pos, traj, rot, scale, colliding: true, type, l);
				break;
			case 56:
				if (type > 0)
				{
					this.AddEmitRocks(pos + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), traj, 0f, 2f, type, 5);
				}
				break;
			case 57:
				this.AddEmitLava(pos, traj, scale, colliding: true, type, l);
				break;
			}
		}

		public void MakeFootStep(Vector2 loc, int ID, CharDir face, float size, float rot, float alpha, int l, TerrainType terrainType)
		{
			switch (terrainType)
			{
			case TerrainType.Dirt:
				this.AddFootStep(loc, size, alpha, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_grass");
				}
				break;
			case TerrainType.Wood:
				this.AddFootStep(loc, size, alpha, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_wood");
				}
				break;
			case TerrainType.WoodHeavy:
				this.AddFootStep(loc, size, alpha, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_woodheavy");
				}
				break;
			case TerrainType.Rock:
				this.AddFootStep(loc, size, alpha, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_rock");
				}
				break;
			case TerrainType.Water:
				this.AddFootStepWater(loc, size * 1.75f, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_water");
					this.AddSpray(loc, Rand.GetRandomVector2(-50f, 50f, -300f, -150f) + new Vector2((face == CharDir.Left) ? (-200) : 200, 0f), 0.6f, 0, 3, l);
				}
				break;
			case TerrainType.Snow:
				if (!this.character[ID].AnimName.StartsWith("crouch") && !this.character[ID].WallInWay)
				{
					this.AddFootStepSnow(loc, size, rot, l);
				}
				if (ID == 0)
				{
					Sound.PlayCue("footstep_snow");
					this.AddSpray(loc, Rand.GetRandomVector2(-50f, 50f, -300f, -150f) + new Vector2((face == CharDir.Left) ? (-200) : 200, 0f), 0.4f, 1, 3, l);
				}
				break;
			case TerrainType.Ice:
				this.AddFootStepWater(loc, size * 1.75f, l);
				if (ID == 0)
				{
					Sound.PlayCue("footstep_ice");
					this.AddSpray(loc, Rand.GetRandomVector2(-50f, 50f, -300f, -150f) + new Vector2((face == CharDir.Left) ? (-200) : 200, 0f), 0.6f, 0, 3, l);
				}
				break;
			}
			if (ID == 0)
			{
				Sound.PlayCue("footstep_foley");
			}
		}

		public void MakeGroundDust(Vector2 loc, Vector2 traj, int ID, float size, float alpha, int type, int l, TerrainType terrainType)
		{
			switch (terrainType)
			{
			case TerrainType.Dirt:
			case TerrainType.Wood:
			case TerrainType.Rock:
				this.AddGroundDust(loc, traj, size, alpha, type, l);
				break;
			case TerrainType.Water:
				Sound.PlayCue("footstep_water");
				switch (type)
				{
				case 0:
				{
					this.AddGroundDust(loc, traj, size / 2f, alpha / 2f, 2, l);
					for (int j = 0; j < 4; j++)
					{
						this.AddFootStepWater(loc + new Vector2(j * 100 - 150, 0f) * size, size * 4f, l);
						this.AddSpray(loc + new Vector2((float)(j * 100 - 150) * size, -24f), new Vector2(0f, Rand.GetRandomFloat(-800f, -600f)) * size, 0.6f, 0, 8, l);
					}
					break;
				}
				case 1:
					this.AddGroundDust(loc, traj, size, alpha / 2f, 3, l);
					this.AddSpray(loc, new Vector2(traj.X * 2f, Rand.GetRandomFloat(-800f, -200f)) * size, 0.6f, 0, 20, l);
					break;
				}
				break;
			case TerrainType.Snow:
				Sound.PlayCue("footstep_snow");
				switch (type)
				{
				case 0:
				{
					int num = ((ID == 0) ? 4 : 2);
					for (int i = 0; i < num; i++)
					{
						this.AddFootStepSnow(loc + new Vector2((float)(i * 100 - 150) * size, -24f), size * 2f, 0f, l);
						this.AddSpray(loc + new Vector2(i * 100 - 150, 0f) * size, new Vector2(0f, Rand.GetRandomFloat(-1000f, -600f)) * size, size * 1.5f, 1, 8, l);
					}
					break;
				}
				case 1:
					this.AddSpray(loc, new Vector2(traj.X, Rand.GetRandomFloat(-1000f, -600f)) * size, size, 1, 8, l);
					this.AddGroundDust(loc, traj, size, alpha / 2f, 5, l);
					break;
				}
				break;
			}
		}
	}
}

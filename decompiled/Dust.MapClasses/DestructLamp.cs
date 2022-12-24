using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class DestructLamp
	{
		private Vector2 location;

		private Vector2 birthLoc;

		private int Id;

		private string mapName;

		private bool exists = true;

		public int GetID => this.Id;

		public Vector2 GetLocation
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
			}
		}

		public bool Exists
		{
			get
			{
				return this.exists;
			}
			set
			{
				this.exists = value;
			}
		}

		public string GetMapName => this.mapName;

		public DestructLamp(string path, int _Id, Vector2 loc)
		{
			this.mapName = path;
			this.Id = _Id;
			this.location = (this.birthLoc = loc);
		}

		public void Draw(SpriteBatch sprite, Map map, Texture2D[] particlesTex)
		{
			float x = (float)Math.Sin((double)map.MapSegFrame * 2.0 + (double)this.Id) * 50f;
			float y = (float)Math.Sin((double)map.MapSegFrame * 4.0 + (double)this.Id) * 50f;
			this.location = this.birthLoc + new Vector2(x, y);
			Vector2 position = this.location * Game1.worldScale - Game1.Scroll;
			if (new Rectangle(-200, -200, Game1.screenWidth + 400, Game1.screenHeight + 400).Contains((int)position.X, (int)position.Y))
			{
				map.GetTransVal();
				float a = (float)Math.Sin((double)map.MapSegFrame * 800.0 + (double)this.Id);
				float num = 0.8f;
				sprite.Draw(particlesTex[2], position, new Rectangle(0, 1000, 194, 240), Color.White, 0f, new Vector2(117f, 110f), Game1.worldScale * num, SpriteEffects.None, 0f);
				sprite.Draw(particlesTex[2], position, new Rectangle(194, 1000, 196, 240), new Color(1f, 1f, 1f, a), 0f, new Vector2(157f, 110f), Game1.worldScale * num, SpriteEffects.None, 0f);
				if (Game1.longSkipFrame > 3)
				{
					Game1.pManager.AddGlowSpark(this.location + (new Vector2(-45f, 10f) + Rand.GetRandomVector2(-50f, 50f, -10f, 100f) * num), new Vector2(0f, Rand.GetRandomFloat(0f, 100f)), Rand.GetRandomFloat(0.2f, 1f), 1f, 1f, 6);
				}
			}
		}

		public void KillMe(ParticleManager pMan, CharDir dir)
		{
			ChallengeManager cManager = Game1.cManager;
			this.exists = false;
			if (cManager.currentChallenge > -1)
			{
				cManager.AddScore(500, this.location);
			}
			for (int i = 0; i < cManager.challengeArenas[cManager.currentChallenge].lampList.Count; i++)
			{
				if (cManager.challengeArenas[cManager.currentChallenge].lampList[i].GetLocation == this.birthLoc)
				{
					cManager.challengeArenas[cManager.currentChallenge].lampList[i].exists = false;
				}
			}
			float num = 0.8f;
			Math.Sin((double)Game1.map.MapSegFrame * 2.0 + (double)this.Id);
			Math.Sin((double)Game1.map.MapSegFrame * 4.0 + (double)this.Id);
			int num2 = ((dir != CharDir.Right) ? 1 : (-1));
			Sound.PlayCue("challenge_lamp_destroy");
			pMan.AddFidgetPuff(this.location, 5);
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 10; k++)
				{
					if (j < 1 || k > 5)
					{
						pMan.AddLampDebris(this.location + Rand.GetRandomVector2(-70f, 70f, -100f, 0f) * num, new Vector2((float)num2 * Rand.GetRandomFloat(-400f, -40f), Rand.GetRandomFloat(-400f, -100f)), num, k, Rand.GetRandomInt(5, 7));
					}
				}
			}
		}
	}
}

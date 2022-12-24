using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class DestructWall
	{
		private int Id;

		public Vector2 location;

		public bool Exists = true;

		public int debrisType;

		public int bombType;

		private float MaxHP;

		public float HP;

		public float hurtTime;

		public float shakeTime;

		private float glowFrame;

		private SpriteEffects spriteDir;

		public DestructWall(int _Id, Vector2 loc, int _bombType, bool flip)
		{
			this.Id = _Id;
			this.location = loc;
			this.HP = 70 + 25 * Game1.stats.gameDifficulty;
			this.MaxHP = (this.HP = 100f);
			if (Game1.map.path.Contains("mansion"))
			{
				this.MaxHP = (this.HP = 50f);
			}
			this.debrisType = 1000 + Game1.dManager.TextureType(Game1.map.path);
			this.bombType = _bombType;
			this.spriteDir = (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
		}

		public void Update(float gameTime, Map map, DestructableManager dMan, Character[] c)
		{
			if (this.hurtTime > 0f)
			{
				this.hurtTime -= gameTime;
			}
			if (this.shakeTime > 0f)
			{
				this.shakeTime -= gameTime;
			}
			if (this.HP < this.MaxHP * 0.9f)
			{
				this.glowFrame += Game1.HudTime * (this.MaxHP - this.HP) / 10f;
				if (this.glowFrame > 6.28f)
				{
					this.glowFrame -= 6.28f;
				}
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D destTex)
		{
			if (destTex != null && !destTex.IsDisposed)
			{
				Vector2 position = this.location * Game1.worldScale - Game1.Scroll;
				if (this.shakeTime > 0f)
				{
					position += Rand.GetRandomVector2(-50f, 50f, -20f, 20f) * this.shakeTime * Game1.FrameTime * 20f;
				}
				sprite.Draw(destTex, position, new Rectangle(0, 0, 400, 900), Color.White, 0f, new Vector2(200f, 870f), Game1.worldScale, this.spriteDir, 0f);
				if (this.bombType > 0)
				{
					sprite.Draw(destTex, position, new Rectangle(400, 0, 400, 900), Color.White * Rand.GetRandomFloat(0.8f, 1.1f), 0f, new Vector2(200f, 870f), Game1.worldScale, this.spriteDir, 0f);
				}
				if (this.glowFrame > 0f)
				{
					sprite.End();
					sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
					sprite.Draw(color: new Color(1f, 1f, 1f, Math.Abs((float)Math.Sin(this.glowFrame)) * (1f - this.HP / this.MaxHP)), texture: destTex, position: position, sourceRectangle: new Rectangle(0, 0, 400, 900), rotation: 0f, origin: new Vector2(200f, 870f), scale: Game1.worldScale, effects: this.spriteDir, layerDepth: 0f);
					sprite.End();
					sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				}
			}
			else if (Game1.longSkipFrame > 3 && Game1.map.GetTransVal() <= 0f)
			{
				Game1.dManager.SetDestTextures();
			}
		}

		public void KillMe(ParticleManager pMan, CharDir dir)
		{
			Game1.navManager.RevealMap[Game1.navManager.NavPath].DestructableList[this.Id].Stage = 1;
			pMan.AddShockRing(this.location, 1f, 5);
			VibrationManager.SetScreenShake(0.75f);
			VibrationManager.SetBlast(0.25f, this.location);
			Game1.map.MapSegFrameSpeed = 0.4f;
			Sound.PlayCue("destruct_die");
			this.Exists = false;
			CharacterType bloodType = (CharacterType)this.debrisType;
			int num = ((dir != CharDir.Right) ? 1 : (-1));
			Game1.stats.GetChestFromFile(Game1.navManager.RevealMap[Game1.navManager.NavPath].DestructableList[this.Id].UniqueID+"wall", pMan);
			for (int i = 0; i < 30; i++)
			{
				pMan.AddBlood(this.location + Rand.GetRandomVector2(-200f, 200f, -700f, 0f), new Vector2((float)num * Rand.GetRandomFloat(-400f, -40f), Rand.GetRandomFloat(-600f, -200f)), 1f, 1f, 1f, 1f, 2.7f, bloodType, 0, Rand.GetRandomInt(5, 7));
			}
			Game1.navManager.CheckRegionTreasure(pMan);
		}
	}
}

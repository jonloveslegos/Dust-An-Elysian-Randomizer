using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Whirlwind : Particle
	{
		public Whirlwind(Vector2 loc, Vector2 traj, int audioID)
		{
			this.Reset(loc, traj, audioID);
		}

		public void Reset(Vector2 loc, Vector2 traj, int audioID)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.trajectory = traj;
			base.flag = audioID;
			Sound.PlayFollowParticleCue("whirlwind", base.flag, 5);
			Game1.wManager.precipCount++;
			base.renderState = RenderState.AllEffects;
			base.exists = Exists.Exists;
		}

		public override bool InitAction(int type)
		{
			switch (type)
			{
			case 0:
				base.trajectory = Game1.character[0].Location + new Vector2(0f, -300f);
				if ((base.trajectory - base.location).Length() < 40f)
				{
					return true;
				}
				break;
			case 1:
				base.trajectory.X = -300f;
				break;
			}
			return false;
		}

		public override void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = base.location;
			_exists = base.exists != Exists.Dead;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.GameLocation(l);
			base.location += (base.trajectory - base.location) * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			base.renderState = RenderState.AllEffects;
			Vector2 vector = base.GameLocation(l);
			for (int i = 0; i < 40; i++)
			{
				if (i % 2 == 0 || !Game1.refractive)
				{
					sprite.Draw(sourceRectangle: new Rectangle(642 + 120 * Rand.GetRandomInt(0, 2), 3570 + 30 * Rand.GetRandomInt(0, 2), 120, 30), texture: particlesTex[2], position: vector + new Vector2((float)Math.Cos(Game1.map.MapSegFrameLocked * 60f + (float)i / 10f) * 50f, (float)(i * 10) * worldScale), color: new Color(1f, 1f, 1f, 0.2f), rotation: 0f, origin: new Vector2(60f, 15f), scale: (3f - (float)i * 0.05f) * worldScale * Rand.GetRandomFloat(0.8f, 1.5f), effects: SpriteEffects.None, layerDepth: 1f);
				}
			}
		}
	}
}

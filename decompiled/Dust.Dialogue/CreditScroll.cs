using System;
using Dust.Audio;
using Dust.MapClasses;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dust.Dialogue
{
	public class CreditScroll
	{
		private float timer;

		private float creditScroll;

		private int scrollStage;

		private Texture2D background;

		private Song[] voice = new Song[1];

		private string[] creditStaticString;

		private string[] creditScrollString;

		private int[] entryLoc;

		public CreditScroll()
		{
			WeatherAudio.Play(WeatherAudioType.Silent);
			Music.Play("silent");
			this.scrollStage = 0;
			this.timer = 1f;
			this.creditScroll = Game1.screenHeight;
			this.voice[0] = Game1.GetPortraitContent0().Load<Song>("voice/endcredits");
			MediaPlayer.Volume = Sound.masterMusicVolume;
			VoiceManager.PlayVoice(this.voice[0]);
			int num = 5;
			this.creditStaticString = new string[num];
			for (int i = 0; i < num; i++)
			{
				this.creditStaticString[i] = Game1.bigText.WordWrap(Strings_Credits.ResourceManager.GetString("credits" + $"{i:D3}"), 0.8f, (float)Game1.screenWidth * 0.75f, TextAlign.LeftAndCenter);
			}
			float num2 = 0.7f;
			string text = string.Empty;
			for (int j = num; j < 9; j++)
			{
				text = text + "\n\n\n\n" + Strings_Credits.ResourceManager.GetString("credits" + $"{j:D3}");
			}
			_ = Game1.bigFont.LineSpacing;
			this.creditScrollString = text.Split('\n');
			for (int k = 0; k < this.creditScrollString.Length; k++)
			{
				this.creditScrollString[k] = Game1.bigText.WordWrap(this.creditScrollString[k], num2, (float)Game1.screenWidth * 0.8f, TextAlign.LeftAndCenter);
			}
			this.entryLoc = new int[this.creditScrollString.Length];
			for (int l = 0; l < this.entryLoc.Length; l++)
			{
				this.entryLoc[l] = (int)(Game1.bigFont.MeasureString(this.creditScrollString[l]).Y * num2);
			}
		}

		public void ExitCredits()
		{
			this.creditStaticString = null;
			this.creditScrollString = null;
			this.entryLoc = null;
			VoiceManager.StopVoice();
			Game1.GetTitleContent().Unload();
			Game1.gameMode = Game1.GameModes.Game;
		}

		public void Draw(SpriteBatch sprite, GraphicsDevice device, float hudTime, Texture2D nullTex)
		{
			this.timer -= hudTime;
			Game1.bigText.Color = Color.White;
			device.Clear(Color.Black);
			sprite.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
			float num = 3f;
			float num2 = 5f;
			float num3 = 0.5f;
			float size = 0.8f;
			Vector2 pos = new Vector2(0f, (float)Game1.screenHeight * 0.7f);
			if (this.scrollStage == 0)
			{
				if (this.timer < 0f)
				{
					this.background = Game1.GetTitleContent().Load<Texture2D>("gfx/splash/credits_01");
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 1)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[0], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num2;
				}
			}
			else if (this.scrollStage == 2)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[0], size, Game1.screenWidth, TextAlign.Center);
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 3)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[0], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / num));
				if (this.timer < 0f)
				{
					this.background = Game1.GetTitleContent().Load<Texture2D>("gfx/splash/credits_02");
					this.scrollStage++;
					this.timer = num3;
				}
			}
			else if (this.scrollStage == 4)
			{
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 5)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[1], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num2;
				}
			}
			else if (this.scrollStage == 6)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[1], size, Game1.screenWidth, TextAlign.Center);
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 7)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[1], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / num));
				if (this.timer < 0f)
				{
					this.background = Game1.GetTitleContent().Load<Texture2D>("gfx/splash/credits_03");
					this.scrollStage++;
					this.timer = num3;
				}
			}
			else if (this.scrollStage == 8)
			{
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 9)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[2], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num2;
				}
			}
			else if (this.scrollStage == 10)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[2], size, Game1.screenWidth, TextAlign.Center);
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 11)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[2], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / num));
				if (this.timer < 0f)
				{
					this.background = Game1.GetTitleContent().Load<Texture2D>("gfx/splash/credits_04");
					this.scrollStage++;
					this.timer = num3;
				}
			}
			else if (this.scrollStage == 12)
			{
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 13)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[3], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num2;
				}
			}
			else if (this.scrollStage == 14)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[3], size, Game1.screenWidth, TextAlign.Center);
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 15)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[3], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / num));
				if (this.timer < 0f)
				{
					this.background = Game1.GetTitleContent().Load<Texture2D>("gfx/splash/credits_05");
					this.scrollStage++;
					this.timer = num3;
				}
			}
			else if (this.scrollStage == 16)
			{
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 17)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[4], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num2;
				}
			}
			else if (this.scrollStage == 18)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[4], size, Game1.screenWidth, TextAlign.Center);
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num;
				}
			}
			else if (this.scrollStage == 19)
			{
				sprite.Draw(this.background, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, this.background.Width, this.background.Height), Color.White, 0f, new Vector2(this.background.Width, this.background.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(pos, this.creditStaticString[4], size, Game1.screenWidth, TextAlign.Center);
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / num));
				if (this.timer < 0f)
				{
					this.scrollStage++;
					this.timer = num3;
					this.scrollStage = 20;
				}
			}
			else if (this.scrollStage >= 20 && this.scrollStage < 23)
			{
				size = 0.7f;
				bool flag = true;
				int num4 = 0;
				for (int i = 0; i < this.creditScrollString.Length; i++)
				{
					int num5 = (int)(this.creditScroll + (float)num4);
					if (num5 > -this.entryLoc[i] && num5 < Game1.screenHeight)
					{
						Game1.bigText.DrawText(new Vector2(0f, num5), this.creditScrollString[i], size, Game1.screenWidth, TextAlign.Center);
					}
					num4 += this.entryLoc[i];
					if (i == this.creditScrollString.Length - 1 && (float)num4 + this.creditScroll < (float)Math.Min(Game1.screenHeight / 2, 400))
					{
						if (this.scrollStage == 20)
						{
							this.timer = 4f;
							this.scrollStage++;
						}
						flag = false;
					}
				}
				if (flag)
				{
					this.creditScroll -= hudTime * 74f;
				}
				if (this.scrollStage == 21 && this.timer < 0f)
				{
					this.timer = 4f;
					this.scrollStage++;
				}
				if (this.scrollStage == 22)
				{
					sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 1f - this.timer / 4f));
					if (this.timer < 0f)
					{
						this.scrollStage++;
					}
				}
			}
			else
			{
				this.ExitCredits();
			}
			sprite.End();
		}
	}
}

using System;
using Dust.Audio;
using Dust.MapClasses;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dust.HUD
{
	public class Chapter
	{
		private int chapterStage;

		private float textFade1;

		private float textFade2;

		private float textScroll;

		private float textGlow1;

		private float textGlow2;

		private string chapterTitle;

		private string chapterSubtitle;

		private Map map;

		public Chapter(int chapter, Map _map)
		{
			this.GetChapter(chapter);
			this.map = _map;
			Music.Play("silent");
			Game1.pManager.ResetHud();
			this.chapterStage = 0;
			this.textFade1 = (this.textFade2 = (this.textScroll = 0f));
			this.map.transInFrame = 2f;
			Game1.GetTitleContent().Unload();
			Game1.video = Game1.GetTitleContent().Load<Video>("video/chapter");
		}

		private void GetChapter(int chapter)
		{
			this.chapterSubtitle = Strings_Chapters.ResourceManager.GetString("Chapter_" + chapter);
			this.chapterTitle = Strings_Chapters.Title + " " + chapter;
		}

		public void Update(float hudTime)
		{
			Sound.weatherVolume = -1f;
			this.textScroll += hudTime * 10f;
			if (this.chapterStage < 3)
			{
				if (this.textFade1 > 0f && this.textFade1 < 0.25f)
				{
					this.textGlow1 = Math.Min(this.textGlow1 + hudTime * 2f, 1f);
				}
				else
				{
					this.textGlow1 = Math.Max(this.textGlow1 - hudTime / 4f, 0f);
				}
				if (this.textFade2 > 0f && this.textFade2 < 0.25f)
				{
					this.textGlow2 = Math.Min(this.textGlow2 + hudTime * 2f, 1f);
				}
				else
				{
					this.textGlow2 = Math.Max(this.textGlow2 - hudTime / 4f, 0f);
				}
			}
			switch (this.chapterStage)
			{
			case 0:
				while (!this.map.mapAssetsLoaded)
				{
				}
				this.textFade1 = -1.5f;
				this.chapterStage++;
				Game1.videoPlayer.Play(Game1.video);
				Game1.videoPlayer.IsLooped = false;
				Game1.videoPlayer.Volume = 0f;
				Sound.PlayCue("chapter");
				break;
			case 1:
				this.textFade1 += hudTime;
				if (this.textFade1 > 2f)
				{
					this.chapterStage++;
				}
				break;
			case 2:
				this.textFade2 += hudTime;
				if (this.textFade2 > 2f)
				{
					this.textFade1 = 1f;
					this.textFade2 = 2f;
					this.chapterStage++;
				}
				break;
			case 3:
				this.textFade1 -= hudTime;
				this.textFade2 -= hudTime;
				if (Game1.videoPlayer.State == MediaState.Stopped && this.textFade1 < 0f && this.textFade2 < 0f)
				{
					this.chapterStage++;
				}
				break;
			default:
				Game1.GetTitleContent().Unload();
				this.map.SetMusic(this.map.path);
				Game1.menu.ExitChapter();
				break;
			}
		}

		public void Draw(SpriteBatch sprite)
		{
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				sprite.Draw(Game1.videoPlayer.GetTexture(), new Rectangle(0, 0, Game1.screenWidth, (int)((float)Game1.screenWidth * 0.5625f)), new Rectangle(0, 0, Game1.video.Width, Game1.video.Height), Color.White);
			}
			Game1.bigText.Color = new Color(0.6f, 0.6f, 0.6f, this.textFade1);
			Game1.bigText.DrawOutlineText(new Vector2((float)(Game1.screenWidth / 2 - 700) - this.textScroll, Game1.screenHeight / 2 - 180), this.chapterTitle, Game1.hiDefScaleOffset, 1000, TextAlign.Center, fullOutline: true);
			Game1.bigText.Color = new Color(1f, 1f, 1f, this.textFade2);
			Game1.bigText.DrawOutlineText(new Vector2((float)(Game1.screenWidth / 2 - 700) + this.textScroll, Game1.screenHeight / 2 - 150), this.chapterSubtitle, 1.5f * Game1.hiDefScaleOffset, 1000, TextAlign.Center, fullOutline: true);
			sprite.End();
			sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			for (int num = 15; num > 0; num--)
			{
				float num2 = Math.Min(Math.Min(this.textGlow1, 1f) / (float)num, 0.3f);
				if (num2 > 0.01f)
				{
					Game1.bigText.Color = new Color(0.6f, 0.6f, 0.6f, num2);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) - this.textScroll, Game1.screenHeight / 2 - 184 - num * 4), this.chapterTitle, 1.1f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) - this.textScroll, Game1.screenHeight / 2 - 184 + num * 4), this.chapterTitle, 1.1f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
				}
				num2 = Math.Min(Math.Min(this.textGlow2, 1f) / (float)num, 0.3f);
				if (num2 > 0.01f)
				{
					Game1.bigText.Color = new Color(1f, 1f, 1f, num2);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) + this.textScroll, Game1.screenHeight / 2 - 154 - num * 4), this.chapterSubtitle, 1.6f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) + this.textScroll, Game1.screenHeight / 2 - 154 + num * 4), this.chapterSubtitle, 1.6f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
				}
			}
			for (int num3 = 60; num3 > 0; num3--)
			{
				float num2 = Math.Min(Math.Min(this.textGlow1, 1f) / (float)num3, 0.3f);
				if (num2 > 0.01f)
				{
					Game1.bigText.Color = new Color(0.6f, 0.6f, 0.6f, num2);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) - this.textScroll - (float)(num3 * 5), Game1.screenHeight / 2 - 180), this.chapterTitle, 1.1f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) - this.textScroll + (float)(num3 * 5), Game1.screenHeight / 2 - 180), this.chapterTitle, 1.1f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
				}
				num2 = Math.Min(Math.Min(this.textGlow2, 1f) / (float)num3, 0.3f);
				if (num2 > 0.01f)
				{
					Game1.bigText.Color = new Color(1f, 1f, 1f, num2);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) + this.textScroll - (float)(num3 * 5), Game1.screenHeight / 2 - 150), this.chapterSubtitle, 1.6f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
					Game1.bigText.DrawText(new Vector2((float)(Game1.screenWidth / 2 - 700) + this.textScroll + (float)(num3 * 5), Game1.screenHeight / 2 - 150), this.chapterSubtitle, 1.6f * Game1.hiDefScaleOffset, 1000f, TextAlign.Center);
				}
			}
			sprite.End();
			sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
		}
	}
}

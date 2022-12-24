using System;
using Dust.Audio;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dust.HUD
{
	internal class UpsellManager
	{
		private Texture2D sellTex;

		private int sellStage;

		private float timer;

		private bool viewTrailer;

		public UpsellManager(bool quitting)
		{
			Game1.GetLargeContent().Unload();
			WeatherAudio.Stop();
			Music.Play("silent");
			this.sellTex = Game1.GetLargeContent().Load<Texture2D>("gfx/splash/upsell");
			Game1.gameMode = Game1.GameModes.UpSell;
			Game1.video = Game1.GetLargeContent().Load<Video>("video/cutscene_200");
			Game1.videoPlayer.Play(Game1.video);
			Game1.videoPlayer.IsLooped = true;
			Game1.videoPlayer.Volume = Sound.masterMusicVolume;
			this.viewTrailer = false;
			this.timer = 2f;
			if (quitting)
			{
				this.sellStage = 0;
			}
			else
			{
				this.sellStage = 1;
			}
		}

		private void ExitUpSell()
		{
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				Game1.videoPlayer.Stop();
			}
			Game1.videoPlayer.IsLooped = false;
			Game1.GetLargeContent().Unload();
		}

		public void Update(float hudTime)
		{
			this.timer = Math.Max(this.timer - hudTime * 2f, 0f);
			if (Game1.GamerServices && !Guide.IsVisible && this.timer == 0f)
			{
				if (Game1.hud.KeyY)
				{
					this.viewTrailer = !this.viewTrailer;
					if (this.viewTrailer)
					{
						Sound.PlayCue("menu_confirm");
					}
					else
					{
						Sound.PlayCue("menu_cancel");
					}
					Game1.hud.KeyY = false;
				}
				if (this.viewTrailer)
				{
					if (Game1.hud.KeyX || Game1.hud.KeySelect || Game1.hud.KeyCancel)
					{
						this.viewTrailer = false;
						Sound.PlayCue("menu_cancel");
						Game1.hud.KeyX = (Game1.hud.KeySelect = (Game1.hud.KeyCancel = false));
					}
				}
				else
				{
					if (Game1.hud.KeyX && this.sellStage == 0)
					{
						Game1.gameMode = Game1.GameModes.Quit;
						Sound.PlayCue("menu_cancel");
						Game1.hud.KeyX = false;
						return;
					}
					if (Game1.hud.KeyCancel)
					{
						Game1.hud.KeyCancel = false;
						Sound.PlayCue("menu_cancel");
						this.ExitUpSell();
						if (this.sellStage == 0)
						{
							Game1.menu.SkipToStartPage();
							Music.Play("beauty");
						}
						else
						{
							Game1.menu.QuitGame(Game1.pManager);
						}
					}
					if (Game1.hud.KeySelect)
					{
						Sound.PlayCue("menu_confirm");
						SignedInGamer signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
						if (signedInGamer != null && signedInGamer.IsSignedInToLive && signedInGamer.Privileges.AllowPurchaseContent)
						{
							Game1.awardsManager.InitPurchase();
						}
						else if (Game1.GamerServices)
						{
							Guide.ShowSignIn(1, onlineOnly: false);
						}
					}
				}
			}
			if (Game1.GamerServices && Guide.IsVisible)
			{
				Game1.IsTrial = Guide.IsTrialMode;
			}
			if (!Game1.IsTrial)
			{
				Game1.menu.QuitGame(Game1.pManager);
				this.ExitUpSell();
			}
		}

		public void Draw(SpriteBatch sprite, GraphicsDevice graphicsDevice, Texture2D nullTex)
		{
			graphicsDevice.Clear(Color.Black);
			sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				Texture2D texture = Game1.videoPlayer.GetTexture();
				_ = Game1.standardDef;
				sprite.Draw(texture, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, 1280, 720), Color.White, 0f, new Vector2(640f, 360f), Game1.standardDef ? 0.75f : 1f, SpriteEffects.None, 0f);
			}
			if ((!Game1.GamerServices || !Guide.IsVisible) && !this.viewTrailer)
			{
				Color color2 = (Game1.bigText.Color = (Game1.smallText.Color = Color.White));
				float num = 0.7f;
				int num2 = 700;
				Vector2 vector = new Vector2((float)Game1.screenWidth * 0.9f - (float)num2, (float)Game1.screenHeight * 0.12f);
				if (this.sellTex != null)
				{
					sprite.Draw(this.sellTex, new Vector2(0f, Game1.screenHeight / 2 - 360), Color.White);
				}
				string text = Game1.bigText.WordWrap(Strings_MainMenu.Upsell, num, num2, TextAlign.Center);
				string s = Game1.smallText.WordWrap(Strings_MainMenu.UpsellBullets, num, num2, TextAlign.Left);
				Game1.hud.DrawBorder(vector - new Vector2(20f, 20f), num2 + 40, (int)((float)Game1.screenHeight * 0.8f), Color.White, 0.9f, 0);
				Game1.bigText.DrawOutlineText(vector, text, num, num2, TextAlign.Left, fullOutline: true);
				vector.Y += Game1.bigFont.MeasureString(text).Y * num + 20f;
				Game1.smallText.DrawOutlineText(vector, s, num, num2, TextAlign.Left, fullOutline: true);
				num = 0.7f;
				string s2 = Game1.smallText.WordWrap(Strings_MainMenu.UpsellControls + ((this.sellStage == 0) ? ("     " + Strings_MainMenu.UpsellControlsExit) : ""), num, Game1.screenWidth, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				Game1.smallText.DrawButtonOutlineText(new Vector2(vector.X, (float)Game1.screenHeight * 0.8f), s2, num, Game1.inventoryManager.itemControlsButtonList, bounce: true, num2, TextAlign.Center);
			}
			sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, this.timer));
			sprite.End();
		}
	}
}

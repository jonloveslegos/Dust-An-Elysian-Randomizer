using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Dust.Audio;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Dust.Dialogue
{
	public class Cutscene
	{
		public CutsceneType SceneType;

		private int sceneID;

		private int curLine;

		private float timer;

		private float textAlpha;

		private bool loadingVideo;

		private Song[] voice = new Song[1];

		private List<string> subtitles = new List<string>();

		private List<float> subtitleTime = new List<float>();

		private WeatherType prevWeather;

		private string prevPath;

		private Map map;

		private bool canDraw;

		public Cutscene(Map _map)
		{
			this.map = _map;
		}

		public void InitCutscene(int _sceneID)
		{
			this.sceneID = _sceneID;
			this.SceneType = CutsceneType.Video;
			this.curLine = 0;
			this.timer = 0f;
			this.textAlpha = 0f;
			this.canDraw = false;
			this.prevWeather = Game1.wManager.weatherType;
			this.prevPath = Game1.map.path;
			this.loadingVideo = true;
			this.LoadStrings();
			this.LoadContent();
		}

		public void ExitCutscene()
		{
			this.curLine = 0;
			this.timer = 0f;
			this.textAlpha = 0f;
			this.subtitles.Clear();
			this.subtitleTime.Clear();
			VoiceManager.StopVoice();
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				Game1.videoPlayer.Stop();
			}
			if (Game1.menu.prompt == promptDialogue.SkipEvent)
			{
				Game1.menu.ClearPrompt();
			}
			Game1.GetTitleContent().Unload();
			this.ResetWeather();
			this.SceneType = CutsceneType.None;
		}

		private void LoadContent()
		{
			Game1.GetTitleContent().Unload();
			string text = $"{this.sceneID:D3}";
			try
			{
				Game1.video = Game1.GetTitleContent().Load<Video>("video/cutscene_" + text);
				this.loadingVideo = false;
				Game1.pManager.ResetHud();
				Game1.videoPlayer.Play(Game1.video);
				Game1.videoPlayer.IsLooped = false;
				Game1.videoPlayer.Volume = Sound.masterSFXVolume * Sound.volumeOverride * 0.55f;
				this.SceneType = CutsceneType.Video;
			}
			catch
			{
				this.loadingVideo = false;
				Game1.video = null;
				this.SceneType = CutsceneType.SubtitleOnly;
			}
			try
			{
				this.voice[0] = Game1.GetTitleContent().Load<Song>("voice/Narration/narration_" + text);
				VoiceManager.PlayVoice(this.voice[0]);
			}
			catch (Exception)
			{
			}
		}

		private void LoadStrings()
		{
			this.subtitles.Clear();
			this.subtitleTime.Clear();
			string value = $"{this.sceneID:D3}";
			ResourceSet resourceSet = Dialogue_Cutscenes.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			if (resourceSet != null)
			{
				List<string> list = new List<string>();
				foreach (DictionaryEntry item in resourceSet)
				{
					string text = (string)item.Key;
					if (text.StartsWith(value))
					{
						list.Add(text);
					}
				}
				list.Sort();
				for (int i = 0; i < list.Count; i++)
				{
					string[] array = list[i].Split('_');
					this.subtitles.Add(Game1.bigText.WordWrap(Dialogue_Cutscenes.ResourceManager.GetString(list[i]), 0.8f, (float)Game1.screenWidth * 0.7f, TextAlign.LeftAndCenter));
					this.subtitleTime.Add(Convert.ToSingle(array[1]));
				}
			}
			if (this.subtitleTime.Count > 0)
			{
				this.subtitleTime.Add(this.subtitleTime[this.subtitleTime.Count - 1] + 6f);
				this.textAlpha = (0f - this.subtitleTime[0]) * 4f;
			}
		}

		private void ExitNoProfile()
		{
			this.ExitCutscene();
			Game1.hud.isPaused = false;
			Sound.PlayCue("shop_fail");
			Game1.menu.prompt = promptDialogue.SignedOut;
			Game1.menu.ClearPrompt();
		}

		public void Update(float hudTime)
		{
			this.canDraw = false;
			if ((Game1.FrameTime == 0f && (!Game1.events.anyEvent || Game1.menu.prompt != promptDialogue.None)) || Game1.hud.isPaused || (Game1.GamerServices && Guide.IsVisible))
			{
				Game1.videoPlayer.Pause();
				VoiceManager.PauseVoice();
				return;
			}
			if (Game1.videoPlayer.State == MediaState.Paused)
			{
				Game1.videoPlayer.Resume();
			}
			VoiceManager.ResumeVoice();
			this.timer += hudTime;
			float num = 1f;
			if (this.curLine < this.subtitles.Count)
			{
				num += Math.Min((Game1.bigFont.MeasureString(this.subtitles[this.curLine]).X + Game1.bigFont.MeasureString(this.subtitles[this.curLine]).Y) / 100f, 10f);
			}
			if (this.curLine + 1 >= this.subtitleTime.Count)
			{
				this.textAlpha = Math.Min(this.textAlpha - hudTime * 4f, 1f);
				if (this.textAlpha < 0f && Game1.videoPlayer.State != MediaState.Playing)
				{
					this.ExitCutscene();
					return;
				}
			}
			else if (this.timer > this.subtitleTime[this.curLine + 1] - 0.5f || this.timer > this.subtitleTime[this.curLine] + num)
			{
				this.textAlpha = Math.Min(this.textAlpha - hudTime * 4f, 1f);
				if (this.textAlpha <= 0f)
				{
					this.textAlpha = 0f;
					if (this.timer > this.subtitleTime[this.curLine + 1] - 0.5f)
					{
						this.curLine++;
					}
				}
			}
			else
			{
				this.textAlpha = Math.Min(this.textAlpha + hudTime * 4f, 1f);
			}
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				Sound.weatherVolume = -1f;
			}
			else if (this.curLine < this.subtitles.Count || this.textAlpha > 0f)
			{
				this.ResetWeather();
				this.SceneType = CutsceneType.SubtitleOnly;
			}
			if ((Game1.GamerServices && Game1.IsTrial) || Game1.Convention)
			{
				GamePadState state = GamePad.GetState((PlayerIndex)Game1.currentGamePad);
				if (state.Buttons.A == ButtonState.Pressed || state.Buttons.B == ButtonState.Pressed || state.Buttons.X == ButtonState.Pressed || state.Buttons.Y == ButtonState.Pressed || state.Buttons.Back == ButtonState.Pressed || state.Buttons.Start == ButtonState.Pressed || state.Buttons.LeftShoulder == ButtonState.Pressed || state.Buttons.RightShoulder == ButtonState.Pressed || state.DPad.Up == ButtonState.Pressed || state.DPad.Down == ButtonState.Pressed || state.DPad.Left == ButtonState.Pressed || state.DPad.Right == ButtonState.Pressed || state.Triggers.Left > 0.5f || state.Triggers.Right > 0.5f || state.ThumbSticks.Left.Length() > 0.5f || state.ThumbSticks.Right.Length() > 0.5f)
				{
					this.ExitCutscene();
					return;
				}
			}
			this.canDraw = true;
		}

		private void ResetWeather()
		{
		}

		public void Draw(SpriteBatch sprite, Texture2D nullTex)
		{
			if (this.loadingVideo)
			{
				this.timer = 0f;
			}
			else
			{
				if (!this.canDraw)
				{
					return;
				}
				sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				if (this.SceneType == CutsceneType.Video)
				{
					sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black);
					if (Game1.videoPlayer.State != 0)
					{
						sprite.Draw(Game1.videoPlayer.GetTexture(), new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, 1280, 720), Color.White, 0f, new Vector2(640f, 360f), Game1.standardDef ? 0.75f : Game1.hiDefScaleOffset, SpriteEffects.None, 0f);
					}
				}
				Game1.bigText.Color = new Color(1f, 1f, 1f, this.textAlpha);
				if (this.curLine < this.subtitles.Count)
				{
					Game1.bigText.DrawOutlineText(new Vector2(0f, (float)Game1.screenHeight * 0.88f - Game1.bigFont.MeasureString(this.subtitles[this.curLine]).Y * 0.8f), this.subtitles[this.curLine], 0.8f, Game1.screenWidth, TextAlign.Center, fullOutline: true);
				}
				sprite.End();
			}
		}
	}
}

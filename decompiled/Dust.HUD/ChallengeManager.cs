using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Storage;
using Dust.Strings;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.HUD
{
	public class ChallengeManager
	{
		public enum ChallengeMode
		{
			None,
			ViewScore,
			ViewLeaderBoard,
			InChallenge,
			TallyScore
		}

		public ChallengeMode challengeMode;

		public int challengeState;

		private static int tempScore;

		public int currentScore;

		private static int totalScore;

		private static float tallyStars;

		private static float challengeClock;

		private static float challengeTime;

		private Texture2D challengeTex;

		private static float screenShake;

		private static float stoneLoc;

		private static int animFrame;

		private static float animFrameTime;

		public int currentChallenge;

		public List<ChallengeArena> challengeArenas = new List<ChallengeArena>();

		public Vector2 challengeInitPos;

		public Vector2 challengeCompletePos;

		public ChallengeManager()
		{
			this.challengeArenas.Clear();
			for (int i = 0; i < 20; i++)
			{
				this.challengeArenas.Add(new ChallengeArena(i));
			}
			this.Reset();
		}

		public void Reset()
		{
			this.challengeMode = ChallengeMode.None;
			ChallengeManager.challengeClock = (ChallengeManager.challengeTime = (this.challengeState = (this.currentScore = (ChallengeManager.totalScore = (ChallengeManager.tempScore = 0)))));
			this.ResetPositions();
		}

		public void ResetPositions()
		{
			this.currentChallenge = -1;
			this.challengeInitPos = (this.challengeCompletePos = Vector2.Zero);
			ChallengeManager.animFrameTime = (ChallengeManager.animFrame = 0);
		}

		private void SetTextures()
		{
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTextures)));
		}

		private void LoadTextures()
		{
			this.challengeTex = Game1.GetInventoryContent().Load<Texture2D>("gfx/ui/challenge_elements");
		}

		public void InitScoreBoard()
		{
			if (this.challengeMode == ChallengeMode.None && this.currentChallenge >= 0 && !Game1.hud.isPaused && Game1.hud.inventoryState == InventoryState.None && Game1.menu.prompt == promptDialogue.None)
			{
				ChallengeManager.stoneLoc = 1440f;
				ChallengeManager.screenShake = 3f;
				this.SetTextures();
				this.challengeMode = ChallengeMode.ViewScore;
				Game1.hud.isPaused = true;
				Game1.hud.helpState = 0;
				Sound.PlayCue("challenge_score_open");
				Game1.menu.optionDesc = Game1.smallText.WordWrap(Game1.isPCBuild ? Strings_HudInv.Exit : Strings_Leaderboards.challengecontrols, 0.8f, 1000f, Game1.menu.optionDescButtonList, TextAlign.Left);
			}
		}

		private void InitLeaderBoard()
		{
			if (Game1.awardsManager.InitLeaderBoard(LeaderBoardFilter.Challenge, this.currentChallenge))
			{
				this.challengeMode = ChallengeMode.ViewLeaderBoard;
			}
			else
			{
				this.challengeMode = ChallengeMode.None;
				Game1.hud.isPaused = false;
			}
			this.challengeState = 0;
		}

		public void UpdateLeaderBoard()
		{
			if (Game1.menu.menuMode == MenuMode.LeaderBoards)
			{
				Game1.awardsManager.UpdateLeaderBoard();
			}
			else
			{
				this.ExitScoreBoard(canRestart: false);
			}
		}

		public void ExitScoreBoard(bool canRestart)
		{
			if (this.challengeMode != 0)
			{
				this.challengeMode = ChallengeMode.None;
				Game1.hud.isPaused = false;
				Game1.menu.menuMode = MenuMode.None;
				Game1.GetInventoryContent().Unload();
				if (canRestart)
				{
					this.challengeState = 0;
				}
			}
		}

		private void SetHighScore(int newScore)
		{
			if (newScore > this.challengeArenas[this.currentChallenge].HighScore)
			{
				this.challengeArenas[this.currentChallenge].HighScore = newScore;
				Game1.navManager.BeginReadThread(Game1.map.regionName, forcePopulate: true);
			}
			if (this.challengeArenas[this.currentChallenge].CheckStarCount() == 4)
			{
				Game1.awardsManager.EarnAchievement(Achievement.ChallengeFourStar, forceCheck: false);
			}
			bool flag = true;
			int num = 0;
			for (int i = 0; i < Game1.cManager.challengeArenas.Count; i++)
			{
				if (Game1.cManager.challengeArenas[i].RankScore > 0)
				{
					num = Math.Max(num, i + 1);
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (this.challengeArenas[j].CheckStarCount() < 4)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Game1.awardsManager.EarnAchievement(Achievement.ChallengeFourStarAll, forceCheck: false);
			}
		}

		private void SetLeaderboardScore()
		{
			int num = this.currentScore;
			float num2 = ChallengeManager.challengeClock * 10f;
			int num3 = 0;
			while (num > 0)
			{
				num--;
				num3++;
			}
			while (num2 > 0f)
			{
				num2 -= 0.1f;
				if (num2 > 0f)
				{
					num3++;
				}
			}
			Game1.awardsManager.SetWrite(this.currentChallenge, num3);
		}

		public void AddScore(int score, Vector2 loc)
		{
			if (this.challengeMode == ChallengeMode.InChallenge)
			{
				this.currentScore = Math.Max(Game1.cManager.currentScore + score, 0);
				Game1.pManager.AddScore(loc, score, 9);
			}
		}

		public void InitChallenge()
		{
			if (this.challengeMode == ChallengeMode.None && this.currentChallenge >= 0)
			{
				this.challengeMode = ChallengeMode.InChallenge;
				ChallengeManager.challengeClock = this.challengeArenas[this.currentChallenge].Timer;
				this.currentScore = (ChallengeManager.totalScore = (ChallengeManager.tempScore = 0));
				this.challengeState = 1;
				ChallengeManager.challengeTime = 0.2f;
				this.challengeArenas[this.currentChallenge].ResetLamps(resetChallenge: true);
				Game1.hud.helpState = 0;
				VibrationManager.SetBlast(1f, Game1.character[0].Location);
				VibrationManager.SetScreenShake(0.5f);
				Sound.PlayCue("challenge_start");
				Music.Play("challenge");
			}
		}

		private void UpdateChallenge(Character[] c)
		{
			if (this.challengeState > 2)
			{
				Game1.hud.LimitInput();
				Game1.hud.canInput = false;
				c[0].Ethereal = EtherealState.Ethereal;
			}
			if (this.challengeState == 1)
			{
				if (ChallengeManager.challengeTime <= 0f)
				{
					this.challengeState++;
				}
			}
			else if (this.challengeState == 2)
			{
				if (ChallengeManager.tempScore < this.currentScore)
				{
					if (ChallengeManager.tempScore < this.currentScore - 100)
					{
						ChallengeManager.tempScore += 25;
					}
					else
					{
						ChallengeManager.tempScore++;
					}
				}
				else if (ChallengeManager.tempScore > this.currentScore)
				{
					if (ChallengeManager.tempScore > this.currentScore + 100)
					{
						ChallengeManager.tempScore -= 25;
					}
					else
					{
						ChallengeManager.tempScore--;
					}
				}
				if (Game1.stats.playerLifeState == 0)
				{
					ChallengeManager.challengeClock = Math.Max(ChallengeManager.challengeClock - Game1.FrameTime, 0f);
				}
				if (this.challengeCompletePos != Vector2.Zero && c[0].Location.X > this.challengeCompletePos.X - 100f && c[0].Location.X < this.challengeCompletePos.X + 100f && c[0].Location.Y > this.challengeCompletePos.Y - 1200f && c[0].Location.Y < this.challengeCompletePos.Y + 600f)
				{
					if (Game1.stats.playerLifeState == 0)
					{
						this.SetLeaderboardScore();
						ChallengeManager.challengeTime = 1f;
						this.challengeState++;
						VibrationManager.SetBlast(1f, Game1.character[0].Location);
						VibrationManager.SetScreenShake(0.5f);
						Sound.PlayCue("challenge_end");
					}
					else
					{
						c[0].Trajectory = new Vector2(0f, c[0].Trajectory.Y);
					}
				}
			}
			else if (this.challengeState == 3)
			{
				if (ChallengeManager.challengeTime <= 0f)
				{
					this.challengeMode = ChallengeMode.TallyScore;
					Game1.menu.menuMode = MenuMode.LeaderBoards;
					Game1.hud.isPaused = true;
					this.challengeState++;
					ChallengeManager.challengeTime = 0.5f;
					ChallengeManager.stoneLoc = 1440f;
					ChallengeManager.screenShake = 3f;
					Sound.PlayCue("challenge_score_open");
					ChallengeManager.tempScore = this.currentScore;
					ChallengeManager.challengeClock *= 10f;
				}
			}
			else if (this.challengeState == 4)
			{
				if (ChallengeManager.challengeTime <= 0f)
				{
					ChallengeManager.challengeTime = 1.5f;
					this.challengeState++;
					ChallengeManager.tallyStars = -10f;
					Game1.menu.optionDesc = Game1.smallText.WordWrap(Game1.isPCBuild ? Strings_HudInv.Exit : Strings_Leaderboards.challengecontrols, 0.8f, 1000f, Game1.menu.optionDescButtonList, TextAlign.Left);
				}
			}
			else if (this.challengeState == 5)
			{
				if (ChallengeManager.challengeTime <= 0f)
				{
					ChallengeManager.challengeTime = 0f;
					if (this.currentScore > 80)
					{
						this.currentScore -= 18;
						ChallengeManager.totalScore += 18;
						Sound.PlayCue("challenge_score_tally");
					}
					else if (this.currentScore > 0)
					{
						if (Game1.skipFrame > 1)
						{
							this.currentScore--;
							ChallengeManager.totalScore++;
							Sound.PlayCue("challenge_score_tally");
						}
					}
					else if (ChallengeManager.challengeClock > 8f)
					{
						ChallengeManager.challengeClock -= 2.1f;
						ChallengeManager.totalScore += 21;
						Sound.PlayCue("challenge_score_tally");
					}
					else if (ChallengeManager.challengeClock > 0f)
					{
						if (Game1.skipFrame > 1)
						{
							ChallengeManager.challengeClock -= 0.1f;
							if (ChallengeManager.challengeClock > 0f)
							{
								ChallengeManager.totalScore++;
							}
							Sound.PlayCue("challenge_score_tally");
						}
					}
					else
					{
						this.SetHighScore(ChallengeManager.totalScore);
						ChallengeManager.challengeTime = 4f;
						this.challengeState++;
						ChallengeManager.screenShake = 0.75f;
						Sound.PlayCue("challenge_score_tally_end");
					}
					int num = (int)Math.Min((float)ChallengeManager.totalScore / (float)(this.challengeArenas[this.currentChallenge].RankScore - 1) * 3f, 4f);
					if (ChallengeManager.tallyStars != (float)num && num < 4)
					{
						Sound.PlayCue("challenge_score_star" + num);
						ChallengeManager.screenShake = 0.5f;
					}
					ChallengeManager.tallyStars = num;
				}
				if ((Game1.hud.KeySelect && !Game1.isPCBuild) || Game1.hud.KeyCancel)
				{
					while (this.currentScore > 0)
					{
						this.currentScore--;
						ChallengeManager.totalScore++;
					}
					while (ChallengeManager.challengeClock > 0f)
					{
						ChallengeManager.challengeClock -= 0.1f;
						if (ChallengeManager.challengeClock > 0f)
						{
							ChallengeManager.totalScore++;
						}
					}
					this.SetHighScore(ChallengeManager.totalScore);
					this.challengeState++;
					ChallengeManager.screenShake = 0.75f;
					Sound.PlayCue("challenge_score_tally_end");
				}
				ChallengeManager.tempScore = this.currentScore;
			}
			else if (this.challengeState == 6)
			{
				if (Game1.hud.KeyCancel)
				{
					this.ExitScoreBoard(canRestart: true);
					Sound.PlayCue("menu_cancel");
				}
				else if (Game1.hud.KeySelect && !Game1.isPCBuild)
				{
					this.InitLeaderBoard();
					Sound.PlayCue("menu_confirm");
				}
			}
		}

		public void Update(Character[] c, Map map, ParticleManager pMan)
		{
			if (this.challengeMode == ChallengeMode.None)
			{
				if (!(this.challengeInitPos != Vector2.Zero))
				{
					return;
				}
				if (c[0].Location.X > this.challengeInitPos.X - 100f && c[0].Location.X < this.challengeInitPos.X + 100f && c[0].Location.Y > this.challengeInitPos.Y - 1200f && c[0].Location.Y < this.challengeInitPos.Y + 600f)
				{
					if (Game1.stats.playerLifeState == 0)
					{
						this.InitChallenge();
					}
					else
					{
						c[0].Trajectory.X = 0f;
					}
				}
			}
			else if (this.challengeMode == ChallengeMode.ViewScore)
			{
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2 + 400, (float)Game1.screenHeight * 0.075f), 0.8f, Color.White, 0, draw: false))
				{
					Game1.hud.KeyCancel = true;
				}
				if (Game1.hud.KeyCancel)
				{
					this.ExitScoreBoard(canRestart: false);
					Sound.PlayCue("menu_cancel");
				}
				else if (Game1.hud.KeySelect && !Game1.isPCBuild)
				{
					this.InitLeaderBoard();
					Sound.PlayCue("menu_confirm");
				}
			}
			else if (this.challengeMode == ChallengeMode.ViewLeaderBoard)
			{
				this.UpdateLeaderBoard();
			}
			else if (this.challengeMode == ChallengeMode.InChallenge || this.challengeMode == ChallengeMode.TallyScore)
			{
				this.UpdateChallenge(c);
			}
			ChallengeManager.challengeTime = Math.Max(ChallengeManager.challengeTime - Game1.HudTime, 0f);
			ChallengeManager.animFrameTime += Game1.HudTime * 24f;
			if (ChallengeManager.animFrameTime > 1f)
			{
				ChallengeManager.animFrameTime -= 1f;
				ChallengeManager.animFrame++;
				if (ChallengeManager.animFrame > 33)
				{
					ChallengeManager.animFrame = 0;
				}
			}
			if (this.challengeTex == null || this.challengeTex.IsDisposed)
			{
				return;
			}
			if (ChallengeManager.screenShake > 0f)
			{
				ChallengeManager.screenShake = Math.Max(ChallengeManager.screenShake - Game1.HudTime * 4f, 0f);
				VibrationManager.Rumble(Game1.currentGamePad, ChallengeManager.screenShake);
				if (ChallengeManager.stoneLoc == 0f)
				{
					pMan.AddMenuDebris(Rand.GetRandomVector2(0f, Game1.screenWidth, 0f, Game1.screenHeight), Rand.GetRandomVector2(-200f, 200f, -100f, 400f), 0.5f, 0.5f, 0.5f, 1f, 0.6f, 0, 9);
				}
			}
			if (ChallengeManager.stoneLoc > 0f)
			{
				for (int i = 0; i < 4; i++)
				{
					pMan.AddMenuDebris(new Vector2(Rand.GetRandomFloat(0f, Game1.screenWidth), Game1.screenHeight), Rand.GetRandomVector2(-400f, 400f, -2000f, -200f), 0.5f, 0.5f, 0.5f, 1f, 1f, 0, 9);
				}
				ChallengeManager.stoneLoc = Math.Max(ChallengeManager.stoneLoc - Game1.HudTime * 5000f, 0f);
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D[] hudTex, float scoreScale)
		{
			if (this.challengeMode == ChallengeMode.InChallenge)
			{
				this.DrawMiniScore(sprite, Math.Min(scoreScale, 1.2f));
			}
			else if (this.challengeMode != 0)
			{
				this.DrawScoreBoard(sprite, hudTex);
			}
		}

		private void DrawMiniScore(SpriteBatch sprite, float scoreScale)
		{
			Color color = Color.White;
			float num = 0f;
			if (this.challengeState == 1)
			{
				color = new Color(1f, 1f, 1f, 1f - ChallengeManager.challengeTime * 5f);
				num = ChallengeManager.challengeTime * 5f * 20f;
			}
			int num2 = (int)(280f * scoreScale);
			int num3 = (int)(90f * scoreScale);
			int count = this.challengeArenas[this.currentChallenge].lampList.Count;
			if (count > 0)
			{
				num3 += (int)(10f * scoreScale);
			}
			Vector2 vector = new Vector2(Game1.screenWidth - num2 + 55 - Game1.hud.screenLeftOffset, (float)(Game1.hud.screenTopOffset - 20) - num);
			if (Game1.hud.regionIntroState == 0)
			{
				vector += new Vector2(Game1.hud.eventLeftOffset, (0f - Game1.hud.eventLeftOffset) / 2f);
			}
			Game1.hud.DrawMiniBorder(vector, num2, num3, 1f, 0.9f, num3, 1f);
			float size = 0.8f * scoreScale;
			Game1.smallText.Color = color;
			Game1.smallText.DrawText(vector + new Vector2(20f, 10f) * scoreScale, Strings_HudInv.ChallengeMiniScore, size);
			Game1.hud.scoreDraw.Draw(ChallengeManager.tempScore, vector + new Vector2(250f, 7f) * scoreScale, scoreScale * 0.5f, color, ScoreDraw.Justify.Right, 2);
			Game1.smallText.DrawText(vector + new Vector2(20f, 47f) * scoreScale, Strings_HudInv.ChallengeMiniTime, size);
			Game1.hud.DrawTimer(vector + new Vector2(70f, 50f) * scoreScale, ChallengeManager.challengeClock, scoreScale * 0.5f, color, pad: false, milli: true, 2);
			this.DrawLamps(sprite, vector + new Vector2(0f, (float)num3 - 16f * scoreScale), count, scoreScale, (float)num2 - 100f * scoreScale);
		}

		private void DrawLamps(SpriteBatch sprite, Vector2 lampPos, int lampCount, float lampScale, float lampWindowWidth)
		{
			float num = lampWindowWidth / (float)Math.Max(lampCount - 1, 1);
			Vector2 vector = lampPos + new Vector2(lampWindowWidth + 70f * lampScale, 0f);
			Color color = new Color(0f, 0.4f, 0.4f, 1f);
			for (int i = 0; i < lampCount; i++)
			{
				sprite.Draw(Game1.hud.HudTex[0], vector - new Vector2(num * (float)i, 0f), new Rectangle(372, 140, 37, 45), this.challengeArenas[this.currentChallenge].lampList[lampCount - 1 - i].Exists ? color : Color.Aqua, 0f, new Vector2(37f, 0f), lampScale, SpriteEffects.None, 0f);
			}
		}

		private void DrawScoreBoard(SpriteBatch sprite, Texture2D[] hudTex)
		{
			if (this.challengeTex == null || this.challengeTex.IsDisposed)
			{
				this.SetTextures();
				return;
			}
			float num = 0.7f;
			float scale = 2f * Game1.hiDefScaleOffset;
			if (Game1.standardDef)
			{
				scale = 1.8f;
			}
			Vector2 vector = Rand.GetRandomVector2(0f - ChallengeManager.screenShake, ChallengeManager.screenShake, 0f, ChallengeManager.screenShake) * 10f;
			Vector2 vector2 = new Vector2((float)(Game1.screenWidth / 2) - 260f * num, (float)(Game1.screenHeight / 2 - 230) + ChallengeManager.stoneLoc) + vector;
			sprite.Draw(this.challengeTex, new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight + ChallengeManager.stoneLoc) + vector, new Rectangle(0, 0, 640, 720), Color.White, 0f, new Vector2(320f, 720f), scale, SpriteEffects.None, 0f);
			if (this.challengeMode == ChallengeMode.ViewScore)
			{
				int num2 = (int)((float)Game1.screenHeight * 0.075f + ChallengeManager.stoneLoc);
				Color color = new Color(0f, 0f, 0f, 0.5f);
				sprite.Draw(hudTex[1], vector + new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color, 0f, Vector2.Zero, new Vector2(1.5f, 0.5f), SpriteEffects.None, 0f);
				sprite.Draw(hudTex[1], vector + new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color, 0f, new Vector2(234f, 0f), new Vector2(1.5f, 0.5f), SpriteEffects.FlipHorizontally, 0f);
				sprite.Draw(hudTex[2], vector + new Vector2(Game1.screenWidth / 2, num2 + 60), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 0.5f), SpriteEffects.None, 0f);
				Game1.bigText.Color = Color.White;
				Game1.bigText.DrawShadowText(vector + new Vector2(0f, num2 + 20), Game1.events.regionDisplayName, 1f, Game1.screenWidth, TextAlign.Center, outline: true);
				Game1.bigText.DrawText(new Vector2(vector.X, vector2.Y + 10f * num), Strings_HudInv.ChallengePersonalBest, 0.8f, Game1.screenWidth, TextAlign.Center);
				Vector2 vector3 = new Vector2((float)(Game1.screenWidth / 2) + vector.X, vector2.Y);
				Game1.hud.scoreDraw.Draw(this.challengeArenas[this.currentChallenge].HighScore, vector3 + new Vector2(0f, 70f * num), 1f, Color.White, ScoreDraw.Justify.Center, 2);
				this.DrawStars(sprite, vector3 + new Vector2(-150f, 230f * num), Math.Max(ChallengeManager.totalScore, this.challengeArenas[this.currentChallenge].HighScore), this.challengeArenas[this.currentChallenge].RankScore, 100f, 1.4f, 1f, alignCenter: true);
				for (int i = 2; i < 5; i++)
				{
					Vector2 vector4 = vector3 + new Vector2((i - 2) * 200 - 200, 340f);
					Game1.hud.scoreDraw.Draw((int)((float)this.challengeArenas[this.currentChallenge].RankScore * ((float)(i - 1) / 3f)), vector4, 0.5f, Color.White, ScoreDraw.Justify.Center, 2);
					this.DrawStars(sprite, vector4 + new Vector2(-45f, 50f), i * 100, 400, 30f, 0.5f, 1f, alignCenter: true);
				}
				ChallengeManager.DrawControls(sprite);
			}
			else if (this.challengeMode == ChallengeMode.TallyScore)
			{
				int num3 = (int)((float)Game1.screenHeight * 0.075f + ChallengeManager.stoneLoc);
				Color color2 = new Color(0f, 0f, 0f, 0.5f);
				sprite.Draw(hudTex[1], vector + new Vector2(Game1.screenWidth / 2, num3 + 93), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
				sprite.Draw(hudTex[1], vector + new Vector2(Game1.screenWidth / 2, num3 + 93), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.5f, 1f), SpriteEffects.FlipHorizontally, 0f);
				sprite.Draw(hudTex[2], vector + new Vector2(Game1.screenWidth / 2, num3 + 90), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 0.5f), SpriteEffects.None, 0f);
				Game1.bigText.Color = Color.White;
				Game1.bigText.DrawShadowText(vector + new Vector2(0f, num3 + 20), Strings_HudInv.ChallengeChallengeComplete, 1f, Game1.screenWidth, TextAlign.Center, outline: true);
				this.DrawLamps(sprite, new Vector2((float)(Game1.screenWidth / 2 - 200) + vector.X, vector2.Y), this.challengeArenas[this.currentChallenge].lampList.Count, 1f, 300f);
				float size = 0.8f;
				Game1.bigText.DrawText(vector2 + new Vector2(10f, 90f) * num, Strings_HudInv.ChallengeMiniScore, size);
				Game1.hud.scoreDraw.Draw(ChallengeManager.tempScore, vector2 + new Vector2(490f, 90f) * num, num, Color.White, ScoreDraw.Justify.Right, 2);
				Game1.bigText.DrawText(vector2 + new Vector2(10f, 160f) * num, Strings_HudInv.ChallengeMiniTime, size);
				Game1.hud.DrawTimer(vector2 + new Vector2(133f, 160f) * num, ChallengeManager.challengeClock / 10f, num, Color.White, pad: false, milli: true, 2);
				sprite.Draw(hudTex[2], new Vector2((float)(Game1.screenWidth / 2) + vector.X, vector2.Y + 235f * num), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 0.5f), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(vector2 + new Vector2(10f, 280f) * num, Strings_HudInv.ChallengeMiniTotal, size);
				Game1.hud.scoreDraw.Draw(ChallengeManager.totalScore, vector2 + new Vector2(490f, 280f) * num, num, Color.White, ScoreDraw.Justify.Right, 2);
				sprite.Draw(hudTex[2], new Vector2((float)(Game1.screenWidth / 2) + vector.X, vector2.Y + 495f * num), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 0.5f), SpriteEffects.None, 0f);
				Game1.bigText.DrawText(vector2 + new Vector2(10f, 540f) * num, Strings_HudInv.ChallengeMiniBest, size);
				Game1.hud.scoreDraw.Draw(Math.Max(ChallengeManager.totalScore, this.challengeArenas[this.currentChallenge].HighScore), vector2 + new Vector2(490f, 540f) * num, num, Color.White, ScoreDraw.Justify.Right, 2);
				this.DrawStars(sprite, vector2 + new Vector2(400f, 620f) * num, Math.Max(ChallengeManager.totalScore, this.challengeArenas[this.currentChallenge].HighScore), this.challengeArenas[this.currentChallenge].RankScore, 24f, 0.3f, 1f, alignCenter: true);
				Vector2 vector5 = new Vector2((float)(Game1.screenWidth / 2 - 150) + vector.X, vector2.Y + 400f * num);
				Game1.hud.DrawBar(vector5, (float)ChallengeManager.totalScore / (float)this.challengeArenas[this.currentChallenge].RankScore, Color.Orange, new Vector2(1f, 1f), backGround: true);
				this.DrawStars(sprite, vector5 + new Vector2(0f, 10f) * num, ChallengeManager.totalScore, this.challengeArenas[this.currentChallenge].RankScore, 100f, 1.2f, 1f, alignCenter: false);
				for (int j = 1; j < 5; j++)
				{
					Vector2 loc = vector5 + new Vector2((j - 2) * 100 + 100, 40f);
					Game1.hud.scoreDraw.Draw((int)((float)this.challengeArenas[this.currentChallenge].RankScore * ((float)(j - 1) / 3f)), loc, 0.8f, Color.White, ScoreDraw.Justify.Center, 0);
				}
				if (this.challengeState == 6)
				{
					ChallengeManager.DrawControls(sprite);
				}
			}
			if (this.challengeMode == ChallengeMode.ViewLeaderBoard)
			{
				Game1.awardsManager.DrawLeaderBoard(sprite, hudTex, vector + new Vector2(0f, ChallengeManager.stoneLoc));
			}
		}

		private static void DrawControls(SpriteBatch sprite)
		{
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2 + 400, (float)Game1.screenHeight * 0.075f), 0.8f, Color.White * (1f - ChallengeManager.stoneLoc / 1400f), 0, draw: true);
				Game1.pcManager.DrawCursor(sprite, 0.8f, Color.White);
				return;
			}
			Vector2 vector = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.9f - 40f);
			Game1.hud.DrawMiniBorder(vector + new Vector2(-200f, 0f), 400, 40, 0.6f, 0.75f, 40, 1f);
			Game1.smallText.Color = Color.White;
			Game1.smallText.DrawButtonText(new Vector2(0f, vector.Y + 6f), Game1.menu.optionDesc, 0.8f, Game1.menu.optionDescButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
		}

		private void DrawStars(SpriteBatch sprite, Vector2 starPos, int score, int maxScore, float seperation, float scale, float alpha, bool alignCenter)
		{
			float num = Math.Min((float)score / (float)(maxScore - 1) * 3f, 4f);
			if (alignCenter)
			{
				for (int i = 0; (float)i < num; i++)
				{
					Vector2 position = starPos + new Vector2(seperation * (float)i, 0f);
					position.X += seperation * (float)Math.Max(4 - (int)num - 1, 0) / 2f;
					if (i > 0 && (float)i < num - 1f)
					{
						position.Y += seperation * scale / 4f;
					}
					sprite.Draw(this.challengeTex, position, new Rectangle(640 + ChallengeManager.animFrame / 12 * 60, ChallengeManager.animFrame * 60 - ChallengeManager.animFrame / 12 * 720, 60, 60), new Color(1f, 1f, 1f, alpha), 0f, new Vector2(30f, 30f), scale, SpriteEffects.None, 0f);
				}
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				Vector2 position2 = starPos + new Vector2(seperation * (float)j, 0f);
				if ((float)j < num)
				{
					sprite.Draw(this.challengeTex, position2, new Rectangle(640 + ChallengeManager.animFrame / 12 * 60, ChallengeManager.animFrame * 60 - ChallengeManager.animFrame / 12 * 720, 60, 60), new Color(1f, 1f, 1f, alpha), 0f, new Vector2(30f, 30f), scale, SpriteEffects.None, 0f);
				}
				else
				{
					sprite.Draw(this.challengeTex, position2, new Rectangle(640, 0, 60, 60), new Color(0f, 0f, 0f, alpha), 0f, new Vector2(30f, 30f), scale, SpriteEffects.None, 0f);
				}
			}
		}
	}
}

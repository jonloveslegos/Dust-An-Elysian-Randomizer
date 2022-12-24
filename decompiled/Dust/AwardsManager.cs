using System;
using System.Collections.Generic;
using System.Threading;
using Dust.Audio;
using Dust.HUD;
using Dust.Strings;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Dust
{
	public class AwardsManager
	{
		private static object syncObject = new object();

		private static int selection = 0;

		private static int selectionOffset = 0;

		private static int uploadScore = 0;

		private static int totalPlayers = 0;

		private static byte updateStage = 0;

		private static byte maxViewEntries = 10;

		private static byte maxChallengeLeaderboards = 6;

		private static byte richPresenceTimer = 0;

		private static bool readingLeaderboard = false;

		private static bool writingLeaderboard = false;

		private static int statWriteTimer = 30;

		private static List<string> rankString = new List<string>();

		private static LeaderBoardView leaderboardView;

		private static LeaderBoardFilter writeLeaderboardFilter;

		private static LeaderBoardFilter readLeaderboardFilter;

		private static float inactiveTimer = 0f;

		public static SignedInGamer signedInGamer;

		public SignedInGamer prevGamer;

		private static Gamer selectedGamer;

		public SignedInGamer MeSignedInGamer()
		{
			return AwardsManager.signedInGamer;
		}

		public bool InitLeaderBoard(LeaderBoardFilter setFilter, int challengeID)
		{
			Game1.menu.menuMode = MenuMode.LeaderBoards;
			AwardsManager.leaderboardView = LeaderBoardView.FriendScores;
			if (challengeID > -1)
			{
				AwardsManager.readLeaderboardFilter = (LeaderBoardFilter)(challengeID + 1);
			}
			else
			{
				AwardsManager.readLeaderboardFilter = (LeaderBoardFilter)Math.Max((int)setFilter, 1);
			}
			AwardsManager.signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
			this.prevGamer = AwardsManager.signedInGamer;
			if (Game1.Convention)
			{
				return false;
			}
			if (this.CheckAvailable(liveRequired: true))
			{
				this.SetRead();
				Game1.menu.optionDesc = Game1.smallText.WordWrap(Strings_Leaderboards.controls, 0.8f, 1000f, Game1.menu.optionDescButtonList, TextAlign.LeftAndCenter);
				return true;
			}
			Game1.menu.menuMode = MenuMode.None;
			return false;
		}

		private void ExitLeaderBoard(bool intentional)
		{
			int num = ((!Game1.GamerServices || !Game1.IsTrial) ? 1 : 0);
			int num2 = ((Game1.gameMode == Game1.GameModes.MainMenu) ? 2 : 3);
			if (intentional)
			{
				Game1.menu.ExitMode(0, num2 + (1 - num));
			}
			else
			{
				Game1.menu.ExitMode(0, 0);
			}
			Sound.PlayCue("menu_cancel");
			AwardsManager.rankString.Clear();
			if (AwardsManager.signedInGamer == null || this.prevGamer.Gamertag != AwardsManager.signedInGamer.Gamertag)
			{
				Game1.hud.SignOut(Game1.pManager);
			}
			GC.Collect();
		}

		public bool EarnAchievement(Achievement id, bool forceCheck)
		{
			if (Game1.isPCBuild)
			{
				bool flag = !Game1.stats.achievementEarned[(int)(id - 1)];
				Game1.stats.achievementEarned[(int)(id - 1)] = true;
				if (flag || forceCheck)
				{
					if (flag && Game1.gameMode != Game1.GameModes.WorldMap)
					{
						Game1.storage.Write(0, -1);
						Game1.storage.DisposeContainer();
					}
				}
				return true;
			}
			if (!Game1.GamerServices || Game1.Convention)
			{
				return false;
			}
			if (!Game1.stats.canEarnAchievements)
			{
				return false;
			}
			if (Game1.IsTrial && id != Achievement.LevelUp)
			{
				return false;
			}
			if (!forceCheck)
			{
				if (Game1.stats.achievementEarned[(int)(id - 1)])
				{
					return false;
				}
				Game1.stats.achievementEarned[(int)(id - 1)] = true;
			}
			if (Game1.stats.achievementEarned[(int)(id - 1)])
			{
				this.EarnAchievement(id.ToString());
				return true;
			}
			return false;
		}

		private void EarnAchievement(string key)
		{
			lock (AwardsManager.syncObject)
			{
				new object();
				AwardsManager.signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
				if (this.CheckAvailable(liveRequired: false))
				{
					if (!Game1.IsTrial)
					{
						Game1.menu.prompt = promptDialogue.QueuedAchievement;
						Game1.menu.ClearPrompt();
					}
					else
					{
						Game1.menu.prompt = promptDialogue.PurchaseAchievement;
						Game1.menu.ClearPrompt();
					}
				}
			}
		}

		private static void ProcessAchievementAwarded(IAsyncResult result)
		{
		}

		public void ViewAchievements()
		{
		}

		public bool InitPurchase()
		{
			if (Game1.Convention)
			{
				return false;
			}
			if (!Game1.GamerServices)
			{
				return false;
			}
			if (this.CheckAvailable(liveRequired: true) && !Guide.IsVisible && AwardsManager.signedInGamer.Privileges.AllowPurchaseContent)
			{
				Guide.ShowMarketplace((PlayerIndex)Game1.currentGamePad);
				Game1.menu.curMenuOption = 0;
				Game1.menu.SetGuideTime = 0.2f;
				return true;
			}
			return false;
		}

		private bool CheckAvailable(bool liveRequired)
		{
			return true;
		}

		public bool EarnAvatarAward(string[] keys)
		{
			if (!Game1.GamerServices)
			{
				return false;
			}
			return false;
		}

		private static void ProcessAvatarAwarded(IAsyncResult result)
		{
		}

		public bool EarnGamerPicture(string key)
		{
			if (!Game1.GamerServices)
			{
				return false;
			}
			return false;
		}

		private void SetRead()
		{
			lock (AwardsManager.syncObject)
			{
				if (this.CheckAvailable(liveRequired: true) && !AwardsManager.readingLeaderboard)
				{
					AwardsManager.readingLeaderboard = true;
					AwardsManager.totalPlayers = 0;
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Read)));
				}
			}
		}

		private void Read()
		{
			AwardsManager.readingLeaderboard = false;
			AwardsManager.selection = 0;
			AwardsManager.selectionOffset = 0;
		}

		public void SetWrite(int challengeID, int writeScore)
		{
			lock (AwardsManager.syncObject)
			{
				if (this.CheckAvailable(liveRequired: true) && !AwardsManager.writingLeaderboard && (!Game1.GamerServices || !Game1.IsTrial) && Game1.stats.canEarnAchievements)
				{
					if (challengeID > -1)
					{
						AwardsManager.writeLeaderboardFilter = (LeaderBoardFilter)(challengeID + 1);
					}
					else
					{
						AwardsManager.writeLeaderboardFilter = LeaderBoardFilter.Completion;
					}
					AwardsManager.uploadScore = writeScore;
					AwardsManager.writingLeaderboard = true;
					Thread thread = new Thread(Write);
					thread.IsBackground = true;
					thread.Start();
				}
			}
		}

		private void Write()
		{
			AwardsManager.writingLeaderboard = false;
		}

		private void WriteStatEntries1(LeaderboardWriter leaderboardWriter)
		{
			LeaderboardEntry leaderboard;
			if ((long)Game1.stats.Completion > 0)
			{
				leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard(LeaderBoardFilter.Completion));
				leaderboard.Rating = (long)Game1.stats.Completion;
			}
			if ((long)Game1.stats.Gold > 0L)
			{
				leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard(LeaderBoardFilter.Wealth));
				leaderboard.Rating = Game1.stats.Gold;
			}
			if ((long)Game1.stats.longestChain > 0L)
			{
				leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard(LeaderBoardFilter.MaxCombo));
				leaderboard.Rating = Game1.stats.longestChain;
			}
			if ((long)Game1.stats.enemiesDefeated > 0L)
			{
				leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard(LeaderBoardFilter.Defeated));
				leaderboard.Rating = Game1.stats.enemiesDefeated;
			}
			leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard(LeaderBoardFilter.PlayTime));
			leaderboard.Rating = (long)Game1.stats.gameClock;
		}

		private void WriteStatEntries2(LeaderboardWriter leaderboardWriter)
		{
		}

		private void WriteChallengeEntries(LeaderboardWriter leaderboardWriter, int start)
		{
			try
			{
				for (int i = start; i < AwardsManager.maxChallengeLeaderboards; i++)
				{
					LeaderboardEntry leaderboard = leaderboardWriter.GetLeaderboard(this.GetLeaderBoard((LeaderBoardFilter)i));
					if (i == (int)AwardsManager.writeLeaderboardFilter)
					{
						if (AwardsManager.uploadScore > 0)
						{
							leaderboard.Rating = AwardsManager.uploadScore;
						}
					}
					else if (Game1.cManager.challengeArenas[i - 1].HighScore > 0)
					{
						leaderboard.Rating = Game1.cManager.challengeArenas[i - 1].HighScore;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private LeaderboardIdentity GetLeaderBoard(LeaderBoardFilter filter)
		{
			LeaderboardIdentity result = LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, 0);
			if (filter >= LeaderBoardFilter.Completion)
			{
				result.Key = filter.ToString().ToLower();
			}
			else
			{
				result.Key = "challenge" + $"{(int)filter:D2}".ToLower();
			}
			return result;
		}

		private string GetFilterString(LeaderBoardFilter filter)
		{
			if (filter >= LeaderBoardFilter.Completion)
			{
				return Strings_Leaderboards.ResourceManager.GetString("filter" + (int)filter);
			}
			return Strings_Regions.trial + " #" + (int)filter;
		}

		private string GetScoreString(LeaderBoardFilter filter, int score)
		{
			switch (filter)
			{
			case LeaderBoardFilter.Completion:
				return score + "%";
			case LeaderBoardFilter.PlayTime:
			{
				int num = score;
				int num2 = num / 60;
				int num3 = Math.Min(num2 / 60, 99);
				num -= num2 * 60;
				num2 -= num3 * 60;
				if (num3 >= 99)
				{
					num2 = Math.Min(num2, 59);
					num = Math.Min(num, 59);
				}
				return $"{num3:00}:{num2:00}:{num:00}";
			}
			default:
				return score.ToString();
			}
		}

		public void UpdateLeaderboardStats()
		{
			if (AwardsManager.statWriteTimer > 0)
			{
				if (Game1.menu.menuMode == MenuMode.LeaderBoards)
				{
					AwardsManager.statWriteTimer = 30;
				}
				else if (Game1.cManager.currentChallenge > -1 || Game1.events.anyEvent)
				{
					AwardsManager.statWriteTimer = 30;
				}
				else if (AwardsManager.writingLeaderboard)
				{
					AwardsManager.statWriteTimer = (int)MathHelper.Clamp(AwardsManager.statWriteTimer + 1, 30f, 300f);
				}
				else if (Game1.map.GetTransVal() <= 0f)
				{
					AwardsManager.statWriteTimer--;
				}
			}
			if (AwardsManager.statWriteTimer < 1)
			{
				this.SetWrite(-1, 0);
				AwardsManager.statWriteTimer = 600;
			}
			AwardsManager.inactiveTimer += Game1.HudTime * 30f;
			if (Game1.character[0].Trajectory.X != 0f || Game1.events.anyEvent)
			{
				AwardsManager.inactiveTimer = 0f;
			}
			if (Game1.menu.menuMode == MenuMode.LeaderBoards)
			{
				AwardsManager.SetRichPresence("ViewingLeaderboards");
			}
			else if (Game1.gameMode == Game1.GameModes.MainMenu || Game1.gameMode == Game1.GameModes.Menu)
			{
				AwardsManager.inactiveTimer = 0f;
				AwardsManager.SetRichPresence("ViewingMenus");
			}
			else if (AwardsManager.inactiveTimer > 60f)
			{
				AwardsManager.SetRichPresence("Idle");
			}
			else if (Game1.GamerServices && Game1.IsTrial)
			{
				AwardsManager.SetRichPresence("Trial");
			}
			else
			{
				AwardsManager.SetRichPresence("SinglePlayer");
			}
		}

		public void UpdateLeaderBoard()
		{
			AwardsManager.signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
			if (AwardsManager.readingLeaderboard)
			{
				return;
			}
			int num = AwardsManager.selection;
			if (AwardsManager.signedInGamer != null && AwardsManager.signedInGamer.IsSignedInToLive)
			{
				if (Game1.hud.KeyUp)
				{
					AwardsManager.selection--;
					if (AwardsManager.selection != num)
					{
						AwardsManager.selectionOffset = 0;
						Sound.PlayCue("menu_click");
						this.GetSelectedGamer();
					}
				}
				if (Game1.hud.KeyDown)
				{
					AwardsManager.selection++;
					if (AwardsManager.selection != num)
					{
						AwardsManager.selectionOffset = 0;
						Sound.PlayCue("menu_click");
						this.GetSelectedGamer();
					}
				}
				if (!AwardsManager.readingLeaderboard)
				{
					int num2 = 140;
					if (Game1.hud.KeyRightBumper)
					{
						Sound.PlayCue("menu_page_turn");
						AwardsManager.readLeaderboardFilter++;
						while (true)
						{
							string text = AwardsManager.readLeaderboardFilter.ToString();
							int num3 = (int)AwardsManager.readLeaderboardFilter;
							if ((!(text == num3.ToString()) || (int)AwardsManager.readLeaderboardFilter <= (int)AwardsManager.maxChallengeLeaderboards) && (int)AwardsManager.readLeaderboardFilter <= num2)
							{
								break;
							}
							AwardsManager.readLeaderboardFilter++;
							if ((int)AwardsManager.readLeaderboardFilter > num2)
							{
								AwardsManager.readLeaderboardFilter = LeaderBoardFilter.Challenge;
							}
						}
						this.SetRead();
					}
					if (Game1.hud.KeyLeftBumper)
					{
						Sound.PlayCue("menu_page_turn");
						AwardsManager.readLeaderboardFilter--;
						while (true)
						{
							string text2 = AwardsManager.readLeaderboardFilter.ToString();
							int num4 = (int)AwardsManager.readLeaderboardFilter;
							if ((!(text2 == num4.ToString()) || (int)AwardsManager.readLeaderboardFilter <= (int)AwardsManager.maxChallengeLeaderboards) && AwardsManager.readLeaderboardFilter >= LeaderBoardFilter.Challenge)
							{
								break;
							}
							AwardsManager.readLeaderboardFilter--;
							if (AwardsManager.readLeaderboardFilter < LeaderBoardFilter.Challenge)
							{
								AwardsManager.readLeaderboardFilter = (LeaderBoardFilter)num2;
							}
						}
						this.SetRead();
					}
					if (Game1.hud.KeyX)
					{
						Sound.PlayCue("menu_page_turn");
						AwardsManager.leaderboardView++;
						if (AwardsManager.leaderboardView > LeaderBoardView.OverallScores)
						{
							AwardsManager.leaderboardView -= 3;
						}
						this.SetRead();
					}
				}
			}
			if (Game1.hud.KeyCancel || AwardsManager.signedInGamer == null || AwardsManager.signedInGamer != this.prevGamer)
			{
				this.ExitLeaderBoard(intentional: true);
				if ((AwardsManager.signedInGamer == null || AwardsManager.signedInGamer != this.prevGamer) && Game1.gameMode != 0)
				{
					Game1.hud.isPaused = false;
					Game1.gameMode = Game1.GameModes.Game;
					Game1.menu.prompt = promptDialogue.SignedOut;
					Game1.menu.ClearPrompt();
				}
			}
			if (AwardsManager.signedInGamer != null && !AwardsManager.signedInGamer.IsSignedInToLive && Game1.gameMode != 0)
			{
				Game1.hud.isPaused = false;
				Game1.gameMode = Game1.GameModes.Game;
				Game1.menu.prompt = promptDialogue.LiveDisconnected;
				Game1.menu.ClearPrompt();
			}
			AwardsManager.selectionOffset = (int)MathHelper.Clamp(AwardsManager.selectionOffset + 4, 0f, 20f);
		}

		private void ThreadGamerCard()
		{
			Guide.ShowGamerCard(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One), AwardsManager.selectedGamer);
		}

		private void GetSelectedGamer()
		{
		}

		public static void SetRichPresence(string key)
		{
			_ = Game1.GamerServices;
		}

		public static bool IsConnected()
		{
			AwardsManager.signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
			if (!AwardsManager.signedInGamer.IsSignedInToLive)
			{
				return false;
			}
			return true;
		}

		public void DrawLeaderBoard(SpriteBatch sprite, Texture2D[] hudTex, Vector2 newOffset)
		{
			int num = (int)((float)Game1.screenWidth * 0.075f + newOffset.X);
			int num2 = (int)((float)Game1.screenHeight * 0.075f + newOffset.Y);
			int num3 = (int)((float)Game1.screenWidth * 0.925f);
			Game1.hud.DrawBorder(new Vector2(num, num2), num3 - num, (int)((float)Game1.screenHeight * 0.925f) - num2, Color.White, 1f, 0);
			Color color = new Color(0f, 0f, 0f, 0.5f);
			sprite.Draw(hudTex[1], new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color, 0f, Vector2.Zero, new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			sprite.Draw(hudTex[1], new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color, 0f, new Vector2(234f, 0f), new Vector2(1.5f, 1f), SpriteEffects.FlipHorizontally, 0f);
			sprite.Draw(hudTex[2], new Vector2(Game1.screenWidth / 2, num2 + 60), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			Color color3 = (Game1.bigText.Color = (Game1.smallText.Color = Color.White));
			Game1.bigText.DrawOutlineText(new Vector2(0f, num2 + 20), Strings_MainMenu.Main3, 1f, Game1.screenWidth, TextAlign.Center, fullOutline: true);
			Vector2 vector = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.9f - 40f);
			float num4 = 0.8f;
			int num5 = (int)(Game1.smallFont.MeasureString(Game1.menu.optionDesc).X * num4) + 40;
			int height = (int)(Game1.smallFont.MeasureString(Game1.menu.optionDesc).Y * num4) + 10;
			Game1.hud.DrawMiniBorder(vector + new Vector2(-num5 / 2, 0f), num5, height, Color.White, 0.9f);
			Game1.smallText.DrawButtonText(new Vector2(0f, vector.Y + 6f), Game1.menu.optionDesc, num4, Game1.menu.optionDescButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
			Vector2 vector2 = new Vector2(num + 130, num2 + 130);
			int num6 = (int)MathHelper.Clamp((float)Game1.screenWidth * 0.4f - 20f, vector2.X + 170f, 10000f);
			int num7 = 35;
			num4 = 0.8f;
			if (Game1.standardDef)
			{
				num4 = 0.6f;
			}
			Game1.bigText.DrawOutlineText(vector2 + new Vector2(0f, -40f), Strings_Leaderboards.rank, num4, 0, TextAlign.Left, fullOutline: true);
			Game1.bigText.DrawOutlineText(new Vector2(num6, vector2.Y - 40f), Strings_Leaderboards.gamertag, num4, 0, TextAlign.Left, fullOutline: true);
			Game1.bigText.DrawOutlineText(new Vector2(num3 - 100, vector2.Y - 40f), this.GetFilterString(AwardsManager.readLeaderboardFilter), num4, 2000, TextAlign.Right, fullOutline: true);
			float num8 = Game1.bigFont.MeasureString(this.GetFilterString(AwardsManager.readLeaderboardFilter)).X * num4;
			float num9 = (float)Math.Abs(Math.Cos(Game1.hud.pulse) * 40.0);
			sprite.Draw(hudTex[0], new Vector2((float)(num3 - 130) - num8, vector2.Y - num9 / 8f), new Rectangle(150, 140, 50, 45), Color.White, 0f, new Vector2(25f, 45f), num4, SpriteEffects.None, 0f);
			sprite.Draw(hudTex[0], new Vector2(num3 - 70, vector2.Y - num9 / 8f), new Rectangle(0, 140, 50, 45), Color.White, 0f, new Vector2(25f, 45f), num4, SpriteEffects.None, 0f);
			sprite.Draw(hudTex[2], new Vector2(Game1.screenWidth / 2, vector2.Y - 10f), new Rectangle(0, 502, 326, 18), new Color(1f, 1f, 1f, 0.4f), 0f, new Vector2(163f, 0f), new Vector2((float)Game1.screenWidth / 500f, 0.25f), SpriteEffects.None, 0f);
			if (AwardsManager.readingLeaderboard)
			{
				Game1.DrawLoad(sprite, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				return;
			}
			if (AwardsManager.totalPlayers > 0 && AwardsManager.leaderboardView != 0)
			{
				Game1.bigText.DrawOutlineText(vector2 + new Vector2(0f, 360f), Strings_Leaderboards.totalplayers + " " + AwardsManager.totalPlayers, num4, 0, TextAlign.Left, fullOutline: true);
			}
			Game1.hud.DrawCursor(vector2 + new Vector2(-15f, 10 + AwardsManager.selection * num7), 0.75f, Color.White, flip: false);
			Game1.bigText.DrawOutlineText(new Vector2(num3 - 110, vector2.Y + 360f), Strings_Leaderboards.filter + " " + Strings_Leaderboards.ResourceManager.GetString("category" + (int)AwardsManager.leaderboardView), num4, Game1.screenWidth, TextAlign.Right, fullOutline: true);
			for (int i = 0; i < AwardsManager.maxViewEntries; i++)
			{
				sprite.Draw(hudTex[2], new Vector2(Game1.screenWidth / 2, vector2.Y + (float)(num7 * i) + 25f), new Rectangle(0, 502, 326, 18), new Color(1f, 1f, 1f, 0.4f), 0f, new Vector2(163f, 0f), new Vector2((float)Game1.screenWidth / 500f, 0.25f), SpriteEffects.None, 0f);
				int num10 = 0;
				if (AwardsManager.selection == i)
				{
					num10 = AwardsManager.selectionOffset;
					Game1.bigText.Color = Color.White;
				}
				else
				{
					Game1.bigText.Color = new Color(1f, 1f, 1f, 0.4f);
				}
				Game1.hud.scoreDraw.Draw(1000000 + i, vector2 + new Vector2(num10, num7 * i), 0.8f, Game1.bigText.Color, ScoreDraw.Justify.Left, 1);
				Game1.bigText.DrawText(new Vector2(num6 + num10, vector2.Y - 5f + (float)(num7 * i)), Strings_Leaderboards.gamertag, num4);
				Game1.hud.scoreDraw.Draw(9999999L, new Vector2(num3 - 120 + num10, vector2.Y - 5f + (float)(num7 * i)), 0.8f, Game1.bigText.Color, ScoreDraw.Justify.Right, 1);
			}
		}
	}
}

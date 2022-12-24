using System;
using System.Collections.Generic;
using Dust.ai;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses.bucket;
using Dust.MapClasses.script;
using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class MapScript
	{
		private Map map;

		private Party party;

		public MapScriptLine[] Lines;

		private static int curLine;

		private static float waiting;

		public bool IsReading;

		private static float pRightExitPoint;

		private static float pLeftExitPoint;

		private static float pUpExitPoint;

		private static float pDownExitPoint;

		private static float pRightEdge;

		private static float pLeftEdge;

		private static float pTopEdge;

		private static float pBottomEdge;

		private static List<int> npcIDList = new List<int>();

		private static Vector2 center;

		private static Rectangle renderRect = new Rectangle(-200, -200, Game1.screenWidth + 400, Game1.screenHeight + 400);

		public MapFlags Flags;

		public MapScript(Map map)
		{
			this.map = map;
			this.Flags = new MapFlags(10);
			this.Lines = new MapScriptLine[200];
		}

		public void DoScript(Character[] c, ParticleManager pMan, List<AmbientCue> ambienceList)
		{
			if (ambienceList != null)
			{
				pMan.Reset(removeWeather: false, removeBombs: false);
				Game1.dManager.Reset();
				MapScript.curLine = 0;
				MapScript.waiting = 0f;
				MapScript.pLeftExitPoint = this.map.leftExitPoint;
				MapScript.pRightExitPoint = this.map.rightExitPoint;
				MapScript.pUpExitPoint = this.map.topExitPoint;
				MapScript.pDownExitPoint = this.map.bottomExitPoint;
				MapScript.npcIDList.Clear();
				ambienceList.Clear();
			}
			if (MapScript.waiting > 0f)
			{
				if (!Game1.events.anyEvent)
				{
					MapScript.waiting -= Game1.FrameTime;
				}
				return;
			}
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while (!flag)
			{
				if (MapScript.curLine < this.Lines.Length - 1)
				{
					MapScript.curLine++;
					if (this.Lines[MapScript.curLine] == null)
					{
						continue;
					}
					try
					{
						switch (this.Lines[MapScript.curLine].Command)
						{
						case MapCommands.Enemy:
						{
							bool ground2 = false;
							if (this.Lines[MapScript.curLine].SParam.Length > 5)
							{
								if (this.Lines[MapScript.curLine].SParam[5].StartsWith("g"))
								{
									ground2 = true;
								}
								if (this.Lines[MapScript.curLine].SParam[5].StartsWith("a"))
								{
									ground2 = false;
								}
							}
							if (this.map.GetTransVal() > 0f)
							{
								ground2 = true;
							}
							for (int m = 0; m < c.Length; m++)
							{
								if (c[m].Exists == CharExists.Dead)
								{
									c[m].NewCharacter(this.Lines[MapScript.curLine].VParam * 2f, Game1.charDef[Game1.GetCharacterFromString(this.Lines[MapScript.curLine].SParam[1])], m, (this.Lines[MapScript.curLine].SParam.Length > 4) ? this.Lines[MapScript.curLine].SParam[4] : string.Empty, Team.Enemy, ground2);
									break;
								}
							}
							break;
						}
						case MapCommands.Party:
						{
							bool ground = false;
							if ((this.Lines[MapScript.curLine].SParam.Length > 5 && this.Lines[MapScript.curLine].SParam[5] != "") || this.map.GetTransVal() > 0f)
							{
								ground = true;
							}
							this.party = new Party();
							this.party.GetParty(this.Lines[MapScript.curLine].SParam[1]);
							for (int k = 0; k < this.party.Name.Length; k++)
							{
								for (int l = 0; l < c.Length; l++)
								{
									if (c[l].Exists == CharExists.Dead)
									{
										Vector2 newLoc = this.Lines[MapScript.curLine].VParam * 2f + Rand.GetRandomVector2(-500f, 500f, -200f, 0f);
										newLoc.X = MathHelper.Clamp(newLoc.X, this.map.leftEdge, this.map.rightEdge);
										newLoc.Y = MathHelper.Clamp(newLoc.Y, this.map.topEdge, this.map.bottomEdge);
										c[l].NewCharacter(newLoc, Game1.charDef[this.party.Name[k]], l, this.Lines[MapScript.curLine].SParam[4], Team.Enemy, ground);
										break;
									}
								}
							}
							break;
						}
						case MapCommands.NPC:
						{
							for (int n = 0; n < c.Length; n++)
							{
								if (c[n].Exists == CharExists.Dead)
								{
									c[n].NewCharacter(this.Lines[MapScript.curLine].VParam * 2f, Game1.charDef[Game1.GetCharacterFromString(this.Lines[MapScript.curLine].SParam[1])], n, (this.Lines[MapScript.curLine].SParam.Length > 4) ? this.Lines[MapScript.curLine].SParam[4] : this.Lines[MapScript.curLine].SParam[1], Team.Friendly, ground: true);
									MapScript.npcIDList.Add(n);
									break;
								}
							}
							break;
						}
						case MapCommands.MakeBucket:
							this.map.bucket = new Bucket(this.Lines[MapScript.curLine].IParam);
							break;
						case MapCommands.AddBucket:
							this.map.bucket.AddItem(this.Lines[MapScript.curLine].VParam * 2f, Game1.GetCharacterFromString(this.Lines[MapScript.curLine].SParam[1]));
							break;
						case MapCommands.IfNotBucketGoto:
							if (this.map.bucket.IsEmpty)
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[1]);
							}
							break;
						case MapCommands.QuestCreature:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).Stage == 0)
							{
								for (int j = 0; j < c.Length; j++)
								{
									if (c[j].Exists == CharExists.Dead)
									{
										c[j].NewCharacter(this.Lines[MapScript.curLine].VParam * 2f, Game1.charDef[Game1.GetCharacterFromString(this.Lines[MapScript.curLine].SParam[1])], j, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).UniqueID, Team.Friendly, ground: true);
										c[j].CollectEquipID = Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).ID - 1000;
										break;
									}
								}
							}
							num++;
							break;
						case MapCommands.Save:
							Game1.hud.savePos = this.Lines[MapScript.curLine].VParam * 2f;
							ambienceList?.Add(new AmbientCue("save_hum", this.Lines[MapScript.curLine].RParam, _stereo: true));
							this.map.terrainRegion.Add(new IdRectRegion(this.Lines[MapScript.curLine].RParam, 2));
							break;
						case MapCommands.Shop:
							Game1.hud.shopPos = this.Lines[MapScript.curLine].VParam * 2f;
							Game1.hud.shopID = this.Lines[MapScript.curLine].IParam;
							break;
						case MapCommands.Key:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].KeyList, this.GetMapName() + " " + num2).Stage == 0)
							{
								pMan.AddKey(this.Lines[MapScript.curLine].VParam * 2f, Vector2.Zero, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].KeyList, this.GetMapName() + " " + num2).ID, 5);
							}
							num2++;
							break;
						case MapCommands.Note:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).Stage == 0)
							{
								pMan.AddNote(this.Lines[MapScript.curLine].VParam * 2f, Vector2.Zero, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).ID, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).UniqueID, 5);
							}
							num++;
							break;
						case MapCommands.QuestItem:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).Stage == 0)
							{
								pMan.AddEquipment(this.Lines[MapScript.curLine].VParam * 2f, Vector2.Zero, this.Lines[MapScript.curLine].IParam, bluePrint: false, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).ID, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).UniqueID, 5);
							}
							num++;
							break;
						case MapCommands.Upgrade:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).Stage == 0)
							{
								pMan.AddUpgrade(this.Lines[MapScript.curLine].VParam * 2f, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).ID, 5);
							}
							num++;
							break;
						case MapCommands.Chest:
						{
							int iD = Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].ChestList, this.GetMapName() + " " + num3).ID;
							pMan.AddChest(this.Lines[MapScript.curLine].VParam * 2f, iD, this.Lines[MapScript.curLine].IParam == 1, 5);
							num3++;
							break;
						}
						case MapCommands.Cage:
						{
							int iD2 = Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].CageList, this.GetMapName() + " " + num4).ID;
							pMan.AddCage(this.Lines[MapScript.curLine].VParam * 2f, iD2, 5);
							num4++;
							break;
						}
						case MapCommands.Destructable:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].DestructableList, this.GetMapName() + " " + num5).Stage == 0)
							{
								Game1.dManager.AddDestructable(Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].DestructableList, this.GetMapName() + " " + num5).ID, this.Lines[MapScript.curLine].VParam * 2f, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[3]), Convert.ToInt32(this.Lines[MapScript.curLine].SParam[4]));
							}
							num5++;
							break;
						case MapCommands.DestructPlatform:
							Game1.dManager.AddDestructPlatform(new Vector2(this.Lines[MapScript.curLine].RParam.X, this.Lines[MapScript.curLine].RParam.Y), this.Lines[MapScript.curLine].RParam.Width, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[3]));
							break;
						case MapCommands.MovingPlatform:
							Game1.dManager.AddMovingPlatform(new Vector2(this.Lines[MapScript.curLine].RParam.X, this.Lines[MapScript.curLine].RParam.Y), this.Lines[MapScript.curLine].RParam.Width, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5]), Convert.ToInt32(this.Lines[MapScript.curLine].SParam[6]), Convert.ToInt32(this.Lines[MapScript.curLine].SParam[7]));
							break;
						case MapCommands.Gate:
							Game1.dManager.AddGate(MapScript.curLine, this.Lines[MapScript.curLine].VParam * 2f, this.Lines[MapScript.curLine].IParam);
							this.map.terrainRegion.Add(new IdRectRegion(new Rectangle((int)(this.Lines[MapScript.curLine].VParam * 2f).X - 300, (int)(this.Lines[MapScript.curLine].VParam * 2f).Y - 100, 600, 250), 2));
							break;
						case MapCommands.Door:
							this.map.doorRegions.Add(new DoorRegion(this.Lines[MapScript.curLine].IParam, this.Lines[MapScript.curLine].RParam, this.Lines[MapScript.curLine].VParam, this.Lines[MapScript.curLine].SParam[6]));
							break;
						case MapCommands.Bomb:
							this.map.bombRegions.Add(new BombRegion(this.Lines[MapScript.curLine].VParam * 2f, (int)this.Lines[MapScript.curLine].VParam.Length(), this.Lines[MapScript.curLine].IParam, _forceSpawn: false));
							break;
						case MapCommands.KillBombs:
							this.map.killBombs = true;
							break;
						case MapCommands.Fog:
							this.map.fogRegion = this.Lines[MapScript.curLine].RParam;
							this.map.fogRegionClear = false;
							break;
						case MapCommands.NoFog:
							this.map.fogRegion = this.Lines[MapScript.curLine].RParam;
							this.map.fogRegionClear = true;
							break;
						case MapCommands.Dark:
							this.map.darkRegions.Add(new DarkRegion(this.Lines[MapScript.curLine].RParam, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5]), Convert.ToInt32(this.Lines[MapScript.curLine].SParam[6])));
							break;
						case MapCommands.Bright:
							this.map.brightRegions.Add(new BrightRegion(this.Lines[MapScript.curLine].RParam));
							break;
						case MapCommands.NoPrecipRegion:
							this.map.noPrecipRegion.Add(this.Lines[MapScript.curLine].RParam);
							break;
						case MapCommands.WeatherColorRegion:
							this.map.weatherColorRegion.Add(new IdRectRegion(this.Lines[MapScript.curLine].RParam, this.Lines[MapScript.curLine].IParam));
							break;
						case MapCommands.WeatherRegion:
							this.map.weatherRegion.Add(new IdRectRegion(this.Lines[MapScript.curLine].RParam, this.Lines[MapScript.curLine].IParam));
							break;
						case MapCommands.IfEvent:
							if (Game1.events.currentEvent >= this.Lines[MapScript.curLine].IParam)
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.IfSideEvent:
							if (!Game1.events.sideEventAvailable[this.Lines[MapScript.curLine].IParam])
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.IfNotEvent:
							if (Game1.events.currentEvent < this.Lines[MapScript.curLine].IParam)
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.IfNotSideEvent:
							if (Game1.events.sideEventAvailable[this.Lines[MapScript.curLine].IParam])
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.InitEvent:
							Game1.events.InitEvent(this.Lines[MapScript.curLine].IParam, isSideEvent: false);
							break;
						case MapCommands.InitSideEvent:
							Game1.events.InitEvent(this.Lines[MapScript.curLine].IParam, isSideEvent: true);
							break;
						case MapCommands.TriggerHelp:
							MapScript.center = this.Lines[MapScript.curLine].VParam * 2f;
							if (c[0].Location.X > MapScript.center.X - 200f && c[0].Location.X < MapScript.center.X + 200f && c[0].Location.Y > MapScript.center.Y - 1200f && c[0].Location.Y < MapScript.center.Y + 600f)
							{
								Game1.hud.InitHelp(this.Lines[MapScript.curLine].SParam[3], restart: false, -1);
							}
							break;
						case MapCommands.TriggerEvent:
						{
							if (Game1.events.currentEvent >= this.Lines[MapScript.curLine].IParam || !(this.map.GetTransVal() <= 2f))
							{
								break;
							}
							MapScript.center = this.Lines[MapScript.curLine].VParam * 2f;
							if (c[0].Location.X > MapScript.center.X - 100f && c[0].Location.X < MapScript.center.X + 100f && c[0].Location.Y > MapScript.center.Y - 1200f && c[0].Location.Y < MapScript.center.Y + 600f)
							{
								if (Game1.stats.playerLifeState == 0)
								{
									Game1.events.InitEvent(this.Lines[MapScript.curLine].IParam, isSideEvent: false);
								}
								else
								{
									c[0].Trajectory.X = 0f;
								}
							}
							if (MapScript.npcIDList.Count <= 0)
							{
								break;
							}
							for (int i = 0; i < MapScript.npcIDList.Count; i++)
							{
								if (c[MapScript.npcIDList[i]].Ai != null && c[MapScript.npcIDList[i]].Location.Y > MapScript.center.Y - 1200f && c[MapScript.npcIDList[i]].Location.Y < MapScript.center.Y + 600f)
								{
									if (c[MapScript.npcIDList[i]].Ai.jobType == JobType.RunRight && c[MapScript.npcIDList[i]].Location.X > MapScript.center.X - 160f && c[MapScript.npcIDList[i]].Location.X < MapScript.center.X)
									{
										c[MapScript.npcIDList[i]].Ai.jobType = JobType.RunLeft;
									}
									else if (c[MapScript.npcIDList[i]].Ai.jobType == JobType.RunLeft && c[MapScript.npcIDList[i]].Location.X < MapScript.center.X + 160f && c[MapScript.npcIDList[i]].Location.X > MapScript.center.X)
									{
										c[MapScript.npcIDList[i]].Ai.jobType = JobType.RunRight;
									}
								}
							}
							break;
						}
						case MapCommands.TriggerSideEvent:
							if (!Game1.events.sideEventAvailable[this.Lines[MapScript.curLine].IParam] || Game1.events.anyEvent || !(this.map.GetTransVal() <= 2f))
							{
								break;
							}
							MapScript.center = this.Lines[MapScript.curLine].VParam * 2f;
							if (c[0].Location.X > MapScript.center.X - 100f && c[0].Location.X < MapScript.center.X + 100f && c[0].Location.Y > MapScript.center.Y - 1200f && c[0].Location.Y < MapScript.center.Y + 600f)
							{
								if (Game1.stats.playerLifeState == 0)
								{
									Game1.events.InitEvent(this.Lines[MapScript.curLine].IParam, isSideEvent: true);
								}
								else
								{
									c[0].Trajectory.X = 0f;
								}
							}
							break;
						case MapCommands.TriggerMapEvent:
							MapScript.center = this.Lines[MapScript.curLine].VParam * 2f;
							if (c[0].Location.X > MapScript.center.X - 100f && c[0].Location.X < MapScript.center.X + 100f && c[0].Location.Y > MapScript.center.Y - 1200f && c[0].Location.Y < MapScript.center.Y + 600f)
							{
								if (Game1.stats.playerLifeState == 0)
								{
									this.GotoTag(this.Lines[MapScript.curLine].SParam[3]);
								}
								else
								{
									c[0].Trajectory.X = 0f;
								}
							}
							break;
						case MapCommands.UpdateEvent:
							if (Game1.events.currentEvent < this.Lines[MapScript.curLine].IParam && Game1.events.currentEvent > this.Lines[MapScript.curLine].IParam - 200 && Game1.stats.playerLifeState == 0)
							{
								Game1.events.currentEvent = this.Lines[MapScript.curLine].IParam;
							}
							break;
						case MapCommands.TriggerBlock:
							this.map.leftBlock = this.Lines[MapScript.curLine].VParam.X * 2f;
							this.map.rightBlock = this.Lines[MapScript.curLine].VParam.Y * 2f;
							break;
						case MapCommands.ResetBlock:
							this.map.rightBlock = 0f;
							this.map.leftBlock = 0f;
							break;
						case MapCommands.EventRegion:
							this.map.eventRegion.Add(new EventRegion(this.Lines[MapScript.curLine].RParam, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5])));
							break;
						case MapCommands.InitChallenge:
							Game1.cManager.challengeInitPos = this.Lines[MapScript.curLine].VParam * 2f;
							ambienceList?.Add(new AmbientCue("challenge_hum", new Rectangle((int)(this.Lines[MapScript.curLine].VParam.X * 2f) - 100, (int)(this.Lines[MapScript.curLine].VParam.Y * 2f - 200f), 200, 500), _stereo: true));
							break;
						case MapCommands.EndChallenge:
							Game1.cManager.challengeCompletePos = this.Lines[MapScript.curLine].VParam * 2f;
							ambienceList?.Add(new AmbientCue("challenge_hum", new Rectangle((int)(this.Lines[MapScript.curLine].VParam.X * 2f) - 100, (int)(this.Lines[MapScript.curLine].VParam.Y * 2f - 200f), 200, 500), _stereo: true));
							break;
						case MapCommands.Reward:
							if (Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).Stage == 0)
							{
								Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].SpawnReward(this.Lines[MapScript.curLine].VParam * 2f, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).ID, Game1.savegame.GetItemID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, this.GetMapName() + " " + num).UniqueID, pMan);
							}
							num++;
							break;
						case MapCommands.Warp:
							this.map.warpRegions.Add(new WarpRegion(MapScript.curLine, this.Lines[MapScript.curLine].VParam * 2f, new Vector2(Convert.ToInt32(this.Lines[MapScript.curLine].SParam[3]), Convert.ToInt32(this.Lines[MapScript.curLine].SParam[4])) * 2f, this.Lines[MapScript.curLine].SParam[5]));
							ambienceList?.Add(new AmbientCue("warp_hum", this.Lines[MapScript.curLine].RParam, _stereo: true));
							break;
						case MapCommands.SetMapMin:
							MapScript.pRightEdge = this.map.rightEdge;
							MapScript.pLeftEdge = this.map.leftEdge;
							MapScript.pTopEdge = this.map.topEdge;
							MapScript.pBottomEdge = this.map.bottomEdge;
							this.map.leftEdge = this.Lines[MapScript.curLine].VParam.X * 2f;
							this.map.topEdge = this.Lines[MapScript.curLine].VParam.Y * 2f;
							break;
						case MapCommands.SetMapMax:
							MapScript.pRightEdge = this.map.rightEdge;
							MapScript.pLeftEdge = this.map.leftEdge;
							MapScript.pTopEdge = this.map.topEdge;
							MapScript.pBottomEdge = this.map.bottomEdge;
							this.map.rightEdge = this.Lines[MapScript.curLine].VParam.X * 2f;
							this.map.bottomEdge = this.Lines[MapScript.curLine].VParam.Y * 2f;
							break;
						case MapCommands.SetLeftExit:
							this.map.TransitionDestination[3] = this.Lines[MapScript.curLine].SParam[1];
							break;
						case MapCommands.SetRightExit:
							this.map.TransitionDestination[1] = this.Lines[MapScript.curLine].SParam[1];
							break;
						case MapCommands.SetUpExit:
							this.map.TransitionDestination[0] = this.Lines[MapScript.curLine].SParam[1];
							break;
						case MapCommands.SetDownExit:
							this.map.TransitionDestination[2] = this.Lines[MapScript.curLine].SParam[1];
							break;
						case MapCommands.SetLeftEntrance:
							if (this.map.transDir == TransitionDirection.Right)
							{
								c[0].Location = (c[0].PLoc = new Vector2(this.map.leftEdge, this.Lines[MapScript.curLine].VParam.Y * 2f - (MapScript.pLeftExitPoint - c[0].Location.Y)));
								this.map.transDir = TransitionDirection.None;
							}
							this.map.rightExitPoint = this.Lines[MapScript.curLine].VParam.Y * 2f;
							break;
						case MapCommands.SetRightEntrance:
							if (this.map.transDir == TransitionDirection.Left)
							{
								c[0].Location = (c[0].PLoc = new Vector2(this.map.rightEdge, this.Lines[MapScript.curLine].VParam.Y * 2f - (MapScript.pRightExitPoint - c[0].Location.Y)));
								this.map.transDir = TransitionDirection.None;
							}
							this.map.leftExitPoint = this.Lines[MapScript.curLine].VParam.Y * 2f;
							break;
						case MapCommands.SetUpEntrance:
							if (this.map.transDir == TransitionDirection.Down)
							{
								c[0].Location = (c[0].PLoc = new Vector2(this.Lines[MapScript.curLine].VParam.X * 2f - (MapScript.pUpExitPoint - c[0].Location.X), this.map.topEdge));
								this.map.transDir = TransitionDirection.None;
							}
							this.map.bottomExitPoint = this.Lines[MapScript.curLine].VParam.X * 2f;
							break;
						case MapCommands.SetDownEntrance:
							if (this.map.transDir == TransitionDirection.Up)
							{
								c[0].Location = (c[0].PLoc = new Vector2(this.Lines[MapScript.curLine].VParam.X * 2f - (MapScript.pDownExitPoint - c[0].Location.X), this.map.bottomEdge));
								this.map.transDir = TransitionDirection.None;
							}
							this.map.topExitPoint = this.Lines[MapScript.curLine].VParam.X * 2f;
							break;
						case MapCommands.SetIntroEntrance:
							if (this.map.transDir == TransitionDirection.Intro)
							{
								c[0].Location = (c[0].PLoc = this.Lines[MapScript.curLine].VParam * 2f);
								c[0].Face = CharDir.Right;
								c[0].SetAnim("jump", 2, 0);
								c[0].State = CharState.Air;
								c[0].Trajectory = new Vector2(0f, 0f);
								this.map.transDir = TransitionDirection.None;
							}
							break;
						case MapCommands.AddZoom:
						{
							int num9 = 50;
							if (this.Lines[MapScript.curLine].SParam[6] != "0")
							{
								num9 = Convert.ToInt32(this.Lines[MapScript.curLine].SParam[6]);
							}
							Game1.camera.GetZoomRectList.Add(this.Lines[MapScript.curLine].RParam, new Vector2(Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5]), num9));
							break;
						}
						case MapCommands.AddPan:
						{
							int num6 = Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5]);
							int num7 = Convert.ToInt32(this.Lines[MapScript.curLine].SParam[6]);
							int num8 = 20;
							if (this.Lines[MapScript.curLine].SParam[7] != "0")
							{
								num8 = Convert.ToInt32(this.Lines[MapScript.curLine].SParam[7]);
							}
							Game1.camera.GetPanRectList.Add(this.Lines[MapScript.curLine].RParam, new Vector3(num6, num7, num8));
							break;
						}
						case MapCommands.AddBoost:
							this.map.boostRegions.Add(new BoostRegion(this.Lines[MapScript.curLine].RParam, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5])));
							ambienceList?.Add(new AmbientCue("boost_wind", this.Lines[MapScript.curLine].RParam, _stereo: true));
							break;
						case MapCommands.TerrainRegion:
							this.map.terrainRegion.Add(new IdRectRegion(this.Lines[MapScript.curLine].RParam, Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5])));
							break;
						case MapCommands.AddSound:
							ambienceList?.Add(new AmbientCue(this.Lines[MapScript.curLine].SParam[5], this.Lines[MapScript.curLine].RParam, _stereo: true));
							break;
						case MapCommands.Reverb:
							this.map.reverbRegions.Add(new ReverbRegion(this.Lines[MapScript.curLine].RParam, (float)Convert.ToInt32(this.Lines[MapScript.curLine].SParam[5]) / 100f));
							break;
						case MapCommands.ReverbMin:
							this.map.reverbMin = (float)Convert.ToInt32(this.Lines[MapScript.curLine].SParam[1]) / 100f;
							break;
						case MapCommands.PlayMusic:
							if (this.Lines[MapScript.curLine].SParam[1] == "fade")
							{
								Sound.FadeMusicOut(4f);
							}
							else
							{
								Music.Play(this.Lines[MapScript.curLine].SParam[1]);
							}
							break;
						case MapCommands.Wait:
							MapScript.waiting = (float)this.Lines[MapScript.curLine].IParam / 100f;
							flag = true;
							break;
						case MapCommands.IfTrueGoto:
							if (this.Flags.GetFlag(this.Lines[MapScript.curLine].SParam[1]))
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.IfFalseGoto:
							if (!this.Flags.GetFlag(this.Lines[MapScript.curLine].SParam[1]))
							{
								this.GotoTag(this.Lines[MapScript.curLine].SParam[2]);
							}
							break;
						case MapCommands.Goto:
							this.GotoTag(this.Lines[MapScript.curLine].SParam[1]);
							break;
						case MapCommands.Stop:
							this.map.rightBlock = 0f;
							this.map.leftBlock = 0f;
							this.IsReading = false;
							flag = true;
							break;
						case MapCommands.Tag:
						case MapCommands.Water:
						case MapCommands.DestructLamp:
							break;
						}
					}
					catch (Exception)
					{
					}
					continue;
				}
				this.map.rightBlock = 0f;
				this.map.leftBlock = 0f;
				this.IsReading = false;
				flag = true;
				break;
			}
		}

		public bool GotoTag(string tag)
		{
			for (int i = 0; i < this.Lines.Length; i++)
			{
				if (this.Lines[i] != null && this.Lines[i].Command == MapCommands.Tag && this.Lines[i].SParam[1] == tag)
				{
					MapScript.curLine = i;
					return true;
				}
			}
			return false;
		}

		private string GetMapName()
		{
			if (this.map.path.Contains("alt"))
			{
				string text = this.map.path.Replace("alt", ".");
				return text.Split('.')[0];
			}
			return this.map.path;
		}

		public bool renderable(Vector2 center)
		{
			if (MapScript.renderRect.Contains((int)(center.X * Game1.worldScale - Game1.Scroll.X), (int)(center.Y * Game1.worldScale - Game1.Scroll.Y)))
			{
				return true;
			}
			return false;
		}
	}
}

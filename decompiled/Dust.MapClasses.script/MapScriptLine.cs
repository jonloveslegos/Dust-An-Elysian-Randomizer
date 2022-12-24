using System;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses.script
{
	public class MapScriptLine
	{
		public MapCommands Command;

		public int IParam;

		public Vector2 VParam;

		public Rectangle RParam;

		public string[] SParam;

		public MapScriptLine(string line)
		{
			if (line.Length < 1)
			{
				return;
			}
			try
			{
				this.SParam = line.Split(' ');
				switch (this.SParam[0])
				{
				case "fog":
					this.Command = MapCommands.Fog;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "nofog":
					this.Command = MapCommands.NoFog;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "dark":
					this.Command = MapCommands.Dark;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "bright":
					this.Command = MapCommands.Bright;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "noprecip":
					this.Command = MapCommands.NoPrecipRegion;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "water":
					this.Command = MapCommands.Water;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "enemy":
					this.Command = MapCommands.Enemy;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "party":
					this.Command = MapCommands.Party;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "npc":
					this.Command = MapCommands.NPC;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "makebucket":
					this.Command = MapCommands.MakeBucket;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "addbucket":
					this.Command = MapCommands.AddBucket;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "ifnotbucket":
					this.Command = MapCommands.IfNotBucketGoto;
					break;
				case "wait":
					this.Command = MapCommands.Wait;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "save":
					this.Command = MapCommands.Save;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2 - 300, Convert.ToInt32(this.SParam[2]) * 2 - 150, 600, 400);
					break;
				case "shop":
					this.Command = MapCommands.Shop;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "upgrade":
					this.Command = MapCommands.Upgrade;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "key":
					this.Command = MapCommands.Key;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "note":
					this.Command = MapCommands.Note;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "questitem":
					this.Command = MapCommands.QuestItem;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "questcreature":
					this.Command = MapCommands.QuestCreature;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					this.IParam = Convert.ToInt32(this.SParam[4]);
					break;
				case "chest":
					this.Command = MapCommands.Chest;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "cage":
					this.Command = MapCommands.Cage;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "destruct":
					this.Command = MapCommands.Destructable;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "dplatform":
					this.Command = MapCommands.DestructPlatform;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "mplatform":
					this.Command = MapCommands.MovingPlatform;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "gate":
					this.Command = MapCommands.Gate;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "door":
					this.Command = MapCommands.Door;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					this.IParam = Convert.ToInt32(this.SParam[5]);
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[7]), Convert.ToSingle(this.SParam[8])) * 2f;
					break;
				case "bomb":
					this.Command = MapCommands.Bomb;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "killbombs":
					this.Command = MapCommands.KillBombs;
					break;
				case "warp":
					this.Command = MapCommands.Warp;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2 - 50, Convert.ToInt32(this.SParam[2]) * 2 - 50, 100, 100);
					break;
				case "ifevent":
					this.Command = MapCommands.IfEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "ifsideevent":
					this.Command = MapCommands.IfSideEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "ifnotevent":
					this.Command = MapCommands.IfNotEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "ifnotsideevent":
					this.Command = MapCommands.IfNotSideEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "initevent":
					this.Command = MapCommands.InitEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "initsideevent":
					this.Command = MapCommands.InitSideEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "triggerhelp":
					this.Command = MapCommands.TriggerHelp;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "triggerevent":
					this.Command = MapCommands.TriggerEvent;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "triggersideevent":
					this.Command = MapCommands.TriggerSideEvent;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "triggermapevent":
					this.Command = MapCommands.TriggerMapEvent;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "updateevent":
					this.Command = MapCommands.UpdateEvent;
					this.IParam = Convert.ToInt32(this.SParam[1]);
					break;
				case "eventregion":
					this.Command = MapCommands.EventRegion;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "triggerblock":
					this.Command = MapCommands.TriggerBlock;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "resetblock":
					this.Command = MapCommands.ResetBlock;
					break;
				case "initchallenge":
					this.Command = MapCommands.InitChallenge;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "reward":
					this.Command = MapCommands.Reward;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "lamp":
					this.Command = MapCommands.DestructLamp;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "endchallenge":
					this.Command = MapCommands.EndChallenge;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "iftrue":
					this.Command = MapCommands.IfTrueGoto;
					break;
				case "iffalse":
					this.Command = MapCommands.IfFalseGoto;
					break;
				case "goto":
					this.Command = MapCommands.Goto;
					break;
				case "stop":
					this.Command = MapCommands.Stop;
					break;
				case "tag":
					this.Command = MapCommands.Tag;
					break;
				case "mapmin":
					this.Command = MapCommands.SetMapMin;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "mapmax":
					this.Command = MapCommands.SetMapMax;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "leftexit":
					this.Command = MapCommands.SetLeftExit;
					break;
				case "leftentrance":
					this.Command = MapCommands.SetLeftEntrance;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "rightexit":
					this.Command = MapCommands.SetRightExit;
					break;
				case "rightentrance":
					this.Command = MapCommands.SetRightEntrance;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "topexit":
					this.Command = MapCommands.SetUpExit;
					break;
				case "topentrance":
					this.Command = MapCommands.SetUpEntrance;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "bottomexit":
					this.Command = MapCommands.SetDownExit;
					break;
				case "bottomentrance":
					this.Command = MapCommands.SetDownEntrance;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "introentrance":
					this.Command = MapCommands.SetIntroEntrance;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "zoom":
					this.Command = MapCommands.AddZoom;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "pan":
					this.Command = MapCommands.AddPan;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "boost":
					this.Command = MapCommands.AddBoost;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "terrain":
					this.Command = MapCommands.TerrainRegion;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "weathercolor":
					this.Command = MapCommands.WeatherColorRegion;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					this.IParam = Convert.ToInt32(this.SParam[5]);
					break;
				case "weatherregion":
					this.Command = MapCommands.WeatherRegion;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					this.IParam = Convert.ToInt32(this.SParam[5]);
					break;
				case "sound":
					this.Command = MapCommands.AddSound;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "reverb":
					this.Command = MapCommands.Reverb;
					this.RParam = new Rectangle(Convert.ToInt32(this.SParam[1]) * 2, Convert.ToInt32(this.SParam[2]) * 2, (Convert.ToInt32(this.SParam[3]) - Convert.ToInt32(this.SParam[1])) * 2, (Convert.ToInt32(this.SParam[4]) - Convert.ToInt32(this.SParam[2])) * 2);
					break;
				case "reverbmin":
					this.Command = MapCommands.ReverbMin;
					break;
				case "playmusic":
					this.Command = MapCommands.PlayMusic;
					break;
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

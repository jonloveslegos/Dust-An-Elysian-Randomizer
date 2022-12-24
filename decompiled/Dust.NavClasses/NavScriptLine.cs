using System;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.NavClasses
{
	public class NavScriptLine
	{
		public MapCommands Command;

		public int IParam;

		public Vector2 VParam;

		public Rectangle RParam;

		public string[] SParam;

		public NavScriptLine(string line)
		{
			if (line == null || line.Length < 1)
			{
				return;
			}
			try
			{
				this.SParam = line.Split(' ');
				switch (this.SParam[0])
				{
				case "enemy":
					this.Command = MapCommands.Enemy;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "npc":
					this.Command = MapCommands.NPC;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[2]), Convert.ToSingle(this.SParam[3]));
					break;
				case "save":
					this.Command = MapCommands.Save;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "shop":
					this.Command = MapCommands.Shop;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "upgrade":
					this.Command = MapCommands.Upgrade;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "key":
					this.Command = MapCommands.Key;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "note":
					this.Command = MapCommands.Note;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
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
					break;
				case "cage":
					this.Command = MapCommands.Cage;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "destruct":
					this.Command = MapCommands.Destructable;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					this.IParam = Convert.ToInt32(this.SParam[3]);
					break;
				case "initchallenge":
					this.Command = MapCommands.InitChallenge;
					break;
				case "reward":
					this.Command = MapCommands.Reward;
					break;
				case "lamp":
					this.Command = MapCommands.DestructLamp;
					this.VParam = new Vector2(Convert.ToSingle(this.SParam[1]), Convert.ToSingle(this.SParam[2]));
					break;
				case "stop":
					this.Command = MapCommands.Stop;
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
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

using System;

namespace Dust.CharClasses
{
	public class ScriptLine
	{
		private Commands command;

		private string sParam;

		private int iParam;

		public Commands Command => this.command;

		public int IParam => this.iParam;

		public string SParam => this.sParam;

		public ScriptLine(string line)
		{
			string[] array = line.Split(' ');
			try
			{
				switch (array[0].Trim().ToLower())
				{
				case "setanim":
					this.command = Commands.SetAnim;
					this.sParam = array[1];
					if (array.Length > 2)
					{
						this.iParam = Convert.ToInt32(array[2]);
					}
					else
					{
						this.iParam = 0;
					}
					break;
				case "goto":
					this.command = Commands.Goto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ifupgoto":
					this.command = Commands.IfUpGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ifdowngoto":
					this.command = Commands.IfDownGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ifupgrade":
					this.command = Commands.IfUpgrade;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "float":
					this.command = Commands.Float;
					break;
				case "unfloat":
					this.command = Commands.UnFloat;
					break;
				case "slide":
					this.command = Commands.Slide;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "backup":
					this.command = Commands.Backup;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "setjump":
					this.command = Commands.SetJump;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "setfall":
					this.command = Commands.SetFall;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "joymove":
					this.command = Commands.JoyMove;
					break;
				case "clearkeys":
					this.command = Commands.ClearKeys;
					break;
				case "clearsec":
					this.command = Commands.ClearSec;
					break;
				case "setanygoto":
					this.command = Commands.SetAnyGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "setsecgoto":
					this.command = Commands.SetSecondaryGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "setqgoto":
					this.command = Commands.SetQueueGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "setsecqgoto":
					this.command = Commands.SetSecQueueGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "release":
					this.command = Commands.Release;
					break;
				case "ethereal":
					this.command = Commands.Ethereal;
					if (array.Length > 1)
					{
						this.iParam = Convert.ToInt32(array[1]);
					}
					else
					{
						this.iParam = 2;
					}
					break;
				case "solid":
					this.command = Commands.Solid;
					break;
				case "speed":
					this.command = Commands.Speed;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "jumpvelocity":
					this.command = Commands.JumpVelocity;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "hp":
					this.command = Commands.HP;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "strength":
					this.command = Commands.Strength;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "smoothon":
					this.command = Commands.InterpolateOn;
					break;
				case "smoothoff":
					this.command = Commands.InterpolateOff;
					break;
				case "deathcheck":
					this.command = Commands.DeathCheck;
					break;
				case "ifdyinggoto":
					this.command = Commands.IfDyingGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ifdustdead":
					this.command = Commands.IfDustDead;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ifnoholdgoto":
					this.command = Commands.IfNoHoldGoto;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "infront":
					this.command = Commands.InFront;
					break;
				case "inback":
					this.command = Commands.InBack;
					break;
				case "checkspin":
					this.command = Commands.CheckSpin;
					break;
				case "cancancel":
					this.command = Commands.CanCancel;
					break;
				case "shake":
					this.command = Commands.Shake;
					break;
				case "seek":
					this.command = Commands.Seek;
					break;
				case "killme":
					this.command = Commands.KillMe;
					break;
				case "killmeinstantly":
					this.command = Commands.KillMeInstantly;
					break;
				case "play":
					this.command = Commands.PlaySound;
					this.sParam = array[1];
					break;
				case "slowtime":
					this.command = Commands.SlowTime;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "parry":
					this.command = Commands.Parry;
					break;
				case "parryopen":
					this.command = Commands.ParryOpen;
					break;
				case "blocking":
					this.command = Commands.Blocking;
					break;
				case "uninterruptable":
					this.command = Commands.Uninterruptable;
					break;
				case "initdustseek":
					this.command = Commands.InitDustSeek;
					break;
				case "maxanimframe":
					this.command = Commands.MaxAnimFrame;
					this.iParam = Convert.ToInt32(array[1]);
					break;
				case "ai":
					this.command = Commands.AI;
					this.sParam = array[1];
					break;
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

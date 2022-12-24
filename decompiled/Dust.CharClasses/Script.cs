using Dust.Audio;
using Microsoft.Xna.Framework;

namespace Dust.CharClasses
{
	internal class Script
	{
		private Character character;

		private ScriptLine line;

		private CharDef charDef;

		private Animation animation;

		private KeyFrame keyFrame;

		private static object syncObject = new object();

		public Script(Character character)
		{
			this.character = character;
		}

		public void DoScript(int animIdx, int keyFrameIdx)
		{
			lock (Script.syncObject)
			{
				this.charDef = this.character.Definition;
				this.animation = this.charDef.Animations[animIdx];
				this.keyFrame = this.animation.KeyFrames[keyFrameIdx];
				if (this.keyFrame.Scripts == null)
				{
					return;
				}
				for (int i = 0; i < this.keyFrame.Scripts.Length; i++)
				{
					this.line = this.keyFrame.Scripts[i];
					switch (this.line.Command)
					{
					default:
						return;
					case Commands.SetAnim:
						this.character.SetAnim(this.line.SParam, this.line.IParam, -1);
						return;
					case Commands.Goto:
						if (this.line.IParam != keyFrameIdx)
						{
							this.character.SetFrame(this.line.IParam);
						}
						return;
					case Commands.Float:
						this.character.Floating = true;
						break;
					case Commands.UnFloat:
						this.character.Floating = false;
						break;
					case Commands.Slide:
						this.character.Slide(this.line.IParam);
						break;
					case Commands.Backup:
						this.character.Slide(-this.line.IParam);
						break;
					case Commands.SetJump:
						this.character.SetJump(this.line.IParam, jumped: false);
						break;
					case Commands.SetFall:
						this.character.SetJump(-this.line.IParam, jumped: false);
						break;
					case Commands.JoyMove:
						if (this.character.KeyLeft)
						{
							this.character.Trajectory -= new Vector2(this.character.Speed, 0f);
						}
						else if (this.character.KeyRight)
						{
							this.character.Trajectory += new Vector2(this.character.Speed, 0f);
						}
						break;
					case Commands.ClearKeys:
						this.character.PressedKey = PressedKeys.None;
						this.character.GotoGoal[1] = -1;
						this.character.QueuedKey = QueuedKey.None;
						break;
					case Commands.ClearSec:
						this.character.PressedKey = PressedKeys.None;
						this.character.GotoGoal[2] = -1;
						this.character.QueuedKey = QueuedKey.None;
						break;
					case Commands.SetAnyGoto:
						this.character.GotoGoal[1] = this.line.IParam;
						if (this.character.QueuedKey == QueuedKey.Attack && this.character.QueueGoal[1] > -1)
						{
							this.character.QueueTrig = this.character.QueueGoal[1];
						}
						break;
					case Commands.SetSecondaryGoto:
						this.character.GotoGoal[2] = this.line.IParam;
						if (this.character.QueuedKey == QueuedKey.Secondary && this.character.QueueGoal[2] > -1)
						{
							this.character.QueueTrig = this.character.QueueGoal[2];
						}
						break;
					case Commands.SetQueueGoto:
						this.character.QueueGoal[1] = this.line.IParam;
						break;
					case Commands.SetSecQueueGoto:
						this.character.QueueGoal[2] = this.line.IParam;
						break;
					case Commands.Ethereal:
						this.character.Ethereal = (EtherealState)this.line.IParam;
						break;
					case Commands.Solid:
						if (!Game1.events.anyEvent)
						{
							this.character.Ethereal = EtherealState.Normal;
						}
						break;
					case Commands.Release:
						this.character.Holding = false;
						break;
					case Commands.Speed:
						this.character.Speed = this.line.IParam;
						break;
					case Commands.JumpVelocity:
						this.character.JumpVelocity = this.line.IParam;
						break;
					case Commands.HP:
						this.character.HP = (this.character.MaxHP = this.line.IParam);
						break;
					case Commands.Strength:
						this.character.Strength = this.line.IParam;
						break;
					case Commands.InterpolateOn:
						this.character.InterpolateOff(1);
						break;
					case Commands.InterpolateOff:
						this.character.InterpolateOff(0);
						break;
					case Commands.DeathCheck:
						if (this.character.HP <= 0)
						{
							this.character.KillMe(instantly: false);
						}
						break;
					case Commands.IfDyingGoto:
						if (this.character.DyingFrame > -1f)
						{
							this.character.SetFrame(this.line.IParam);
							return;
						}
						break;
					case Commands.IfDustDead:
						if (Game1.stats.playerLifeState > 0)
						{
							this.character.SetFrame(this.line.IParam);
							return;
						}
						break;
					case Commands.IfNoHoldGoto:
						if (!this.character.Holding)
						{
							this.character.SetFrame(this.line.IParam);
							return;
						}
						break;
					case Commands.IfUpgrade:
						if (Game1.stats.upgrade[this.line.IParam] == 0)
						{
							return;
						}
						break;
					case Commands.InFront:
						Game1.stats.inFront = true;
						break;
					case Commands.InBack:
					{
						bool flag = true;
						for (int j = 1; j < Game1.character.Length; j++)
						{
							if (Game1.character[j].Exists >= CharExists.Exists && Game1.character[j].LiftType == CanLiftType.Immovable && Game1.character[j].renderable)
							{
								flag = false;
								break;
							}
						}
						Game1.stats.inFront = !flag;
						break;
					}
					case Commands.CheckSpin:
						this.character.GotoGoal[2] = 1000;
						break;
					case Commands.CanCancel:
						this.character.CanCancel = true;
						break;
					case Commands.Shake:
						this.character.Impact(this.character, this.character.Location);
						break;
					case Commands.Seek:
						this.character.Seek(force: false);
						break;
					case Commands.KillMe:
						this.character.KillMe(instantly: false);
						break;
					case Commands.KillMeInstantly:
						this.character.KillMe(instantly: true);
						break;
					case Commands.PlaySound:
						if (this.character.Ai == null || this.character.Ai.PlaySound(this.line.SParam))
						{
							Sound.PlayCue(this.line.SParam, this.character.Location, (Game1.character[0].Location - this.character.Location).Length() / 2f);
						}
						break;
					case Commands.SlowTime:
						Game1.SlowTime = (float)this.line.IParam / 10f;
						break;
					case Commands.Parry:
						this.character.Defense = DefenseStates.Parrying;
						break;
					case Commands.ParryOpen:
						this.character.Defense = DefenseStates.ParryOpen;
						break;
					case Commands.Blocking:
						this.character.Defense = DefenseStates.BlockingOnly;
						break;
					case Commands.Uninterruptable:
						this.character.Defense = DefenseStates.Uninterruptable;
						break;
					case Commands.InitDustSeek:
						if (this.character.InitDustSeek())
						{
							return;
						}
						break;
					case Commands.MaxAnimFrame:
						this.character.maxAnimFrames = this.line.IParam;
						break;
					case Commands.AI:
						if (this.character.Ai == null)
						{
							this.character.Ai = Game1.GetCharacterAI(this.line.SParam, this.character);
						}
						break;
					case Commands.IfUpGoto:
					case Commands.IfDownGoto:
						return;
					}
				}
			}
		}
	}
}

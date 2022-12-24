using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	public class AI
	{
		public JobType jobType;

		protected int targ = -1;

		protected float jobFrame = -1f;

		protected bool targVertDistance;

		public bool overrideAI;

		public float regionMultiplier;

		public float initPos;

		protected int maxDistance;

		protected int randomInt;

		protected Character me;

		public virtual void Update(Character[] c, int ID, Map map)
		{
			this.me.KeyLeft = (this.me.KeyRight = false);
			this.me.KeyUp = (this.me.KeyDown = false);
			this.me.KeyAttack = (this.me.KeySecondary = false);
			this.me.KeyJump = false;
			this.JobTimer(c, ID);
			if (this.me.Team == Team.Friendly)
			{
				if (this.me.ID == Game1.hud.collectID)
				{
					this.jobType = JobType.Idle;
					this.jobFrame = 1f;
				}
				if (this.me.ID == Game1.hud.converseWithID && c[0].Trajectory.X == 0f)
				{
					if (this.jobType == JobType.StillCanTurn)
					{
						this.targ = 0;
						this.FaceTarg(c);
						if (Game1.hud.dialogueState == DialogueState.Active)
						{
							if (c[0].Location.X > this.me.Location.X && c[0].Location.X < this.me.Location.X + 150f)
							{
								c[0].Trajectory.X += 800f;
							}
							if (c[0].Location.X < this.me.Location.X && c[0].Location.X > this.me.Location.X - 150f)
							{
								c[0].Trajectory.X -= 800f;
							}
						}
					}
					else
					{
						if (this.jobType == JobType.Still)
						{
							return;
						}
						this.targ = 0;
						if (this.me.Location.X < c[this.targ].Location.X)
						{
							if (this.me.Location.X < c[this.targ].Location.X - 150f)
							{
								this.jobType = JobType.RunRight;
								this.DoJob(c, ID);
							}
							else if (this.me.Location.X > c[this.targ].Location.X - 120f)
							{
								this.jobType = JobType.RunLeft;
								this.DoJob(c, ID);
							}
							else
							{
								this.jobType = JobType.Idle;
								this.FaceTarg(c);
							}
						}
						else if (this.me.Location.X > c[this.targ].Location.X)
						{
							if (this.me.Location.X > c[this.targ].Location.X + 150f)
							{
								this.jobType = JobType.RunLeft;
								this.DoJob(c, ID);
							}
							else if (this.me.Location.X < c[this.targ].Location.X + 120f)
							{
								this.jobType = JobType.RunRight;
								this.DoJob(c, ID);
							}
							else
							{
								this.jobType = JobType.Idle;
								this.FaceTarg(c);
							}
						}
					}
				}
				else if (this.overrideAI)
				{
					this.jobType = JobType.Idle;
					this.targ = 0;
				}
				else
				{
					this.DoJob(c, ID);
				}
			}
			else if (this.me.Team == Team.Enemy && ((!Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None && Game1.hud.unlockState == 0 && this.me.LungeState == LungeStates.None && this.me.DyingFrame == -1f && this.me.DownTime == 0f) || this.overrideAI))
			{
				this.DoJob(c, ID);
			}
		}

		protected void DoJob(Character[] c, int ID)
		{
			switch (this.jobType)
			{
			case JobType.RunLeft:
				this.me.KeyLeft = true;
				this.JumpOverWall(c);
				break;
			case JobType.RunRight:
				this.me.KeyRight = true;
				this.JumpOverWall(c);
				break;
			case JobType.JumpRandomly:
				if (Rand.GetRandomFloat(0f, 1f) < 0.2f && this.CanJumpWall(this.me.JumpVelocity))
				{
					this.me.KeyJump = true;
				}
				if (this.me.Location.Y < c[0].Location.Y - 300f && Rand.GetRandomFloat(0f, 1f) < 0.1f)
				{
					this.me.KeyDown = true;
					this.me.KeyJump = true;
				}
				break;
			case JobType.MeleeChaseLunging:
			case JobType.MeleeChase:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (this.jobType == JobType.MeleeChaseLunging && Rand.GetRandomInt(0, 150 - this.me.Aggression) < 2 && !this.ChaseTarg(c, 800f))
					{
						this.me.SetLunge(LungeStates.Lunging, Math.Min(this.me.Speed * 2f + 200f, 1400f), this.me.JumpVelocity / 2f);
						break;
					}
					if (!this.ChaseTarg(c, this.me.DefaultWidth) && !this.FaceTarg(c) && this.CanReachTarg(c, this.me.DefaultWidth))
					{
						this.targVertDistance = this.me.Location.Y > c[this.targ].Location.Y - (float)this.me.Height && this.me.Location.Y < c[this.targ].Location.Y + (float)this.me.Height;
						if (this.targVertDistance)
						{
							float randomFloat = Rand.GetRandomFloat(0f, 101 - this.me.Aggression);
							if (randomFloat < 2f / (float)this.me.Aggression)
							{
								this.jobType = JobType.Avoid;
								this.jobFrame = Rand.GetRandomFloat(2f, 4f);
							}
							else if (randomFloat < 12f)
							{
								if (Rand.GetRandomInt(0, 2) == 0)
								{
									this.me.KeyAttack = true;
								}
								else
								{
									this.me.KeySecondary = true;
								}
								this.FaceTarg(c);
							}
						}
					}
					this.JumpOverWall(c);
					this.JumpToPlayer(c);
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.ChaseStop:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (this.me.LiftType != CanLiftType.NoLift && Rand.GetRandomInt(0, 150 - this.me.Aggression) < 2 && !this.ChaseTarg(c, 800f))
					{
						this.me.SetLunge(LungeStates.Lunging, Math.Min(this.me.Speed * 2f + 200f, 1400f), this.me.JumpVelocity / 2f);
						break;
					}
					if (this.me.State == CharState.Grounded && !this.ChaseTarg(c, this.me.DefaultWidth + 300))
					{
						this.jobType = JobType.ShootAnywhere;
						this.jobFrame = Rand.GetRandomFloat(1f, 2f);
					}
					this.JumpToPlayer(c);
					this.JumpOverWall(c);
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.ShootAnywhere:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (this.me.LiftType != CanLiftType.NoLift && Rand.GetRandomInt(0, 150 - this.me.Aggression) < 6)
					{
						this.me.SetLunge(LungeStates.Lunging, Math.Min(this.me.Speed * 2f + 200f, 1400f), this.me.JumpVelocity / 2f);
						break;
					}
					if (this.me.renderable && !this.FaceTarg(c))
					{
						float randomFloat2 = Rand.GetRandomFloat(0f, 101 - this.me.Aggression);
						if (randomFloat2 < 20f)
						{
							if (Rand.GetRandomInt(0, 2) == 0)
							{
								this.me.KeyAttack = true;
							}
							else
							{
								this.me.KeySecondary = true;
							}
						}
					}
					this.JumpToPlayer(c);
					this.JumpOverWall(c);
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.FlyMelee:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (!this.me.AnimName.StartsWith("fly"))
					{
						break;
					}
					if (!this.me.AnimName.Contains("attack"))
					{
						if (this.me.Location.Y > c[this.targ].Location.Y - (float)this.randomInt)
						{
							this.me.KeyUp = true;
						}
						else if (this.me.Location.Y < c[this.targ].Location.Y - (float)this.randomInt - 50f)
						{
							this.me.KeyDown = true;
						}
					}
					if (!this.me.WallInWay && this.ChaseTarg(c, this.me.DefaultWidth / 2 + 180))
					{
						break;
					}
					if (this.me.WallInWay)
					{
						this.me.Trajectory.Y = MathHelper.Clamp(this.me.Trajectory.Y * 2f, 0f - this.me.Speed, this.me.Speed);
					}
					else
					{
						if (Rand.GetRandomInt(0, 101 - this.me.Aggression) >= 12 || this.FaceTarg(c) || !this.CanReachTarg(c, this.me.DefaultWidth / 2 + 180) || !this.me.renderable)
						{
							break;
						}
						this.targVertDistance = this.me.Location.Y > c[this.targ].Location.Y - (float)this.me.Height * 2f && this.me.Location.Y < c[this.targ].Location.Y + (float)this.me.Height;
						if (this.targVertDistance)
						{
							if (this.me.Trajectory.Length() > 600f)
							{
								this.me.Trajectory *= 0.5f;
							}
							this.me.KeyDown = (this.me.KeyUp = (this.me.KeyLeft = (this.me.KeyRight = false)));
							this.me.KeyAttack = true;
						}
					}
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.DartAround:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					this.me.Trajectory = new Vector2(MathHelper.Clamp((c[this.targ].Location.X - this.me.Location.X) / 4f + (float)Rand.GetRandomInt(-800, 800), -1000f, 1000f), MathHelper.Clamp((c[this.targ].Location.Y - (float)c[this.targ].Height - this.me.Location.Y) / 4f + (float)Rand.GetRandomInt(-400, 800), -1000f, 1000f));
					this.jobFrame = Rand.GetRandomInt(3, 8);
					this.jobType = JobType.FlyShoot;
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.FlyShoot:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (!this.me.AnimName.StartsWith("fly"))
					{
						break;
					}
					this.ChaseTarg(c, this.me.DefaultWidth / 2 + 180);
					if (this.me.Location.Y > c[this.targ].Location.Y - (float)this.randomInt)
					{
						this.me.KeyUp = true;
					}
					else if (this.me.Location.Y < c[this.targ].Location.Y - (float)this.randomInt - 50f)
					{
						this.me.KeyDown = true;
					}
					if (!this.FaceTarg(c) && Rand.GetRandomInt(0, 101 - this.me.Aggression) < 4)
					{
						if (this.me.Location.Y > c[this.targ].Location.Y - (float)this.me.Height * 1.5f && this.me.Location.Y < c[this.targ].Location.Y + (float)this.me.Height * 0.5f)
						{
							this.targVertDistance = true;
						}
						else
						{
							this.targVertDistance = false;
						}
						if (this.CanReachTarg(c, Game1.screenWidth / 2) && this.targVertDistance && this.me.renderable && !this.me.AnimName.StartsWith("flyattack"))
						{
							this.me.KeyAttack = true;
						}
					}
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.FlyKeepDistance:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (!this.me.AnimName.Contains("attack"))
					{
						if ((this.me.Location - c[this.targ].Location).Length() > (float)this.randomInt)
						{
							if (this.me.Location.Y > c[this.targ].Location.Y - 100f)
							{
								this.me.KeyUp = true;
							}
							else if (this.me.Location.Y < c[this.targ].Location.Y - 300f)
							{
								this.me.KeyDown = true;
							}
							if (this.me.Location.X > c[this.targ].Location.X)
							{
								this.me.KeyLeft = true;
							}
							else if (this.me.Location.X < c[this.targ].Location.X)
							{
								this.me.KeyRight = true;
							}
						}
						else if ((this.me.Location - c[this.targ].Location).Length() < (float)(this.randomInt - 50))
						{
							if (this.me.Location.Y > c[this.targ].Location.Y + 400f)
							{
								this.me.KeyDown = true;
							}
							else if (this.me.Location.Y < c[this.targ].Location.Y + 200f)
							{
								this.me.KeyUp = true;
							}
							if (this.me.Location.X > c[this.targ].Location.X)
							{
								this.me.KeyRight = true;
							}
							else if (this.me.Location.X < c[this.targ].Location.X)
							{
								this.me.KeyLeft = true;
							}
						}
					}
					if (!this.FaceTarg(c) && this.me.renderable && Rand.GetRandomInt(0, 101 - this.me.Aggression) < 4)
					{
						if (this.me.Location.Y > c[this.targ].Location.Y - (float)this.me.Height * 1.5f && this.me.Location.Y < c[this.targ].Location.Y + (float)this.me.Height * 0.5f)
						{
							this.targVertDistance = true;
						}
						else
						{
							this.targVertDistance = false;
						}
						if (this.CanReachTarg(c, Game1.screenWidth / 2) && this.targVertDistance && this.me.renderable && !this.me.AnimName.StartsWith("flyattack") && this.CanAttack())
						{
							this.me.KeyAttack = true;
						}
					}
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.Avoid:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (!this.AvoidTarg(c, 1300f) || this.me.Location.X > Game1.map.rightEdge - 256f || this.me.Location.X < Game1.map.leftBlock + 256f || (Game1.map.leftBlock != 0f && (this.me.Location.X < Game1.map.leftBlock + 100f || this.me.Location.X > Game1.map.rightBlock - 100f)))
					{
						this.jobFrame = -1f;
						this.FaceToTarg(c);
					}
					this.JumpOverWall(c);
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.AttackUp:
				if (this.targ > -1 && c[this.targ].Exists == CharExists.Exists)
				{
					if (!this.ChaseTarg(c, this.me.DefaultWidth / 2))
					{
						this.me.SetAnim("attackup", 0, 2);
					}
				}
				else
				{
					this.targ = this.FindTarg(c);
				}
				break;
			case JobType.Idle:
			case JobType.Still:
			case JobType.StillCanTurn:
				break;
			}
		}

		public void PrepareStats(Character c, float multiplier)
		{
			c.Aggression += Game1.stats.gameDifficulty * (c.Aggression / 4);
			if (Game1.cManager.currentChallenge > -1)
			{
				c.Aggression *= 4;
				this.regionMultiplier = 3f;
			}
			else
			{
				c.Aggression = (int)((float)c.Aggression * multiplier);
				this.regionMultiplier = multiplier;
			}
			c.LifeBarPercent = (c.HP = (c.pHP = (c.MaxHP = (int)((float)c.MaxHP * this.regionMultiplier))));
			c.Strength = (int)((float)c.Strength * this.regionMultiplier);
			c.Aggression = (int)MathHelper.Clamp(c.Aggression, 1f, 100f);
		}

		public int GetTarget()
		{
			return this.targ;
		}

		public void SetTarget(int newTarg)
		{
			if (Game1.character[newTarg].Exists == CharExists.Exists)
			{
				this.targ = newTarg;
			}
		}

		protected int FindTarg(Character[] c)
		{
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < c.Length; i++)
			{
				if (i != this.me.ID && c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && c[i].Team != this.me.Team)
				{
					float num3 = (this.me.Location - c[i].Location).Length();
					if ((num == -1 || num3 < num2) && (num3 < 900f / Game1.worldScale || Game1.map.leftBlock != 0f))
					{
						num2 = num3;
						num = i;
					}
				}
			}
			return num;
		}

		private bool CanTurn()
		{
			if ((!this.me.AnimName.Contains("attack") || this.me.AnimFrame < 1) && !this.me.AnimName.StartsWith("hurt") && !this.me.AnimName.StartsWith("down") && this.me.AnimName != "runend")
			{
				return true;
			}
			return false;
		}

		protected bool ChaseTarg(Character[] c, float distance)
		{
			if (c[this.targ].Exists != CharExists.Exists || this.FriendInWay(c, this.me.ID, this.me.Face))
			{
				return false;
			}
			if (this.CanTurn() && (this.me.State == CharState.Air || this.me.WallInWay || this.CanJumpWall(this.me.JumpVelocity)))
			{
				if (this.me.FlyType == FlyingType.None)
				{
					if (this.me.Location.X > c[this.targ].Location.X + distance)
					{
						return this.me.KeyLeft = true;
					}
					if (this.me.Location.X < c[this.targ].Location.X - distance)
					{
						return this.me.KeyRight = true;
					}
				}
				else if ((this.me.Location - c[this.targ].Location).Length() > distance)
				{
					if (this.me.Location.X > c[this.targ].Location.X)
					{
						return this.me.KeyLeft = true;
					}
					if (this.me.Location.X < c[this.targ].Location.X)
					{
						return this.me.KeyRight = true;
					}
				}
			}
			return false;
		}

		protected bool CanReachTarg(Character[] c, float distance)
		{
			distance /= Game1.worldScale;
			if (this.me.Location.X < c[this.targ].Location.X + distance && this.me.Location.X > c[this.targ].Location.X - distance)
			{
				return true;
			}
			return false;
		}

		protected bool AvoidTarg(Character[] c, float distance)
		{
			if (c[this.targ].Exists != CharExists.Exists)
			{
				return false;
			}
			if (this.FriendInWay(c, this.me.ID, this.me.Face))
			{
				return false;
			}
			if (this.CanTurn() && (this.me.State == CharState.Air || this.CanJumpWall(this.me.JumpVelocity)))
			{
				if (this.me.Location.X < c[this.targ].Location.X && this.me.Location.X > c[this.targ].Location.X - distance)
				{
					return this.me.KeyLeft = true;
				}
				if (this.me.Location.X > c[this.targ].Location.X && this.me.Location.X < c[this.targ].Location.X + distance)
				{
					return this.me.KeyRight = true;
				}
			}
			return false;
		}

		protected bool FaceTarg(Character[] c)
		{
			if (this.targ < 0 || c[this.targ].Exists != CharExists.Exists)
			{
				return false;
			}
			if (this.CanTurn())
			{
				if (this.me.Location.X > c[this.targ].Location.X && this.me.Face == CharDir.Right)
				{
					this.me.Face = CharDir.Left;
					return true;
				}
				if (this.me.Location.X < c[this.targ].Location.X && this.me.Face == CharDir.Left)
				{
					this.me.Face = CharDir.Right;
					return true;
				}
			}
			return false;
		}

		public void FaceToTarg(Character[] c)
		{
			if (this.me.AnimName.StartsWith("hurt") || this.me.AnimName.Contains("attack") || this.me.DyingFrame > -1f || !this.CanTurn())
			{
				return;
			}
			if (this.me.Location.X > c[0].Location.X && this.me.Face == CharDir.Right)
			{
				if (this.me.FlyType > FlyingType.None)
				{
					this.me.ResetRotation();
				}
				this.me.Face = CharDir.Left;
			}
			else if (this.me.Location.X < c[0].Location.X && this.me.Face == CharDir.Left)
			{
				if (this.me.FlyType > FlyingType.None)
				{
					this.me.ResetRotation();
				}
				this.me.Face = CharDir.Right;
			}
		}

		private bool FriendInWay(Character[] c, int ID, CharDir face)
		{
			if (this.me.Ethereal == EtherealState.EtherealVulnerable)
			{
				return false;
			}
			for (int i = 0; i < c.Length; i++)
			{
				if (i == ID || c[i].Exists != CharExists.Exists || this.me.Team != c[i].Team || this.me.Team != Team.Enemy)
				{
					continue;
				}
				float num = (this.me.DefaultWidth + c[i].DefaultWidth) / 2;
				if (!(this.me.Location.Y > c[i].Location.Y - (float)this.me.Height) || !(this.me.Location.Y < c[i].Location.Y + (float)this.me.Height))
				{
					continue;
				}
				if (face == CharDir.Right)
				{
					if (c[i].Location.X > this.me.Location.X && c[i].Location.X < this.me.Location.X + num)
					{
						return true;
					}
				}
				else if (c[i].Location.X < this.me.Location.X && c[i].Location.X > this.me.Location.X - num)
				{
					return true;
				}
			}
			return false;
		}

		public bool CanAttack()
		{
			if (this.me.AnimName.StartsWith("attack") || this.me.AnimName.StartsWith("hurt") || this.me.DownTime > 0f || this.me.DyingFrame > -1f || this.me.LungeState != 0 || !this.CanAttackAI())
			{
				return false;
			}
			return true;
		}

		public bool CanJumpWall(float jVelocity)
		{
			int num = (int)((0f - jVelocity) / 4f);
			if (Game1.map.CheckCol(this.me.Location + new Vector2((this.me.Face == CharDir.Left) ? (-48) : 48, num - num % 64)) == 0)
			{
				return true;
			}
			return false;
		}

		private void JumpOverWall(Character[] c)
		{
			if (this.me.LiftType == CanLiftType.NoLift || this.me.AnimName.StartsWith("attack") || this.me.State != 0 || !this.me.WallInWay || this.me.NPC == NPCType.Friendly)
			{
				return;
			}
			if (this.CanJumpWall(this.me.JumpVelocity))
			{
				this.me.KeyJump = true;
			}
			else if (this.jobType == JobType.Avoid)
			{
				this.jobFrame = -1f;
				if (this.FaceTarg(c))
				{
					this.me.KeyLeft = (this.me.KeyRight = false);
					this.me.SetLunge(LungeStates.Lunging, Math.Min(this.me.Speed * 2f + 200f, 1400f), this.me.JumpVelocity / 2f);
				}
			}
			else
			{
				this.jobType = JobType.Idle;
			}
		}

		private void JumpToPlayer(Character[] c)
		{
			if (this.me.LiftType == CanLiftType.NoLift || this.me.AnimName.StartsWith("attack") || this.me.State != 0)
			{
				return;
			}
			if (this.me.Location.Y > c[this.targ].Location.Y + 200f)
			{
				if (Rand.GetRandomInt(0, 100 - this.me.Aggression) < 1 && this.me.JumpVelocity / 4f > this.me.Location.Y - c[this.targ].Location.Y && this.CanJumpWall(this.me.JumpVelocity))
				{
					this.me.KeyJump = true;
				}
			}
			else if (this.me.Location.Y < c[this.targ].Location.Y - 300f && Rand.GetRandomInt(0, 50) < 1 && this.me.ledgeAttach > -1 && this.me.CanFallThrough)
			{
				this.me.FallOff(c, fallThrough: true);
			}
		}

		public bool DodgeAttack(Character[] c, int targ, float validRange, float speed, float jumpVelocity, bool faceTarg, float aggressiveness)
		{
			if (this.me.State == CharState.Grounded && this.me.GrabbedBy == -1 && targ > -1 && ((c[targ].Face == CharDir.Right && this.me.Location.X > c[targ].Location.X) || (c[targ].Face == CharDir.Left && this.me.Location.X < c[targ].Location.X)) && c[targ].AnimName.Contains("attack") && (this.me.Location - c[targ].Location).Length() < validRange)
			{
				float randomFloat = Rand.GetRandomFloat(0f, 101f - aggressiveness);
				if (randomFloat < 10f)
				{
					if (faceTarg)
					{
						this.FaceTarg(c);
					}
					if (Rand.GetRandomInt(0, 2) == 0)
					{
						if (this.me.SetLunge(LungeStates.DodgeBackCounter, 0f - speed, jumpVelocity))
						{
							return true;
						}
					}
					else if (this.me.SetLunge(LungeStates.DodgeForwardCounter, speed, jumpVelocity))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool DodgeCounter(Character[] c)
		{
			if (this.me.GrabbedBy == -1 && (this.me.LungeState == LungeStates.DodgeBackCounter || this.me.LungeState == LungeStates.DodgeForwardCounter) && this.targ > -1)
			{
				this.FaceTarg(c);
				this.me.SetAnim("attackcounter", 0, 0);
				if (this.me.AnimName == "attackcounter")
				{
					return true;
				}
			}
			return false;
		}

		public void ResetJobFrame()
		{
			this.jobFrame = -1f;
		}

		public virtual void GetAmbientAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = this.me.Location;
			traj = this.me.Trajectory;
			_exists = this.me.Exists == CharExists.Exists;
		}

		public virtual bool PlaySound(string sound)
		{
			Sound.PlayCue(sound, this.me.Location, (Game1.character[0].Location - this.me.Location).Length() / 2f);
			return true;
		}

		public virtual bool TriggerAI(int trigger)
		{
			return true;
		}

		public virtual string CheckAnimName(string animName)
		{
			return animName;
		}

		public virtual bool CanAttackAI()
		{
			return true;
		}

		public virtual bool CheckHit(Particle p)
		{
			return true;
		}

		public virtual void Die(ParticleManager pMan, Texture2D texture)
		{
		}

		public virtual void JobTimer(Character[] c, int ID)
		{
			if (!this.overrideAI)
			{
				this.jobFrame -= Game1.FrameTime * 2f;
			}
		}

		public virtual void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
		}

		public virtual void MapCollision(Map map, Character[] c, Character me)
		{
			me.MapCollision(map, c);
		}

		public virtual void Draw(SpriteBatch spriteBatch, Texture2D[] particleTex, bool specialMode)
		{
			this.me.Draw(spriteBatch, particleTex, specialMode);
		}
	}
}

using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust
{
	internal class HitManager
	{
		private static object syncObject = new object();

		private static bool r;

		private static CharDir tFace;

		private static float hitValue;

		private static Rectangle nullRect;

		private static void UpdateStats(Character[] c, ParticleManager pMan, StatusEffects sType, int i, int pOwner, float finalHitValue, float recharge)
		{
			lock (HitManager.syncObject)
			{
				if (pOwner > 0 && i > 0)
				{
					finalHitValue = c[pOwner].Strength;
				}
				switch (Game1.stats.gameDifficulty)
				{
				case 0:
					finalHitValue = ((pOwner != 0) ? (finalHitValue * 0.5f) : (finalHitValue * 1.5f));
					break;
				case 2:
					finalHitValue = ((pOwner != 0) ? (finalHitValue * 2f) : MathHelper.Max(finalHitValue * 0.8f, 1f));
					break;
				case 3:
					finalHitValue = ((pOwner != 0) ? (finalHitValue * 4f) : MathHelper.Max(finalHitValue * 0.6f, 1f));
					break;
				}
				bool critical = false;
				if (!Game1.hud.inBoss && Rand.GetRandomInt(0, 100) < 5)
				{
					if (pOwner == 0)
					{
						finalHitValue *= 4f;
					}
					else if (c[i].HP > 1)
					{
						finalHitValue = MathHelper.Min(finalHitValue * 2f, c[i].HP - 1);
					}
					critical = true;
				}
				if (pOwner == 0)
				{
					if (Game1.stats.attackBonusTime > 0f)
					{
						finalHitValue *= 1.2f;
					}
				}
				else if (Game1.stats.defenseBonusTime > 0f)
				{
					finalHitValue /= 4f;
				}
				if (i == 0 && Game1.stats.gameDifficulty < 3 && (float)c[i].HP >= (float)c[i].MaxHP * 0.25f && finalHitValue > (float)c[i].HP)
				{
					finalHitValue = c[i].HP - 1;
				}
				if (c[i].DownTime > 0f)
				{
					finalHitValue *= 8f;
				}
				else if (c[i].Defense != 0 && c[i].Defense != DefenseStates.Uninterruptable)
				{
					finalHitValue = 1f;
				}
				StatusEffects statusEffects = HitManager.StatusHit(c, i, sType, finalHitValue);
				if (pOwner == 0)
				{
					Game1.stats.curCharge = MathHelper.Clamp(Game1.stats.curCharge + recharge, 0f, 100f);
					if (Game1.stats.playerLifeState == 0 && c[i].DyingFrame == -1f)
					{
						Game1.stats.comboMeter++;
						Game1.stats.damageMeter += (int)finalHitValue;
						Game1.hud.comboTextSize = 3.5f;
						if (Game1.stats.melodicHitTimer <= 0f && !Game1.events.anyEvent && Game1.events.currentEvent > -1)
						{
							Sound.PlayMelodicHit(c[i].HP > 0 && c[i].HP - (int)finalHitValue < 1, c[i].CanHurtProjectileFrame == 0.075f);
						}
					}
					Game1.stats.comboTimer = 2f;
					Game1.map.canLockEdge = true;
				}
				else if (i == 0)
				{
					if (Game1.invulnerable)
					{
						finalHitValue = 0f;
					}
					Game1.cManager.AddScore(-100, c[i].Location + new Vector2(0f, -260f));
					if (Game1.hud.helpState == 0)
					{
						Game1.events.InitEvent(25, isSideEvent: true);
					}
					if (Game1.stats.comboTimer > 0f)
					{
						Game1.stats.comboTimer = (Game1.stats.comboEnemies = (Game1.stats.damageMeter = (Game1.stats.comboMeter = 0)));
						Game1.stats.comboBreak = 2;
						if (Game1.stats.melodicHitCount > -1 && Game1.events.currentEvent > -1)
						{
							Sound.PlayCue("melodic_hit_fail");
						}
						Game1.stats.melodicHitTimer = (Game1.stats.melodicMidHitTimer = (Game1.stats.melodicHitCount = -1));
					}
				}
				c[i].pHP = c[i].HP;
				if (c[i].HP > 0 && finalHitValue != 0f && Game1.hud.hudDetails)
				{
					pMan.AddHP(c[i].Location - new Vector2(0f, 200f * Game1.worldScale), (int)(0f - finalHitValue), critical, (c[i].HP > 1) ? statusEffects : StatusEffects.Normal, 9);
				}
				c[i].HP -= (int)finalHitValue;
				if (c[i].HP < 1)
				{
					if (c[i].DyingFrame == -1f && c[i].ReturnExp == -1)
					{
						if (pOwner == 0)
						{
							c[i].ReturnExp = Game1.stats.CombatReward(c, pMan, i);
							if (c[0].AnimName == "crouchslide")
							{
								Game1.awardsManager.EarnAchievement(Achievement.DefeatEnemySlide, forceCheck: false);
							}
						}
						else if (i != 0)
						{
							c[i].ReturnExp = 0;
							Game1.stats.CombatReward(c, pMan, i);
						}
						else
						{
							c[i].ReturnExp = -1;
						}
					}
					if (i != 0)
					{
						if (c[i].LiftType == CanLiftType.Normal)
						{
							if (c[i].State == CharState.Grounded)
							{
								c[i].SetAnim("idle00", 0, 2);
								c[i].SetAnim("hurtup", 0, 0);
								c[i].SetJump(Rand.GetRandomInt(500, 1000), jumped: false);
								c[i].Slide(Rand.GetRandomInt(-1200, -600));
							}
							else if (c[i].GrabbedBy == -1)
							{
								c[i].SetAnim("die", 0, 2);
							}
						}
						else
						{
							c[i].SetAnim("idle00", 0, 2);
							c[i].SetAnim("die", 0, 2);
						}
						if (c[i].pHP > 0)
						{
							c[i].KillMe(instantly: false);
							Game1.cManager.AddScore(100, c[i].Location + new Vector2(0f, -260f));
						}
					}
				}
				if (c[i].DyingFrame > -1f && c[i].State == CharState.Air && (c[i].Ai == null || c[i].Ai.CheckHit(null)))
				{
					c[i].DyingFrame = 0f;
					c[i].Ethereal = EtherealState.Normal;
				}
			}
		}

		public static StatusEffects StatusHit(Character[] c, int i, StatusEffects status, float finalHitValue)
		{
			if (Game1.events.anyEvent)
			{
				return StatusEffects.Normal;
			}
			if (status != 0)
			{
				c[i].StatusEffect = status;
			}
			switch (status)
			{
			case StatusEffects.Poison:
				c[i].StatusTime = 9.99f;
				c[i].StatusStrength = (int)MathHelper.Clamp(finalHitValue / 5f, 1f, 1000f);
				Sound.PlayCue("status_poison");
				return StatusEffects.Poison;
			case StatusEffects.Burning:
				c[i].StatusTime = 7.99f;
				c[i].StatusStrength = (int)MathHelper.Clamp(finalHitValue / 5f, 1f, 1000f);
				return StatusEffects.Burning;
			case StatusEffects.Silent:
				c[i].StatusTime = 12.01f;
				c[i].StatusStrength = 0;
				Sound.PlayCue("status_silence");
				return StatusEffects.Silent;
			case StatusEffects.Blind:
				c[i].StatusTime = 12.01f;
				Sound.PlayCue("status_dark");
				return StatusEffects.Blind;
			default:
				return StatusEffects.Normal;
			}
		}

		public static bool CheckPickup(Vector2 pLoc, Character[] c)
		{
			int num = (c[0].AnimName.StartsWith("crouch") ? 100 : 260);
			if (pLoc.X > c[0].Location.X - 80f && pLoc.X < c[0].Location.X + 80f && pLoc.Y > c[0].Location.Y - (float)num && pLoc.Y < c[0].Location.Y)
			{
				return true;
			}
			return false;
		}

		public static void SetHurt(float xLoc, Character[] c, int i, bool up)
		{
			if (c[i].AnimName.StartsWith("crouch") && Game1.map.CheckCol(c[i].Location - new Vector2(0f, 96f)) > 0)
			{
				c[i].SetAnim("idle00", 0, 2);
				c[i].SetAnim("crouch", 0, 0);
				return;
			}
			if (c[i].LiftType != CanLiftType.Immovable)
			{
				if (c[i].Location.X > xLoc)
				{
					c[i].Face = CharDir.Left;
				}
				else if (c[i].Location.X < xLoc)
				{
					c[i].Face = CharDir.Right;
				}
			}
			c[i].LungeState = LungeStates.None;
			if (!up)
			{
				c[i].SetAnim("idle00", 0, 2);
				c[i].SetAnim("hurt0" + Rand.GetRandomInt(0, 2), 0, 0);
				return;
			}
			c[i].SetAnim("idle00", 0, 2);
			if (!c[i].AnimName.StartsWith("idle"))
			{
				c[i].SetAnim("fly", 0, 2);
			}
			c[i].SetAnim("hurtup", 0, 0);
		}

		private static bool SetDownHurt(Character[] c, int i)
		{
			if (c[i].DownTime > 0f && c[i].State == CharState.Grounded)
			{
				c[i].SetAnim("hurtdown00", 0, 1);
				c[i].SetAnim("hurtdown0" + Rand.GetRandomInt(0, 2), 0, 1);
				if (c[i].AnimName.Contains("down"))
				{
					return true;
				}
			}
			return false;
		}

		private static CharDir GetFaceFromTraj(Vector2 trajectory)
		{
			if (!(trajectory.X <= 0f))
			{
				return CharDir.Right;
			}
			return CharDir.Left;
		}

		private static void HitSlide(Particle p, Character[] c, int i)
		{
			if (c[i].AnimName.StartsWith("crouch") || c[i].LiftType == CanLiftType.Immovable)
			{
				return;
			}
			CharDir faceFromTraj = HitManager.GetFaceFromTraj(p.trajectory);
			if (i == 0 && c[i].State == CharState.Grounded && Rand.GetRandomInt(0, 10) == 0)
			{
				c[0].Face = ((faceFromTraj == CharDir.Left) ? CharDir.Right : CharDir.Left);
				c[0].SetAnim("hurtup", 0, 0);
				c[0].SetJump(600f, jumped: false);
				c[0].Ethereal = EtherealState.Ethereal;
				return;
			}
			if ((i != 0 && c[i].LiftType != CanLiftType.NoLift) || (i == 0 && c[i].State == CharState.Air))
			{
				c[i].Face = ((faceFromTraj == CharDir.Left) ? CharDir.Right : CharDir.Left);
			}
			float num = 1f;
			if (c[i].State == CharState.Air)
			{
				num = 2f;
			}
			switch (faceFromTraj)
			{
			case CharDir.Left:
				if (c[i].Face == CharDir.Left)
				{
					c[i].Slide((float)Rand.GetRandomInt(800, 1000) * num);
				}
				else
				{
					c[i].Slide((float)Rand.GetRandomInt(-500, -300) * num);
				}
				break;
			case CharDir.Right:
				if (c[i].Face == CharDir.Left)
				{
					c[i].Slide((float)Rand.GetRandomInt(-500, -300) * num);
				}
				else
				{
					c[i].Slide((float)Rand.GetRandomInt(800, 1000) * num);
				}
				break;
			}
		}

		private static bool CheckParry(Particle p, Character[] c, ParticleManager pMan, int i)
		{
			switch (c[i].Defense)
			{
			case DefenseStates.BlockingOnly:
				c[i].CanHurtFrame = 0.1f;
				if (p is Hit)
				{
					int num2 = 500;
					if (c[p.owner].Face == CharDir.Left)
					{
						num2 = 0;
					}
					for (int m = 0; m < 20; m++)
					{
						pMan.AddBounceSpark(p.location + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(num2 - 500, num2, -500f, 10f), 0.3f, 5);
					}
					Sound.PlayCue("slash_block", p.location, (c[0].Location - p.location).Length() / 4f);
				}
				return true;
			case DefenseStates.Parrying:
				if (p is Hit && (p.flag != 18 || c[p.owner].State == CharState.Air))
				{
					if (HitManager.tFace == c[i].Face)
					{
						return false;
					}
					c[p.owner].InitParry(pMan, parrySuccess: false);
					c[i].InitParry(pMan, parrySuccess: true);
					for (int j = 0; j < 30; j++)
					{
						pMan.AddBounceSpark(p.location + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
					}
					if (i == 0)
					{
						Vector2 vector = (c[i].Location - c[p.owner].Location) / 2f - new Vector2(0f, (c[i].DefaultHeight + c[p.owner].DefaultHeight) / 2);
						vector = (c[i].Location - new Vector2(0f, c[i].DefaultHeight / 2) + (c[p.owner].Location - new Vector2(0f, c[p.owner].DefaultHeight / 2))) / 2f;
						pMan.AddLenseFlare(vector, 0.4f, 0, 5);
						pMan.AddShockRing(vector, 0.5f, 5);
						c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = 0.5f);
						Game1.SlowTime = 0.7f;
						VibrationManager.SetBlast(0.6f, p.location);
						VibrationManager.SetScreenShake(0.25f);
						Game1.stats.curCharge = 100f;
						if ((float)c[i].HP < Math.Min((float)c[0].MaxHP * Game1.stats.bonusHealth, 9999f) * 0.1f)
						{
							Game1.awardsManager.EarnAchievement(Achievement.ParryLowHealth, forceCheck: false);
						}
					}
					float num = MathHelper.Clamp((800f - Math.Abs(c[p.owner].Location.X - c[i].Location.X)) * 4f, 800f, 1600f);
					c[p.owner].Slide(0f - num);
					bool flag = c[p.owner].State == CharState.Air;
					if (p.owner != 0)
					{
						if (c[p.owner].State == CharState.Grounded)
						{
							c[p.owner].SetAnim("godown", 0, 0);
						}
						else
						{
							c[p.owner].SetJump(800f, jumped: true);
							c[p.owner].SetAnim("hurtup", 0, 0);
						}
						bool flag2 = false;
						for (int k = 0; k < c[p.owner].Definition.Animations.Length; k++)
						{
							if (c[p.owner].Definition.Animations[k].name == "godown")
							{
								flag2 = true;
								break;
							}
						}
						if (c[p.owner].AnimName == "godown" || flag2)
						{
							c[p.owner].DownTime = c[p.owner].MaxDownTime;
							flag = true;
						}
						if (!flag && i == 0)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							c[i].Slide(-1000f);
							c[i].SetJump(1400f, jumped: false);
							HitManager.SetHurt(c[i].Location.X, c, p.owner, up: true);
							c[p.owner].Slide(-1000f);
							c[p.owner].SetJump(1400f, jumped: false);
							Game1.SlowTime = 1f;
							for (int l = 1; l < c.Length; l++)
							{
								if (l == p.owner || c[l].Exists != CharExists.Exists || c[l].Team != Team.Enemy || c[l].LiftType >= CanLiftType.Immovable || !((c[l].Location - c[0].Location).Length() < 2000f))
								{
									continue;
								}
								if (c[l].Defense != 0)
								{
									c[l].SetAnim("godown", 0, 0);
									if (c[l].AnimName == "godown")
									{
										c[l].DownTime = c[l].MaxDownTime;
									}
								}
								else
								{
									HitManager.SetHurt(c[i].Location.X, c, l, up: true);
									c[l].Slide(-1000f);
									c[l].SetJump(1400f, jumped: false);
								}
							}
						}
					}
				}
				return true;
			default:
				return false;
			}
		}

		public static bool CheckHitPossible(Particle p, Character[] c, int i, Rectangle hitRect)
		{
			int num = Math.Max(p.owner, 0);
			if (i != num && c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && c[i].Ethereal != EtherealState.Ethereal)
			{
				if (num == 0)
				{
					if ((p is Hit || c[i].Updatable(1800)) && (c[i].CanHurtFrame == 0f || c[i].CanHurtProjectileFrame == 0f || p.flag == 20) && (c[i].GrabbedBy < 0 || p.flag == 8) && HitManager.CheckIfRect(p, c, i, hitRect) && (c[i].Ai == null || c[i].Ai.CheckHit(p)))
					{
						return true;
					}
				}
				else
				{
					DamageTypes damageType = c[p.owner].DamageType;
					if ((damageType == DamageTypes.Everyone || (damageType == DamageTypes.NotSpecies && c[i].Definition.charType != c[num].Definition.charType) || (damageType == DamageTypes.NotTeam && c[i].Team != c[p.owner].Team)) && (c[i].CanHurtFrame == 0f || c[i].CanHurtProjectileFrame == 0f) && c[i].GrabbedBy < 0 && HitManager.CheckIfRect(p, c, i, hitRect) && (i != 0 || !Game1.invulnerable))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool CheckIfRect(Particle p, Character[] c, int i, Rectangle hitRect)
		{
			if (hitRect != HitManager.nullRect)
			{
				return c[i].InRectBounds(hitRect);
			}
			return c[i].InHitBounds(p.location);
		}

		private static void GetPStrength(Particle p, Character[] c)
		{
			if (p.owner <= 0)
			{
				if (p is Hit)
				{
					HitManager.hitValue = Game1.stats.attackEquip;
				}
				else if (p.owner == 0)
				{
					HitManager.hitValue = MathHelper.Max((float)p.strength * (float)Game1.stats.newFidgetEquip / 40f, 1f);
				}
				else
				{
					HitManager.hitValue = p.strength;
				}
				p.owner = 0;
			}
			else if (p.owner < c.Length)
			{
				int num = Game1.stats.defenseEquip / 4;
				HitManager.hitValue = MathHelper.Max(c[p.owner].Strength - num, (float)c[p.owner].Strength * 0.1f);
			}
			else
			{
				HitManager.hitValue = 0f;
			}
		}

		public static bool DamageRegion(Character[] c, int i, int strength, StatusEffects statusType, int owner, bool forceDamage)
		{
			if (i != owner && c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && c[i].Ethereal != EtherealState.Ethereal && (forceDamage || c[i].CanHurtFrame == 0f || c[i].CanHurtProjectileFrame == 0f) && c[i].GrabbedBy < 0)
			{
				if (owner == 0)
				{
					strength = Game1.stats.attackEquip;
				}
				else if (i == 0 && strength > 0)
				{
					int num = Game1.stats.defenseEquip / 2;
					strength = (int)MathHelper.Max(strength - num, 1f);
				}
				if (strength > 0)
				{
					c[i].Defense = DefenseStates.None;
					c[i].Face = ((c[i].Location.X < c[owner].Location.X) ? CharDir.Right : CharDir.Left);
					if (!HitManager.SetDownHurt(c, i) && c[i].GrabbedBy > -1)
					{
						c[i].SetJump(Rand.GetRandomInt(400, 800), jumped: false);
						c[i].Slide(400f);
						if (c[i].GrabbedBy == 0)
						{
							Game1.stats.inFront = true;
						}
						c[c[i].GrabbedBy].Ethereal = EtherealState.Ethereal;
						c[c[i].GrabbedBy].Holding = false;
						c[i].GrabbedBy = -1;
						HitManager.SetHurt(c[i].Location.X, c, i, up: true);
					}
					else if (c[i].Defense == DefenseStates.None)
					{
						c[i].SetJump(Rand.GetRandomInt(800, 2000), jumped: false);
						c[i].Slide(Rand.GetRandomInt(-2000, -800));
						HitManager.SetHurt(c[i].Location.X, c, i, up: true);
					}
					Game1.pManager.AddShockRing(c[i].Location, 0.5f, 5);
					VibrationManager.SetScreenShake(0.4f);
					VibrationManager.SetBlast(0.5f, c[i].Location);
					if (i != 0)
					{
						c[i].CanHurtFrame += 0.1f;
					}
					else
					{
						c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = 0.5f);
					}
					if (c[i].FlyType > FlyingType.None)
					{
						c[i].State = CharState.Air;
					}
				}
				HitManager.UpdateStats(c, Game1.pManager, statusType, i, -1, strength, 0f);
				return true;
			}
			return false;
		}

		public static bool CheckIDHit(Particle p, Character[] c, ParticleManager pMan, int i)
		{
			HitManager.r = false;
			HitManager.tFace = HitManager.GetFaceFromTraj(p.trajectory);
			HitManager.GetPStrength(p, c);
			if (i != p.owner && c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && c[i].Ethereal != EtherealState.Ethereal && (p is Hit || c[i].Updatable(1800)) && (c[i].CanHurtFrame == 0f || c[i].CanHurtProjectileFrame == 0f) && (c[i].Ai == null || c[i].Ai.CheckHit(p)) && c[i].GrabbedBy < 0)
			{
				if (p.owner > c.Length)
				{
					return true;
				}
				HitManager.CheckHit(p, c, pMan, i);
			}
			return HitManager.r;
		}

		public static bool CheckHit(Particle p, Character[] c, ParticleManager pMan)
		{
			HitManager.r = false;
			HitManager.tFace = HitManager.GetFaceFromTraj(p.trajectory);
			HitManager.GetPStrength(p, c);
			for (int i = 0; i < c.Length; i++)
			{
				if (HitManager.CheckHitPossible(p, c, i, HitManager.nullRect))
				{
					HitManager.CheckHit(p, c, pMan, i);
				}
			}
			return HitManager.r;
		}

		private static bool CheckHit(Particle p, Character[] c, ParticleManager pMan, int i)
		{
			if (p.owner < 0)
			{
				return HitManager.r;
			}
			float recharge = 0f;
			bool flag = Game1.stats.gameDifficulty < 2 || p.flag == 20 || c[i].Defense >= DefenseStates.Parrying || i == 0 || Rand.GetRandomInt(0, 16 - Game1.stats.gameDifficulty * 4) > 0;
			flag = c[i].Defense != DefenseStates.Uninterruptable && flag;
			if (HitManager.CheckParry(p, c, pMan, i))
			{
				HitManager.hitValue = 0f;
				if (p is Hit)
				{
					return HitManager.r;
				}
				HitManager.r = true;
				return HitManager.r;
			}
			if (p is Hit)
			{
				if (c[i].CanHurtFrame != 0f)
				{
					HitManager.hitValue = 0f;
					return HitManager.r;
				}
				if (p.owner != 0)
				{
					VibrationManager.SetScreenShake(0.25f);
					VibrationManager.SetBlast(0.5f, p.location);
					Game1.map.MapSegFrameSpeed = 0.4f;
				}
				switch (p.flag)
				{
				case 3:
					recharge = 6f;
					if (!HitManager.SetDownHurt(c, i) && c[i].LiftType == CanLiftType.Normal)
					{
						if (c[i].State == CharState.Grounded)
						{
							if (flag)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
							}
							HitManager.HitSlide(p, c, i);
						}
						else
						{
							if (flag)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							}
							if (c[i].FlyType == FlyingType.None || c[i].DyingFrame != -1f)
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
						}
					}
					else if (c[i].LiftType == CanLiftType.SmallLift)
					{
						if (c[i].State == CharState.Grounded)
						{
							if (Rand.GetRandomInt(0, 2) == 0)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
								HitManager.HitSlide(p, c, i);
							}
						}
						else
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
						}
					}
					else if (c[i].Defense == DefenseStates.None && Rand.GetRandomInt(0, 20) == 0)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
						HitManager.HitSlide(p, c, i);
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					break;
				case 4:
					recharge = 6f;
					if (c[p.owner].AnimName == "airspin" || c[p.owner].AnimName == "crouchslide")
					{
						HitManager.hitValue = MathHelper.Clamp(HitManager.hitValue / 8f, 1f, HitManager.hitValue);
					}
					if (c[i].LiftType == CanLiftType.Normal)
					{
						if (c[i].FlyType > FlyingType.None && c[i].DyingFrame == -1f)
						{
							if (flag)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
								c[i].Trajectory = new Vector2(c[i].Trajectory.X, c[i].Trajectory.Y / 4f);
							}
							pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
						}
						else if (c[i].State == CharState.Grounded)
						{
							if (flag)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
							pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
						}
						else
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							c[i].SetJump(600f, jumped: false);
							if (c[i].Location.Y > c[p.owner].Location.Y)
							{
								c[i].Location = new Vector2(c[i].Location.X, c[p.owner].Location.Y - 50f);
							}
							if (Rand.GetRandomInt(0, 2) == 0)
							{
								pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
							}
							else
							{
								pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
							}
						}
						if (c[p.owner].Ethereal == EtherealState.Immovable)
						{
							HitManager.HitSlide(p, c, i);
						}
					}
					else if (c[i].LiftType == CanLiftType.SmallLift)
					{
						if (flag && Rand.GetRandomInt(0, 2) == 0)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
						}
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
						}
						else
						{
							pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
						}
					}
					else if (!HitManager.SetDownHurt(c, i) && c[i].Defense == DefenseStates.None)
					{
						if (flag && Rand.GetRandomInt(0, 20) == 0)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
						}
						pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					}
					else
					{
						pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "up", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					}
					break;
				case 6:
					recharge = 12f;
					if (c[i].LiftType != CanLiftType.NoLift && c[i].Defense != DefenseStates.Uninterruptable)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
						c[i].Trajectory.X = 0f;
						if (i == 0)
						{
							c[i].Slide(-1000f);
						}
						else if (p.owner > 0)
						{
							c[i].Slide(-Rand.GetRandomInt(1000, 2000));
						}
						if (c[i].FlyType == FlyingType.None)
						{
							c[i].SetJump(Rand.GetRandomInt(1800, 2000), jumped: false);
						}
						else
						{
							c[i].SetJump(Rand.GetRandomInt(800, 1400), jumped: false);
						}
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1.2f, "uphigh", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					Game1.SlowTime = 0.05f;
					VibrationManager.SetBlast(0.5f, p.location);
					VibrationManager.Rumble(Game1.currentGamePad, 0.25f);
					break;
				case 5:
					recharge = 12f;
					if (!HitManager.SetDownHurt(c, i) && c[i].State == CharState.Air)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
						c[i].SetJump(-2500f, jumped: false);
					}
					else if (c[i].LiftType != CanLiftType.NoLift)
					{
						HitManager.hitValue *= 2f;
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
						c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
						c[i].Slide(Rand.GetRandomInt(-1200, -600));
					}
					else if (c[i].Defense == DefenseStates.None && Rand.GetRandomInt(0, 10) == 0)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "down", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					break;
				case 7:
					recharge = 12f;
					HitManager.hitValue *= 2f;
					if (c[i].LiftType == CanLiftType.Normal && c[i].GrabbedBy < 0 && c[i].Defense != DefenseStates.Uninterruptable)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
						c[i].Slide(-2000f);
						c[i].SetJump(600f, jumped: false);
					}
					else if (!HitManager.SetDownHurt(c, i) && c[i].Defense == DefenseStates.None && Rand.GetRandomInt(0, 2) == 0)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
						HitManager.HitSlide(p, c, i);
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1.2f, "KO", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					pMan.AddShockRing(c[i].Location, 0.5f, 5);
					VibrationManager.SetScreenShake(0.5f);
					VibrationManager.SetBlast(0.5f, p.location);
					Game1.map.MapSegFrameSpeed = 0.4f;
					break;
				case 20:
					recharge = 10f;
					if (!c[p.owner].Holding && c[i].LiftType == CanLiftType.Normal && (c[i].State == CharState.Air || c[i].Defense != DefenseStates.Uninterruptable))
					{
						c[i].SetAnim("biggrab", 0, 2);
						if (c[i].AnimName == "biggrab")
						{
							HitManager.hitValue = 0f;
							c[p.owner].Holding = true;
							c[p.owner].Ethereal = EtherealState.Ethereal;
							c[i].GrabbedBy = p.owner;
							c[i].Face = c[p.owner].Face;
							c[i].Ethereal = EtherealState.EtherealVulnerable;
							Sound.PlayCue("grabcontact");
							VibrationManager.Rumble(Game1.currentGamePad, 0.3f);
						}
						pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
						pMan.AddShockRing(c[i].Location, 0.5f, 5);
					}
					else
					{
						pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					}
					break;
				case 8:
					recharge = 24f;
					if (!HitManager.SetDownHurt(c, i) && c[i].GrabbedBy > -1)
					{
						HitManager.hitValue *= 4f;
						c[i].SetJump(Rand.GetRandomInt(400, 800), jumped: false);
						c[i].Slide(400f);
						if (c[i].GrabbedBy == 0)
						{
							Game1.stats.inFront = true;
						}
						c[c[i].GrabbedBy].Ethereal = EtherealState.Ethereal;
						c[c[i].GrabbedBy].Holding = false;
						c[i].GrabbedBy = -1;
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
					}
					else if (c[i].Defense == DefenseStates.None && c[i].LiftType != CanLiftType.NoLift)
					{
						c[i].SetJump(Rand.GetRandomInt(800, 2000), jumped: false);
						c[i].Slide(Rand.GetRandomInt(-2000, -800));
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "down", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					pMan.AddShockRing(c[i].Location, 0.5f, 5);
					VibrationManager.SetScreenShake(0.4f);
					VibrationManager.SetBlast(0.5f, p.location);
					break;
				case 18:
					if (c[p.owner].State == CharState.Grounded)
					{
						HitManager.hitValue /= 10f;
					}
					else
					{
						HitManager.hitValue /= 2f;
					}
					if (HitManager.hitValue < 1f)
					{
						HitManager.hitValue = 1f;
					}
					recharge = 0.2f;
					if (c[i].LiftType == CanLiftType.Normal)
					{
						if (flag)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							if (c[i].FlyType == FlyingType.None || c[i].DyingFrame != -1f)
							{
								c[i].SetJump(Rand.GetRandomInt(300, 600), jumped: false);
							}
							else
							{
								c[i].Trajectory = new Vector2(c[i].Trajectory.X, c[i].Trajectory.Y / 2f);
							}
						}
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							c[i].Slide(500f);
						}
						else
						{
							c[i].Slide(-500f);
						}
					}
					else if (c[i].LiftType == CanLiftType.SmallLift)
					{
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							if (Rand.GetRandomInt(0, 8) == 0)
							{
								c[i].Slide(400f);
							}
							else
							{
								c[i].Slide(-400f);
							}
						}
					}
					else if (c[i].Defense == DefenseStates.None && c[i].DownTime <= 0f && Rand.GetRandomInt(0, 20) == 0)
					{
						HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
					}
					pMan.MakeSlash(c[p.owner].Definition.charType, c[i].Definition.charType, p.location, 1f, "spin", c[i].RandomSkin, c[i].defaultColor, c[p.owner].Face);
					break;
				}
				if (i != 0)
				{
					c[i].CanHurtFrame += 0.1f;
				}
				else
				{
					c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = ((c[i].State == CharState.Grounded) ? 0.5f : 1.5f));
				}
				if (c[i].FlyType > FlyingType.None)
				{
					c[i].State = CharState.Air;
				}
			}
			else
			{
				if (c[i].CanHurtProjectileFrame != 0f)
				{
					HitManager.hitValue = 0f;
					return HitManager.r;
				}
				if (!HitManager.SetDownHurt(c, i) && c[i].GrabbedBy == -1)
				{
					if (c[i].LiftType == CanLiftType.Normal)
					{
						if (c[i].State == CharState.Grounded)
						{
							if (flag)
							{
								HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
								HitManager.HitSlide(p, c, i);
							}
							if (c[i].FlyType > FlyingType.None)
							{
								c[i].State = CharState.Air;
							}
						}
						else
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: true);
							if (c[i].FlyType == FlyingType.None || c[i].DyingFrame != -1f)
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
							else
							{
								c[i].SetJump(Rand.GetRandomInt(100, 200), jumped: false);
							}
						}
					}
					else if (c[i].Defense == DefenseStates.None)
					{
						if (Rand.GetRandomInt(0, 20) == 0)
						{
							HitManager.SetHurt(c[p.owner].Location.X, c, i, up: false);
						}
					}
					else
					{
						HitManager.hitValue = 1f;
					}
				}
				if (i != 0)
				{
					c[i].CanHurtProjectileFrame += 0.075f;
				}
				else
				{
					c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = 1f);
				}
				HitManager.r = true;
			}
			if (i > 0 && c[i].Ai != null && p.owner > -1 && p.owner < c.Length && c[i].Team != c[p.owner].Team)
			{
				c[i].Ai.SetTarget(p.owner);
			}
			HitManager.UpdateStats(c, pMan, p.statusType, i, p.owner, HitManager.hitValue, recharge);
			return HitManager.r;
		}

		private static bool CheckHazardPossible(Particle p, Character[] c, int i, bool canEvade)
		{
			if (c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && (c[i].Ethereal != EtherealState.Ethereal || (!canEvade && c[i].AnimName.StartsWith("evade")) || c[i].AnimName == "crouchslide") && c[i].CanHurtProjectileFrame == 0f && c[i].InHitBounds(p.location) && c[i].Updatable(1800) && c[i].GrabbedBy == -1 && (i != 0 || !Game1.invulnerable))
			{
				return true;
			}
			return false;
		}

		public static bool CheckHazard(Particle p, Character[] c, ParticleManager pMan, bool canEvade)
		{
			HitManager.r = false;
			for (int i = 0; i < c.Length; i++)
			{
				if (!HitManager.CheckHazardPossible(p, c, i, canEvade))
				{
					continue;
				}
				HitManager.hitValue = p.strength;
				if (c[i].Definition.charType == CharacterType.Dust)
				{
					int num = Game1.stats.defenseEquip / 2;
					if (HitManager.hitValue > (float)num)
					{
						HitManager.hitValue -= num;
					}
					else
					{
						HitManager.hitValue = 1f;
					}
					HitManager.SetHurt(p.location.X, c, i, up: true);
					c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = Math.Max(4 - Game1.stats.gameDifficulty, 2.5f));
				}
				else
				{
					if (c[i].LiftType == CanLiftType.Normal)
					{
						if (c[i].State == CharState.Grounded)
						{
							HitManager.SetHurt(p.location.X, c, i, up: true);
							if (c[i].AnimName.StartsWith("hurtup"))
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
							HitManager.HitSlide(p, c, i);
							if (c[i].FlyType > FlyingType.None)
							{
								c[i].State = CharState.Air;
							}
						}
						else
						{
							HitManager.SetHurt(p.location.X, c, i, up: true);
							if (c[i].FlyType == FlyingType.None)
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
							else
							{
								c[i].SetJump(Rand.GetRandomInt(100, 200), jumped: false);
							}
						}
					}
					else
					{
						HitManager.SetHurt(p.location.X, c, i, up: false);
					}
					c[i].CanHurtProjectileFrame = 1f;
				}
				HitManager.r = true;
				CharDir dir = ((c[i].Face == CharDir.Left) ? CharDir.Right : CharDir.Left);
				pMan.MakeSlash(CharacterType.Dust, c[i].Definition.charType, p.location, 1f, "forward", c[i].RandomSkin, c[i].defaultColor, dir);
				pMan.AddShockRing(p.location, 0.5f, 5);
				HitManager.UpdateStats(c, pMan, p.statusType, i, -1, HitManager.hitValue, 0f);
			}
			return HitManager.r;
		}

		public static bool CheckWallHazard(Character[] c, int i, ParticleManager pMan, int damagePercent, ColType type)
		{
			if (c[i].Exists == CharExists.Exists && c[i].NPC == NPCType.None && (c[i].Ethereal != EtherealState.Ethereal || c[i].AnimName.StartsWith("evade") || c[i].AnimName == "crouchslide") && c[i].CanHurtProjectileFrame == 0f && c[i].GrabbedBy == -1 && c[i].DyingFrame == -1f)
			{
				int num = ((i == 0) ? ((int)Math.Min((float)c[0].MaxHP * Game1.stats.bonusHealth, 9999f)) : c[i].MaxHP);
				HitManager.hitValue = (float)(int)MathHelper.Clamp((float)num * 0.15f, 30f, 200f) * ((float)damagePercent * 0.01f);
				if (c[i].Definition.charType == CharacterType.Dust)
				{
					int num2 = Game1.stats.defenseEquip / 10;
					HitManager.hitValue = Math.Max(HitManager.hitValue - (float)num2, 1f);
					HitManager.SetHurt(c[i].Location.X, c, i, up: true);
					c[i].CanHurtFrame = (c[i].CanHurtProjectileFrame = Math.Max(4 - Game1.stats.gameDifficulty, 2.5f));
					VibrationManager.SetScreenShake(0.5f);
					VibrationManager.SetBlast(1f, c[i].Location);
					Game1.map.MapSegFrameSpeed = 0.4f;
				}
				else
				{
					if (c[i].LiftType == CanLiftType.Normal)
					{
						if (c[i].State == CharState.Grounded)
						{
							HitManager.SetHurt(c[i].Location.X, c, i, up: true);
							if (c[i].AnimName.StartsWith("hurtup"))
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
								c[i].Slide(-1500f);
							}
							if (c[i].FlyType > FlyingType.None)
							{
								c[i].State = CharState.Air;
							}
						}
						else
						{
							HitManager.SetHurt(c[i].Location.X, c, i, up: true);
							if (c[i].FlyType == FlyingType.None || c[i].DyingFrame != -1f)
							{
								c[i].SetJump(Rand.GetRandomInt(800, 1200), jumped: false);
							}
							else
							{
								c[i].SetJump(Rand.GetRandomInt(100, 200), jumped: false);
							}
						}
					}
					else
					{
						HitManager.SetHurt(c[i].Location.X, c, i, up: false);
					}
					c[i].CanHurtFrame = 1f;
				}
				CharDir dir = ((c[i].Face == CharDir.Left) ? CharDir.Right : CharDir.Left);
				pMan.MakeSlash(CharacterType.Dust, c[i].Definition.charType, c[i].Location, 1f, "hazard", c[i].RandomSkin, c[i].defaultColor, dir);
				pMan.AddShockRing(c[i].Location, 0.5f, 5);
				StatusEffects sType = ((type == ColType.Lava) ? StatusEffects.Burning : StatusEffects.Normal);
				HitManager.UpdateStats(c, pMan, sType, i, -1, HitManager.hitValue, 0f);
				return true;
			}
			return false;
		}

		public static void CheckDestructionHit(Particle p, Character[] c, ParticleManager pMan, DestructableManager dMan)
		{
			for (int i = 0; i < dMan.destructWalls.Count; i++)
			{
				Vector2 location = dMan.destructWalls[i].location;
				if (!(dMan.destructWalls[i].hurtTime <= 0f) || !(p.location.X > location.X - 180f) || !(p.location.X < location.X + 180f) || !(p.location.Y > location.Y - 760f) || !(p.location.Y < location.Y + 100f))
				{
					continue;
				}
				HitManager.tFace = HitManager.GetFaceFromTraj(p.trajectory);
				if (dMan.destructWalls[i].bombType == 0)
				{
					HitManager.hitValue = 10f;
					CharacterType debrisType = (CharacterType)dMan.destructWalls[i].debrisType;
					switch (p.flag)
					{
					case 3:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "forward", 0, Color.White, c[p.owner].Face);
						break;
					case 4:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "up", 0, Color.White, c[p.owner].Face);
						break;
					case 6:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1.2f, "uphigh", 0, Color.White, c[p.owner].Face);
						Game1.SlowTime = 0.05f;
						VibrationManager.SetBlast(0.5f, p.location);
						break;
					case 5:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "down", 0, Color.White, c[p.owner].Face);
						break;
					case 7:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1.2f, "KO", 0, Color.White, c[p.owner].Face);
						pMan.AddShockRing(p.location, 0.5f, 5);
						VibrationManager.SetScreenShake(0.5f);
						VibrationManager.SetBlast(0.5f, p.location);
						Game1.map.MapSegFrameSpeed = 0.4f;
						break;
					case 8:
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "down", 0, Color.White, c[p.owner].Face);
						pMan.AddShockRing(p.location, 0.5f, 5);
						break;
					case 18:
						if (c[p.owner].State == CharState.Air)
						{
							HitManager.hitValue = 0f;
							break;
						}
						HitManager.hitValue /= 5f;
						pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "spin", 0, Color.White, c[p.owner].Face);
						break;
					}
					Game1.stats.curCharge += HitManager.hitValue;
					pMan.MakeSlash(CharacterType.Dust, debrisType, p.location, 1f, "forward", 0, Color.White, c[p.owner].Face);
					VibrationManager.Rumble(Game1.currentGamePad, 0.25f);
					dMan.destructWalls[i].HP -= HitManager.hitValue;
					dMan.destructWalls[i].shakeTime = 0.5f;
					dMan.destructWalls[i].hurtTime = 0.1f;
					if (dMan.destructWalls[i].HP <= 0f)
					{
						dMan.destructWalls[i].KillMe(pMan, c[p.owner].Face);
					}
				}
				else if (p.flag != 18)
				{
					pMan.AddBounceSpark(p.location + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
					dMan.destructWalls[i].hurtTime = 0.01f;
					dMan.destructWalls[i].shakeTime = 0.5f;
					VibrationManager.Rumble(Game1.currentGamePad, 0.25f);
					Game1.events.InitEvent(22, isSideEvent: true);
					pMan.AddShockRing(p.location, 0.5f, 5);
					VibrationManager.SetScreenShake(0.4f);
					VibrationManager.SetBlast(0.5f, p.location);
					if (Game1.map.CheckCol(c[p.owner].Location - new Vector2(0f, 96f)) > 0)
					{
						c[p.owner].SetAnim("crouch", 0, 0);
					}
					else if ((dMan.destructWalls[i].location.X < c[p.owner].Location.X && c[p.owner].Face == CharDir.Left) || (dMan.destructWalls[i].location.X > c[p.owner].Location.X && c[p.owner].Face == CharDir.Right))
					{
						c[p.owner].InitParry(pMan, parrySuccess: false);
					}
					for (int j = 0; j < 30; j++)
					{
						pMan.AddBounceSpark(p.location + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
					}
				}
			}
			for (int k = 0; k < dMan.destructLamps.Count; k++)
			{
				Vector2 location = Game1.dManager.destructLamps[k].GetLocation;
				if (dMan.destructLamps[k].Exists && p.location.X > location.X - 100f && p.location.X < location.X + 100f && p.location.Y > location.Y - 100f && p.location.Y < location.Y + 100f)
				{
					HitManager.tFace = HitManager.GetFaceFromTraj(p.trajectory);
					pMan.MakeSlash(CharacterType.Dust, CharacterType.Dust, p.location, 1f, "forward", 0, Color.White, c[p.owner].Face);
					dMan.destructLamps[k].KillMe(pMan, c[p.owner].Face);
					VibrationManager.Rumble(Game1.currentGamePad, 0.25f);
				}
			}
		}
	}
}

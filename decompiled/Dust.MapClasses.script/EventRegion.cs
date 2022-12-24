using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.NavClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses.script
{
	public class EventRegion
	{
		private Rectangle region;

		private int ID;

		private float timer;

		private int stage;

		private Vector2 triggerLoc;

		private float specialFloat;

		private bool specialBool;

		private CharDir playerFace;

		public Rectangle Region => this.region;

		public int EventRegionID => this.ID;

		public EventRegion(Rectangle _region, int id)
		{
			this.region = _region;
			this.ID = id;
			this.Reset();
		}

		public void Reset()
		{
			this.timer = 0f;
			this.stage = 0;
			this.specialFloat = 0f;
			this.specialBool = false;
		}

		public void Update(Character[] c, ParticleManager pMan, float frameTime)
		{
			bool flag = this.region.Contains((int)c[0].Location.X, (int)c[0].Location.Y);
			switch (this.ID)
			{
			case 0:
			{
				if (!flag)
				{
					break;
				}
				bool flag9 = false;
				for (int num29 = 0; num29 < Game1.dManager.destructWalls.Count; num29++)
				{
					if (Game1.dManager.destructWalls[num29].Exists)
					{
						flag9 = true;
						Vector2 location2 = Game1.dManager.destructWalls[num29].location;
						for (int num30 = 0; num30 < (int)((100f - Game1.dManager.destructWalls[num29].HP) / 25f); num30++)
						{
							pMan.AddWaterSplash(location2 + Rand.GetRandomVector2(-150f, 150f, -700f, -500f), Rand.GetRandomFloat(0.002f, 0.008f), 1, 6);
						}
					}
				}
				if (!flag9)
				{
					Game1.events.InitEvent(86, isSideEvent: false);
				}
				break;
			}
			case 1:
				if (flag)
				{
					if (c[0].AnimName.StartsWith("crouch"))
					{
						this.timer += frameTime;
					}
					else
					{
						this.timer = 0f;
					}
					if (this.timer > 0.8f)
					{
						VibrationManager.SetScreenShake(0.2f);
					}
					if (this.timer > 1f)
					{
						this.InitWarp(new Vector2(470f, 470f) * 2f, "grave16b", c, pMan);
						this.timer = 0f;
					}
				}
				break;
			case 2:
				if (flag)
				{
					if (Game1.dManager.movingPlatforms.Count == 0)
					{
						Game1.dManager.AddMovingPlatform(new Vector2(2800f, 1130f) * 2f, 720, 0, 20, 2);
					}
					else if (Game1.dManager.PlatformTrajectory(c[0], c[0].Location) != Vector2.Zero)
					{
						if (c[0].Location.X < 4000f)
						{
							VibrationManager.SetScreenShake(1000f / c[0].Location.X);
						}
						if (c[0].Location.X < 1000f)
						{
							this.InitWarp(new Vector2(960f, 1280f) * 2f, "smith03b", c, pMan);
							Game1.dManager.movingPlatforms.Clear();
						}
					}
				}
				else if (c[0].State == CharState.Grounded && c[0].Location.Y > (float)(this.region.Y + this.region.Height + 400) && Game1.dManager.movingPlatforms.Count > 0)
				{
					Game1.dManager.movingPlatforms.Clear();
				}
				break;
			case 3:
			{
				if (!flag)
				{
					break;
				}
				bool flag3 = false;
				for (int k = 0; k < Game1.dManager.destructWalls.Count; k++)
				{
					if (Game1.dManager.destructWalls[k].Exists)
					{
						flag3 = true;
						Vector2 location = Game1.dManager.destructWalls[k].location;
						for (int l = 0; l < (int)((100f - Game1.dManager.destructWalls[k].HP) / 50f); l++)
						{
							pMan.AddSpray(location + Rand.GetRandomVector2(-150f, 150f, -800f, -600f), Rand.GetRandomVector2(-100f, 100f, -100f, 800f), 1f, 1, 10, 5);
						}
						VibrationManager.SetScreenShake(Math.Min(10f / Game1.dManager.destructWalls[k].HP, 1f));
					}
				}
				if (!flag3)
				{
					switch (Game1.map.path)
					{
					case "snow08":
						Game1.events.InitEvent(50, isSideEvent: true);
						break;
					case "snow11":
						Game1.events.InitEvent(51, isSideEvent: true);
						break;
					case "snow13":
						Game1.events.InitEvent(52, isSideEvent: true);
						break;
					case "snow14":
						Game1.events.InitEvent(53, isSideEvent: true);
						break;
					}
				}
				this.timer -= frameTime;
				if (this.timer < 0f)
				{
					this.timer = Rand.GetRandomFloat(0f, 0.1f);
					pMan.AddSpray(Rand.GetRandomVector2(this.region.X, this.region.X + this.region.Width, this.region.Y - 500, this.region.Y + 100), Rand.GetRandomVector2(0f, 200f, -100f, 800f), Rand.GetRandomFloat(0.5f, 2f), 1, 10, 7);
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						VibrationManager.SetScreenShake(0.5f);
					}
				}
				break;
			}
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				this.timer -= frameTime * 4f;
				if (this.stage == 0)
				{
					if (flag)
					{
						switch (this.ID)
						{
						default:
							this.triggerLoc = new Vector2(2740f, 2020f);
							this.playerFace = CharDir.Left;
							break;
						case 5:
							this.triggerLoc = new Vector2(2740f, 2020f);
							this.playerFace = CharDir.Left;
							break;
						case 6:
							this.triggerLoc = new Vector2(1460f, 1280f);
							this.playerFace = CharDir.Left;
							break;
						case 7:
							this.triggerLoc = new Vector2(-300f, 420f);
							this.playerFace = CharDir.Left;
							break;
						case 8:
							this.triggerLoc = new Vector2(4230f, 1980f);
							this.playerFace = CharDir.Left;
							break;
						case 9:
							this.triggerLoc = new Vector2(2180f, 1050f);
							this.playerFace = CharDir.Left;
							break;
						case 10:
							this.triggerLoc = new Vector2(4060f, 1790f);
							this.playerFace = CharDir.Right;
							break;
						case 11:
							this.triggerLoc = new Vector2(4530f, 1670f);
							this.playerFace = CharDir.Right;
							break;
						}
						if ((c[0].Trajectory.X < 0f && this.playerFace == CharDir.Left) || (c[0].Trajectory.X > 0f && this.playerFace == CharDir.Right))
						{
							Sound.PlayCue("avalanche_start", this.triggerLoc * 2f, 400f);
							this.stage++;
							this.timer = 1f;
						}
					}
				}
				else if (this.stage == 1)
				{
					VibrationManager.SetScreenShake(1f);
					if (this.timer < 0f)
					{
						pMan.AddAvalanche(this.triggerLoc * 2f, (this.playerFace == CharDir.Left) ? CharDir.Right : CharDir.Left, 400, 24, 5);
						this.stage++;
						this.timer = 6f;
					}
				}
				else if (this.stage == 2 && this.timer < 0f)
				{
					this.stage = 0;
				}
				break;
			case 20:
				if (flag && Game1.stats.Equipment[330] > 0)
				{
					Game1.events.InitEvent(342, isSideEvent: false);
				}
				break;
			case 21:
				if (flag && Game1.stats.Equipment[331] > 0 && Game1.stats.Equipment[332] > 0 && Game1.stats.Equipment[333] > 0)
				{
					Game1.events.InitEvent(370, isSideEvent: false);
				}
				break;
			case 22:
				if (c[0].Location.X < 6000f)
				{
					for (int num31 = 0; num31 < 6; num31++)
					{
						pMan.AddVerticleBeam(new Vector2((float)this.region.X + Rand.GetRandomFloat(0f, this.region.Width), (float)this.region.Y + Rand.GetRandomFloat(0f, this.region.Height)) * 0.85f, Vector2.Zero, 1f, 0f, 0f, 0.5f, 40, 400, 1f, -1, 3);
						pMan.AddUpgradeBurn(new Vector2((float)this.region.X + Rand.GetRandomFloat(0f, this.region.Width), (float)this.region.Y + Rand.GetRandomFloat(0f, this.region.Height)) * 0.9f, 2f, 4);
					}
				}
				break;
			case 23:
				if (flag && Game1.stats.Equipment[306] > 0 && !Game1.events.anyEvent)
				{
					if (c[0].AnimName.StartsWith("crouch"))
					{
						this.timer += frameTime;
					}
					else
					{
						this.timer = 0f;
					}
					if (this.timer > 0.8f)
					{
						VibrationManager.SetScreenShake(0.5f);
					}
					if (this.timer > 1f)
					{
						Game1.events.InitEvent(78, isSideEvent: true);
						this.timer = 0f;
					}
				}
				break;
			case 24:
				if (Game1.map.GetTransVal() <= 0f && !Game1.events.anyEvent)
				{
					c[0].HP = c[0].MaxHP;
					c[0].Ethereal = EtherealState.Ethereal;
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						int num23 = 0;
						for (int num24 = 1; num24 < c.Length; num24++)
						{
							if (c[num24].Exists != 0)
							{
								num23++;
							}
						}
						if (num23 < 20)
						{
							Game1.events.SpawnCharacter(c[0].Location + new Vector2((Rand.GetRandomInt(0, 2) == 0) ? (-1000) : 1000, -200f), "enemy", CharacterType.DarkVillager, Team.Enemy, ground: true);
						}
						for (int num25 = 1; num25 < c.Length; num25++)
						{
							if (Math.Abs(c[num25].Location.X - c[0].Location.X) > 1500f)
							{
								c[num25].Exists = CharExists.Dead;
							}
						}
					}
					int num26 = 12;
					if (this.timer < (float)num26)
					{
						this.timer += frameTime * 4f;
						if (this.timer >= (float)num26)
						{
							Game1.events.screenFade = new Color(0f, 0f, 0f, 0f);
							Game1.events.fadeLength = (Game1.events.fadeTimer = -4f);
						}
					}
					if (this.timer >= (float)num26 && (Game1.events.screenFade.A >= 250 || this.timer > (float)(num26 + 6)))
					{
						Game1.events.InitEvent(-90, isSideEvent: false);
					}
				}
				else
				{
					this.timer = 0f;
				}
				break;
			case 25:
			{
				if (!flag)
				{
					break;
				}
				Game1.map.leftBlock = 5400f;
				Game1.map.rightBlock = 9700f;
				bool flag4 = false;
				for (int m = 0; m < Game1.dManager.destructWalls.Count; m++)
				{
					if (Game1.dManager.destructWalls[m].Exists)
					{
						flag4 = true;
					}
				}
				if (!flag4)
				{
					Game1.map.leftBlock = (Game1.map.rightBlock = 0f);
					Game1.events.InitEvent(580, isSideEvent: false);
				}
				break;
			}
			case 26:
			{
				if (Game1.events.currentEvent >= 240 || !(Game1.map.transInFrame > 0f))
				{
					break;
				}
				int num32 = Game1.map.GetMapLayer().Length;
				for (int num33 = 0; num33 < num32; num33++)
				{
					int num34 = Game1.map.GetMapLayer()[num33].mapSeg.Length;
					for (int num35 = 0; num35 < num34; num35++)
					{
						if (Game1.map.GetMapLayer()[num33].mapSeg[num35] != null && Game1.map.GetMapLayer()[num33].mapSeg[num35].SourceIndex == 5 && (Game1.map.GetMapLayer()[num33].mapSeg[num35].Index == 10 || Game1.map.GetMapLayer()[num33].mapSeg[num35].Index == 12 || Game1.map.GetMapLayer()[num33].mapSeg[num35].Index == 14))
						{
							Game1.map.GetMapLayer()[num33].mapSeg[num35].color.A = 0;
							Game1.map.GetMapLayer()[num33].mapSeg[num35].FlagEnabled = false;
						}
					}
				}
				for (int num36 = 0; num36 < Sound.ambientCue.Count; num36++)
				{
					if (Sound.ambientCue[num36].GetCueName() == "bubbling_water")
					{
						Sound.ambientCue.Remove(Sound.ambientCue[num36]);
					}
				}
				break;
			}
			case 27:
			{
				if (Game1.events.currentEvent <= 250 || !(Game1.map.GetTransVal() > 0f))
				{
					break;
				}
				int num4 = Game1.map.GetMapLayer()[5].mapSeg.Length;
				for (int n = 0; n < num4; n++)
				{
					if (Game1.map.GetMapLayer()[5].mapSeg[n] != null && Game1.map.GetMapLayer()[5].mapSeg[n].Index == 1)
					{
						Game1.map.GetMapLayer()[5].mapSeg[n].color.A = 0;
					}
				}
				break;
			}
			case 28:
				if (Game1.events.sideEventAvailable[62])
				{
					if (flag)
					{
						Game1.map.Counter = -4f;
						if (!Game1.events.sideEventAvailable[80] && !Game1.events.anyEvent)
						{
							this.timer += frameTime;
							if (this.timer > 5f)
							{
								this.timer = 0f;
								Game1.hud.InitHelp("machinehint", restart: false, -1);
							}
						}
					}
					if (flag && c[0].Face == CharDir.Left && c[0].Location.X < 6200f && pMan.CheckExistingParticle(new FidgetBolt(Vector2.Zero, CharDir.Left, 0, 0, 5)) != Vector2.Zero)
					{
						Vector2 vector = new Vector2(2620f, 360f) * 2f;
						VibrationManager.SetBlast(0.5f, vector);
						VibrationManager.SetScreenShake(2f);
						pMan.AddShockRing(vector, 1f, 5);
						for (int num9 = 0; num9 < 20; num9++)
						{
							pMan.AddElectricBolt(vector + Rand.GetRandomVector2(-300f, 100f, -100f, 100f), -1, Rand.GetRandomFloat(0.1f, 0.5f), 1000, 100, 1f, 5);
							pMan.AddBounceSpark(vector + Rand.GetRandomVector2(-300f, 100f, -100f, 100f), Rand.GetRandomVector2(-400f, 400f, -800f, 10f), 0.5f, 6);
						}
						Game1.events.InitEvent(62, isSideEvent: true);
					}
				}
				else
				{
					if (!flag)
					{
						break;
					}
					bool flag7 = true;
					for (int num10 = 0; num10 < Sound.ambientCue.Count; num10++)
					{
						if (Sound.ambientCue[num10].GetCueName() == "engine")
						{
							flag7 = false;
						}
					}
					if (flag7)
					{
						Sound.ambientCue.Add(new AmbientCue("engine", new Rectangle(5000, 100, 1000, 700), _stereo: true));
					}
					Vector2 vector2 = new Vector2(2620f, 360f) * 2f;
					VibrationManager.Rumble(Game1.currentGamePad, Math.Min(200f / (vector2 - c[0].Location).Length(), 0.3f));
					pMan.AddSmoke(vector2, Rand.GetRandomVector2(-60f, 60f, -300f, -100f), 0f, 0f, 0f, 0.1f, Rand.GetRandomFloat(0.5f, 1f), 3f, 6);
					pMan.AddElectricBolt(new Vector2(2700f, 355f) * 2f + Rand.GetRandomVector2(-300f, 100f, -100f, 100f), -1, Rand.GetRandomFloat(0.05f, 0.25f), 600, 30, 0.3f, 6);
				}
				break;
			case 29:
				if (Game1.events.sideEventAvailable[63])
				{
					if (Game1.map.GetTransVal() > 0f)
					{
						int num14 = Game1.map.GetMapLayer()[5].mapSeg.Length;
						for (int num15 = 0; num15 < num14; num15++)
						{
							if (Game1.map.GetMapLayer()[5].mapSeg[num15] != null && Game1.map.GetMapLayer()[5].mapSeg[num15].SourceIndex == 18 && Game1.map.GetMapLayer()[5].mapSeg[num15].Index == 83)
							{
								Game1.map.GetMapLayer()[5].mapSeg[num15].FlagEnabled = false;
							}
						}
					}
					if (!flag)
					{
						break;
					}
					if (!Game1.events.sideEventAvailable[81] && !Game1.events.anyEvent)
					{
						this.timer += frameTime;
						if (this.timer > 5f)
						{
							this.timer = 0f;
							Game1.hud.InitHelp("machinehint", restart: false, -1);
						}
					}
					if (this.specialFloat < 10f)
					{
						if (c[0].Location.X > 6600f)
						{
							for (int num16 = 0; num16 < 3; num16++)
							{
								Vector2 vector3;
								if ((vector3 = pMan.CheckExistingParticle(new FidgetPillar(Vector2.Zero, Vector2.Zero, 0, 0, 0, 5))) != Vector2.Zero)
								{
									Vector2 loc2 = Vector2.Zero;
									Vector2 traj = Vector2.Zero;
									int flag8 = 0;
									float floatVar = 0f;
									pMan.GetParticle((int)vector3.X, (int)vector3.Y, ref loc2, ref traj, ref flag8);
									pMan.GetParticleInfo((int)vector3.X, (int)vector3.Y, ref flag8, ref floatVar);
									if (loc2.X > 7440f && loc2.X < 8000f)
									{
										this.specialFloat += ((flag8 == 0) ? 1f : 0.1f);
										pMan.DirectParticle((int)vector3.X, (int)vector3.Y, 0);
									}
								}
							}
						}
						if (this.specialFloat > 0f)
						{
							this.specialFloat += frameTime * 0.2f;
							Vector2 vector4 = new Vector2(3800f, 600f) * 2f;
							for (int num17 = 0; (float)num17 < this.specialFloat / 2f; num17++)
							{
								pMan.AddFlamePuff(vector4 + Rand.GetRandomVector2(-100f, 100f, -1000f, 0f), new Vector2(0f, -200f), 1.5f, Rand.GetRandomInt(5, 7));
							}
							Sound.PlayCue("enemy_die_flame", vector4, (c[0].Location - vector4).Length() / 1.5f);
						}
						break;
					}
					int num18 = Game1.map.GetMapLayer()[5].mapSeg.Length;
					for (int num19 = 0; num19 < num18; num19++)
					{
						if (Game1.map.GetMapLayer()[5].mapSeg[num19] != null && Game1.map.GetMapLayer()[5].mapSeg[num19].SourceIndex == 18 && Game1.map.GetMapLayer()[5].mapSeg[num19].Index == 83)
						{
							Game1.map.GetMapLayer()[5].mapSeg[num19].FlagEnabled = true;
						}
					}
					Vector2 vector5 = new Vector2(3800f, 540f) * 2f;
					VibrationManager.SetBlast(0.5f, vector5);
					VibrationManager.SetScreenShake(2f);
					pMan.AddShockRing(vector5, 1f, 5);
					Game1.events.InitEvent(63, isSideEvent: true);
				}
				else
				{
					if (!flag)
					{
						break;
					}
					int num20 = Game1.map.GetMapLayer()[5].mapSeg.Length;
					for (int num21 = 0; num21 < num20; num21++)
					{
						if (Game1.map.GetMapLayer()[5].mapSeg[num21] != null && Game1.map.GetMapLayer()[5].mapSeg[num21].SourceIndex == 16 && Game1.map.GetMapLayer()[5].mapSeg[num21].Index == 54 && Game1.map.GetMapLayer()[5].mapSeg[num21].color.A > 0)
						{
							Vector2 vector6 = new Vector2(3800f, 600f) * 2f;
							for (int num22 = 0; num22 < 10; num22++)
							{
								pMan.AddFlamePuff(vector6 + Rand.GetRandomVector2(-100f, 100f, -1000f, 0f), new Vector2(0f, -200f), 1.5f, Rand.GetRandomInt(5, 7));
							}
							Sound.PlayCue("enemy_die_flame", vector6, (c[0].Location - vector6).Length() / 1.5f);
							Game1.map.GetMapLayer()[5].mapSeg[num21].color.A = (byte)Math.Max(Game1.map.GetMapLayer()[5].mapSeg[num21].color.A - 10, 0);
						}
					}
				}
				break;
			case 30:
			case 31:
			case 32:
			case 33:
			{
				if (Game1.events.sideEventAvailable[this.ID + 34])
				{
					bool flag10 = false;
					for (int num37 = 1; num37 < c.Length; num37++)
					{
						if (c[num37].Exists == CharExists.Exists && c[num37].Definition.charType == CharacterType.Cannon && this.region.Contains((int)c[num37].Location.X, (int)c[num37].Location.Y))
						{
							flag10 = true;
						}
					}
					if (flag10)
					{
						break;
					}
					Game1.events.sideEventAvailable[this.ID + 34] = false;
					bool flag11 = true;
					for (int num38 = 64; num38 < 68; num38++)
					{
						if (Game1.events.sideEventAvailable[num38])
						{
							flag11 = false;
						}
					}
					if (flag11)
					{
						Game1.awardsManager.EarnAchievement(Achievement.DefeatCannonAll, forceCheck: false);
					}
					break;
				}
				for (int num39 = 1; num39 < c.Length; num39++)
				{
					if (c[num39].Exists == CharExists.Exists && c[num39].Definition.charType == CharacterType.Cannon && this.region.Contains((int)c[num39].Location.X, (int)c[num39].Location.Y) && Game1.map.GetTransVal() <= 0f)
					{
						c[num39].SetAnim("die", 5, 0);
						c[num39].KillMe(instantly: false);
					}
				}
				break;
			}
			case 34:
			case 35:
			case 36:
			case 37:
			{
				if (!Game1.events.sideEventAvailable[this.ID + 30])
				{
					break;
				}
				float num13 = Game1.Scroll.Y / Game1.worldScale;
				if (!(num13 > (float)this.region.Y) || !(num13 < (float)(this.region.Y + this.region.Height)))
				{
					break;
				}
				if (flag)
				{
					if (Game1.halfSecFrame % 10 == 0)
					{
						pMan.AddCannonFireBall(new Vector2(c[0].Location.X + (float)Rand.GetRandomInt(400, 1200), Game1.Scroll.Y / Game1.worldScale), new Vector2(Rand.GetRandomFloat(-3200f, -2800f), 5000f), 1f, 200, -1, 5);
					}
				}
				else if (Game1.halfSecFrame % 10 == 0)
				{
					pMan.AddCannonFireBall(new Vector2(MathHelper.Clamp(c[0].Location.X, this.region.X, this.region.X + this.region.Width - 2000) + (float)Rand.GetRandomInt(400, 2000), Game1.Scroll.Y / Game1.worldScale), new Vector2(Rand.GetRandomFloat(-3200f, -2800f), 5000f), 1f, 50, -1, 5);
				}
				break;
			}
			case 38:
			{
				if (Game1.events.anyEvent)
				{
					break;
				}
				CharDir charDir = this.playerFace;
				this.playerFace = c[0].Face;
				bool flag2 = this.specialBool;
				this.specialBool = c[0].Trajectory.X != 0f;
				if (this.specialBool != flag2 || this.playerFace != charDir)
				{
					if (c[0].Trajectory.X != 0f || this.playerFace != charDir)
					{
						Sound.PlayCue("time_start");
						Sound.PlayPersistCue("time_persist", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
					}
					else
					{
						Sound.PlayCue("time_end");
						Sound.StopPersistCue("time_persist");
					}
				}
				if (c[0].Trajectory.X == 0f)
				{
					Sound.StopPersistCue("time_persist");
				}
				break;
			}
			case 39:
				Game1.renderMode = Game1.RenderMode.Panels;
				Game1.worldScale = 0.5f * Game1.hiDefScaleOffset;
				break;
			case 40:
				if (flag)
				{
					Game1.worldScale = (Game1.standardDef ? 0.8f : 1f) * Game1.hiDefScaleOffset;
					Game1.renderMode = Game1.RenderMode.Normal;
				}
				break;
			case 50:
			{
				this.timer -= frameTime;
				if (!(this.timer <= 0f))
				{
					break;
				}
				this.timer = Rand.GetRandomFloat(-0.1f, 0.4f);
				int num11 = 0;
				for (int num12 = 0; num12 < c.Length; num12++)
				{
					if (c[num12].Exists == CharExists.Exists && c[num12].Definition.charType == CharacterType.Moonblood)
					{
						num11++;
						if (num11 > 8)
						{
							break;
						}
					}
				}
				if (num11 < 9)
				{
					Vector2 loc = new Vector2(Game1.Scroll.X / Game1.worldScale - 50f, c[0].Location.Y - 600f);
					if (this.region.Contains((int)loc.X, (int)loc.Y))
					{
						Game1.events.SpawnCharacter(loc, "runner", CharacterType.Moonblood, Team.Enemy, ground: true);
					}
				}
				break;
			}
			case 51:
			{
				if (!flag)
				{
					break;
				}
				for (int num8 = 1; num8 < c.Length; num8++)
				{
					if (c[num8].Exists == CharExists.Exists && c[num8].Name == "runner" && c[num8].Trajectory.X == 0f && c[num8].State == CharState.Grounded && c[num8].Ai != null)
					{
						c[num8].Ai.TriggerAI(0);
					}
				}
				break;
			}
			case 52:
				if (!flag)
				{
					break;
				}
				if (c[0].Location.Y < 600f)
				{
					c[0].Location.X = 4200f;
					c[0].Trajectory.X = 0f;
					c[0].State = CharState.Air;
					c[0].SetAnim("hurtup", 16, 0);
					Game1.camera.ResetCamera(c);
				}
				else
				{
					if (!(c[0].Location.Y > 1400f))
					{
						break;
					}
					bool flag5 = true;
					for (int num5 = 1; num5 < 4; num5++)
					{
						if (c[num5].Exists == CharExists.Exists && c[num5].Definition.charType == CharacterType.Gaius)
						{
							flag5 = false;
						}
					}
					if (flag5)
					{
						_ = c[0].Location.X;
						Game1.events.SpawnCharacter(new Vector2(c[0].Location.X + 500f, 400f), "gaiusboss", CharacterType.Gaius, Team.Enemy, ground: false);
						Game1.events.GetCharacter(CharacterType.Gaius).SetAnim("hurtup", 0, 0);
						Game1.hud.InitBoss(Game1.character, "gaiusboss");
					}
				}
				break;
			case 53:
			{
				if (!c[0].AnimName.StartsWith("crawl") && c[0].State == CharState.Grounded)
				{
					c[0].SetAnim("crawlidle", 0, 0);
				}
				int num27 = Game1.map.GetMapLayer()[7].mapSeg.Length;
				for (int num28 = 0; num28 < num27; num28++)
				{
					if (Game1.map.GetMapLayer()[7].mapSeg[num28] != null)
					{
						Game1.map.GetMapLayer()[7].mapSeg[num28].Location.Y -= c[0].Trajectory.X * 0.02f;
						Game1.map.GetMapLayer()[7].mapSeg[num28].Location.X += c[0].Trajectory.X * 0.02f;
					}
				}
				this.timer -= frameTime;
				if (Game1.events.screenFade.A <= 100 && this.timer < 0f)
				{
					this.timer = Rand.GetRandomFloat(1f, 5f);
					Vector2 vector7 = new Vector2(1700f, 1080f) * 2f;
					Sound.PlayCue("gaius_cassius", vector7, (c[0].Location - vector7).Length() / 4f);
				}
				break;
			}
			case 54:
			{
				this.timer += frameTime;
				if (!(this.timer > 0.2f))
				{
					break;
				}
				this.timer = 0f;
				if (!(c[0].Location.X < 6880f) || !(c[0].Location.Y < 2200f) || !Game1.questManager.IfQuestCompleted(11))
				{
					break;
				}
				bool flag6 = true;
				for (int num6 = 1; num6 < c.Length; num6++)
				{
					if (c[num6].Exists == CharExists.Exists && c[num6].Definition.charType == CharacterType.GappySheep)
					{
						flag6 = false;
						break;
					}
				}
				if (flag6)
				{
					for (int num7 = 0; num7 < 6; num7++)
					{
						Game1.events.SpawnCharacter(new Vector2(Rand.GetRandomInt(this.region.X, this.region.X + this.region.Width), this.region.Y), "gappysheep", CharacterType.GappySheep, Team.Friendly, ground: true);
					}
				}
				break;
			}
			case 55:
				if (Game1.map.transInFrame > 0f)
				{
					this.specialBool = Game1.stats.CanHyperDub();
					if (!this.specialBool)
					{
						int num = Game1.map.GetMapLayer()[4].mapSeg.Length;
						for (int i = 0; i < num; i++)
						{
							if (Game1.map.GetMapLayer()[4].mapSeg[i] != null && Game1.map.GetMapLayer()[4].mapSeg[i].SourceIndex == 25 && Game1.map.GetMapLayer()[4].mapSeg[i].Index == 1)
							{
								Game1.map.GetMapLayer()[4].mapSeg[i].color.A = 0;
							}
						}
					}
					else
					{
						Music.Play("hyperdub");
						int num2 = 0;
						int num3 = 0;
						foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
						{
							for (int j = 0; j < item.Value.CageList.Count; j++)
							{
								num2++;
								if (item.Value.CageList[j].Stage > 0)
								{
									num3++;
								}
							}
						}
						if (num3 == num2)
						{
							Game1.awardsManager.EarnAchievement(Achievement.OpenCageAll, forceCheck: false);
						}
					}
				}
				if (this.specialBool)
				{
					int randomInt = Rand.GetRandomInt(4, 9);
					pMan.AddRaveBeam((c[0].Location + new Vector2(Rand.GetRandomInt(-1000, 1000), -2000f)) * this.LayerScale(randomInt), 2f, randomInt);
				}
				break;
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
				break;
			}
		}

		private void InitWarp(Vector2 loc, string destMap, Character[] c, ParticleManager pMan)
		{
			if (Game1.map.warpStage <= 0)
			{
				Game1.map.InitWarp(loc, destMap, c);
				Vector2 vector = c[0].Location + new Vector2(0f, -50f);
				VibrationManager.SetScreenShake(1f);
				Game1.map.MapSegFrameSpeed = 0.4f;
				Game1.pManager.AddShockRing(vector, 1f, 5);
				VibrationManager.SetBlast(1.5f, vector);
				for (int i = 0; i < 20; i++)
				{
					Vector2 loc2 = vector + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
					pMan.AddBounceSpark(loc2, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
				}
			}
		}

		public float LayerScale(int l)
		{
			switch (l)
			{
				case 0:
					return 0.2f;
				case 1:
					return 0.3f;
				case 2:
					return 0.5f;
				case 3:
					return 0.85f;
				case 4:
					return 0.9f;
				case 7:
					return 1.1f;
				case 8:
					return 1.25f;
				case 10:
					return 1.5f;
				case 11:
					return 1.75f;
			}
			return 1f;
		}
	}
}

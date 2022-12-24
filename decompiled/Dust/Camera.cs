using System;
using System.Collections.Generic;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust
{
	public class Camera
	{
		public Vector2 tempScroll;

		public Vector2 prevTempScroll;

		public float tempScale;

		public Vector2 camOffset = Vector2.Zero;

		private static Vector2 playerScroll;

		private static Vector2 followOffset = Vector2.Zero;

		private static Vector2 panOffset = Vector2.Zero;

		public float eventCamInfluence;

		private static float followSpeed;

		private static float cameraCatchup = 0f;

		private static float bottomOffset = 0.85f;

		public bool HudInWay;

		public bool NavInWay;

		private static bool wall;

		private static Vector2 viewPoint;

		private static Vector2 prevViewPoint;

		private static int viewPointLedge = 0;

		private static CamState viewPointState;

		private static CharDir prevCharFace;

		public float playerJumpPoint;

		private static float viewPointHorz;

		private static float finalViewPoint;

		private static float verticleLook;

		private static float camLeftBlock;

		private static float camRightBlock;

		private static float forceZoom;

		private static bool onMPlatform;

		private static Dictionary<Rectangle, Vector2> zoomRectList = new Dictionary<Rectangle, Vector2>();

		private static Dictionary<Rectangle, Vector3> panRectList = new Dictionary<Rectangle, Vector3>();

		public Dictionary<Rectangle, Vector2> GetZoomRectList
		{
			get
			{
				return Camera.zoomRectList;
			}
			set
			{
				Camera.zoomRectList = value;
			}
		}

		public Dictionary<Rectangle, Vector3> GetPanRectList
		{
			get
			{
				return Camera.panRectList;
			}
			set
			{
				Camera.panRectList = value;
			}
		}

		public void ResetCamera(Vector2 newLoc)
		{
			Camera.playerScroll = newLoc - new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.7f) / Game1.hiDefScaleOffset;
			this.camOffset = new Vector2(0f, Camera.bottomOffset);
			Camera.followOffset = Vector2.Zero;
			Camera.followSpeed = 10f;
			Camera.verticleLook = 0f;
			Camera.camLeftBlock = Game1.map.leftEdge;
			Camera.camRightBlock = Game1.map.rightEdge;
			this.tempScroll.X = newLoc.X - (float)(Game1.screenWidth / 2) / Game1.hiDefScaleOffset;
			this.tempScroll.Y = newLoc.Y - (float)Game1.screenHeight * Camera.bottomOffset / Game1.hiDefScaleOffset;
			Game1.Scroll = (Game1.pScroll = this.tempScroll);
		}

		public void ResetCamera(Character[] c)
		{
			this.ResetCamera(c[0].Location);
			c[0].PLoc = c[0].Location;
			this.ResetViewPoint(c, fallOff: false);
		}

		public void ResetViewPoint(Character[] c, bool fallOff)
		{
			if (fallOff)
			{
				Camera.viewPoint.Y = (Camera.prevViewPoint.Y = c[0].Location.Y + 10f);
			}
			else
			{
				Camera.viewPoint.Y = (Camera.prevViewPoint.Y = c[0].Location.Y - 220f);
			}
			if (Camera.viewPoint.X < Game1.map.leftEdge)
			{
				Camera.viewPoint.X = Game1.map.leftEdge;
			}
			if (Camera.viewPoint.X > Game1.map.rightEdge)
			{
				Camera.viewPoint.X = Game1.map.rightEdge;
			}
			Camera.viewPointState = CamState.Air;
			Camera.viewPointHorz = 0f;
			Camera.finalViewPoint = c[0].Location.Y;
		}

		public void ResetCameraFromEvent(Character[] c)
		{
			Vector2 vector = Vector2.Zero;
			if (Camera.panRectList.Count > 0 && !Game1.events.anyEvent)
			{
				foreach (KeyValuePair<Rectangle, Vector3> panRect in Camera.panRectList)
				{
					if (panRect.Key.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
					{
						int num = (int)panRect.Value.Y;
						float num2 = (panRect.Value.X + 6f) / 12f * 6.28f;
						vector = new Vector2((float)(0.0 - Math.Sin(num2)) * (float)num, (float)Math.Cos(num2) * (float)num);
					}
				}
			}
			Camera.panOffset = vector;
			this.eventCamInfluence = 0f;
			this.tempScroll = c[0].Location - new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.35f * Game1.hiDefScaleOffset) / Game1.worldScale - Camera.panOffset;
		}

		public void SetJumpPoint(Character c)
		{
			if (Camera.viewPointState == CamState.Air || (Camera.viewPointState == CamState.Grounded && Camera.viewPoint.Y - this.tempScroll.Y > (float)Game1.screenHeight) || c.Location.Y - this.tempScroll.Y < (float)Game1.screenHeight * 0.4f)
			{
				this.playerJumpPoint = c.Location.Y * Game1.worldScale / Game1.hiDefScaleOffset - this.tempScroll.Y;
			}
			else
			{
				this.playerJumpPoint = Camera.viewPoint.Y * Game1.worldScale / Game1.hiDefScaleOffset - this.tempScroll.Y;
			}
		}

		private bool CheckWallCol(Character[] c, Map map, Vector2 pLoc)
		{
			if (map.leftBlock != 0f && (Camera.viewPoint.X < map.leftBlock || Camera.viewPoint.X > map.rightBlock))
			{
				return true;
			}
			int num = map.CheckCol(new Vector2(Camera.viewPoint.X, Camera.viewPoint.Y - 15f));
			if (((num > 0 && num < 3) || Camera.viewPoint.X < map.leftEdge || Camera.viewPoint.X > map.rightEdge) && Game1.hud.runInTime < 0.1f)
			{
				return true;
			}
			return false;
		}

		private void UpdateViewPoint(Character[] c, Map map, float frameTime)
		{
			bool sideCollision = false;
			Camera.onMPlatform = Game1.dManager.CheckCol(c[0], 10, ref sideCollision) != -1f && !c[0].KeyLeft && !c[0].KeyRight;
			Camera.onMPlatform = Game1.dManager.PlatformTrajectory(c[0], c[0].Location) != Vector2.Zero;
			if (c[0].State == CharState.Air || map.GetTransVal() > 0f || Camera.onMPlatform)
			{
				Camera.viewPointState = CamState.Air;
				Camera.viewPoint = c[0].Location + new Vector2(0f, -32f);
				Camera.prevViewPoint = c[0].Location + new Vector2(0f, -200f);
				Camera.viewPointLedge = -1;
				Camera.viewPointHorz = 0f;
			}
			if (Camera.viewPointHorz < 400f)
			{
				Camera.viewPointHorz += frameTime * 1600f;
				if (Camera.viewPointHorz > 400f)
				{
					Camera.viewPointHorz = 400f;
				}
			}
			if (map.transInFrame > 0f && c[0].Trajectory.X == 0f)
			{
				while (!this.CheckWallCol(c, map, Camera.prevViewPoint) && Camera.viewPointHorz < 400f)
				{
					Camera.viewPointHorz += 32f;
					if (c[0].Face == CharDir.Left)
					{
						Camera.viewPoint.X = c[0].Location.X - Camera.viewPointHorz;
					}
					else
					{
						Camera.viewPoint.X = c[0].Location.X + Camera.viewPointHorz;
					}
					Camera.prevViewPoint.X = Camera.viewPoint.X;
				}
			}
			if (Camera.prevCharFace != c[0].Face)
			{
				Camera.viewPointState = CamState.Air;
				if (!c[0].AnimName.StartsWith("crouch"))
				{
					Camera.viewPoint = c[0].Location + new Vector2(0f, -100f);
				}
				else
				{
					Camera.viewPoint = c[0].Location + new Vector2(0f, -60f);
				}
				Camera.prevViewPoint = c[0].Location + new Vector2(0f, -200f);
				Camera.viewPointLedge = -1;
				Camera.viewPointHorz = 0f;
			}
			if (c[0].Face == CharDir.Left)
			{
				Camera.viewPoint.X = c[0].Location.X - Camera.viewPointHorz;
			}
			else
			{
				Camera.viewPoint.X = c[0].Location.X + Camera.viewPointHorz;
			}
			int num;
			if (c[0].State == CharState.Grounded && !Camera.wall)
			{
				while (Camera.viewPointState == CamState.Air)
				{
					num = map.CheckCol(Camera.viewPoint);
					Camera.viewPoint.Y += 32f;
					if (num > 0 && num < 3)
					{
						Camera.viewPoint.Y = (int)(Camera.viewPoint.Y / 64f) * 64;
						Camera.viewPointLedge = -1;
						Camera.viewPointState = CamState.Grounded;
					}
					for (int i = 0; i < map.ledges.Length; i++)
					{
						if ((i >= map.maxPlayerLedges && (map.ledges[i] == null || map.ledges[i].Flag != LedgeFlags.CameraPath)) || !map.GetLedgeMinMax(i, Camera.viewPoint.X))
						{
							continue;
						}
						int ledgeSec = map.GetLedgeSec(i, Camera.prevViewPoint.X);
						int ledgeSec2 = map.GetLedgeSec(i, Camera.viewPoint.X);
						if (ledgeSec > -1 && ledgeSec2 > -1)
						{
							float ledgeYLoc = map.GetLedgeYLoc(i, ledgeSec, Camera.prevViewPoint.X);
							float ledgeYLoc2 = map.GetLedgeYLoc(i, ledgeSec2, Camera.viewPoint.X);
							if (Camera.prevViewPoint.Y <= ledgeYLoc + 30f && Camera.viewPoint.Y >= ledgeYLoc2)
							{
								Camera.viewPoint.Y = ledgeYLoc2;
								Camera.viewPointLedge = i;
								Camera.viewPointState = CamState.Grounded;
								break;
							}
						}
					}
					if (Camera.viewPoint.Y > c[0].Location.Y + 600f)
					{
						Camera.viewPointState = CamState.Grounded;
						Camera.viewPoint.Y = c[0].Location.Y + 600f;
						break;
					}
				}
			}
			num = map.CheckCol(Camera.viewPoint);
			if (Camera.viewPointState == CamState.Grounded)
			{
				if (Camera.viewPointLedge != -1)
				{
					if (num == 0)
					{
						int ledgeSec3 = map.GetLedgeSec(Camera.viewPointLedge, Camera.viewPoint.X);
						if (ledgeSec3 == -1)
						{
							Camera.viewPointState = CamState.Air;
						}
						else
						{
							Camera.viewPoint.Y = map.GetLedgeYLoc(Camera.viewPointLedge, ledgeSec3, Camera.viewPoint.X);
						}
					}
					else
					{
						Camera.viewPoint.Y = (int)(Camera.viewPoint.Y / 64f) * 64;
						Camera.viewPointLedge = -1;
					}
				}
				else
				{
					for (int j = 0; j < map.ledges.Length; j++)
					{
						if ((j >= map.maxPlayerLedges && (map.ledges[j] == null || map.ledges[j].Flag != LedgeFlags.CameraPath)) || !map.GetLedgeMinMax(j, Camera.viewPoint.X))
						{
							continue;
						}
						int ledgeSec4 = map.GetLedgeSec(j, Camera.prevViewPoint.X);
						int ledgeSec5 = map.GetLedgeSec(j, Camera.viewPoint.X);
						if (ledgeSec4 > -1 && ledgeSec5 > -1)
						{
							float ledgeYLoc3 = map.GetLedgeYLoc(j, ledgeSec4, Camera.prevViewPoint.X);
							float ledgeYLoc4 = map.GetLedgeYLoc(j, ledgeSec5, Camera.viewPoint.X);
							if (Camera.prevViewPoint.Y <= ledgeYLoc3 + 20f && Camera.viewPoint.Y >= ledgeYLoc4 - 2f)
							{
								Camera.viewPoint.Y = ledgeYLoc4;
								Camera.viewPointLedge = j;
								break;
							}
						}
					}
					if (num == 0 && Camera.viewPointLedge == -1)
					{
						Camera.viewPointState = CamState.Air;
					}
				}
				if (Camera.viewPoint.Y > c[0].Location.Y + 600f)
				{
					Camera.viewPoint.Y = c[0].Location.Y + 600f;
				}
				else if (Camera.viewPoint.Y < c[0].Location.Y - 768f)
				{
					Camera.viewPoint.Y = c[0].Location.Y - 100f;
					Camera.viewPointState = CamState.Air;
				}
			}
			Camera.wall = false;
			while (this.CheckWallCol(c, map, Camera.prevViewPoint))
			{
				Camera.viewPoint.X = Camera.prevViewPoint.X;
				Camera.viewPointHorz = Math.Abs(Camera.viewPoint.X - c[0].Location.X);
				Camera.viewPointState = CamState.Air;
				Camera.wall = true;
				if (!(Camera.viewPoint.Y > c[0].Location.Y - 768f) || this.CheckWallCol(c, map, Camera.viewPoint))
				{
					break;
				}
				Camera.viewPoint.Y -= 128f;
			}
			Camera.prevViewPoint = Camera.viewPoint;
			Camera.prevCharFace = c[0].Face;
			if (c[0].IsFalling && c[0].Trajectory.Y < 1200f)
			{
				Camera.finalViewPoint += ((Camera.viewPoint.Y + c[0].Location.Y) / 2f - Camera.finalViewPoint) * frameTime * 1f;
				return;
			}
			if (c[0].State == CharState.Air || Game1.hud.saveable || Camera.onMPlatform)
			{
				Camera.finalViewPoint = c[0].Location.Y;
				return;
			}
			if (Camera.wall)
			{
				Camera.finalViewPoint += (c[0].Location.Y - Camera.finalViewPoint) * frameTime * 2f;
				return;
			}
			float num2 = Camera.viewPoint.Y;
			if (num2 < c[0].Location.Y - 100f)
			{
				num2 = c[0].Location.Y - 100f;
			}
			else if (num2 > c[0].Location.Y + 300f)
			{
				num2 = c[0].Location.Y + 200f;
			}
			Camera.finalViewPoint = GlobalFunctions.EaseInAndOut(Camera.finalViewPoint, num2, 4f + Math.Abs(c[0].Trajectory.X / 200f), frameTime);
			if (map.transInFrame > 0f)
			{
				Camera.finalViewPoint = num2;
			}
		}

		private bool UpdateRegionIntro(float frameTime)
		{
			if (Game1.events.regionIntroStage == 0)
			{
				return false;
			}
			Game1.pScroll = Game1.Scroll;
			float num = 4f;
			if (Game1.events.regionIntroStage == 1)
			{
				Game1.Scroll = Game1.events.regionIntroTargets[0] * (Game1.events.regionIntroTimer / num) + Game1.events.regionIntroTargets[1] * (1f - Game1.events.regionIntroTimer / num);
				Game1.worldScale = Game1.events.regionIntroZooms[0] * (Game1.events.regionIntroTimer / num) + Game1.events.regionIntroZooms[1] * (1f - Game1.events.regionIntroTimer / num);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				if (Game1.events.regionIntroTimer < 0f)
				{
					Game1.events.regionIntroTimer = num;
					Game1.events.regionIntroStage++;
				}
			}
			else if (Game1.events.regionIntroStage == 2)
			{
				Game1.Scroll = Game1.events.regionIntroTargets[2] * (Game1.events.regionIntroTimer / num) + Game1.events.regionIntroTargets[3] * (1f - Game1.events.regionIntroTimer / num);
				Game1.worldScale = Game1.events.regionIntroZooms[2] * (Game1.events.regionIntroTimer / num) + Game1.events.regionIntroZooms[3] * (1f - Game1.events.regionIntroTimer / num);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				if (Game1.events.regionIntroTimer < 0f)
				{
					Game1.hud.regionIntroState = 1;
					Game1.hud.regionIntroTime = 0.2f;
					Game1.events.regionIntroTimer = 6f;
					Game1.events.regionIntroStage++;
				}
			}
			else
			{
				if (Game1.events.regionIntroStage != 3)
				{
					Game1.worldScale = 0.8f * Game1.hiDefScaleOffset;
					Game1.map.transInFrame = 1f;
					Game1.events.regionIntroFade = 0f;
					Game1.events.regionIntroStage = 0;
					this.tempScroll = Game1.character[0].Location - new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.7f);
					Game1.Scroll = (Game1.pScroll = this.tempScroll * Game1.worldScale - new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * (1f - Game1.worldScale) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset);
					Game1.pManager.ResetWeather();
					Game1.events.regionIntroFade = 1f;
					return true;
				}
				num = 6f;
				Game1.Scroll = Game1.events.regionIntroTargets[4];
				Game1.worldScale = Game1.events.regionIntroZooms[4] * (Game1.events.regionIntroTimer / num) + Game1.events.regionIntroZooms[5] * (1f - Game1.events.regionIntroTimer / num);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				if (Game1.events.regionIntroTimer < 0f)
				{
					Game1.events.regionIntroStage++;
				}
			}
			Game1.events.regionIntroFade = 1f - (float)Math.Sin(6.28 * (double)(Game1.events.regionIntroTimer / 2f / num));
			Game1.Scroll *= Game1.worldScale;
			Game1.Scroll -= new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f;
			if (Game1.events.regionIntroFade > 0.9f)
			{
				Game1.pManager.ResetWeather();
			}
			if (Game1.map.GetTransVal() <= 0f)
			{
				Game1.events.regionIntroTimer -= frameTime;
			}
			return true;
		}

		private void UpdateScale(int stage, Character[] c, float frameTime, bool forceFollow, ref int followCount, ref Vector2 playerScrollTemp)
		{
			if (stage == 1)
			{
				Camera.forceZoom = 0f;
				if (Camera.zoomRectList.Count > 0)
				{
					foreach (KeyValuePair<Rectangle, Vector2> zoomRect in Camera.zoomRectList)
					{
						if (zoomRect.Key.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
						{
							this.tempScale = zoomRect.Value.X / 100f;
							Camera.forceZoom = zoomRect.Value.Y / 100f;
						}
					}
				}
				if (Game1.events.eventCamera != Vector2.Zero || this.UpdateFollowScale(c, frameTime, forceFollow, ref followCount, ref playerScrollTemp))
				{
					return;
				}
				if (c[0].State == CharState.Air)
				{
					this.tempScale = GlobalFunctions.EaseInAndOut(this.tempScale, 0.75f, 1f, frameTime);
				}
				else if (Math.Abs(c[0].Location.Y - c[0].PLoc.Y) > 3.5f)
				{
					this.tempScale = Math.Max(this.tempScale - frameTime / 2f, 0.8f);
				}
				else if (!Camera.wall)
				{
					this.tempScale = 1f - MathHelper.Clamp(Math.Abs((Camera.viewPoint.Y - c[0].Location.Y) / 1200f), 0f, 0.35f);
					if (c[0].CanFallThrough && c[0].ledgeAttach > -1 && this.tempScale > 0.8f)
					{
						this.tempScale = 0.8f;
					}
				}
				return;
			}
			if (Camera.forceZoom == 0f)
			{
				if (Game1.standardDef)
				{
					this.tempScale = Math.Max(this.tempScale * 0.8f, 0.6f);
				}
				else
				{
					this.tempScale = Math.Min(this.tempScale, 0.9f);
				}
			}
			if (Camera.forceZoom > 0f)
			{
				if (Game1.map.transInFrame > 1f)
				{
					Game1.worldScale = this.tempScale * Game1.hiDefScaleOffset;
				}
				else
				{
					Game1.worldScale += (this.tempScale * Game1.hiDefScaleOffset - Game1.worldScale) * frameTime * Camera.forceZoom;
				}
			}
			else if (Game1.worldScale > this.tempScale * Game1.hiDefScaleOffset)
			{
				Game1.worldScale += (this.tempScale * Game1.hiDefScaleOffset - Game1.worldScale) * frameTime;
			}
			else if (Math.Abs(Camera.viewPointHorz) > 300f || Camera.wall)
			{
				Game1.worldScale += (this.tempScale * Game1.hiDefScaleOffset - Game1.worldScale) * frameTime * 0.3f;
			}
		}

		private bool UpdateFollowScale(Character[] c, float frameTime, bool forceFollow, ref int followCount, ref Vector2 playerScrollTemp)
		{
			bool flag = false;
			float num = 0f;
			if (!Game1.events.anyEvent)
			{
				for (int i = 1; i < c.Length; i++)
				{
					if (c[i].Exists == CharExists.Exists && c[i].Team == Team.Enemy && (c[i].renderable || (c[i].AnimName.StartsWith("hurt") && c[i].Location.Y < c[0].Location.Y - 200f && c[i].Location.X > this.tempScroll.X - 200f && c[i].Location.X < this.tempScroll.X + (float)Game1.screenWidth + 200f) || (forceFollow && (!Game1.hud.inBoss || c[i].Name == Game1.hud.bossName) && c[i].Updatable(10000))) && c[i].Location.Y < c[0].Location.Y + 1000f / Game1.worldScale / Game1.hiDefScaleOffset)
					{
						float num2;
						if (c[i].FlyType > FlyingType.None)
						{
							flag = true;
							num2 = (c[0].Location - (c[i].Location - new Vector2(0f, c[i].DefaultHeight * 4))).Length() * (float)((!forceFollow) ? 1 : 2);
							num2 += 400f;
						}
						else
						{
							num2 = (c[0].Location - (c[i].Location - new Vector2(0f, c[i].DefaultHeight * 2))).Length() * (float)((!forceFollow) ? 1 : 2);
						}
						if (num2 > num)
						{
							num = num2;
						}
						playerScrollTemp += c[i].Location;
						followCount++;
					}
				}
				for (int j = 0; j < Game1.map.bombRegions.Count; j++)
				{
					if (Game1.map.bombRegions[j] == null)
					{
						continue;
					}
					Vector2 loc = Vector2.Zero;
					Vector2 traj = Vector2.Zero;
					int flag2 = 0;
					Game1.pManager.GetParticle(5, Game1.map.bombRegions[j].BombID, ref loc, ref traj, ref flag2);
					float num3 = ((!(traj == Vector2.Zero)) ? (1200f / this.tempScale) : (600f / this.tempScale));
					if (loc.X > c[0].Location.X - num3 && loc.X < c[0].Location.X + num3 && loc.Y > c[0].Location.Y - num3 && loc.Y < c[0].Location.Y + num3 / 2f)
					{
						float num4 = (c[0].Location - new Vector2(loc.X, loc.Y)).Length() * 2f;
						if (num4 > num)
						{
							num = num4;
						}
						playerScrollTemp += new Vector2(loc.X, loc.Y);
						followCount++;
					}
				}
			}
			if (Camera.forceZoom > 0f)
			{
				return true;
			}
			if (num > 0f)
			{
				float num5 = 6000f;
				if (flag || Game1.hud.inBoss)
				{
					num5 = 2000f;
				}
				MathHelper.Clamp(Math.Abs((Camera.viewPoint.Y - c[0].Location.Y) / 1600f), 0f, 0.35f);
				float target = MathHelper.Clamp(1f - num / num5, 0.65f, 1f);
				this.tempScale = GlobalFunctions.EaseInAndOut(this.tempScale, target, 8f, frameTime);
				return true;
			}
			return false;
		}

		public void Update(Character[] c, Map map, float frameTime, bool updateViewPoint)
		{
			if (this.UpdateRegionIntro(frameTime))
			{
				return;
			}
			bool forceFollow = Game1.hud.inBoss || Game1.map.leftBlock != 0f;
			int followCount = 1;
			Vector2 playerScrollTemp = c[0].Location;
			this.prevTempScroll = this.tempScroll;
			this.UpdateScale(1, c, frameTime, forceFollow, ref followCount, ref playerScrollTemp);
			if (updateViewPoint)
			{
				this.UpdateViewPoint(c, map, frameTime);
			}
			if (c[0].KeyDown)
			{
				Camera.bottomOffset = 0.8f;
			}
			else
			{
				Camera.bottomOffset = 0.85f;
			}
			if (!c[0].AnimName.StartsWith("hurt"))
			{
				if (c[0].Face == CharDir.Left)
				{
					if (c[0].Trajectory.X < 0f || Game1.stats.isSpinning)
					{
						if (this.camOffset.X < (float)Game1.screenWidth * 0.2f)
						{
							this.camOffset.X += frameTime * 1000f;
						}
					}
					else if (c[0].Trajectory.X > 0f && c[0].AnimName.StartsWith("attack") && 0f - this.camOffset.X < (float)Game1.screenWidth * 0.2f)
					{
						this.camOffset.X -= frameTime * 500f;
					}
				}
				else if (c[0].Face == CharDir.Right)
				{
					if (c[0].Trajectory.X > 0f || Game1.stats.isSpinning)
					{
						if (0f - this.camOffset.X < (float)Game1.screenWidth * 0.2f)
						{
							this.camOffset.X -= frameTime * 1000f;
						}
					}
					else if (c[0].Trajectory.X < 0f && c[0].AnimName.StartsWith("attack") && this.camOffset.X < (float)Game1.screenWidth * 0.2f)
					{
						this.camOffset.X += frameTime * 500f;
					}
				}
			}
			if (c[0].Holding && c[0].State == CharState.Air)
			{
				this.camOffset.Y -= frameTime;
				if (this.camOffset.Y < 0.5f)
				{
					this.camOffset.Y = 0.5f;
				}
			}
			else if ((c[0].CanFallThrough && c[0].ledgeAttach > -1) || c[0].AnimName == "attackair")
			{
				if (c[0].State == CharState.Grounded)
				{
					this.camOffset.Y += (0.6f - this.camOffset.Y) * frameTime * 10f;
				}
				else
				{
					this.camOffset.Y += frameTime;
					if (this.camOffset.Y > Camera.bottomOffset)
					{
						this.camOffset.Y = Camera.bottomOffset;
					}
				}
			}
			else
			{
				float num = ((!Camera.wall) ? MathHelper.Clamp(0.6f - (Camera.viewPoint.Y - c[0].Location.Y) / 800f, 0.4f, 1.2f) : 0.5f);
				this.camOffset.Y = Camera.bottomOffset * Game1.worldScale / Game1.hiDefScaleOffset + num * (1f - Game1.worldScale / Game1.hiDefScaleOffset);
			}
			if (Game1.events.eventCamera == Vector2.Zero)
			{
				if (!c[0].Holding || c[0].State == CharState.Grounded)
				{
					float target = c[0].Location.X - this.camOffset.X / Game1.hiDefScaleOffset - (float)(Game1.screenWidth / 2) / Game1.hiDefScaleOffset;
					this.tempScroll.X = GlobalFunctions.EaseInAndOut(this.tempScroll.X, target, 7f, frameTime);
				}
				if (map.warpStage > 3)
				{
					this.camOffset.X = 0f;
					if (Camera.cameraCatchup < 6f)
					{
						Camera.cameraCatchup += frameTime * 80f;
					}
					this.tempScroll.Y += (c[0].Location.Y + 200f - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * Camera.cameraCatchup;
				}
				else if (c[0].State == CharState.Air)
				{
					if (c[0].AnimName == "jumpboost")
					{
						this.tempScroll.Y += (c[0].Location.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset - this.tempScroll.Y) * frameTime * 10f;
						Camera.cameraCatchup = 20f;
					}
					else if (c[0].Boosting == 0)
					{
						this.tempScroll.Y += (c[0].Location.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * 0.75f - this.tempScroll.Y) * frameTime * 2f;
					}
					else if (c[0].AnimName == "jump" && c[0].Trajectory.Y < 0f && this.playerJumpPoint < (float)(Game1.screenHeight / 2) / Game1.hiDefScaleOffset)
					{
						int num2 = (int)(800f * Game1.worldScale / Game1.hiDefScaleOffset);
						this.tempScroll.Y += (c[0].Location.Y - (float)num2 - this.tempScroll.Y - (float)Game1.screenHeight * this.camOffset.Y / Game1.hiDefScaleOffset) * frameTime * (1f - (c[0].Location.Y - this.tempScroll.Y) * Game1.hiDefScaleOffset / (float)Game1.screenHeight);
						Camera.cameraCatchup = 0f;
					}
					else if (c[0].AnimName == "airspin")
					{
						if (c[0].Trajectory.Y < -500f)
						{
							if (Camera.cameraCatchup < 10f)
							{
								Camera.cameraCatchup += frameTime * 80f;
							}
						}
						else if (Camera.cameraCatchup < 6f)
						{
							Camera.cameraCatchup += frameTime * 10f;
						}
						this.tempScroll.Y += (Camera.finalViewPoint - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * 0.7f) * frameTime * Camera.cameraCatchup;
					}
					else if (c[0].AnimName == "airhike")
					{
						this.tempScroll.Y += (Camera.finalViewPoint - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * 2f;
					}
					else if (c[0].AnimName.StartsWith("attack") && c[0].AnimName != "attackairdown")
					{
						if (c[0].Holding)
						{
							if (Camera.cameraCatchup < 8f)
							{
								Camera.cameraCatchup += frameTime * 10f;
							}
							this.tempScroll.Y += (Camera.finalViewPoint - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * Camera.cameraCatchup;
						}
						else
						{
							if (c[0].Location.Y - 200f < this.tempScroll.Y)
							{
								Camera.cameraCatchup = 6f;
							}
							else
							{
								Camera.cameraCatchup += frameTime * 1f;
								if (Camera.cameraCatchup > 2f)
								{
									Camera.cameraCatchup = 2f;
								}
							}
							this.tempScroll.Y += (c[0].Location.Y + 200f - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * Camera.cameraCatchup;
						}
					}
					else if (c[0].AnimName.StartsWith("hang"))
					{
						Camera.cameraCatchup += frameTime * 5f;
						if (Camera.cameraCatchup > 4f)
						{
							Camera.cameraCatchup = 4f;
						}
						this.tempScroll.Y += (Camera.finalViewPoint - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * 0.7f) * frameTime * Camera.cameraCatchup;
						this.camOffset.X += ((float)(250 * ((c[0].Face != 0) ? 1 : (-1))) - this.camOffset.X) * frameTime * 2f;
					}
					else if (c[0].Trajectory.Y > 0f && this.tempScroll.Y < c[0].Location.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset + 300f)
					{
						Camera.cameraCatchup = MathHelper.Clamp(Camera.cameraCatchup + frameTime * (c[0].Trajectory.Y / 100f), -50f, 50f);
						this.tempScroll.Y += (c[0].Location.Y - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * Camera.cameraCatchup;
						if (this.tempScroll.Y < c[0].Location.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset)
						{
							this.tempScroll.Y = c[0].Location.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset;
						}
					}
					else if (c[0].Trajectory.Y < 1000f)
					{
						Camera.cameraCatchup = 0.4f;
						this.tempScroll.Y += (Camera.finalViewPoint - this.tempScroll.Y - (float)Game1.screenHeight / Game1.hiDefScaleOffset * this.camOffset.Y) * frameTime * Camera.cameraCatchup;
					}
				}
				else
				{
					Camera.cameraCatchup += frameTime * 4f;
					if (Camera.cameraCatchup > 4f)
					{
						Camera.cameraCatchup = 4f;
					}
					this.tempScroll.Y += (Camera.finalViewPoint - (float)Game1.screenHeight * this.camOffset.Y / Game1.hiDefScaleOffset - this.tempScroll.Y) * frameTime * Camera.cameraCatchup;
					if (map.transInFrame > 0.5f)
					{
						this.tempScroll.Y = Camera.finalViewPoint - (float)Game1.screenHeight * this.camOffset.Y / Game1.hiDefScaleOffset;
					}
				}
				if (this.eventCamInfluence > 0f)
				{
					this.eventCamInfluence += (0f - this.eventCamInfluence) * frameTime * 2f;
					if (this.eventCamInfluence < 0.01f)
					{
						this.eventCamInfluence = 0f;
					}
				}
			}
			else
			{
				if (this.eventCamInfluence < 1f)
				{
					this.eventCamInfluence += (1f - this.eventCamInfluence) * frameTime * 2f;
					if (this.eventCamInfluence > 0.9999f)
					{
						this.eventCamInfluence = 1f;
					}
				}
				Camera.cameraCatchup = Math.Min(Camera.cameraCatchup + frameTime * 2f, 4f);
				this.tempScroll = GlobalFunctions.EaseInAndOut(this.tempScroll, Game1.events.eventCamera - new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * (1f / Game1.hiDefScaleOffset), Camera.cameraCatchup, frameTime);
			}
			this.tempScroll += VibrationManager.ScreenShake.Vector * Game1.hiDefScaleOffset;
			this.UpdateScale(2, c, frameTime, forceFollow, ref followCount, ref playerScrollTemp);
			Game1.pScroll = Game1.Scroll;
			Vector2 vector = Vector2.Zero;
			float num3 = 1f;
			if (Camera.panRectList.Count > 0 && !Game1.events.anyEvent)
			{
				foreach (KeyValuePair<Rectangle, Vector3> panRect in Camera.panRectList)
				{
					if (panRect.Key.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
					{
						int num4 = (int)panRect.Value.Y;
						float num5 = (panRect.Value.X + 6f) / 12f * 6.28f;
						vector = new Vector2((float)(0.0 - Math.Sin(num5)) * (float)num4, (float)Math.Cos(num5) * (float)num4);
						num3 = panRect.Value.Z / 10f;
					}
				}
			}
			if (map.transInFrame > 0f && c[0].Trajectory.X == 0f)
			{
				Camera.panOffset = vector;
			}
			else
			{
				Camera.panOffset += (vector - Camera.panOffset) * frameTime * num3;
			}
			if (!this.UpdateFollow(c, frameTime, forceFollow, followCount, playerScrollTemp))
			{
				Camera.followSpeed = Math.Min(Camera.followSpeed + frameTime, 10f);
				Vector2 target2 = c[0].Location - new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.8f) / Game1.hiDefScaleOffset;
				Camera.playerScroll = GlobalFunctions.EaseInAndOut(Camera.playerScroll, target2, Camera.followSpeed, frameTime);
				float num6 = (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
				Vector2 vector2 = this.tempScroll * (1f - num6) + Camera.playerScroll * num6 + Camera.panOffset;
				if (this.eventCamInfluence > 0f)
				{
					if (map.GetTransVal() > 0f)
					{
						this.eventCamInfluence = 1f;
					}
					vector2 = vector2 * (1f - this.eventCamInfluence) + this.tempScroll * this.eventCamInfluence;
				}
				Game1.Scroll = vector2 * Game1.worldScale - new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * num6;
			}
			FidgetPrompt fidgetPrompt = Game1.hud.fidgetPrompt;
			if (c[0].KeyUp && ((c[0].State == CharState.Grounded && fidgetPrompt != FidgetPrompt.NoLookUp && fidgetPrompt != FidgetPrompt.OpenTreasure && fidgetPrompt != FidgetPrompt.OpenCage && fidgetPrompt != FidgetPrompt.Speak) || c[0].AnimName.StartsWith("hang")))
			{
				Camera.verticleLook += (-300f * Game1.worldScale - Camera.verticleLook) * frameTime * 2f;
			}
			else if (c[0].KeyDown)
			{
				if (c[0].CanFallThrough && c[0].ledgeAttach > -1)
				{
					Camera.verticleLook += (200f * Game1.worldScale - Camera.verticleLook) * frameTime * 2f;
				}
				else
				{
					Camera.verticleLook += (MathHelper.Clamp(Camera.finalViewPoint - c[0].Location.Y, 0f, 200f) * Game1.worldScale - Camera.verticleLook) * frameTime * 2f;
				}
			}
			else
			{
				Camera.verticleLook += (0f - Camera.verticleLook) * frameTime * 4f;
			}
			if (this.eventCamInfluence == 0f && map.warpStage == 0)
			{
				Vector2 vector3 = c[0].Location * Game1.worldScale;
				Game1.Scroll.X = MathHelper.Clamp(Game1.Scroll.X, vector3.X - (float)Game1.screenWidth, vector3.X);
				Game1.Scroll.Y = MathHelper.Clamp(Game1.Scroll.Y, vector3.Y - (float)Game1.screenHeight * 0.87f, vector3.Y - 200f * Game1.hiDefScaleOffset) + Camera.verticleLook;
			}
			Game1.Scroll.X = MathHelper.Clamp(Game1.Scroll.X, map.leftEdge * Game1.worldScale, map.rightEdge * Game1.worldScale - (float)Game1.screenWidth);
			Game1.Scroll.Y = MathHelper.Clamp(Game1.Scroll.Y, map.topEdge * Game1.worldScale, map.bottomEdge * Game1.worldScale - (float)Game1.screenHeight);
			int num7 = (int)((map.leftBlock == 0f) ? map.leftEdge : map.leftBlock);
			int num8 = (int)((map.rightBlock == 0f) ? map.rightEdge : map.rightBlock);
			if (this.eventCamInfluence > 0f)
			{
				Camera.camLeftBlock = Math.Min(Camera.camLeftBlock, Game1.events.eventCamera.X);
				Camera.camRightBlock = Math.Max(Camera.camRightBlock, Game1.events.eventCamera.X + (float)Game1.screenWidth / Game1.worldScale);
			}
			else
			{
				if (Camera.camLeftBlock > (float)num7)
				{
					Camera.camLeftBlock = Math.Max(Camera.camLeftBlock - frameTime * 1000f, num7);
				}
				else if (Camera.camLeftBlock < (float)num7)
				{
					Camera.camLeftBlock += ((float)num7 - Camera.camLeftBlock) * frameTime * 10f;
				}
				if (Camera.camRightBlock < (float)num8)
				{
					Camera.camRightBlock = Math.Min(Camera.camRightBlock + frameTime * 1000f, num8);
				}
				else if (Camera.camRightBlock > (float)num8)
				{
					Camera.camRightBlock += ((float)num8 - Camera.camRightBlock) * frameTime * 10f;
				}
			}
			Game1.Scroll.X = MathHelper.Clamp(Game1.Scroll.X, Camera.camLeftBlock * Game1.worldScale, Camera.camRightBlock * Game1.worldScale - (float)Game1.screenWidth);
			if (Game1.longSkipFrame != 2)
			{
				return;
			}
			this.HudInWay = (this.NavInWay = false);
			Vector2 vector4 = new Vector2(c[0].Location.X, c[0].Location.Y - (float)c[0].Height) * Game1.worldScale - Game1.Scroll;
			if (vector4.Y < (float)Game1.hud.screenTopOffset + 80f * Game1.hud.hudScale && c[0].CanHurtFrame == 0f)
			{
				if (vector4.X < (float)Game1.hud.screenLeftOffset + 300f * Game1.hud.hudScale)
				{
					this.HudInWay = true;
				}
				if (vector4.X > (float)(Game1.screenWidth - Game1.hud.screenLeftOffset) - 120f * Game1.hud.hudScale)
				{
					this.NavInWay = true;
				}
			}
		}

		private bool UpdateFollow(Character[] c, float frameTime, bool forceFollow, int followCount, Vector2 playerScrollTemp)
		{
			if (!Game1.events.anyEvent && followCount > 1)
			{
				playerScrollTemp /= (float)followCount;
				Camera.followSpeed = 2f;
				if (forceFollow)
				{
					Camera.followSpeed = 4f;
				}
				Vector2 target = playerScrollTemp - new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.7f) / Game1.hiDefScaleOffset;
				Camera.playerScroll = GlobalFunctions.EaseInAndOut(Camera.playerScroll, target, Camera.followSpeed, frameTime);
				float num = (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
				Vector2 vector = this.tempScroll * (1f - num) + Camera.playerScroll * num + Camera.panOffset;
				Game1.Scroll = vector * Game1.worldScale - new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * num;
				return true;
			}
			return false;
		}
	}
}

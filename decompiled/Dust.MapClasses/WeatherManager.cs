using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class WeatherManager
	{
		public string weather;

		public float weatherTimer = 240f;

		private static float particleTimer;

		public float lightningBloom;

		private static float lightningTimer;

		private static float windTimer;

		private static float windTarget;

		public float windStrength;

		public float fogWeatherAlpha;

		public RandomType randomType = RandomType.Forest;

		public int inStorm;

		public WeatherType weatherType;

		public WeatherAudioType weatherAudioType;

		public Vector4 weatherColor = new Vector4(1f, 1f, 1f, 0f);

		public float weatherSaturation = 1.2f;

		public float weatherBloom = 0.35f;

		private static Vector4 newWeatherColor;

		private static float newWeatherSaturation;

		private static float newWeatherBloom;

		public Vector2 mapBackPos;

		public int cavePollenCount;

		public int pollenCount;

		public int precipCount = 200;

		public int fogCount = 200;

		public int fireCount = 200;

		private Character player;

		public bool playerCanPrecipitate;

		public WeatherManager()
		{
			WeatherManager.newWeatherColor = this.weatherColor;
			WeatherManager.newWeatherBloom = this.weatherBloom;
			WeatherManager.newWeatherSaturation = this.weatherSaturation;
			this.ResetWeather();
		}

		public void ResetWeather()
		{
			this.randomType = RandomType.None;
			this.weatherType = WeatherType.None;
			this.pollenCount = 200;
			this.cavePollenCount = 200;
			this.fogCount = 200;
			this.fireCount = 200;
			this.precipCount = 200;
			this.weatherColor = new Vector4(1f, 1f, 1f, 0f);
			this.weatherSaturation = 1.2f;
			this.weatherBloom = 0.0875f;
			this.windStrength = (WeatherManager.windTarget = (WeatherManager.windTimer = 0f));
		}

		public void SetWeather(WeatherType weather, bool forceReset)
		{
			RandomType randomType;
			if (forceReset)
			{
				randomType = RandomType.None;
			}
			else
			{
				if (this.weatherType == weather && ((this.randomType != 0 && weather == WeatherType.None) || (this.randomType == RandomType.None && weather != 0) || weather == WeatherType.Realtime))
				{
					return;
				}
				randomType = this.randomType;
			}
			this.randomType = RandomType.None;
			this.lightningBloom = 0f;
			this.inStorm = 0;
			this.cavePollenCount = 200;
			this.pollenCount = 200;
			this.precipCount = 200;
			this.fogCount = 200;
			this.fireCount = 200;
			this.windStrength = (WeatherManager.windTarget = 0f);
			switch (weather)
			{
			case WeatherType.None:
				this.randomType = RandomType.None;
				WeatherAudio.Play(WeatherAudioType.Silent);
				break;
			case WeatherType.Realtime:
			{
				bool flag = false;
				RandomType randomType2 = this.randomType;
				WeatherType weatherType = WeatherType.None;
				switch (Game1.map.regionName)
				{
				case "glade":
				case "aurora":
				case "forest":
				case "smith":
					randomType2 = RandomType.Forest;
					if ((flag = this.weatherType == WeatherType.CaveCalm) || randomType != randomType2)
					{
						weatherType = WeatherType.Pollen;
					}
					break;
				case "cave":
					randomType2 = RandomType.Cave;
					if (randomType != randomType2)
					{
						weatherType = WeatherType.CaveCalm;
					}
					break;
				case "snow":
					randomType2 = RandomType.Snow;
					if (randomType != randomType2)
					{
						weatherType = WeatherType.SnowLight;
					}
					break;
				case "grave":
				case "trial":
					randomType2 = RandomType.Grave;
					if ((flag = this.weatherType == WeatherType.Mansion) || randomType != randomType2)
					{
						weatherType = ((Rand.GetRandomInt(0, 2) == 1) ? WeatherType.RainLight : WeatherType.Graveyard);
					}
					break;
				}
				if ((weatherType != WeatherType.None && this.randomType != 0) || this.weatherType == WeatherType.None || flag)
				{
					this.SetWeather(weatherType, forceReset: false);
					this.weatherTimer = Rand.GetRandomInt(20, 200);
				}
				else if (randomType != randomType2 && Game1.events != null && Game1.events.currentEvent > 22)
				{
					this.weatherTimer = Rand.GetRandomInt(2, 30);
				}
				this.randomType = randomType2;
				break;
			}
			case WeatherType.Pollen:
				WeatherAudio.Play(WeatherAudioType.Forest);
				this.pollenCount = 0;
				break;
			case WeatherType.RainLight:
				WeatherAudio.Play(WeatherAudioType.RainLight);
				this.precipCount = 0;
				break;
			case WeatherType.RainHeavy:
				if (weather != WeatherType.RainLight)
				{
					this.precipCount = 0;
				}
				WeatherAudio.Play(WeatherAudioType.RainHeavy);
				break;
			case WeatherType.Fog:
				if (Game1.map.regionName == "cave")
				{
					WeatherAudio.Play(WeatherAudioType.CaveCalm);
				}
				else
				{
					WeatherAudio.Play(WeatherAudioType.Silent);
				}
				Game1.pManager.RemoveParticle(new Fog(0));
				this.fogCount = 0;
				break;
			case WeatherType.Fire:
				WeatherAudio.Play(WeatherAudioType.Fire);
				this.fireCount = 0;
				break;
			case WeatherType.CaveCalm:
				WeatherAudio.Play(WeatherAudioType.CaveCalm);
				this.cavePollenCount = 0;
				break;
			case WeatherType.CaveRumble:
				WeatherAudio.Play(WeatherAudioType.CaveRumbly);
				this.cavePollenCount = 0;
				this.weatherTimer = 4f;
				break;
			case WeatherType.Graveyard:
				WeatherAudio.Play(WeatherAudioType.Graveyard);
				break;
			case WeatherType.Mansion:
				WeatherAudio.Play(WeatherAudioType.Mansion);
				Game1.pManager.RemoveParticle(new Fog(0));
				this.fogCount = 0;
				break;
			case WeatherType.SnowLight:
				WeatherAudio.Play(WeatherAudioType.SnowLight);
				this.precipCount = 0;
				break;
			case WeatherType.SnowHeavy:
				WeatherAudio.Play(WeatherAudioType.SnowHeavy);
				this.precipCount = 0;
				break;
			case WeatherType.SnowFierce:
				WeatherAudio.Play(WeatherAudioType.SnowFierce);
				this.precipCount = 0;
				break;
			case WeatherType.SnowTime:
				WeatherAudio.Play(WeatherAudioType.Silent);
				this.precipCount = 0;
				break;
			case WeatherType.LavaLight:
				WeatherAudio.Play(WeatherAudioType.LavaLight);
				this.precipCount = 0;
				break;
			case WeatherType.LavaWar:
				WeatherAudio.Play(WeatherAudioType.LavaWar);
				this.precipCount = 0;
				break;
			case WeatherType.ThunderStorm:
				this.weatherTimer = 2f;
				this.precipCount = 0;
				this.inStorm = 1;
				WeatherAudio.Play(WeatherAudioType.RainLight);
				break;
			default:
				this.randomType = RandomType.None;
				WeatherAudio.Play(WeatherAudioType.Silent);
				break;
			}
			if (weather != 0)
			{
				this.weatherType = weather;
			}
			this.SetWeatherColor(weather);
		}

		public void SetManualColor(Vector4 newColor, float newSaturation, float newBloom, bool immediately)
		{
			WeatherManager.newWeatherColor = newColor;
			WeatherManager.newWeatherSaturation = newSaturation;
			WeatherManager.newWeatherBloom = newBloom;
			if (immediately)
			{
				this.ForceColor();
			}
		}

		public void ForceColor()
		{
			this.weatherColor = WeatherManager.newWeatherColor;
			this.weatherSaturation = WeatherManager.newWeatherSaturation;
			this.weatherBloom = WeatherManager.newWeatherBloom;
		}

		public void SetWeatherColor(WeatherType weather)
		{
			switch (weather)
			{
			case WeatherType.LavaLight:
			case WeatherType.LavaWar:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 0.5f, 0f);
				WeatherManager.newWeatherSaturation = 0.6f;
				WeatherManager.newWeatherBloom = 0.4f;
				break;
			case WeatherType.SnowLight:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, -0.05f);
				WeatherManager.newWeatherSaturation = 1f;
				WeatherManager.newWeatherBloom = 0.15f;
				break;
			case WeatherType.SnowHeavy:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, -0.05f);
				WeatherManager.newWeatherSaturation = 1f;
				WeatherManager.newWeatherBloom = 0.1f;
				break;
			case WeatherType.SnowFierce:
				WeatherManager.newWeatherColor = new Vector4(0.3f, 0.7f, 1f, -0.1f);
				WeatherManager.newWeatherSaturation = 0f;
				WeatherManager.newWeatherBloom = 0.4f;
				break;
			case WeatherType.SnowTime:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 0.5f, -0.05f);
				WeatherManager.newWeatherSaturation = 1f;
				WeatherManager.newWeatherBloom = 0.5f;
				break;
			case WeatherType.Graveyard:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, -0.05f);
				WeatherManager.newWeatherSaturation = 1.6f;
				WeatherManager.newWeatherBloom = 0.25f;
				break;
			case WeatherType.Mansion:
				WeatherManager.newWeatherColor = new Vector4(0.8f, 0.8f, 0.8f, -0.1f);
				WeatherManager.newWeatherSaturation = 0.2f;
				WeatherManager.newWeatherBloom = 0.25f;
				break;
			case WeatherType.CaveCalm:
			case WeatherType.CaveRumble:
				if (Game1.map.regionName == "snow")
				{
					WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, -0.05f);
					WeatherManager.newWeatherSaturation = 1f;
					WeatherManager.newWeatherBloom = 0.2f;
				}
				else
				{
					float randomFloat = Rand.GetRandomFloat(0.6f, 0.9f);
					WeatherManager.newWeatherColor = new Vector4(randomFloat, randomFloat, randomFloat, 0f);
					WeatherManager.newWeatherSaturation = 1.2f;
					WeatherManager.newWeatherBloom = 0.3f;
				}
				break;
			case WeatherType.Fire:
				WeatherManager.newWeatherColor = new Vector4(1f, 0.8f, 0.6f, -0.05f);
				WeatherManager.newWeatherSaturation = 0.75f;
				WeatherManager.newWeatherBloom = 0.25f;
				break;
			case WeatherType.Fog:
				if (Game1.map.path.StartsWith("challenge"))
				{
					WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, -0.05f);
					WeatherManager.newWeatherSaturation = 1.2f;
					WeatherManager.newWeatherBloom = 0.1f;
				}
				else
				{
					WeatherManager.newWeatherColor = new Vector4(0.6f, 0.6f, 1f, -0.05f);
					WeatherManager.newWeatherSaturation = 0.5f;
					WeatherManager.newWeatherBloom = 0.2f;
				}
				break;
			case WeatherType.RainHeavy:
				WeatherManager.newWeatherColor = new Vector4(0.6f, 0.6f, 1f, 0f);
				WeatherManager.newWeatherSaturation = 0.25f;
				WeatherManager.newWeatherBloom = 0.2f;
				break;
			case WeatherType.RainLight:
				WeatherManager.newWeatherColor = new Vector4(0.8f, 0.8f, 1f, -0.01f);
				WeatherManager.newWeatherSaturation = 0.75f;
				WeatherManager.newWeatherBloom = 0.15f;
				break;
			case WeatherType.Pollen:
				WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, 0f);
				WeatherManager.newWeatherSaturation = 1.2f;
				WeatherManager.newWeatherBloom = 0.1f;
				break;
			default:
				if (weather != 0)
				{
					WeatherManager.newWeatherColor = new Vector4(1f, 1f, 1f, 0f);
					WeatherManager.newWeatherSaturation = 1.2f;
					WeatherManager.newWeatherBloom = 0.1f;
				}
				break;
			}
		}

		private void CheckPrecipitate(Map map, Vector2 loc)
		{
			bool flag = true;
			for (int i = 0; i < map.noPrecipRegion.Count; i++)
			{
				_ = map.noPrecipRegion[i];
				if (map.noPrecipRegion[i].Contains((int)loc.X, (int)loc.Y))
				{
					flag = (this.playerCanPrecipitate = false);
					if (map.noPrecipRegion[i].Contains((int)loc.X, (int)loc.Y - 500))
					{
						Sound.weatherVolume = Math.Max(Sound.weatherVolume - Game1.FrameTime * 0.2f, Math.Max(2000f / (float)(map.noPrecipRegion[i].Width + map.noPrecipRegion[i].Height), 0f));
					}
				}
			}
			if (flag)
			{
				this.playerCanPrecipitate = true;
				Sound.weatherVolume = Math.Min(Sound.weatherVolume + Game1.FrameTime, 1f);
			}
		}

		public void UpdateWeather(ParticleManager pMan, Map map, string path)
		{
			if (map.GetTransVal() >= 1f)
			{
				return;
			}
			if (this.precipCount < 100)
			{
				Vector2 randomVector = Rand.GetRandomVector2(-60f, Game1.screenWidth + 360, -100f, 0f);
				switch (this.weatherType)
				{
				case WeatherType.RainLight:
					pMan.AddRain(Game1.Scroll * 1.25f + randomVector, Rand.GetRandomVector2(-800f, -200f, 1200f, 2000f), Rand.GetRandomFloat(0.5f, 2f), 0, 8);
					break;
				case WeatherType.RainHeavy:
					pMan.AddRain(Game1.Scroll * 1.25f + randomVector + new Vector2(Rand.GetRandomInt(-100, 100), 0f), Rand.GetRandomVector2(-1000f, 0f, 1200f, 1600f), Rand.GetRandomFloat(2f, 2.2f), 1, 8);
					break;
				case WeatherType.SnowLight:
					pMan.AddSnow(Rand.GetRandomVector2(-200f, 0f, 100f, 400f), Rand.GetRandomFloat(0.4f, 0.8f), 0, 8);
					break;
				case WeatherType.SnowTime:
					pMan.AddSnowTime(Rand.GetRandomVector2(-200f, 0f, 100f, 400f), Rand.GetRandomFloat(1f, 2f), 8);
					break;
				case WeatherType.SnowHeavy:
				case WeatherType.SnowFierce:
					pMan.AddSnow(Rand.GetRandomVector2(-1000f, 0f, 800f, 1000f), Rand.GetRandomFloat(0.5f, 1f), 1, 8);
					break;
				case WeatherType.LavaLight:
				case WeatherType.LavaWar:
					pMan.AddVolcanicAsh(Rand.GetRandomVector2(-200f, 0f, 100f, 400f), Rand.GetRandomFloat(0.4f, 0.8f), 0, 8);
					break;
				}
			}
			switch (this.weatherType)
			{
			case WeatherType.Pollen:
				this.UpdateWind(-10, 10, 5f, Game1.FrameTime);
				break;
			case WeatherType.RainLight:
			case WeatherType.RainHeavy:
			case WeatherType.Fire:
				this.UpdateLightning();
				this.UpdateWind(-50, 10, 5f, Game1.FrameTime);
				if (this.weatherType == WeatherType.RainHeavy && Rand.GetRandomInt(0, 50) == 0 && this.playerCanPrecipitate && map.MapSegFrameSpeed > 2f)
				{
					map.MapSegFrameSpeed /= 2f;
				}
				break;
			case WeatherType.Graveyard:
				this.UpdateWind(-50, -10, 5f, Game1.FrameTime);
				if (Rand.GetRandomInt(0, 100) == 0)
				{
					Vector2 vector = new Vector2(Game1.screenWidth, Rand.GetRandomFloat(-200f, 700f)) / Game1.worldScale;
					int randomInt = Rand.GetRandomInt(5, 30);
					for (int j = 0; j < randomInt; j++)
					{
						pMan.AddGraveLeaf(vector + Rand.GetRandomVector2(0f, 100f, -100f, 100f), Rand.GetRandomVector2(-1000f, -400f, 0f, 200f), Rand.GetRandomFloat(0.6f, 1.2f), 8);
					}
				}
				if (map.bugCount < 4 && Rand.GetRandomInt(0, 200) == 0)
				{
					if (this.player.Trajectory.X == 0f)
					{
						pMan.AddFlySwarm(this.player.Location + Rand.GetRandomVector2(-1000f, 1000f, -700f, 200f), 5);
					}
					else
					{
						pMan.AddFlySwarm(this.player.Location + this.player.Trajectory * 1.5f + new Vector2(0f, Rand.GetRandomFloat(-300f, 100f)), 5);
					}
				}
				break;
			case WeatherType.SnowLight:
			case WeatherType.SnowHeavy:
				this.UpdateWind(-200, 200, 5f, Game1.FrameTime);
				break;
			case WeatherType.LavaLight:
			case WeatherType.LavaWar:
				this.UpdateWind(-200, 200, 1f, Game1.FrameTime);
				if (this.weatherType == WeatherType.LavaWar && this.playerCanPrecipitate && Rand.GetRandomInt(0, 40) == 0)
				{
					float randomFloat = Rand.GetRandomFloat(0.1f, 1.5f);
					Vector2 randomVector2 = Rand.GetRandomVector2(200f, 1620f, 0f, 600f);
					for (int i = 0; i < 8; i++)
					{
						pMan.AddExplosion(randomVector2 + Rand.GetRandomVector2(-50f, 50f, -50f, 50f) * randomFloat, Rand.GetRandomFloat(0.4f, 1f) * randomFloat, makeSmoke: false, 2);
					}
				}
				if (Rand.GetRandomInt(0, 10) == 0)
				{
					WeatherManager.newWeatherBloom = Rand.GetRandomFloat(0.3f, 0.5f);
				}
				this.weatherBloom += (WeatherManager.newWeatherBloom - this.weatherBloom) * Game1.FrameTime * 4f;
				if (Game1.halfSecFrame % 10 == 0)
				{
					Vector2 loc = (Game1.Scroll + Rand.GetRandomVector2(-200f, Game1.screenWidth + 400, 400f, Game1.screenHeight)) / Game1.worldScale;
					pMan.AddHeatWave(loc, Rand.GetRandomVector2(-100f + this.windStrength * 2f, 100f + this.windStrength * 2f, -400f, -50f), 2.5f, Rand.GetRandomFloat(1f, 4f), 6);
				}
				if (this.playerCanPrecipitate)
				{
					map.fogColor.X += (this.windStrength / 100f - map.fogColor.X) * Game1.FrameTime * 10f;
					map.fogColor.W += (0.12f - map.fogColor.W) * Game1.FrameTime * 0.2f;
				}
				else
				{
					map.fogColor.X += (0f - map.fogColor.X) * Game1.FrameTime * 10f;
					map.fogColor.W += (0.06f - map.fogColor.W) * Game1.FrameTime * 0.2f;
				}
				break;
			case WeatherType.SnowFierce:
				this.UpdateWind(-250, -210, 5f, Game1.FrameTime);
				break;
			default:
				this.UpdateWind(-10, 10, 5f, Game1.FrameTime);
				if (this.precipCount < 100)
				{
					this.precipCount++;
				}
				break;
			}
			if (this.fogCount < 10)
			{
				pMan.AddFog(8, 8);
			}
			this.UpdateFog(path);
			if (this.pollenCount < 100)
			{
				pMan.AddPollen(8, 8);
			}
			if (this.pollenCount < 102 && this.weatherType != WeatherType.Pollen)
			{
				this.pollenCount++;
			}
			if (this.cavePollenCount < 100)
			{
				pMan.AddCavePollen(8, 8);
			}
			if (this.cavePollenCount < 102 && this.weatherType != WeatherType.CaveCalm && this.weatherType != WeatherType.CaveRumble)
			{
				this.cavePollenCount++;
			}
			if (this.fireCount < 60 && Rand.GetRandomInt(0, 3) == 0)
			{
				pMan.AddFlameSpark(Game1.Scroll * 1.25f + Rand.GetRandomVector2(0f, Game1.screenWidth, (float)Game1.screenHeight * 0.5f, Game1.screenHeight + 130), Rand.GetRandomVector2(-200f, 200f, -1000f, -200f), 90f, Rand.GetRandomFloat(1f, 2f), 1, 8);
			}
			if (this.weatherType != WeatherType.Fire && this.fireCount < 62)
			{
				this.fireCount++;
			}
			if (this.randomType != 0)
			{
				this.weatherTimer -= Game1.FrameTime;
				if (this.weatherTimer < 0f)
				{
					switch (this.randomType)
					{
					case RandomType.Forest:
						if (this.weatherType != WeatherType.RainLight && this.weatherType != WeatherType.Fog)
						{
							if (this.weatherType != WeatherType.RainHeavy)
							{
								this.precipCount = 0;
							}
							WeatherAudio.Play(WeatherAudioType.RainLight);
							this.weatherType = WeatherType.RainLight;
							this.weatherTimer = Rand.GetRandomFloat(20f, 60f);
							if (this.inStorm == 2)
							{
								this.inStorm = 0;
							}
							else if (Rand.GetRandomInt(0, 2) == 0)
							{
								this.inStorm = 1;
							}
						}
						else if (this.weatherType == WeatherType.RainLight && this.inStorm == 1)
						{
							WeatherAudio.Play(WeatherAudioType.RainHeavy);
							this.inStorm = 2;
							this.weatherType = WeatherType.RainHeavy;
							this.weatherTimer = Rand.GetRandomFloat(20f, 40f);
						}
						else if (this.weatherType != WeatherType.Fog && Rand.GetRandomInt(0, 2) == 0)
						{
							WeatherAudio.Play(WeatherAudioType.Silent);
							this.inStorm = 0;
							this.weatherType = WeatherType.Fog;
							this.precipCount = 0;
							this.fogCount = 0;
							this.lightningBloom = 0f;
							this.weatherTimer = Rand.GetRandomFloat(20f, 60f);
						}
						else
						{
							WeatherAudio.Play(WeatherAudioType.Forest);
							this.precipCount = 0;
							this.inStorm = 0;
							this.pollenCount = 0;
							this.weatherType = WeatherType.Pollen;
							this.lightningBloom = 0f;
							this.weatherTimer = Rand.GetRandomFloat(120f, 600f);
						}
						break;
					case RandomType.Cave:
						this.weatherType = WeatherType.CaveCalm;
						this.weatherTimer = Rand.GetRandomFloat(2f, 5f);
						break;
					case RandomType.Grave:
						if (this.weatherType != WeatherType.RainLight)
						{
							if (this.weatherType != WeatherType.RainHeavy)
							{
								this.precipCount = 0;
							}
							WeatherAudio.Play(WeatherAudioType.RainLight);
							this.weatherType = WeatherType.RainLight;
							this.weatherTimer = Rand.GetRandomFloat(20f, 60f);
							if (this.inStorm == 2)
							{
								this.inStorm = 0;
							}
							else if (Rand.GetRandomInt(0, 2) == 0)
							{
								this.inStorm = 1;
							}
						}
						else if (this.weatherType == WeatherType.RainLight && this.inStorm == 1)
						{
							WeatherAudio.Play(WeatherAudioType.RainHeavy);
							this.inStorm = 2;
							this.weatherType = WeatherType.RainHeavy;
							this.weatherTimer = Rand.GetRandomFloat(20f, 40f);
						}
						else
						{
							WeatherAudio.Play(WeatherAudioType.Graveyard);
							this.precipCount = 0;
							this.inStorm = 0;
							this.weatherType = WeatherType.Graveyard;
							this.lightningBloom = 0f;
							this.weatherTimer = Rand.GetRandomFloat(120f, 600f);
						}
						break;
					case RandomType.Snow:
						if (this.weatherType != WeatherType.SnowHeavy)
						{
							if (this.weatherType != WeatherType.SnowFierce)
							{
								this.precipCount = 0;
							}
							WeatherAudio.Play(WeatherAudioType.SnowHeavy);
							this.weatherType = WeatherType.SnowHeavy;
							this.weatherTimer = Rand.GetRandomFloat(20f, 60f);
							if (this.inStorm == 2)
							{
								this.inStorm = 0;
							}
							else if (Rand.GetRandomInt(0, 2) == 0 && this.playerCanPrecipitate)
							{
								this.inStorm = 1;
							}
						}
						else if (this.weatherType == WeatherType.SnowHeavy && this.inStorm == 1)
						{
							WeatherAudio.Play(WeatherAudioType.SnowFierce);
							this.inStorm = 2;
							this.weatherType = WeatherType.SnowFierce;
							this.weatherTimer = Rand.GetRandomFloat(20f, 24f);
						}
						else
						{
							WeatherAudio.Play(WeatherAudioType.SnowLight);
							this.precipCount = 0;
							this.inStorm = 0;
							this.weatherType = WeatherType.SnowLight;
							this.weatherTimer = Rand.GetRandomFloat(120f, 600f);
						}
						break;
					}
					WeatherManager.windTimer = 1f;
					this.SetWeatherColor(this.weatherType);
				}
			}
			WeatherType weatherType = this.weatherType;
			if (weatherType == WeatherType.CaveRumble)
			{
				this.weatherTimer -= Game1.FrameTime;
				this.windStrength = 0f;
				if (this.weatherTimer < 5f)
				{
					this.windStrength = 2f;
				}
				if (this.weatherTimer < 3f)
				{
					VibrationManager.ScreenShake.value = 0.2f * this.weatherTimer;
					VibrationManager.Rumble(Game1.currentGamePad, Math.Min(0.1f * this.weatherTimer, 0.15f));
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						Vector2 vector2 = (Game1.Scroll + new Vector2(Rand.GetRandomFloat(100f, Game1.screenWidth - 200), 0f)) / Game1.worldScale;
						for (int k = 0; k < 10; k++)
						{
							Game1.pManager.AddBlood(vector2 + new Vector2(Rand.GetRandomFloat(-60f, 60f), 0f), new Vector2(0f, Rand.GetRandomFloat(0f, 1000f)), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.1f, 0.5f), (CharacterType)1000, 0, 5);
						}
					}
				}
				if (this.weatherTimer < 0f)
				{
					this.weatherTimer = Rand.GetRandomFloat(2f, 20f);
				}
			}
			if (this.weatherColor != WeatherManager.newWeatherColor || this.weatherSaturation != WeatherManager.newWeatherSaturation || this.weatherBloom != WeatherManager.newWeatherBloom)
			{
				int num = 10;
				if (this.weatherColor.X < WeatherManager.newWeatherColor.X)
				{
					this.weatherColor.X += Game1.FrameTime / (float)num;
					if (this.weatherColor.X > WeatherManager.newWeatherColor.X)
					{
						this.weatherColor.X = WeatherManager.newWeatherColor.X;
					}
				}
				if (this.weatherColor.X > WeatherManager.newWeatherColor.X)
				{
					this.weatherColor.X -= Game1.FrameTime / (float)num;
					if (this.weatherColor.X < WeatherManager.newWeatherColor.X)
					{
						this.weatherColor.X = WeatherManager.newWeatherColor.X;
					}
				}
				if (this.weatherColor.Y < WeatherManager.newWeatherColor.Y)
				{
					this.weatherColor.Y += Game1.FrameTime / (float)num;
					if (this.weatherColor.Y > WeatherManager.newWeatherColor.Y)
					{
						this.weatherColor.Y = WeatherManager.newWeatherColor.Y;
					}
				}
				if (this.weatherColor.Y > WeatherManager.newWeatherColor.Y)
				{
					this.weatherColor.Y -= Game1.FrameTime / (float)num;
					if (this.weatherColor.Y < WeatherManager.newWeatherColor.Y)
					{
						this.weatherColor.Y = WeatherManager.newWeatherColor.Y;
					}
				}
				if (this.weatherColor.Z < WeatherManager.newWeatherColor.Z)
				{
					this.weatherColor.Z += Game1.FrameTime / (float)num;
					if (this.weatherColor.Z > WeatherManager.newWeatherColor.Z)
					{
						this.weatherColor.Z = WeatherManager.newWeatherColor.Z;
					}
				}
				if (this.weatherColor.Z > WeatherManager.newWeatherColor.Z)
				{
					this.weatherColor.Z -= Game1.FrameTime / (float)num;
					if (this.weatherColor.Z < WeatherManager.newWeatherColor.Z)
					{
						this.weatherColor.Z = WeatherManager.newWeatherColor.Z;
					}
				}
				if (this.weatherColor.W < WeatherManager.newWeatherColor.W)
				{
					this.weatherColor.W += Game1.FrameTime / (float)num;
					if (this.weatherColor.W > WeatherManager.newWeatherColor.W)
					{
						this.weatherColor.W = WeatherManager.newWeatherColor.W;
					}
				}
				if (this.weatherColor.W > WeatherManager.newWeatherColor.W)
				{
					this.weatherColor.W -= Game1.FrameTime / (float)num;
					if (this.weatherColor.W < WeatherManager.newWeatherColor.W)
					{
						this.weatherColor.W = WeatherManager.newWeatherColor.W;
					}
				}
				if (this.weatherSaturation < WeatherManager.newWeatherSaturation)
				{
					this.weatherSaturation += Game1.FrameTime / (float)num;
					if (this.weatherSaturation > WeatherManager.newWeatherSaturation)
					{
						this.weatherSaturation = WeatherManager.newWeatherSaturation;
					}
				}
				if (this.weatherSaturation > WeatherManager.newWeatherSaturation)
				{
					this.weatherSaturation -= Game1.FrameTime / (float)num;
					if (this.weatherSaturation < WeatherManager.newWeatherSaturation)
					{
						this.weatherSaturation = WeatherManager.newWeatherSaturation;
					}
				}
				if (this.weatherBloom < WeatherManager.newWeatherBloom)
				{
					this.weatherBloom += Game1.FrameTime / (float)num;
					if (this.weatherBloom > WeatherManager.newWeatherBloom)
					{
						this.weatherBloom = WeatherManager.newWeatherBloom;
					}
				}
				if (this.weatherBloom > WeatherManager.newWeatherBloom)
				{
					this.weatherBloom -= Game1.FrameTime / (float)num;
					if (this.weatherBloom < WeatherManager.newWeatherBloom)
					{
						this.weatherBloom = WeatherManager.newWeatherBloom;
					}
				}
			}
			if ((double)WeatherManager.particleTimer > 0.4)
			{
				WeatherManager.particleTimer = 0f;
			}
			WeatherManager.particleTimer += Game1.FrameTime;
		}

		private void UpdateWind(int minStrength, int maxStrength, float newTime, float frameTime)
		{
			WeatherManager.windTimer -= frameTime;
			if (WeatherManager.windTimer < 0f)
			{
				WeatherManager.windTarget = Rand.GetRandomFloat(minStrength, maxStrength);
				WeatherManager.windTimer = Math.Max(newTime + Rand.GetRandomFloat(-5f, 5f), 0.5f);
				if (!this.playerCanPrecipitate)
				{
					WeatherManager.windTarget /= 8f;
				}
			}
			if (this.windStrength < WeatherManager.windTarget)
			{
				this.windStrength += frameTime * 100f;
			}
			else if (this.windStrength > WeatherManager.windTarget + 4f)
			{
				this.windStrength -= frameTime * 100f;
			}
		}

		private void UpdateLightning()
		{
			WeatherManager.lightningTimer -= Game1.FrameTime * 2f;
			if (this.lightningBloom > 0f)
			{
				if (Rand.GetRandomInt(0, 40) == 0)
				{
					this.lightningBloom = Rand.GetRandomFloat(0.2f, 0.8f);
					WeatherManager.lightningTimer += 1f;
				}
				this.lightningBloom -= Game1.FrameTime;
				if (this.lightningBloom < 0f)
				{
					this.lightningBloom = 0f;
				}
			}
			if (!(WeatherManager.lightningTimer < 0f))
			{
				return;
			}
			if (this.playerCanPrecipitate && Game1.map.GetTransVal() <= 0f && Game1.events.screenFade.A == 0)
			{
				if (this.weatherType == WeatherType.RainLight)
				{
					this.lightningBloom = Rand.GetRandomFloat(0.2f, 0.8f);
					WeatherManager.lightningTimer = Rand.GetRandomFloat(4f, 15f);
					if (this.lightningBloom > 0.3f)
					{
						Sound.PlayCue("thunder");
					}
				}
				else
				{
					this.lightningBloom = Rand.GetRandomFloat(0.4f, 1.1f);
					WeatherManager.lightningTimer = Rand.GetRandomFloat(1f, 6f);
					Sound.PlayCue("thunder");
				}
				int randomInt = Rand.GetRandomInt(1, 3);
				for (int i = 0; i < randomInt; i++)
				{
					Game1.pManager.AddLightning(Rand.GetRandomVector2(200f, 1620f, 0f, 300f) * Game1.hiDefScaleOffset, master: true, Rand.GetRandomFloat(0.5f, 1.5f), Math.Max(Rand.GetRandomInt(4, 10) / randomInt, 4), 60, 0.4f, 2);
				}
			}
			else
			{
				WeatherManager.lightningTimer = Rand.GetRandomFloat(4f, 15f);
			}
		}

		private void UpdateFog(string path)
		{
			if (this.weatherType == WeatherType.Fog)
			{
				if (!path.StartsWith("challenge"))
				{
					if (this.fogWeatherAlpha < 0.1f)
					{
						this.fogWeatherAlpha += Game1.FrameTime * 0.02f;
						if (this.fogWeatherAlpha > 0.1f)
						{
							this.fogWeatherAlpha = 0.1f;
						}
					}
				}
				else if (this.fogWeatherAlpha > 0f)
				{
					this.fogWeatherAlpha -= Game1.FrameTime * 0.02f;
				}
			}
			else
			{
				if (this.fogCount < 20)
				{
					this.fogCount++;
				}
				if (this.fogWeatherAlpha > 0f)
				{
					this.fogWeatherAlpha -= Game1.FrameTime * 0.02f;
				}
			}
		}

		public void UpdateRegions(ParticleManager pMan, Character[] c, Map map)
		{
			this.player = Game1.character[0];
			this.CheckPrecipitate(map, this.player.Location);
			for (int i = 0; i < map.boostRegions.Count; i++)
			{
				if (map.boostRegions[i] != null)
				{
					int x = (int)((float)(map.boostRegions[i].Region.X + map.boostRegions[i].Region.Width / 2) * Game1.worldScale - Game1.Scroll.X);
					int y = (int)((float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height / 2) * Game1.worldScale - Game1.Scroll.Y);
					int num = (int)((float)map.boostRegions[i].Region.Width * Game1.worldScale);
					int num2 = (int)((float)map.boostRegions[i].Region.Height * Game1.worldScale);
					if (new Rectangle(-num, -num2, Game1.screenWidth + num * 2, Game1.screenHeight + num2 * 2).Contains(x, y))
					{
						map.boostRegions[i].renderable = true;
						map.boostRegions[i].Update(Game1.FrameTime, map);
					}
					else
					{
						map.boostRegions[i].renderable = false;
					}
				}
			}
			if (Game1.halfSecFrame <= 30)
			{
				return;
			}
			if (map.weatherRegion.Count > 0)
			{
				WeatherType id = this.weatherType;
				for (int j = 0; j < map.weatherRegion.Count; j++)
				{
					if (map.weatherRegion[j] != null && map.weatherRegion[j].Region.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
					{
						id = (WeatherType)map.weatherRegion[j].Id;
					}
				}
				if (id != this.weatherType)
				{
					this.SetWeather(id, forceReset: false);
				}
			}
			for (int k = 0; k < map.weatherColorRegion.Count; k++)
			{
				if (map.weatherColorRegion[k] != null && map.weatherColorRegion[k].Region.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
				{
					if (map.weatherColorRegion[k].Id == 0)
					{
						this.SetWeatherColor(this.weatherType);
					}
					else
					{
						this.SetWeatherColor((WeatherType)map.weatherColorRegion[k].Id);
					}
				}
			}
		}

		public void DrawRegions(SpriteBatch sprite, Map map, Texture2D[] particlesTex, float worldScale, bool refractive, bool additive)
		{
			for (int i = 0; i < map.boostRegions.Count; i++)
			{
				if (map.boostRegions[i] != null && map.boostRegions[i].renderable)
				{
					map.boostRegions[i].Draw(sprite, particlesTex, worldScale, refractive, additive);
				}
			}
		}

		public void DrawBackdrop(SpriteBatch sprite, ParticleManager pMan, Texture2D backDropTex, Texture2D backDropVid, Texture2D[] mapsTex, Texture2D[] particleTex, Texture2D nullTex, int backDrop, float leftEdge, float topEdge)
		{
			Color color = new Color(this.weatherColor.X * (1f - Game1.map.fogColor.X * Game1.map.fogColor.W), this.weatherColor.Y * (1f - Game1.map.fogColor.Y * Game1.map.fogColor.W), this.weatherColor.Z * (1f - Game1.map.fogColor.Z * Game1.map.fogColor.W), 1f);
			leftEdge = (leftEdge - Game1.Scroll.X) * 0.05f;
			topEdge = (topEdge - Game1.Scroll.Y) * 0.05f;
			this.mapBackPos = new Vector2(MathHelper.Min(leftEdge, 0f), MathHelper.Min(topEdge, 0f));
			if (backDrop > 1)
			{
				if (backDropVid != null)
				{
					sprite.Draw(backDropVid, this.mapBackPos, new Rectangle(0, 0, 640, 216), color, 0f, Vector2.Zero, 3f * Game1.hiDefScaleOffset, SpriteEffects.None, 0f);
				}
				if (backDropTex != null && !backDropTex.IsDisposed)
				{
					sprite.Draw(backDropTex, this.mapBackPos, new Rectangle(0, 0, 960, 540), color, 0f, Vector2.Zero, 2f * Game1.hiDefScaleOffset, SpriteEffects.None, 0f);
				}
			}
			else if (backDrop == 1)
			{
				sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Rectangle(0, 0, 1, 1), color);
			}
			else
			{
				Game1.graphics.GraphicsDevice.Clear(Color.Black);
			}
			pMan.DrawMapParticles(particleTex, 1f, 2);
		}
	}
}

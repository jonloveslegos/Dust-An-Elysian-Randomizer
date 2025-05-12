using System.Globalization;
using System.IO;
using Dust.Audio;
using Dust.PCClasses;
using Microsoft.Xna.Framework;

namespace Dust.Storage
{
	public class Settings
	{
		private bool rumble;

		private bool autoHeal;

		private bool autoCombo;

		private bool autoLevelUp;

		private bool autoAdvance;

		private bool colorBlind;

		private Vector2 resolution;

		private bool fullScreen;

		private bool depthOfField;

		private bool bloom;

		private bool hiQualityPortraits;

		private bool commentary;

		private bool weatherOn;

		private bool unlockedTime;

		private bool randomizeStartingAbilities;

		private bool[] globalAchievement = new bool[30];

		private int inputMethod;

		public bool Rumble
		{
			get
			{
				return this.rumble;
			}
			set
			{
				this.rumble = value;
			}
		}

		public bool AutoHeal
		{
			get
			{
				return this.autoHeal;
			}
			set
			{
				this.autoHeal = value;
			}
		}

		public bool AutoCombo
		{
			get
			{
				return this.autoCombo;
			}
			set
			{
				this.autoCombo = value;
			}
		}

		public bool AutoLevelUp
		{
			get
			{
				return this.autoLevelUp;
			}
			set
			{
				this.autoLevelUp = value;
			}
		}

		public bool AutoAdvance
		{
			get
			{
				return this.autoAdvance;
			}
			set
			{
				this.autoAdvance = value;
			}
		}

		public bool ColorBlind
		{
			get
			{
				return this.colorBlind;
			}
			set
			{
				this.colorBlind = value;
			}
		}

		public Vector2 Resolution
		{
			get
			{
				return this.resolution;
			}
			set
			{
				this.resolution = value;
			}
		}

		public bool FullScreen
		{
			get
			{
				return this.fullScreen;
			}
			set
			{
				this.fullScreen = value;
			}
		}

		public bool Bloom
		{
			get
			{
				return this.bloom;
			}
			set
			{
				this.bloom = value;
			}
		}

		public bool DepthOfField
		{
			get
			{
				return this.depthOfField;
			}
			set
			{
				this.depthOfField = value;
			}
		}

		public bool HiQualityPortraits
		{
			get
			{
				return this.hiQualityPortraits;
			}
			set
			{
				this.hiQualityPortraits = value;
			}
		}

		public bool RandomizeStartingAbilities
		{
			get
			{
				return this.randomizeStartingAbilities;
			}
			set
			{
				this.randomizeStartingAbilities = value;
			}
		}

		public bool Commentary
		{
			get
			{
				return this.commentary;
			}
			set
			{
				this.commentary = value;
			}
		}

		public bool WeatherOn
		{
			get
			{
				return this.weatherOn;
			}
			set
			{
				this.weatherOn = value;
			}
		}

		public bool UnlockedTime
		{
			get
			{
				return this.unlockedTime;
			}
			set
			{
				this.unlockedTime = value;
			}
		}

		public bool[] GlobalAchievement
		{
			get
			{
				return this.globalAchievement;
			}
			set
			{
				this.globalAchievement = value;
			}
		}

		public int InputMethod
		{
			get
			{
				return this.inputMethod;
			}
			set
			{
				this.inputMethod = value;
			}
		}

		public Settings()
		{
			this.ResetSettings();
			for (int i = 0; i < this.globalAchievement.Length; i++)
			{
				this.globalAchievement[i] = false;
			}
		}

		public void ResetSettings()
		{
			this.rumble = true;
			this.colorBlind = false;
			this.autoHeal = false;
			this.autoCombo = false;
			this.autoLevelUp = false;
			if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
			{
				this.autoAdvance = true;
			}
			else
			{
				this.autoAdvance = false;
			}
			this.fullScreen = true;
			this.depthOfField = true;
			this.bloom = true;
			this.randomizeStartingAbilities = false;
			this.hiQualityPortraits = false;
			this.weatherOn = true;
			this.unlockedTime = false;
			this.commentary = false;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Game1.hud.screenLeftOffset);
			writer.Write(Game1.hud.screenTopOffset);
			writer.Write(Game1.hud.hudScale);
			writer.Write(Game1.hud.hudDetails);
			writer.Write(Sound.masterSFXVolume);
			writer.Write(Sound.masterMusicVolume);
			writer.Write(Game1.stats.saveSlot);
			writer.Write(Game1.stats.manualSaveSlot);
			writer.Write(this.Rumble);
			writer.Write(this.AutoHeal);
			writer.Write(this.AutoCombo);
			writer.Write(this.AutoLevelUp);
			writer.Write(this.AutoAdvance);
			writer.Write(this.ColorBlind);
			writer.Write(this.Resolution.X);
			writer.Write(this.Resolution.Y);
			writer.Write(this.FullScreen);
			writer.Write(this.Bloom);
			writer.Write(this.DepthOfField);
			writer.Write(this.HiQualityPortraits);
			writer.Write(this.WeatherOn);
			writer.Write(this.UnlockedTime);
			writer.Write(this.Commentary);
			this.SyncAchievements();
			for (int i = 0; i < this.globalAchievement.Length; i++)
			{
				writer.Write(this.globalAchievement[i]);
			}
			writer.Write(this.InputMethod);
			writer.Write(this.randomizeStartingAbilities);
			Game1.pcManager.WriteMapping(writer);
		}

		public void Read(BinaryReader reader)
		{
			Game1.hud.screenLeftOffset = reader.ReadInt32();
			Game1.hud.screenTopOffset = reader.ReadInt32();
			Game1.hud.hudScale = reader.ReadSingle();
			Game1.hud.hudDetails = reader.ReadBoolean();
			Sound.masterSFXVolume = reader.ReadSingle();
			Sound.SetSFXVolume(Sound.masterSFXVolume);
			Sound.masterMusicVolume = reader.ReadSingle();
			Game1.stats.saveSlot = reader.ReadByte();
			Game1.stats.manualSaveSlot = reader.ReadByte();
			this.Rumble = reader.ReadBoolean();
			this.AutoHeal = reader.ReadBoolean();
			this.AutoCombo = reader.ReadBoolean();
			this.AutoLevelUp = reader.ReadBoolean();
			this.AutoAdvance = reader.ReadBoolean();
			this.ColorBlind = reader.ReadBoolean();
			this.Resolution = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			this.FullScreen = reader.ReadBoolean();
			if (this.FullScreen != Game1.graphics.IsFullScreen)
			{
				Game1.graphics.ToggleFullScreen();
			}
			Game1.SetResolution((int)this.resolution.X, (int)this.resolution.Y);
			this.Bloom = reader.ReadBoolean();
			this.DepthOfField = reader.ReadBoolean();
			this.HiQualityPortraits = reader.ReadBoolean();
			this.WeatherOn = reader.ReadBoolean();
			this.UnlockedTime = reader.ReadBoolean();
			this.Commentary = reader.ReadBoolean();
			for (int i = 0; i < this.globalAchievement.Length; i++)
			{
				if (reader.ReadBoolean())
				{
					this.globalAchievement[i] = true;
				}
			}
			this.SyncAchievements();
			int num2 = (int)(Game1.pcManager.inputDevice = (InputDevice)(this.InputMethod = reader.ReadInt32()));
			this.RandomizeStartingAbilities = reader.ReadBoolean();
			Game1.pcManager.ReadMapping(reader);
		}

		public void SyncAchievements()
		{
			for (int i = 0; i < Game1.stats.achievementEarned.Length; i++)
			{
				if (Game1.stats.achievementEarned[i])
				{
					this.globalAchievement[i] = true;
				}
			}
		}
	}
}

using System;
using System.IO;

namespace Dust.CharClasses
{
	public class CharDef
	{
		private Animation[] animations;

		private Frame[] frames;

		public string Path;

		public int[] sourceWidth;

		public int[] sourceHeight;

		public int[] maxSprites;

		public int[] maxSpritesHorz;

		public CharacterType charType;

		public byte Sprites_01_Index;

		public byte Sprites_02_Index;

		public byte Sprites_03_Index;

		public byte Sprites_04_Index;

		public byte Sprites_05_Index;

		public byte maxParts;

		public Animation[] Animations => this.animations;

		public Frame[] Frames => this.frames;

		public CharDef(string path, CharacterType _charType)
		{
			this.sourceWidth = new int[5];
			this.sourceHeight = new int[this.sourceWidth.Length];
			this.maxSprites = new int[this.sourceWidth.Length];
			this.maxSpritesHorz = new int[this.sourceWidth.Length];
			for (int i = 0; i < this.sourceWidth.Length; i++)
			{
				this.sourceWidth[i] = 300;
				this.sourceHeight[i] = 300;
				this.maxSprites[i] = 156;
				this.maxSpritesHorz[i] = 13;
			}
			this.charType = _charType;
			this.Path = path;
			this.Read();
		}

		public void Read()
		{
			BinaryReader binaryReader;
			try
			{
				binaryReader = ((!Game1.Xbox360 && !Game1.isPCBuild) ? new BinaryReader(File.Open("../../../../../animations/" + this.Path + ".anm", FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/chars/" + this.Path + ".anm", FileMode.Open, FileAccess.Read)));
			}
			catch (Exception)
			{
				return;
			}
			this.Path = binaryReader.ReadString();
			this.Sprites_01_Index = (byte)binaryReader.ReadInt32();
			this.Sprites_02_Index = (byte)binaryReader.ReadInt32();
			this.Sprites_03_Index = (byte)binaryReader.ReadInt32();
			this.Sprites_04_Index = (byte)binaryReader.ReadInt32();
			this.Sprites_05_Index = (byte)binaryReader.ReadInt32();
			for (int i = 0; i < this.sourceWidth.Length; i++)
			{
				this.sourceWidth[i] = binaryReader.ReadInt32();
				this.sourceHeight[i] = binaryReader.ReadInt32();
				this.maxSprites[i] = binaryReader.ReadInt32();
				this.maxSpritesHorz[i] = binaryReader.ReadInt32();
			}
			int num = binaryReader.ReadInt32();
			this.animations = new Animation[num];
			for (int j = 0; j < num; j++)
			{
				string text = binaryReader.ReadString();
				if (!(text != string.Empty))
				{
					continue;
				}
				this.animations[j] = new Animation(text, binaryReader.ReadBoolean(), binaryReader.ReadInt32());
				for (int k = 0; k < this.animations[j].KeyFrames.Length; k++)
				{
					int num2 = binaryReader.ReadInt32();
					int duration = binaryReader.ReadInt32();
					int num3 = binaryReader.ReadInt32();
					ScriptLine[] array = new ScriptLine[num3];
					for (int l = 0; l < array.Length; l++)
					{
						array[l] = new ScriptLine(binaryReader.ReadString());
					}
					if (num2 <= -1)
					{
						continue;
					}
					this.animations[j].KeyFrames[k] = new KeyFrame(num3);
					KeyFrame keyFrame = this.animations[j].KeyFrames[k];
					keyFrame.FrameRef = num2;
					keyFrame.Duration = duration;
					if (num3 > 0)
					{
						keyFrame.Scripts = new ScriptLine[num3];
						ScriptLine[] scripts = keyFrame.Scripts;
						for (int m = 0; m < scripts.Length; m++)
						{
							scripts[m] = array[m];
						}
					}
				}
			}
			this.maxParts = binaryReader.ReadByte();
			int num4 = binaryReader.ReadInt32();
			this.frames = new Frame[num4];
			for (int n = 0; n < num4; n++)
			{
				string text2 = binaryReader.ReadString();
				if (text2 != string.Empty)
				{
					this.frames[n] = new Frame(this.maxParts);
					for (int num5 = 0; num5 < this.maxParts; num5++)
					{
						Part part = this.frames[n].Parts[num5];
						part.Index = binaryReader.ReadInt32();
						part.Location.X = binaryReader.ReadSingle();
						part.Location.Y = binaryReader.ReadSingle();
						part.Rotation = binaryReader.ReadSingle();
						part.Scaling.X = binaryReader.ReadSingle();
						part.Scaling.Y = binaryReader.ReadSingle();
						part.Flip = (byte)binaryReader.ReadInt32();
					}
					this.frames[n].CheckMaxRender();
				}
			}
			binaryReader.Close();
		}
	}
}

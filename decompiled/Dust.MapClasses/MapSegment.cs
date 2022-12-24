using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class MapSegment
	{
		public byte Index;

		public byte SourceIndex;

		public byte SourceIndexSlot;

		public byte FrameCount;

		public Vector2 Location;

		public Vector2 Scale;

		public float Rotation;

		public Color color;

		public bool FlagEnabled;

		public bool Flip;

		public byte Refractive;

		public MapSegment()
		{
			this.Scale = new Vector2(1f, 1f);
			this.FlagEnabled = true;
		}

		public void PrepareLayerColor(Color layerColor, SegmentDefinition[,] segDef)
		{
			this.FrameCount = 0;
			int flag = segDef[this.SourceIndexSlot, this.Index].Flag;
			if (flag < 100)
			{
				byte frameCount = segDef[this.SourceIndexSlot, this.Index].FrameCount;
				if (frameCount > 1)
				{
					this.FrameCount = (byte)Rand.GetRandomInt(0, frameCount);
				}
			}
			if (flag < 20 || flag >= 30 || !this.FlagEnabled)
			{
				this.Refractive = 0;
			}
			else if (flag >= 28)
			{
				this.Refractive = 2;
			}
			else
			{
				this.Refractive = 1;
			}
			switch (flag)
			{
			case 4:
			case 5:
			case 8:
			case 9:
			case 11:
			case 42:
			case 43:
				return;
			}
			this.color = new Color(this.color.R * layerColor.R / 255, this.color.G * layerColor.G / 255, this.color.B * layerColor.B / 255, this.color.A * layerColor.A / 255);
		}

		public Color MultiplyColor(Color inputColor)
		{
			return new Color(inputColor.R * this.color.R / 255, inputColor.G * this.color.G / 255, inputColor.B * this.color.B / 255, inputColor.A * this.color.A / 255);
		}

		public Color AddColor(Vector4 inputColor)
		{
			return new Color(inputColor.X + (float)(int)this.color.R / 255f, inputColor.Y + (float)(int)this.color.G / 255f, inputColor.Z + (float)(int)this.color.B / 255f, inputColor.W + (float)(int)this.color.A / 255f);
		}
	}
}

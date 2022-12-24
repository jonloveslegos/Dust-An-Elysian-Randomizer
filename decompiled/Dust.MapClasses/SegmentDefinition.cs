using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class SegmentDefinition
	{
		public Rectangle SrcRect;

		public byte Speed;

		public byte Flag;

		public byte FrameCount;

		public int RowOffset;

		public SegmentDefinition(Rectangle _srcRect, byte _flag, byte _speed, byte _frameCount, int _rowOffset)
		{
			this.SrcRect = _srcRect;
			this.Flag = _flag;
			this.Speed = _speed;
			this.FrameCount = _frameCount;
			this.RowOffset = _rowOffset;
		}
	}
}

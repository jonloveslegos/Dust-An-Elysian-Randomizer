using System;

namespace Dust.CharClasses
{
	public class Frame
	{
		private Part[] parts;

		public byte maxRender;

		public Part[] Parts => this.parts;

		public Frame(int partMax)
		{
			this.parts = new Part[partMax];
			for (int i = 0; i < this.parts.Length; i++)
			{
				this.parts[i] = new Part();
			}
		}

		public void CheckMaxRender()
		{
			this.maxRender = 0;
			for (int i = 0; i < this.parts.Length; i++)
			{
				if (this.parts[i].Index > -1)
				{
					this.maxRender = (byte)Math.Max(this.maxRender, i + 1);
				}
			}
		}
	}
}

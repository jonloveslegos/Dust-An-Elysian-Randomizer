namespace Dust.CharClasses
{
	public class KeyFrame
	{
		public int FrameRef;

		public int Duration;

		private ScriptLine[] scripts;

		public ScriptLine[] Scripts
		{
			get
			{
				return this.scripts;
			}
			set
			{
				this.scripts = value;
			}
		}

		public KeyFrame(int maxScripts)
		{
			this.FrameRef = -1;
			this.Duration = 0;
			if (maxScripts > 0)
			{
				this.scripts = new ScriptLine[maxScripts];
			}
		}
	}
}

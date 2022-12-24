namespace Dust.CharClasses
{
	public class Animation
	{
		public string name;

		public bool interpolate;

		private KeyFrame[] keyFrames;

		public KeyFrame[] KeyFrames => this.keyFrames;

		public Animation(string _name, bool _interpolate, int keyFrameCount)
		{
			this.name = _name;
			this.interpolate = _interpolate;
			this.keyFrames = new KeyFrame[keyFrameCount];
		}
	}
}

namespace Dust.MapClasses
{
	public class MapFlags
	{
		private string[] flags;

		public MapFlags(int size)
		{
			this.flags = new string[size];
			for (int i = 0; i < this.flags.Length; i++)
			{
				this.flags[i] = "";
			}
		}

		public bool GetFlag(string flag)
		{
			for (int i = 0; i < this.flags.Length; i++)
			{
				if (this.flags[i] == flag)
				{
					return true;
				}
			}
			return false;
		}

		public void SetFlag(string flag)
		{
			if (this.GetFlag(flag))
			{
				return;
			}
			for (int i = 0; i < this.flags.Length; i++)
			{
				if (this.flags[i] == "")
				{
					this.flags[i] = flag;
					break;
				}
			}
		}
	}
}

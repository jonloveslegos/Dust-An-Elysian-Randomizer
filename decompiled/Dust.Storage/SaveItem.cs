namespace Dust.Storage
{
	public class SaveItem
	{
		private int id;

		private string uniqueID;

		private byte stage;

		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string UniqueID
		{
			get
			{
				return this.uniqueID;
			}
			set
			{
				this.uniqueID = value;
			}
		}

		public byte Stage
		{
			get
			{
				return this.stage;
			}
			set
			{
				this.stage = value;
			}
		}

		public SaveItem(int _id, string _uniqueID, byte _stage)
		{
			this.id = _id;
			this.uniqueID = _uniqueID;
			this.stage = _stage;
		}
	}
}

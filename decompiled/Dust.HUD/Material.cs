using Dust.Strings;

namespace Dust.HUD
{
	public class Material
	{
		public string name;

		protected string descID;

		public int value;

		public string getDescription => Strings_Materials.ResourceManager.GetString(this.descID);

		public Material(string _nameID, string _descID, int _value)
		{
			this.name = _nameID;
			this.descID = _descID;
			this.value = _value;
		}
	}
}

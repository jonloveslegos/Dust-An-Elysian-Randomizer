using Dust.Strings;

namespace Dust.Quests
{
	public class Notes
	{
		public int NoteID = -1;

		public string Name = string.Empty;

		public string Description = string.Empty;

		public Notes(int id)
		{
			this.NoteID = id;
			this.GetDesc();
		}

		private void GetDesc()
		{
			this.Name = Strings_Notes.ResourceManager.GetString($"{this.NoteID:D3}");
			this.Description = Strings_Notes.ResourceManager.GetString($"{this.NoteID:D3}" + "_Desc");
			if (this.Description != null)
			{
				this.Description = Game1.smallText.WordWrap(this.Description, 0.8f, 360f, TextAlign.Left);
			}
		}
	}
}

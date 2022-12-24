using System.Collections.Generic;

namespace Dust.HUD
{
	public class TextBox
	{
		public int ID;

		public TextBoxType textBoxType;

		public string lineKey;

		public int portraitID;

		public bool portraitFlip;

		public int startConversation = -1;

		public int flag_0 = -1;

		public int flag_1 = -1;

		public string flag_string = string.Empty;

		public bool global;

		public byte random = (byte)Rand.GetRandomInt(0, 100);

		public string[] responseKey;

		public int[] responseTarget;

		public List<byte> visualizationData;

		public TextBox(int _ID, TextBoxType _textBoxType)
		{
			this.ID = _ID;
			this.textBoxType = _textBoxType;
			this.visualizationData = new List<byte>();
		}
	}
}

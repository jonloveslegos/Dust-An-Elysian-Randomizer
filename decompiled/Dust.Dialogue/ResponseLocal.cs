using Dust.HUD;

namespace Dust.Dialogue
{
	public class ResponseLocal
	{
		public string responseKey;

		public string responseString;

		public float reponseOffset;

		public int responseTarget;

		public int responseHeight;

		public ResponseLocal(string _responseKey, int _responseTarget, DialogueScript dialogueScript, float dialogueTextSize, int windowWidth)
		{
			this.responseKey = _responseKey;
			if (this.responseKey != null)
			{
				this.responseString = Game1.bigText.WordWrap(dialogueScript.manager.GetString(this.responseKey), dialogueTextSize, windowWidth, TextAlign.Left);
				this.responseHeight = (int)(Game1.bigFont.MeasureString(this.responseString).Y * dialogueTextSize * 0.9f);
			}
			this.reponseOffset = 0f;
			this.responseTarget = _responseTarget;
		}
	}
}

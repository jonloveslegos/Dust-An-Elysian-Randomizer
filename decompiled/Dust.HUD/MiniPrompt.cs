using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Dust.HUD
{
	public class MiniPrompt
	{
		public string rawText;

		public string text;

		public int ID;

		public int displayTime;

		public int multiplier;

		public int questID = -1;

		public int audio = -1;

		public Dictionary<Vector3, string> textButtonList = new Dictionary<Vector3, string>();

		public MiniPrompt(string _text, int _ID, int _displayTime)
		{
			this.rawText = _text;
			if (_ID >= 2000)
			{
				this.ID = _ID;
				this.text = Game1.smallText.WordWrap(_text, 0.8f, Game1.screenWidth - Game1.hud.screenLeftOffset * 2, this.textButtonList, TextAlign.LeftAndCenter);
				if (_ID < 10000)
				{
					this.questID = this.ID;
					this.audio = this.ID / 1000;
				}
			}
			else
			{
				this.ID = short.Parse($"{_ID:D4}");
				this.text = Game1.smallText.WordWrap("[BACK] " + _text + "  [ICON_" + $"{_ID:D4}" + "]", 0.8f, Game1.screenWidth - Game1.hud.screenLeftOffset * 2, this.textButtonList, TextAlign.LeftAndCenter);
			}
			this.displayTime = _displayTime;
			this.multiplier = 1;
		}

		public void AddMultiplier()
		{
			this.multiplier++;
			this.text = Game1.smallText.WordWrap("[BACK] " + this.rawText + "  [ICON_" + $"{this.ID:D4}" + "] [MULTIPLY] " + this.multiplier, 0.8f, Game1.screenWidth - Game1.hud.screenLeftOffset * 2, this.textButtonList, TextAlign.LeftAndCenter);
		}
	}
}

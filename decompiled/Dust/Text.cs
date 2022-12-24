using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dust
{
	public class Text
	{
		private float alignPos;

		private float buttonScale;

		private float buttonWordCenter;

		private float alpha = 1f;

		private Color color = Color.White;

		private static Texture2D[] particleTex;

		private Vector2 Position = Vector2.Zero;

		private SpriteFont font;

		private SpriteFont buttonFont;

		private SpriteBatch sprite;

		private static Dictionary<string, string> textSymbols = new Dictionary<string, string>();

		private static Dictionary<string, string> buttonGlyphs = new Dictionary<string, string>();

		private static List<string> punctuation = new List<string>();

		private static object syncObject = new object();

		private int spacesLeftCentered;

		private string buttonSpace;

		public SpriteFont Font => this.font;

		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.alpha = (float)(int)this.color.A / 255f;
			}
		}

		public Text(ContentManager content, SpriteBatch _sprite, SpriteFont _font, SpriteFont _buttonFont, Texture2D[] _particleTex)
		{
			this.font = _font;
			this.buttonFont = _buttonFont;
			this.sprite = _sprite;
			Text.particleTex = _particleTex;
			this.buttonScale = (float)this.font.LineSpacing / 40f;
			Text.textSymbols = new Dictionary<string, string>();
			Text.textSymbols.Add("SkipLine", "[N]");
			Text.textSymbols.Add("[", "[");
			Text.textSymbols.Add("]", "]");
			Text.textSymbols.Add("Image", "[IMAGE");
			Text.textSymbols.Add("BookMark", "[BOOKMARK");
			Text.buttonGlyphs = new Dictionary<string, string>();
			Text.buttonGlyphs.Add("[LTHUMB]", " ");
			Text.buttonGlyphs.Add("[RTHUMB]", "\"");
			Text.buttonGlyphs.Add("[DPAD]", "!");
			Text.buttonGlyphs.Add("[BACK]", "#");
			Text.buttonGlyphs.Add("[GUIDE]", "$");
			Text.buttonGlyphs.Add("[START]", "%");
			Text.buttonGlyphs.Add("[X]", "&");
			Text.buttonGlyphs.Add("[Y]", "(");
			Text.buttonGlyphs.Add("[A]", "'");
			Text.buttonGlyphs.Add("[B]", ")");
			Text.buttonGlyphs.Add("[LB]", "-");
			Text.buttonGlyphs.Add("[RB]", "*");
			Text.buttonGlyphs.Add("[RT]", "+");
			Text.buttonGlyphs.Add("[LT]", ",");
			Text.buttonGlyphs.Add("[UP]", ".");
			Text.buttonGlyphs.Add("[DOWN]", "/");
			Text.buttonGlyphs.Add("[MULTIPLY]", "0");
			Text.buttonGlyphs.Add("[KEY]", "1");
			Text.buttonGlyphs.Add("[MOUSEL]", "2");
			Text.buttonGlyphs.Add("[MOUSER]", "3");
			Text.buttonGlyphs.Add("[MOUSEM]", "4");
			Text.buttonGlyphs.Add("[MAP]", "5");
			Text.punctuation.Add(",");
			Text.punctuation.Add(".");
			Text.punctuation.Add("'");
			Text.punctuation.Add("(");
			Text.punctuation.Add(")");
			Text.punctuation.Add("!");
			this.GetDimensions();
		}

		private void GetDimensions()
		{
			this.buttonSpace = string.Empty;
			int num = 0;
			do
			{
				num++;
				this.buttonSpace += " ";
			}
			while (this.font.MeasureString(this.buttonSpace).X < 50f * this.buttonScale);
			string text = string.Empty;
			for (int i = 0; i < num - 1; i++)
			{
				text += " ";
			}
			this.buttonWordCenter = this.font.MeasureString(text).X / 2f;
		}

		public string WordWrap(string s, float size, float width, TextAlign align)
		{
			if (s == null)
			{
				return "";
			}
			return this.WordWrap(s, size, width, null, align);
		}

		public string WordWrap(string s, float size, float width, Dictionary<Vector3, string> buttonList, TextAlign align)
		{
			if (s == null)
			{
				return "";
			}
			lock (Text.syncObject)
			{
				bool flag = CultureInfo.CurrentCulture.EnglishName.StartsWith("Japanese");
				if (s != "" && flag)
				{
					s = this.PopulateJapaneseArray(this.font, s, width / size);
				}
				this.spacesLeftCentered = 10000;
				string text = string.Empty;
				string text2 = string.Empty;
				string[] array = this.PopulateWordArray(s, hasButtons: true).Split(' ');
				int num = 0;
				int num2 = 0;
				_ = string.Empty;
				buttonList?.Clear();
				List<string> list = new List<string>();
				List<Vector3> list2 = new List<Vector3>();
				List<Vector2> list3 = new List<Vector2>();
				Vector2 vector = new Vector2(this.buttonWordCenter, this.font.LineSpacing / 2) * size;
				float spaceWidth = (this.font.MeasureString(" ").X + this.font.Spacing) * size;
				_ = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].StartsWith(Text.textSymbols["SkipLine"]))
					{
						text = this.CenterLine(size, width, align, text, num2, list3, spaceWidth);
						if (i < array.Length)
						{
							text2 = text2 + text + "\n";
							text = string.Empty;
							num2++;
						}
					}
					else
					{
						string text3 = text;
						if (!array[i].StartsWith("["))
						{
							text3 += array[i];
						}
						else if (text != " ")
						{
							text3 += " ";
						}
						else
						{
							text = string.Empty;
						}
						if (this.font.MeasureString(text3).X * size > width)
						{
							text = this.CenterLine(size, width, align, text, num2, list3, spaceWidth);
							text2 = text2 + text + "\n";
							text = string.Empty;
							num2++;
						}
						if (array[i].StartsWith("["))
						{
							if (array[i] != "[NOSPACE]" && buttonList != null)
							{
								float num3 = this.font.MeasureString(text).X;
								if (this.font.MeasureString(text + this.buttonSpace).X * size > width)
								{
									text = this.CenterLine(size, width, align, text, num2, list3, spaceWidth);
									text2 = text2 + text + "\n";
									text = this.buttonSpace;
									num3 = 0f;
									num2++;
								}
								else
								{
									text += this.buttonSpace;
								}
								list.Add(this.GetButtonName(array[i]));
								list2.Add(new Vector3(num3 * size + vector.X, (float)(this.font.LineSpacing * num2) * size + vector.Y, num));
								num++;
							}
							else if (text.Length > 0)
							{
								text = text.Remove(text.Length - 1, 1);
							}
						}
						else
						{
							if (text == " " && array[i] != string.Empty)
							{
								text = string.Empty;
							}
							text = text + array[i] + ' ';
						}
					}
					if (i < array.Length)
					{
						_ = array[i];
					}
				}
				text = this.CenterLine(size, width, align, text, num2, list3, spaceWidth);
				text2 += text;
				float num4 = 0f;
				if (align == TextAlign.LeftAndCenter)
				{
					string text4 = string.Empty;
					for (int j = 0; j < this.spacesLeftCentered; j++)
					{
						text4 += " ";
					}
					num4 = (0f - this.font.MeasureString(text4).X) * size;
					text2 = text2.Remove(0, text4.Count());
					text2 = text2.Replace("\n" + text4, "\n");
				}
				for (int k = 0; k < list.Count; k++)
				{
					Vector3 key = list2[k];
					for (int l = 0; l < list3.Count; l++)
					{
						if ((int)list2[k].Y == (int)((float)this.font.LineSpacing * size * list3[l].Y + vector.Y))
						{
							key = new Vector3(list2[k].X + list3[l].X + num4, list2[k].Y, list2[k].Z);
						}
					}
					buttonList.Add(key, list[k]);
				}
				if (flag)
				{
					while (text2.EndsWith("\n"))
					{
						text2 = text2.Remove(text2.Length - 1, 1);
					}
				}
				return text2;
			}
		}

		public Dictionary<float, string> scrollWrap(string s, float size, float width, Dictionary<Vector3, string> buttonList, Dictionary<Rectangle, Vector2> imageList, Dictionary<Vector2, string> bookMarks, float imageSize)
		{
			lock (Text.syncObject)
			{
				bool flag = CultureInfo.CurrentCulture.EnglishName.StartsWith("Japanese");
				if (s != "" && flag)
				{
					s = this.PopulateJapaneseArray(this.font, s, width / size);
				}
				Dictionary<float, string> dictionary = new Dictionary<float, string>();
				string text = string.Empty;
				string text2 = string.Empty;
				string[] array = this.PopulateWordArray(s, hasButtons: true).Split(' ');
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 40;
				int num5 = 0;
				float key = 0f;
				float num6 = (float)this.font.LineSpacing * size;
				string empty = string.Empty;
				Vector2 vector = new Vector2(this.buttonWordCenter, this.font.LineSpacing / 2) * size;
				buttonList?.Clear();
				imageList?.Clear();
				bookMarks?.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].StartsWith(Text.textSymbols["Image"]))
					{
						string[] array2 = array[i].Split('_', ']', ',');
						int num7 = int.Parse(array2[1].Substring(array2[1].Length - 1));
						if (num7 > 0)
						{
							text2 = text2 + text + "\n";
							text = string.Empty;
							num2++;
							num3++;
							if (num3 > num4)
							{
								num3 = 0;
								dictionary.Add(key, text2);
								key = (float)num2 * num6;
								text2 = string.Empty;
							}
						}
						if (array2[5] == null)
						{
							continue;
						}
						Rectangle key2 = new Rectangle(int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]));
						imageList.Add(key2, new Vector2(num7, (float)num2 * num6));
						int num8 = (int)((float)key2.Height * imageSize * Math.Min(Game1.hiDefScaleOffset, 1f) / num6);
						int num9 = 0;
						if (num7 > 0)
						{
							num9 = 3;
						}
						for (int j = 0; j < num8 + num9; j++)
						{
							text2 = text2 + text + "\n";
							text = string.Empty;
							num2++;
							num3++;
							if (num3 > num4)
							{
								num3 = 0;
								dictionary.Add(key, text2);
								key = (float)num2 * num6;
								text2 = string.Empty;
							}
						}
						continue;
					}
					if (array[i].StartsWith(Text.textSymbols["BookMark"]))
					{
						string[] array3 = array[i].Split('_', ']');
						if (array3[1] != null)
						{
							string text3 = string.Empty;
							for (int k = 1; k < array3.Length - 2; k++)
							{
								text3 = text3 + array3[k] + " ";
							}
							bookMarks.Add(new Vector2(num5, (float)num2 * num6), text3);
							num5++;
							text2 = text2 + text + "\n";
							text = string.Empty;
							num2++;
							num3++;
							if (num3 > num4)
							{
								num3 = 0;
								dictionary.Add(key, text2);
								key = (float)num2 * num6;
								text2 = string.Empty;
							}
						}
						continue;
					}
					if (array[i].StartsWith(Text.textSymbols["SkipLine"]))
					{
						text2 = text2 + text + "\n";
						text = string.Empty;
						num2++;
						num3++;
						if (num3 > num4)
						{
							num3 = 0;
							dictionary.Add(key, text2);
							key = (float)num2 * num6;
							text2 = string.Empty;
						}
						continue;
					}
					string text4 = text;
					if (!array[i].StartsWith("["))
					{
						text4 += array[i];
					}
					else if (text != " ")
					{
						text4 += " ";
					}
					else
					{
						text = string.Empty;
					}
					if (this.font.MeasureString(text4).X * size > width)
					{
						text2 = text2 + text + "\n";
						text = string.Empty;
						num2++;
						num3++;
						if (num3 > num4)
						{
							num3 = 0;
							dictionary.Add(key, text2);
							key = (float)num2 * num6;
							text2 = string.Empty;
						}
					}
					if (array[i].StartsWith("["))
					{
						if (array[i] != "[NOSPACE]" && buttonList != null)
						{
							empty = this.GetButtonName(array[i]);
							buttonList.Add(new Vector3(this.font.MeasureString(text).X * size + vector.X, (float)this.font.LineSpacing * size * (float)num2 + vector.Y, num), empty);
							num++;
							text += this.buttonSpace;
						}
						else if (text.Length > 0)
						{
							text = text.Remove(text.Length - 1, 1);
						}
					}
					else
					{
						if (text == " " && array[i] != string.Empty)
						{
							text = string.Empty;
						}
						text = text + array[i] + ' ';
					}
				}
				text2 += text;
				dictionary.Add(key, text2);
				return dictionary;
			}
		}

		private string PopulateWordArray(string line, bool hasButtons)
		{
			string result;
			lock (Text.syncObject)
			{
				bool flag2 = CultureInfo.CurrentCulture.EnglishName.StartsWith("Japanese");
				line = this.ReplaceWords(line);
				string[] array = line.Split(new char[]
				{
					'[',
					']'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == string.Empty)
					{
						array[i] = null;
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != null && (Text.buttonGlyphs.ContainsKey("[" + array[j] + "]") || array[j] == "N" || array[j].Contains("IMAGE") || array[j].Contains("BOOKMARK") || array[j].Contains("ICON_")))
					{
						if (array[j].Contains("BOOKMARK"))
						{
							string[] array2 = array[j].Split(new char[]
							{
								' '
							});
							array[j] = string.Empty;
							for (int k = 0; k < array2.Length; k++)
							{
								string[] array3;
								IntPtr intPtr;
								(array3 = array)[(int)(intPtr = (IntPtr)j)] = array3[(int)intPtr] + array2[k] + "_";
							}
						}
						array[j] = "[" + array[j] + "]";
						if (array[j + 1] != null && !array[j + 1].StartsWith(" "))
						{
							bool flag3 = true;
							for (int l = 0; l < Text.punctuation.Count; l++)
							{
								if (array[j + 1].Contains(Text.punctuation[l]))
								{
									flag3 = false;
								}
							}
							if (flag2)
							{
								flag3 = true;
							}
							if (flag3 || !hasButtons)
							{
								string[] array4;
								IntPtr intPtr2;
								(array4 = array)[(int)(intPtr2 = (IntPtr)j)] = array4[(int)intPtr2] + " ";
							}
							else
							{
								string[] array5;
								IntPtr intPtr3;
								(array5 = array)[(int)(intPtr3 = (IntPtr)j)] = array5[(int)intPtr3] + " [NOSPACE] ";
							}
						}
						if (array[j - 1] != null && !array[j - 1].EndsWith(" "))
						{
							bool flag4 = true;
							for (int m = 0; m < Text.punctuation.Count; m++)
							{
								if (array[j - 1].Contains(Text.punctuation[m]))
								{
									flag4 = false;
								}
							}
							if (flag2)
							{
								flag4 = true;
							}
							if (flag4 || !hasButtons)
							{
								array[j] = " " + array[j];
							}
							else
							{
								array[j] = " [NOSPACE] " + array[j];
							}
						}
						if (hasButtons && j - 2 > -1 && array[j - 2] != null && array[j - 2].Contains('['))
						{
							array[j - 1] = " [NOSPACE] " + array[j - 1];
						}
					}
				}
				string text = string.Empty;
				for (int n = 0; n < array.Length; n++)
				{
					if (array[n] != null)
					{
						text += array[n];
					}
					else
					{
						text += " ";
					}
				}
				result = text;
			}
			return result;
		}


		private string CenterLine(float size, float width, TextAlign align, string line, int curLine, List<Vector2> buttonLineOffset, float spaceWidth)
		{
			int num = 0;
			if ((align == TextAlign.Center || align == TextAlign.LeftAndCenter) && width > 0f)
			{
				float num2 = this.font.MeasureString(line).X * size;
				float num3 = (width - num2) / 2f;
				num = (int)Math.Max(num3 / spaceWidth, 0f);
				for (int i = 0; i < num; i++)
				{
					line = " " + line;
				}
				if (num < this.spacesLeftCentered)
				{
					this.spacesLeftCentered = num;
				}
			}
			buttonLineOffset.Add(new Vector2((float)num * spaceWidth, curLine));
			return line;
		}

		private string ReplaceWords(string line)
		{
			if (line == null)
			{
				return line;
			}
			line = line.Replace("\n", "[N]");
			line = line.Replace("\r", "");
			if (line.Contains("[REVSTONES]"))
			{
				line = line.Replace("[REVSTONES]", Game1.stats.Equipment[301].ToString());
			}
			if (line.Contains("[TELSTONES]"))
			{
				line = line.Replace("[TELSTONES]", Game1.stats.Equipment[300].ToString());
			}
			if (line.Contains("[CURRENTDIFFICULTY]") && Game1.menu != null)
			{
				line = line.Replace("[CURRENTDIFFICULTY]", Strings_Options.ResourceManager.GetString("Difficulty" + Game1.menu.prevDifficulty));
			}
			if (Game1.isPCBuild && Game1.pcManager != null && Game1.pcManager.inputDevice != 0)
			{
				line = line.Replace("[A]", Game1.pcManager.inputKeyList["a"].MappedUsingString(parantheses: true));
				line = line.Replace("[B]", Game1.pcManager.inputKeyList["b"].MappedUsingString(parantheses: true));
				line = line.Replace("[X]", Game1.pcManager.inputKeyList["x"].MappedUsingString(parantheses: true));
				line = line.Replace("[Y]", Game1.pcManager.inputKeyList["y"].MappedUsingString(parantheses: true));
				line = line.Replace("[UP]", Game1.pcManager.inputKeyList["up"].MappedUsingString(parantheses: true));
				line = line.Replace("[DOWN]", Game1.pcManager.inputKeyList["down"].MappedUsingString(parantheses: true));
				line = line.Replace("[LEFT]", Game1.pcManager.inputKeyList["left"].MappedUsingString(parantheses: true));
				line = line.Replace("[RIGHT]", Game1.pcManager.inputKeyList["right"].MappedUsingString(parantheses: true));
				line = line.Replace("[BACK]", Game1.pcManager.inputKeyList["back"].MappedUsingString(parantheses: true));
				line = line.Replace("[START]", Game1.pcManager.inputKeyList["start"].MappedUsingString(parantheses: true));
				line = line.Replace("[RB]", Game1.pcManager.inputKeyList["rb"].MappedUsingString(parantheses: true));
				line = line.Replace("[LB]", Game1.pcManager.inputKeyList["lb"].MappedUsingString(parantheses: true));
				line = line.Replace("[RT]", Game1.pcManager.inputKeyList["rt"].MappedUsingString(parantheses: true));
				line = line.Replace("[LT]", Game1.pcManager.inputKeyList["lt"].MappedUsingString(parantheses: true));
				line = line.Replace("[INVLEFT]", Game1.pcManager.inputKeyList["invleft"].MappedUsingString(parantheses: true));
				line = line.Replace("[INVRIGHT]", Game1.pcManager.inputKeyList["invright"].MappedUsingString(parantheses: true));
				line = line.Replace("[INVSUBLEFT]", Game1.pcManager.inputKeyList["invsubleft"].MappedUsingString(parantheses: true));
				line = line.Replace("[INVSUBRIGHT]", Game1.pcManager.inputKeyList["invsubright"].MappedUsingString(parantheses: true));
				line = line.Replace("[RTHUMB]", Game1.pcManager.inputKeyList["rclick"].MappedUsingString(parantheses: true));
				line = line.Replace("[MAP]", Game1.pcManager.inputKeyList["rclick"].MappedUsingString(parantheses: true));
				line = line.Replace("[LTHUMB]", "'" + Game1.pcManager.inputKeyList["up"].MappedUsingString(parantheses: false) + ", " + Game1.pcManager.inputKeyList["left"].MappedUsingString(parantheses: false) + ", " + Game1.pcManager.inputKeyList["down"].MappedUsingString(parantheses: false) + ", " + Game1.pcManager.inputKeyList["right"].MappedUsingString(parantheses: false) + "'");
			}
			return line;
		}

		private static uint MyGetCharWidth(SpriteFont spriteFont, char c)
		{
			return (uint)spriteFont.MeasureString(new string(c, 1)).X;
		}

		private string PopulateJapaneseArray(SpriteFont spriteFont, string text, float width)
		{
			Dust.WordWrap.SetCallback(MyGetCharWidth, null);
			string text2 = text;
			string text3 = null;
			do
			{
				string EOL = null;
				int EOLOffset;
				string text4 = Dust.WordWrap.FindNextLine(spriteFont, text2, (uint)width, out EOL, out EOLOffset);
				int num = ((EOL != null) ? (EOLOffset + 1) : 0);
				if (num != 0)
				{
					string text5 = text2.Substring(0, num);
					text3 = text3 + text5 + "\n";
				}
				text2 = text4;
			}
			while (text2 != null);
			if (text3 != null)
			{
				while (text3.EndsWith("\n"))
				{
					text3 = text3.Remove(text3.Length - 1, 1);
				}
			}
			else
			{
				text3 = "";
			}
			return text3;
		}

		private string GetIconName(string word)
		{
			lock (Text.syncObject)
			{
				string[] array = word.Split(']');
				string[] array2;
				(array2 = array)[0] = array2[0] + "]";
				return array[0];
			}
		}

		private string GetButtonName(string word)
		{
			string[] array = word.Split(']');
			string[] array2;
			(array2 = array)[0] = array2[0] + "]";
			if (Text.buttonGlyphs.ContainsKey(array[0]))
			{
				array[0] = Text.buttonGlyphs[array[0]];
			}
			else if (array[0].Contains("ICON_"))
			{
				array = array[0].Split('_');
				array = array[1].Split(']');
			}
			return array[0];
		}

		private void DrawButtons(Dictionary<Vector3, string> buttonList, Vector2 pos, float scale, bool bounce)
		{
			this.DrawButtons(buttonList, pos, new Vector2(scale, scale), bounce);
		}

		private void DrawButtons(Dictionary<Vector3, string> buttonList, Vector2 pos, Vector2 scale, bool bounce)
		{
			lock (Text.syncObject)
			{
				Vector2 vector = this.buttonScale * scale;
				Vector2 vector2 = this.buttonFont.MeasureString(" ");
				Vector2 origin = new Vector2(25f, 25f);
				float num = (float)Math.Sin(Game1.hud.pulse) * (vector2.X / 36f);
				float num2 = (float)Math.Cos(Game1.hud.pulse) * (vector2.X / 36f);
				Vector2 vector3 = new Vector2(num * 2f, 0f - Math.Abs(num2 * 4f)) * vector;
				try
				{
					foreach (KeyValuePair<Vector3, string> button in buttonList)
					{
						Vector2 vector4 = new Vector2(button.Key.X, button.Key.Y) + pos;
						if (button.Value.Length > 1)
						{
							int num3 = short.Parse(button.Value);
							int num4 = ((num3 >= 1000 && num3 < 2000) ? 5 : 4);
							while (num3 >= 1000)
							{
								num3 -= 1000;
							}
							int num5 = ((num4 == 5) ? 12 : Game1.inventoryManager.invSelMax);
							int num6 = num3 / num5;
							Rectangle value = new Rectangle(num6 * 60, (num3 - num6 * num5) * 60, 60, 60);
							this.sprite.Draw(Text.particleTex[num4], vector4, value, this.color, 0f, new Vector2(30f, 30f), vector * 0.87f, SpriteEffects.None, 0f);
						}
						else if (bounce)
						{
							this.sprite.DrawString(this.buttonFont, button.Value, vector4 + vector3, this.color, (0f - num) * 0.05f, origin, vector, SpriteEffects.None, 0f);
						}
						else
						{
							this.sprite.DrawString(this.buttonFont, button.Value, vector4, this.color, 0f, origin, vector, SpriteEffects.None, 0f);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public void DrawText(Vector2 pos, string s, float size)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, Vector2.Zero, size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawText(Vector2 pos, string s, float size, float width, TextAlign align)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
			if (width == 0f)
			{
				this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, Vector2.Zero, size, SpriteEffects.None, 1f);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				return;
			}
			switch (align)
			{
			case TextAlign.Center:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X / 2f;
					pos.X += (int)(width / 2f);
				}
				break;
			case TextAlign.Right:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X;
				}
				break;
			default:
				this.alignPos = 0f;
				break;
			}
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawText(Vector2 pos, string s, float size, Vector2 newSize, float width, TextAlign align)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			if (width == 0f)
			{
				this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, Vector2.Zero, newSize, SpriteEffects.None, 1f);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				return;
			}
			switch (align)
			{
			case TextAlign.Center:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X / 2f;
					pos.X += (int)(width / 2f);
				}
				break;
			case TextAlign.Right:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X;
				}
				break;
			default:
				this.alignPos = 0f;
				break;
			}
			Color color = this.color;
			this.color = Color.Black;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					this.sprite.DrawString(this.font, s, pos + new Vector2((float)(3 * i) - 1.5f, (float)(3 * j) - 1.5f), this.color * this.alpha, 0f, new Vector2(this.alignPos, this.font.MeasureString(s).Y / 2f), newSize, SpriteEffects.None, 1f);
				}
			}
			this.color = color;
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, this.font.MeasureString(s).Y / 2f), newSize, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawOutlineText(Vector2 pos, string s, float size, int width, TextAlign align, bool fullOutline)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			if (width == 0)
			{
				this.alignPos = 0f;
			}
			else
			{
				switch (align)
				{
				case TextAlign.Center:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X / 2f;
						pos.X += width / 2;
					}
					break;
				case TextAlign.Right:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X;
					}
					break;
				default:
					this.alignPos = 0f;
					break;
				}
			}
			Color color = this.Color;
			this.Color = new Color(0f, 0f, 0f, (float)(int)this.Color.A / 255f);
			if (fullOutline)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						this.sprite.DrawString(this.font, s, pos + new Vector2((float)(3 * i) - 1.5f, (float)(3 * j) - 1.5f), this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
					}
				}
			}
			else
			{
				Vector2 value;
				switch (Game1.longSkipFrame)
				{
					case 1:
						value = new Vector2(1.5f, -1.5f);
						break;
					case 2:
						value = new Vector2(-1.5f, 1.5f);
						break;
					case 3:
						value = new Vector2(1.5f, 1.5f);
						break;
					default:
						value = new Vector2(-1.5f, -1.5f);
						break;
				}
				this.sprite.DrawString(this.font, s, pos + value, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			}
			this.Color = color;
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawButtonOutlineText(Vector2 pos, string s, float size, Dictionary<Vector3, string> buttonList, bool bounce, float width, TextAlign align)
		{
			Color color = this.Color;
			this.Color = Color.Black * this.alpha;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					this.DrawButtonText(pos + new Vector2((float)(3 * i) - 1.5f, (float)(3 * j) - 1.5f), s, size, buttonList, bounce, width, align);
				}
			}
			this.Color = color;
			this.DrawButtonText(pos, s, size, buttonList, bounce, width, align);
		}

		public void DrawShadowTextOld(Vector2 pos, string s, float size, int width, TextAlign align)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			if (width == 0)
			{
				this.alignPos = 0f;
			}
			else
			{
				switch (align)
				{
				case TextAlign.Center:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X / 2f;
						pos.X += width / 2;
					}
					break;
				case TextAlign.Right:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X;
					}
					break;
				default:
					this.alignPos = 0f;
					break;
				}
			}
			Color color = this.Color;
			this.Color = new Color(0f, 0f, 0f, (float)(int)this.Color.A / 255f);
			this.sprite.DrawString(this.font, s, pos + new Vector2(3f, 3f), this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 1; j++)
				{
					this.sprite.DrawString(this.font, s, pos + new Vector2((float)(3 * i) - 1.5f, (float)(3 * j) - 1.5f), this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
				}
			}
			this.Color = color;
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawShadowText(Vector2 pos, string s, float size, int width, TextAlign align, bool outline)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			if (width == 0)
			{
				this.alignPos = 0f;
			}
			else
			{
				switch (align)
				{
				case TextAlign.Center:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X / 2f;
						pos.X += width / 2;
					}
					break;
				case TextAlign.Right:
					if (this.font.MeasureString(s).X * size < (float)width)
					{
						this.alignPos = this.font.MeasureString(s).X;
					}
					break;
				default:
					this.alignPos = 0f;
					break;
				}
			}
			Color color = this.Color;
			this.Color = new Color(0f, 0f, 0f, (float)(int)this.Color.A / 255f);
			this.sprite.DrawString(this.font, s, pos + new Vector2(3f, 3f), this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			if (outline)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						this.sprite.DrawString(this.font, s, pos + new Vector2((float)(3 * i) - 1.5f, (float)(4 * j) - 1.5f), this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
					}
				}
			}
			this.Color = color;
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		}

		public void DrawButtonText(Vector2 pos, string s, float size, Dictionary<Vector3, string> buttonList, bool bounce, float width, TextAlign align)
		{
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
			this.Position = pos;
			if (width == 0f)
			{
				this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, Vector2.Zero, size, SpriteEffects.None, 1f);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				if (buttonList.Count > 0)
				{
					this.DrawButtons(buttonList, pos, size, bounce);
				}
				return;
			}
			switch (align)
			{
			case TextAlign.Center:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X / 2f;
					pos.X += (int)(width / 2f);
				}
				break;
			case TextAlign.Right:
				if (this.font.MeasureString(s).X * size < width)
				{
					this.alignPos = this.font.MeasureString(s).X;
				}
				break;
			default:
				this.alignPos = 0f;
				break;
			}
			this.sprite.DrawString(this.font, s, pos, this.color * this.alpha, 0f, new Vector2(this.alignPos, 0f), size, SpriteEffects.None, 1f);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			if (buttonList.Count > 0)
			{
				this.DrawButtons(buttonList, pos - new Vector2(this.alignPos, 0f) * size, size, bounce);
			}
		}

		public void DrawScrollText(Vector2 pos, Dictionary<float, string> s, Dictionary<Vector3, string> buttonList, Dictionary<Rectangle, Vector2> imageList, Dictionary<Vector2, string> bookMarkList, float size, float topEdge, float scrollHeight, float lineWidth, float imageSize)
		{
			this.Position = pos;
			float num = (float)this.font.LineSpacing * size;
			new Vector2(25f, 25f);
			foreach (KeyValuePair<Rectangle, Vector2> image in imageList)
			{
				int num2 = 0;
				if (image.Value.X > 0f)
				{
					num2 = 40;
				}
				if (image.Value.Y + topEdge > (float)(-image.Key.Height - 80 - num2) && image.Value.Y + topEdge < scrollHeight + (float)num2)
				{
					Game1.menu.DrawScrollImage(pos + new Vector2(0f, image.Value.Y), new Rectangle(image.Key.X, image.Key.Y, image.Key.Width, image.Key.Height), new Rectangle((int)pos.X, (int)(pos.Y - topEdge), 500, (int)scrollHeight), imageSize, this.color, lineWidth, (int)image.Value.X);
				}
			}
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
			foreach (KeyValuePair<float, string> item in s)
			{
				float num3 = item.Key + topEdge;
				bool flag = true;
				if (num3 + num * 40f < 0f || num3 > scrollHeight)
				{
					flag = false;
				}
				if (flag)
				{
					this.sprite.DrawString(this.font, item.Value, pos + new Vector2(0f, item.Key), this.color * this.alpha, 0f, Vector2.Zero, size, SpriteEffects.None, 1f);
				}
			}
			foreach (KeyValuePair<Vector2, string> bookMark in bookMarkList)
			{
				bool flag2 = true;
				if (bookMark.Key.Y + topEdge + num * 3f < 0f || bookMark.Key.Y + topEdge > scrollHeight)
				{
					flag2 = false;
				}
				if (flag2)
				{
					this.sprite.DrawString(this.font, bookMark.Value, pos + new Vector2(lineWidth / 2f, bookMark.Key.Y), this.color * this.alpha, 0f, new Vector2(this.font.MeasureString(bookMark.Value).X / 2f, 0f), size * 1.5f, SpriteEffects.None, 1f);
				}
			}
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			this.DrawButtons(buttonList, pos, this.buttonScale * size, bounce: false);
		}
	}
}

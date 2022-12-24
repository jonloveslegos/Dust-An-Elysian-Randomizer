using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.CharClasses
{
	public class PartsSnapshot
	{
		public int Index;

		public Texture2D texture;

		public Rectangle sRect;

		public PartsSnapshot()
		{
			this.Index = -1;
			this.texture = null;
		}

		public bool GetSource(int _index, CharDef Definition, int[] textureRange, int randomSkin, int randomWeapon, int maxAnimFrames, int curAnimFrame, Texture2D[] sprites_01_Tex, Texture2D[] sprites_02_Tex, Texture2D[] sprites_03_Tex, Texture2D[] sprites_04_Tex, Texture2D[] sprites_05_Tex)
		{
			bool result = false;
			this.Index = _index;
			this.texture = null;
			int index = this.Index;
			int num = index;
			if (index < Definition.maxSprites[0])
			{
				this.texture = sprites_01_Tex[Definition.Sprites_01_Index + randomSkin];
				index = 0;
			}
			else if (index < textureRange[0])
			{
				this.texture = sprites_02_Tex[Definition.Sprites_02_Index + randomWeapon];
				num = index - Definition.maxSprites[0];
				index = 1;
			}
			else if (index < textureRange[1])
			{
				this.texture = sprites_03_Tex[Definition.Sprites_03_Index];
				num = index - textureRange[0];
				index = 2;
			}
			else if (index < textureRange[2])
			{
				this.texture = sprites_04_Tex[Definition.Sprites_04_Index];
				num = index - textureRange[1];
				if (Definition.charType != 0)
				{
					result = true;
				}
				index = 3;
			}
			else if (index < textureRange[3])
			{
				this.texture = sprites_05_Tex[Definition.Sprites_05_Index];
				num = index - textureRange[2];
				if (num >= (textureRange[3] - textureRange[2]) / 2)
				{
					result = true;
				}
				index = 4;
			}
			else
			{
				index = 0;
			}
			if (this.texture != null && !this.texture.IsDisposed)
			{
				if (maxAnimFrames > 0 && this.Index >= Definition.maxSprites[0] + Definition.maxSprites[1] && this.Index < Definition.maxSprites[0] + Definition.maxSprites[1] + Definition.maxSprites[2])
				{
					int num2 = (num % Definition.maxSprites[index] % Definition.maxSpritesHorz[index] + curAnimFrame) * Definition.sourceWidth[index];
					int num3 = num % Definition.maxSprites[index] / Definition.maxSpritesHorz[index] * Definition.sourceHeight[index];
					while (num2 >= sprites_03_Tex[Definition.Sprites_03_Index].Width)
					{
						num2 -= sprites_03_Tex[Definition.Sprites_03_Index].Width;
						num3 += Definition.sourceHeight[index];
					}
					this.sRect = new Rectangle(num2, num3, Definition.sourceWidth[index], Definition.sourceHeight[index]);
				}
				else
				{
					this.sRect = new Rectangle(num % Definition.maxSprites[index] % Definition.maxSpritesHorz[index] * Definition.sourceWidth[index], num % Definition.maxSprites[index] / Definition.maxSpritesHorz[index] * Definition.sourceHeight[index], Definition.sourceWidth[index], Definition.sourceHeight[index]);
				}
			}
			return result;
		}
	}
}

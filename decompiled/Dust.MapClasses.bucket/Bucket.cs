using Dust.CharClasses;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses.bucket
{
	public class Bucket
	{
		private BucketItem[] bucketItem = new BucketItem[64];

		public int Size;

		private float updateFrame;

		public bool IsEmpty;

		public Bucket(int size)
		{
			for (int i = 0; i < this.bucketItem.Length; i++)
			{
				this.bucketItem[i] = null;
			}
			this.Size = size;
		}

		public void AddItem(Vector2 loc, int charDef)
		{
			for (int i = 0; i < this.bucketItem.Length; i++)
			{
				if (this.bucketItem[i] == null)
				{
					this.bucketItem[i] = new BucketItem(loc, charDef);
					break;
				}
			}
		}

		public void Update(Character[] c)
		{
			this.updateFrame -= Game1.FrameTime;
			if (this.updateFrame > 0f)
			{
				return;
			}
			this.updateFrame = 1f;
			int num = 0;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Team == Team.Enemy)
				{
					num = num;
				}
			}
			if (num >= this.Size)
			{
				return;
			}
			for (int j = 0; j < this.bucketItem.Length; j++)
			{
				if (this.bucketItem[j] == null)
				{
					continue;
				}
				for (int k = 0; k < c.Length; k++)
				{
					if (c[k].Exists == CharExists.Dead)
					{
						c[k].NewCharacter(this.bucketItem[j].Location, Game1.charDef[this.bucketItem[j].Definition], k, "bucket", Team.Enemy, ground: false);
						this.bucketItem[j] = null;
						return;
					}
				}
			}
			if (num == 0)
			{
				this.IsEmpty = true;
			}
		}
	}
}

using Microsoft.Xna.Framework;

namespace Dust
{
	public static class LogicalGamer
	{
		private static readonly PlayerIndex[] playerIndices = new PlayerIndex[4]
		{
			PlayerIndex.One,
			PlayerIndex.Two,
			PlayerIndex.Three,
			PlayerIndex.Four
		};

		public static PlayerIndex GetPlayerIndex(LogicalGamerIndex index)
		{
			return LogicalGamer.playerIndices[(int)index];
		}

		public static void SetPlayerIndex(LogicalGamerIndex gamerIndex, PlayerIndex playerIndex)
		{
			int num = 0;
			for (int i = 0; i < LogicalGamer.playerIndices.Length; i++)
			{
				if (LogicalGamer.playerIndices[i] == playerIndex)
				{
					num = i;
					break;
				}
			}
			PlayerIndex playerIndex2 = LogicalGamer.playerIndices[(int)gamerIndex];
			LogicalGamer.playerIndices[(int)gamerIndex] = playerIndex;
			LogicalGamer.playerIndices[num] = playerIndex2;
		}
	}
}

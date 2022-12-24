using System;

namespace Dust
{
	public struct BreakInfo
	{
		private uint m_Character;

		private bool m_IsNonBeginningCharacter;

		private bool m_IsNonEndingCharacter;

		public uint Character => this.m_Character;

		public bool IsNonBeginningCharacter => this.m_IsNonBeginningCharacter;

		public bool IsNonEndingCharacter => this.m_IsNonEndingCharacter;

		public BreakInfo(uint character, bool isNonBeginningCharacter, bool isNonEndingCharacter)
		{
			if (character > 1114111)
			{
				throw new ArgumentException("Invalid code point.");
			}
			this.m_Character = character;
			this.m_IsNonBeginningCharacter = isNonBeginningCharacter;
			this.m_IsNonEndingCharacter = isNonEndingCharacter;
		}
	}
}

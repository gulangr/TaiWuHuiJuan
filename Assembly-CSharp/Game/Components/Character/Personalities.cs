using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F3E RID: 3902
	public class Personalities : MonoBehaviour
	{
		// Token: 0x0600B34F RID: 45903 RVA: 0x0051A2BC File Offset: 0x005184BC
		public void Set(CharacterDisplayData displayData)
		{
			this.Set(displayData.Personalities);
		}

		// Token: 0x0600B350 RID: 45904 RVA: 0x0051A2CC File Offset: 0x005184CC
		public unsafe void Set(Personalities personalityData)
		{
			for (int i = 0; i < 7; i++)
			{
				this.personalities[i].Set((sbyte)i, (int)(*personalityData[i]));
			}
		}

		// Token: 0x04008B46 RID: 35654
		[SerializeField]
		private Personality[] personalities;
	}
}

using System;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F39 RID: 3897
	public class NameAndTitle : MonoBehaviour
	{
		// Token: 0x0600B32D RID: 45869 RVA: 0x005191FC File Offset: 0x005173FC
		public void Set(CharacterDisplayData displayData, bool isTaiwu = false, bool isHideTitleWhenNone = false)
		{
			bool flag = displayData == null;
			if (!flag)
			{
				bool flag2 = this.characterName != null;
				if (flag2)
				{
					this.characterName.Set(displayData, isTaiwu);
				}
				bool flag3 = this.characterTitle != null;
				if (flag3)
				{
					this.characterTitle.Set(displayData, isHideTitleWhenNone);
				}
			}
		}

		// Token: 0x04008B31 RID: 35633
		[SerializeField]
		private Name characterName;

		// Token: 0x04008B32 RID: 35634
		[SerializeField]
		private Title characterTitle;
	}
}

using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000994 RID: 2452
	public class TaiwuSummaryTemplate : MonoBehaviour
	{
		// Token: 0x06007626 RID: 30246 RVA: 0x003711E0 File Offset: 0x0036F3E0
		public void Set(int index, CharacterDisplayData data, bool isCurrentTaiwu, AvatarRelatedData avatarRelatedData)
		{
			this.taiwuGeneration.SetText((index + 1).ToString(), true);
			this.taiwuName.SetText(NameCenter.GetNameByDisplayData(data, false, true), true);
			this.currentTaiwu.SetActive(isCurrentTaiwu);
			bool flag = avatarRelatedData != null;
			if (flag)
			{
				this.avatar.Refresh(data, false);
			}
			else
			{
				this.avatar.RefreshAsGrave();
			}
		}

		// Token: 0x040058F4 RID: 22772
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x040058F5 RID: 22773
		public GameObject currentTaiwu;

		// Token: 0x040058F6 RID: 22774
		public TextMeshProUGUI taiwuName;

		// Token: 0x040058F7 RID: 22775
		public TextMeshProUGUI taiwuGeneration;
	}
}

using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x02000744 RID: 1860
	[RequireComponent(typeof(CToggle))]
	public class VillagerToggleHelper : MonoBehaviour
	{
		// Token: 0x06005A2E RID: 23086 RVA: 0x0029D908 File Offset: 0x0029BB08
		public void RefreshName(IAsyncMethodRequestHandler handler, short templateId, bool isInteractable)
		{
			DisableStyleRoot disableStyleRoot = this.root;
			this.toggle.interactable = isInteractable;
			disableStyleRoot.SetStyleEffect(!isInteractable, false);
			bool flag = templateId < 0;
			if (flag)
			{
				this.text.text = LanguageKey.LK_Villager.Tr();
			}
		}

		// Token: 0x04003E15 RID: 15893
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04003E16 RID: 15894
		[SerializeField]
		private TMP_Text text;

		// Token: 0x04003E17 RID: 15895
		[SerializeField]
		private DisableStyleRoot root;
	}
}

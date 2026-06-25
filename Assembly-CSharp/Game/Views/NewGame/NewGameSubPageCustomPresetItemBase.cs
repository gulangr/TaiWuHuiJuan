using System;
using GameData.Domains.Global;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000802 RID: 2050
	public abstract class NewGameSubPageCustomPresetItemBase : MonoBehaviour
	{
		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x0600643B RID: 25659
		public abstract DialogCmd StartGameCheck { get; }

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x0600643C RID: 25660
		// (set) Token: 0x0600643D RID: 25661
		public abstract bool StartGameChecked { get; set; }

		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x0600643E RID: 25662
		public abstract int SpentPoints { get; }

		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x0600643F RID: 25663
		public abstract int RemainingPoints { get; }

		// Token: 0x06006440 RID: 25664
		public abstract void RefreshUI();

		// Token: 0x06006441 RID: 25665
		public abstract void ApplyToPreset(CustomProtagonistPresetItem presetItem);

		// Token: 0x06006442 RID: 25666
		public abstract void ApplyFromPreset(CustomProtagonistPresetItem presetItem);

		// Token: 0x06006443 RID: 25667 RVA: 0x002DEB4F File Offset: 0x002DCD4F
		protected void NotifyDataModified()
		{
			NewGameSubPageCustomPreset parentCustomPresetPage = this.ParentCustomPresetPage;
			if (parentCustomPresetPage != null)
			{
				parentCustomPresetPage.OnSubPageDataModified();
			}
		}

		// Token: 0x040045FB RID: 17915
		public NewGameSubPageCustomPreset ParentCustomPresetPage;
	}
}

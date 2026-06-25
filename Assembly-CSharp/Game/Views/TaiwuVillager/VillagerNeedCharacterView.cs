using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.TaiwuVillager
{
	// Token: 0x0200075A RID: 1882
	public class VillagerNeedCharacterView : MonoBehaviour
	{
		// Token: 0x06005B2C RID: 23340 RVA: 0x002A50EB File Offset: 0x002A32EB
		private void Awake()
		{
		}

		// Token: 0x06005B2D RID: 23341 RVA: 0x002A50F0 File Offset: 0x002A32F0
		public void Set(AvatarRelatedData avatarData, NameRelatedData nameData, int remainMonth)
		{
			this.avatar.Refresh(avatarData);
			this.txtName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameData, false, false);
			this.txtTake.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_TakeAfterMonth, remainMonth);
		}

		// Token: 0x04003EE0 RID: 16096
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003EE1 RID: 16097
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x04003EE2 RID: 16098
		[SerializeField]
		private TextMeshProUGUI txtTake;

		// Token: 0x04003EE3 RID: 16099
		public TooltipInvoker toolTip;
	}
}

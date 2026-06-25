using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000840 RID: 2112
	public class AvatarAndName : MonoBehaviour
	{
		// Token: 0x060066DD RID: 26333 RVA: 0x002EEEDC File Offset: 0x002ED0DC
		public void Set(NameAndAvatar data)
		{
			this.Set(data.Avatar, ref data.Name, data.IsTaiwu);
		}

		// Token: 0x060066DE RID: 26334 RVA: 0x002EEEF8 File Offset: 0x002ED0F8
		public void SetEmpty()
		{
			this.emptyIcon.gameObject.SetActive(true);
			this.charName.gameObject.SetActive(false);
			this.avatar.gameObject.SetActive(false);
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x002EEF34 File Offset: 0x002ED134
		public void Set(AvatarRelatedData avatarData, ref NameRelatedData nameData, bool isTaiwu)
		{
			this.emptyIcon.gameObject.SetActive(false);
			bool flag = avatarData == null;
			if (flag)
			{
				this.avatar.RefreshAsGrave();
			}
			else
			{
				this.avatar.Refresh(avatarData, nameData.CharTemplateId);
			}
			this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameData, isTaiwu, false);
			this.charName.gameObject.SetActive(true);
			this.avatar.gameObject.SetActive(true);
		}

		// Token: 0x04004860 RID: 18528
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004861 RID: 18529
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x04004862 RID: 18530
		[SerializeField]
		private CImage emptyIcon;
	}
}

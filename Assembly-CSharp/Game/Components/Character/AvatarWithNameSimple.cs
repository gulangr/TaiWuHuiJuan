using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F14 RID: 3860
	public class AvatarWithNameSimple : MonoBehaviour
	{
		// Token: 0x0600B1E6 RID: 45542 RVA: 0x00510910 File Offset: 0x0050EB10
		public void Set(AvatarRelatedData avatarRelatedData, short templateId, string displayName)
		{
			this.avatar.ResetToBlank(false);
			bool flag = avatarRelatedData == null;
			if (flag)
			{
				this.avatar.RefreshAsGrave();
			}
			else
			{
				this.avatar.Refresh(avatarRelatedData, templateId);
			}
			this.nameLabel.text = displayName;
		}

		// Token: 0x040089E6 RID: 35302
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040089E7 RID: 35303
		[SerializeField]
		private TextMeshProUGUI nameLabel;
	}
}

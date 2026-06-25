using System;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B81 RID: 2945
	public class SkillBreakBonusSelectAvatarAndNameCell : MonoBehaviour, ICellContent<SkillBreakBonusSelectAvatarAndNameCellData>, ICellContent
	{
		// Token: 0x06009195 RID: 37269 RVA: 0x0043D78E File Offset: 0x0043B98E
		public void SetData(SkillBreakBonusSelectAvatarAndNameCellData data)
		{
			this.avatar.Refresh(data.DisplayData, true);
			this.nameLabel.text = data.GetName();
		}

		// Token: 0x04007020 RID: 28704
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007021 RID: 28705
		[SerializeField]
		private TextMeshProUGUI nameLabel;
	}
}

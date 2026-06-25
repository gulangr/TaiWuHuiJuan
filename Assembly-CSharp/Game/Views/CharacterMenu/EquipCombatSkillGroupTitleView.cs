using System;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B77 RID: 2935
	public class EquipCombatSkillGroupTitleView : MonoBehaviour
	{
		// Token: 0x060090F7 RID: 37111 RVA: 0x004395A8 File Offset: 0x004377A8
		public void Set(bool isEquippedGroup)
		{
			this.icon.sprite = (isEquippedGroup ? this.equippedIcon : this.unEquippedIcon);
			LanguageKey key = isEquippedGroup ? LanguageKey.LK_EquipCombatSkill_ScrollGroupTitle_0 : LanguageKey.LK_EquipCombatSkill_ScrollGroupTitle_1;
			this.title.text = key.Tr().ColorReplace();
		}

		// Token: 0x04006FB6 RID: 28598
		[SerializeField]
		private CImage icon;

		// Token: 0x04006FB7 RID: 28599
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04006FB8 RID: 28600
		[SerializeField]
		private Sprite equippedIcon;

		// Token: 0x04006FB9 RID: 28601
		[SerializeField]
		private Sprite unEquippedIcon;
	}
}

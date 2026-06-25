using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000881 RID: 2177
	public class TooltipCombatSkillBodyPartItem : MonoBehaviour
	{
		// Token: 0x060068DD RID: 26845 RVA: 0x0030310C File Offset: 0x0030130C
		public void Set(string iconSpriteName, string partName, bool showSlash)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.nameText.text = partName;
			this.slashDivider.SetActive(showSlash);
		}

		// Token: 0x04004AED RID: 19181
		[SerializeField]
		private CImage icon;

		// Token: 0x04004AEE RID: 19182
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004AEF RID: 19183
		[SerializeField]
		private GameObject slashDivider;
	}
}

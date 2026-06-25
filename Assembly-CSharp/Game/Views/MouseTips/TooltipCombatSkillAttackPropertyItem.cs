using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087F RID: 2175
	public class TooltipCombatSkillAttackPropertyItem : MonoBehaviour
	{
		// Token: 0x060068D6 RID: 26838 RVA: 0x003030A6 File Offset: 0x003012A6
		public void Set(string iconSpriteName, string title, string value)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.titleText.text = title;
			this.valueText.text = value;
		}

		// Token: 0x04004AE7 RID: 19175
		[SerializeField]
		private CImage icon;

		// Token: 0x04004AE8 RID: 19176
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004AE9 RID: 19177
		[SerializeField]
		private TextMeshProUGUI valueText;
	}
}

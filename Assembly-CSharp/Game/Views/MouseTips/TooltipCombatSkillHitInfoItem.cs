using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000889 RID: 2185
	public class TooltipCombatSkillHitInfoItem : MonoBehaviour
	{
		// Token: 0x060068F2 RID: 26866 RVA: 0x00303593 File Offset: 0x00301793
		public void Set(string iconSpriteName, string title, string value, string distributionStr)
		{
			this.Set(iconSpriteName, title, value, distributionStr, false);
		}

		// Token: 0x060068F3 RID: 26867 RVA: 0x003035A4 File Offset: 0x003017A4
		public void Set(string iconSpriteName, string title, string value, string distributionStr, bool showSpecialBack)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.titleText.text = title;
			this.valueText.text = value;
			this.distributionText.text = distributionStr;
			bool flag = this.specialBack;
			if (flag)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004B12 RID: 19218
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B13 RID: 19219
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B14 RID: 19220
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004B15 RID: 19221
		[SerializeField]
		private TextMeshProUGUI distributionText;

		// Token: 0x04004B16 RID: 19222
		[SerializeField]
		private CImage specialBack;
	}
}

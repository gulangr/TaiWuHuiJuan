using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000885 RID: 2181
	public class TooltipCombatSkillDefenseSpecialItem : MonoBehaviour
	{
		// Token: 0x060068E6 RID: 26854 RVA: 0x00303349 File Offset: 0x00301549
		public void Set(string iconSpriteName, string title, string value, string desc, bool detailMode)
		{
			this.Set(iconSpriteName, title, value, desc, detailMode, false);
		}

		// Token: 0x060068E7 RID: 26855 RVA: 0x0030335C File Offset: 0x0030155C
		public void Set(string iconSpriteName, string title, string value, string desc, bool detailMode, bool showSpecialBack)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.titleText.text = title.ColorReplace();
			this.valueText.text = value.ColorReplace();
			this.descText.text = desc.ColorReplace();
			this.descText.transform.parent.gameObject.SetActive(detailMode);
			bool flag = this.specialBack;
			if (flag)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004B00 RID: 19200
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B01 RID: 19201
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B02 RID: 19202
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004B03 RID: 19203
		[SerializeField]
		private TextMeshProUGUI descText;

		// Token: 0x04004B04 RID: 19204
		[SerializeField]
		private CImage specialBack;
	}
}

using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000883 RID: 2179
	public class TooltipCombatSkillCastEffectItem : MonoBehaviour
	{
		// Token: 0x060068E1 RID: 26849 RVA: 0x00303281 File Offset: 0x00301481
		public void Set(string iconSpriteName, string title, string value)
		{
			this.Set(iconSpriteName, title, value, false);
		}

		// Token: 0x060068E2 RID: 26850 RVA: 0x00303290 File Offset: 0x00301490
		public void Set(string iconSpriteName, string title, string value, bool showSpecialBack)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.titleText.text = title;
			this.valueText.text = value;
			bool flag = this.line;
			if (flag)
			{
				this.line.gameObject.SetActive(base.transform.GetSiblingIndex() % 2 == 0);
			}
			bool flag2 = this.specialBack;
			if (flag2)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004AF9 RID: 19193
		[SerializeField]
		private CImage icon;

		// Token: 0x04004AFA RID: 19194
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004AFB RID: 19195
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004AFC RID: 19196
		[SerializeField]
		private CImage line;

		// Token: 0x04004AFD RID: 19197
		[SerializeField]
		private CImage specialBack;
	}
}

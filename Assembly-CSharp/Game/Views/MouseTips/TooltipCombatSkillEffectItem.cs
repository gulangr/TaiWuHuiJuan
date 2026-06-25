using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000887 RID: 2183
	public class TooltipCombatSkillEffectItem : MonoBehaviour
	{
		// Token: 0x060068EB RID: 26859 RVA: 0x00303438 File Offset: 0x00301638
		public void Set(string iconSpriteName, string title, string value)
		{
			this.Set(iconSpriteName, title, value, false);
		}

		// Token: 0x060068EC RID: 26860 RVA: 0x00303448 File Offset: 0x00301648
		public void Set(string iconSpriteName, string title, string value, bool showSpecialBack)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.titleText.text = title;
			this.valueText.text = value;
			bool flag = this.specialBack;
			if (flag)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x060068ED RID: 26861 RVA: 0x0030349C File Offset: 0x0030169C
		public void SetDividerVisible(bool visible)
		{
			this.divider.SetActive(visible);
		}

		// Token: 0x04004B09 RID: 19209
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B0A RID: 19210
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B0B RID: 19211
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004B0C RID: 19212
		[SerializeField]
		private GameObject divider;

		// Token: 0x04004B0D RID: 19213
		[SerializeField]
		private CImage specialBack;
	}
}

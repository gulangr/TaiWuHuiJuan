using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088B RID: 2187
	public class TooltipCombatSkillHitPartItem : MonoBehaviour
	{
		// Token: 0x060068F8 RID: 26872 RVA: 0x003036C6 File Offset: 0x003018C6
		public void Set(string iconSpriteName, string content, Color color, bool showDivider = true)
		{
			this.Set(iconSpriteName, content, color, showDivider, false);
		}

		// Token: 0x060068F9 RID: 26873 RVA: 0x003036D8 File Offset: 0x003018D8
		public void Set(string iconSpriteName, string content, Color color, bool showDivider, bool showSpecialBack)
		{
			this.icon.SetSprite(iconSpriteName, false, null);
			this.text.text = content;
			this.text.color = color;
			this.canvasGroup.alpha = 1f;
			this.divider.SetActive(showDivider);
			bool flag = this.specialBack;
			if (flag)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004B19 RID: 19225
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04004B1A RID: 19226
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B1B RID: 19227
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04004B1C RID: 19228
		[SerializeField]
		private GameObject divider;

		// Token: 0x04004B1D RID: 19229
		[SerializeField]
		private CImage specialBack;
	}
}

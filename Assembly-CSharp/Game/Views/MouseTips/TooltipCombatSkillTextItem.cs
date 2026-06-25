using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088E RID: 2190
	public class TooltipCombatSkillTextItem : MonoBehaviour
	{
		// Token: 0x06006900 RID: 26880 RVA: 0x003039E8 File Offset: 0x00301BE8
		public void Set(string title, string value)
		{
			this.titleText.text = title;
			this.valueText.text = value;
		}

		// Token: 0x06006901 RID: 26881 RVA: 0x00303A05 File Offset: 0x00301C05
		public void SetTitle(string title)
		{
			this.titleText.text = title;
		}

		// Token: 0x04004B29 RID: 19241
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B2A RID: 19242
		[SerializeField]
		private TextMeshProUGUI valueText;
	}
}

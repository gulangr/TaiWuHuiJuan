using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000886 RID: 2182
	public class TooltipCombatSkillDistanceItem : MonoBehaviour
	{
		// Token: 0x060068E9 RID: 26857 RVA: 0x003033F3 File Offset: 0x003015F3
		public void Set(string title, string nearValue, string farValue)
		{
			this.titleText.text = title;
			this.nearValueText.text = nearValue;
			this.farValueText.text = farValue;
			this.unusedValueText.gameObject.SetActive(false);
		}

		// Token: 0x04004B05 RID: 19205
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B06 RID: 19206
		[SerializeField]
		private TextMeshProUGUI nearValueText;

		// Token: 0x04004B07 RID: 19207
		[SerializeField]
		private TextMeshProUGUI farValueText;

		// Token: 0x04004B08 RID: 19208
		[SerializeField]
		private TextMeshProUGUI unusedValueText;
	}
}

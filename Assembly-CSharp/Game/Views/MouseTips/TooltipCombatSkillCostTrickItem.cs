using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000884 RID: 2180
	public class TooltipCombatSkillCostTrickItem : MonoBehaviour
	{
		// Token: 0x060068E4 RID: 26852 RVA: 0x0030331D File Offset: 0x0030151D
		public void Set(string trickName, string trickFontColor, string count)
		{
			this.nameText.text = trickName.SetColor(trickFontColor);
			this.countText.text = count;
		}

		// Token: 0x04004AFE RID: 19198
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004AFF RID: 19199
		[SerializeField]
		private TextMeshProUGUI countText;
	}
}

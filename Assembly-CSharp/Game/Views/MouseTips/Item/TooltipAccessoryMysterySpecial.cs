using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x02000897 RID: 2199
	public class TooltipAccessoryMysterySpecial : MonoBehaviour
	{
		// Token: 0x0600694A RID: 26954 RVA: 0x00306574 File Offset: 0x00304774
		public void Refresh(MysteryEffectItem config, int index, int otherCount, short requirementsPower, bool isTemplate)
		{
			int realIndex = index + otherCount;
			LanguageKey indexKey = CommonUtils.TraditionalChineseNumber[realIndex + 1];
			int powerRequirement = config.PowerRequirements[realIndex];
			string indexStr = indexKey.Tr() + "·";
			string powerStr = string.Format("{0}%", powerRequirement);
			this.textIndex.SetText(indexStr, true);
			this.textPower.SetText(powerStr, true);
			this.styleRoot.SetInteractable(powerRequirement <= (int)requirementsPower || isTemplate);
			short templateId = config.BonusEffects[index];
			SpecialEffectItem specialEffectConfig = SpecialEffect.Instance[templateId];
			this.textEffectTitle.SetText(specialEffectConfig.Name, true);
			this.textEffectDesc.SetText(specialEffectConfig.Desc[0], true);
		}

		// Token: 0x04004B8B RID: 19339
		[SerializeField]
		private TextMeshProUGUI textIndex;

		// Token: 0x04004B8C RID: 19340
		[SerializeField]
		private TextMeshProUGUI textPower;

		// Token: 0x04004B8D RID: 19341
		[SerializeField]
		private TextMeshProUGUI textEffectTitle;

		// Token: 0x04004B8E RID: 19342
		[SerializeField]
		private TextMeshProUGUI textEffectDesc;

		// Token: 0x04004B8F RID: 19343
		[SerializeField]
		private DisableStyleRoot styleRoot;
	}
}

using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Components.MouseTip
{
	// Token: 0x02000E9E RID: 3742
	public class MysterySpecialEffect : MonoBehaviour
	{
		// Token: 0x0600AD79 RID: 44409 RVA: 0x004F22B8 File Offset: 0x004F04B8
		public void RefreshInfo(MysteryEffectItem config, int index, int otherCount, short requirementsPower)
		{
			int realIndex = index + otherCount;
			this.indexIcon.SetSprite("ui9_icon_mouse_tip_mystery_level_" + realIndex.ToString(), false, null);
			int powerRequirement = config.PowerRequirements[realIndex];
			this.disableStyleRoot.SetStyleEffect(powerRequirement > (int)requirementsPower, false);
			this.powerRequirements.SetText(powerRequirement.ToString() + "%", true);
			short templateId = config.BonusEffects[index];
			SpecialEffectItem specialEffectConfig = SpecialEffect.Instance[templateId];
			this.effectName.SetText(specialEffectConfig.Name + "：", true);
			this.effectDesc.SetText(specialEffectConfig.Desc[0], true);
		}

		// Token: 0x04008600 RID: 34304
		[SerializeField]
		private CImage indexIcon;

		// Token: 0x04008601 RID: 34305
		[SerializeField]
		private TextMeshProUGUI powerRequirements;

		// Token: 0x04008602 RID: 34306
		[SerializeField]
		private TextMeshProUGUI effectName;

		// Token: 0x04008603 RID: 34307
		[SerializeField]
		private TextMeshProUGUI effectDesc;

		// Token: 0x04008604 RID: 34308
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;
	}
}

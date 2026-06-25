using System;
using Config;
using FrameWork.UISystem.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.MouseTip
{
	// Token: 0x02000E9C RID: 3740
	public class MysteryEffectInfo : MonoBehaviour
	{
		// Token: 0x0600AD75 RID: 44405 RVA: 0x004F2098 File Offset: 0x004F0298
		public void RefreshInfo(int mysteryEffectId, short requirementsPower)
		{
			MysteryEffectItem config = MysteryEffect.Instance[mysteryEffectId];
			this.property.Rebuild<MysteryProperty>(config.BonusValues.Count, delegate(MysteryProperty mysteryProperty, int index)
			{
				mysteryProperty.RefreshInfo(config, index, requirementsPower);
			});
			this.specialEffect.Rebuild<MysterySpecialEffect>(config.BonusEffects.Count, delegate(MysterySpecialEffect mysterySpecialEffect, int index)
			{
				mysterySpecialEffect.RefreshInfo(config, index, config.BonusValues.Count, requirementsPower);
			});
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		}

		// Token: 0x040085F9 RID: 34297
		[SerializeField]
		private TemplatedContainerAssemblyNew property;

		// Token: 0x040085FA RID: 34298
		[SerializeField]
		private TemplatedContainerAssemblyNew specialEffect;
	}
}

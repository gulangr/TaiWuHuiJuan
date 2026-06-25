using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B1E RID: 2846
	public class CombatConsummateLevel : MonoBehaviour
	{
		// Token: 0x06008BB3 RID: 35763 RVA: 0x00408384 File Offset: 0x00406584
		public void Set(sbyte consummateLevel)
		{
			string spriteName = CommonUtils.GetConsummateLevelShowDataLegacy(consummateLevel).Item1;
			this.icon.SetSprite(spriteName, false, null);
			this.text.text = consummateLevel.ToString();
			this.mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Consummate_Level) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + consummateLevel.ToString();
		}

		// Token: 0x04006AF2 RID: 27378
		[SerializeField]
		private CImage icon;

		// Token: 0x04006AF3 RID: 27379
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04006AF4 RID: 27380
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}

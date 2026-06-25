using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B43 RID: 2883
	public class CombatBodyPartPrefab : MonoBehaviour
	{
		// Token: 0x04006D88 RID: 28040
		public CImage back;

		// Token: 0x04006D89 RID: 28041
		public TextMeshProUGUI bodyPartName;

		// Token: 0x04006D8A RID: 28042
		public TextMeshProUGUI skillEffectCount;

		// Token: 0x04006D8B RID: 28043
		public CImage highlight;

		// Token: 0x04006D8C RID: 28044
		public CImage checkMark;

		// Token: 0x04006D8D RID: 28045
		public TooltipInvoker mouseTipDisplayer;
	}
}

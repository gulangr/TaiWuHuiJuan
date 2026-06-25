using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008FF RID: 2303
	public class HealCostItem : MonoBehaviour
	{
		// Token: 0x04004F9D RID: 20381
		public CImage icon;

		// Token: 0x04004F9E RID: 20382
		public TextMeshProUGUI value;

		// Token: 0x04004F9F RID: 20383
		public TooltipInvoker mouseTip;

		// Token: 0x04004FA0 RID: 20384
		public PointerTrigger pointerTrigger;

		// Token: 0x04004FA1 RID: 20385
		public GameObject highLightArea;
	}
}

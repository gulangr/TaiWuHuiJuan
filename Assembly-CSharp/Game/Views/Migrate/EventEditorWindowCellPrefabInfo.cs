using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008F8 RID: 2296
	public class EventEditorWindowCellPrefabInfo : MonoBehaviour
	{
		// Token: 0x04004F84 RID: 20356
		public PointClickBridge clickBridge;

		// Token: 0x04004F85 RID: 20357
		public TooltipInvoker mouseTip;

		// Token: 0x04004F86 RID: 20358
		public TextMeshProUGUI txtMeshEventName;

		// Token: 0x04004F87 RID: 20359
		public GameObject goEditingEvent;

		// Token: 0x04004F88 RID: 20360
		public GameObject goHasTrigger;

		// Token: 0x04004F89 RID: 20361
		public GameObject goHasInstruction;

		// Token: 0x04004F8A RID: 20362
		public GameObject goHasCode;
	}
}

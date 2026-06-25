using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008FB RID: 2299
	public class EventGroupTreeViewGroupPrefabInfo : MonoBehaviour
	{
		// Token: 0x04004F91 RID: 20369
		public PointClickBridge clickBridge;

		// Token: 0x04004F92 RID: 20370
		public GameObject goSelected;

		// Token: 0x04004F93 RID: 20371
		public TextMeshProUGUI txtMeshGroupInfo;

		// Token: 0x04004F94 RID: 20372
		public CButton btnExportSwitch;

		// Token: 0x04004F95 RID: 20373
		public TooltipInvoker exportMouseTip;

		// Token: 0x04004F96 RID: 20374
		public GameObject goExportFlag;
	}
}

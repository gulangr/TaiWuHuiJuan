using System;
using Game.Components.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BCC RID: 3020
	public class BuildingOverViewCoreItemHolder : MonoBehaviour
	{
		// Token: 0x040074FD RID: 29949
		public ItemBack coreItemView;

		// Token: 0x040074FE RID: 29950
		public TextMeshProUGUI itemName;

		// Token: 0x040074FF RID: 29951
		public TextMeshProUGUI itemDesc;

		// Token: 0x04007500 RID: 29952
		public TextMeshProUGUI cur;

		// Token: 0x04007501 RID: 29953
		public TooltipInvoker mouseTip;
	}
}

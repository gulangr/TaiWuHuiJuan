using System;
using System.Collections.Generic;
using Game.Views.MouseTips.Item.Common;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200085E RID: 2142
	public class MouseTipFuyuItemType : MonoBehaviour
	{
		// Token: 0x060067B0 RID: 26544 RVA: 0x002F5F01 File Offset: 0x002F4101
		public void Set(List<ItemFunction> disableFunctions)
		{
			this.otherArea.Refresh(disableFunctions);
		}

		// Token: 0x04004946 RID: 18758
		[SerializeField]
		private TooltipItemOtherArea otherArea;
	}
}

using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDF RID: 3807
	public class RefineEffectInfo : MonoBehaviour
	{
		// Token: 0x0600AF38 RID: 44856 RVA: 0x004FD544 File Offset: 0x004FB744
		public void Refresh(List<ItemKey> itemKeyList, sbyte targetItemType)
		{
			for (int i = 0; i < this.refineEffects.Length; i++)
			{
				RefineEffectItem item = this.refineEffects[i];
				item.Refresh(itemKeyList[i], targetItemType);
			}
		}

		// Token: 0x040087BF RID: 34751
		[SerializeField]
		private RefineEffectItem[] refineEffects = new RefineEffectItem[5];
	}
}

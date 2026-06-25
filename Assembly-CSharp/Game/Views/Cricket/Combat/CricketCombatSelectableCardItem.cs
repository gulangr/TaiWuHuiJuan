using System;
using Game.Components.ListStyleGeneralScroll.Item;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD0 RID: 2768
	public class CricketCombatSelectableCardItem : MonoBehaviour
	{
		// Token: 0x0600885D RID: 34909 RVA: 0x003F4277 File Offset: 0x003F2477
		private void OnDisable()
		{
			this.orderBadge.SetOrder(-1);
		}

		// Token: 0x0600885E RID: 34910 RVA: 0x003F4288 File Offset: 0x003F2488
		public void Set(RowItemMain rowItemMain, bool showTip = true)
		{
			this.cardItem.Set(rowItemMain, showTip);
			int order = (rowItemMain.Data.RealKey.ItemType == 11) ? CricketCombatKit.Board.GetSelfCricketOrder(rowItemMain.Data.RealKey) : -1;
			this.orderBadge.SetOrder(order);
		}

		// Token: 0x04006877 RID: 26743
		[SerializeField]
		private CardItem cardItem;

		// Token: 0x04006878 RID: 26744
		[SerializeField]
		private CricketCombatOrderMark orderBadge;
	}
}

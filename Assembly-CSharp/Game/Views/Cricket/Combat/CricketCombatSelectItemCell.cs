using System;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD1 RID: 2769
	public class CricketCombatSelectItemCell : MonoBehaviour, ICellContent<ITradeableContent>, ICellContent
	{
		// Token: 0x06008860 RID: 34912 RVA: 0x003F42E7 File Offset: 0x003F24E7
		private void OnDisable()
		{
			this.orderBadge.SetOrder(-1);
		}

		// Token: 0x06008861 RID: 34913 RVA: 0x003F42F8 File Offset: 0x003F24F8
		public void SetData(ITradeableContent data)
		{
			this.rowItemMain.SetData(data);
			int order = (data.RealKey.ItemType == 11) ? CricketCombatKit.Board.GetSelfCricketOrder(data.RealKey) : -1;
			this.orderBadge.SetOrder(order);
		}

		// Token: 0x04006879 RID: 26745
		[SerializeField]
		private RowItemMain rowItemMain;

		// Token: 0x0400687A RID: 26746
		[SerializeField]
		private CricketCombatOrderMark orderBadge;
	}
}

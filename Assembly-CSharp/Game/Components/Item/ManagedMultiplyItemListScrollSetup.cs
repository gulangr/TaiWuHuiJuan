using System;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Item.Display;

namespace Game.Components.Item
{
	// Token: 0x02000EF2 RID: 3826
	public class ManagedMultiplyItemListScrollSetup
	{
		// Token: 0x0400880C RID: 34828
		public string MainSortSaveKey;

		// Token: 0x0400880D RID: 34829
		public string SelectedSortSaveKey;

		// Token: 0x0400880E RID: 34830
		public ESortAndFilterControllerType SortType = ESortAndFilterControllerType.Item;

		// Token: 0x0400880F RID: 34831
		public ESortAndFilterControllerType? SelectedSortType;

		// Token: 0x04008810 RID: 34832
		public bool EnableRowInteraction = true;

		// Token: 0x04008811 RID: 34833
		public Action<ITradeableContent, RowItemLine> OnRender;

		// Token: 0x04008812 RID: 34834
		public Action<ITradeableContent, RowItemLine> OnClick;

		// Token: 0x04008813 RID: 34835
		public ItemListScroll.EColumnType ColumnType = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount;

		// Token: 0x04008814 RID: 34836
		public Func<ITradeableContent, string> AmountGenerator;

		// Token: 0x04008815 RID: 34837
		public Action OnMainSortAndFilterChanged;
	}
}

using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CC9 RID: 3273
	public interface ISortUi
	{
		// Token: 0x0600A590 RID: 42384
		void Setup(SortUiConfig uiConfig, Action onSortChanged, Action onSortMenuShow, Action onSortMenuHide);

		// Token: 0x0600A591 RID: 42385
		SortStateData GetSortData();

		// Token: 0x0600A592 RID: 42386
		void SetSortData(SortStateData data);
	}
}

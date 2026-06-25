using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042C RID: 1068
	public interface ICommonSortUi
	{
		// Token: 0x06003F66 RID: 16230
		void Setup(CommonSortUiConfig uiConfig, Action onSortChanged, Action onSortMenuShow, Action onSortMenuHide);

		// Token: 0x06003F67 RID: 16231
		SortStateData GetSortData();

		// Token: 0x06003F68 RID: 16232
		void SetSortData(SortStateData data);
	}
}

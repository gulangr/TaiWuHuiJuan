using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000428 RID: 1064
	public interface ISortAndFilterLine
	{
		// Token: 0x06003F0E RID: 16142
		LineState GetLineState();

		// Token: 0x06003F0F RID: 16143
		void SetActive(bool isActive);

		// Token: 0x06003F10 RID: 16144
		bool IsActive();

		// Token: 0x06003F11 RID: 16145
		void ApplyCustomOrder(IFilterLineCustomOrderData orderData);

		// Token: 0x06003F12 RID: 16146
		void ResetCustomOrder();

		// Token: 0x06003F13 RID: 16147
		IFilterLineCustomOrderData GetCustomOrderData();

		// Token: 0x06003F14 RID: 16148
		void OnSwitchCustomOrderSettingMode(bool isSettingMode);

		// Token: 0x06003F15 RID: 16149
		void ClearAllFilter();

		// Token: 0x06003F16 RID: 16150
		void ApplyDynamicConfig(DynamicLineConfig dynamicConfig);
	}
}

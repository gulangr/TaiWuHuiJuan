using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA5 RID: 3237
	public interface ISortAndFilterLine
	{
		// Token: 0x0600A48B RID: 42123
		LineState GetLineState();

		// Token: 0x0600A48C RID: 42124
		void SetActive(bool isActive);

		// Token: 0x0600A48D RID: 42125
		bool IsActive();

		// Token: 0x0600A48E RID: 42126
		void ClearAllFilter();

		// Token: 0x0600A48F RID: 42127
		void ApplyDynamicConfig(DynamicLineConfig dynamicConfig);
	}
}

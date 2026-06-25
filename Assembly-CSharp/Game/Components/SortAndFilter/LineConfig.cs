using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CB0 RID: 3248
	public class LineConfig
	{
		// Token: 0x0600A504 RID: 42244 RVA: 0x004CF240 File Offset: 0x004CD440
		public static LineConfig CreateToggleGroupLineConfig(ToggleGroupLineConfig toggleGroupLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.ToggleGroup,
				ToggleGroupLineConfig = toggleGroupLineConfig
			};
		}

		// Token: 0x0600A505 RID: 42245 RVA: 0x004CF268 File Offset: 0x004CD468
		public static LineConfig CreateDetailedFilterLineConfig(DetailedFilterLineConfig detailedFilterLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.DetailedFilter,
				DetailedFilterLineConfig = detailedFilterLineConfig
			};
		}

		// Token: 0x0600A506 RID: 42246 RVA: 0x004CF290 File Offset: 0x004CD490
		public static LineConfig CreateDetailedSecondaryLineConfig(DetailedFilterLineConfig detailedFilterLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.SingleSelectFilter,
				DetailedFilterLineConfig = detailedFilterLineConfig
			};
		}

		// Token: 0x0400826A RID: 33386
		public int Id;

		// Token: 0x0400826B RID: 33387
		public ActiveCondition? ActiveCondition;

		// Token: 0x0400826C RID: 33388
		public bool DefaultActive = true;

		// Token: 0x0400826D RID: 33389
		public bool IndividualLine = true;

		// Token: 0x0400826E RID: 33390
		public ToggleGroupLineConfig ToggleGroupLineConfig;

		// Token: 0x0400826F RID: 33391
		public DetailedFilterLineConfig DetailedFilterLineConfig;

		// Token: 0x04008270 RID: 33392
		public int Level;

		// Token: 0x04008271 RID: 33393
		public ESortAndFilterOneLineType Type;
	}
}

using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200044D RID: 1101
	public class LineConfig
	{
		// Token: 0x0600404D RID: 16461 RVA: 0x001FF2B0 File Offset: 0x001FD4B0
		public static LineConfig CreateToggleGroupLineConfig(ToggleGroupLineConfig toggleGroupLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.ToggleGroup,
				ToggleGroupLineConfig = toggleGroupLineConfig
			};
		}

		// Token: 0x0600404E RID: 16462 RVA: 0x001FF2D8 File Offset: 0x001FD4D8
		public static LineConfig CreateDetailedFilterLineConfig(DetailedFilterLineConfig detailedFilterLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.DetailedFilter,
				DetailedFilterLineConfig = detailedFilterLineConfig
			};
		}

		// Token: 0x0600404F RID: 16463 RVA: 0x001FF300 File Offset: 0x001FD500
		public static LineConfig CreateDetailedSecondaryLineConfig(DetailedFilterLineConfig detailedFilterLineConfig)
		{
			return new LineConfig
			{
				Type = ESortAndFilterOneLineType.SingleSelectFilter,
				DetailedFilterLineConfig = detailedFilterLineConfig
			};
		}

		// Token: 0x04002DED RID: 11757
		public int Id;

		// Token: 0x04002DEE RID: 11758
		public ActiveCondition? ActiveCondition;

		// Token: 0x04002DEF RID: 11759
		public bool DefaultActive = true;

		// Token: 0x04002DF0 RID: 11760
		public bool IndividualLine = true;

		// Token: 0x04002DF1 RID: 11761
		public ToggleGroupLineConfig ToggleGroupLineConfig;

		// Token: 0x04002DF2 RID: 11762
		public DetailedFilterLineConfig DetailedFilterLineConfig;

		// Token: 0x04002DF3 RID: 11763
		public int Level;

		// Token: 0x04002DF4 RID: 11764
		public ESortAndFilterOneLineType Type;
	}
}

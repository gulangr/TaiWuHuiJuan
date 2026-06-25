using System;

namespace DisplayConfig
{
	// Token: 0x020006DE RID: 1758
	public class FiveElement
	{
		// Token: 0x17000A52 RID: 2642
		public FiveElementItem this[int index]
		{
			get
			{
				return this._dataArray[index];
			}
		}

		// Token: 0x04003884 RID: 14468
		public static FiveElement Instance = new FiveElement();

		// Token: 0x04003885 RID: 14469
		private readonly FiveElementItem[] _dataArray = new FiveElementItem[]
		{
			new FiveElementItem(0, LanguageKey.LK_FiveElements_Type_0, "mousetip_shuxing_0"),
			new FiveElementItem(1, LanguageKey.LK_FiveElements_Type_1, "mousetip_shuxing_1"),
			new FiveElementItem(2, LanguageKey.LK_FiveElements_Type_2, "mousetip_shuxing_2"),
			new FiveElementItem(3, LanguageKey.LK_FiveElements_Type_3, "mousetip_shuxing_3"),
			new FiveElementItem(4, LanguageKey.LK_FiveElements_Type_4, "mousetip_shuxing_4"),
			new FiveElementItem(5, LanguageKey.LK_FiveElements_Type_5, "mousetip_shuxing_5")
		};
	}
}

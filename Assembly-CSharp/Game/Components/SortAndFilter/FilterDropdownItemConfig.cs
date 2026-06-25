using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C95 RID: 3221
	public struct FilterDropdownItemConfig
	{
		// Token: 0x0600A3F4 RID: 41972 RVA: 0x004CA23F File Offset: 0x004C843F
		public FilterDropdownItemConfig(StringKey text)
		{
			this.Text = text;
		}

		// Token: 0x0600A3F5 RID: 41973 RVA: 0x004CA24C File Offset: 0x004C844C
		public static FilterDropdownItemConfig CreateOptionNamedAll()
		{
			return new FilterDropdownItemConfig
			{
				Text = LanguageKey.LK_Common_All
			};
		}

		// Token: 0x040081D5 RID: 33237
		public StringKey Text;
	}
}

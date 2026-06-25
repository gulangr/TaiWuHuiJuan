using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CB5 RID: 3253
	public struct FilterToggleConfig
	{
		// Token: 0x0600A50C RID: 42252 RVA: 0x004CF2F8 File Offset: 0x004CD4F8
		public FilterToggleConfig(string toggleImageBase, StringKey tipContent)
		{
			this.ToggleImageBase = toggleImageBase;
			this.TipContent = tipContent;
		}

		// Token: 0x0600A50D RID: 42253 RVA: 0x004CF30C File Offset: 0x004CD50C
		public static FilterToggleConfig CreateNamedAll()
		{
			return new FilterToggleConfig
			{
				ToggleImageBase = "ui9_btn_filter_entire",
				TipContent = LanguageKey.LK_Common_All
			};
		}

		// Token: 0x04008276 RID: 33398
		public string ToggleImageBase;

		// Token: 0x04008277 RID: 33399
		public StringKey TipContent;
	}
}

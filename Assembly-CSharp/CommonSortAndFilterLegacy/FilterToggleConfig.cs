using System;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000452 RID: 1106
	public struct FilterToggleConfig
	{
		// Token: 0x06004055 RID: 16469 RVA: 0x001FF368 File Offset: 0x001FD568
		public FilterToggleConfig(ToggleTransitionIconSpriteNames iconNames, StringKey tipContent)
		{
			this.IconNames = iconNames;
			this.TipContent = tipContent;
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x001FF37C File Offset: 0x001FD57C
		public static FilterToggleConfig CreateNamedAll()
		{
			return new FilterToggleConfig
			{
				IconNames = new ToggleTransitionIconSpriteNames("ui_sp_icon_filter_all_0", "ui_sp_icon_filter_all_0", "ui_sp_icon_filter_all_1", "ui_sp_icon_filter_all_2"),
				TipContent = LanguageKey.LK_Common_All
			};
		}

		// Token: 0x04002DF9 RID: 11769
		public ToggleTransitionIconSpriteNames IconNames;

		// Token: 0x04002DFA RID: 11770
		public StringKey TipContent;
	}
}

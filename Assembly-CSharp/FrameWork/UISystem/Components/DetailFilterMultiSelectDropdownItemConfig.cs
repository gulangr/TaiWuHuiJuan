using System;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001014 RID: 4116
	public struct DetailFilterMultiSelectDropdownItemConfig
	{
		// Token: 0x0600BC10 RID: 48144 RVA: 0x005586C8 File Offset: 0x005568C8
		public static DetailFilterMultiSelectDropdownItemConfig CreateOptionNamedAll()
		{
			return new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = LanguageKey.LK_Common_All
			};
		}

		// Token: 0x040090EA RID: 37098
		public string IconPath;

		// Token: 0x040090EB RID: 37099
		public StringKey Text;
	}
}

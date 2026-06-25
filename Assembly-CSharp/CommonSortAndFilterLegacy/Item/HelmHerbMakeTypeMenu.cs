using System;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D9 RID: 1241
	public class HelmHerbMakeTypeMenu : HelmCommonMakeTypeMenu
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06004259 RID: 16985 RVA: 0x00203F2C File Offset: 0x0020212C
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x0600425A RID: 16986 RVA: 0x00203F2F File Offset: 0x0020212F
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 4));
			}
		}

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x0600425B RID: 16987 RVA: 0x00203F3D File Offset: 0x0020213D
		protected override sbyte MyResourceType
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x00203F40 File Offset: 0x00202140
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}
	}
}

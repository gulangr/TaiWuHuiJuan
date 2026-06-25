using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000442 RID: 1090
	public class DummyMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x0600401E RID: 16414 RVA: 0x001FD2DC File Offset: 0x001FB4DC
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x001FD2DF File Offset: 0x001FB4DF
		public DummyMenu(int id)
		{
			this._id = id;
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x001FD2F0 File Offset: 0x001FB4F0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect("Dummy")
			};
		}

		// Token: 0x06004021 RID: 16417 RVA: 0x001FD31C File Offset: 0x001FB51C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004022 RID: 16418 RVA: 0x001FD334 File Offset: 0x001FB534
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return true;
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06004023 RID: 16419 RVA: 0x001FD347 File Offset: 0x001FB547
		public override int Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x04002DC8 RID: 11720
		private int _id;
	}
}

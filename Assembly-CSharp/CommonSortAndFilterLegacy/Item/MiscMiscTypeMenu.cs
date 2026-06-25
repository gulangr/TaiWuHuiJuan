using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000551 RID: 1361
	internal class MiscMiscTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060043F3 RID: 17395 RVA: 0x00208963 File Offset: 0x00206B63
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060043F4 RID: 17396 RVA: 0x00208968 File Offset: 0x00206B68
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060043F5 RID: 17397 RVA: 0x0020898C File Offset: 0x00206B8C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from subType in this._itemSubType
			select StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", subType)) into key
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = key
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060043F6 RID: 17398 RVA: 0x002089F4 File Offset: 0x00206BF4
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MiscItem miscItem = Misc.Instance[data.Key.TemplateId];
			short subType = miscItem.ItemSubType;
			return selectedIndices.Any((int index) => this._itemSubType[index] == subType);
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060043F7 RID: 17399 RVA: 0x00208A47 File Offset: 0x00206C47
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04003004 RID: 12292
		private readonly List<short> _itemSubType = new List<short>
		{
			1204,
			1201
		};
	}
}

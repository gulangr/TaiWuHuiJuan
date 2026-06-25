using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000520 RID: 1312
	public class MaterialTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06004341 RID: 17217 RVA: 0x00206577 File Offset: 0x00204777
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06004342 RID: 17218 RVA: 0x0020657A File Offset: 0x0020477A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004343 RID: 17219 RVA: 0x00206580 File Offset: 0x00204780
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004344 RID: 17220 RVA: 0x002065A4 File Offset: 0x002047A4
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < this._itemSubTypes.Count; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", this._itemSubTypes[i]))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004345 RID: 17221 RVA: 0x00206620 File Offset: 0x00204820
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 5;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int item in selectedIndices)
				{
					short selectedItemSubType = this._itemSubTypes[item];
					bool flag2 = ItemFilterCommon.IsItemMatchItemSubType(data, selectedItemSubType);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04002FBA RID: 12218
		private readonly List<short> _itemSubTypes = new List<short>
		{
			504,
			501,
			503,
			502,
			505,
			506,
			500
		};
	}
}

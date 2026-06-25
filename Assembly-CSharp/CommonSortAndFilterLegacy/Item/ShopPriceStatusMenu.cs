using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055C RID: 1372
	public class ShopPriceStatusMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x0600442E RID: 17454 RVA: 0x0020918C File Offset: 0x0020738C
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x0600442F RID: 17455 RVA: 0x0020918F File Offset: 0x0020738F
		public override int Id
		{
			get
			{
				return 40;
			}
		}

		// Token: 0x06004430 RID: 17456 RVA: 0x00209194 File Offset: 0x00207394
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_Shop_Filter_Price);
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x002091B8 File Offset: 0x002073B8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_Shop_Filter_Price_Origin)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_Shop_Filter_Price_Up)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_Shop_Filter_Price_Down)
				}
			};
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x00209258 File Offset: 0x00207458
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int selectedSubType = enumerator.Current;
					return selectedSubType == (int)data.ItemPriceState;
				}
			}
			return false;
		}
	}
}

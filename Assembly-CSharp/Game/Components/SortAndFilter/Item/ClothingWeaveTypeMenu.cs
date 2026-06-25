using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D73 RID: 3443
	public class ClothingWeaveTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001221 RID: 4641
		// (get) Token: 0x0600A872 RID: 43122 RVA: 0x004E0D5F File Offset: 0x004DEF5F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001222 RID: 4642
		// (get) Token: 0x0600A873 RID: 43123 RVA: 0x004E0D62 File Offset: 0x004DEF62
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A874 RID: 43124 RVA: 0x004E0D68 File Offset: 0x004DEF68
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A875 RID: 43125 RVA: 0x004E0D84 File Offset: 0x004DEF84
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (EClothingWeaveType t = EClothingWeaveType.Normal; t < EClothingWeaveType.Count; t += 1)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_ClothingWeaveType_{0}", (int)t))));
			}
			return configs;
		}

		// Token: 0x0600A876 RID: 43126 RVA: 0x004E0DD4 File Offset: 0x004DEFD4
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			ClothingItem clothingConfig = Clothing.Instance[data.Key.TemplateId];
			bool flag = clothingConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte weaveType = clothingConfig.WeaveType;
				foreach (int selectedSubType in selectedIndices)
				{
					bool flag2 = selectedSubType == (int)weaveType;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DEF RID: 3567
	public class GearMateFeatureEquipTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012BE RID: 4798
		// (get) Token: 0x0600AA95 RID: 43669 RVA: 0x004E8B21 File Offset: 0x004E6D21
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012BF RID: 4799
		// (get) Token: 0x0600AA96 RID: 43670 RVA: 0x004E8B24 File Offset: 0x004E6D24
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA97 RID: 43671 RVA: 0x004E8B28 File Offset: 0x004E6D28
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA98 RID: 43672 RVA: 0x004E8B44 File Offset: 0x004E6D44
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i <= 5; i++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Sub_Equip_{0}", i))));
			}
			return configs;
		}

		// Token: 0x0600AA99 RID: 43673 RVA: 0x004E8B94 File Offset: 0x004E6D94
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			int equipmentType = ItemTemplateHelper.GetEquipmentType(data.Key.ItemType, data.Key.TemplateId);
			if (!true)
			{
			}
			bool result;
			switch (equipmentType)
			{
			case 0:
				result = selectedIndices.Any((int x) => (sbyte)x == 0);
				goto IL_141;
			case 1:
				result = selectedIndices.Any((int x) => (sbyte)x == 1);
				goto IL_141;
			case 3:
				result = selectedIndices.Any((int x) => (sbyte)x == 2);
				goto IL_141;
			case 4:
				result = selectedIndices.Any((int x) => (sbyte)x == 3);
				goto IL_141;
			case 5:
				result = selectedIndices.Any((int x) => (sbyte)x == 4);
				goto IL_141;
			case 6:
				result = selectedIndices.Any((int x) => (sbyte)x == 5);
				goto IL_141;
			}
			result = false;
			IL_141:
			if (!true)
			{
			}
			return result;
		}
	}
}

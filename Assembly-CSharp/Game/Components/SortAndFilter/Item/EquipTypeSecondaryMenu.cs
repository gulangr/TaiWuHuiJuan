using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDD RID: 3549
	public class EquipTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012A7 RID: 4775
		// (get) Token: 0x0600AA40 RID: 43584 RVA: 0x004E7A4B File Offset: 0x004E5C4B
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012A8 RID: 4776
		// (get) Token: 0x0600AA41 RID: 43585 RVA: 0x004E7A4E File Offset: 0x004E5C4E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA42 RID: 43586 RVA: 0x004E7A54 File Offset: 0x004E5C54
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA43 RID: 43587 RVA: 0x004E7A70 File Offset: 0x004E5C70
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 11; i++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Sub_Equip_{0}", i))));
			}
			return configs;
		}

		// Token: 0x0600AA44 RID: 43588 RVA: 0x004E7AC0 File Offset: 0x004E5CC0
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
				break;
			case 1:
				result = selectedIndices.Any((int x) => (sbyte)x == 1);
				break;
			case 2:
				result = selectedIndices.Any((int x) => (sbyte)x == 6);
				break;
			case 3:
				result = selectedIndices.Any((int x) => (sbyte)x == 2);
				break;
			case 4:
				result = selectedIndices.Any((int x) => (sbyte)x == 3);
				break;
			case 5:
				result = selectedIndices.Any((int x) => (sbyte)x == 4);
				break;
			case 6:
				result = selectedIndices.Any((int x) => (sbyte)x == 5);
				break;
			case 7:
				result = selectedIndices.Any((int x) => (sbyte)x == 7);
				break;
			case 8:
				result = selectedIndices.Any((int x) => (sbyte)x == 8);
				break;
			case 9:
				result = selectedIndices.Any((int x) => (sbyte)x == 9);
				break;
			case 10:
				result = selectedIndices.Any((int x) => (sbyte)x == 10);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CF RID: 1231
	public class EquipTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06004231 RID: 16945 RVA: 0x00203827 File Offset: 0x00201A27
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06004232 RID: 16946 RVA: 0x0020382A File Offset: 0x00201A2A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004233 RID: 16947 RVA: 0x00203830 File Offset: 0x00201A30
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004234 RID: 16948 RVA: 0x00203854 File Offset: 0x00201A54
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 11; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Sub_Equip_{0}", i))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x002038B8 File Offset: 0x00201AB8
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
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
				result = selectedIndices.Any(delegate(int x)
				{
					EEquipSubFilterKeys eequipSubFilterKeys = (EEquipSubFilterKeys)x;
					return eequipSubFilterKeys == EEquipSubFilterKeys.LivestockCarrier || eequipSubFilterKeys == EEquipSubFilterKeys.BeastCarrier;
				});
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

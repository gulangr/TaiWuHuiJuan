using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CD RID: 1229
	public class EquipFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06004226 RID: 16934 RVA: 0x00203520 File Offset: 0x00201720
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06004227 RID: 16935 RVA: 0x00203523 File Offset: 0x00201723
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x00203528 File Offset: 0x00201728
		public override bool IsDataMatch(ItemDisplayData data, LineState equipLineState)
		{
			ToggleKey equipToggleState = equipLineState.ToggleGroupState;
			bool isAll = equipToggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = ItemFilterCommon.IsEquip(data.Key.ItemType);
				bool flag2 = flag;
				if (flag2)
				{
					int equipmentType = ItemTemplateHelper.GetEquipmentType(data.Key.ItemType, data.Key.TemplateId);
					if (!true)
					{
					}
					bool flag3;
					switch (equipmentType)
					{
					case 0:
						flag3 = ((sbyte)equipToggleState.Index == 0);
						break;
					case 1:
						flag3 = ((sbyte)equipToggleState.Index == 1);
						break;
					case 2:
						flag3 = ((sbyte)equipToggleState.Index == 6);
						break;
					case 3:
						flag3 = ((sbyte)equipToggleState.Index == 2);
						break;
					case 4:
						flag3 = ((sbyte)equipToggleState.Index == 3);
						break;
					case 5:
						flag3 = ((sbyte)equipToggleState.Index == 4);
						break;
					case 6:
						flag3 = ((sbyte)equipToggleState.Index == 5);
						break;
					case 7:
						flag3 = ((sbyte)equipToggleState.Index == 7);
						break;
					case 8:
						flag3 = ((sbyte)equipToggleState.Index == 8);
						break;
					case 9:
					{
						EEquipSubFilterKeys eequipSubFilterKeys = (EEquipSubFilterKeys)equipToggleState.Index;
						flag3 = (eequipSubFilterKeys == EEquipSubFilterKeys.LivestockCarrier || eequipSubFilterKeys == EEquipSubFilterKeys.BeastCarrier);
						break;
					}
					case 10:
						flag3 = ((sbyte)equipToggleState.Index == 10);
						break;
					default:
						flag3 = false;
						break;
					}
					if (!true)
					{
					}
					flag2 = flag3;
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x0020368C File Offset: 0x0020188C
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			ToggleTransitionIconSpriteNames defaultIconNames = ToggleTransitionIconSpriteNames.Default();
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_0),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_1),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_2),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_3),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_4),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_5),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_6),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_7),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_8),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_9),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_10)
			};
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x002037A8 File Offset: 0x002019A8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}

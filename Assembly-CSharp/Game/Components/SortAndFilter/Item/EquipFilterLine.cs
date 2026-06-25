using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7E RID: 3454
	public class EquipFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x17001235 RID: 4661
		// (get) Token: 0x0600A8AE RID: 43182 RVA: 0x004E151C File Offset: 0x004DF71C
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17001236 RID: 4662
		// (get) Token: 0x0600A8AF RID: 43183 RVA: 0x004E151F File Offset: 0x004DF71F
		protected override bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600A8B0 RID: 43184 RVA: 0x004E1524 File Offset: 0x004DF724
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_0),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_1),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_2),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_3),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_4),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_5),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_6),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_7),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_8),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_9),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Equip_10)
			};
		}

		// Token: 0x0600A8B1 RID: 43185 RVA: 0x004E1664 File Offset: 0x004DF864
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x17001237 RID: 4663
		// (get) Token: 0x0600A8B2 RID: 43186 RVA: 0x004E1677 File Offset: 0x004DF877
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8B3 RID: 43187 RVA: 0x004E167C File Offset: 0x004DF87C
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
		{
			ToggleKey equipToggleState = lineState.ToggleGroupState;
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
	}
}

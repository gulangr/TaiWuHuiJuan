using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB0 RID: 3504
	public class MaterialHerbTreatmentMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001267 RID: 4711
		// (get) Token: 0x0600A966 RID: 43366 RVA: 0x004E521F File Offset: 0x004E341F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001268 RID: 4712
		// (get) Token: 0x0600A967 RID: 43367 RVA: 0x004E5222 File Offset: 0x004E3422
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A968 RID: 43368 RVA: 0x004E5228 File Offset: 0x004E3428
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Treatment;
		}

		// Token: 0x0600A969 RID: 43369 RVA: 0x004E5244 File Offset: 0x004E3444
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_4")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_5")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_2"))
			};
		}

		// Token: 0x0600A96A RID: 43370 RVA: 0x004E52CC File Offset: 0x004E34CC
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			List<MedicineItem> resultItems = CommonUtils.GetResultMedicine(data.Key.TemplateId);
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = resultItems.Any((MedicineItem t) => t.BreakBonusEffect == 16);
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = resultItems.Any((MedicineItem t) => t.BreakBonusEffect == 17);
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = resultItems.Any((MedicineItem t) => t.BreakBonusEffect == 19);
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = resultItems.Any((MedicineItem t) => t.BreakBonusEffect == 20);
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = resultItems.Any((MedicineItem t) => t.BreakBonusEffect == 18);
						if (flag5)
						{
							return true;
						}
						break;
					}
					}
				}
			}
			return false;
		}
	}
}

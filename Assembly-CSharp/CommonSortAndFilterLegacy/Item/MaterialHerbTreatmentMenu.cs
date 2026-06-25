using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200052A RID: 1322
	public class MaterialHerbTreatmentMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06004356 RID: 17238 RVA: 0x002067DB File Offset: 0x002049DB
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06004357 RID: 17239 RVA: 0x002067DE File Offset: 0x002049DE
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004358 RID: 17240 RVA: 0x002067E4 File Offset: 0x002049E4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Treatment);
		}

		// Token: 0x06004359 RID: 17241 RVA: 0x00206808 File Offset: 0x00204A08
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_4")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_5")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_2")
				}
			};
		}

		// Token: 0x0600435A RID: 17242 RVA: 0x00206904 File Offset: 0x00204B04
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
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

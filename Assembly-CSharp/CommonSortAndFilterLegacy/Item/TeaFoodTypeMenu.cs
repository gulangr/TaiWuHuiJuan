using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004FF RID: 1279
	public class TeaFoodTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x060042DD RID: 17117 RVA: 0x00205493 File Offset: 0x00203693
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x060042DE RID: 17118 RVA: 0x00205496 File Offset: 0x00203696
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060042DF RID: 17119 RVA: 0x0020549C File Offset: 0x0020369C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060042E0 RID: 17120 RVA: 0x002054C0 File Offset: 0x002036C0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_3")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_5")
				}
			};
		}

		// Token: 0x060042E1 RID: 17121 RVA: 0x00205534 File Offset: 0x00203734
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			TeaWineItem food = TeaWine.Instance[data.Key.TemplateId];
			foreach (int selectedSubType in selectedIndices)
			{
				EFoodTeaDetailType efoodTeaDetailType = (EFoodTeaDetailType)selectedSubType;
				EFoodTeaDetailType efoodTeaDetailType2 = efoodTeaDetailType;
				if (efoodTeaDetailType2 != EFoodTeaDetailType.Defence)
				{
					if (efoodTeaDetailType2 == EFoodTeaDetailType.Defuse)
					{
						bool flag = food.AvoidRateStrength > 0 || food.AvoidRateTechnique > 0 || food.AvoidRateSpeed > 0 || food.AvoidRateMind > 0;
						if (flag)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag2 = food.PenetrateResistOfOuter > 0 || food.PenetrateResistOfInner > 0;
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

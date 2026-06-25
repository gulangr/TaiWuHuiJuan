using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D89 RID: 3465
	public class TeaFoodTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001248 RID: 4680
		// (get) Token: 0x0600A8E3 RID: 43235 RVA: 0x004E2027 File Offset: 0x004E0227
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001249 RID: 4681
		// (get) Token: 0x0600A8E4 RID: 43236 RVA: 0x004E202A File Offset: 0x004E022A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8E5 RID: 43237 RVA: 0x004E2030 File Offset: 0x004E0230
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A8E6 RID: 43238 RVA: 0x004E204C File Offset: 0x004E024C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_3")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_5"))
			};
		}

		// Token: 0x0600A8E7 RID: 43239 RVA: 0x004E2090 File Offset: 0x004E0290
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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

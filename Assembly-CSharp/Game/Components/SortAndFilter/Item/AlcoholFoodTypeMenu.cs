using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D8B RID: 3467
	public class AlcoholFoodTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700124A RID: 4682
		// (get) Token: 0x0600A8E9 RID: 43241 RVA: 0x004E2165 File Offset: 0x004E0365
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700124B RID: 4683
		// (get) Token: 0x0600A8EA RID: 43242 RVA: 0x004E2168 File Offset: 0x004E0368
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8EB RID: 43243 RVA: 0x004E216C File Offset: 0x004E036C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A8EC RID: 43244 RVA: 0x004E2188 File Offset: 0x004E0388
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_4"))
			};
		}

		// Token: 0x0600A8ED RID: 43245 RVA: 0x004E21CC File Offset: 0x004E03CC
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			TeaWineItem food = TeaWine.Instance[data.Key.TemplateId];
			foreach (int selectedSubType in selectedIndices)
			{
				EFoodAlcoholDetailType efoodAlcoholDetailType = (EFoodAlcoholDetailType)selectedSubType;
				EFoodAlcoholDetailType efoodAlcoholDetailType2 = efoodAlcoholDetailType;
				if (efoodAlcoholDetailType2 != EFoodAlcoholDetailType.Attack)
				{
					if (efoodAlcoholDetailType2 == EFoodAlcoholDetailType.HitRate)
					{
						bool flag = food.HitRateStrength > 0 || food.HitRateTechnique > 0 || food.HitRateSpeed > 0 || food.HitRateMind > 0;
						if (flag)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag2 = food.PenetrateOfOuter > 0 || food.PenetrateOfInner > 0;
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

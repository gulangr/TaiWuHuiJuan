using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004F4 RID: 1268
	public class AlcoholFoodTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x060042BB RID: 17083 RVA: 0x00204BC3 File Offset: 0x00202DC3
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x060042BC RID: 17084 RVA: 0x00204BC6 File Offset: 0x00202DC6
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x00204BCC File Offset: 0x00202DCC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x00204BF0 File Offset: 0x00202DF0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MedicineBuff_4")
				}
			};
		}

		// Token: 0x060042BF RID: 17087 RVA: 0x00204C64 File Offset: 0x00202E64
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
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

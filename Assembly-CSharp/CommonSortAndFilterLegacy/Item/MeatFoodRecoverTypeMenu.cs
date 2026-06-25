using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004FA RID: 1274
	public class MeatFoodRecoverTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x060042CC RID: 17100 RVA: 0x00204F23 File Offset: 0x00203123
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x060042CD RID: 17101 RVA: 0x00204F26 File Offset: 0x00203126
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060042CE RID: 17102 RVA: 0x00204F2C File Offset: 0x0020312C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Recover);
		}

		// Token: 0x060042CF RID: 17103 RVA: 0x00204F50 File Offset: 0x00203150
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Strength")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Dexterity")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Vitality")
				}
			};
		}

		// Token: 0x060042D0 RID: 17104 RVA: 0x00204FF0 File Offset: 0x002031F0
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			FoodItem food = Food.Instance[data.Key.TemplateId];
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = food.MainAttributesRegen.Get(0) != 0;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = food.MainAttributesRegen.Get(1) != 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = food.MainAttributesRegen.Get(3) != 0;
						if (flag3)
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

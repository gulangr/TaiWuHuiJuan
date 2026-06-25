using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000504 RID: 1284
	public class FoodRecoverTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x060042E8 RID: 17128 RVA: 0x00205653 File Offset: 0x00203853
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x060042E9 RID: 17129 RVA: 0x00205656 File Offset: 0x00203856
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x0020565C File Offset: 0x0020385C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Recover);
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x00205680 File Offset: 0x00203880
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Concentration")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Energy")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Main_Attribute_Intelligence")
				}
			};
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x00205720 File Offset: 0x00203920
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
						bool flag = food.MainAttributesRegen.Get(2) != 0;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = food.MainAttributesRegen.Get(4) != 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = food.MainAttributesRegen.Get(5) != 0;
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

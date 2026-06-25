using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004FB RID: 1275
	public class MeatFoodBuffTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x060042D2 RID: 17106 RVA: 0x002050DD File Offset: 0x002032DD
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x060042D3 RID: 17107 RVA: 0x002050E0 File Offset: 0x002032E0
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060042D4 RID: 17108 RVA: 0x002050E4 File Offset: 0x002032E4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Buff);
		}

		// Token: 0x060042D5 RID: 17109 RVA: 0x00205108 File Offset: 0x00203308
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Outer")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Inner")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_3")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Resist_Outer")
				}
			};
		}

		// Token: 0x060042D6 RID: 17110 RVA: 0x002052B8 File Offset: 0x002034B8
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
						bool flag = food.PenetrateOfOuter > 0;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = food.PenetrateOfInner > 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = food.HitRateStrength > 0;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = food.HitRateTechnique > 0;
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = food.HitRateSpeed > 0;
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = food.HitRateMind > 0;
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = food.AvoidRateStrength > 0;
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = food.AvoidRateSpeed > 0;
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = food.PenetrateResistOfOuter > 0;
						if (flag9)
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

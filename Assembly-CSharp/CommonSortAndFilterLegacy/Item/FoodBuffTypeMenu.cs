using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000505 RID: 1285
	public class FoodBuffTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x060042EE RID: 17134 RVA: 0x0020580D File Offset: 0x00203A0D
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x060042EF RID: 17135 RVA: 0x00205810 File Offset: 0x00203A10
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060042F0 RID: 17136 RVA: 0x00205814 File Offset: 0x00203A14
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Buff);
		}

		// Token: 0x060042F1 RID: 17137 RVA: 0x00205838 File Offset: 0x00203A38
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Food_Prop_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_3")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Resist_Inner")
				}
			};
		}

		// Token: 0x060042F2 RID: 17138 RVA: 0x00205908 File Offset: 0x00203B08
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
						bool flag = food.BreakBonusEffect == 35;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = food.AvoidRateTechnique != 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = food.AvoidRateMind != 0;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = food.PenetrateResistOfInner != 0;
						if (flag4)
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

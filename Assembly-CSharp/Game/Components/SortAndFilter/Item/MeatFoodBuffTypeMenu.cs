using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D85 RID: 3461
	public class MeatFoodBuffTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001242 RID: 4674
		// (get) Token: 0x0600A8D3 RID: 43219 RVA: 0x004E1CFD File Offset: 0x004DFEFD
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001243 RID: 4675
		// (get) Token: 0x0600A8D4 RID: 43220 RVA: 0x004E1D00 File Offset: 0x004DFF00
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A8D5 RID: 43221 RVA: 0x004E1D04 File Offset: 0x004DFF04
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Buff;
		}

		// Token: 0x0600A8D6 RID: 43222 RVA: 0x004E1D20 File Offset: 0x004DFF20
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Outer")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Inner")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_3")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Resist_Outer"))
			};
		}

		// Token: 0x0600A8D7 RID: 43223 RVA: 0x004E1E00 File Offset: 0x004E0000
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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

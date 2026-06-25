using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D82 RID: 3458
	public class FoodBuffTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700123C RID: 4668
		// (get) Token: 0x0600A8C2 RID: 43202 RVA: 0x004E19C9 File Offset: 0x004DFBC9
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700123D RID: 4669
		// (get) Token: 0x0600A8C3 RID: 43203 RVA: 0x004E19CC File Offset: 0x004DFBCC
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A8C4 RID: 43204 RVA: 0x004E19D0 File Offset: 0x004DFBD0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Buff;
		}

		// Token: 0x0600A8C5 RID: 43205 RVA: 0x004E19EC File Offset: 0x004DFBEC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Food_Prop_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_3")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Resist_Inner"))
			};
		}

		// Token: 0x0600A8C6 RID: 43206 RVA: 0x004E1A5C File Offset: 0x004DFC5C
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

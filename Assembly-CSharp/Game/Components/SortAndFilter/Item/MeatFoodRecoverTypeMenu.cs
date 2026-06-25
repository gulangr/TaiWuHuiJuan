using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D84 RID: 3460
	public class MeatFoodRecoverTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001240 RID: 4672
		// (get) Token: 0x0600A8CD RID: 43213 RVA: 0x004E1B8F File Offset: 0x004DFD8F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001241 RID: 4673
		// (get) Token: 0x0600A8CE RID: 43214 RVA: 0x004E1B92 File Offset: 0x004DFD92
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8CF RID: 43215 RVA: 0x004E1B98 File Offset: 0x004DFD98
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Recover;
		}

		// Token: 0x0600A8D0 RID: 43216 RVA: 0x004E1BB4 File Offset: 0x004DFDB4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Strength")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Dexterity")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Vitality"))
			};
		}

		// Token: 0x0600A8D1 RID: 43217 RVA: 0x004E1C10 File Offset: 0x004DFE10
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

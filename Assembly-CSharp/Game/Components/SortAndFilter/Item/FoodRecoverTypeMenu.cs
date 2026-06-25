using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D81 RID: 3457
	public class FoodRecoverTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700123A RID: 4666
		// (get) Token: 0x0600A8BC RID: 43196 RVA: 0x004E185B File Offset: 0x004DFA5B
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700123B RID: 4667
		// (get) Token: 0x0600A8BD RID: 43197 RVA: 0x004E185E File Offset: 0x004DFA5E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8BE RID: 43198 RVA: 0x004E1864 File Offset: 0x004DFA64
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Recover;
		}

		// Token: 0x0600A8BF RID: 43199 RVA: 0x004E1880 File Offset: 0x004DFA80
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Concentration")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Energy")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Main_Attribute_Intelligence"))
			};
		}

		// Token: 0x0600A8C0 RID: 43200 RVA: 0x004E18DC File Offset: 0x004DFADC
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

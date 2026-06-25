using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDF RID: 3551
	public class BookTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012AB RID: 4779
		// (get) Token: 0x0600AA4B RID: 43595 RVA: 0x004E7D4F File Offset: 0x004E5F4F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012AC RID: 4780
		// (get) Token: 0x0600AA4C RID: 43596 RVA: 0x004E7D52 File Offset: 0x004E5F52
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA4D RID: 43597 RVA: 0x004E7D58 File Offset: 0x004E5F58
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA4E RID: 43598 RVA: 0x004E7D74 File Offset: 0x004E5F74
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1001))),
				new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1000)))
			};
		}

		// Token: 0x0600AA4F RID: 43599 RVA: 0x004E7DD8 File Offset: 0x004E5FD8
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				EBookSubFilterKeys ebookSubFilterKeys = (EBookSubFilterKeys)selectedSubType;
				EBookSubFilterKeys ebookSubFilterKeys2 = ebookSubFilterKeys;
				if (ebookSubFilterKeys2 != EBookSubFilterKeys.CombatSkill)
				{
					if (ebookSubFilterKeys2 == EBookSubFilterKeys.LifeSkill)
					{
						bool flag = ItemFilterCommon.IsItemMatchItemSubType(data, 1000);
						if (flag)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag2 = ItemFilterCommon.IsItemMatchItemSubType(data, 1001);
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

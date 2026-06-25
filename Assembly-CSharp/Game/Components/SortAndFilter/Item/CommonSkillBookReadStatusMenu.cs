using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D2C RID: 3372
	public abstract class CommonSkillBookReadStatusMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011AB RID: 4523
		// (get) Token: 0x0600A790 RID: 42896 RVA: 0x004DF3EF File Offset: 0x004DD5EF
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A791 RID: 42897 RVA: 0x004DF3F4 File Offset: 0x004DD5F4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Reading;
		}

		// Token: 0x0600A792 RID: 42898 RVA: 0x004DF410 File Offset: 0x004DD610
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_0),
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_1),
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_2)
			};
		}

		// Token: 0x0600A793 RID: 42899 RVA: 0x004DF46C File Offset: 0x004DD66C
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
						{
							bool isReadingFinished = data.IsReadingFinished;
							if (isReadingFinished)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag2 = data.UsingType == ItemDisplayData.ItemUsingType.Reading;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag3 = !data.IsReadingFinished && data.UsingType > ItemDisplayData.ItemUsingType.Reading;
							if (flag3)
							{
								return true;
							}
							break;
						}
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}

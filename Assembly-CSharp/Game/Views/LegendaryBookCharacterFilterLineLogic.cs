using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using GameData.Domains.LegendaryBook;

namespace Game.Views
{
	// Token: 0x020006EE RID: 1774
	public class LegendaryBookCharacterFilterLineLogic : FilterLineBase<LegendaryBookCharacterRelatedData>
	{
		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06005447 RID: 21575 RVA: 0x002706A1 File Offset: 0x0026E8A1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06005448 RID: 21576 RVA: 0x002706A4 File Offset: 0x0026E8A4
		public override LineConfig GenerateConfig()
		{
			return LineConfig.CreateDetailedFilterLineConfig(new DetailedFilterLineConfig(new DetailedFilterConfig
			{
				MenuConfigs = new List<DetailedFilterMenuConfig>
				{
					new DetailedFilterMenuConfig
					{
						Id = 1,
						DropdownConfig = new FilterDropdownConfig
						{
							IsMultiSelect = true,
							MenuBarLabel = LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_0,
							ItemConfigs = new List<FilterDropdownItemConfig>
							{
								new FilterDropdownItemConfig(LanguageKey.LK_Gender_Woman),
								new FilterDropdownItemConfig(LanguageKey.LK_Gender_Man)
							}
						}
					},
					new DetailedFilterMenuConfig
					{
						Id = 2,
						DropdownConfig = new FilterDropdownConfig
						{
							IsMultiSelect = true,
							MenuBarLabel = LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_1,
							ItemConfigs = new List<FilterDropdownItemConfig>
							{
								new FilterDropdownItemConfig(LanguageKey.LK_Gender_Woman),
								new FilterDropdownItemConfig(LanguageKey.LK_Gender_Man)
							}
						}
					},
					new DetailedFilterMenuConfig
					{
						Id = 3,
						DropdownConfig = new FilterDropdownConfig
						{
							IsMultiSelect = true,
							MenuBarLabel = LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_2,
							ItemConfigs = new List<FilterDropdownItemConfig>()
						}
					}
				}
			}));
		}

		// Token: 0x06005449 RID: 21577 RVA: 0x00270810 File Offset: 0x0026EA10
		public override bool IsDataMatch(LegendaryBookCharacterRelatedData data, LineState lineState)
		{
			string text = ViewLegendaryBookCharacters.SearchingText;
			return string.IsNullOrWhiteSpace(text) || NameCenter.GetDisplayName(ref data.NameRelatedData, false).Contains(text.Trim()) || NameCenter.GetMonasticTitleOrDisplayName(ref data.NameRelatedData, false, false).Contains(text.Trim());
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x00270864 File Offset: 0x0026EA64
		public override DynamicLineConfig GenerateDynamicConfig(IEnumerable<LegendaryBookCharacterRelatedData> dataList)
		{
			return null;
		}
	}
}

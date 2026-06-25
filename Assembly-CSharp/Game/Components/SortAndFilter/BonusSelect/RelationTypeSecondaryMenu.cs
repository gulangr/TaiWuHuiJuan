using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E98 RID: 3736
	public class RelationTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001393 RID: 5011
		// (get) Token: 0x0600AD45 RID: 44357 RVA: 0x004F121F File Offset: 0x004EF41F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001394 RID: 5012
		// (get) Token: 0x0600AD46 RID: 44358 RVA: 0x004F1222 File Offset: 0x004EF422
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD47 RID: 44359 RVA: 0x004F1228 File Offset: 0x004EF428
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AD48 RID: 44360 RVA: 0x004F1244 File Offset: 0x004EF444
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_RelationShip_Adored
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_RelationShip_Enemy
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_RelationShip_Friend
				}
			};
		}

		// Token: 0x0600AD49 RID: 44361 RVA: 0x004F12C0 File Offset: 0x004EF4C0
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.Character;
			bool result;
			if (flag)
			{
				result = false;
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
							bool flag2 = data.BonusData.Effect.TemplateId == 33;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = data.BonusData.Effect.TemplateId == 34;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = data.BonusData.Effect.TemplateId == 47;
							if (flag4)
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

		// Token: 0x040085D2 RID: 34258
		public static readonly sbyte[] RelationFilterTypes = new sbyte[]
		{
			33,
			34,
			47
		};
	}
}

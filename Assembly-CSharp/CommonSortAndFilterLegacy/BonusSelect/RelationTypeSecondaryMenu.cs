using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005C1 RID: 1473
	public class RelationTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x0600460B RID: 17931 RVA: 0x0020DD1F File Offset: 0x0020BF1F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x0600460C RID: 17932 RVA: 0x0020DD22 File Offset: 0x0020BF22
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x0020DD28 File Offset: 0x0020BF28
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x0600460E RID: 17934 RVA: 0x0020DD4C File Offset: 0x0020BF4C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = LanguageKey.LK_RelationShip_Adored
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = LanguageKey.LK_RelationShip_Enemy
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = LanguageKey.LK_RelationShip_Friend
				}
			};
		}

		// Token: 0x0600460F RID: 17935 RVA: 0x0020DDEC File Offset: 0x0020BFEC
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

		// Token: 0x04003090 RID: 12432
		public static readonly sbyte[] RelationFilterTypes = new sbyte[]
		{
			33,
			34,
			47
		};
	}
}

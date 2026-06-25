using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E56 RID: 3670
	public class CharacterTaskStatusMenu : DetailedFilterMenuLogic<int>
	{
		// Token: 0x0600AC36 RID: 44086 RVA: 0x004EDD36 File Offset: 0x004EBF36
		public CharacterTaskStatusMenu(CharacterDetailDisplayDataSortAndFilterControllerController controller)
		{
			this._controller = controller;
		}

		// Token: 0x17001340 RID: 4928
		// (get) Token: 0x0600AC37 RID: 44087 RVA: 0x004EDD46 File Offset: 0x004EBF46
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001341 RID: 4929
		// (get) Token: 0x0600AC38 RID: 44088 RVA: 0x004EDD49 File Offset: 0x004EBF49
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AC39 RID: 44089 RVA: 0x004EDD4C File Offset: 0x004EBF4C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_VillagerRole_WorkStatus;
		}

		// Token: 0x0600AC3A RID: 44090 RVA: 0x004EDD58 File Offset: 0x004EBF58
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_VillagerRole_WorkStatus_Idle
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_VillagerRole_WorkStatus_InVillage
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_VillagerRole_WorkStatus_Assigned
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_VillagerRole_WorkStatus_AsRole
				}
			};
		}

		// Token: 0x0600AC3B RID: 44091 RVA: 0x004EDDF0 File Offset: 0x004EBFF0
		public override bool IsDataMatch(int data, IReadOnlyCollection<int> selectedIndices)
		{
			VillagerRoleCharacterDisplayData villager;
			return this._controller.Data.Villagers.TryGetValue(data, out villager) && selectedIndices.Any(delegate(int index)
			{
				if (!true)
				{
				}
				bool result;
				switch (index)
				{
				case 0:
				{
					VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = villager.ArrangementDisplayData;
					bool flag;
					if (arrangementDisplayData == null || arrangementDisplayData.ArrangementTemplateId == -1)
					{
						VillagerWorkData villagerWorkData = villager.VillagerWorkData;
						flag = (villagerWorkData == null || villagerWorkData.WorkType == -1);
					}
					else
					{
						flag = false;
					}
					result = flag;
					break;
				}
				case 1:
				{
					VillagerWorkData villagerWorkData = villager.VillagerWorkData;
					bool flag2;
					if (villagerWorkData != null)
					{
						sbyte workType = villagerWorkData.WorkType;
						if (workType < 10)
						{
							flag2 = (workType > -1);
							goto IL_80;
						}
					}
					flag2 = false;
					IL_80:
					result = flag2;
					break;
				}
				case 2:
				{
					VillagerWorkData villagerWorkData = villager.VillagerWorkData;
					bool flag3;
					if (villagerWorkData != null)
					{
						sbyte workType = villagerWorkData.WorkType;
						if (workType - 12 <= 1)
						{
							flag3 = true;
							goto IL_AA;
						}
					}
					flag3 = false;
					IL_AA:
					result = flag3;
					break;
				}
				case 3:
				{
					VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = villager.ArrangementDisplayData;
					result = (arrangementDisplayData != null && arrangementDisplayData.ArrangementTemplateId != -1);
					break;
				}
				default:
					result = false;
					break;
				}
				if (!true)
				{
				}
				return result;
			});
		}

		// Token: 0x04008545 RID: 34117
		private readonly CharacterDetailDisplayDataSortAndFilterControllerController _controller;
	}
}

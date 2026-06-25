using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CEC RID: 3308
	public class TaiwuFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001172 RID: 4466
		// (get) Token: 0x0600A679 RID: 42617 RVA: 0x004D74C2 File Offset: 0x004D56C2
		public override int Id
		{
			get
			{
				return 12;
			}
		}

		// Token: 0x0600A67A RID: 42618 RVA: 0x004D74C6 File Offset: 0x004D56C6
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_UI_Following_SubTitle_List;
		}

		// Token: 0x0600A67B RID: 42619 RVA: 0x004D74D4 File Offset: 0x004D56D4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Common_All)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Villager)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_6)
				}
			};
		}

		// Token: 0x0600A67C RID: 42620 RVA: 0x004D7570 File Offset: 0x004D5770
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			ITaiwuSelectCharacterData wrapper = data as ITaiwuSelectCharacterData;
			bool flag = wrapper == null;
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
						case 1:
						{
							bool isTaiwuTeammate = wrapper.IsTaiwuTeammate;
							if (isTaiwuTeammate)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag2 = wrapper.GetGeneralScrollListData().OrgInfo.OrgTemplateId == 16;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 3:
						{
							bool flag3 = !wrapper.IsTaiwuTeammate && wrapper.GetGeneralScrollListData().OrgInfo.OrgTemplateId != 16;
							if (flag3)
							{
								return true;
							}
							break;
						}
						default:
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}

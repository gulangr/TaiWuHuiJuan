using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E73 RID: 3699
	public class CharacterStatusMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600AC9E RID: 44190 RVA: 0x004EF044 File Offset: 0x004ED244
		public CharacterStatusMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x17001367 RID: 4967
		// (get) Token: 0x0600AC9F RID: 44191 RVA: 0x004EF054 File Offset: 0x004ED254
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001368 RID: 4968
		// (get) Token: 0x0600ACA0 RID: 44192 RVA: 0x004EF057 File Offset: 0x004ED257
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600ACA1 RID: 44193 RVA: 0x004EF05A File Offset: 0x004ED25A
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_State;
		}

		// Token: 0x0600ACA2 RID: 44194 RVA: 0x004EF068 File Offset: 0x004ED268
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_MapBlockChar_Filter_0
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_MapBlockChar_Filter_1
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_MapBlockChar_Filter_2
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_MapBlockChar_Filter_3
				}
			};
		}

		// Token: 0x0600ACA3 RID: 44195 RVA: 0x004EF100 File Offset: 0x004ED300
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				if (!true)
				{
				}
				bool result;
				switch (index)
				{
				case 0:
					result = (data != null && data.FavorabilityToTaiwu != short.MinValue);
					break;
				case 1:
					result = (data != null && data.FavorabilityToTaiwu == short.MinValue);
					break;
				case 2:
				{
					bool flag;
					if (data != null)
					{
						CharacterDisplayDataSortAndFilterController controller = this._controller;
						flag = (controller != null && controller.Data.InteractedCharSet.Contains(data.CharacterId));
					}
					else
					{
						flag = false;
					}
					result = flag;
					break;
				}
				case 3:
				{
					bool flag2;
					if (data != null)
					{
						CharacterDisplayDataSortAndFilterController controller2 = this._controller;
						flag2 = (controller2 != null && !controller2.Data.InteractedCharSet.Contains(data.CharacterId));
					}
					else
					{
						flag2 = false;
					}
					result = flag2;
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

		// Token: 0x040085B6 RID: 34230
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}

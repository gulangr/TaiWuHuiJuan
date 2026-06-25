using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E74 RID: 3700
	public class CharacterBehaviourMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600ACA4 RID: 44196 RVA: 0x004EF133 File Offset: 0x004ED333
		public CharacterBehaviourMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x17001369 RID: 4969
		// (get) Token: 0x0600ACA5 RID: 44197 RVA: 0x004EF143 File Offset: 0x004ED343
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700136A RID: 4970
		// (get) Token: 0x0600ACA6 RID: 44198 RVA: 0x004EF146 File Offset: 0x004ED346
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600ACA7 RID: 44199 RVA: 0x004EF149 File Offset: 0x004ED349
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_VillagerRole_WorkStatus;
		}

		// Token: 0x0600ACA8 RID: 44200 RVA: 0x004EF158 File Offset: 0x004ED358
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = Config.BehaviorType.DefValue.Just.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Config.BehaviorType.DefValue.Kind.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Config.BehaviorType.DefValue.Even.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Config.BehaviorType.DefValue.Rebel.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Config.BehaviorType.DefValue.Egoistic.Name
				}
			};
		}

		// Token: 0x0600ACA9 RID: 44201 RVA: 0x004EF228 File Offset: 0x004ED428
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => data != null && (int)GameData.Domains.Character.BehaviorType.GetBehaviorType((short)data.BehaviorType) == index);
		}

		// Token: 0x040085B7 RID: 34231
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}

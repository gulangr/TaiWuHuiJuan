using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E75 RID: 3701
	public class CharacterRelationMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600ACAA RID: 44202 RVA: 0x004EF254 File Offset: 0x004ED454
		public CharacterRelationMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x1700136B RID: 4971
		// (get) Token: 0x0600ACAB RID: 44203 RVA: 0x004EF264 File Offset: 0x004ED464
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700136C RID: 4972
		// (get) Token: 0x0600ACAC RID: 44204 RVA: 0x004EF267 File Offset: 0x004ED467
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x0600ACAD RID: 44205 RVA: 0x004EF26A File Offset: 0x004ED46A
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_VillagerRole_WorkStatus;
		}

		// Token: 0x0600ACAE RID: 44206 RVA: 0x004EF278 File Offset: 0x004ED478
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

		// Token: 0x0600ACAF RID: 44207 RVA: 0x004EF348 File Offset: 0x004ED548
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => data != null && (int)GameData.Domains.Character.BehaviorType.GetBehaviorType((short)data.BehaviorType) == index);
		}

		// Token: 0x040085B8 RID: 34232
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}

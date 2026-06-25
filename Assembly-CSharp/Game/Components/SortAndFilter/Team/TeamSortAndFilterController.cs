using System;
using System.Collections.Generic;
using Game.Views.CharacterMenu.Team;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Team
{
	// Token: 0x02000CD8 RID: 3288
	public class TeamSortAndFilterController : SortAndFilterController<GroupCharDisplayData>
	{
		// Token: 0x0600A61C RID: 42524 RVA: 0x004D4B8B File Offset: 0x004D2D8B
		public TeamSortAndFilterController(SortAndFilter sortAndFilter, Func<int, bool> isTaiwuFunc) : base(sortAndFilter, LanguageKey.LK_CommonSortAndFilter_FilterPanel_Title_Character)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this.SortController = new TeamSortController(isTaiwuFunc);
		}

		// Token: 0x0600A61D RID: 42525 RVA: 0x004D4BB5 File Offset: 0x004D2DB5
		public void SetSubPage(TeamSubPage subPage)
		{
			this._currentSubPage = subPage;
		}

		// Token: 0x0600A61E RID: 42526 RVA: 0x004D4BBF File Offset: 0x004D2DBF
		protected override IEnumerable<FilterLineBase<GroupCharDisplayData>> GenerateFilterLines()
		{
			yield return new TeamCharacterMainFilterLine(TeamSortAndFilterController.DefaultFilterMenuIds);
			yield break;
		}

		// Token: 0x04008305 RID: 33541
		private static readonly List<ESelectCharacterFilterMenuId> DefaultFilterMenuIds = new List<ESelectCharacterFilterMenuId>
		{
			ESelectCharacterFilterMenuId.Gender,
			ESelectCharacterFilterMenuId.BehaviorType,
			ESelectCharacterFilterMenuId.Organization,
			ESelectCharacterFilterMenuId.Sect
		};

		// Token: 0x04008306 RID: 33542
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04008307 RID: 33543
		private TeamSubPage _currentSubPage = TeamSubPage.State;
	}
}

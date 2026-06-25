using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Recruit;
using Game.Views.Select;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CE8 RID: 3304
	public class RecruitCharacterSortAndFilterController : SortAndFilterController<BuildingRecruitCharacterData>
	{
		// Token: 0x0600A66B RID: 42603 RVA: 0x004D720A File Offset: 0x004D540A
		public RecruitCharacterSortAndFilterController(SortAndFilter sortAndFilter, List<ESelectCharacterFilterMenuId> menuIds) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._basicMenuIds = (menuIds ?? new List<ESelectCharacterFilterMenuId>());
			this.SortController = new RecruitCharacterSortController();
		}

		// Token: 0x0600A66C RID: 42604 RVA: 0x004D7231 File Offset: 0x004D5431
		protected override IEnumerable<FilterLineBase<BuildingRecruitCharacterData>> GenerateFilterLines()
		{
			bool flag = this._basicMenuIds.Count > 0;
			if (flag)
			{
				yield return new RecruitCharacterMainFilterLine(this._basicMenuIds);
			}
			yield break;
		}

		// Token: 0x04008318 RID: 33560
		private readonly List<ESelectCharacterFilterMenuId> _basicMenuIds;
	}
}

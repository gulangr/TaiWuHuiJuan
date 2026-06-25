using System;
using System.Collections.Generic;
using Game.Views.Select;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D0A RID: 3338
	public class RecruitCharacterMainFilterLine : DetailedFilterLineLogic<BuildingRecruitCharacterData>
	{
		// Token: 0x0600A6FF RID: 42751 RVA: 0x004DAEBF File Offset: 0x004D90BF
		public RecruitCharacterMainFilterLine(List<ESelectCharacterFilterMenuId> menuIds)
		{
			this._menuIds = (menuIds ?? new List<ESelectCharacterFilterMenuId>());
		}

		// Token: 0x17001189 RID: 4489
		// (get) Token: 0x0600A700 RID: 42752 RVA: 0x004DAED9 File Offset: 0x004D90D9
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700118A RID: 4490
		// (get) Token: 0x0600A701 RID: 42753 RVA: 0x004DAEDC File Offset: 0x004D90DC
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700118B RID: 4491
		// (get) Token: 0x0600A702 RID: 42754 RVA: 0x004DAEDF File Offset: 0x004D90DF
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A703 RID: 42755 RVA: 0x004DAEE2 File Offset: 0x004D90E2
		protected override IEnumerable<DetailedFilterMenuLogic<BuildingRecruitCharacterData>> GenerateMenus()
		{
			foreach (ESelectCharacterFilterMenuId menuId in this._menuIds)
			{
				DetailedFilterMenuLogic<BuildingRecruitCharacterData> menu = RecruitCharacterFilterMenuFactory.CreateBasicMenu(menuId);
				bool flag = menu != null;
				if (flag)
				{
					yield return menu;
				}
				menu = null;
			}
			List<ESelectCharacterFilterMenuId>.Enumerator enumerator = default(List<ESelectCharacterFilterMenuId>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600A704 RID: 42756 RVA: 0x004DAEF4 File Offset: 0x004D90F4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x04008346 RID: 33606
		private readonly List<ESelectCharacterFilterMenuId> _menuIds;
	}
}

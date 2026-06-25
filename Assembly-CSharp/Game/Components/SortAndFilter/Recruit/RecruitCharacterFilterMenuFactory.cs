using System;
using Game.Views.Select;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D09 RID: 3337
	public static class RecruitCharacterFilterMenuFactory
	{
		// Token: 0x0600A6FE RID: 42750 RVA: 0x004DAE84 File Offset: 0x004D9084
		public static DetailedFilterMenuLogic<BuildingRecruitCharacterData> CreateBasicMenu(ESelectCharacterFilterMenuId menuId)
		{
			if (!true)
			{
			}
			DetailedFilterMenuLogic<BuildingRecruitCharacterData> result;
			if (menuId != ESelectCharacterFilterMenuId.Gender)
			{
				if (menuId != ESelectCharacterFilterMenuId.Taiwu)
				{
					result = null;
				}
				else
				{
					result = new TaiwuFilterMenu<BuildingRecruitCharacterData>();
				}
			}
			else
			{
				result = new GenderFilterMenu<BuildingRecruitCharacterData>();
			}
			if (!true)
			{
			}
			return result;
		}
	}
}

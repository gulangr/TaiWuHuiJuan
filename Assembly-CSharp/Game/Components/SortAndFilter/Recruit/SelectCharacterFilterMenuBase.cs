using System;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D07 RID: 3335
	public abstract class SelectCharacterFilterMenuBase<TData> : DetailedFilterMenuLogic<TData> where TData : BuildingRecruitCharacterData
	{
		// Token: 0x0600A6F7 RID: 42743 RVA: 0x004DAD30 File Offset: 0x004D8F30
		protected BuildingRecruitCharacterData GetGeneralData(BuildingRecruitCharacterData data)
		{
			return data;
		}
	}
}

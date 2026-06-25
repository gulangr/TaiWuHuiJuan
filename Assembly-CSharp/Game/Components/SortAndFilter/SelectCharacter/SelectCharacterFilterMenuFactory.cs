using System;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFD RID: 3325
	public static class SelectCharacterFilterMenuFactory<TData> where TData : ISelectCharacterData
	{
		// Token: 0x0600A6C7 RID: 42695 RVA: 0x004D87C4 File Offset: 0x004D69C4
		public static DetailedFilterMenuLogic<TData> CreateBasicMenu(ESelectCharacterFilterMenuId menuId)
		{
			if (!true)
			{
			}
			DetailedFilterMenuLogic<TData> result;
			switch (menuId)
			{
			case ESelectCharacterFilterMenuId.Gender:
				result = new GenderFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.BehaviorType:
				result = new BehaviorTypeFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Relation:
				result = new RelationFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.AdoreRelation:
				result = new AdoreRelationFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.EnemyRelation:
				result = new EnemyRelationFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Organization:
				result = new OrganizationFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Sect:
				result = new SectFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.WorkStatus:
				result = new WorkStatusFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.RoleArrangement:
				result = new RoleArrangementFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.RoleArrangementWork:
				result = new RoleArrangementWorkFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Identity:
				result = new IdentityFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Taiwu:
				result = new TaiwuFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.PunishmentType:
				result = new PunishmentTypeFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.PunishmentSeverity:
				result = new PunishmentSeverityFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.CaseStatus:
				result = new CaseStatusFilterMenu<TData>();
				goto IL_D8;
			case ESelectCharacterFilterMenuId.Location:
				result = new LocationFilterMenu<TData>();
				goto IL_D8;
			}
			result = null;
			IL_D8:
			if (!true)
			{
			}
			return result;
		}
	}
}

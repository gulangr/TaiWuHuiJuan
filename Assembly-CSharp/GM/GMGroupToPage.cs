using System;
using System.Collections.Generic;

namespace GM
{
	// Token: 0x0200060F RID: 1551
	public static class GMGroupToPage
	{
		// Token: 0x060048B9 RID: 18617 RVA: 0x002201E4 File Offset: 0x0021E3E4
		public static EGMPage GetPage(EGMGroup group)
		{
			return GMGroupToPage.GroupToPage[group];
		}

		// Token: 0x0400328B RID: 12939
		public static readonly Dictionary<EGMGroup, EGMPage> GroupToPage = new Dictionary<EGMGroup, EGMPage>
		{
			{
				EGMGroup.CharacterBase,
				EGMPage.Character
			},
			{
				EGMGroup.CharacterResource,
				EGMPage.Character
			},
			{
				EGMGroup.CharacterItem,
				EGMPage.Character
			},
			{
				EGMGroup.CharacterInformation,
				EGMPage.Character
			},
			{
				EGMGroup.CharacterEvent,
				EGMPage.Character
			},
			{
				EGMGroup.CharacterGroup,
				EGMPage.Character
			},
			{
				EGMGroup.MapQuickOperation,
				EGMPage.Map
			},
			{
				EGMGroup.MapAreaOperation,
				EGMPage.Map
			},
			{
				EGMGroup.MapBase,
				EGMPage.Map
			},
			{
				EGMGroup.MapWorldFunction,
				EGMPage.WorldFunction
			},
			{
				EGMGroup.SettlementTreasury,
				EGMPage.Map
			},
			{
				EGMGroup.Caravan,
				EGMPage.Map
			},
			{
				EGMGroup.Merchant,
				EGMPage.Map
			},
			{
				EGMGroup.MapPickup,
				EGMPage.Map
			},
			{
				EGMGroup.CombatBase,
				EGMPage.Combat
			},
			{
				EGMGroup.BuildBase,
				EGMPage.Build
			},
			{
				EGMGroup.VillagerRole,
				EGMPage.Build
			},
			{
				EGMGroup.Chicken,
				EGMPage.Build
			},
			{
				EGMGroup.CombatSkill,
				EGMPage.CombatSkill
			},
			{
				EGMGroup.LifeSkill,
				EGMPage.LifeSkill
			},
			{
				EGMGroup.AdventureRemake,
				EGMPage.AdventureRemake
			},
			{
				EGMGroup.AdventureMajorEvent,
				EGMPage.AdventureMajorEvent
			},
			{
				EGMGroup.SystemGlobal,
				EGMPage.Misc
			}
		};
	}
}

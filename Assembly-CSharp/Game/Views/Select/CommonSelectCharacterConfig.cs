using System;
using System.Collections.Generic;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x02000792 RID: 1938
	public class CommonSelectCharacterConfig
	{
		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06005DF6 RID: 24054 RVA: 0x002B492F File Offset: 0x002B2B2F
		// (set) Token: 0x06005DF7 RID: 24055 RVA: 0x002B4937 File Offset: 0x002B2B37
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x06005DF8 RID: 24056 RVA: 0x002B4940 File Offset: 0x002B2B40
		// (set) Token: 0x06005DF9 RID: 24057 RVA: 0x002B4948 File Offset: 0x002B2B48
		public ESelectCharacterInteractionMode InteractionMode { get; set; } = ESelectCharacterInteractionMode.Slot;

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x06005DFA RID: 24058 RVA: 0x002B4951 File Offset: 0x002B2B51
		// (set) Token: 0x06005DFB RID: 24059 RVA: 0x002B4959 File Offset: 0x002B2B59
		public List<ESelectCharacterSubPage> Subpages { get; set; } = new List<ESelectCharacterSubPage>();

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x06005DFC RID: 24060 RVA: 0x002B4962 File Offset: 0x002B2B62
		// (set) Token: 0x06005DFD RID: 24061 RVA: 0x002B496A File Offset: 0x002B2B6A
		public ESelectCharacterSelectionMode SelectionMode { get; set; } = ESelectCharacterSelectionMode.Single;

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x06005DFE RID: 24062 RVA: 0x002B4973 File Offset: 0x002B2B73
		// (set) Token: 0x06005DFF RID: 24063 RVA: 0x002B497B File Offset: 0x002B2B7B
		public int TargetCount { get; set; } = 1;

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06005E00 RID: 24064 RVA: 0x002B4984 File Offset: 0x002B2B84
		// (set) Token: 0x06005E01 RID: 24065 RVA: 0x002B498C File Offset: 0x002B2B8C
		public List<ESelectCharacterFilterMenuId> FilterMenuIds { get; set; }

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06005E02 RID: 24066 RVA: 0x002B4995 File Offset: 0x002B2B95
		// (set) Token: 0x06005E03 RID: 24067 RVA: 0x002B499D File Offset: 0x002B2B9D
		public string CustomSubPageName { get; set; }

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x06005E04 RID: 24068 RVA: 0x002B49A6 File Offset: 0x002B2BA6
		// (set) Token: 0x06005E05 RID: 24069 RVA: 0x002B49AE File Offset: 0x002B2BAE
		public Dictionary<ESelectCharacterSubPage, Func<IEnumerable<ColumnDefinition>>> CustomColumnGenerator { get; set; }

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x06005E06 RID: 24070 RVA: 0x002B49B7 File Offset: 0x002B2BB7
		// (set) Token: 0x06005E07 RID: 24071 RVA: 0x002B49BF File Offset: 0x002B2BBF
		public ColumnDefinition CustomAvatar { get; set; }

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x06005E08 RID: 24072 RVA: 0x002B49C8 File Offset: 0x002B2BC8
		// (set) Token: 0x06005E09 RID: 24073 RVA: 0x002B49D0 File Offset: 0x002B2BD0
		public string InfoText { get; set; } = string.Empty;

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06005E0A RID: 24074 RVA: 0x002B49D9 File Offset: 0x002B2BD9
		// (set) Token: 0x06005E0B RID: 24075 RVA: 0x002B49E1 File Offset: 0x002B2BE1
		public Func<ISelectCharacterData, string> SearchTextExtractor { get; set; }

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x06005E0C RID: 24076 RVA: 0x002B49EA File Offset: 0x002B2BEA
		// (set) Token: 0x06005E0D RID: 24077 RVA: 0x002B49F2 File Offset: 0x002B2BF2
		public List<int> InitialSelectedCharacterIds { get; set; }

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06005E0E RID: 24078 RVA: 0x002B49FB File Offset: 0x002B2BFB
		// (set) Token: 0x06005E0F RID: 24079 RVA: 0x002B4A03 File Offset: 0x002B2C03
		public HashSet<int> BannedCharacterIds { get; set; }

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06005E10 RID: 24080 RVA: 0x002B4A0C File Offset: 0x002B2C0C
		// (set) Token: 0x06005E11 RID: 24081 RVA: 0x002B4A14 File Offset: 0x002B2C14
		public bool RefreshDeadAsAlive { get; set; } = false;

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06005E12 RID: 24082 RVA: 0x002B4A1D File Offset: 0x002B2C1D
		// (set) Token: 0x06005E13 RID: 24083 RVA: 0x002B4A25 File Offset: 0x002B2C25
		public int MinSelectionCount { get; set; } = 0;

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06005E14 RID: 24084 RVA: 0x002B4A2E File Offset: 0x002B2C2E
		// (set) Token: 0x06005E15 RID: 24085 RVA: 0x002B4A36 File Offset: 0x002B2C36
		public Func<IReadOnlyList<int>, string> CustomTextGenerator { get; set; }

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06005E16 RID: 24086 RVA: 0x002B4A3F File Offset: 0x002B2C3F
		// (set) Token: 0x06005E17 RID: 24087 RVA: 0x002B4A47 File Offset: 0x002B2C47
		public Func<IReadOnlyList<int>, bool> ConfirmInteractableChecker { get; set; }

		// Token: 0x06005E18 RID: 24088 RVA: 0x002B4A50 File Offset: 0x002B2C50
		public static CommonSelectCharacterConfig CreateBasicFilterConfig(ESelectCharacterSubPage additionalSubpage = ESelectCharacterSubPage.None)
		{
			return new CommonSelectCharacterConfig
			{
				Subpages = CommonSelectCharacterConfig.GetAdditionalSubpages(additionalSubpage),
				FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Favorability,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect
				}
			};
		}

		// Token: 0x06005E19 RID: 24089 RVA: 0x002B4AA8 File Offset: 0x002B2CA8
		public static List<ESelectCharacterSubPage> GetBasicSubpages()
		{
			return new List<ESelectCharacterSubPage>
			{
				ESelectCharacterSubPage.State,
				ESelectCharacterSubPage.Property,
				ESelectCharacterSubPage.Property2,
				ESelectCharacterSubPage.LifeSkill,
				ESelectCharacterSubPage.CombatSkill,
				ESelectCharacterSubPage.Personality,
				ESelectCharacterSubPage.Item,
				ESelectCharacterSubPage.Command
			};
		}

		// Token: 0x06005E1A RID: 24090 RVA: 0x002B4B00 File Offset: 0x002B2D00
		public static List<ESelectCharacterSubPage> GetAdditionalSubpages(ESelectCharacterSubPage additionalSubpage)
		{
			bool flag = additionalSubpage == ESelectCharacterSubPage.None;
			List<ESelectCharacterSubPage> result;
			if (flag)
			{
				result = CommonSelectCharacterConfig.GetBasicSubpages();
			}
			else
			{
				result = new List<ESelectCharacterSubPage>
				{
					additionalSubpage,
					ESelectCharacterSubPage.State,
					ESelectCharacterSubPage.Property,
					ESelectCharacterSubPage.Property2,
					ESelectCharacterSubPage.LifeSkill,
					ESelectCharacterSubPage.CombatSkill,
					ESelectCharacterSubPage.Personality,
					ESelectCharacterSubPage.Item,
					ESelectCharacterSubPage.Command
				};
			}
			return result;
		}

		// Token: 0x040040F9 RID: 16633
		public bool CanShowCharacterMenu = true;

		// Token: 0x04004107 RID: 16647
		public Action<TooltipInvoker, int> MouseTipModifier = null;

		// Token: 0x04004108 RID: 16648
		public bool SkipFallbackSort;
	}
}

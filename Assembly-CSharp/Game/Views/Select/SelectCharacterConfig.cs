using System;
using System.Collections.Generic;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Character.Display;

namespace Game.Views.Select
{
	// Token: 0x02000798 RID: 1944
	public class SelectCharacterConfig
	{
		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06005E22 RID: 24098 RVA: 0x002B4F1F File Offset: 0x002B311F
		// (set) Token: 0x06005E23 RID: 24099 RVA: 0x002B4F27 File Offset: 0x002B3127
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06005E24 RID: 24100 RVA: 0x002B4F30 File Offset: 0x002B3130
		// (set) Token: 0x06005E25 RID: 24101 RVA: 0x002B4F38 File Offset: 0x002B3138
		public ESelectCharacterInteractionMode InteractionMode { get; set; } = ESelectCharacterInteractionMode.Slot;

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06005E26 RID: 24102 RVA: 0x002B4F41 File Offset: 0x002B3141
		// (set) Token: 0x06005E27 RID: 24103 RVA: 0x002B4F49 File Offset: 0x002B3149
		public ESelectCharacterSelectionMode SelectionMode { get; set; } = ESelectCharacterSelectionMode.Single;

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06005E28 RID: 24104 RVA: 0x002B4F52 File Offset: 0x002B3152
		// (set) Token: 0x06005E29 RID: 24105 RVA: 0x002B4F5A File Offset: 0x002B315A
		public int TargetCount { get; set; } = 1;

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x06005E2A RID: 24106 RVA: 0x002B4F63 File Offset: 0x002B3163
		// (set) Token: 0x06005E2B RID: 24107 RVA: 0x002B4F6B File Offset: 0x002B316B
		public List<ESelectCharacterFilterMenuId> FilterMenuIds { get; set; }

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x06005E2C RID: 24108 RVA: 0x002B4F74 File Offset: 0x002B3174
		// (set) Token: 0x06005E2D RID: 24109 RVA: 0x002B4F7C File Offset: 0x002B317C
		public string CustomSubPageName { get; set; }

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x06005E2E RID: 24110 RVA: 0x002B4F85 File Offset: 0x002B3185
		// (set) Token: 0x06005E2F RID: 24111 RVA: 0x002B4F8D File Offset: 0x002B318D
		public Func<IEnumerable<ColumnDefinition>> CustomColumnGenerator { get; set; }

		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06005E30 RID: 24112 RVA: 0x002B4F96 File Offset: 0x002B3196
		// (set) Token: 0x06005E31 RID: 24113 RVA: 0x002B4F9E File Offset: 0x002B319E
		public ColumnDefinition CustomAvatar { get; set; }

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06005E32 RID: 24114 RVA: 0x002B4FA7 File Offset: 0x002B31A7
		// (set) Token: 0x06005E33 RID: 24115 RVA: 0x002B4FAF File Offset: 0x002B31AF
		public string InfoText { get; set; } = string.Empty;

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06005E34 RID: 24116 RVA: 0x002B4FB8 File Offset: 0x002B31B8
		// (set) Token: 0x06005E35 RID: 24117 RVA: 0x002B4FC0 File Offset: 0x002B31C0
		public Func<ISelectCharacterData, string> SearchTextExtractor { get; set; }

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06005E36 RID: 24118 RVA: 0x002B4FC9 File Offset: 0x002B31C9
		// (set) Token: 0x06005E37 RID: 24119 RVA: 0x002B4FD1 File Offset: 0x002B31D1
		public List<int> InitialSelectedCharacterIds { get; set; }

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x06005E38 RID: 24120 RVA: 0x002B4FDA File Offset: 0x002B31DA
		// (set) Token: 0x06005E39 RID: 24121 RVA: 0x002B4FE2 File Offset: 0x002B31E2
		public HashSet<int> BannedCharacterIds { get; set; }

		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x06005E3A RID: 24122 RVA: 0x002B4FEB File Offset: 0x002B31EB
		// (set) Token: 0x06005E3B RID: 24123 RVA: 0x002B4FF3 File Offset: 0x002B31F3
		public bool RefreshDeadAsAlive { get; set; } = false;

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x06005E3C RID: 24124 RVA: 0x002B4FFC File Offset: 0x002B31FC
		// (set) Token: 0x06005E3D RID: 24125 RVA: 0x002B5004 File Offset: 0x002B3204
		public int MinSelectionCount { get; set; } = 0;

		// Token: 0x06005E3E RID: 24126 RVA: 0x002B5010 File Offset: 0x002B3210
		public static SelectCharacterConfig CreateBasicFilterConfig()
		{
			return new SelectCharacterConfig
			{
				FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Favorability
				}
			};
		}
	}
}

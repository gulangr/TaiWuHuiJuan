using System;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AE3 RID: 2787
	public enum ECricketCombatGlobalEventType
	{
		// Token: 0x040068E5 RID: 26853
		Initialize,
		// Token: 0x040068E6 RID: 26854
		RequestData,
		// Token: 0x040068E7 RID: 26855
		CharacterDataReady,
		// Token: 0x040068E8 RID: 26856
		CricketDataReady,
		// Token: 0x040068E9 RID: 26857
		AllowSelectCricketChanged,
		// Token: 0x040068EA RID: 26858
		SelfCricketChanged,
		// Token: 0x040068EB RID: 26859
		CricketPolymorphCharacterChanged,
		// Token: 0x040068EC RID: 26860
		CombatStatusChanged,
		// Token: 0x040068ED RID: 26861
		MatchPrepare,
		// Token: 0x040068EE RID: 26862
		PauseStateChanged,
		// Token: 0x040068EF RID: 26863
		StartButtonHoverChanged,
		// Token: 0x040068F0 RID: 26864
		SkillResolved,
		// Token: 0x040068F1 RID: 26865
		ForceGiveUpCheck,
		// Token: 0x040068F2 RID: 26866
		ForceGiveUpRefuse,
		// Token: 0x040068F3 RID: 26867
		CharacterMenuShowed,
		// Token: 0x040068F4 RID: 26868
		CharacterMenuHide
	}
}

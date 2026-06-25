using System;
using DG.Tweening;
using FrameWork;
using GameData.Combat.Cricket;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AE2 RID: 2786
	public interface ICricketCombatComponent
	{
		// Token: 0x17000F20 RID: 3872
		// (set) Token: 0x06008908 RID: 35080
		ICricketCombatHandler Handler { set; }

		// Token: 0x06008909 RID: 35081
		void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox);

		// Token: 0x0600890A RID: 35082 RVA: 0x003F6C42 File Offset: 0x003F4E42
		ECricketCombatSequencePriority GetPriority(CricketCombatLog log)
		{
			return ECricketCombatSequencePriority.Normal;
		}

		// Token: 0x0600890B RID: 35083 RVA: 0x003F6C45 File Offset: 0x003F4E45
		Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			return sequence;
		}
	}
}

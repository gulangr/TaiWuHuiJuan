using System;
using GameData.Domains.Combat;
using GameData.Utilities;

namespace Game.Views.Combat
{
	// Token: 0x02000B28 RID: 2856
	public class CombatDamageValueLayoutData
	{
		// Token: 0x06008C17 RID: 35863 RVA: 0x0040BD1D File Offset: 0x00409F1D
		public CombatDamageValueLayoutData(DefeatMarkKey markKey)
		{
			this.MarkKey = markKey;
		}

		// Token: 0x04006B32 RID: 27442
		public readonly DefeatMarkKey MarkKey;

		// Token: 0x04006B33 RID: 27443
		public int MarkCount;

		// Token: 0x04006B34 RID: 27444
		public IntPair DamageValue;

		// Token: 0x04006B35 RID: 27445
		public bool ReachLimit;

		// Token: 0x04006B36 RID: 27446
		public EHeavyOrBreakType HeavyOrBreak;
	}
}

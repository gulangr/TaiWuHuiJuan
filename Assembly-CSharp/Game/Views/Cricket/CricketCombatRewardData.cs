using System;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB6 RID: 2742
	public class CricketCombatRewardData
	{
		// Token: 0x0400674B RID: 26443
		public CricketCombatRewardKind Kind;

		// Token: 0x0400674C RID: 26444
		public ITradeableContent Content;

		// Token: 0x0400674D RID: 26445
		public CharacterDisplayData CharacterDisplayData;

		// Token: 0x0400674E RID: 26446
		public string DisplayName;

		// Token: 0x0400674F RID: 26447
		public int Delta;

		// Token: 0x04006750 RID: 26448
		public bool IsGain;

		// Token: 0x04006751 RID: 26449
		public bool ShowDelta;

		// Token: 0x04006752 RID: 26450
		public bool ShowCount;
	}
}

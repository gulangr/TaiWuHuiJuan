using System;
using FrameWork;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AE1 RID: 2785
	public interface ICricketCombatHandler
	{
		// Token: 0x17000F1F RID: 3871
		// (get) Token: 0x06008903 RID: 35075
		IAsyncMethodRequestHandler Async { get; }

		// Token: 0x06008904 RID: 35076
		void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox = null);

		// Token: 0x06008905 RID: 35077
		void DoSettlement(bool win, bool giveUp = false);

		// Token: 0x06008906 RID: 35078
		bool CanReorderSelfCricket(int fromJarIndex, int toJarIndex);

		// Token: 0x06008907 RID: 35079
		void ReorderSelfCricket(int fromJarIndex, int toJarIndex);
	}
}

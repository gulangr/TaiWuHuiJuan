using System;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F58 RID: 3928
	public interface ILifeRecord
	{
		// Token: 0x1700146D RID: 5229
		// (get) Token: 0x0600B3FC RID: 46076
		int CurrDate { get; }

		// Token: 0x0600B3FD RID: 46077
		void ScrollToMonth(int month, bool smooth = true, bool requestFill = false, ESelectDateDirection direction = ESelectDateDirection.SelectDefault);
	}
}

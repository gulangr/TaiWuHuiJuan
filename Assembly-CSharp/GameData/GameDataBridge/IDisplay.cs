using System;
using System.Collections.Generic;

namespace GameData.GameDataBridge
{
	// Token: 0x02000FB7 RID: 4023
	public interface IDisplay
	{
		// Token: 0x0600B92A RID: 47402
		void OnNotifyGameData(List<NotificationWrapper> notifications);
	}
}

using System;
using GameData.Utilities;

namespace GameData.GameDataBridge
{
	// Token: 0x02000FB8 RID: 4024
	public readonly struct NotificationWrapper
	{
		// Token: 0x0600B92B RID: 47403 RVA: 0x00546497 File Offset: 0x00544697
		public NotificationWrapper(Notification notification, RawDataPool dataPool)
		{
			this.Notification = notification;
			this.DataPool = dataPool;
		}

		// Token: 0x04008F78 RID: 36728
		public readonly Notification Notification;

		// Token: 0x04008F79 RID: 36729
		public readonly RawDataPool DataPool;
	}
}

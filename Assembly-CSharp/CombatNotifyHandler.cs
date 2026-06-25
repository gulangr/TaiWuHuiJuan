using System;
using GameData.GameDataBridge;

// Token: 0x02000112 RID: 274
public struct CombatNotifyHandler
{
	// Token: 0x060009DF RID: 2527 RVA: 0x00041354 File Offset: 0x0003F554
	public bool IsMatch(Notification notification)
	{
		byte type = notification.Type;
		if (!true)
		{
		}
		bool result;
		if (type != 0)
		{
			result = (type == 1 && (this.MethodAttribute != null && this.MethodAttribute.DomainId == notification.DomainId) && this.MethodAttribute.MethodId == notification.MethodId);
		}
		else
		{
			result = (this.DataAttribute != null && this.DataAttribute.Uid.DomainId == notification.Uid.DomainId && this.DataAttribute.Uid.DataId == notification.Uid.DataId && (this.SubProcessor == null || (this.SubProcessor.SubId0 == notification.Uid.SubId0 && this.DataAttribute.Uid.SubId1 == notification.Uid.SubId1)));
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x0004144A File Offset: 0x0003F64A
	public void Handle(NotificationWrapper wrapper)
	{
		this.HandlerDelegate(wrapper.DataPool, wrapper.Notification.ValueOffset);
	}

	// Token: 0x04000CE8 RID: 3304
	public ICombatNotifySubProcessor SubProcessor;

	// Token: 0x04000CE9 RID: 3305
	public CombatNotifyHandlerDelegate HandlerDelegate;

	// Token: 0x04000CEA RID: 3306
	public CombatNotifyMethodAttribute MethodAttribute;

	// Token: 0x04000CEB RID: 3307
	public CombatNotifyDataAttribute DataAttribute;
}

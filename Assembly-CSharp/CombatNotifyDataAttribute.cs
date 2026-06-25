using System;
using GameData.Common;

// Token: 0x02000115 RID: 277
[AttributeUsage(AttributeTargets.Method)]
public class CombatNotifyDataAttribute : Attribute
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x00041482 File Offset: 0x0003F682
	public CombatNotifyDataAttribute(DataUid uid)
	{
		this.Uid = uid;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00041492 File Offset: 0x0003F692
	public CombatNotifyDataAttribute(ushort domainId, ushort dataId)
	{
		this.Uid = new DataUid(domainId, dataId, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x000414AB File Offset: 0x0003F6AB
	public CombatNotifyDataAttribute(ushort domainId, ushort dataId, uint subId1)
	{
		this.Uid = new DataUid(domainId, dataId, ulong.MaxValue, subId1);
	}

	// Token: 0x04000CEE RID: 3310
	public DataUid Uid;
}

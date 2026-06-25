using System;

// Token: 0x02000114 RID: 276
[AttributeUsage(AttributeTargets.Method)]
public class CombatNotifyMethodAttribute : Attribute
{
	// Token: 0x060009E5 RID: 2533 RVA: 0x0004146A File Offset: 0x0003F66A
	public CombatNotifyMethodAttribute(ushort domainId, ushort methodId)
	{
		this.DomainId = domainId;
		this.MethodId = methodId;
	}

	// Token: 0x04000CEC RID: 3308
	public ushort DomainId;

	// Token: 0x04000CED RID: 3309
	public ushort MethodId;
}

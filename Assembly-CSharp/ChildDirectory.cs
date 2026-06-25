using System;

// Token: 0x020000DE RID: 222
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class ChildDirectory : Attribute
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060007E1 RID: 2017 RVA: 0x00036D0F File Offset: 0x00034F0F
	public Type ChildPathField { get; }

	// Token: 0x060007E2 RID: 2018 RVA: 0x00036D17 File Offset: 0x00034F17
	public ChildDirectory(Type childPathField)
	{
		this.ChildPathField = childPathField;
	}
}

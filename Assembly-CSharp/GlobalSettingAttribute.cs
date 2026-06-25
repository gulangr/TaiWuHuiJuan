using System;
using Game.Views.SystemSetting;

// Token: 0x02000131 RID: 305
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class GlobalSettingAttribute : Attribute
{
	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000E2C RID: 3628 RVA: 0x0005982F File Offset: 0x00057A2F
	public ESettingSubCategory GlobalSettingType { get; }

	// Token: 0x06000E2D RID: 3629 RVA: 0x00059837 File Offset: 0x00057A37
	public GlobalSettingAttribute(ESettingSubCategory globalSettingType)
	{
		this.GlobalSettingType = globalSettingType;
	}
}

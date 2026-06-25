using System;
using Game.Views.SystemSetting;

// Token: 0x02000135 RID: 309
public class GlobalSettingFloat : GlobalSettingAttribute
{
	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00059896 File Offset: 0x00057A96
	public float ResetValue { get; }

	// Token: 0x06000E35 RID: 3637 RVA: 0x0005989E File Offset: 0x00057A9E
	public GlobalSettingFloat(ESettingSubCategory globalSettingType, float resetValue) : base(globalSettingType)
	{
		this.ResetValue = resetValue;
	}
}

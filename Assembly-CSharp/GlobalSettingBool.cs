using System;
using Game.Views.SystemSetting;

// Token: 0x02000132 RID: 306
public class GlobalSettingBool : GlobalSettingAttribute
{
	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000E2E RID: 3630 RVA: 0x00059848 File Offset: 0x00057A48
	public bool ResetValue { get; }

	// Token: 0x06000E2F RID: 3631 RVA: 0x00059850 File Offset: 0x00057A50
	public GlobalSettingBool(ESettingSubCategory globalSettingType, bool resetValue) : base(globalSettingType)
	{
		this.ResetValue = resetValue;
	}
}

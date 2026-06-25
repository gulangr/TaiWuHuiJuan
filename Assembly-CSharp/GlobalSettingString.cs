using System;
using Game.Views.SystemSetting;

// Token: 0x02000136 RID: 310
public class GlobalSettingString : GlobalSettingAttribute
{
	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000E36 RID: 3638 RVA: 0x000598B0 File Offset: 0x00057AB0
	public string ResetValue { get; }

	// Token: 0x06000E37 RID: 3639 RVA: 0x000598B8 File Offset: 0x00057AB8
	public GlobalSettingString(ESettingSubCategory globalSettingType, string resetValue) : base(globalSettingType)
	{
		this.ResetValue = resetValue;
	}
}

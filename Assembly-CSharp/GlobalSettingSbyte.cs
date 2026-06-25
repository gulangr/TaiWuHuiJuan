using System;
using Game.Views.SystemSetting;

// Token: 0x02000133 RID: 307
public class GlobalSettingSbyte : GlobalSettingAttribute
{
	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000E30 RID: 3632 RVA: 0x00059862 File Offset: 0x00057A62
	public sbyte ResetValue { get; }

	// Token: 0x06000E31 RID: 3633 RVA: 0x0005986A File Offset: 0x00057A6A
	public GlobalSettingSbyte(ESettingSubCategory globalSettingType, sbyte resetValue) : base(globalSettingType)
	{
		this.ResetValue = resetValue;
	}
}

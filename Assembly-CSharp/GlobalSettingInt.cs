using System;
using Game.Views.SystemSetting;

// Token: 0x02000134 RID: 308
public class GlobalSettingInt : GlobalSettingAttribute
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000E32 RID: 3634 RVA: 0x0005987C File Offset: 0x00057A7C
	public int ResetValue { get; }

	// Token: 0x06000E33 RID: 3635 RVA: 0x00059884 File Offset: 0x00057A84
	public GlobalSettingInt(ESettingSubCategory globalSettingType, int resetValue) : base(globalSettingType)
	{
		this.ResetValue = resetValue;
	}
}

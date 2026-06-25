using System;

// Token: 0x0200026A RID: 618
public class MoreInfo2 : Refers
{
	// Token: 0x060028DB RID: 10459 RVA: 0x0012E9CC File Offset: 0x0012CBCC
	private void Awake()
	{
		this.InitRefers();
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x0012E9D6 File Offset: 0x0012CBD6
	public void Init()
	{
		this.InitRefers();
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x0012E9E0 File Offset: 0x0012CBE0
	public void RefreshPressToDetail()
	{
		this.InitRefers();
		this._hotkeyDisplay.Refresh(EHotKeyDisplayType.Detail);
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x0012E9F8 File Offset: 0x0012CBF8
	public void RefreshPressToDetailAndCompare()
	{
		this.InitRefers();
		this._hotkeyDisplay.Refresh(EHotKeyDisplayType.DetailAndCompareEquipment);
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x0012EA10 File Offset: 0x0012CC10
	public void RefreshPressToCompare()
	{
		this.InitRefers();
		this._hotkeyDisplay.Refresh(EHotKeyDisplayType.CompareEquipment);
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x0012EA28 File Offset: 0x0012CC28
	public void RefreshCancelDetail()
	{
		this.InitRefers();
		this._hotkeyDisplay.Refresh(EHotKeyDisplayType.CancelDetail);
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x0012EA40 File Offset: 0x0012CC40
	public void RefreshCancelCompare()
	{
		this.InitRefers();
		this._hotkeyDisplay.Refresh(EHotKeyDisplayType.CancelCompareEquipment);
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x0012EA58 File Offset: 0x0012CC58
	private void InitRefers()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._hotkeyDisplay = base.CGet<HotkeyDisplay>("HotkeyDisplay");
			this._inited = true;
		}
	}

	// Token: 0x04001DC7 RID: 7623
	private bool _inited = false;

	// Token: 0x04001DC8 RID: 7624
	private HotkeyDisplay _hotkeyDisplay;
}

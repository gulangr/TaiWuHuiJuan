using System;
using TMPro;
using UnityEngine.EventSystems;

// Token: 0x0200005A RID: 90
public class DisableHotkeyInputField : TMP_InputField
{
	// Token: 0x060002F3 RID: 755 RVA: 0x00011BC0 File Offset: 0x0000FDC0
	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		bool flag = !this._hotkeysDisabledByFocus;
		if (flag)
		{
			this._hotkeysDisabledByFocus = true;
			CommandKitBase.SetDisable(true);
		}
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00011BF3 File Offset: 0x0000FDF3
	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		this.RestoreHotkeys();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00011C05 File Offset: 0x0000FE05
	protected override void OnDisable()
	{
		this.RestoreHotkeys();
		base.OnDisable();
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00011C16 File Offset: 0x0000FE16
	protected override void OnDestroy()
	{
		this.RestoreHotkeys();
		base.OnDestroy();
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00011C28 File Offset: 0x0000FE28
	private void RestoreHotkeys()
	{
		bool hotkeysDisabledByFocus = this._hotkeysDisabledByFocus;
		if (hotkeysDisabledByFocus)
		{
			this._hotkeysDisabledByFocus = false;
			CommandKitBase.SetDisable(false);
		}
	}

	// Token: 0x0400019C RID: 412
	private bool _hotkeysDisabledByFocus;
}

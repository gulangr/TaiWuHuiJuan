using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class CommonCustomizeTableRow : Refers
{
	// Token: 0x1700034F RID: 847
	// (get) Token: 0x060020DF RID: 8415 RVA: 0x000EFE3E File Offset: 0x000EE03E
	public int ChildCount
	{
		get
		{
			return this.content.childCount;
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000EFE4C File Offset: 0x000EE04C
	public Refers GetChildRefers(int index)
	{
		bool flag = index >= this.content.childCount;
		Refers result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = this.content.GetChild(index).GetComponent<Refers>();
		}
		return result;
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000EFE88 File Offset: 0x000EE088
	public void InitClickEvent(bool isOn, int charId, Action<int> onClicked)
	{
		this._onClicked = onClicked;
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000EFE92 File Offset: 0x000EE092
	public void Refresh(bool isOn)
	{
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000EFE95 File Offset: 0x000EE095
	public void OnRowClicked(bool _)
	{
		Action<int> onClicked = this._onClicked;
		if (onClicked != null)
		{
			onClicked(this.DataId);
		}
	}

	// Token: 0x0400193D RID: 6461
	public RectTransform content;

	// Token: 0x0400193E RID: 6462
	public int DataId;

	// Token: 0x0400193F RID: 6463
	private Action<int> _onClicked;
}

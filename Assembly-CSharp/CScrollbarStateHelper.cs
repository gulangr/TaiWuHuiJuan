using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
[Obsolete]
[RequireComponent(typeof(CScrollbarLegacy))]
public class CScrollbarStateHelper : MonoBehaviour
{
	// Token: 0x0600072F RID: 1839 RVA: 0x00031F32 File Offset: 0x00030132
	public void SetStatus(CScrollbarStateHelper.ShowStatus newStatus)
	{
		this.Status = (sbyte)newStatus;
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000730 RID: 1840 RVA: 0x00031F3C File Offset: 0x0003013C
	// (set) Token: 0x06000731 RID: 1841 RVA: 0x00031F44 File Offset: 0x00030144
	public sbyte Status
	{
		get
		{
			return (sbyte)this.status;
		}
		set
		{
			switch (value)
			{
			case 0:
				this.Hide();
				break;
			case 1:
				this.ShowBase();
				break;
			case 2:
				this.ShowBar();
				break;
			default:
				Debug.LogError(string.Format("Unknown ShowStatus {0}", value));
				return;
			}
			this.status = (CScrollbarStateHelper.ShowStatus)value;
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00031FA4 File Offset: 0x000301A4
	private void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x00031FB4 File Offset: 0x000301B4
	private void ShowBase()
	{
		base.gameObject.SetActive(true);
		this.scrollBase.SetActive(true);
		this.scrollContainer.SetActive(false);
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00031FDE File Offset: 0x000301DE
	private void ShowBar()
	{
		base.gameObject.SetActive(true);
		this.scrollBase.SetActive(false);
		this.scrollContainer.SetActive(true);
	}

	// Token: 0x04000786 RID: 1926
	[SerializeField]
	private CScrollbarStateHelper.ShowStatus status;

	// Token: 0x04000787 RID: 1927
	[SerializeField]
	private CScrollbarLegacy bar;

	// Token: 0x04000788 RID: 1928
	[SerializeField]
	private GameObject scrollBase;

	// Token: 0x04000789 RID: 1929
	[SerializeField]
	private GameObject scrollContainer;

	// Token: 0x0200112F RID: 4399
	public enum ShowStatus : sbyte
	{
		// Token: 0x040095F5 RID: 38389
		Hide,
		// Token: 0x040095F6 RID: 38390
		ShowBase,
		// Token: 0x040095F7 RID: 38391
		ShowBar
	}
}

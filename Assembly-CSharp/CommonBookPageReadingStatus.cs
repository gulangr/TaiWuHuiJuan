using System;
using UnityEngine;

// Token: 0x02000320 RID: 800
public class CommonBookPageReadingStatus : MonoBehaviour
{
	// Token: 0x1700051F RID: 1311
	// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x0017105A File Offset: 0x0016F25A
	// (set) Token: 0x06002EC8 RID: 11976 RVA: 0x00171062 File Offset: 0x0016F262
	public int Progress
	{
		get
		{
			return this.progress;
		}
		set
		{
			this.SetProgress(value);
		}
	}

	// Token: 0x17000520 RID: 1312
	// (get) Token: 0x06002EC9 RID: 11977 RVA: 0x0017106C File Offset: 0x0016F26C
	// (set) Token: 0x06002ECA RID: 11978 RVA: 0x00171074 File Offset: 0x0016F274
	public CommonBookPageReadingStatus.Status PageStatus
	{
		get
		{
			return this.pageStatus;
		}
		set
		{
			this.SetPageStatus((int)value, this.disable);
		}
	}

	// Token: 0x17000521 RID: 1313
	// (get) Token: 0x06002ECB RID: 11979 RVA: 0x00171084 File Offset: 0x0016F284
	// (set) Token: 0x06002ECC RID: 11980 RVA: 0x0017108C File Offset: 0x0016F28C
	public bool Disable
	{
		get
		{
			return this.disable;
		}
		set
		{
			this.SetPageStatus((int)this.pageStatus, value);
		}
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x0017109C File Offset: 0x0016F29C
	public void SetBookPageReadingStatus(CommonBookPageReadingStatus.Status newPageStatus, bool isDisable, int currProgress)
	{
		this.SetPageStatus((int)newPageStatus, isDisable);
		this.SetProgress(currProgress);
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x001710B0 File Offset: 0x0016F2B0
	public void SetPageStatus(int currStatus, bool isDisable)
	{
		this.pageStatus = (CommonBookPageReadingStatus.Status)currStatus;
		this.disable = isDisable;
		bool active = this.spritesActive.CheckIndex(currStatus);
		bool flag = active;
		if (flag)
		{
			this.image.sprite = this.spritesActive[currStatus];
		}
		base.gameObject.SetActive(active);
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x00171100 File Offset: 0x0016F300
	public void SetProgress(int currProgress)
	{
		bool flag = currProgress < 0 || currProgress > 100;
		if (!flag)
		{
			bool full = currProgress == 100;
			this.progress = currProgress;
			this.progressBar.gameObject.SetActive(!full);
			this.progressBarFull.gameObject.SetActive(full);
			this.progressBar.fillAmount = (float)currProgress / 100f;
		}
	}

	// Token: 0x040021F6 RID: 8694
	[Header("缺/残/全 三个Sprite")]
	[SerializeField]
	private Sprite[] spritesActive;

	// Token: 0x040021F7 RID: 8695
	[Header("缺/残/全 三个Sprite")]
	[SerializeField]
	private Sprite[] spritesInactive;

	// Token: 0x040021F8 RID: 8696
	[SerializeField]
	private CImage image;

	// Token: 0x040021F9 RID: 8697
	[SerializeField]
	private CImage progressBar;

	// Token: 0x040021FA RID: 8698
	[SerializeField]
	private CImage progressBarFull;

	// Token: 0x040021FB RID: 8699
	[SerializeField]
	private int progress;

	// Token: 0x040021FC RID: 8700
	[SerializeField]
	private CommonBookPageReadingStatus.Status pageStatus = CommonBookPageReadingStatus.Status.Missing;

	// Token: 0x040021FD RID: 8701
	[SerializeField]
	private bool disable;

	// Token: 0x020016A5 RID: 5797
	public enum Status : sbyte
	{
		// Token: 0x0400A893 RID: 43155
		Hide = -1,
		// Token: 0x0400A894 RID: 43156
		Missing,
		// Token: 0x0400A895 RID: 43157
		Partial,
		// Token: 0x0400A896 RID: 43158
		Full
	}
}

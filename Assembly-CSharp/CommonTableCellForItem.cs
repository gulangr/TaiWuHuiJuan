using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000333 RID: 819
[RequireComponent(typeof(CImage))]
public class CommonTableCellForItem : MonoBehaviour
{
	// Token: 0x17000536 RID: 1334
	// (get) Token: 0x06002F67 RID: 12135 RVA: 0x00173B1F File Offset: 0x00171D1F
	public Sprite[] Sprites
	{
		get
		{
			return this.sprites;
		}
	}

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x06002F68 RID: 12136 RVA: 0x00173B27 File Offset: 0x00171D27
	public int Status
	{
		get
		{
			return this.status;
		}
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x00173B30 File Offset: 0x00171D30
	public void SetCurrentStatus(int currStatus)
	{
		Behaviour behaviour = this.image;
		ICollection collection = this.sprites;
		this.status = currStatus;
		behaviour.enabled = (collection.CheckIndex(currStatus) && (this.image.sprite = this.sprites[currStatus]) != null);
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x00173B84 File Offset: 0x00171D84
	public void Refresh()
	{
		this.image.enabled = (this.sprites.CheckIndex(this.status) && (this.image.sprite = this.sprites[this.status]) != null);
	}

	// Token: 0x0400227C RID: 8828
	[Header("正常/悬停/正常锁定/悬停锁定 四个Sprite")]
	[SerializeField]
	private Sprite[] sprites;

	// Token: 0x0400227D RID: 8829
	[SerializeField]
	[ReadOnly]
	private CImage image;

	// Token: 0x0400227E RID: 8830
	[SerializeField]
	private int status;
}

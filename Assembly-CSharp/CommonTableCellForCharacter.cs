using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
[RequireComponent(typeof(CImage))]
public class CommonTableCellForCharacter : Refers
{
	// Token: 0x06002F63 RID: 12131 RVA: 0x00173A8B File Offset: 0x00171C8B
	public void SetCurrentStatus(int currStatus)
	{
		this._status = currStatus;
		this.RefreshImage();
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x00173A9C File Offset: 0x00171C9C
	public void SetSpecial(bool isSecial)
	{
		this._isSpecial = isSecial;
		this.RefreshImage();
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x00173AB0 File Offset: 0x00171CB0
	private void RefreshImage()
	{
		Sprite[] targetSprites = this._isSpecial ? this._spSsprites : this._sprites;
		this._image.enabled = (targetSprites.CheckIndex(this._status) && (this._image.sprite = targetSprites[this._status]) != null);
	}

	// Token: 0x04002277 RID: 8823
	[Header("正常/悬停/正常锁定/悬停锁定 四个Sprite")]
	public Sprite[] _sprites;

	// Token: 0x04002278 RID: 8824
	[Header("正常/悬停/正常锁定/悬停锁定 四个特殊Sprite")]
	public Sprite[] _spSsprites;

	// Token: 0x04002279 RID: 8825
	[SerializeField]
	[ReadOnly]
	private CImage _image;

	// Token: 0x0400227A RID: 8826
	[SerializeField]
	private int _status;

	// Token: 0x0400227B RID: 8827
	[SerializeField]
	private bool _isSpecial = false;
}

using System;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class UVAnimationSpeed : MonoBehaviour
{
	// Token: 0x0600061F RID: 1567 RVA: 0x0002900C File Offset: 0x0002720C
	private void Start()
	{
		this._cRawImage = base.GetComponent<CRawImage>();
		this._uvX = this._cRawImage.uvRect.x;
		this._uvY = this._cRawImage.uvRect.y;
		this._animRect = new Rect(this._cRawImage.uvRect);
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00029070 File Offset: 0x00027270
	private void Update()
	{
		this._animRect.x = this._animRect.x + this.SpeedScale * this.XSpeed * Time.deltaTime;
		this._animRect.y = this._animRect.y + this.SpeedScale * this.YSpeed * Time.deltaTime;
		this._animRect.x = this._animRect.x % 1f;
		this._animRect.y = this._animRect.y % 1f;
		this._cRawImage.uvRect = this._animRect;
	}

	// Token: 0x04000505 RID: 1285
	[Range(-1f, 1f)]
	public float XSpeed;

	// Token: 0x04000506 RID: 1286
	[Range(-1f, 1f)]
	public float YSpeed;

	// Token: 0x04000507 RID: 1287
	public float SpeedScale = 1f;

	// Token: 0x04000508 RID: 1288
	private CRawImage _cRawImage;

	// Token: 0x04000509 RID: 1289
	private float _uvX;

	// Token: 0x0400050A RID: 1290
	private float _uvY;

	// Token: 0x0400050B RID: 1291
	private Rect _animRect;
}

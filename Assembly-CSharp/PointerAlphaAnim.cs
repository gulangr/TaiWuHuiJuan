using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class PointerAlphaAnim : MonoBehaviour
{
	// Token: 0x060004E6 RID: 1254 RVA: 0x000220E4 File Offset: 0x000202E4
	private void Awake()
	{
		bool flag = null == this.Target;
		if (flag)
		{
			this.Target = base.transform;
		}
		this._cGroup = this.Target.GetComponent<CanvasGroup>();
		bool flag2 = null == this._cGroup;
		if (flag2)
		{
			this._cGroup = this.Target.gameObject.AddComponent<CanvasGroup>();
		}
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00022147 File Offset: 0x00020347
	private void Start()
	{
		this.Show(false);
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00022154 File Offset: 0x00020354
	public void Show(bool isShow)
	{
		bool flag = null != this._cGroup;
		if (flag)
		{
			this._cGroup.DOPause();
			this._cGroup.DOFade(isShow ? this.AlphaIn : this.AlphaOut, this.Duration);
			if (isShow)
			{
				Action onShow = this.OnShow;
				if (onShow != null)
				{
					onShow();
				}
			}
			else
			{
				Action onHide = this.OnHide;
				if (onHide != null)
				{
					onHide();
				}
			}
		}
	}

	// Token: 0x040003EE RID: 1006
	[Tooltip("发生透明度变化的目标")]
	public Transform Target;

	// Token: 0x040003EF RID: 1007
	private CanvasGroup _cGroup;

	// Token: 0x040003F0 RID: 1008
	[Tooltip("鼠标退出时变化到的透明度")]
	public float AlphaOut = 0f;

	// Token: 0x040003F1 RID: 1009
	[Tooltip("鼠标进入时变化到的透明度")]
	public float AlphaIn = 1f;

	// Token: 0x040003F2 RID: 1010
	[Tooltip("透明度渐变时间")]
	public float Duration = 0.3f;

	// Token: 0x040003F3 RID: 1011
	public Action OnShow;

	// Token: 0x040003F4 RID: 1012
	public Action OnHide;
}

using System;
using FrameWork;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class UI_FullScreenCover : UIBase
{
	// Token: 0x060034D4 RID: 13524 RVA: 0x001A61EE File Offset: 0x001A43EE
	private void Awake()
	{
		this._objHolder = base.CGet<GameObject>("ObjectHolder").transform;
		this._coverImage = base.CGet<CImage>("Cover");
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x001A6218 File Offset: 0x001A4418
	public override void OnInit(ArgumentBox argsBox)
	{
		RectTransform objectRect;
		argsBox.Get<RectTransform>("HighlightObj", out objectRect);
		this._objectRect = objectRect;
		this._originalParent = this._objectRect.parent;
		bool activeSelf = base.gameObject.activeSelf;
		if (activeSelf)
		{
			this.ChangeParent();
		}
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x001A6264 File Offset: 0x001A4464
	private void ChangeParent()
	{
		this._objectRect.parent = this._objHolder;
	}

	// Token: 0x060034D7 RID: 13527 RVA: 0x001A6279 File Offset: 0x001A4479
	private void OnEnable()
	{
		this.ChangeParent();
		GEvent.Add(UiEvents.ReleaseCoverObject, new GEvent.Callback(this.OnReleaseCoverObject));
	}

	// Token: 0x060034D8 RID: 13528 RVA: 0x001A629F File Offset: 0x001A449F
	private void OnReleaseCoverObject(ArgumentBox argBox)
	{
		this.QuickHide();
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x001A62A9 File Offset: 0x001A44A9
	private void OnDisable()
	{
		this._objectRect.parent = this._originalParent;
		this._objectRect = null;
	}

	// Token: 0x0400265F RID: 9823
	private Transform _objHolder;

	// Token: 0x04002660 RID: 9824
	private CImage _coverImage;

	// Token: 0x04002661 RID: 9825
	private RectTransform _objectRect;

	// Token: 0x04002662 RID: 9826
	private Transform _originalParent;
}

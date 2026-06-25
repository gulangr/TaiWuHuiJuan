using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

// Token: 0x0200036E RID: 878
public class UI_BlackMask : UIBase
{
	// Token: 0x060032F1 RID: 13041 RVA: 0x0019198D File Offset: 0x0018FB8D
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Refresh(argsBox, false);
		this._maskTweener = null;
	}

	// Token: 0x060032F2 RID: 13042 RVA: 0x001919A0 File Offset: 0x0018FBA0
	private void Awake()
	{
		this._maskImage = base.CGet<CImage>("BlackMask");
	}

	// Token: 0x060032F3 RID: 13043 RVA: 0x001919B4 File Offset: 0x0018FBB4
	private void OnEnable()
	{
		this.StartAnimation();
		CommandKitBase.SetDisable(true);
	}

	// Token: 0x060032F4 RID: 13044 RVA: 0x001919C5 File Offset: 0x0018FBC5
	private void OnDisable()
	{
		CommandKitBase.SetDisable(false);
	}

	// Token: 0x060032F5 RID: 13045 RVA: 0x001919D0 File Offset: 0x0018FBD0
	public void Refresh(ArgumentBox argBox, bool executeImmediate = true)
	{
		argBox.Get("AnimToShowMask", out this._animToShowMask);
		argBox.Get("AnimTime", out this._animTime);
		argBox.Get("HideAfterShow", out this._hideAfterShow);
		if (executeImmediate)
		{
			this.StartAnimation();
		}
	}

	// Token: 0x060032F6 RID: 13046 RVA: 0x00191A20 File Offset: 0x0018FC20
	private void StartAnimation()
	{
		bool animToShowMask = this._animToShowMask;
		if (animToShowMask)
		{
			this.AnimShowMask();
		}
		else
		{
			this.AnimHideMask();
		}
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x00191A48 File Offset: 0x0018FC48
	private void AnimShowMask()
	{
		CommonUtils.TryKillTween(this._maskTweener, false);
		this._maskImage.SetAlpha(0f);
		this._maskTweener = this._maskImage.DOFade(1f, this._animTime).SetUpdate(true).OnComplete(new TweenCallback(this.OnAnimComplete)).SetAutoKill(true);
	}

	// Token: 0x060032F8 RID: 13048 RVA: 0x00191AB0 File Offset: 0x0018FCB0
	private void AnimHideMask()
	{
		CommonUtils.TryKillTween(this._maskTweener, false);
		this._maskImage.SetAlpha(1f);
		this._maskTweener = this._maskImage.DOFade(0f, this._animTime).SetUpdate(true).OnComplete(new TweenCallback(this.OnAnimComplete)).SetAutoKill(true);
	}

	// Token: 0x060032F9 RID: 13049 RVA: 0x00191B18 File Offset: 0x0018FD18
	private void OnAnimComplete()
	{
		bool hideAfterShow = this._hideAfterShow;
		if (hideAfterShow)
		{
			this.Element.Hide(false);
		}
		TaiwuEventDomainMethod.Call.OnBlackMaskAnimationComplete(this._animToShowMask);
		TaiwuEventDomainMethod.Call.TriggerListener(this._animToShowMask ? "OnBlackMaskShowComplete" : "OnBlackMaskHideComplete", true);
	}

	// Token: 0x04002532 RID: 9522
	public const string AnimToShowMask = "AnimToShowMask";

	// Token: 0x04002533 RID: 9523
	public const string AnimTime = "AnimTime";

	// Token: 0x04002534 RID: 9524
	public const string HideAfterShow = "HideAfterShow";

	// Token: 0x04002535 RID: 9525
	private bool _hideAfterShow;

	// Token: 0x04002536 RID: 9526
	private float _animTime;

	// Token: 0x04002537 RID: 9527
	private bool _animToShowMask;

	// Token: 0x04002538 RID: 9528
	private CImage _maskImage;

	// Token: 0x04002539 RID: 9529
	private Tweener _maskTweener;
}

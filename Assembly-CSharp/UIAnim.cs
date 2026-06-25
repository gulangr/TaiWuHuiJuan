using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class UIAnim : MonoBehaviour
{
	// Token: 0x1700008C RID: 140
	// (get) Token: 0x0600059E RID: 1438 RVA: 0x00025818 File Offset: 0x00023A18
	// (set) Token: 0x0600059F RID: 1439 RVA: 0x00025820 File Offset: 0x00023A20
	public Sequence AnimSequenceIn { get; private set; }

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060005A0 RID: 1440 RVA: 0x00025829 File Offset: 0x00023A29
	// (set) Token: 0x060005A1 RID: 1441 RVA: 0x00025831 File Offset: 0x00023A31
	public Sequence AnimSequenceOut { get; private set; }

	// Token: 0x060005A2 RID: 1442 RVA: 0x0002583C File Offset: 0x00023A3C
	public void Init(Vector3 inAnchorPos, Vector3 outAnchorPos)
	{
		float duration = 0.3f;
		RectTransform rectTrans = base.gameObject.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		Sequence animSequenceIn = this.AnimSequenceIn;
		if (animSequenceIn != null)
		{
			animSequenceIn.Kill(false);
		}
		this.AnimSequenceIn = DOTween.Sequence();
		this.AnimSequenceIn.AppendInterval(0.01f);
		this.AnimSequenceIn.Append(rectTrans.DOAnchorPos(inAnchorPos, duration, false));
		this.AnimSequenceIn.Join(canvasGroup.DOFade(1f, duration));
		this.AnimSequenceIn.Pause<Sequence>();
		Sequence animSequenceOut = this.AnimSequenceOut;
		if (animSequenceOut != null)
		{
			animSequenceOut.Kill(false);
		}
		this.AnimSequenceOut = DOTween.Sequence();
		this.AnimSequenceOut.AppendInterval(0.01f);
		this.AnimSequenceOut.Append(rectTrans.DOAnchorPos(outAnchorPos, duration, false));
		this.AnimSequenceOut.Join(canvasGroup.DOFade(0f, duration));
		this.AnimSequenceOut.Pause<Sequence>();
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00025944 File Offset: 0x00023B44
	public void PlayShowAnimation(Action onAnimationComplete, bool skipLastEvent = true)
	{
		bool flag = !skipLastEvent;
		if (flag)
		{
			Action onAnimationInComplete = this._onAnimationInComplete;
			if (onAnimationInComplete != null)
			{
				onAnimationInComplete();
			}
		}
		this._onAnimationInComplete = onAnimationComplete;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			bool flag3 = this.AnimSequenceIn == null;
			if (flag3)
			{
				this.<PlayShowAnimation>g__OnComplete|11_0();
			}
		});
		bool flag2 = this.AnimSequenceIn != null;
		if (flag2)
		{
			this.AnimSequenceIn.SetAutoKill(false);
			this.AnimSequenceIn.OnComplete(new TweenCallback(this.<PlayShowAnimation>g__OnComplete|11_0));
			this.AnimSequenceIn.timeScale = 1f;
			this.AnimSequenceIn.Restart(true, -1f);
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x000259E4 File Offset: 0x00023BE4
	public void PlayHideAnimation(Action onAnimationComplete, bool skipLastEvent = true)
	{
		bool flag = !skipLastEvent;
		if (flag)
		{
			Action onAnimationOutComplete = this._onAnimationOutComplete;
			if (onAnimationOutComplete != null)
			{
				onAnimationOutComplete();
			}
		}
		this._onAnimationOutComplete = onAnimationComplete;
		bool flag2 = this.AnimSequenceIn != null;
		if (flag2)
		{
			bool flag3 = this.AnimSequenceIn.IsPlaying();
			if (flag3)
			{
				this.AnimSequenceIn.Pause<Sequence>();
				bool flag4 = !skipLastEvent;
				if (flag4)
				{
					TweenCallback onComplete = this.AnimSequenceIn.onComplete;
					if (onComplete != null)
					{
						onComplete();
					}
				}
			}
			this.AnimSequenceIn.onComplete = null;
		}
		bool flag5 = this.AnimSequenceOut != null;
		if (flag5)
		{
			this.AnimSequenceOut.SetAutoKill(false);
			this.AnimSequenceOut.OnComplete(new TweenCallback(this.<PlayHideAnimation>g__OnComplete|12_0));
			this.AnimSequenceOut.Restart(true, -1f);
		}
		else
		{
			this.<PlayHideAnimation>g__OnComplete|12_0();
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00025AC7 File Offset: 0x00023CC7
	[CompilerGenerated]
	private void <PlayShowAnimation>g__OnComplete|11_0()
	{
		Action onAnimationInComplete = this._onAnimationInComplete;
		if (onAnimationInComplete != null)
		{
			onAnimationInComplete();
		}
		this._onAnimationInComplete = null;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00025B06 File Offset: 0x00023D06
	[CompilerGenerated]
	private void <PlayHideAnimation>g__OnComplete|12_0()
	{
		Action onAnimationOutComplete = this._onAnimationOutComplete;
		if (onAnimationOutComplete != null)
		{
			onAnimationOutComplete();
		}
		this._onAnimationOutComplete = null;
	}

	// Token: 0x04000498 RID: 1176
	private Action _onAnimationInComplete;

	// Token: 0x04000499 RID: 1177
	private Action _onAnimationOutComplete;
}

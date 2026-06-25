using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

// Token: 0x0200005C RID: 92
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPTextFadeAnimation : MonoBehaviour
{
	// Token: 0x060002FF RID: 767 RVA: 0x00012206 File Offset: 0x00010406
	private void Awake()
	{
		this._textComponent = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00012218 File Offset: 0x00010418
	private void OnEnable()
	{
		bool autoPlay = this.AutoPlay;
		if (autoPlay)
		{
			this.Play();
		}
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00012237 File Offset: 0x00010437
	private void OnDisable()
	{
		CommonUtils.TryKillTween(this._tweener, false);
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00012248 File Offset: 0x00010448
	public void Play()
	{
		CommonUtils.TryKillTween(this._tweener, true);
		Color color = this._textComponent.color;
		color.a = this.From;
		this._textComponent.color = color;
		this._tweener = DOVirtual.Float(this.From, this.To, this.Duration, delegate(float stepValue)
		{
			color.a = stepValue;
			this._textComponent.color = color;
		}).SetLoops(this.LoopCount, this.LoopType).SetEase(this.Ease).SetAutoKill(true);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x000122F0 File Offset: 0x000104F0
	public void Rewind(TweenCallback onComplete = null)
	{
		CommonUtils.TryKillTween(this._tweener, false);
		Color color = this._textComponent.color;
		color.a = this.To;
		this._textComponent.color = color;
		this._tweener = DOVirtual.Float(this.To, this.From, this.Duration, delegate(float stepValue)
		{
			color.a = stepValue;
			this._textComponent.color = color;
		}).OnComplete(delegate
		{
			TweenCallback onComplete2 = onComplete;
			if (onComplete2 != null)
			{
				onComplete2();
			}
		}).SetLoops(this.LoopCount, this.LoopType).SetEase(this.Ease).SetAutoKill(true);
	}

	// Token: 0x040001A9 RID: 425
	public float Duration;

	// Token: 0x040001AA RID: 426
	public float From;

	// Token: 0x040001AB RID: 427
	public float To;

	// Token: 0x040001AC RID: 428
	public bool AutoPlay;

	// Token: 0x040001AD RID: 429
	public int LoopCount;

	// Token: 0x040001AE RID: 430
	public LoopType LoopType;

	// Token: 0x040001AF RID: 431
	public Ease Ease;

	// Token: 0x040001B0 RID: 432
	private Tweener _tweener;

	// Token: 0x040001B1 RID: 433
	private TextMeshProUGUI _textComponent;
}

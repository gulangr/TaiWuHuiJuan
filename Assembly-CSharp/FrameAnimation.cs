using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000062 RID: 98
[RequireComponent(typeof(Image))]
public class FrameAnimation : MonoBehaviour
{
	// Token: 0x17000055 RID: 85
	// (get) Token: 0x0600032F RID: 815 RVA: 0x00013731 File Offset: 0x00011931
	private float PureTime
	{
		get
		{
			return this.unscaled ? Time.unscaledTime : Time.time;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000330 RID: 816 RVA: 0x00013747 File Offset: 0x00011947
	private float PureDeltaTime
	{
		get
		{
			return this.unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000331 RID: 817 RVA: 0x0001375D File Offset: 0x0001195D
	public sbyte FrameSpeed
	{
		get
		{
			return (sbyte)Mathf.RoundToInt(1f / this._frameAnimationSwitchTime);
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000332 RID: 818 RVA: 0x00013771 File Offset: 0x00011971
	// (set) Token: 0x06000333 RID: 819 RVA: 0x00013779 File Offset: 0x00011979
	public bool Playing { get; private set; }

	// Token: 0x06000334 RID: 820 RVA: 0x00013782 File Offset: 0x00011982
	public void Play()
	{
		this.Reset();
		this.Playing = true;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00013794 File Offset: 0x00011994
	public void Reset()
	{
		this._spriteIndex = 0;
		bool flag = this._image;
		if (flag)
		{
			this._image.sprite = this._sprites[(int)this._spriteIndex];
			bool flag2 = this.autoSize;
			if (flag2)
			{
				this._image.SetNativeSize();
			}
		}
		this._scaleCache = this.ScaleFrom;
		this._alphaCache = this.AlphaFrom;
		this._lastFrameApplyTime = this.PureTime;
		this._animationTime = 0f;
		this._frameAnimationTimes = 0;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0001381F File Offset: 0x00011A1F
	public void Resume()
	{
		this.Playing = true;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0001382A File Offset: 0x00011A2A
	public void Pause()
	{
		this.Playing = false;
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00013838 File Offset: 0x00011A38
	public void SetFrameSpeed(sbyte speed)
	{
		bool flag = speed == 0;
		if (flag)
		{
			throw new Exception("FrameSpeed cannot set to zero!");
		}
		this._frameAnimationSwitchTime = 1f / (float)speed;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x00013868 File Offset: 0x00011A68
	private void ApplyFrameAnimation()
	{
		bool flag = this._sprites == null || this._sprites.Length == 0;
		if (!flag)
		{
			bool flag2 = null == this._image;
			if (flag2)
			{
				this._image = base.GetComponent<Image>();
			}
			bool flag3 = this.FrameLoopCount > 0 && this._frameAnimationTimes >= this.FrameLoopCount;
			if (!flag3)
			{
				bool flag4 = this.PureTime >= this._lastFrameApplyTime + this._frameAnimationSwitchTime;
				if (flag4)
				{
					this._spriteIndex += 1;
					bool flag5 = (int)this._spriteIndex >= this._sprites.Length;
					if (flag5)
					{
						bool flag6 = this._frameAnimationTimes < int.MaxValue;
						if (flag6)
						{
							this._frameAnimationTimes++;
						}
						bool flag7 = this.FrameLoopCount > 0 && this._frameAnimationTimes >= this.FrameLoopCount;
						if (flag7)
						{
							return;
						}
						this._spriteIndex = 0;
					}
					this._image.sprite = this._sprites[(int)this._spriteIndex];
					bool flag8 = this.autoSize;
					if (flag8)
					{
						this._image.SetNativeSize();
					}
					this._lastFrameApplyTime = this.PureTime;
				}
			}
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x000139AC File Offset: 0x00011BAC
	private void ApplyScaleAnimation()
	{
		bool flag = this._animationTime > this.Duration;
		if (flag)
		{
			bool flag2 = !this.ScaleLoop;
			if (flag2)
			{
				bool flag3 = this._scaleCache != this.ScaleTo;
				if (flag3)
				{
					this._scaleCache = this.ScaleTo;
					this._image.rectTransform.localScale = this._scaleCache;
				}
				return;
			}
		}
		float lerp = this.ScaleCurve.Evaluate(this._animationTime % this.Duration / this.Duration);
		this._scaleCache = Vector2.Lerp(this.ScaleFrom, this.ScaleTo, lerp);
		this._image.rectTransform.localScale = this._scaleCache;
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00013A74 File Offset: 0x00011C74
	private void ApplyAlphaAnimation()
	{
		bool flag = this._animationTime > this.Duration;
		if (flag)
		{
			bool flag2 = !this.AlphaLoop;
			if (flag2)
			{
				bool flag3 = Math.Abs(this._alphaCache - this.AlphaTo) > 0.001f;
				if (flag3)
				{
					this._alphaCache = this.AlphaTo;
					this._image.color = this._image.color.SetAlpha(this._alphaCache);
				}
				return;
			}
		}
		Color color = this._image.color;
		float lerp = this.AlphaCurve.Evaluate(this._animationTime % this.Duration / this.Duration);
		this._alphaCache = Mathf.Lerp(this.AlphaFrom, this.AlphaTo, lerp);
		this._image.color = color.SetAlpha(this._alphaCache);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00013B51 File Offset: 0x00011D51
	private void Awake()
	{
		this._image = base.GetComponent<Image>();
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00013B60 File Offset: 0x00011D60
	private void OnEnable()
	{
		bool autoPlay = this.AutoPlay;
		if (autoPlay)
		{
			this.Play();
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x00013B7F File Offset: 0x00011D7F
	private void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00013B8C File Offset: 0x00011D8C
	private void Update()
	{
		bool flag = !this.Playing;
		if (!flag)
		{
			bool flag2 = this.Duration == 0f;
			if (!flag2)
			{
				this.ApplyFrameAnimation();
				bool scaleAnimationOn = this.ScaleAnimationOn;
				if (scaleAnimationOn)
				{
					this.ApplyScaleAnimation();
				}
				bool alphaAnimationOn = this.AlphaAnimationOn;
				if (alphaAnimationOn)
				{
					this.ApplyAlphaAnimation();
				}
				this._animationTime += this.PureDeltaTime;
				bool flag3 = this._animationTime >= this.Duration * 65535f;
				if (flag3)
				{
					this._animationTime = 0f;
				}
			}
		}
	}

	// Token: 0x040001D4 RID: 468
	public bool autoSize = true;

	// Token: 0x040001D5 RID: 469
	public bool unscaled;

	// Token: 0x040001D6 RID: 470
	public bool AutoPlay = true;

	// Token: 0x040001D7 RID: 471
	public int FrameLoopCount = -1;

	// Token: 0x040001D8 RID: 472
	private Image _image;

	// Token: 0x040001D9 RID: 473
	[SerializeField]
	private Sprite[] _sprites;

	// Token: 0x040001DA RID: 474
	private ushort _spriteIndex;

	// Token: 0x040001DB RID: 475
	private float _lastFrameApplyTime;

	// Token: 0x040001DC RID: 476
	[SerializeField]
	private float _frameAnimationSwitchTime = 0.04f;

	// Token: 0x040001DD RID: 477
	private int _frameAnimationTimes;

	// Token: 0x040001DE RID: 478
	public bool ScaleAnimationOn;

	// Token: 0x040001DF RID: 479
	public bool ScaleLoop;

	// Token: 0x040001E0 RID: 480
	public AnimationCurve ScaleCurve;

	// Token: 0x040001E1 RID: 481
	public Vector2 ScaleFrom = Vector2.one;

	// Token: 0x040001E2 RID: 482
	public Vector2 ScaleTo = Vector2.one;

	// Token: 0x040001E3 RID: 483
	private Vector2 _scaleCache;

	// Token: 0x040001E4 RID: 484
	public bool AlphaAnimationOn;

	// Token: 0x040001E5 RID: 485
	public bool AlphaLoop;

	// Token: 0x040001E6 RID: 486
	public AnimationCurve AlphaCurve;

	// Token: 0x040001E7 RID: 487
	public float AlphaFrom = 1f;

	// Token: 0x040001E8 RID: 488
	public float AlphaTo = 1f;

	// Token: 0x040001E9 RID: 489
	private float _alphaCache;

	// Token: 0x040001EB RID: 491
	public float Duration = 1f;

	// Token: 0x040001EC RID: 492
	private float _animationTime;
}

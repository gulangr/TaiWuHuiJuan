using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000063 RID: 99
[ExecuteAlways]
public class GraphicAnimEffect : MonoBehaviour
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000341 RID: 833 RVA: 0x00013C8B File Offset: 0x00011E8B
	private Transform _RealTarget
	{
		get
		{
			return (this.Target == null) ? base.transform : this.Target;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000342 RID: 834 RVA: 0x00013CA9 File Offset: 0x00011EA9
	private Graphic[] _RealGraphic
	{
		get
		{
			return this._RealTarget.GetComponents<Graphic>();
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00013CB8 File Offset: 0x00011EB8
	public void Stop()
	{
		this._Process = -1f;
		this._LastSampingTargetTime = -1f;
		foreach (Graphic g in this._RealGraphic)
		{
			g.enabled = false;
		}
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00013D00 File Offset: 0x00011F00
	public void Play()
	{
		this.Stop();
		this._Process = 0f;
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00013D18 File Offset: 0x00011F18
	private void UpdateAnimation()
	{
		Transform target = this._RealTarget;
		Graphic[] graphic = this._RealGraphic;
		bool flag = this._Process >= 0f;
		if (flag)
		{
			bool flag2 = this._Process == 0f;
			if (flag2)
			{
				foreach (Graphic g in graphic)
				{
					g.enabled = true;
				}
			}
			bool flag3 = this.DurationSampingTarget != null;
			if (flag3)
			{
				this._LastSampingTargetTime = this._Process;
				this._Process = this.DurationSampingTarget.time % this.DurationSampingTarget.main.duration;
			}
			else
			{
				this._Process += Time.deltaTime;
			}
			float normalizedDuration = this._Process / this.Duration;
			float normalizedAnimTime = this._Process / this.AnimTime;
			bool flag4 = (double)normalizedAnimTime >= 1.0;
			if (flag4)
			{
				foreach (Graphic g2 in graphic)
				{
					g2.enabled = false;
				}
			}
			else
			{
				target.localScale = new Vector3(this.SizeOverAnimTime.Evaluate(normalizedAnimTime), this.SizeOverAnimTime.Evaluate(normalizedAnimTime), target.localScale.z);
				foreach (Graphic g3 in graphic)
				{
					g3.color = this.ColorOverAnimTime.Evaluate(normalizedAnimTime);
				}
			}
			bool flag5 = (double)normalizedDuration >= 1.0 || (this.DurationSampingTarget != null && this._Process < this._LastSampingTargetTime);
			if (flag5)
			{
				bool isLoop = this.IsLoop;
				if (isLoop)
				{
					this._Process = 0f;
				}
				else
				{
					this._Process = -1f;
				}
			}
		}
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00013F0B File Offset: 0x0001210B
	private void Update()
	{
		this.UpdateAnimation();
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00013F18 File Offset: 0x00012118
	private void OnDrawGizmos()
	{
		bool flag = !Application.isPlaying;
		if (flag)
		{
			this.UpdateAnimation();
		}
	}

	// Token: 0x040001ED RID: 493
	public RectTransform Target;

	// Token: 0x040001EE RID: 494
	public ParticleSystem DurationSampingTarget;

	// Token: 0x040001EF RID: 495
	public bool IsLoop;

	// Token: 0x040001F0 RID: 496
	public float Duration;

	// Token: 0x040001F1 RID: 497
	public float AnimTime;

	// Token: 0x040001F2 RID: 498
	public AnimationCurve SizeOverAnimTime;

	// Token: 0x040001F3 RID: 499
	public Gradient ColorOverAnimTime;

	// Token: 0x040001F4 RID: 500
	private float _Process;

	// Token: 0x040001F5 RID: 501
	private float _LastSampingTargetTime;
}

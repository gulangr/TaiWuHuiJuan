using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000042 RID: 66
public class YieldHelper : MonoBehaviour
{
	// Token: 0x0600022D RID: 557 RVA: 0x0000D100 File Offset: 0x0000B300
	public IEnumerator ReStartCoroutine(IEnumerator currentCoroutine, IEnumerator newCoroutine)
	{
		bool destroying = this._destroying;
		IEnumerator result;
		if (destroying)
		{
			result = null;
		}
		else
		{
			bool flag = currentCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(currentCoroutine);
			}
			base.StartCoroutine(newCoroutine);
			result = newCoroutine;
		}
		return result;
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000D13C File Offset: 0x0000B33C
	public Coroutine StartYield(IEnumerator co)
	{
		bool destroying = this._destroying;
		Coroutine result;
		if (destroying)
		{
			result = null;
		}
		else
		{
			result = base.StartCoroutine(co);
		}
		return result;
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000D164 File Offset: 0x0000B364
	public void StopYield(Coroutine co)
	{
		bool destroying = this._destroying;
		if (!destroying)
		{
			base.StopCoroutine(co);
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000D186 File Offset: 0x0000B386
	private IEnumerator internal_DelayFrameDo(uint frame, Action job)
	{
		while (frame > 0U)
		{
			uint num = frame;
			frame = num - 1U;
			yield return this._waitFrame;
		}
		job();
		yield break;
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000D1A4 File Offset: 0x0000B3A4
	public Coroutine DelayFrameDo(uint frame, Action job)
	{
		bool destroying = this._destroying;
		Coroutine result;
		if (destroying)
		{
			result = null;
		}
		else
		{
			result = base.StartCoroutine(this.internal_DelayFrameDo(frame, job));
		}
		return result;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000D1D2 File Offset: 0x0000B3D2
	private IEnumerator internal_DelaySecondsDo(float sec, Action job)
	{
		yield return new WaitForSeconds(sec);
		job();
		yield break;
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000D1F0 File Offset: 0x0000B3F0
	public Coroutine DelaySecondsDo(float sec, Action job)
	{
		bool destroying = this._destroying;
		Coroutine result;
		if (destroying)
		{
			result = null;
		}
		else
		{
			result = base.StartCoroutine(this.internal_DelaySecondsDo(sec, job));
		}
		return result;
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000D21E File Offset: 0x0000B41E
	private void OnDestroy()
	{
		base.StopAllCoroutines();
		this._destroying = true;
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000235 RID: 565 RVA: 0x0000D230 File Offset: 0x0000B430
	// (remove) Token: 0x06000236 RID: 566 RVA: 0x0000D268 File Offset: 0x0000B468
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<float> _onUpdate;

	// Token: 0x06000237 RID: 567 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
	public void StartUpdate(Action<float> job)
	{
		bool destroying = this._destroying;
		if (!destroying)
		{
			this._onUpdate += job;
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000D2C4 File Offset: 0x0000B4C4
	public void StopUpdate(Action<float> job)
	{
		bool destroying = this._destroying;
		if (!destroying)
		{
			this._onUpdate -= job;
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000D2E6 File Offset: 0x0000B4E6
	private void Update()
	{
		Profiler.BeginSample("YieldHelper.Update");
		Action<float> onUpdate = this._onUpdate;
		if (onUpdate != null)
		{
			onUpdate(Time.deltaTime);
		}
		Profiler.EndSample();
	}

	// Token: 0x04000111 RID: 273
	private readonly WaitForEndOfFrame _waitFrame = new WaitForEndOfFrame();

	// Token: 0x04000112 RID: 274
	private bool _destroying;
}

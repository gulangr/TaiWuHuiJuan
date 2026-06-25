using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000087 RID: 135
[RequireComponent(typeof(PointerTrigger))]
public class PointerScaleAnim : MonoBehaviour
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x060004EA RID: 1258 RVA: 0x000221F8 File Offset: 0x000203F8
	private Vector3 CurrentScale
	{
		get
		{
			return (this._status == PointerScaleAnim.EStatus.Base) ? this.BaseScale : this.TargetScale;
		}
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00022210 File Offset: 0x00020410
	private void Awake()
	{
		bool flag = null == this.ScaleTarget;
		if (flag)
		{
			this.ScaleTarget = base.transform;
		}
		PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
		bool flag2 = null != pointerTrigger;
		if (flag2)
		{
			bool flag3 = pointerTrigger.EnterEvent.GetPersistentEventCount() <= 0;
			if (flag3)
			{
				pointerTrigger.EnterEvent.AddListener(new UnityAction(this.ScaleToTarget));
			}
			bool flag4 = pointerTrigger.ExitEvent.GetPersistentEventCount() <= 0;
			if (flag4)
			{
				pointerTrigger.ExitEvent.AddListener(new UnityAction(this.ScaleBack));
			}
		}
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x000222AA File Offset: 0x000204AA
	private void OnEnable()
	{
		this.RevertToBase();
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x000222B4 File Offset: 0x000204B4
	private void OnDisable()
	{
		this.RevertToBase();
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x000222C0 File Offset: 0x000204C0
	public void ScaleToTarget()
	{
		bool flag = !base.gameObject.activeInHierarchy || !base.enabled;
		if (!flag)
		{
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._status = PointerScaleAnim.EStatus.Target;
			this._duration = ((this.Duration > 0f) ? this.Duration : 0.1f);
			this._coroutine = base.StartCoroutine(this.DoScale());
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00022340 File Offset: 0x00020540
	public void ScaleBack()
	{
		bool flag = !base.gameObject.activeInHierarchy || !base.enabled;
		if (!flag)
		{
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._status = PointerScaleAnim.EStatus.Base;
			this._duration = ((this.ScaleBackDuration > 0f) ? this.ScaleBackDuration : 0.2f);
			this._coroutine = base.StartCoroutine(this.DoScale());
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x000223C0 File Offset: 0x000205C0
	public void ScaleReset()
	{
		bool flag = this._duration <= 0f;
		if (flag)
		{
			base.transform.localScale = this.CurrentScale;
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x000223F4 File Offset: 0x000205F4
	private IEnumerator DoScale()
	{
		for (;;)
		{
			this.ScaleTarget.localScale = Vector3.SmoothDamp(this.ScaleTarget.localScale, this.CurrentScale, ref this._velocity, this._duration);
			this._duration -= Time.deltaTime;
			bool flag = this._duration <= 0f;
			if (flag)
			{
				break;
			}
			yield return null;
		}
		this.ScaleTarget.localScale = this.CurrentScale;
		this._coroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00022403 File Offset: 0x00020603
	private void RevertToBase()
	{
		this._status = PointerScaleAnim.EStatus.Base;
		this._duration = 0f;
		this.ScaleTarget.localScale = this.BaseScale;
	}

	// Token: 0x040003F5 RID: 1013
	public Transform ScaleTarget;

	// Token: 0x040003F6 RID: 1014
	public float Duration;

	// Token: 0x040003F7 RID: 1015
	public float ScaleBackDuration;

	// Token: 0x040003F8 RID: 1016
	public Vector3 BaseScale = Vector3.one;

	// Token: 0x040003F9 RID: 1017
	public Vector3 TargetScale = 1.1f * Vector3.one;

	// Token: 0x040003FA RID: 1018
	private PointerScaleAnim.EStatus _status;

	// Token: 0x040003FB RID: 1019
	private float _duration;

	// Token: 0x040003FC RID: 1020
	private Coroutine _coroutine;

	// Token: 0x040003FD RID: 1021
	private Vector3 _velocity;

	// Token: 0x020010FF RID: 4351
	private enum EStatus
	{
		// Token: 0x0400951B RID: 38171
		Base,
		// Token: 0x0400951C RID: 38172
		Target
	}
}

using System;
using UnityEngine;

// Token: 0x02000092 RID: 146
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class SizeFollower : MonoBehaviour
{
	// Token: 0x0600052B RID: 1323 RVA: 0x00023643 File Offset: 0x00021843
	private void Awake()
	{
		this._rectTransform = base.transform.GetComponent<RectTransform>();
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00023657 File Offset: 0x00021857
	private void Update()
	{
		this.FitToTarget();
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00023664 File Offset: 0x00021864
	private void FitToTarget()
	{
		bool flag = this._target == null || this._rectTransform == null;
		if (!flag)
		{
			this._rectTransform.sizeDelta = this._target.sizeDelta + this._offset;
		}
	}

	// Token: 0x04000434 RID: 1076
	[Tooltip("要跟随大小的目标")]
	[SerializeField]
	private RectTransform _target;

	// Token: 0x04000435 RID: 1077
	[Tooltip("与目标的大小偏移值")]
	[SerializeField]
	private Vector2 _offset = Vector2.zero;

	// Token: 0x04000436 RID: 1078
	private RectTransform _rectTransform;
}

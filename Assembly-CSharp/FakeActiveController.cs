using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
[RequireComponent(typeof(RectTransform))]
public class FakeActiveController : MonoBehaviour
{
	// Token: 0x0600032B RID: 811 RVA: 0x000136AF File Offset: 0x000118AF
	private void Awake()
	{
		this._rectTransform = base.GetComponent<RectTransform>();
		this._initialLocalScale = this._rectTransform.localScale;
		this.FakeActive = true;
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x0600032C RID: 812 RVA: 0x000136D7 File Offset: 0x000118D7
	// (set) Token: 0x0600032D RID: 813 RVA: 0x000136E0 File Offset: 0x000118E0
	public bool FakeActive
	{
		get
		{
			return this._fakeActive;
		}
		set
		{
			bool flag = null == this._rectTransform;
			if (flag)
			{
				this.Awake();
			}
			this._rectTransform.localScale = (value ? this._initialLocalScale : Vector3.zero);
			this._fakeActive = value;
		}
	}

	// Token: 0x040001D1 RID: 465
	private RectTransform _rectTransform;

	// Token: 0x040001D2 RID: 466
	private Vector3 _initialLocalScale;

	// Token: 0x040001D3 RID: 467
	private bool _fakeActive;
}

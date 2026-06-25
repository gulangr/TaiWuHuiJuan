using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Token: 0x0200003E RID: 62
public class UniEventSync_LifeCycle : MonoBehaviour
{
	// Token: 0x06000218 RID: 536 RVA: 0x0000C7FC File Offset: 0x0000A9FC
	private void Awake()
	{
		Assert.IsNotNull<GameObject>(this.m_SyncSource, "UniEventSync_LifeCycle requires a SyncSource, but it is not assigned. Defaulting to self.");
		bool flag = this.m_SyncSource == null;
		if (!flag)
		{
			this._lifeCycleDispatcher = this.m_SyncSource.GetComponent<UniEventDispatcher_LifeCyle>();
			bool flag2 = this._lifeCycleDispatcher == null;
			if (flag2)
			{
				this._lifeCycleDispatcher = this.m_SyncSource.AddComponent<UniEventDispatcher_LifeCyle>();
			}
			bool isSyncEnable = this.m_IsSyncEnable;
			if (isSyncEnable)
			{
				this._lifeCycleDispatcher.OnEnableEvent.AddListener(new UnityAction(this.OnSourceEnable));
				bool activeInHierarchy = this.m_SyncSource.activeInHierarchy;
				if (activeInHierarchy)
				{
					this.OnSourceEnable();
				}
			}
			bool isSyncDisable = this.m_IsSyncDisable;
			if (isSyncDisable)
			{
				this._lifeCycleDispatcher.OnDisableEvent.AddListener(new UnityAction(this.OnSourceDisable));
				bool flag3 = !this.m_SyncSource.activeInHierarchy;
				if (flag3)
				{
					this.OnSourceDisable();
				}
			}
			bool isSyncDestroy = this.m_IsSyncDestroy;
			if (isSyncDestroy)
			{
				this._lifeCycleDispatcher.OnDestroyEvent.AddListener(new UnityAction(this.OnSourceDestroy));
			}
		}
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000C91C File Offset: 0x0000AB1C
	private void OnSourceEnable()
	{
		bool flag = !this.m_IsSyncEnable;
		if (!flag)
		{
			this.m_SyncTarget.SetActive(true);
		}
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000C948 File Offset: 0x0000AB48
	private void OnSourceDisable()
	{
		bool flag = !this.m_IsSyncDisable;
		if (!flag)
		{
			this.m_SyncTarget.SetActive(false);
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000C974 File Offset: 0x0000AB74
	private void OnSourceDestroy()
	{
		bool flag = !this.m_IsSyncDestroy;
		if (!flag)
		{
			Object.Destroy(this.m_SyncTarget);
			Object.Destroy(this);
		}
	}

	// Token: 0x0400010A RID: 266
	[SerializeField]
	[Tooltip("要同步的源对象")]
	private GameObject m_SyncSource;

	// Token: 0x0400010B RID: 267
	[SerializeField]
	[Tooltip("当源对象未指定时，是否默认同步自身")]
	private GameObject m_SyncTarget;

	// Token: 0x0400010C RID: 268
	private UniEventDispatcher_LifeCyle _lifeCycleDispatcher;

	// Token: 0x0400010D RID: 269
	[SerializeField]
	private bool m_IsSyncEnable = true;

	// Token: 0x0400010E RID: 270
	[SerializeField]
	private bool m_IsSyncDisable = true;

	// Token: 0x0400010F RID: 271
	[SerializeField]
	private bool m_IsSyncDestroy = false;
}

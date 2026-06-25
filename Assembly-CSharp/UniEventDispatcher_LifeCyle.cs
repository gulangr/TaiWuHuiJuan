using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003D RID: 61
public class UniEventDispatcher_LifeCyle : MonoBehaviour
{
	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000210 RID: 528 RVA: 0x0000C72F File Offset: 0x0000A92F
	public UnityEvent OnEnableEvent
	{
		get
		{
			return this.m_OnEnableEvent;
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000211 RID: 529 RVA: 0x0000C737 File Offset: 0x0000A937
	public UnityEvent OnDisableEvent
	{
		get
		{
			return this.m_OnDisableEvent;
		}
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000212 RID: 530 RVA: 0x0000C73F File Offset: 0x0000A93F
	public UnityEvent<bool> OnEnableStateChangeEvent
	{
		get
		{
			return this.m_OnEnableStateChangeEvent;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000213 RID: 531 RVA: 0x0000C747 File Offset: 0x0000A947
	public UnityEvent OnDestroyEvent
	{
		get
		{
			return this.m_OnDestroyEvent;
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000C74F File Offset: 0x0000A94F
	private void OnEnable()
	{
		this.m_OnEnableEvent.Invoke();
		this.m_OnEnableStateChangeEvent.Invoke(true);
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000C76B File Offset: 0x0000A96B
	private void OnDisable()
	{
		this.m_OnDisableEvent.Invoke();
		this.m_OnEnableStateChangeEvent.Invoke(false);
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000C787 File Offset: 0x0000A987
	private void OnDestroy()
	{
		this.m_OnDestroyEvent.Invoke();
		this.m_OnEnableEvent.RemoveAllListeners();
		this.m_OnDisableEvent.RemoveAllListeners();
		this.m_OnEnableStateChangeEvent.RemoveAllListeners();
		this.m_OnDestroyEvent.RemoveAllListeners();
	}

	// Token: 0x04000106 RID: 262
	[SerializeField]
	private UnityEvent m_OnEnableEvent = new UnityEvent();

	// Token: 0x04000107 RID: 263
	[SerializeField]
	private UnityEvent m_OnDisableEvent = new UnityEvent();

	// Token: 0x04000108 RID: 264
	[SerializeField]
	private UnityEvent<bool> m_OnEnableStateChangeEvent = new UnityEvent<bool>();

	// Token: 0x04000109 RID: 265
	[SerializeField]
	private UnityEvent m_OnDestroyEvent = new UnityEvent();
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000052 RID: 82
[Obsolete]
[RequireComponent(typeof(PointerTrigger))]
[DisallowMultipleComponent]
public class SelectableCursorTriggerObsolete : MonoBehaviour
{
	// Token: 0x060002BE RID: 702 RVA: 0x00010A5C File Offset: 0x0000EC5C
	private void Start()
	{
		this._trigger = base.GetComponent<PointerTrigger>();
		bool flag = null == this._trigger;
		if (flag)
		{
			this._trigger = base.gameObject.AddComponent<PointerTrigger>();
		}
		this.AddCursorEvent();
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00010AA0 File Offset: 0x0000ECA0
	public void AddCursorEvent()
	{
		bool flag = null == this._trigger;
		if (!flag)
		{
			bool flag2 = this._trigger.EnterEvent == null;
			if (flag2)
			{
				this._trigger.EnterEvent = new UnityEvent();
			}
			bool flag3 = this._trigger.ExitEvent == null;
			if (flag3)
			{
				this._trigger.ExitEvent = new UnityEvent();
			}
			this._trigger.EnterEvent.AddListener(new UnityAction(this.Activate));
			this._trigger.ExitEvent.AddListener(new UnityAction(this.Deactivate));
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00010B3E File Offset: 0x0000ED3E
	private void OnDestroy()
	{
		SelectableCursorTriggerObsolete.ActiveTriggerSet.Remove(this);
		SelectableCursorTriggerObsolete.RefreshCursor();
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00010B53 File Offset: 0x0000ED53
	private void OnDisable()
	{
		SelectableCursorTriggerObsolete.ActiveTriggerSet.Remove(this);
		SelectableCursorTriggerObsolete.RefreshCursor();
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00010B68 File Offset: 0x0000ED68
	private void Activate()
	{
		bool flag = !string.IsNullOrEmpty(this.CursorSpriteNameOnActive);
		if (flag)
		{
			this.Active = true;
			SelectableCursorTriggerObsolete.ActiveTriggerSet.Add(this);
			SelectableCursorTriggerObsolete.RefreshCursor();
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00010BA4 File Offset: 0x0000EDA4
	private void Deactivate()
	{
		bool active = this.Active;
		if (active)
		{
			this.Active = false;
			SelectableCursorTriggerObsolete.ActiveTriggerSet.Remove(this);
			SelectableCursorTriggerObsolete.RefreshCursor();
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00010BD8 File Offset: 0x0000EDD8
	private static void RefreshCursor()
	{
		bool flag = SelectableCursorTriggerObsolete.ActiveTriggerSet.Count > 0;
		if (flag)
		{
			ConchShipCursor.Instance.TrySetClickableCursor();
		}
		else
		{
			ConchShipCursor.Instance.TrySetDefaultCursor();
		}
	}

	// Token: 0x04000172 RID: 370
	private static readonly HashSet<SelectableCursorTriggerObsolete> ActiveTriggerSet = new HashSet<SelectableCursorTriggerObsolete>();

	// Token: 0x04000173 RID: 371
	public bool Active;

	// Token: 0x04000174 RID: 372
	public string CursorSpriteNameOnActive;

	// Token: 0x04000175 RID: 373
	private PointerTrigger _trigger;
}

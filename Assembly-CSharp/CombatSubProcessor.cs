using System;
using System.Collections.Generic;
using GameData.Common;
using GameData.GameDataBridge;

// Token: 0x02000119 RID: 281
public abstract class CombatSubProcessor
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060009FB RID: 2555 RVA: 0x00041C19 File Offset: 0x0003FE19
	protected static CombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<CombatModel>();
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00041C20 File Offset: 0x0003FE20
	protected CombatSubProcessor()
	{
		this._handlers = new List<CombatNotifyHandler>(CombatNotifyHelper.ParseHandlerData<CombatSubProcessor>(this));
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00041C3C File Offset: 0x0003FE3C
	public void Setup()
	{
		bool ready = this._ready;
		if (!ready)
		{
			CombatSubProcessor.Model.AddSubProcessor(this);
			this._ready = true;
			this.OnSetup();
		}
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x00041C70 File Offset: 0x0003FE70
	public void Close()
	{
		bool flag = !this._ready;
		if (!flag)
		{
			CombatSubProcessor.Model.RemoveSubProcessor(this);
			this._ready = false;
			this.OnClose();
		}
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00041CA7 File Offset: 0x0003FEA7
	public void AntiReady()
	{
		this._ready = false;
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00041CB0 File Offset: 0x0003FEB0
	public void Process(NotificationWrapper wrapper)
	{
		bool flag = this._handlers == null;
		if (!flag)
		{
			foreach (CombatNotifyHandler handler in this._handlers)
			{
				bool flag2 = handler.IsMatch(wrapper.Notification);
				if (flag2)
				{
					handler.Handle(wrapper);
				}
			}
		}
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00041D24 File Offset: 0x0003FF24
	public IEnumerable<DataUid> GetProcessorDataUids()
	{
		bool flag = this._handlers == null;
		if (flag)
		{
			yield break;
		}
		foreach (CombatNotifyHandler handler in this._handlers)
		{
			bool flag2 = handler.DataAttribute == null;
			if (!flag2)
			{
				DataUid uid = handler.DataAttribute.Uid;
				bool flag3 = handler.SubProcessor != null;
				if (flag3)
				{
					yield return new DataUid(uid.DomainId, uid.DataId, handler.SubProcessor.SubId0, uid.SubId1);
				}
				else
				{
					yield return new DataUid(uid.DomainId, uid.DataId, ulong.MaxValue, uint.MaxValue);
				}
				uid = default(DataUid);
				handler = default(CombatNotifyHandler);
			}
		}
		IEnumerator<CombatNotifyHandler> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00041D34 File Offset: 0x0003FF34
	protected virtual void OnSetup()
	{
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00041D37 File Offset: 0x0003FF37
	protected virtual void OnClose()
	{
	}

	// Token: 0x04000CF7 RID: 3319
	private readonly IReadOnlyList<CombatNotifyHandler> _handlers;

	// Token: 0x04000CF8 RID: 3320
	private bool _ready;
}

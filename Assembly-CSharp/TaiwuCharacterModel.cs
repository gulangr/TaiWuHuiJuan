using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

// Token: 0x02000142 RID: 322
public class TaiwuCharacterModel : ISingletonInit, IDisposable
{
	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x0600114E RID: 4430 RVA: 0x00069192 File Offset: 0x00067392
	public bool HasInheritedTaiwu
	{
		get
		{
			return this.OrgInfo.OrgTemplateId == 16;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x0600114F RID: 4431 RVA: 0x000691A3 File Offset: 0x000673A3
	// (set) Token: 0x06001150 RID: 4432 RVA: 0x000691AB File Offset: 0x000673AB
	public short LoopingNeigong { get; private set; }

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001151 RID: 4433 RVA: 0x000691B4 File Offset: 0x000673B4
	// (set) Token: 0x06001152 RID: 4434 RVA: 0x000691BC File Offset: 0x000673BC
	public int Exp { get; private set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001153 RID: 4435 RVA: 0x000691C5 File Offset: 0x000673C5
	// (set) Token: 0x06001154 RID: 4436 RVA: 0x000691CD File Offset: 0x000673CD
	public OrganizationInfo OrgInfo { get; private set; }

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06001155 RID: 4437 RVA: 0x000691D8 File Offset: 0x000673D8
	// (remove) Token: 0x06001156 RID: 4438 RVA: 0x00069210 File Offset: 0x00067410
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<int, int> _onExpChanged;

	// Token: 0x06001157 RID: 4439 RVA: 0x00069245 File Offset: 0x00067445
	public void AddOnExpChangedListener(Action<int, int> listener)
	{
		this._onExpChanged += listener;
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x00069250 File Offset: 0x00067450
	public void RemoveOnExpChangedListener(Action<int, int> listener)
	{
		this._onExpChanged -= listener;
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0006925C File Offset: 0x0006745C
	public void AddOnExpChangedListener(Action listener)
	{
		this._onExpChanged += delegate(int _, int _)
		{
			listener();
		};
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0006928A File Offset: 0x0006748A
	public void RemoveOnExpChangedListener(Action listener)
	{
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x00069290 File Offset: 0x00067490
	public void Init()
	{
		this.InitDefaultData();
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)taiwuCharId, TaiwuCharacterModel.TaiwuObjectsFields);
		this._taiwuCharId = taiwuCharId;
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x000692FC File Offset: 0x000674FC
	private void InitDefaultData()
	{
		this.LoopingNeigong = -1;
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x00069308 File Offset: 0x00067508
	public void Dispose()
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._taiwuCharId = taiwuCharId;
		GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)taiwuCharId, TaiwuCharacterModel.TaiwuObjectsFields);
		GameDataBridge.UnregisterListener(this._listenerId);
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x00069364 File Offset: 0x00067564
	private void OnTaiwuCharIdChange(ArgumentBox argBox)
	{
		int oldTaiwuCharId;
		argBox.Get("OldTaiwuCharId", out oldTaiwuCharId);
		int newTaiwuCharId;
		argBox.Get("NewTaiwuCharId", out newTaiwuCharId);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)oldTaiwuCharId, TaiwuCharacterModel.TaiwuObjectsFields);
		GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)newTaiwuCharId, TaiwuCharacterModel.TaiwuObjectsFields);
		this._taiwuCharId = newTaiwuCharId;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x000693C0 File Offset: 0x000675C0
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				ushort domainId = uid.DomainId;
				ushort num = domainId;
				if (num == 4)
				{
					this.HandlerDataCharacterDomain(uid, wrapper, notification);
				}
			}
		}
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x00069450 File Offset: 0x00067650
	private void HandlerDataCharacterDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		bool flag = uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId;
		if (flag)
		{
			this.HandlerDataCharacterDomainTaiwu(uid, wrapper, notification);
		}
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x00069488 File Offset: 0x00067688
	private void HandlerDataCharacterDomainTaiwu(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		uint subId = uid.SubId1;
		uint num = subId;
		if (num <= 8U)
		{
			if (num != 3U)
			{
				if (num == 8U)
				{
					OrganizationInfo orgInfo = default(OrganizationInfo);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref orgInfo);
					this.OrgInfo = orgInfo;
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.Gender);
				this.DisplayingGender = (this._transgender ? GameData.Domains.Character.Gender.Flip(this.Gender) : this.Gender);
			}
		}
		else if (num != 13U)
		{
			if (num != 46U)
			{
				if (num == 66U)
				{
					int exp = 0;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref exp);
					int oldExp = this.Exp;
					bool changed = exp != oldExp;
					this.Exp = exp;
					bool flag = changed;
					if (flag)
					{
						Action<int, int> onExpChanged = this._onExpChanged;
						if (onExpChanged != null)
						{
							onExpChanged(oldExp, exp);
						}
					}
				}
			}
			else
			{
				short loopingNeigong = 0;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref loopingNeigong);
				this.LoopingNeigong = loopingNeigong;
				GEvent.OnEvent(UiEvents.OnTaiwuLoopingNeigongMayChange, null);
			}
		}
		else
		{
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._transgender);
			this.DisplayingGender = (this._transgender ? GameData.Domains.Character.Gender.Flip(this.Gender) : this.Gender);
		}
	}

	// Token: 0x04000F13 RID: 3859
	private int _listenerId;

	// Token: 0x04000F14 RID: 3860
	private int _taiwuCharId;

	// Token: 0x04000F15 RID: 3861
	private bool _transgender;

	// Token: 0x04000F16 RID: 3862
	public sbyte Gender;

	// Token: 0x04000F17 RID: 3863
	public sbyte DisplayingGender;

	// Token: 0x04000F18 RID: 3864
	private static readonly uint[] TaiwuObjectsFields = new uint[]
	{
		46U,
		66U,
		3U,
		13U,
		8U
	};
}

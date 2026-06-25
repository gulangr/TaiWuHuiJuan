using System;
using System.Collections.Generic;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;

// Token: 0x0200013A RID: 314
public class MonthlyNotificationSortingGroups : ISingletonInit, IDisposable
{
	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06001084 RID: 4228 RVA: 0x00062D88 File Offset: 0x00060F88
	public Dictionary<int, NotificationSortingGroup> Data
	{
		get
		{
			return this._monthlyNotificationSortingGroups;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06001085 RID: 4229 RVA: 0x00062D90 File Offset: 0x00060F90
	public Dictionary<int, int> MonthlyNotificationSortingGroupOrder
	{
		get
		{
			return this._monthlyNotificationSortingGroupOrder;
		}
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x00062D98 File Offset: 0x00060F98
	public void Init()
	{
		GameApp.ClockAndLogInfo("Call MonthlyNotificationSortingGroups.Init", false);
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		this._dispatcher = DispatcherUtils.RegisterDispatcher();
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 1, 33, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 127, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00062DFC File Offset: 0x00060FFC
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 1, 33, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 127, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		DispatcherUtils.UnregisterDispatcher(this._dispatcher);
		this._gameDataListenerId = -1;
		this._dispatcher = null;
		this.IsReady = false;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x00062E5C File Offset: 0x0006105C
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = notification.Uid.DomainId == 19;
				if (flag)
				{
					ushort dataId = notification.Uid.DataId;
					ushort num = dataId;
					if (num == 127)
					{
						Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this._monthlyNotificationSortingGroups);
						this.IsReady = true;
						GEvent.OnEvent(UiEvents.OnMonthNotifySortingGroupChanged, null);
					}
				}
				else
				{
					bool flag2 = notification.Uid.DomainId == 1;
					if (flag2)
					{
						ushort dataId2 = notification.Uid.DataId;
						ushort num2 = dataId2;
						if (num2 == 33)
						{
							List<int> items = new List<int>();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref items);
							for (int i = 0; i < items.Count; i++)
							{
								this._monthlyNotificationSortingGroupOrder[items[i]] = i;
							}
							this.IsReady = true;
							GEvent.OnEvent(UiEvents.OnMonthNotifySortingGroupChanged, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x04000ED3 RID: 3795
	private int _gameDataListenerId = -1;

	// Token: 0x04000ED4 RID: 3796
	private DispatcherInstance _dispatcher;

	// Token: 0x04000ED5 RID: 3797
	public bool IsReady = false;

	// Token: 0x04000ED6 RID: 3798
	private Dictionary<int, NotificationSortingGroup> _monthlyNotificationSortingGroups = new Dictionary<int, NotificationSortingGroup>();

	// Token: 0x04000ED7 RID: 3799
	private Dictionary<int, int> _monthlyNotificationSortingGroupOrder = new Dictionary<int, int>();

	// Token: 0x04000ED8 RID: 3800
	public Dictionary<EMonthlyNotificationSectionType, LanguageKey> ReviewTitles = new Dictionary<EMonthlyNotificationSectionType, LanguageKey>
	{
		{
			EMonthlyNotificationSectionType.Biography,
			LanguageKey.UI_MonthNotify_Review_Biography
		},
		{
			EMonthlyNotificationSectionType.Gossip,
			LanguageKey.UI_MonthNotify_Review_Gossip
		},
		{
			EMonthlyNotificationSectionType.TaiwuVillage,
			LanguageKey.UI_MonthNotify_Review_TaiwuVillage
		},
		{
			EMonthlyNotificationSectionType.Worldwide,
			LanguageKey.UI_MonthNotify_Review_Worldwide
		}
	};
}

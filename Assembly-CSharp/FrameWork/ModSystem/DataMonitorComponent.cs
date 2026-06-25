using System;
using System.Collections.Generic;
using GameData.Common;
using GameData.GameDataBridge;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x02001043 RID: 4163
	public sealed class DataMonitorComponent : MonoBehaviour, IAsyncMethodRequestHandler
	{
		// Token: 0x0600BDE6 RID: 48614 RVA: 0x00563568 File Offset: 0x00561768
		private void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					this.HandleDataModification(wrapper);
				}
			}
		}

		// Token: 0x0600BDE7 RID: 48615 RVA: 0x005635D8 File Offset: 0x005617D8
		public void HandleDataModification(NotificationWrapper wrapper)
		{
			Notification notification = wrapper.Notification;
			DataUid uid = notification.Uid;
			foreach (ValueTuple<UIBase.MonitorDataField, AsyncMethodCallbackDelegate> tuple in this.MonitorFieldHandlers)
			{
				UIBase.MonitorDataField monitorField = tuple.Item1;
				bool flag = uid.DomainId != monitorField.DomainId || uid.DataId != monitorField.DataId || uid.SubId0 != monitorField.SubId0;
				if (!flag)
				{
					bool flag2 = monitorField.SubId1List == null;
					if (flag2)
					{
						tuple.Item2(notification.ValueOffset, wrapper.DataPool);
						break;
					}
					foreach (uint subId in monitorField.SubId1List)
					{
						bool flag3 = uid.SubId1 != subId;
						if (!flag3)
						{
							tuple.Item2(notification.ValueOffset, wrapper.DataPool);
							return;
						}
					}
				}
			}
		}

		// Token: 0x0600BDE8 RID: 48616 RVA: 0x00563704 File Offset: 0x00561904
		private void OnEnable()
		{
			this.InitMonitorFieldHandlers();
			bool flag = this.GameDataListenerId < 0 && this.NeedGameDataListenerId();
			if (flag)
			{
				this.GameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
			}
		}

		// Token: 0x0600BDE9 RID: 48617 RVA: 0x00563748 File Offset: 0x00561948
		private void OnDisable()
		{
			this.ClearAsyncMethodCalls();
			bool flag = this.GameDataListenerId >= 0;
			if (flag)
			{
				this.ClearMonitorFieldHandlers();
				GameDataBridge.UnregisterListener(this.GameDataListenerId);
			}
			this.GameDataListenerId = -1;
		}

		// Token: 0x0600BDEA RID: 48618 RVA: 0x00563789 File Offset: 0x00561989
		public void InitMonitorFieldHandlers()
		{
		}

		// Token: 0x0600BDEB RID: 48619 RVA: 0x0056378C File Offset: 0x0056198C
		public void AppendMonitorFieldHandler(UIBase.MonitorDataField dataField, AsyncMethodCallbackDelegate callback)
		{
			bool flag = this.GameDataListenerId < 0;
			if (flag)
			{
				this.GameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
			}
			this.MonitorFieldHandlers.Add(new ValueTuple<UIBase.MonitorDataField, AsyncMethodCallbackDelegate>(dataField, callback));
			bool flag2 = dataField.SubId1List != null;
			if (flag2)
			{
				GameDataBridge.AddDataMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
		}

		// Token: 0x0600BDEC RID: 48620 RVA: 0x00563824 File Offset: 0x00561A24
		public void AppendMonitorFieldHandler(ushort domainId, ushort dataId, ulong subId0 = 18446744073709551615UL, uint subId1 = 4294967295U, AsyncMethodCallbackDelegate callback = null)
		{
			bool flag = subId1 == uint.MaxValue;
			if (flag)
			{
				this.AppendMonitorFieldHandler(new UIBase.MonitorDataField(domainId, dataId, subId0, null), callback);
			}
			else
			{
				this.AppendMonitorFieldHandler(new UIBase.MonitorDataField(domainId, dataId, subId0, new uint[]
				{
					subId1
				}), callback);
			}
		}

		// Token: 0x0600BDED RID: 48621 RVA: 0x0056386C File Offset: 0x00561A6C
		public void RemoveMonitorFieldHandler(int index)
		{
			UIBase.MonitorDataField dataField = this.MonitorFieldHandlers[index].Item1;
			this.MonitorFieldHandlers.RemoveAt(index);
			bool flag = dataField.SubId1List != null;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
		}

		// Token: 0x0600BDEE RID: 48622 RVA: 0x005638EC File Offset: 0x00561AEC
		public void RemoveMonitorFieldHandler(ushort domainId, ushort dataId, ulong subId0)
		{
			for (int i = 0; i < this.MonitorFieldHandlers.Count; i++)
			{
				UIBase.MonitorDataField dataField = this.MonitorFieldHandlers[i].Item1;
				bool flag = dataField.DomainId == domainId && dataField.DataId == dataId && dataField.SubId0 == subId0;
				if (flag)
				{
					bool flag2 = dataField.SubId1List != null;
					if (flag2)
					{
						GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, domainId, dataId, subId0, dataField.SubId1List);
					}
					else
					{
						GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, domainId, dataId, subId0, uint.MaxValue);
					}
					this.MonitorFieldHandlers.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x0600BDEF RID: 48623 RVA: 0x00563994 File Offset: 0x00561B94
		public List<ValueTuple<UIBase.MonitorDataField, AsyncMethodCallbackDelegate>> GetMonitorFieldHandlers()
		{
			return this.MonitorFieldHandlers;
		}

		// Token: 0x0600BDF0 RID: 48624 RVA: 0x005639AC File Offset: 0x00561BAC
		public void ClearMonitorFieldHandlers()
		{
			for (int i = 0; i < this.MonitorFieldHandlers.Count; i++)
			{
				UIBase.MonitorDataField dataField = this.MonitorFieldHandlers[i].Item1;
				bool flag = dataField.SubId1List != null;
				if (flag)
				{
					GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
				}
				else
				{
					GameDataBridge.AddDataUnMonitor(this.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
				}
			}
			this.MonitorFieldHandlers.Clear();
		}

		// Token: 0x0600BDF1 RID: 48625 RVA: 0x00563A48 File Offset: 0x00561C48
		public bool NeedGameDataListenerId()
		{
			return this.MonitorFieldHandlers.Count > 0 || this.NeedDataListenerId;
		}

		// Token: 0x0600BDF2 RID: 48626 RVA: 0x00563A71 File Offset: 0x00561C71
		public void RegisterAsyncMethodCall(int requestId)
		{
			this._requestedAsyncMethods.Add(requestId);
		}

		// Token: 0x0600BDF3 RID: 48627 RVA: 0x00563A84 File Offset: 0x00561C84
		public void ClearAsyncMethodCalls()
		{
			AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
			foreach (int one in this._requestedAsyncMethods)
			{
				dispatcher.UnregisterAsyncMethodCall(one);
			}
			this._requestedAsyncMethods.Clear();
		}

		// Token: 0x0600BDF4 RID: 48628 RVA: 0x00563AF0 File Offset: 0x00561CF0
		public void AsyncMethodCall(ushort domainId, ushort methodId, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(domainId, methodId, callback));
		}

		// Token: 0x0600BDF5 RID: 48629 RVA: 0x00563B0C File Offset: 0x00561D0C
		public void AsyncMethodCall<T1>(ushort domainId, ushort methodId, T1 arg1, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1>(domainId, methodId, arg1, callback));
		}

		// Token: 0x0600BDF6 RID: 48630 RVA: 0x00563B2A File Offset: 0x00561D2A
		public void AsyncMethodCall<T1, T2>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2>(domainId, methodId, arg1, arg2, callback));
		}

		// Token: 0x0600BDF7 RID: 48631 RVA: 0x00563B4A File Offset: 0x00561D4A
		public void AsyncMethodCall<T1, T2, T3>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3>(domainId, methodId, arg1, arg2, arg3, callback));
		}

		// Token: 0x0600BDF8 RID: 48632 RVA: 0x00563B6C File Offset: 0x00561D6C
		public void AsyncMethodCall<T1, T2, T3, T4>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4>(domainId, methodId, arg1, arg2, arg3, arg4, callback));
		}

		// Token: 0x0600BDF9 RID: 48633 RVA: 0x00563B9C File Offset: 0x00561D9C
		public void AsyncMethodCall<T1, T2, T3, T4, T5>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, callback));
		}

		// Token: 0x0600BDFA RID: 48634 RVA: 0x00563BD0 File Offset: 0x00561DD0
		public void AsyncMethodCall<T1, T2, T3, T4, T5, T6>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, callback));
		}

		// Token: 0x0600BDFB RID: 48635 RVA: 0x00563C04 File Offset: 0x00561E04
		public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback));
		}

		// Token: 0x0600BDFC RID: 48636 RVA: 0x00563C3C File Offset: 0x00561E3C
		public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, AsyncMethodCallbackDelegate callback)
		{
			this._requestedAsyncMethods.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback));
		}

		// Token: 0x0400921C RID: 37404
		public int GameDataListenerId = -1;

		// Token: 0x0400921D RID: 37405
		public bool NeedDataListenerId = false;

		// Token: 0x0400921E RID: 37406
		public readonly List<ValueTuple<UIBase.MonitorDataField, AsyncMethodCallbackDelegate>> MonitorFieldHandlers = new List<ValueTuple<UIBase.MonitorDataField, AsyncMethodCallbackDelegate>>();

		// Token: 0x0400921F RID: 37407
		private readonly List<int> _requestedAsyncMethods = new List<int>();
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameData.Common;
using GameData.DLC;
using GameData.Domains.Global;
using GameData.Domains.Mod;
using GameData.GameDataBridge.VnPipe;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace GameData.GameDataBridge
{
	// Token: 0x02000FB4 RID: 4020
	public static class GameDataBridge
	{
		// Token: 0x0600B8D7 RID: 47319 RVA: 0x005440EC File Offset: 0x005422EC
		public static int RegisterListener(GameDataBridge.NotificationHandler handler)
		{
			int listenerId = GameDataBridge.GetNextListenerId();
			GameDataBridge.NotificationHandlers.Add(listenerId, handler);
			return listenerId;
		}

		// Token: 0x0600B8D8 RID: 47320 RVA: 0x00544112 File Offset: 0x00542312
		public static void RegisterDisplayEventHandler(GameDataBridge.NotificationHandler handler)
		{
			GameDataBridge._displayEventHandler = handler;
		}

		// Token: 0x0600B8D9 RID: 47321 RVA: 0x0054411B File Offset: 0x0054231B
		public static void UnregisterListener(int listenerId)
		{
			GameDataBridge.NotificationHandlers.Remove(listenerId);
		}

		// Token: 0x0600B8DA RID: 47322 RVA: 0x0054412C File Offset: 0x0054232C
		public static void AddDataMonitor(int listenerId, ushort domainId, ushort dataId, ulong subId0 = 18446744073709551615UL, uint subId1 = 4294967295U)
		{
			Operation operation = Operation.CreateDataMonitor(domainId, dataId, subId0, subId1);
			GameDataBridge._pendingOperations.Operations.Add(operation);
			GameDataBridge.AddRelationship(listenerId, domainId, dataId, subId0, subId1);
		}

		// Token: 0x0600B8DB RID: 47323 RVA: 0x00544164 File Offset: 0x00542364
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddDataMonitor(int listenerId, ushort domainId, ushort dataId, ulong[] subIds0)
		{
			foreach (ulong subId0 in subIds0)
			{
				GameDataBridge.AddDataMonitor(listenerId, domainId, dataId, subId0, uint.MaxValue);
			}
		}

		// Token: 0x0600B8DC RID: 47324 RVA: 0x00544194 File Offset: 0x00542394
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddDataMonitor(int listenerId, ushort domainId, ushort dataId, ulong subId0, uint[] subIds1)
		{
			foreach (uint subId in subIds1)
			{
				GameDataBridge.AddDataMonitor(listenerId, domainId, dataId, subId0, subId);
			}
		}

		// Token: 0x0600B8DD RID: 47325 RVA: 0x005441C4 File Offset: 0x005423C4
		public static void AddDataUnMonitor(int listenerId, ushort domainId, ushort dataId, ulong subId0 = 18446744073709551615UL, uint subId1 = 4294967295U)
		{
			bool isStillListening = GameDataBridge.RemoveRelationship(listenerId, domainId, dataId, subId0, subId1);
			bool flag = isStillListening;
			if (!flag)
			{
				Operation operation = Operation.CreateDataUnMonitor(domainId, dataId, subId0, subId1);
				GameDataBridge._pendingOperations.Operations.Add(operation);
			}
		}

		// Token: 0x0600B8DE RID: 47326 RVA: 0x00544204 File Offset: 0x00542404
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddDataUnMonitor(int listenerId, ushort domainId, ushort dataId, ulong[] subIds0)
		{
			foreach (ulong subId0 in subIds0)
			{
				GameDataBridge.AddDataUnMonitor(listenerId, domainId, dataId, subId0, uint.MaxValue);
			}
		}

		// Token: 0x0600B8DF RID: 47327 RVA: 0x00544234 File Offset: 0x00542434
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddDataUnMonitor(int listenerId, ushort domainId, ushort dataId, ulong subId0, uint[] subIds1)
		{
			foreach (uint subId in subIds1)
			{
				GameDataBridge.AddDataUnMonitor(listenerId, domainId, dataId, subId0, subId);
			}
		}

		// Token: 0x0600B8E0 RID: 47328 RVA: 0x00544264 File Offset: 0x00542464
		public static void AddDataModification<T>(ushort domainId, ushort dataId, ulong subId0, uint subId1, T value)
		{
			int offset = SerializerHolder<T>.Serialize(value, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateDataModification(domainId, dataId, subId0, subId1, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E1 RID: 47329 RVA: 0x005442A0 File Offset: 0x005424A0
		public static void AddMethodCall(int listenerId, ushort domainId, ushort methodId)
		{
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 0, -1);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E2 RID: 47330 RVA: 0x005442CC File Offset: 0x005424CC
		public static void AddMethodCall<T1>(int listenerId, ushort domainId, ushort methodId, T1 arg1)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 1, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E3 RID: 47331 RVA: 0x00544308 File Offset: 0x00542508
		public static void AddMethodCall<T1, T2>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 2, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E4 RID: 47332 RVA: 0x00544358 File Offset: 0x00542558
		public static void AddMethodCall<T1, T2, T3>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 3, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E5 RID: 47333 RVA: 0x005443B8 File Offset: 0x005425B8
		public static void AddMethodCall<T1, T2, T3, T4>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T4>.Serialize(arg4, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 4, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E6 RID: 47334 RVA: 0x0054442C File Offset: 0x0054262C
		public static void AddMethodCall<T1, T2, T3, T4, T5>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T4>.Serialize(arg4, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T5>.Serialize(arg5, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 5, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E7 RID: 47335 RVA: 0x005444B0 File Offset: 0x005426B0
		public static void AddMethodCall<T1, T2, T3, T4, T5, T6>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T4>.Serialize(arg4, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T5>.Serialize(arg5, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T6>.Serialize(arg6, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 6, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E8 RID: 47336 RVA: 0x00544548 File Offset: 0x00542748
		public static void AddMethodCall<T1, T2, T3, T4, T5, T6, T7>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T4>.Serialize(arg4, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T5>.Serialize(arg5, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T6>.Serialize(arg6, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T7>.Serialize(arg7, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 7, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8E9 RID: 47337 RVA: 0x005445F0 File Offset: 0x005427F0
		public static void AddMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(int listenerId, ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			int offset = SerializerHolder<T1>.Serialize(arg1, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T2>.Serialize(arg2, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T3>.Serialize(arg3, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T4>.Serialize(arg4, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T5>.Serialize(arg5, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T6>.Serialize(arg6, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T7>.Serialize(arg7, GameDataBridge._pendingOperations.DataPool);
			SerializerHolder<T8>.Serialize(arg8, GameDataBridge._pendingOperations.DataPool);
			Operation operation = Operation.CreateMethodCall(listenerId, domainId, methodId, 8, offset);
			GameDataBridge._pendingOperations.Operations.Add(operation);
		}

		// Token: 0x0600B8EA RID: 47338 RVA: 0x005446AC File Offset: 0x005428AC
		public static void TransferPendingOperations()
		{
			bool flag = GameDataBridge._pendingOperations.Operations.Count <= 0;
			if (!flag)
			{
				object operationCollectionsLock = GameDataBridge.OperationCollectionsLock;
				lock (operationCollectionsLock)
				{
					GameDataBridge._operationCollections.Add(GameDataBridge._pendingOperations);
					GameDataBridge._pendingOperations = new OperationCollection(65536);
				}
			}
		}

		// Token: 0x0600B8EB RID: 47339 RVA: 0x00544724 File Offset: 0x00542924
		public static void ProcessNotifications()
		{
			object notificationCollectionsLock = GameDataBridge.NotificationCollectionsLock;
			lock (notificationCollectionsLock)
			{
				bool flag2 = GameDataBridge._notificationCollections.Count <= 0;
				if (flag2)
				{
					return;
				}
				List<NotificationCollection> notificationCollections = GameDataBridge._notificationCollections;
				List<NotificationCollection> processingNotificationCollections = GameDataBridge._processingNotificationCollections;
				GameDataBridge._processingNotificationCollections = notificationCollections;
				GameDataBridge._notificationCollections = processingNotificationCollections;
				GameDataBridge._notificationCollections.Clear();
			}
			GameDataBridge.ClearNotificationCache();
			List<NotificationWrapper> displayEvents = GameDataBridge.NotificationListPool.Get();
			displayEvents.Clear();
			int i = 0;
			int collectionsCount = GameDataBridge._processingNotificationCollections.Count;
			while (i < collectionsCount)
			{
				NotificationCollection collection = GameDataBridge._processingNotificationCollections[i];
				RawDataPool dataPool = collection.DataPool;
				int j = 0;
				int notificationsCount = collection.Notifications.Count;
				while (j < notificationsCount)
				{
					Notification notification = collection.Notifications[j];
					switch (notification.Type)
					{
					case 0:
					{
						HashSet<int> initialListenerIds;
						bool flag3 = GameDataBridge.Data2InitialListeners.TryGetValue(notification.Uid, out initialListenerIds);
						if (flag3)
						{
							foreach (int listenerId in initialListenerIds)
							{
								GameDataBridge.AddNotificationToCache(listenerId, notification, dataPool);
							}
						}
						HashSet<int> listenerIds;
						bool flag4 = GameDataBridge.Data2Listeners.TryGetValue(notification.Uid, out listenerIds);
						if (flag4)
						{
							foreach (int listenerId2 in listenerIds)
							{
								bool flag5 = initialListenerIds == null || !initialListenerIds.Contains(listenerId2);
								if (flag5)
								{
									GameDataBridge.AddNotificationToCache(listenerId2, notification, dataPool);
								}
							}
						}
						bool flag6 = initialListenerIds != null;
						if (flag6)
						{
							GameDataBridge.Data2InitialListeners.Remove(notification.Uid);
							GameDataBridge.HashSetPool.Return(initialListenerIds);
						}
						break;
					}
					case 1:
						GameDataBridge.AddNotificationToCache(notification.ListenerId, notification, dataPool);
						break;
					case 2:
						displayEvents.Add(new NotificationWrapper(notification, dataPool));
						break;
					default:
						throw new Exception(string.Format("Unsupported notification type: {0}", notification.Type));
					}
					j++;
				}
				i++;
			}
			GameDataBridge._processingNotificationCollections.Clear();
			foreach (KeyValuePair<int, List<NotificationWrapper>> entry in GameDataBridge.ListenerId2Notifications)
			{
				int listenerId3 = entry.Key;
				List<NotificationWrapper> notifications = entry.Value;
				GameDataBridge.NotificationHandler notificationHandler;
				bool flag7 = GameDataBridge.NotificationHandlers.TryGetValue(listenerId3, out notificationHandler);
				if (flag7)
				{
					notificationHandler(notifications);
				}
			}
			GameDataBridge._displayEventHandler(displayEvents);
			GameDataBridge.NotificationListPool.Return(displayEvents);
		}

		// Token: 0x0600B8EC RID: 47340 RVA: 0x00544A3C File Offset: 0x00542C3C
		private static void ClearNotificationCache()
		{
			foreach (KeyValuePair<int, List<NotificationWrapper>> entry in GameDataBridge.ListenerId2Notifications)
			{
				GameDataBridge.NotificationListPool.Return(entry.Value);
			}
			GameDataBridge.ListenerId2Notifications.Clear();
		}

		// Token: 0x0600B8ED RID: 47341 RVA: 0x00544AA8 File Offset: 0x00542CA8
		private static void AddNotificationToCache(int listenerId, Notification notification, RawDataPool dataPool)
		{
			List<NotificationWrapper> currNotifications;
			bool flag = !GameDataBridge.ListenerId2Notifications.TryGetValue(listenerId, out currNotifications);
			if (flag)
			{
				currNotifications = GameDataBridge.NotificationListPool.Get();
				currNotifications.Clear();
				currNotifications.Add(new NotificationWrapper(notification, dataPool));
				GameDataBridge.ListenerId2Notifications.Add(listenerId, currNotifications);
			}
			else
			{
				currNotifications.Add(new NotificationWrapper(notification, dataPool));
			}
		}

		// Token: 0x0600B8EE RID: 47342 RVA: 0x00544B0C File Offset: 0x00542D0C
		private static int GetNextListenerId()
		{
			int listenerId = GameDataBridge._nextListenerId;
			GameDataBridge._nextListenerId++;
			bool flag = GameDataBridge._nextListenerId > int.MaxValue;
			if (flag)
			{
				GameDataBridge._nextListenerId = 0;
			}
			return listenerId;
		}

		// Token: 0x0600B8EF RID: 47343 RVA: 0x00544B48 File Offset: 0x00542D48
		private static void AddRelationship(int listenerId, ushort domainId, ushort dataId, ulong subId0, uint subId1)
		{
			DataUid uid = new DataUid(domainId, dataId, subId0, subId1);
			HashSet<int> listenerIds;
			bool flag = !GameDataBridge.Data2InitialListeners.TryGetValue(uid, out listenerIds);
			if (flag)
			{
				listenerIds = GameDataBridge.HashSetPool.Get();
				listenerIds.Clear();
				listenerIds.Add(listenerId);
				GameDataBridge.Data2InitialListeners.Add(uid, listenerIds);
			}
			else
			{
				listenerIds.Add(listenerId);
			}
			bool flag2 = !GameDataBridge.Data2Listeners.TryGetValue(uid, out listenerIds);
			if (flag2)
			{
				listenerIds = GameDataBridge.HashSetPool.Get();
				listenerIds.Clear();
				listenerIds.Add(listenerId);
				GameDataBridge.Data2Listeners.Add(uid, listenerIds);
			}
			else
			{
				listenerIds.Add(listenerId);
			}
		}

		// Token: 0x0600B8F0 RID: 47344 RVA: 0x00544BF0 File Offset: 0x00542DF0
		private static bool RemoveRelationship(int listenerId, ushort domainId, ushort dataId, ulong subId0, uint subId1)
		{
			DataUid uid = new DataUid(domainId, dataId, subId0, subId1);
			HashSet<int> listenerIds;
			bool flag = GameDataBridge.Data2InitialListeners.TryGetValue(uid, out listenerIds);
			if (flag)
			{
				listenerIds.Remove(listenerId);
				bool flag2 = listenerIds.Count <= 0;
				if (flag2)
				{
					GameDataBridge.Data2InitialListeners.Remove(uid);
					GameDataBridge.HashSetPool.Return(listenerIds);
				}
			}
			bool flag3 = GameDataBridge.Data2Listeners.TryGetValue(uid, out listenerIds);
			if (flag3)
			{
				listenerIds.Remove(listenerId);
				bool flag4 = listenerIds.Count > 0;
				if (flag4)
				{
					return true;
				}
				GameDataBridge.Data2Listeners.Remove(uid);
				GameDataBridge.HashSetPool.Return(listenerIds);
			}
			return false;
		}

		// Token: 0x0600B8F1 RID: 47345 RVA: 0x00544CA0 File Offset: 0x00542EA0
		private static void CheckOrphanMonitoringData(int unregisteredListenerId, string className)
		{
			foreach (KeyValuePair<DataUid, HashSet<int>> entry in GameDataBridge.Data2Listeners)
			{
				HashSet<int> listenerIds = entry.Value;
				bool flag = listenerIds.Contains(unregisteredListenerId);
				if (flag)
				{
					throw new Exception(string.Format("{0}: DataUid {1} is still monitoring after listener unregistered", className, entry.Key));
				}
			}
		}

		// Token: 0x0600B8F2 RID: 47346 RVA: 0x00544D20 File Offset: 0x00542F20
		[return: TupleElementNames(new string[]
		{
			"regular",
			"initial"
		})]
		public static ValueTuple<int, int> GetMonitoringStats()
		{
			return new ValueTuple<int, int>(GameDataBridge.Data2Listeners.Count, GameDataBridge.Data2InitialListeners.Count);
		}

		// Token: 0x0600B8F3 RID: 47347 RVA: 0x00544D4C File Offset: 0x00542F4C
		public static void AppendErrorMessage(string message)
		{
			object errorMessagesLock = GameDataBridge.ErrorMessagesLock;
			lock (errorMessagesLock)
			{
				GameDataBridge.ErrorMessages.Add(message);
			}
		}

		// Token: 0x0600B8F4 RID: 47348 RVA: 0x00544D98 File Offset: 0x00542F98
		public static bool CheckErrorMessages()
		{
			object errorMessagesLock = GameDataBridge.ErrorMessagesLock;
			bool result;
			lock (errorMessagesLock)
			{
				bool flag2 = GameDataBridge.ErrorMessages.Count <= 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool hasError = false;
					foreach (string errorMessage in GameDataBridge.ErrorMessages)
					{
						Debug.LogError(errorMessage);
						hasError = true;
					}
					bool flag3 = hasError;
					if (flag3)
					{
						GameApp.HasUnhandledExceptionOccurred = true;
					}
					GameDataBridge.ErrorMessages.Clear();
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600B8F5 RID: 47349 RVA: 0x00544E5C File Offset: 0x0054305C
		public static bool CheckWarningMessages()
		{
			object warningMessageLock = GameDataBridge.WarningMessageLock;
			bool result;
			lock (warningMessageLock)
			{
				bool flag2 = GameDataBridge.WarningMessages.Count <= 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					foreach (string warningMessage in GameDataBridge.WarningMessages)
					{
						Debug.LogError(warningMessage.SetColor(Color.yellow));
					}
					GameDataBridge.WarningMessages.Clear();
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600B8F6 RID: 47350 RVA: 0x00544F10 File Offset: 0x00543110
		public static void AllowSendingInitializationMessage()
		{
			GameDataBridge.SetGameDataModuleInitializationState(1);
		}

		// Token: 0x0600B8F7 RID: 47351 RVA: 0x00544F1C File Offset: 0x0054311C
		private static void SetGameDataModuleInitializationState(sbyte state)
		{
			bool flag = !GameDataModuleInitializationState.CheckTransition(GameDataBridge._gameDataModuleInitializationState, state);
			if (flag)
			{
				throw new Exception(string.Format("Invalid transition: {0} -> {1}", GameDataBridge._gameDataModuleInitializationState, state));
			}
			GameDataBridge._gameDataModuleInitializationState = state;
		}

		// Token: 0x0600B8F8 RID: 47352 RVA: 0x00544F68 File Offset: 0x00543168
		public static sbyte GetGameDataModuleInitializationState()
		{
			return GameDataBridge._gameDataModuleInitializationState;
		}

		// Token: 0x0600B8F9 RID: 47353 RVA: 0x00544F84 File Offset: 0x00543184
		public static void Initialize()
		{
			Debug.Log("Initializing GameDataBridge.");
			GameDataBridge.ShouldDisconnect = false;
			GameDataBridge._gameDataModuleInitializationState = 0;
			GameDataBridge._masterPipe = Master.Create("taiwu");
			string gameDataExePath = GameDataBridge.GetGameDataModuleExecutablePath();
			GameDataBridge.TerminateRunningGameDataModule(gameDataExePath);
			GameDataBridge._connectionTask = new Task<bool>(delegate()
			{
				bool flag = !string.IsNullOrEmpty(gameDataExePath);
				if (flag)
				{
					GameDataBridge.StartBackendProcess(gameDataExePath);
				}
				else
				{
					Debug.Log("GameData module is not given.");
				}
				return GameDataBridge._masterPipe.Wait();
			});
			GameDataBridge._connectionTask.Start();
			GameDataBridge._connectionTimer = Stopwatch.StartNew();
			GameDataBridge._readingThread = new Thread(new ThreadStart(GameDataBridge.ReadInterProcessMessages))
			{
				IsBackground = false,
				Name = "ReadInterProcessMessages"
			};
			GameDataBridge._writingThread = new Thread(new ThreadStart(GameDataBridge.WriteInterProcessMessages))
			{
				IsBackground = false,
				Name = "WriteInterProcessMessages"
			};
		}

		// Token: 0x0600B8FA RID: 47354 RVA: 0x00545058 File Offset: 0x00543258
		private static void StartBackendProcess(string gameDataModulePath)
		{
			bool skipBackend = GameApp.Instance.SkipBackend;
			if (skipBackend)
			{
				Debug.Log("Skipping GameData module.");
			}
			else
			{
				Debug.Log("Starting GameData module.");
				Process gameDataModule = new Process();
				gameDataModule.StartInfo.FileName = gameDataModulePath;
				gameDataModule.StartInfo.WorkingDirectory = (new FileInfo(gameDataModulePath).DirectoryName ?? Environment.CurrentDirectory);
				bool isTestBranch = GameApp.Instance.IsTestBranch;
				if (isTestBranch)
				{
					gameDataModule.StartInfo.ArgumentList.Add("--test-branch");
				}
				bool advanceMonthSingleThread = GameApp.Instance.AdvanceMonthSingleThread;
				if (advanceMonthSingleThread)
				{
					gameDataModule.StartInfo.ArgumentList.Add("--advance-month-single-thread");
				}
				gameDataModule.StartInfo.CreateNoWindow = !GameApp.Instance.ShowBackendWindow;
				gameDataModule.StartInfo.UseShellExecute = false;
				gameDataModule.Start();
				Debug.Log("Process of the GameData module at" + gameDataModulePath + " has started.");
			}
		}

		// Token: 0x0600B8FB RID: 47355 RVA: 0x00545150 File Offset: 0x00543350
		private static void TerminateRunningGameDataModule(string gameDataModulePath)
		{
			Process[] processes = Process.GetProcessesByName("GameData");
			Process[] array = processes;
			int i = 0;
			while (i < array.Length)
			{
				Process process = array[i];
				try
				{
					bool hasExited = process.HasExited;
					if (!hasExited)
					{
						bool flag = process.Modules.Count <= 0;
						if (!flag)
						{
							ProcessModule mainModule = process.MainModule;
							string fileName = (mainModule != null) ? mainModule.FileName : null;
							bool flag2 = fileName == gameDataModulePath;
							if (flag2)
							{
								Debug.Log(string.Format("Killing running game data module process with id {0}", process.Id));
								process.Kill();
							}
						}
					}
				}
				catch (Exception)
				{
				}
				IL_92:
				i++;
				continue;
				goto IL_92;
			}
		}

		// Token: 0x0600B8FC RID: 47356 RVA: 0x0054520C File Offset: 0x0054340C
		private static string GetGameDataModuleExecutablePath()
		{
			string baseDataDir = Directory.GetParent(Application.dataPath).FullName;
			string path = Path.Combine(baseDataDir, "Backend", "GameData.exe");
			return Path.GetFullPath(path);
		}

		// Token: 0x0600B8FD RID: 47357 RVA: 0x00545248 File Offset: 0x00543448
		public static bool CheckConnection()
		{
			bool isCompleted = GameDataBridge._connectionTask.IsCompleted;
			bool result;
			if (isCompleted)
			{
				bool flag = !GameDataBridge._connectionTask.Result;
				if (flag)
				{
					throw new Exception("pipe wait failed");
				}
				GameDataBridge._readingThread.Start();
				GameDataBridge._writingThread.Start();
				GameDataBridge._connectionTask = null;
				GameDataBridge._connectionTimer = null;
				result = true;
			}
			else
			{
				bool flag2 = GameDataBridge._connectionTask.Exception != null;
				if (flag2)
				{
					throw new Exception(string.Format("CheckConnection: {0}", GameDataBridge._connectionTask.Exception));
				}
				bool flag3 = GameDataBridge._connectionTimer.ElapsedMilliseconds >= 300000L;
				if (flag3)
				{
					throw new Exception("CheckConnection: Timeout when connecting to GameData module.");
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600B8FE RID: 47358 RVA: 0x00545300 File Offset: 0x00543500
		public static void UnInitialize()
		{
			GameDataBridge.ShouldDisconnect = true;
			bool flag = GameDataBridge._readingThread != null;
			if (flag)
			{
				bool flag2 = GameDataBridge._readingThread.ThreadState != ThreadState.Unstarted && !GameDataBridge._readingThread.Join(1000);
				if (flag2)
				{
					Debug.LogError("Failed to wait for _readingThread to terminate.");
				}
				GameDataBridge._readingThread = null;
			}
			bool flag3 = GameDataBridge._writingThread != null;
			if (flag3)
			{
				bool flag4 = GameDataBridge._writingThread.ThreadState != ThreadState.Unstarted && !GameDataBridge._writingThread.Join(1000);
				if (flag4)
				{
					Debug.LogError("Failed to wait for _writingThread to terminate.");
				}
				GameDataBridge._writingThread = null;
			}
			bool flag5 = GameDataBridge._masterPipe != null;
			if (flag5)
			{
				GameDataBridge._masterPipe.Dispose();
				GameDataBridge._masterPipe = null;
			}
		}

		// Token: 0x0600B8FF RID: 47359 RVA: 0x005453C4 File Offset: 0x005435C4
		private static void ReadInterProcessMessages()
		{
			Debug.Log("ReadInterProcessMessages thread started.");
			try
			{
				bool shouldExit = false;
				while (!shouldExit)
				{
					shouldExit = GameDataBridge.ReadInterProcessMessage();
				}
			}
			catch (Exception ex)
			{
				GameDataBridge.AppendErrorMessage(string.Format("ReadInterProcessMessages: {0}", ex));
			}
			Debug.Log("ReadInterProcessMessages thread is about to exit.");
		}

		// Token: 0x0600B900 RID: 47360 RVA: 0x00545428 File Offset: 0x00543628
		private static bool ReadInterProcessMessage()
		{
			byte messageType;
			int contentLength;
			GameDataBridge.GetUnmanagedValuesFromSocket<byte, int>(out messageType, out contentLength);
			bool result;
			switch (messageType)
			{
			case 0:
				result = GameDataBridge.ReadInterProcessMessageGameModuleInitialized(contentLength);
				break;
			case 1:
				result = GameDataBridge.ReadInterProcessMessageNotifications(contentLength);
				break;
			case 2:
				result = GameDataBridge.ReadInterProcessMessageErrorMessages(contentLength);
				break;
			case 3:
				result = GameDataBridge.ReadInterProcessMessageWarningMessages(contentLength);
				break;
			case 4:
				result = GameDataBridge.ReadInterProcessMessageDisconnect(contentLength);
				break;
			default:
				throw new Exception("Unknown message type: " + messageType.ToString());
			}
			return result;
		}

		// Token: 0x0600B901 RID: 47361 RVA: 0x005454AC File Offset: 0x005436AC
		private static bool ReadInterProcessMessageGameModuleInitialized(int contentLength)
		{
			Debug.Log("Incoming message: GameModuleInitialized");
			bool flag = contentLength != 0;
			if (flag)
			{
				throw new Exception("Content length of GameModuleInitialized message must be zero: " + contentLength.ToString());
			}
			GameDataBridge.SetGameDataModuleInitializationState(3);
			return false;
		}

		// Token: 0x0600B902 RID: 47362 RVA: 0x005454F0 File Offset: 0x005436F0
		private unsafe static bool ReadInterProcessMessageNotifications(int contentLength)
		{
			uint notificationsCount;
			GameDataBridge.GetUnmanagedValuesFromSocket<uint>(out notificationsCount);
			int notificationsContentCount = sizeof(Notification) * (int)notificationsCount;
			byte[] buffer = GameDataBridge.GetRawDataFromSocket(notificationsContentCount);
			List<Notification> notifications = new List<Notification>();
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			byte* pCurrData = pBuffer;
			byte* pEnd = pBuffer + notificationsContentCount;
			while (pCurrData < pEnd)
			{
				notifications.Add(*(Notification*)pCurrData);
				pCurrData += sizeof(Notification);
			}
			array = null;
			uint dataPoolSize;
			GameDataBridge.GetUnmanagedValuesFromSocket<uint>(out dataPoolSize);
			RawDataPool dataPool = new RawDataPool(GameDataBridge._masterPipe, (int)dataPoolSize);
			NotificationCollection notificationCollection = new NotificationCollection(notifications, dataPool);
			object notificationCollectionsLock = GameDataBridge.NotificationCollectionsLock;
			lock (notificationCollectionsLock)
			{
				GameDataBridge._notificationCollections.Add(notificationCollection);
			}
			long readSize = (long)(4 + notificationsContentCount + 4) + (long)((ulong)dataPoolSize);
			bool flag2 = (long)contentLength != readSize;
			if (flag2)
			{
				throw new Exception(string.Format("Content length: expected {0}, actual {1}", contentLength, readSize));
			}
			return false;
		}

		// Token: 0x0600B903 RID: 47363 RVA: 0x00545610 File Offset: 0x00543810
		private unsafe static bool ReadInterProcessMessageErrorMessages(int contentLength)
		{
			byte[] buffer = GameDataBridge.GetRawDataFromSocket(contentLength);
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			byte* pCurrData = pBuffer;
			byte* pEnd = pBuffer + contentLength;
			object errorMessagesLock = GameDataBridge.ErrorMessagesLock;
			lock (errorMessagesLock)
			{
				while (pCurrData < pEnd)
				{
					ushort messageLength = *(ushort*)pCurrData;
					pCurrData += 2;
					string message = Encoding.Unicode.GetString(pCurrData, (int)messageLength);
					pCurrData += messageLength;
					GameDataBridge.ErrorMessages.Add(message);
				}
			}
			bool flag2 = pCurrData != pEnd;
			if (flag2)
			{
				throw new Exception(string.Format("Content length: expected {0}, actual {1}", contentLength, (long)(pCurrData - pBuffer)));
			}
			array = null;
			Debug.Log("Incoming message: ErrorMessages");
			return false;
		}

		// Token: 0x0600B904 RID: 47364 RVA: 0x005456F4 File Offset: 0x005438F4
		private unsafe static bool ReadInterProcessMessageWarningMessages(int contentLength)
		{
			byte[] buffer = GameDataBridge.GetRawDataFromSocket(contentLength);
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			byte* pCurrData = pBuffer;
			byte* pEnd = pBuffer + contentLength;
			object warningMessageLock = GameDataBridge.WarningMessageLock;
			lock (warningMessageLock)
			{
				while (pCurrData < pEnd)
				{
					ushort messageLength = *(ushort*)pCurrData;
					pCurrData += 2;
					string message = Encoding.Unicode.GetString(pCurrData, (int)messageLength);
					pCurrData += messageLength;
					GameDataBridge.WarningMessages.Add(message);
				}
			}
			bool flag2 = pCurrData != pEnd;
			if (flag2)
			{
				throw new Exception(string.Format("Content length: expected {0}, actual {1}", contentLength, (long)(pCurrData - pBuffer)));
			}
			array = null;
			Debug.Log("Incoming message: WarningMessages");
			return false;
		}

		// Token: 0x0600B905 RID: 47365 RVA: 0x005457D8 File Offset: 0x005439D8
		private static bool ReadInterProcessMessageDisconnect(int contentLength)
		{
			Debug.Log("Incoming message: Disconnect");
			bool flag = contentLength != 0;
			if (flag)
			{
				throw new Exception("Content length of Disconnect message must be zero: " + contentLength.ToString());
			}
			GameDataBridge.ShouldDisconnect = true;
			return true;
		}

		// Token: 0x0600B906 RID: 47366 RVA: 0x00545820 File Offset: 0x00543A20
		private static void WriteInterProcessMessages()
		{
			Debug.Log("WriteInterProcessMessages thread started.");
			for (;;)
			{
				try
				{
					bool flag = GameDataBridge.WriteInterProcessMessagesDisconnect();
					if (flag)
					{
						break;
					}
					GameDataBridge.WriteInterProcessMessagesInitialize();
					GameDataBridge.WriteInterProcessMessagesOperations();
					Thread.Sleep(16);
				}
				catch (Exception ex)
				{
					GameDataBridge.AppendErrorMessage(string.Format("WriteInterProcessMessages: {0}", ex));
				}
			}
			Debug.Log("WriteInterProcessMessages thread is about to exit.");
		}

		// Token: 0x0600B907 RID: 47367 RVA: 0x00545898 File Offset: 0x00543A98
		private static int WriteInterProcessMessagesInitialize()
		{
			bool flag = GameDataBridge._gameDataModuleInitializationState != 1;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				Debug.Log("Outgoing message: Initialize");
				RawDataPool dataPool = GameDataBridge.CreateInitializeData();
				dataPool.CopyTo(GameDataBridge._masterPipe);
				GameDataBridge.SetGameDataModuleInitializationState(2);
				result = dataPool.RawDataSize;
			}
			return result;
		}

		// Token: 0x0600B908 RID: 47368 RVA: 0x005458EC File Offset: 0x00543AEC
		private unsafe static RawDataPool CreateInitializeData()
		{
			GameDataBridge.<>c__DisplayClass82_0 CS$<>8__locals1;
			CS$<>8__locals1.dataPool = new RawDataPool(25165824);
			byte* pHeader;
			int offset = CS$<>8__locals1.dataPool.Allocate(5, &pHeader);
			CS$<>8__locals1.dataPool.SetUnmanaged<byte>(offset, 0);
			object onLoadingModLock = ModManager.OnLoadingModLock;
			lock (onLoadingModLock)
			{
				int gameVersionInfoSize = SerializationHelper.GetSerializedSize(GameApp.Instance.GameVersion);
				byte* pGameVersionInfoData;
				CS$<>8__locals1.dataPool.Allocate(gameVersionInfoSize, &pGameVersionInfoData);
				SerializationHelper.Serialize(pGameVersionInfoData, GameApp.Instance.GameVersion);
				int gameBuildDateInfoSize = SerializationHelper.GetSerializedSize(GameApp.Instance.GameBuildDate);
				byte* pGameBuildDateInfoData;
				CS$<>8__locals1.dataPool.Allocate(gameBuildDateInfoSize, &pGameBuildDateInfoData);
				SerializationHelper.Serialize(pGameBuildDateInfoData, GameApp.Instance.GameBuildDate);
				GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
				settings.EnsureLoaded();
				SharedGlobalSettings sharedSettings = settings.DumpSharedGlobalSettings();
				GameDataBridge.<CreateInitializeData>g__DumpObject|82_0<SharedGlobalSettings>(sharedSettings, ref CS$<>8__locals1);
				GameDataBridge.<CreateInitializeData>g__DumpObject|82_0<DlcInfoList>(SingletonObject.getInstance<DlcManager>().GetDlcInfoList(), ref CS$<>8__locals1);
				GameDataBridge.<CreateInitializeData>g__DumpObject|82_0<ModInfoList>(ModManager.GetLoadedModInfoList(), ref CS$<>8__locals1);
			}
			int contentLength = CS$<>8__locals1.dataPool.RawDataSize - 5;
			CS$<>8__locals1.dataPool.SetUnmanaged<uint>(offset + 1, (uint)contentLength);
			bool flag2 = CS$<>8__locals1.dataPool.RawDataSize > 25165824;
			if (flag2)
			{
				Debug.Log(string.Format("RawDataPool reallocated. Current size: {0}", CS$<>8__locals1.dataPool.RawDataSize));
			}
			return CS$<>8__locals1.dataPool;
		}

		// Token: 0x0600B909 RID: 47369 RVA: 0x00545A6C File Offset: 0x00543C6C
		private static int WriteInterProcessMessagesOperations()
		{
			object operationCollectionsLock = GameDataBridge.OperationCollectionsLock;
			lock (operationCollectionsLock)
			{
				bool flag2 = GameDataBridge._operationCollections.Count <= 0;
				if (flag2)
				{
					return 0;
				}
				List<OperationCollection> operationCollections = GameDataBridge._operationCollections;
				List<OperationCollection> writingOperationCollections = GameDataBridge._writingOperationCollections;
				GameDataBridge._writingOperationCollections = operationCollections;
				GameDataBridge._operationCollections = writingOperationCollections;
				GameDataBridge._operationCollections.Clear();
			}
			int totalDataSize = 0;
			foreach (OperationCollection collection in GameDataBridge._writingOperationCollections)
			{
				int dataSize;
				byte[] buffer = GameDataBridge.CreateOperationCollectionData(collection, out dataSize);
				GameDataBridge._masterPipe.Write(buffer, 0, dataSize);
				collection.DataPool.CopyTo(GameDataBridge._masterPipe);
				totalDataSize += dataSize + collection.DataPool.RawDataSize;
			}
			GameDataBridge._writingOperationCollections.Clear();
			return totalDataSize;
		}

		// Token: 0x0600B90A RID: 47370 RVA: 0x00545B7C File Offset: 0x00543D7C
		private unsafe static byte[] CreateOperationCollectionData(OperationCollection collection, out int dataSize)
		{
			int operationsCount = collection.Operations.Count;
			int contentLengthWithoutDataPool = 4 + sizeof(Operation) * operationsCount + 4;
			int contentLength = contentLengthWithoutDataPool + collection.DataPool.RawDataSize;
			dataSize = 5 + contentLengthWithoutDataPool;
			byte[] buffer = GameDataBridge.OutgoingMessageBuffer.Get(dataSize);
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			byte* pCurrData = pBuffer;
			*pCurrData = 1;
			pCurrData++;
			*(int*)pCurrData = contentLength;
			pCurrData += 4;
			*(int*)pCurrData = operationsCount;
			pCurrData += 4;
			for (int i = 0; i < operationsCount; i++)
			{
				*(Operation*)pCurrData = collection.Operations[i];
				pCurrData += sizeof(Operation);
			}
			*(int*)pCurrData = collection.DataPool.RawDataSize;
			array = null;
			return buffer;
		}

		// Token: 0x0600B90B RID: 47371 RVA: 0x00545C54 File Offset: 0x00543E54
		private unsafe static bool WriteInterProcessMessagesDisconnect()
		{
			bool flag = !GameDataBridge.ShouldDisconnect;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Debug.Log("Outgoing message: Disconnect");
				try
				{
					byte[] buffer = GameDataBridge.OutgoingMessageBuffer.Get(5);
					try
					{
						byte[] array;
						byte* pBuffer;
						if ((array = buffer) == null || array.Length == 0)
						{
							pBuffer = null;
						}
						else
						{
							pBuffer = &array[0];
						}
						*pBuffer = 2;
						*(int*)(pBuffer + 1) = 0;
					}
					finally
					{
						byte[] array = null;
					}
					GameDataBridge._masterPipe.Write(buffer, 0, 5);
				}
				catch (Exception ex)
				{
					GameDataBridge.AppendErrorMessage(string.Format("WriteInterProcessMessagesDisconnect: {0}", ex));
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600B90C RID: 47372 RVA: 0x00545D04 File Offset: 0x00543F04
		private static byte[] GetRawDataFromSocket(int size)
		{
			byte[] buffer = GameDataBridge.IncomingMessageBuffer.Get(size);
			int received;
			for (int totalReceived = 0; totalReceived < size; totalReceived += received)
			{
				received = GameDataBridge._masterPipe.Read(buffer, totalReceived, size - totalReceived);
			}
			return buffer;
		}

		// Token: 0x0600B90D RID: 47373 RVA: 0x00545D48 File Offset: 0x00543F48
		private unsafe static void GetUnmanagedValuesFromSocket<[IsUnmanaged] T1>(out T1 item1) where T1 : struct, ValueType
		{
			int size = sizeof(T1);
			byte[] buffer = GameDataBridge.IncomingMessageBuffer.Get(size);
			int received;
			for (int totalReceived = 0; totalReceived < size; totalReceived += received)
			{
				received = GameDataBridge._masterPipe.Read(buffer, totalReceived, size - totalReceived);
			}
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			item1 = *(T1*)pBuffer;
			array = null;
		}

		// Token: 0x0600B90E RID: 47374 RVA: 0x00545DBC File Offset: 0x00543FBC
		private unsafe static void GetUnmanagedValuesFromSocket<[IsUnmanaged] T1, [IsUnmanaged] T2>(out T1 item1, out T2 item2) where T1 : struct, ValueType where T2 : struct, ValueType
		{
			int size = sizeof(T1) + sizeof(T2);
			byte[] buffer = GameDataBridge.IncomingMessageBuffer.Get(size);
			int received;
			for (int totalReceived = 0; totalReceived < size; totalReceived += received)
			{
				received = GameDataBridge._masterPipe.Read(buffer, totalReceived, size - totalReceived);
			}
			byte[] array;
			byte* pBuffer;
			if ((array = buffer) == null || array.Length == 0)
			{
				pBuffer = null;
			}
			else
			{
				pBuffer = &array[0];
			}
			item1 = *(T1*)pBuffer;
			item2 = *(T2*)(pBuffer + sizeof(T1));
			array = null;
		}

		// Token: 0x0600B910 RID: 47376 RVA: 0x00545F38 File Offset: 0x00544138
		[CompilerGenerated]
		internal unsafe static void <CreateInitializeData>g__DumpObject|82_0<T>(T dataObject, ref GameDataBridge.<>c__DisplayClass82_0 A_1) where T : ISerializableGameData
		{
			int objectSize = dataObject.GetSerializedSize();
			byte* pData;
			A_1.dataPool.Allocate(4 + objectSize, &pData);
			*(int*)pData = objectSize;
			int writtenSize = dataObject.Serialize(pData + 4);
			Debug.Assert(writtenSize == objectSize);
		}

		// Token: 0x04008F4C RID: 36684
		private static readonly Dictionary<int, GameDataBridge.NotificationHandler> NotificationHandlers = new Dictionary<int, GameDataBridge.NotificationHandler>();

		// Token: 0x04008F4D RID: 36685
		private static GameDataBridge.NotificationHandler _displayEventHandler;

		// Token: 0x04008F4E RID: 36686
		private static int _nextListenerId;

		// Token: 0x04008F4F RID: 36687
		private static readonly Dictionary<DataUid, HashSet<int>> Data2Listeners = new Dictionary<DataUid, HashSet<int>>();

		// Token: 0x04008F50 RID: 36688
		private static readonly Dictionary<DataUid, HashSet<int>> Data2InitialListeners = new Dictionary<DataUid, HashSet<int>>();

		// Token: 0x04008F51 RID: 36689
		private static readonly Dictionary<int, List<NotificationWrapper>> ListenerId2Notifications = new Dictionary<int, List<NotificationWrapper>>();

		// Token: 0x04008F52 RID: 36690
		private static readonly LocalObjectPool<HashSet<int>> HashSetPool = new LocalObjectPool<HashSet<int>>(1024, 4096);

		// Token: 0x04008F53 RID: 36691
		private static readonly LocalObjectPool<List<NotificationWrapper>> NotificationListPool = new LocalObjectPool<List<NotificationWrapper>>(32, 128);

		// Token: 0x04008F54 RID: 36692
		private const int OperationDataPoolDefaultCapacity = 65536;

		// Token: 0x04008F55 RID: 36693
		private const int InterProcessMessageBufferDefaultSize = 65536;

		// Token: 0x04008F56 RID: 36694
		private const int WritingThreadSleepInterval = 16;

		// Token: 0x04008F57 RID: 36695
		private const int ThreadJoinTimeout = 1000;

		// Token: 0x04008F58 RID: 36696
		private static Master _masterPipe;

		// Token: 0x04008F59 RID: 36697
		private static Thread _readingThread;

		// Token: 0x04008F5A RID: 36698
		private static Thread _writingThread;

		// Token: 0x04008F5B RID: 36699
		private static readonly IncreasableBuffer IncomingMessageBuffer = new IncreasableBuffer(65536);

		// Token: 0x04008F5C RID: 36700
		private static readonly IncreasableBuffer OutgoingMessageBuffer = new IncreasableBuffer(65536);

		// Token: 0x04008F5D RID: 36701
		private static Task<bool> _connectionTask;

		// Token: 0x04008F5E RID: 36702
		private static Stopwatch _connectionTimer;

		// Token: 0x04008F5F RID: 36703
		public static volatile bool ShouldDisconnect;

		// Token: 0x04008F60 RID: 36704
		private static volatile sbyte _gameDataModuleInitializationState;

		// Token: 0x04008F61 RID: 36705
		private static List<OperationCollection> _operationCollections = new List<OperationCollection>();

		// Token: 0x04008F62 RID: 36706
		private static readonly object OperationCollectionsLock = new object();

		// Token: 0x04008F63 RID: 36707
		private static OperationCollection _pendingOperations = new OperationCollection(65536);

		// Token: 0x04008F64 RID: 36708
		private static List<OperationCollection> _writingOperationCollections = new List<OperationCollection>();

		// Token: 0x04008F65 RID: 36709
		private static List<NotificationCollection> _notificationCollections = new List<NotificationCollection>();

		// Token: 0x04008F66 RID: 36710
		private static readonly object NotificationCollectionsLock = new object();

		// Token: 0x04008F67 RID: 36711
		private static List<NotificationCollection> _processingNotificationCollections = new List<NotificationCollection>();

		// Token: 0x04008F68 RID: 36712
		private static readonly List<string> ErrorMessages = new List<string>();

		// Token: 0x04008F69 RID: 36713
		private static readonly object ErrorMessagesLock = new object();

		// Token: 0x04008F6A RID: 36714
		private static readonly List<string> WarningMessages = new List<string>();

		// Token: 0x04008F6B RID: 36715
		private static readonly object WarningMessageLock = new object();

		// Token: 0x020025EE RID: 9710
		// (Invoke) Token: 0x06010D17 RID: 68887
		public delegate void NotificationHandler(List<NotificationWrapper> notifications);
	}
}

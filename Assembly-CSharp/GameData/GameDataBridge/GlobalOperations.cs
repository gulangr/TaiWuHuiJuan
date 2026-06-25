using System;
using System.Collections.Generic;
using GameData.Common;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using GameData.Serializer;
using GameData.Utilities;

namespace GameData.GameDataBridge
{
	// Token: 0x02000FB6 RID: 4022
	public static class GlobalOperations
	{
		// Token: 0x170014EF RID: 5359
		// (get) Token: 0x0600B912 RID: 47378 RVA: 0x00545FB1 File Offset: 0x005441B1
		// (set) Token: 0x0600B913 RID: 47379 RVA: 0x00545FB8 File Offset: 0x005441B8
		public static bool CanResetWorldSettings { get; private set; }

		// Token: 0x0600B914 RID: 47380 RVA: 0x00545FC0 File Offset: 0x005441C0
		public static void Initialize()
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalOperations.InscribedCharacters = new Dictionary<InscribedCharacterKey, InscribedCharacter>();
			GlobalOperations.InscribedCharacterPinOrders = new Dictionary<InscribedCharacterKey, int>();
			GlobalOperations.CurrGameWorldType = 0;
			GlobalOperations._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(GlobalOperations.OnNotifyGameData));
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 6, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 3, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 7, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B915 RID: 47381 RVA: 0x00546034 File Offset: 0x00544234
		public static void UnInitialize()
		{
			GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 0, 6, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 0, 3, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 0, 7, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 1, 36, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.UnregisterListener(GlobalOperations._listenerId);
		}

		// Token: 0x0600B916 RID: 47382 RVA: 0x0054608E File Offset: 0x0054428E
		public static void OnWorldDataReady()
		{
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 1, 36, ulong.MaxValue, uint.MaxValue);
			SingletonObject.getInstance<BasicGameData>().OnWorldDataReady();
			SingletonObject.getInstance<AsyncMethodDispatcher>().OnWorldDataReady();
		}

		// Token: 0x0600B917 RID: 47383 RVA: 0x005460B8 File Offset: 0x005442B8
		public static void OnLeaveWorld()
		{
			GlobalOperations.ReMonitorGlobalData();
			GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 1, 36, ulong.MaxValue, uint.MaxValue);
			SingletonObject.getInstance<BasicGameData>().OnLeaveWorld();
			SingletonObject.getInstance<AsyncMethodDispatcher>().OnLeaveWorld();
		}

		// Token: 0x0600B918 RID: 47384 RVA: 0x005460E8 File Offset: 0x005442E8
		public static void LoadEndSaving(sbyte worldIndex)
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalDomainMethod.Call.LoadEnding(worldIndex);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B919 RID: 47385 RVA: 0x00546108 File Offset: 0x00544308
		public static void EnterInGameGuideWorld()
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalDomainMethod.Call.EnterInGameGuideWorld(0);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B91A RID: 47386 RVA: 0x00546128 File Offset: 0x00544328
		public static void ExitInGameGuideWorld()
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalDomainMethod.Call.ExitInGameGuideWorld();
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B91B RID: 47387 RVA: 0x00546147 File Offset: 0x00544347
		public static void EnterTutorialWorld(short templateId)
		{
			GlobalOperations.EnterNewWorld(0);
			GlobalDomainMethod.Call.EnterTutorialWorld(templateId);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B91C RID: 47388 RVA: 0x00546168 File Offset: 0x00544368
		private static void ReMonitorGlobalData()
		{
			GlobalOperations.InscribedCharacters.Clear();
			GlobalOperations.InscribedCharacterPinOrders.Clear();
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 6, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 3, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 7, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B91D RID: 47389 RVA: 0x005461BC File Offset: 0x005443BC
		public static void EnterNewWorld(sbyte archiveId)
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalDomainMethod.Call.EnterNewWorld(archiveId);
			GlobalOperations.SetCanResetWorldSettings(false);
		}

		// Token: 0x0600B91E RID: 47390 RVA: 0x005461D3 File Offset: 0x005443D3
		public static void LoadWorld(sbyte archiveId, long backupTimestamp = -1L)
		{
			GlobalOperations.LoadedAllArchiveData = false;
			GlobalDomainMethod.Call.LoadWorld(archiveId, backupTimestamp);
			GameDataBridge.AddDataMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
		}

		// Token: 0x0600B91F RID: 47391 RVA: 0x005461F4 File Offset: 0x005443F4
		public static void SaveWorld()
		{
			GlobalDomainMethod.Call.SaveWorld();
		}

		// Token: 0x0600B920 RID: 47392 RVA: 0x005461FD File Offset: 0x005443FD
		public static void LeaveWorld()
		{
			GlobalDomainMethod.Call.LeaveWorld();
			GlobalOperations.LoadedAllArchiveData = false;
		}

		// Token: 0x0600B921 RID: 47393 RVA: 0x0054620C File Offset: 0x0054440C
		public static void PackCrossArchiveGameData()
		{
			GlobalDomainMethod.Call.PackAllCrossArchiveGameData();
		}

		// Token: 0x0600B922 RID: 47394 RVA: 0x00546215 File Offset: 0x00544415
		public static void GetArchivesInfo()
		{
			GlobalDomainMethod.Call.GetArchivesInfo(GlobalOperations._listenerId);
		}

		// Token: 0x0600B923 RID: 47395 RVA: 0x00546223 File Offset: 0x00544423
		public static void DeleteArchive(sbyte archiveId)
		{
			GlobalDomainMethod.Call.DeleteArchive(archiveId);
		}

		// Token: 0x0600B924 RID: 47396 RVA: 0x0054622D File Offset: 0x0054442D
		public static void SetCanResetWorldSettings(bool newState)
		{
			GlobalOperations.CanResetWorldSettings = newState;
			GEvent.OnEvent(EEvents.ResetWorldSettingsStateChanged, null);
		}

		// Token: 0x0600B925 RID: 47397 RVA: 0x00546248 File Offset: 0x00544448
		private static void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 0;
						if (flag)
						{
							GlobalOperations.GetMethodReturnValue(notification.MethodId, notification.ValueOffset, wrapper.DataPool);
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag2 = uid.DomainId == 0;
					if (flag2)
					{
						GlobalOperations.UpdateGlobalDomainData(uid, notification.ValueOffset, wrapper.DataPool);
					}
					else
					{
						bool flag3 = uid.DomainId == 1;
						if (flag3)
						{
							GlobalOperations.UpdateWorldDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						}
					}
				}
			}
		}

		// Token: 0x0600B926 RID: 47398 RVA: 0x00546338 File Offset: 0x00544538
		private static void UpdateGlobalDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
		{
			switch (uid.DataId)
			{
			case 1:
			{
				Serializer.Deserialize(dataPool, valueOffset, ref GlobalOperations.LoadedAllArchiveData);
				bool flag = !GlobalOperations.LoadedAllArchiveData;
				if (!flag)
				{
					GameDataBridge.AddDataUnMonitor(GlobalOperations._listenerId, 0, 1, ulong.MaxValue, uint.MaxValue);
					GlobalOperations.OnWorldDataReady();
				}
				break;
			}
			case 3:
				Serializer.DeserializeModifications<InscribedCharacterKey>(dataPool, valueOffset, GlobalOperations.InscribedCharacters);
				GEvent.OnEvent(EEvents.InscriptionChange, null);
				break;
			case 6:
				Serializer.Deserialize(dataPool, valueOffset, ref GlobalOperations.CurrGameWorldType);
				break;
			case 7:
				Serializer.DeserializeModifications<InscribedCharacterKey>(dataPool, valueOffset, GlobalOperations.InscribedCharacterPinOrders);
				GEvent.OnEvent(EEvents.InscriptionChange, null);
				break;
			}
		}

		// Token: 0x0600B927 RID: 47399 RVA: 0x005463F8 File Offset: 0x005445F8
		private static void UpdateWorldDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
		{
			ushort dataId = uid.DataId;
			ushort num = dataId;
			if (num == 36)
			{
				bool flag = false;
				Serializer.Deserialize(dataPool, valueOffset, ref flag);
				bool flag2 = !GlobalOperations.LoadedAllArchiveData;
				if (!flag2)
				{
					GlobalOperations.SetCanResetWorldSettings(flag);
				}
			}
		}

		// Token: 0x0600B928 RID: 47400 RVA: 0x0054643C File Offset: 0x0054463C
		private static void GetMethodReturnValue(ushort methodId, int valueOffset, RawDataPool dataPool)
		{
			if (methodId == 5)
			{
				GameApp.ClockAndLogInfo("Get GetArchivesInfo & Start Deserialize ArchiveInfos", false);
				SerializerHolder<ArchiveInfo[]>.Deserialize(dataPool, valueOffset, ref GlobalOperations.ArchivesInfo);
				GameApp.ClockAndLogInfo("Get GetArchivesInfo & End Deserialize ArchiveInfos", false);
				GEvent.OnEvent(EEvents.ArchivesInfoReady, null);
			}
		}

		// Token: 0x04008F70 RID: 36720
		public const sbyte ArchiveSlotsCount = 5;

		// Token: 0x04008F71 RID: 36721
		public static bool LoadedAllArchiveData;

		// Token: 0x04008F72 RID: 36722
		public static ArchiveInfo[] ArchivesInfo;

		// Token: 0x04008F74 RID: 36724
		public static Dictionary<InscribedCharacterKey, InscribedCharacter> InscribedCharacters;

		// Token: 0x04008F75 RID: 36725
		public static Dictionary<InscribedCharacterKey, int> InscribedCharacterPinOrders;

		// Token: 0x04008F76 RID: 36726
		public static sbyte CurrGameWorldType = 0;

		// Token: 0x04008F77 RID: 36727
		private static int _listenerId = -1;
	}
}

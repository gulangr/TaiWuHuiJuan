using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.GameLineScroll;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Story.MainStory;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class BasicGameData : IDisplay
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x060008BB RID: 2235 RVA: 0x0003B9BE File Offset: 0x00039BBE
	// (set) Token: 0x060008BC RID: 2236 RVA: 0x0003B9C6 File Offset: 0x00039BC6
	public bool HideAllTeammates { get; private set; }

	// Token: 0x060008BD RID: 2237 RVA: 0x0003B9D0 File Offset: 0x00039BD0
	public BasicGameData()
	{
		this.CustomTexts = new Dictionary<int, string>();
		this._listenerId = -1;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0003BA3C File Offset: 0x00039C3C
	public void OnWorldDataReady()
	{
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		this._beatRanChenZiInited = false;
		GameDataBridge.AddDataMonitor(this._listenerId, 0, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 0, 8, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 26, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 44, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 57, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 58, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 12, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 13, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 14, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 51, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 15, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 16, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 17, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 18, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 19, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 20, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 21, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 7, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 28, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 4, 52, ulong.MaxValue, uint.MaxValue);
		for (sbyte i = 0; i < 9; i += 1)
		{
			GameDataBridge.AddDataMonitor(this._listenerId, 1, 2, (ulong)((long)i), uint.MaxValue);
			this._xiangshuAvatarTaskStatusArrayInited[(int)i] = false;
		}
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 0, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 55, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 5, 0, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 5, 55, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 18, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 18, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 41, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 40, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 1, 39, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 54, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 75, ulong.MaxValue, uint.MaxValue);
		SingletonObject.getInstance<DisplayTriggerModel>();
		SingletonObject.getInstance<TaskModel>();
		SingletonObject.getInstance<ProfessionModel>();
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0003BD2C File Offset: 0x00039F2C
	public void OnLeaveWorld()
	{
		bool flag = this._listenerId >= 0;
		if (flag)
		{
			GameDataBridge.AddDataUnMonitor(this._listenerId, 0, 2, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 0, 8, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 26, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 44, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 57, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 58, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 12, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 13, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 14, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 51, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 15, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 16, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 17, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 18, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 19, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 20, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 21, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 7, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 28, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 6, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 1, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 52, ulong.MaxValue, uint.MaxValue);
			for (sbyte i = 0; i < 9; i += 1)
			{
				GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 2, (ulong)((long)i), uint.MaxValue);
			}
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 0, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 55, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 5, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 5, 0, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 5, 55, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 3, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 9, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 18, 3, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 18, 4, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 41, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 40, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 1, 39, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 54, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 75, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.UnregisterListener(this._listenerId);
			this._listenerId = -1;
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0003C008 File Offset: 0x0003A208
	public void OnNotifyGameData(List<NotificationWrapper> notifications)
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
				if (num <= 12)
				{
					switch (num)
					{
					case 0:
						this.UpdateGlobalDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						break;
					case 1:
						this.UpdateWorldDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						break;
					case 2:
					case 3:
						break;
					case 4:
						this.UpdateCharacterDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						break;
					case 5:
						this.UpdateTaiwuDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						break;
					default:
						if (num == 12)
						{
							this.UpdateTaiwuEventDomainData(uid, notification.ValueOffset, wrapper.DataPool);
						}
						break;
					}
				}
				else if (num != 18)
				{
					if (num == 19)
					{
						this.UpdateExtraDomainData(uid, notification.ValueOffset, wrapper.DataPool);
					}
				}
				else
				{
					this.UpdateInformationData(uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0003C184 File Offset: 0x0003A384
	private void UpdateInformationData(DataUid uid, int notificationValueOffset, RawDataPool wrapperDataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 3)
		{
			if (num == 4)
			{
				List<NormalInformation> informations = new List<NormalInformation>();
				Serializer.Deserialize(wrapperDataPool, notificationValueOffset, ref informations);
				bool flag = informations.Count > 0;
				if (flag)
				{
					bool flag2 = this.AdvancingMonthState == 0;
					if (flag2)
					{
						NormalInformation information = informations[informations.Count - 1];
						ArgumentBox argBox = new ArgumentBox();
						argBox.Set("Title", LocalStringManager.Get(LanguageKey.LK_GetItem_Information));
						argBox.SetObject("NormalInformation", information);
						UIElement.GetItem.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.GetItem);
					}
				}
			}
		}
		else
		{
			List<int> informationIds = new List<int>();
			Serializer.Deserialize(wrapperDataPool, notificationValueOffset, ref informationIds);
			bool flag3 = informationIds.Count > 0;
			if (flag3)
			{
				bool flag4 = this.AdvancingMonthState == 0;
				if (flag4)
				{
					List<int> information2 = new List<int>();
					information2.Add(informationIds[informationIds.Count - 1]);
					InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackage(null, information2, delegate(int offset, RawDataPool dataPool)
					{
						SecretInformationDisplayPackage displayData = new SecretInformationDisplayPackage();
						Serializer.Deserialize(dataPool, offset, ref displayData);
						ArgumentBox argBox2 = new ArgumentBox();
						argBox2.Set("Title", LocalStringManager.Get(LanguageKey.LK_GetItem_SelectInformation));
						argBox2.SetObject("SecretInformationDisplayPackage", displayData);
						UIElement.GetItem.SetOnInitArgs(argBox2);
						UIManager.Instance.MaskUI(UIElement.GetItem);
					});
				}
			}
		}
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0003C2C0 File Offset: 0x0003A4C0
	private void UpdateGlobalDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 2)
		{
			if (num == 8)
			{
				Dictionary<short, long> data = new Dictionary<short, long>();
				Serializer.DeserializeModifications<short>(dataPool, valueOffset, data);
				foreach (short id in data.Keys)
				{
					this.AchievedAchievements.Add(id);
				}
				bool isAchievementDataInitialized = this._isAchievementDataInitialized;
				if (isAchievementDataInitialized)
				{
					foreach (short id2 in data.Keys)
					{
						this.ToPopupAchievements.Add(id2);
					}
				}
				this._isAchievementDataInitialized = true;
				GEvent.OnEvent(EEvents.AchievementUnlocked, null);
			}
		}
		else
		{
			bool oriValue = this.SavingWorld;
			Serializer.Deserialize(dataPool, valueOffset, ref this.SavingWorld);
			bool flag = this.SavingWorld != oriValue;
			if (flag)
			{
				GEvent.OnEvent(EEvents.OnSavingWorldStateChange, null);
				bool flag2 = GameApp.Instance.GetCurrentGameStateName() != EGameState.Loading;
				if (flag2)
				{
					bool savingWorld = this.SavingWorld;
					if (savingWorld)
					{
						GameApp.ClockAndLogInfo("Start saving", true);
					}
					else
					{
						GameApp.ClockAndLogInfo("End saving", false);
					}
				}
				this.ActionPointConsumed = 0;
			}
		}
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0003C43C File Offset: 0x0003A63C
	private void UpdateWorldDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		BasicGameData.<>c__DisplayClass50_0 CS$<>8__locals1;
		CS$<>8__locals1.uid = uid;
		ushort dataId = CS$<>8__locals1.uid.DataId;
		ushort num = dataId;
		if (num <= 51)
		{
			switch (num)
			{
			case 0:
				Serializer.Deserialize(dataPool, valueOffset, ref this.WorldId);
				break;
			case 1:
				Serializer.Deserialize(dataPool, valueOffset, ref this.XiangshuProgress);
				break;
			case 2:
			{
				XiangshuAvatarTaskStatus status = default(XiangshuAvatarTaskStatus);
				Serializer.Deserialize(dataPool, valueOffset, ref status);
				bool flag = this.XiangshuAvatarTaskStatusArray == null;
				if (flag)
				{
					this.XiangshuAvatarTaskStatusArray = new XiangshuAvatarTaskStatus[9];
				}
				bool flag2;
				checked
				{
					bool showGameLineScroll = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame && this._xiangshuAvatarTaskStatusArrayInited[(int)((IntPtr)CS$<>8__locals1.uid.SubId0)] && this.XiangshuAvatarTaskStatusArray[(int)((IntPtr)CS$<>8__locals1.uid.SubId0)].JuniorXiangshuTaskStatus != status.JuniorXiangshuTaskStatus;
					this.XiangshuAvatarTaskStatusArray[(int)((IntPtr)CS$<>8__locals1.uid.SubId0)] = status;
					this._xiangshuAvatarTaskStatusArrayInited[(int)((IntPtr)CS$<>8__locals1.uid.SubId0)] = true;
					flag2 = showGameLineScroll;
				}
				if (flag2)
				{
					BasicGameData.<UpdateWorldDomainData>g__ShowGameLineScroll|50_0(ref CS$<>8__locals1);
					Game.Views.GameLineScroll.ScrollHelper.ProcessUnlockScrollList((int)CS$<>8__locals1.uid.SubId0, true);
					bool flag3 = status.JuniorXiangshuTaskStatus == 1;
					if (flag3)
					{
						GEvent.OnEvent(UiEvents.ShowUnlockScrollBtnAnim, EasyPool.Get<ArgumentBox>().Set("NeedShowAnim", true));
					}
				}
				break;
			}
			case 3:
				Serializer.Deserialize(dataPool, valueOffset, ref this._xiangshuAvatarTasksInOrder);
				break;
			case 4:
			case 8:
			case 9:
			case 10:
			case 11:
			case 22:
			case 23:
			case 24:
			case 25:
			case 27:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 42:
			case 43:
				break;
			case 5:
				break;
			case 6:
			{
				ulong statusFlag = 0UL;
				Serializer.Deserialize(dataPool, valueOffset, ref statusFlag);
				SingletonObject.getInstance<FunctionLockManager>().UpdateFunctionUnlockStates(statusFlag);
				break;
			}
			case 7:
				Serializer.DeserializeModifications<int>(dataPool, valueOffset, this.CustomTexts);
				break;
			case 12:
				Serializer.Deserialize(dataPool, valueOffset, ref this.WorldPopulationType);
				break;
			case 13:
				Serializer.Deserialize(dataPool, valueOffset, ref this.CharacterLifespanType);
				break;
			case 14:
				Serializer.Deserialize(dataPool, valueOffset, ref this.CombatDifficulty);
				break;
			case 15:
				Serializer.Deserialize(dataPool, valueOffset, ref this.HereticsAmountType);
				break;
			case 16:
				Serializer.Deserialize(dataPool, valueOffset, ref this.BossInvasionSpeedType);
				break;
			case 17:
				Serializer.Deserialize(dataPool, valueOffset, ref this.WorldResourceAmountType);
				break;
			case 18:
				Serializer.Deserialize(dataPool, valueOffset, ref this.AllowRandomTaiwuHeir);
				break;
			case 19:
				Serializer.Deserialize(dataPool, valueOffset, ref this.RestrictOptionsBehaviorType);
				break;
			case 20:
				Serializer.Deserialize(dataPool, valueOffset, ref this.TaiwuVillageStateTemplateId);
				break;
			case 21:
				Serializer.Deserialize(dataPool, valueOffset, ref this.TaiwuVillageLandFormType);
				break;
			case 26:
			{
				int oriValue = this.CurrDate;
				Serializer.Deserialize(dataPool, valueOffset, ref this.CurrDate);
				GEvent.OnEvent(EEvents.OnMonthChange, EasyPool.Get<ArgumentBox>().SetObject("Data", new int[]
				{
					oriValue,
					this.CurrDate
				}));
				break;
			}
			case 28:
				Serializer.Deserialize(dataPool, valueOffset, ref this.AdvancingMonthState);
				this.OnAdvancingMonthState();
				break;
			case 39:
				Serializer.Deserialize(dataPool, valueOffset, ref this.LoopingDifficulty);
				break;
			case 40:
				Serializer.Deserialize(dataPool, valueOffset, ref this.BreakoutDifficulty);
				break;
			case 41:
				Serializer.Deserialize(dataPool, valueOffset, ref this.ReadingDifficulty);
				break;
			case 44:
				Serializer.DeserializeModifications<sbyte>(dataPool, valueOffset, this.StateWeathers);
				GEvent.OnEvent(UiEvents.WeatherChanged, null);
				break;
			default:
				if (num == 51)
				{
					Serializer.Deserialize(dataPool, valueOffset, ref this.ChallengeModeData);
				}
				break;
			}
		}
		else if (num != 55)
		{
			if (num == 57)
			{
				Serializer.DeserializeModifications<short>(dataPool, valueOffset, this.AreaStoryWeathers);
				GEvent.OnEvent(UiEvents.WeatherChanged, null);
			}
		}
		else
		{
			Dictionary<short, sbyte> dictionary = new Dictionary<short, sbyte>();
			Serializer.DeserializeModifications<short>(dataPool, valueOffset, dictionary);
			dictionary = (from pair in dictionary
			where !GuidingChapter.Instance[pair.Key].ObsoleteItem
			select pair).ToDictionary((KeyValuePair<short, sbyte> pair) => pair.Key, (KeyValuePair<short, sbyte> pair) => pair.Value);
			this.TriggeredGuidingChapterChange(dictionary);
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0003C8F8 File Offset: 0x0003AAF8
	private void UpdateTaiwuDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 0)
		{
			if (num == 55)
			{
				Serializer.Deserialize(dataPool2, valueOffset, ref this.MaterialResourceMaxCount);
			}
		}
		else
		{
			int oldTaiwuCharId = this.TaiwuCharId;
			Serializer.Deserialize(dataPool2, valueOffset, ref this.TaiwuCharId);
			GEvent.OnEvent(EEvents.OnTaiwuCharIdChange, EasyPool.Get<ArgumentBox>().Set("OldTaiwuCharId", oldTaiwuCharId).Set("NewTaiwuCharId", this.TaiwuCharId));
			bool flag = SingletonObject.IsCreatedInstance<WorldMapModel>();
			if (flag)
			{
				SingletonObject.getInstance<WorldMapModel>().ResetTaiwuCharId();
			}
			else
			{
				SingletonObject.getInstance<WorldMapModel>();
				SingletonObject.getInstance<AdventureRemakeModel>();
			}
			Debug.LogWarning(string.Format("太吾ID：{0}", this.TaiwuCharId));
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(dataPool, offset, ref displayData);
				this.TaiwuMonasticTitleOrDisplayName = NameCenter.GetMonasticTitleOrDisplayName(displayData, true);
			});
		}
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0003C9D0 File Offset: 0x0003ABD0
	private void UpdateTaiwuEventDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num == 9)
		{
			bool hideState = false;
			Serializer.Deserialize(dataPool, valueOffset, ref hideState);
			bool needNotify = hideState != this.HideAllTeammates;
			this.HideAllTeammates = hideState;
			bool flag = needNotify;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.OnTeammateHideStateChange, null);
			}
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0003CA28 File Offset: 0x0003AC28
	private void UpdateExtraDomainData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 54)
		{
			if (num != 58)
			{
				if (num == 75)
				{
					Serializer.Deserialize(dataPool, valueOffset, ref this.DreamBackUnlockStates);
				}
			}
			else
			{
				int actionPointCurrMonth = 0;
				Serializer.Deserialize(dataPool, valueOffset, ref actionPointCurrMonth);
				this.ActionPointConsumed += Math.Max(0, this.ActionPointCurrMonth - actionPointCurrMonth);
				this.ActionPointCurrMonth = actionPointCurrMonth;
				GEvent.OnEvent(EEvents.OnActionPointChange, null);
			}
		}
		else
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this.IsDreamBack);
		}
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0003CAB4 File Offset: 0x0003ACB4
	private void UpdateCharacterDomainData(DataUid uid, int offset, RawDataPool pool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num == 52)
		{
			Serializer.Deserialize(pool, offset, ref this.TwelveImmortalsCache);
			GEvent.OnEvent(UiEvents.OnMoveTimeCostPercentChanged, null);
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0003CAF3 File Offset: 0x0003ACF3
	public void SetRestrictOptionsBehaviorType(bool restrict)
	{
		GameDataBridge.AddDataModification<bool>(1, 19, ulong.MaxValue, uint.MaxValue, restrict);
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0003CB04 File Offset: 0x0003AD04
	public sbyte GetXiangshuAvatarTaskInOrderIndex(int index)
	{
		bool flag = this._xiangshuAvatarTasksInOrder.CheckIndex(index);
		sbyte result;
		if (flag)
		{
			result = this._xiangshuAvatarTasksInOrder[index];
		}
		else
		{
			result = -1;
		}
		return result;
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0003CB34 File Offset: 0x0003AD34
	private void OnAdvancingMonthState()
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
		if (!flag)
		{
			bool flag2 = this.AdvancingMonthState == 7;
			if (flag2)
			{
				SingletonObject.getInstance<CharacterMonitorModel>().RefreshAllMonitorCharacterAliveState();
			}
			bool flag3 = this.AdvancingMonthState == 14;
			if (flag3)
			{
				bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (inGuiding)
				{
					GEvent.OnEvent(EEvents.OnAdvancingMonthStateChange, null);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						WorldDomainMethod.Call.AdvanceMonth_DisplayedMonthlyNotifications(false);
						GEvent.OnEvent(UiEvents.MonthNotifyProcessComplete, null);
						TaiwuEventDomainMethod.Call.TriggerListener("MonthNotifyShowed", true);
						TaiwuEventDomainMethod.Call.OnNewGameMonth();
					});
					return;
				}
				GlobalDomainMethod.AsyncCall.CheckDriveSpace(null, delegate(int offset, RawDataPool dataPool)
				{
					bool hasSpace = false;
					Serializer.Deserialize(dataPool, offset, ref hasSpace);
					UIManager.Instance.HideUI(UIElement.MonthNotify);
					bool needSave = !SingletonObject.getInstance<GlobalSettings>().SkipSaving && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding && hasSpace;
					UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", needSave));
					UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
				});
			}
			GEvent.OnEvent(EEvents.OnAdvancingMonthStateChange, null);
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0003CBFC File Offset: 0x0003ADFC
	public bool IsDreamBackStateUnlocked(sbyte stateType)
	{
		return BitOperation.GetBit(this.DreamBackUnlockStates, (int)stateType);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0003CC1A File Offset: 0x0003AE1A
	public bool CanShowMoreTogOnWarehouse()
	{
		return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0003CC28 File Offset: 0x0003AE28
	private void TriggeredGuidingChapterChange(Dictionary<short, sbyte> newDictionary)
	{
		List<short> newList = (from pair in newDictionary
		where pair.Value == 0
		select pair.Key).ToList<short>();
		List<short> oldList = this._triggeredGuidingChapterDictionary.Keys.ToList<short>();
		foreach (KeyValuePair<short, sbyte> keyValuePair in newDictionary)
		{
			short num;
			sbyte b;
			keyValuePair.Deconstruct(out num, out b);
			short templateId = num;
			sbyte state = b;
			this._triggeredGuidingChapterDictionary[templateId] = state;
		}
		List<short> changeList = newList.Except(oldList).ToList<short>();
		bool flag = changeList.Count > 0;
		if (flag)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.SetObject("NewTriggeredTemplateIdList", changeList);
			bool flag2 = UIElement.TutorialGuidingChapterTip.Exist || UIElement.TutorialGuidingChapterTip.IsWaitShowing;
			if (flag2)
			{
				GEvent.OnEvent(EEvents.GuidingChapterDataChange, argumentBox);
			}
			else
			{
				bool flag3 = !UIManager.Instance.IsElementActive(UIElement.Loading) && GlobalOperations.CurrGameWorldType == 1;
				if (flag3)
				{
					UIElement.TutorialGuidingChapterTip.SetOnInitArgs(argumentBox);
					UIElement.TutorialGuidingChapterTip.Show();
				}
			}
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0003CD9C File Offset: 0x0003AF9C
	public Dictionary<short, sbyte> GetTriggeredGuidingChapterDictionary()
	{
		return this._triggeredGuidingChapterDictionary;
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0003CDB4 File Offset: 0x0003AFB4
	public sbyte GetTriggeredGuidingChapterState(short templateId)
	{
		return this._triggeredGuidingChapterDictionary.GetValueOrDefault(templateId, 0);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0003CDD4 File Offset: 0x0003AFD4
	public bool HaveAnyTriggeredGuidingChapter()
	{
		return this._triggeredGuidingChapterDictionary.Keys.Any<short>();
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0003CDF8 File Offset: 0x0003AFF8
	public short GetDefaultTriggeredGuidingChapterTemplateId()
	{
		List<short> list = this._triggeredGuidingChapterDictionary.Keys.ToList<short>();
		list.Sort((short a, short b) => a.CompareTo(b));
		return list.FirstOrDefault<short>();
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0003CE48 File Offset: 0x0003B048
	public bool IsXiangshuAvatarTaskStatusGood(int index)
	{
		XiangshuAvatarTaskStatus taskStatus = SingletonObject.getInstance<BasicGameData>().XiangshuAvatarTaskStatusArray.GetOrDefault(index);
		return taskStatus.JuniorXiangshuTaskStatus == 6;
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0003CE78 File Offset: 0x0003B078
	[CompilerGenerated]
	internal static void <UpdateWorldDomainData>g__ShowGameLineScroll|50_0(ref BasicGameData.<>c__DisplayClass50_0 A_0)
	{
		UIElement.GameLineScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("index", (int)A_0.uid.SubId0).Set("targetScrollIndex", 0));
		UIManager.Instance.MaskUI(UIElement.GameLineScroll);
	}

	// Token: 0x04000BCD RID: 3021
	public int CurrDate;

	// Token: 0x04000BCE RID: 3022
	public readonly Dictionary<sbyte, sbyte> StateWeathers = new Dictionary<sbyte, sbyte>();

	// Token: 0x04000BCF RID: 3023
	public readonly Dictionary<short, sbyte> AreaStoryWeathers = new Dictionary<short, sbyte>();

	// Token: 0x04000BD0 RID: 3024
	public uint WorldId;

	// Token: 0x04000BD1 RID: 3025
	public int ActionPointConsumed;

	// Token: 0x04000BD2 RID: 3026
	public int ActionPointCurrMonth;

	// Token: 0x04000BD3 RID: 3027
	public byte WorldPopulationType;

	// Token: 0x04000BD4 RID: 3028
	public byte CharacterLifespanType;

	// Token: 0x04000BD5 RID: 3029
	public byte CombatDifficulty;

	// Token: 0x04000BD6 RID: 3030
	public ChallengeModeData ChallengeModeData;

	// Token: 0x04000BD7 RID: 3031
	public byte ReadingDifficulty;

	// Token: 0x04000BD8 RID: 3032
	public byte BreakoutDifficulty;

	// Token: 0x04000BD9 RID: 3033
	public byte LoopingDifficulty;

	// Token: 0x04000BDA RID: 3034
	public byte HereticsAmountType;

	// Token: 0x04000BDB RID: 3035
	public byte BossInvasionSpeedType;

	// Token: 0x04000BDC RID: 3036
	public byte WorldResourceAmountType;

	// Token: 0x04000BDD RID: 3037
	public bool AllowRandomTaiwuHeir;

	// Token: 0x04000BDE RID: 3038
	public bool RestrictOptionsBehaviorType;

	// Token: 0x04000BDF RID: 3039
	public sbyte TaiwuVillageStateTemplateId;

	// Token: 0x04000BE0 RID: 3040
	public sbyte TaiwuVillageLandFormType;

	// Token: 0x04000BE1 RID: 3041
	public readonly Dictionary<int, string> CustomTexts;

	// Token: 0x04000BE2 RID: 3042
	public sbyte AdvancingMonthState;

	// Token: 0x04000BE3 RID: 3043
	public bool SavingWorld;

	// Token: 0x04000BE4 RID: 3044
	public int TaiwuCharId;

	// Token: 0x04000BE5 RID: 3045
	public string TaiwuMonasticTitleOrDisplayName;

	// Token: 0x04000BE6 RID: 3046
	public sbyte XiangshuProgress;

	// Token: 0x04000BE7 RID: 3047
	public XiangshuAvatarTaskStatus[] XiangshuAvatarTaskStatusArray;

	// Token: 0x04000BE8 RID: 3048
	private bool[] _xiangshuAvatarTaskStatusArrayInited = new bool[9];

	// Token: 0x04000BE9 RID: 3049
	public TwelveImmortalsCacheData TwelveImmortalsCache;

	// Token: 0x04000BEA RID: 3050
	[Obsolete]
	public short MainStoryLineProgress;

	// Token: 0x04000BEB RID: 3051
	public bool BeatRanChenZi;

	// Token: 0x04000BEC RID: 3052
	private bool _beatRanChenZiInited;

	// Token: 0x04000BED RID: 3053
	private sbyte[] _xiangshuAvatarTasksInOrder;

	// Token: 0x04000BEF RID: 3055
	public int MaterialResourceMaxCount;

	// Token: 0x04000BF0 RID: 3056
	public bool IsDreamBack;

	// Token: 0x04000BF1 RID: 3057
	public ulong DreamBackUnlockStates;

	// Token: 0x04000BF2 RID: 3058
	public HashSet<short> AchievedAchievements = new HashSet<short>();

	// Token: 0x04000BF3 RID: 3059
	public HashSet<short> ToPopupAchievements = new HashSet<short>();

	// Token: 0x04000BF4 RID: 3060
	private bool _isAchievementDataInitialized;

	// Token: 0x04000BF5 RID: 3061
	private int _listenerId;

	// Token: 0x04000BF6 RID: 3062
	private readonly Dictionary<short, sbyte> _triggeredGuidingChapterDictionary = new Dictionary<short, sbyte>();
}

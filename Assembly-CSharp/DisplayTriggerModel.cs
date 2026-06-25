using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Taiwu;
using GameData.Domains.World.Notification;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UILogic.DisplayDataStructure;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class DisplayTriggerModel : ISingletonInit, IDisposable
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000C56 RID: 3158 RVA: 0x00050897 File Offset: 0x0004EA97
	public CharacterSet GroupCharIds
	{
		get
		{
			return this._groupCharIds;
		}
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C57 RID: 3159 RVA: 0x0005089F File Offset: 0x0004EA9F
	// (set) Token: 0x06000C58 RID: 3160 RVA: 0x000508A7 File Offset: 0x0004EAA7
	public bool HealingOuterRestriction { get; private set; }

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000C59 RID: 3161 RVA: 0x000508B0 File Offset: 0x0004EAB0
	public byte LegacyPassingState
	{
		get
		{
			return this._legacyPassingState;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000C5A RID: 3162 RVA: 0x000508B8 File Offset: 0x0004EAB8
	// (set) Token: 0x06000C5B RID: 3163 RVA: 0x000508C0 File Offset: 0x0004EAC0
	public bool HealingInnerRestriction { get; private set; }

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000C5C RID: 3164 RVA: 0x000508C9 File Offset: 0x0004EAC9
	// (set) Token: 0x06000C5D RID: 3165 RVA: 0x000508D1 File Offset: 0x0004EAD1
	public bool HandlingMonthlyEventBlock { get; private set; }

	// Token: 0x06000C5E RID: 3166 RVA: 0x000508DC File Offset: 0x0004EADC
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 37, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 31, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 12, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 1, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 48, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 49, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 50, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 1, 10, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 46, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x000509BC File Offset: 0x0004EBBC
	public void Init()
	{
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 31, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 1, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 37, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 12, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 48, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 49, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 50, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 1, 10, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 46, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 5, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x00050AA8 File Offset: 0x0004ECA8
	public bool IsNeiliAllocationTypeRestricted(byte allocationType)
	{
		return (this._neiliAllocationTypeRestriction >> (int)allocationType & 1) == 1;
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x00050ACA File Offset: 0x0004ECCA
	public bool IsReachGameEnd()
	{
		return false;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x00050AD0 File Offset: 0x0004ECD0
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
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
					this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				this.HandleData(uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x00050B7C File Offset: 0x0004ED7C
	private void HandleData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort domainId = uid.DomainId;
		ushort num = domainId;
		if (num != 1)
		{
			if (num != 5)
			{
				if (num == 12)
				{
					bool flag = uid.DataId == 2;
					if (flag)
					{
						Serializer.Deserialize(dataPool, valueOffset, ref this._cgName);
						bool flag2 = !string.IsNullOrEmpty(this._cgName);
						if (flag2)
						{
							GameApp.PlayCg(this._cgName, null);
						}
					}
				}
			}
			else
			{
				ushort dataId = uid.DataId;
				ushort num2 = dataId;
				if (num2 <= 6)
				{
					if (num2 != 5)
					{
						if (num2 == 6)
						{
							List<CharacterDisplayData> receivedCharacters = null;
							Serializer.Deserialize(dataPool, valueOffset, ref receivedCharacters);
							bool flag3 = receivedCharacters == null;
							if (!flag3)
							{
								GameDataBridge.AddDataModification<List<CharacterDisplayData>>(5, 5, ulong.MaxValue, uint.MaxValue, null);
							}
						}
					}
					else
					{
						List<ItemDisplayData> receivedItems = null;
						Serializer.Deserialize(dataPool, valueOffset, ref receivedItems);
						bool flag4 = receivedItems == null;
						if (!flag4)
						{
							GameDataBridge.AddDataModification<List<ItemDisplayData>>(5, 5, ulong.MaxValue, uint.MaxValue, null);
						}
					}
				}
				else if (num2 != 31)
				{
					if (num2 != 37)
					{
						switch (num2)
						{
						case 48:
						{
							bool healingRestriction = false;
							Serializer.Deserialize(dataPool, valueOffset, ref healingRestriction);
							this.HealingOuterRestriction = healingRestriction;
							GEvent.OnEvent(UiEvents.OnHealingOuterRestrictionUpdate, null);
							break;
						}
						case 49:
						{
							bool healingRestriction2 = false;
							Serializer.Deserialize(dataPool, valueOffset, ref healingRestriction2);
							this.HealingInnerRestriction = healingRestriction2;
							GEvent.OnEvent(UiEvents.OnHealingInnerRestrictionUpdate, null);
							break;
						}
						case 50:
							Serializer.Deserialize(dataPool, valueOffset, ref this._neiliAllocationTypeRestriction);
							GEvent.OnEvent(UiEvents.OnNeiliAllocationTypeRestrictionUpdate, null);
							break;
						}
					}
					else
					{
						Serializer.Deserialize(dataPool, valueOffset, ref this._legacyPassingState);
						bool flag5 = this._legacyPassingState == 1 || this._legacyPassingState == 2;
						if (flag5)
						{
							UIManager.Instance.HideUI(UIElement.EventWindow);
							SingletonObject.getInstance<AdventureModel>().SetPlayState(AdventureModel.EPlayState.End);
							SingletonObject.getInstance<AdventureModel>().SetPlayStateIsLocked(true);
							TaiwuDomainMethod.Call.FindSuccessorCandidates(this._gameDataListenerId, true);
						}
						else
						{
							bool flag6 = this._legacyPassingState == 3;
							if (!flag6)
							{
								bool flag7 = this._legacyPassingState == 4;
								if (flag7)
								{
									UIManager.Instance.HideUI(UIElement.SucceedingSelect);
									UIManager.Instance.HideUI(UIElement.SelectRandomSuccessor);
								}
							}
						}
					}
				}
				else
				{
					Serializer.Deserialize(dataPool, valueOffset, ref this._groupCharIds);
				}
			}
		}
		else
		{
			ushort dataId2 = uid.DataId;
			ushort num3 = dataId2;
			if (num3 != 9)
			{
				if (num3 == 10)
				{
					bool onHandlingMonthlyEventBlock = false;
					Serializer.Deserialize(dataPool, valueOffset, ref onHandlingMonthlyEventBlock);
					this.HandlingMonthlyEventBlock = onHandlingMonthlyEventBlock;
					sbyte advancingMonthState = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState;
					bool flag8 = !onHandlingMonthlyEventBlock && GameApp.Instance.GetCurrentGameStateName() != EGameState.Loading && advancingMonthState == 14;
					if (flag8)
					{
						GEvent.OnEvent(UiEvents.OnHandlingMonthlyEventBlockChange, null);
					}
				}
			}
			else
			{
				bool initFlag = this.InstantNotifications != null;
				bool flag9 = !initFlag;
				if (flag9)
				{
					this.InstantNotifications = new InstantNotificationCollection();
				}
				List<ValueTuple<int, int>> newDataInfos = EasyPool.Get<List<ValueTuple<int, int>>>();
				Serializer.DeserializeModifications(dataPool, valueOffset, this.InstantNotifications, newDataInfos);
				bool flag10 = initFlag;
				if (flag10)
				{
					this.Handle_NewInstantNotifications(newDataInfos);
				}
			}
		}
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x00050E8C File Offset: 0x0004F08C
	private void HandleMethodReturn(ushort domainId, ushort methodId, int valueOffset, RawDataPool dataPool)
	{
		if (domainId != 5)
		{
			if (domainId == 13)
			{
				if (methodId == 5)
				{
					ArgumentCollectionRenderArguments collectionRenderArguments = null;
					Serializer.Deserialize(dataPool, valueOffset, ref collectionRenderArguments);
					string frameKey = collectionRenderArguments.Key;
					string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
					string argumentCollectionKey = frameKey + "_ArgumentCollection";
					ArgumentCollection argumentCollection;
					this._renderInstantNotificationHelperBox.Get<ArgumentCollection>(argumentCollectionKey, out argumentCollection);
					RenderedArgumentCollection renderedArgumentCollection;
					this._renderInstantNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection);
					bool flag = argumentCollection != null && renderedArgumentCollection != null;
					if (flag)
					{
						GameMessageUtils.RenderDynamicArguments(collectionRenderArguments, argumentCollection, renderedArgumentCollection, false, false);
						this.RenderInstantNotification(frameKey);
					}
				}
			}
		}
		else
		{
			bool flag2 = methodId == 26;
			if (flag2)
			{
				this.Handle_FindSuccessorCandidates(valueOffset, dataPool);
			}
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00050F48 File Offset: 0x0004F148
	public void Handle_FindSuccessorCandidates(int valueOffset, RawDataPool dataPool)
	{
		List<SuccessorCandidates> candidates = new List<SuccessorCandidates>();
		Serializer.Deserialize(dataPool, valueOffset, ref candidates);
		bool flag = this._legacyPassingState == 1;
		if (flag)
		{
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
			config.Title = LanguageKey.LK_SucceedingSelect_Title.Tr();
			config.InteractionMode = ESelectCharacterInteractionMode.Instant;
			config.SelectionMode = ESelectCharacterSelectionMode.Single;
			config.BannedCharacterIds = (from x in candidates
			where !GameData.Domains.Character.SharedMethods.CanBeTaiwu(x.TeammateData.CharacterTemplateId) || x.TeammateData.PhysiologicalAge < 16
			select x.TeammateData.CharacterId).ToHashSet<int>();
			config.MinSelectionCount = 1;
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", candidates).SetObject("SelectCharacterCallback", new SelectCharacterCallback(DisplayTriggerModel.<Handle_FindSuccessorCandidates>g__OnSelectChar|32_2)));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
			ArgumentBox args = new ArgumentBox();
			args.SetObject("TargetPageIndex", ECharacterSubToggleBase.CharacterBase);
			GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		}
		else
		{
			bool flag2 = this._legacyPassingState == 2;
			if (flag2)
			{
				UIElement.SelectRandomSuccessor.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Candidates", candidates));
				UIManager.Instance.ShowUI(UIElement.SelectRandomSuccessor, true);
			}
		}
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000510B2 File Offset: 0x0004F2B2
	public static void ShowInheritUILegacy(int charId)
	{
		UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", true).Set("CharacterId", charId));
		UIManager.Instance.MaskUI(UIElement.Legacy);
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x000510EB File Offset: 0x0004F2EB
	private void Handle_BackToMainMenu()
	{
		GameApp.ReturnToMainMenu(null, null, null);
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x000510F8 File Offset: 0x0004F2F8
	private void RenderInstantNotification(string frameKey)
	{
		string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
		string argumentCollectionKey = frameKey + "_ArgumentCollection";
		string cacheDataListKey = frameKey + "_DataList";
		List<InstantNotificationRenderInfo> cacheList;
		this._renderInstantNotificationHelperBox.Get<List<InstantNotificationRenderInfo>>(cacheDataListKey, out cacheList);
		RenderedArgumentCollection renderedArgumentCollection;
		this._renderInstantNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection);
		bool flag = cacheList == null || renderedArgumentCollection == null;
		if (flag)
		{
			GLog.TagError("InstantNotificationError", frameKey + " instant notification render failure ... ", Array.Empty<object>());
		}
		else
		{
			int newMessageIndex = this.RenderedNotificationList.Count;
			int date = SingletonObject.getInstance<BasicGameData>().CurrDate;
			foreach (InstantNotificationRenderInfo renderInfo in cacheList)
			{
				bool merged = false;
				for (int index = newMessageIndex; index < this.RenderedNotificationList.Count; index++)
				{
					NotificationItem item = this.RenderedNotificationList[index];
					bool flag2 = item.TryMerge(date, renderInfo);
					if (flag2)
					{
						merged = true;
						break;
					}
				}
				bool flag3 = merged;
				if (!flag3)
				{
					NotificationItem newItem = new NotificationItem(renderInfo.Date, renderInfo, (short id) => InstantNotification.Instance.GetItem(id).MergeableParameters);
					newItem.RenderedArgumentCollection = renderedArgumentCollection;
					newItem.ReadState = false;
					string _ = newItem.ToString();
					this.RenderedNotificationList.Add(newItem);
				}
			}
			this._renderInstantNotificationHelperBox.SetObject(renderedArgCollectionKey, null);
			this._renderInstantNotificationHelperBox.SetObject(argumentCollectionKey, null);
			this._renderInstantNotificationHelperBox.SetObject(cacheDataListKey, null);
			GEvent.OnEvent(UiEvents.OnNewInstantNotification, EasyPool.Get<ArgumentBox>().Set("NewMessageIndex", newMessageIndex));
		}
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x000512D0 File Offset: 0x0004F4D0
	private void Handle_NewInstantNotifications([TupleElementNames(new string[]
	{
		"offset",
		"size"
	})] List<ValueTuple<int, int>> newDataInfos)
	{
		bool flag = newDataInfos == null || newDataInfos.Count <= 0;
		if (!flag)
		{
			List<InstantNotificationRenderInfo> cacheList = new List<InstantNotificationRenderInfo>();
			ArgumentCollection argumentCollection = new ArgumentCollection();
			int i = 0;
			int count = newDataInfos.Count;
			while (i < count)
			{
				ValueTuple<int, int> valueTuple = newDataInfos[i];
				int offset = valueTuple.Item1;
				int size = valueTuple.Item2;
				int currOffset = offset;
				while (currOffset >= 0 && currOffset < offset + size)
				{
					InstantNotificationRenderInfo renderInfo = this.InstantNotifications.GetRenderInfo(currOffset, argumentCollection);
					cacheList.Add(renderInfo);
					currOffset = this.InstantNotifications.Next(currOffset);
				}
				i++;
			}
			bool flag2 = this.RenderedNotificationList == null;
			if (flag2)
			{
				this.RenderedNotificationList = new List<NotificationItem>();
			}
			string frameKey = string.Format("Frame_{0}", Time.frameCount);
			string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
			string cacheDataListKey = frameKey + "_DataList";
			string argumentCollectionKey = frameKey + "_ArgumentCollection";
			int index = 0;
			RenderedArgumentCollection renderedArgumentCollection2;
			List<InstantNotificationRenderInfo> list;
			ArgumentCollection argumentCollection2;
			while (this._renderInstantNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection2) || this._renderInstantNotificationHelperBox.Get<List<InstantNotificationRenderInfo>>(cacheDataListKey, out list) || this._renderInstantNotificationHelperBox.Get<ArgumentCollection>(cacheDataListKey, out argumentCollection2))
			{
				frameKey = string.Format("Frame_{0}_{1}", Time.frameCount, index++);
				renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
				cacheDataListKey = frameKey + "_DataList";
				argumentCollectionKey = frameKey + "_ArgumentCollection";
			}
			RenderedArgumentCollection renderedArgumentCollection = new RenderedArgumentCollection();
			GameMessageUtils.RenderFixedArguments(argumentCollection, renderedArgumentCollection, false);
			this._renderInstantNotificationHelperBox.SetObject(renderedArgCollectionKey, renderedArgumentCollection);
			this._renderInstantNotificationHelperBox.SetObject(cacheDataListKey, cacheList);
			this._renderInstantNotificationHelperBox.SetObject(argumentCollectionKey, argumentCollection);
			bool flag3 = argumentCollection.Characters.Count <= 0 && argumentCollection.Locations.Count <= 0 && argumentCollection.Settlements.Count <= 0 && argumentCollection.CharacterRealNames.Count <= 0 && argumentCollection.JiaoLoongs.Count <= 0;
			if (flag3)
			{
				this.RenderInstantNotification(frameKey);
			}
			else
			{
				this._charIdList.Clear();
				this._charIdList.AddRange(argumentCollection.Characters);
				this._charIdList.AddRange(argumentCollection.CharacterRealNames);
				RecordArgumentsRequest argRequest = new RecordArgumentsRequest(argumentCollection)
				{
					Characters = this._charIdList
				};
				LifeRecordDomainMethod.Call.GetRecordRenderInfoArguments(this._gameDataListenerId, frameKey, argRequest);
			}
		}
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x00051584 File Offset: 0x0004F784
	[CompilerGenerated]
	internal static void <Handle_FindSuccessorCandidates>g__OnSelectChar|32_2(List<int> chars)
	{
		int selectedCharId = chars[0];
		DisplayTriggerModel.ShowInheritUILegacy(selectedCharId);
	}

	// Token: 0x04000DA4 RID: 3492
	private int _gameDataListenerId = -1;

	// Token: 0x04000DA5 RID: 3493
	private short _mainStorylineProgress;

	// Token: 0x04000DA6 RID: 3494
	private byte _legacyPassingState;

	// Token: 0x04000DA7 RID: 3495
	private string _cgName;

	// Token: 0x04000DA8 RID: 3496
	private CharacterSet _groupCharIds;

	// Token: 0x04000DA9 RID: 3497
	public InstantNotificationCollection InstantNotifications;

	// Token: 0x04000DAA RID: 3498
	public List<NotificationItem> RenderedNotificationList;

	// Token: 0x04000DAB RID: 3499
	private ArgumentBox _renderInstantNotificationHelperBox = new ArgumentBox();

	// Token: 0x04000DAE RID: 3502
	private byte _neiliAllocationTypeRestriction;

	// Token: 0x04000DB0 RID: 3504
	private List<int> _charIdList = new List<int>();
}

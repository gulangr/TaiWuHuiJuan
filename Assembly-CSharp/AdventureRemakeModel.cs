using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FrameWork;
using GameData.Adventure;
using GameData.Common;
using GameData.Domains.Adventure;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class AdventureRemakeModel : ISingletonInit, IDisposable
{
	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x0600089B RID: 2203 RVA: 0x0003ADFD File Offset: 0x00038FFD
	// (set) Token: 0x0600089C RID: 2204 RVA: 0x0003AE04 File Offset: 0x00039004
	public static AdventureCore Core { get; private set; }

	// Token: 0x0600089D RID: 2205 RVA: 0x0003AE0C File Offset: 0x0003900C
	public static string GetExportPath()
	{
		return Path.Combine(Application.streamingAssetsPath, "AdventureCore");
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0003AE1D File Offset: 0x0003901D
	public static void InitializeAdventureCore()
	{
		AdventureExternalBridge.Initialize(new AdventureExternalBridgeImplement());
		AdventureRemakeModel.Core = new AdventureCore();
		AdventureRemakeModel.Core.LoadFrom(AdventureRemakeModel.GetExportPath());
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x0600089F RID: 2207 RVA: 0x0003AE46 File Offset: 0x00039046
	public bool NotInAdventureAndMajorEvent
	{
		get
		{
			return this.AdventureTaiwu.NotInAdventure && this.AdventureMajorEventTaiwu.NotInAdventure;
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0003AE64 File Offset: 0x00039064
	public void Init()
	{
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyAdventureData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 10, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 10, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 10, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 10, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 10, 5, ulong.MaxValue, uint.MaxValue);
		GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0003AEFC File Offset: 0x000390FC
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 10, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 10, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 10, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 10, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 10, 5, ulong.MaxValue, uint.MaxValue);
		GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0003AF7C File Offset: 0x0003917C
	private void OnGameStateChange(ArgumentBox argBox)
	{
		Enum preState;
		argBox.Get("preState", out preState);
		Enum newState;
		argBox.Get("newState", out newState);
		bool flag = (EGameState)preState == EGameState.Loading && (EGameState)newState == EGameState.InGame;
		if (flag)
		{
			this.HandlerAdventureLoaded();
		}
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0003AFC8 File Offset: 0x000391C8
	private void OnNotifyAdventureData(List<NotificationWrapper> notifications)
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
					bool flag = notification.DomainId == 10;
					if (flag)
					{
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag2 = uid.DomainId == 10;
				if (flag2)
				{
					bool flag3 = uid.DataId == 1;
					if (flag3)
					{
						Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this.AdventureRemakeDict);
						GEvent.OnEvent(UiEvents.AdventureRemakeDictChange, null);
					}
					else
					{
						bool flag4 = uid.DataId == 2;
						if (flag4)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.AdventureTaiwu);
							GEvent.OnEvent(UiEvents.OnAdventureTaiwuChanged, null);
						}
						else
						{
							bool flag5 = uid.DataId == 3;
							if (flag5)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.AdventureCacheData);
								GEvent.OnEvent(UiEvents.OnStateAdventureDataReceived, null);
							}
							else
							{
								bool flag6 = uid.DataId == 4;
								if (flag6)
								{
									Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this.AdventureMajorEventDict);
									GEvent.OnEvent(UiEvents.AdventureMajorEventChange, null);
								}
								else
								{
									bool flag7 = uid.DataId == 5;
									if (flag7)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.AdventureMajorEventTaiwu);
										GEvent.OnEvent(UiEvents.OnAdventureMajorEventTaiwuChanged, null);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0003B1B0 File Offset: 0x000393B0
	public AdventureBlockCacheData TryGetLocationAdventureRemake(Location location)
	{
		return this.AdventureCacheData.GetCacheData(location);
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0003B1D0 File Offset: 0x000393D0
	public bool LocationHaveAnyAdventureOrMajorEvent(Location location)
	{
		AdventureBlockCacheData adventureBlockCacheData = this.AdventureCacheData.GetCacheData(location);
		return adventureBlockCacheData != null && adventureBlockCacheData.AnyAdventureOrMajorEvent;
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0003B1FC File Offset: 0x000393FC
	public bool TryGetAny(int runtimeId, out IAdventureRuntime runtime)
	{
		AdventureRuntime adventure;
		bool flag = this.AdventureRemakeDict.TryGetValue(runtimeId, out adventure);
		if (flag)
		{
			runtime = adventure;
		}
		else
		{
			AdventureMajorEvent majorEvent;
			bool flag2 = this.AdventureMajorEventDict.TryGetValue(runtimeId, out majorEvent);
			if (flag2)
			{
				runtime = majorEvent;
			}
			else
			{
				runtime = null;
			}
		}
		return runtime != null;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0003B248 File Offset: 0x00039448
	public bool TryGetAnyByCoreId(int coreId, out IAdventureRuntime runtime)
	{
		Dictionary<int, AdventureRuntime> adventureRemakeDict = this.AdventureRemakeDict;
		bool flag = adventureRemakeDict != null && adventureRemakeDict.Count > 0;
		if (flag)
		{
			foreach (AdventureRuntime adventure in this.AdventureRemakeDict.Values)
			{
				bool flag2 = adventure.CoreId == coreId;
				if (flag2)
				{
					runtime = adventure;
					return true;
				}
			}
		}
		Dictionary<int, AdventureMajorEvent> adventureMajorEventDict = this.AdventureMajorEventDict;
		bool flag3 = adventureMajorEventDict != null && adventureMajorEventDict.Count > 0;
		if (flag3)
		{
			foreach (AdventureMajorEvent majorEvent in this.AdventureMajorEventDict.Values)
			{
				bool flag4 = majorEvent.CoreId == coreId;
				if (flag4)
				{
					runtime = majorEvent;
					return true;
				}
			}
		}
		runtime = null;
		return false;
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0003B360 File Offset: 0x00039560
	public bool SpecialBottomAdventure(int adventureId)
	{
		int coreId = this.AdventureRemakeDict[adventureId].CoreId;
		return coreId == 441546069 || coreId == 428932014 || coreId == 430262065;
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0003B3A8 File Offset: 0x000395A8
	private void HandlerAdventureLoaded()
	{
		bool flag;
		if (this.AdventureTaiwu.InAdventure && !UIManager.Instance.IsElementActive(UIElement.AdventureRemake))
		{
			Dictionary<int, AdventureRuntime> adventureRemakeDict = this.AdventureRemakeDict;
			flag = (adventureRemakeDict != null && adventureRemakeDict.Count > 0);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			bool waitingForLoadAndEnterAdventure = this._waitingForLoadAndEnterAdventure;
			if (!waitingForLoadAndEnterAdventure)
			{
				this._waitingForLoadAndEnterAdventure = true;
				SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.StackToAdventure());
			}
		}
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0003B417 File Offset: 0x00039617
	private IEnumerator StackToAdventure()
	{
		yield return new WaitUntil(() => WorldMapModel.MapBlockUiLoadFinish && WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockRenderFinish);
		int num;
		for (int i = 0; i < 20; i = num + 1)
		{
			bool flag = !WorldMapModel.MapBlockRenderFinish;
			if (flag)
			{
				break;
			}
			yield return null;
			num = i;
		}
		yield return new WaitUntil(() => WorldMapModel.MapBlockRenderFinish);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("AdventureId", this.AdventureTaiwu.AdventureId);
		box.Set("ArchiveEnter", true);
		UIElement.AdventureRemake.SetOnInitArgs(box);
		UIManager.Instance.StackToUI(this.SpecialBottomAdventure(this.AdventureTaiwu.AdventureId) ? UIElement.StateAdventureRemakeSpecialBottom : UIElement.StateAdventureRemake);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
		this._waitingForLoadAndEnterAdventure = false;
		bool flag2 = SingletonObject.getInstance<BasicGameData>().CurrDate > 8;
		if (flag2)
		{
			UIElement monthNotify = UIElement.MonthNotify;
			monthNotify.OnHide = (Action)Delegate.Combine(monthNotify.OnHide, new Action(TaiwuEventDomainMethod.Call.OnRecordEnterGame));
			UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", false));
			UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
		}
		else
		{
			TaiwuEventDomainMethod.Call.OnRecordEnterGame();
		}
		yield break;
	}

	// Token: 0x04000BC0 RID: 3008
	private int _gameDataListenerId = -1;

	// Token: 0x04000BC1 RID: 3009
	public Dictionary<int, AdventureRuntime> AdventureRemakeDict = new Dictionary<int, AdventureRuntime>();

	// Token: 0x04000BC2 RID: 3010
	public Dictionary<int, AdventureMajorEvent> AdventureMajorEventDict = new Dictionary<int, AdventureMajorEvent>();

	// Token: 0x04000BC3 RID: 3011
	public AdventureCacheData AdventureCacheData = new AdventureCacheData();

	// Token: 0x04000BC4 RID: 3012
	public AdventureTaiwu AdventureTaiwu;

	// Token: 0x04000BC5 RID: 3013
	public AdventureMajorEventTaiwu AdventureMajorEventTaiwu;

	// Token: 0x04000BC6 RID: 3014
	private bool _waitingForLoadAndEnterAdventure;
}

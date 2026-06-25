using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.Map;
using GameData.Common;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TutorialChapter;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x02000148 RID: 328
public class TutorialChapterModel : ISingletonInit, IDisposable
{
	// Token: 0x170001EE RID: 494
	// (get) Token: 0x0600119C RID: 4508 RVA: 0x0006AAE3 File Offset: 0x00068CE3
	public int ListenerId
	{
		get
		{
			return this._listenerId;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x0600119D RID: 4509 RVA: 0x0006AAEB File Offset: 0x00068CEB
	public bool InGuiding
	{
		get
		{
			return GlobalOperations.CurrGameWorldType == 2;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x0600119E RID: 4510 RVA: 0x0006AAF5 File Offset: 0x00068CF5
	public bool AdvanceMonthEnable
	{
		get
		{
			return this.GetFunctionStatus(13);
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x0600119F RID: 4511 RVA: 0x0006AAFF File Offset: 0x00068CFF
	public bool MoveEnable
	{
		get
		{
			return this.GetFunctionStatus(16);
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060011A0 RID: 4512 RVA: 0x0006AB09 File Offset: 0x00068D09
	public bool OpenCharacterMenuEnable
	{
		get
		{
			return this.GetFunctionStatus(2);
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x060011A1 RID: 4513 RVA: 0x0006AB12 File Offset: 0x00068D12
	public bool CanShowEnterIndustry
	{
		get
		{
			return this.GetFunctionStatus(0);
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060011A2 RID: 4514 RVA: 0x0006AB1B File Offset: 0x00068D1B
	// (set) Token: 0x060011A3 RID: 4515 RVA: 0x0006AB32 File Offset: 0x00068D32
	public Location ForceNextLocation
	{
		get
		{
			return this.InGuiding ? this._forceNextLocation : Location.Invalid;
		}
		private set
		{
			this._forceNextLocation = value;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060011A4 RID: 4516 RVA: 0x0006AB3B File Offset: 0x00068D3B
	// (set) Token: 0x060011A5 RID: 4517 RVA: 0x0006AB4E File Offset: 0x00068D4E
	public bool NeiliAllocateFitChapter7
	{
		get
		{
			return this.InGuiding && this._neiliAllocateFitChapter7;
		}
		private set
		{
			this._neiliAllocateFitChapter7 = value;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060011A6 RID: 4518 RVA: 0x0006AB57 File Offset: 0x00068D57
	// (set) Token: 0x060011A7 RID: 4519 RVA: 0x0006AB6A File Offset: 0x00068D6A
	public bool HuanxinSurprised
	{
		get
		{
			return !this.InGuiding || this._huanxinSurprised;
		}
		private set
		{
			this._huanxinSurprised = value;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060011A8 RID: 4520 RVA: 0x0006AB73 File Offset: 0x00068D73
	// (set) Token: 0x060011A9 RID: 4521 RVA: 0x0006AB86 File Offset: 0x00068D86
	public bool HuanxinDying
	{
		get
		{
			return !this.InGuiding || this._huanxinDying;
		}
		private set
		{
			this._huanxinDying = value;
		}
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0006AB90 File Offset: 0x00068D90
	public void Init()
	{
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 7, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 15, 6, ulong.MaxValue, uint.MaxValue);
		this.BambooHouseStartBuildFlag = false;
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0006AC50 File Offset: 0x00068E50
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 7, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 15, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._listenerId);
		this._listenerId = -1;
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0006AD04 File Offset: 0x00068F04
	public bool IsInTutorialChapter(int index)
	{
		return this.InGuiding && (int)this.TutorialChapterIndex == index;
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0006AD2A File Offset: 0x00068F2A
	public bool GetFunctionStatus(short tutorialFunctionType)
	{
		return !this.InGuiding || (this._functionStatuses.Length > (int)tutorialFunctionType && this._functionStatuses[(int)tutorialFunctionType]);
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0006AD54 File Offset: 0x00068F54
	public bool CanAdvanceForChapter2()
	{
		bool flag = !this.AdvanceMonthEnable;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.TutorialChapterIndex == 1;
			if (flag2)
			{
				bool flag3 = !this.BambooHouseStartBuildFlag;
				if (flag3)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_GameName),
						Content = LocalStringManager.Get(LanguageKey.LK_TutorialChapter2_NeedStartBuildBambooHouse_Tips),
						Type = 2
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				result = this.BambooHouseStartBuildFlag;
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0006ADFC File Offset: 0x00068FFC
	public bool CanAdvanceForChapter5(short loopNeigongId)
	{
		bool flag = !this.AdvanceMonthEnable;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.TutorialChapterIndex == 4;
			if (flag2)
			{
				bool isLoopingPeiranjue = loopNeigongId == 0;
				TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("TutorialChaptersTryAdvanceMonth", "LoopingPeiranjue", isLoopingPeiranjue);
				TaiwuEventDomainMethod.Call.TriggerListener("TutorialChaptersTryAdvanceMonth", true);
				result = isLoopingPeiranjue;
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0006AE54 File Offset: 0x00069054
	public bool CanAdvanceForChapter6(ItemKey itemKey)
	{
		bool flag = !this.AdvanceMonthEnable;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.TutorialChapterIndex == 5;
			if (flag2)
			{
				bool flag3 = !itemKey.IsValid() || itemKey.ItemType != 10 || itemKey.TemplateId != 878;
				if (flag3)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_GameName),
						Content = LocalStringManager.Get(LanguageKey.LK_TutorialChapter3_NeedReadFirst_Tips),
						Type = 2
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x0006AF16 File Offset: 0x00069116
	public void UpdateForceNextLocation()
	{
		TutorialChapterDomainMethod.Call.GetNextForceMoveToLocation(this._listenerId);
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x0006AF28 File Offset: 0x00069128
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
					bool flag = notification.DomainId == 15 && notification.MethodId == 1;
					if (flag)
					{
						Location location = Location.Invalid;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref location);
						this.ForceNextLocation = location;
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag2 = uid.DomainId == 15;
				if (flag2)
				{
					this.UpdateGameData(uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x0006B010 File Offset: 0x00069210
	private void UpdateGameData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		switch (uid.DataId)
		{
		case 1:
			Serializer.Deserialize(dataPool, valueOffset, ref this.TutorialChapterIndex);
			GEvent.OnEvent(UiEvents.UpdateMapSettlementBtn, null);
			break;
		case 2:
		{
			string tutorialVideoName = string.Empty;
			Serializer.Deserialize(dataPool, valueOffset, ref tutorialVideoName);
			bool flag4 = !string.IsNullOrEmpty(tutorialVideoName);
			if (flag4)
			{
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("TutorialVideoPathName", tutorialVideoName);
				bool exist = UIElement.TutorialVideoPlayer.Exist;
				if (exist)
				{
					GEvent.OnEvent(UiEvents.RefreshVideo, argumentBox);
				}
				else
				{
					UIElement.TutorialVideoPlayer.SetOnInitArgs(argumentBox);
					UIManager.Instance.ShowUI(UIElement.TutorialVideoPlayer, true);
				}
			}
			break;
		}
		case 3:
		{
			Location location = Location.Invalid;
			Serializer.Deserialize(dataPool, valueOffset, ref location);
			this.ForceNextLocation = location;
			break;
		}
		case 4:
		{
			bool flag = false;
			Serializer.Deserialize(dataPool, valueOffset, ref flag);
			this.NeiliAllocateFitChapter7 = flag;
			break;
		}
		case 5:
		{
			bool flag2 = false;
			Serializer.Deserialize(dataPool, valueOffset, ref flag2);
			this.HuanxinDying = flag2;
			break;
		}
		case 6:
		{
			bool flag3 = false;
			Serializer.Deserialize(dataPool, valueOffset, ref flag3);
			this.HuanxinSurprised = flag3;
			break;
		}
		case 7:
			Serializer.Deserialize(dataPool, valueOffset, ref this._functionStatuses);
			this.OnFunctionStatusChanged();
			break;
		case 9:
		{
			short tutorialVideoTemplateId = 0;
			Serializer.Deserialize(dataPool, valueOffset, ref tutorialVideoTemplateId);
			bool flag5 = tutorialVideoTemplateId >= 0;
			if (flag5)
			{
				ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>().Set("TutorialVideoTemplateId", tutorialVideoTemplateId);
				bool exist2 = UIElement.TutorialVideoPlayer.Exist;
				if (exist2)
				{
					GEvent.OnEvent(UiEvents.RefreshVideo, argumentBox2);
				}
				else
				{
					UIElement.TutorialVideoPlayer.SetOnInitArgs(argumentBox2);
					UIManager.Instance.ShowUI(UIElement.TutorialVideoPlayer, true);
				}
			}
			break;
		}
		}
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0006B1EC File Offset: 0x000693EC
	private void OnFunctionStatusChanged()
	{
		bool flag = !this.InGuiding;
		if (!flag)
		{
			GEvent.OnEvent(EEvents.OnTutorialFunctionStatusChange, null);
			bool exist = UIElement.WorldMap.Exist;
			if (exist)
			{
				ViewWorldMap.SetDisableMoving(!this.MoveEnable);
			}
			GEvent.OnEvent(UiEvents.UpdateMapSettlementBtn, null);
		}
	}

	// Token: 0x04000F2B RID: 3883
	private int _listenerId = -1;

	// Token: 0x04000F2C RID: 3884
	public short TutorialChapterIndex;

	// Token: 0x04000F2D RID: 3885
	private bool _advanceMonthEnable;

	// Token: 0x04000F2E RID: 3886
	private Location _forceNextLocation = Location.Invalid;

	// Token: 0x04000F2F RID: 3887
	private bool _neiliAllocateFitChapter7 = false;

	// Token: 0x04000F30 RID: 3888
	private bool _huanxinSurprised;

	// Token: 0x04000F31 RID: 3889
	private bool _huanxinDying;

	// Token: 0x04000F32 RID: 3890
	public bool WaitOpenCharacterNeili;

	// Token: 0x04000F33 RID: 3891
	public bool BambooHouseStartBuildFlag = false;

	// Token: 0x04000F34 RID: 3892
	private BoolArray _functionStatuses = new BoolArray();
}

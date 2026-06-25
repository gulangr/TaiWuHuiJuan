using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.ListStyleGeneralScroll;
using Game.Views;
using Game.Views.CharacterMenu;
using Game.Views.Exchange;
using Game.Views.Map;
using Game.Views.Profession;
using Game.Views.SectInteract;
using Game.Views.Select;
using Game.Views.SettlementPrison;
using GameData.Domains.Adventure;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;

// Token: 0x02000126 RID: 294
public static class DisplayEventHandler
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x0004B520 File Offset: 0x00049720
	public static void Initialize()
	{
		GameDataBridge.RegisterDisplayEventHandler(new GameDataBridge.NotificationHandler(DisplayEventHandler.HandleDisplayEvents));
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0004B538 File Offset: 0x00049738
	private static void HandleDisplayEvents(List<NotificationWrapper> notifications)
	{
		int i = 0;
		int count = notifications.Count;
		while (i < count)
		{
			NotificationWrapper wrapper = notifications[i];
			Notification notification = wrapper.Notification;
			int argsOffset = notification.ValueOffset;
			RawDataPool dataPool = wrapper.DataPool;
			bool flag = notification.Type != 2;
			if (flag)
			{
				throw new Exception(string.Format("Unsupported notification type: {0}", notification.Type));
			}
			switch (notification.DisplayEventType)
			{
			case 0:
				DisplayEventHandler.HandleDisplayEvent_OpenCatchCricket();
				break;
			case 1:
				DisplayEventHandler.HandleDisplayEvent_ExitAdventure();
				break;
			case 2:
				DisplayEventHandler.HandleDisplayEvent_OpenCricketBattle(dataPool, argsOffset);
				break;
			case 3:
				DisplayEventHandler.HandleDisplayEvent_OpenCricketBattleWithConfig(dataPool, argsOffset);
				break;
			case 4:
				DisplayEventHandler.HandleDisplayEvent_OpenShop(dataPool, argsOffset);
				break;
			case 5:
				DisplayEventHandler.HandleDisplayEvent_OpenBookTrade(dataPool, argsOffset);
				break;
			case 6:
				DisplayEventHandler.HandleDisplayEvent_StopAutoStartCombat();
				break;
			case 7:
				DisplayEventHandler.HandleDisplayEvent_StartCombat(dataPool, argsOffset);
				break;
			case 8:
				DisplayEventHandler.HandleDisplayEvent_ChangeTeammateCommandOnCombatBegin(dataPool, argsOffset);
				break;
			case 9:
				DisplayEventHandler.HandleDisplayEvent_CombatShowCommand(dataPool, argsOffset);
				break;
			case 10:
				DisplayEventHandler.HandleDisplayEvent_CombatShowSpecialEffect(dataPool, argsOffset);
				break;
			case 11:
				DisplayEventHandler.HandleDisplayEvent_CombatShowSurrenderMark(dataPool, argsOffset);
				break;
			case 12:
				DisplayEventHandler.HandleDisplayEvent_CombatShowFleeAnimation(dataPool, argsOffset);
				break;
			case 13:
				DisplayEventHandler.HandleDisplayEvent_CombatShowAbsorbNeiliAllocation(dataPool, argsOffset);
				break;
			case 14:
				DisplayEventHandler.HandleDisplayEvent_CombatShowTianSuiBaoLu(dataPool, argsOffset);
				break;
			case 15:
				DisplayEventHandler.HandlerDisplayEvent_ShowDialogFromEvent(dataPool, argsOffset);
				break;
			case 16:
				DisplayEventHandler.HandleDisplayEvent_OpenLifeSkillCombat(dataPool, argsOffset);
				break;
			case 17:
				DisplayEventHandler.HandleDisplayEvent_BackToTutorialChapterMenu(dataPool, argsOffset);
				break;
			case 18:
				DisplayEventHandler.HandleDisplayEvent_RefreshCaravanData(dataPool, argsOffset);
				break;
			case 19:
				DisplayEventHandler.HandleDisplayEvent_ShowGmPanelMessage(dataPool, argsOffset);
				break;
			case 20:
				DisplayEventHandler.HandleDisplayEvent_StartShavingActionByEvent(dataPool, argsOffset);
				break;
			case 21:
				DisplayEventHandler.HandleDisplayEvent_OpenMonthNotifyForCricketContent();
				break;
			case 22:
				DisplayEventHandler.HandleDisplayEvent_OpenMajorEventPrepare(dataPool, argsOffset);
				break;
			case 23:
				DisplayEventHandler.HandleDisplayEvent_OpenAdventurePrepare(dataPool, argsOffset);
				break;
			case 24:
				DisplayEventHandler.HandleDisplayEvent_OperateBlackMask(dataPool, argsOffset);
				break;
			case 25:
				DisplayEventHandler.HandleDisplayEvent_StartCreateSwordTomb(dataPool, argsOffset);
				break;
			case 26:
				DisplayEventHandler.HandleDisplayEvent_RemoveSwordTomb(dataPool, argsOffset);
				break;
			case 27:
				DisplayEventHandler.HandleDisplayEvent_StartUnlockTaiwuStation(dataPool, argsOffset);
				break;
			case 28:
				DisplayEventHandler.HandleDisplayEvent_OpenTextureShow(dataPool, argsOffset);
				break;
			case 29:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_Items(dataPool, argsOffset);
				break;
			case 30:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_Chicken(dataPool, argsOffset);
				break;
			case 31:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_Characters(dataPool, argsOffset);
				break;
			case 32:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_SecretInformation(dataPool, argsOffset);
				break;
			case 33:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_Legacy(dataPool, argsOffset);
				break;
			case 34:
				DisplayEventHandler.HandleDisplayEvent_OpenGetItem_Feature(dataPool, argsOffset);
				break;
			case 35:
				DisplayEventHandler.HandleDisplayEvent_ShowEventCgTexture(dataPool, argsOffset);
				break;
			case 36:
				DisplayEventHandler.HandleDisplayEvent_OpenLegacyActivate();
				break;
			case 37:
				DisplayEventHandler.HandleDisplayEvent_MoveTaiwuAndRanChenZi(dataPool, argsOffset);
				break;
			case 38:
				DisplayEventHandler.HandleDisplayEvent_PlayAudioCommand(dataPool, argsOffset);
				break;
			case 39:
				DisplayEventHandler.HandleDisplayEvent_PlayMediaCommand(dataPool, argsOffset);
				break;
			case 40:
				DisplayEventHandler.HandleDisplayEvent_PerformCutscene(dataPool, argsOffset);
				break;
			case 41:
				DisplayEventHandler.HandleDisplayEvent_SetMainStoryBgm(dataPool, argsOffset);
				break;
			case 42:
				DisplayEventHandler.HandleDisplayEvent_AreaTravelCommand(dataPool, argsOffset);
				break;
			case 43:
				DisplayEventHandler.HandleDisplayEvent_LifeSkillCombatForceSilentResult(dataPool, argsOffset);
				break;
			case 44:
				DisplayEventHandler.HandleDisplayEvent_StartSelectSoul(dataPool, argsOffset);
				break;
			case 45:
				DisplayEventHandler.HandleDisplayEvent_OpenChangeWeaponTrick();
				break;
			case 46:
				DisplayEventHandler.HandleDisplayEvent_OpenInvestCaravan();
				break;
			case 47:
				DisplayEventHandler.HandleDisplayEvent_OpenMakeMedicine();
				break;
			case 48:
				DisplayEventHandler.HandleDisplayEvent_ConfirmProfessionSkillExecute(dataPool, argsOffset);
				break;
			case 49:
				DisplayEventHandler.HandlerDisplayEvent_UltimateSelectCharacterForDirectSamsaraMother(dataPool, argsOffset);
				break;
			case 50:
				DisplayEventHandler.HandleDisPlayEvent_ConfirmSkillExecuteAndPlayAnim(dataPool, argsOffset);
				break;
			case 51:
				DisplayEventHandler.HandlerDisplayEvent_OpenProfessionSkillUnlocked(dataPool, argsOffset);
				break;
			case 52:
				DisplayEventHandler.HandlerDisplayEvent_SetDisableMoving(dataPool, argsOffset);
				break;
			case 53:
				DisplayEventHandler.HandlerDisplayEvent_SetEventLockInputState(dataPool, argsOffset);
				break;
			case 54:
				DisplayEventHandler.HandlerDisplayEvent_EventHandleComplete();
				break;
			case 55:
				DisplayEventHandler.HandlerDisplayEvent_SectMainStoryEndScroll(dataPool, argsOffset);
				break;
			case 56:
				DisplayEventHandler.HandlerDisplayEvent_FindTreasureMaterial();
				break;
			case 57:
				DisplayEventHandler.HandlerDisplayEvent_CloseCharacterMenu();
				break;
			case 58:
				DisplayEventHandler.HandlerDisplayEvent_OpenKongsangSelectMapArea();
				break;
			case 59:
				DisplayEventHandler.HandlerDisplayEvent_HideBuildingArea();
				break;
			case 60:
				DisplayEventHandler.HandlerDisplayEvent_WorldMapDoShakeByEvent();
				break;
			case 61:
				DisplayEventHandler.HandlerDisplayEvent_OpenSectShaolinDemonSlayer();
				break;
			case 62:
				DisplayEventHandler.HandlerDisplayEvent_OpenModifyBook();
				break;
			case 63:
				DisplayEventHandler.HandlerDisplayEvent_OpenUpgradeTeammateCommand();
				break;
			case 64:
				DisplayEventHandler.HandlerDisplayEvent_OpenDefendHeavenlyTree(dataPool, argsOffset);
				break;
			case 65:
				DisplayEventHandler.HandlerDisplayEvent_OpenExtraordinaryCricket(dataPool, argsOffset);
				break;
			case 66:
				DisplayEventHandler.HandlerDisplayEvent_WorldMapDoFocusByEvent(dataPool, argsOffset);
				break;
			case 67:
				DisplayEventHandler.HandlerDisplayEvent_ForceClearLifeRecordCache();
				break;
			case 68:
				DisplayEventHandler.HandlerDisplayEvent_OpenCombatSkillSpecialBreak(dataPool, argsOffset);
				break;
			case 69:
				DisplayEventHandler.HandlerDisplayEvent_TaiwuCrossArchive(dataPool, argsOffset);
				break;
			case 70:
				DisplayEventHandler.HandlerDisplayEvent_TaiwuCrossArchiveSpecialEffect(dataPool, argsOffset);
				break;
			case 71:
				DisplayEventHandler.HandlerDisplayEvent_ShowEventConfirmWindow(dataPool, argsOffset);
				break;
			case 72:
				DisplayEventHandler.HandlerDisplayEvent_FuyuHiltMove(dataPool, argsOffset);
				break;
			case 73:
				DisplayEventHandler.HandlerDisplayEvent_FuyuHiltGuidingFinish(dataPool, argsOffset);
				break;
			case 74:
				DisplayEventHandler.HandlerDisplayEvent_OpenSelectLegacy(dataPool, argsOffset);
				break;
			case 75:
				DisplayEventHandler.HandlerDisplayEvent_OpenCombatConflict(dataPool, argsOffset);
				break;
			case 76:
				DisplayEventHandler.HandlerDisplayEvent_DreamBackSetAudioSetting(dataPool, argsOffset);
				break;
			case 77:
				DisplayEventHandler.HandlerDisplayEvent_RenderJiaoPoolState(dataPool, argsOffset);
				break;
			case 78:
				DisplayEventHandler.HandlerDisplayEvent_ConfirmJiaoNurturanceDialog(dataPool, argsOffset);
				break;
			case 79:
				DisplayEventHandler.HandlerDisplayEvent_PlayLoongDebuffAnimation(dataPool, argsOffset);
				break;
			case 80:
				DisplayEventHandler.HandlerDisplayEvent_ModDisplayEvent(dataPool, argsOffset);
				break;
			case 81:
				DisplayEventHandler.HandlerDisplayEvent_WuxianWugFairy(dataPool, argsOffset);
				break;
			case 82:
				DisplayEventHandler.HandlerDisplayEvent_WuxianWugKingDrive(dataPool, argsOffset);
				break;
			case 83:
				DisplayEventHandler.HandlerDisplayEvent_RanshanLegendaryBookKeeping(dataPool, argsOffset);
				break;
			case 84:
				DisplayEventHandler.HandlerDisplayEvent_BaihuaSectMainStorySpecial(dataPool, argsOffset);
				break;
			case 85:
				DisplayEventHandler.HandlerDisplayEvent_SettlementTreasuryEffect(dataPool, argsOffset);
				break;
			case 86:
				DisplayEventHandler.HandlerDisplayEvent_OpenSettlementPrison(dataPool, argsOffset);
				break;
			case 87:
				DisplayEventHandler.HandleDisplayEvent_OpenWarehouse(dataPool, argsOffset);
				break;
			case 88:
				DisplayEventHandler.HandleDisplayEvent_OpenProfessionSkillSpecial(dataPool, argsOffset);
				break;
			case 89:
				DisplayEventHandler.HandleDisplayEvent_OpenTasterUltimateResult(dataPool, argsOffset);
				break;
			case 90:
				DisplayEventHandler.HandleDisplayEvent_OpenTravelingTaoistMonkSkill2(dataPool, argsOffset);
				break;
			case 91:
				DisplayEventHandler.HandleDisplayEvent_OpenSelectSecretInformationLiteratiSkill2(dataPool, argsOffset);
				break;
			case 92:
				DisplayEventHandler.HandleDisplayEvent_OpenSelectNormalInformationLiteratiSkill3(dataPool, argsOffset);
				break;
			case 93:
				DisplayEventHandler.HandleDisplayEvent_OpenSelectCricketDukeSkill2(dataPool, argsOffset);
				break;
			case 94:
				DisplayEventHandler.HandleDisplayEvent_TaiwuBeKidnapped(dataPool, argsOffset);
				break;
			case 95:
				DisplayEventHandler.HandleDisplayEvent_ExtraProfessionSkillUnlocked(dataPool, argsOffset);
				break;
			case 96:
				DisplayEventHandler.HandleDisplayEvent_HidePartWorldMap(dataPool, argsOffset);
				break;
			case 97:
				DisplayEventHandler.HandleDisplayEvent_ShowNormalDialog(dataPool, argsOffset);
				break;
			case 98:
				DisplayEventHandler.HandleDisplayEvent_InteractCheckAnimation(dataPool, argsOffset);
				break;
			case 99:
				DisplayEventHandler.HandlerDisplayEvent_CheckSensitiveWord(dataPool, argsOffset);
				break;
			case 100:
				DisplayEventHandler.HandlerDisplayEvent_OpenCityPunishmentSeverityCustomizeUI(dataPool, argsOffset);
				break;
			case 101:
				DisplayEventHandler.HandlerDisplayEvent_StartDoctorHeal(dataPool, argsOffset);
				break;
			case 102:
				DisplayEventHandler.HandlerDisplayEvent_StartHeal(dataPool, argsOffset);
				break;
			case 103:
				DisplayEventHandler.HandlerDisplayEvent_SelectCombatSkill(dataPool, argsOffset);
				break;
			case 104:
				DisplayEventHandler.HandleDisplayEvent_SelectDemonSlayerCharacterFeature(dataPool, argsOffset);
				break;
			case 105:
				DisplayEventHandler.HandleDisplayEvent_CommonSelectCharacterFeature(dataPool, argsOffset);
				break;
			case 106:
				DisplayEventHandler.HandlerDisplayEvent_UnlockLifeSkillCombatStrategy(dataPool, argsOffset);
				break;
			case 107:
				DisplayEventHandler.HandlerDisplayEvent_ChangeLifeSkillCombatData(dataPool, argsOffset);
				break;
			case 108:
				DisplayEventHandler.HandlerDisplayEvent_PlayMapPickupEffect(dataPool, argsOffset);
				break;
			case 109:
				DisplayEventHandler.HandlerDisplayEvent_OpenCraftsmanPanelUI(dataPool, argsOffset);
				break;
			case 110:
				DisplayEventHandler.HandlerDisplayEvent_SectMainStoryWudangStart(dataPool, argsOffset);
				break;
			case 111:
				DisplayEventHandler.HandlerDisplayEvent_ShowYuanshanMiniGame(dataPool, argsOffset);
				break;
			case 112:
				DisplayEventHandler.HandlerDisplayEvent_PlayVitalAnim(dataPool, argsOffset);
				break;
			case 113:
				DisplayEventHandler.HandlerDisplayEvent_SwitchGuardedPageStatus(dataPool, argsOffset);
				break;
			case 114:
				DisplayEventHandler.HandlerDisplayEvent_ShowSectMainStoryUnlock(dataPool, argsOffset);
				break;
			case 115:
				DisplayEventHandler.HandlerDisplayEvent_ShowSectMainStorySpecialInteract(dataPool, argsOffset);
				break;
			case 116:
				DisplayEventHandler.HandlerDisplayEvent_ChangeMusicStatus(dataPool, argsOffset);
				break;
			case 117:
				DisplayEventHandler.HandlerDisplayEvent_ChangeSoundStatus(dataPool, argsOffset);
				break;
			case 118:
				DisplayEventHandler.HandleDisplayEvent_ChangeMusicVolume(dataPool, argsOffset);
				break;
			case 119:
				DisplayEventHandler.HandleDisplayEvent_PlayMusicForCount(dataPool, argsOffset);
				break;
			case 120:
				DisplayEventHandler.HandlerDisplayEvent_ShowAdventureFinish(dataPool, argsOffset);
				break;
			case 121:
				DisplayEventHandler.HandlerDisplayEvent_AdventureElementAlertAnim(dataPool, argsOffset);
				break;
			case 122:
				DisplayEventHandler.HandlerDisplayEvent_AdventureBlockChangeIcon(dataPool, argsOffset);
				break;
			case 123:
				DisplayEventHandler.HandlerDisplayEvent_AdventureElementShowHideEffect(dataPool, argsOffset);
				break;
			case 124:
				goto IL_B0D;
			case 125:
				DisplayEventHandler.HandlerDisplayEvent_AdventureRefreshBlockEffect(dataPool, argsOffset);
				break;
			case 126:
				DisplayEventHandler.HandlerDisplayEvent_AdventureRefreshGlobalEffect(dataPool, argsOffset);
				break;
			case 127:
				DisplayEventHandler.HandlerDisplayEvent_AdventureTaiwuShowDialog(dataPool, argsOffset);
				break;
			case 128:
				DisplayEventHandler.HandlerDisplayEvent_AdventureElementShowDialog(dataPool, argsOffset);
				break;
			case 129:
				DisplayEventHandler.HandlerDisplayEvent_AdventureElementDeleteAnim(dataPool, argsOffset);
				break;
			case 130:
				DisplayEventHandler.HandlerDisplayEvent_AdventureCameraMoveToBlock(dataPool, argsOffset);
				break;
			case 131:
				DisplayEventHandler.HandlerDisplayEvent_AdventureDelayAction(dataPool, argsOffset);
				break;
			case 132:
				DisplayEventHandler.HandlerDisplayEvent_AdventureEventHandled(dataPool, argsOffset);
				break;
			case 133:
				DisplayEventHandler.HandlerDisplayEvent_NewFeatureUnlock(dataPool, argsOffset);
				break;
			case 134:
				DisplayEventHandler.HandlerDisplayEvent_SectStoryPopUpToggle(dataPool, argsOffset);
				break;
			case 135:
				DisplayEventHandler.HandlerDisplayEvent_PlayAudioCommandWithFade(dataPool, argsOffset);
				break;
			case 136:
				DisplayEventHandler.HandlerDisplayEvent_ShowExchangePanel(dataPool, argsOffset);
				break;
			case 137:
				DisplayEventHandler.HandlerDisplayEvent_ShowCreateMirrorCharacter(dataPool, argsOffset);
				break;
			case 138:
				DisplayEventHandler.HandleDisplayEvent_BackToMainMenu(dataPool, argsOffset);
				break;
			case 139:
				DisplayEventHandler.HandleDisplayEvent_TriggerCricketCatch(dataPool, argsOffset);
				break;
			case 140:
				DisplayEventHandler.HandleDisplayEvent_AdventureStartSelectElement(dataPool, argsOffset);
				break;
			case 141:
				DisplayEventHandler.HandlerDisplayEvent_EnterAdventureFromEvent(dataPool, argsOffset);
				break;
			case 142:
				DisplayEventHandler.HandlerDisplayEvent_EnterMajorEventFromEvent(dataPool, argsOffset);
				break;
			case 143:
				DisplayEventHandler.HandlerDisplayEvent_MajorEventSkipCompleteAnim(dataPool, argsOffset);
				break;
			case 144:
				DisplayEventHandler.HandleDisplayEvent_ReadLifeRecord(dataPool, argsOffset);
				break;
			case 145:
				DisplayEventHandler.HandleDisplayEvent_ShowUnlockSkillSlotAnim(dataPool, argsOffset);
				break;
			case 146:
				DisplayEventHandler.HandlerDisplayEvent_SwitchDate(dataPool, argsOffset);
				break;
			case 147:
				DisplayEventHandler.HandleDisplayEvent_CricketPolymorphEffect(dataPool, argsOffset);
				break;
			case 148:
				DisplayEventHandler.HandleDisplayEvent_MoveTaiwuVillageAreaOnMonthNotifyClosed();
				break;
			default:
				goto IL_B0D;
			}
			i++;
			continue;
			IL_B0D:
			throw new Exception(string.Format("Unsupported DisplayEventType: {0}", (DisplayEventType)notification.DisplayEventType));
		}
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0004C080 File Offset: 0x0004A280
	private static void HandlerDisplayEvent_NewFeatureUnlock(RawDataPool dataPool, int argsOffset)
	{
		int functionUnlockTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref functionUnlockTemplateId);
		ViewNewFunctionUnlock.Queue.Enqueue(functionUnlockTemplateId);
		GEvent.OnEvent(UiEvents.NewFeatureUnlockHint, null);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0004C0BC File Offset: 0x0004A2BC
	private static void HandlerDisplayEvent_EnterMajorEventFromEvent(RawDataPool dataPool, int argsOffset)
	{
		int majorEventId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref majorEventId);
		UIElement.AdventureMajorEvent.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("MajorEventId", majorEventId));
		bool exist = UIElement.TextureShow.Exist;
		if (exist)
		{
			UIManager.Instance.HideUI(UIElement.TextureShow);
		}
		UIManager.Instance.StackToUI(UIElement.StateMajorEvent);
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0004C122 File Offset: 0x0004A322
	private static void HandlerDisplayEvent_MajorEventSkipCompleteAnim(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.MajorEventSkipCompleteAnim, null);
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0004C138 File Offset: 0x0004A338
	private static void HandlerDisplayEvent_EnterAdventureFromEvent(RawDataPool dataPool, int argsOffset)
	{
		int adventureId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref adventureId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("AdventureId", adventureId);
		GEvent.OnEvent(UiEvents.AdventureRemakeOpenPartOne, argBox);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0004C17C File Offset: 0x0004A37C
	private static void HandleDisplayEvent_OpenWarehouse(RawDataPool dataPool, int argsOffset)
	{
		int itemSourceInt = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref itemSourceInt);
		ItemSourceType itemSourceType = (ItemSourceType)itemSourceInt;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("WarehouseItemSourceType", itemSourceType).Set("CallTriggerListner", true);
		UIElement.Warehouse.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.Warehouse, true);
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0004C1DC File Offset: 0x0004A3DC
	private static void HandlerDisplayEvent_OpenSettlementPrison(RawDataPool dataPool, int argsOffset)
	{
		short settlementId = 0;
		bool isBreaking = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref settlementId);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isBreaking);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("SettlementId", settlementId).Set("IsBreaking", isBreaking);
		UIElement.SettlementPrison.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.SettlementPrison, true);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0004C241 File Offset: 0x0004A441
	private static void HandlerDisplayEvent_SettlementTreasuryEffect(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.SettlementTreasuryEffect, null);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0004C252 File Offset: 0x0004A452
	private static void HandlerDisplayEvent_BaihuaSectMainStorySpecial(RawDataPool dataPool, int argsOffset)
	{
		UIManager.Instance.ShowUI(UIElement.LifeLink, true);
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0004C268 File Offset: 0x0004A468
	private static void HandlerDisplayEvent_ModDisplayEvent(RawDataPool dataPool, int argsOffset)
	{
		string modIdStr = null;
		string customData = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref modIdStr);
		Serializer.Deserialize(dataPool, argsOffset, ref customData);
		ModManager.InvokeModDisplayEvent(modIdStr, customData);
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0004C299 File Offset: 0x0004A499
	private static void HandlerDisplayEvent_RanshanLegendaryBookKeeping(RawDataPool dataPool, int argsOffset)
	{
		UIManager.Instance.ShowUI(UIElement.LegendaryBookKeeping, true);
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0004C2AD File Offset: 0x0004A4AD
	private static void HandlerDisplayEvent_WuxianWugFairy(RawDataPool dataPool, int argsOffset)
	{
		UIManager.Instance.MaskUI(UIElement.MakeWugKing);
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0004C2C0 File Offset: 0x0004A4C0
	private static void HandlerDisplayEvent_WuxianWugKingDrive(RawDataPool dataPool, int argsOffset)
	{
		int targetCharId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref targetCharId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("TargetCharacterId", targetCharId);
		argBox.Set("InitialTabIndex", 1);
		UIElement.MakeWugKing.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.MakeWugKing);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0004C31C File Offset: 0x0004A51C
	private static void HandlerDisplayEvent_PlayLoongDebuffAnimation(RawDataPool dataPool, int argsOffset)
	{
		short characterTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref characterTemplateId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("LoongCharacterTemplateId", characterTemplateId);
		UIElement.LoongDebuffAnimation.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.LoongDebuffAnimation, true);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0004C36C File Offset: 0x0004A56C
	private static void HandlerDisplayEvent_DreamBackSetAudioSetting(RawDataPool dataPool, int argsOffset)
	{
		bool isOn = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isOn);
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		bool flag = !isOn;
		if (flag)
		{
			DisplayEventHandler.AudioSettingSeon = settingData.SeOn;
			settingData.SeOn = false;
		}
		else
		{
			bool audioSettingSeon = DisplayEventHandler.AudioSettingSeon;
			if (audioSettingSeon)
			{
				settingData.SeOn = true;
			}
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0004C3C4 File Offset: 0x0004A5C4
	private static void HandlerDisplayEvent_OpenCombatConflict(RawDataPool dataPool, int argsOffset)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		argBox.Set("CanOperate", true);
		argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None));
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0004C430 File Offset: 0x0004A630
	private static void HandlerDisplayEvent_RenderJiaoPoolState(RawDataPool dataPool, int argsOffset)
	{
		List<short> data = new List<short>();
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("PoolId", data[0]);
		argBox.Set("InteractionType", data[1]);
		argBox.Set("InteractionOptions", data[2]);
		argBox.Set("NurtureType", data[3]);
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0004C498 File Offset: 0x0004A698
	private static void HandlerDisplayEvent_ConfirmJiaoNurturanceDialog(RawDataPool dataPool, int argsOffset)
	{
		short nurturanceTemplateId = 0;
		bool isChange = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref nurturanceTemplateId);
		Serializer.Deserialize(dataPool, argsOffset, ref isChange);
		JiaoNurturanceItem config = JiaoNurturance.Instance[nurturanceTemplateId];
		bool flag = nurturanceTemplateId == 0;
		string contentString;
		if (flag)
		{
			contentString = LocalStringManager.Get(isChange ? LanguageKey.LK_Dlc_FiveLoong_Jiao_Nurturance_Dialog_Content_Natural_Change : LanguageKey.LK_Dlc_FiveLoong_Jiao_Nurturance_Dialog_Content_Natural);
		}
		else
		{
			LanguageKey contentTemplate = isChange ? LanguageKey.LK_Dlc_FiveLoong_Jiao_Nurturance_Dialog_Content_Change : LanguageKey.LK_Dlc_FiveLoong_Jiao_Nurturance_Dialog_Content;
			string typeString = CommonUtils.GetResOrExpName((int)config.ResourceCostType);
			string typeIcon = CommonUtils.GetResOrExpIconLegacy((int)config.ResourceCostType);
			string valueString = ((config.ResourceCostType < 0) ? config.ExpCost : config.ResourceCost).ToString();
			string addPropertyString = string.Join(LocalStringManager.Get(LanguageKey.LK_Separator), from x in config.BasePropertyChange
			select JiaoProperty.Instance[x.First].Name);
			contentString = LocalStringManager.GetFormat(contentTemplate, new object[]
			{
				config.Name,
				config.NurturanceCostMonth,
				typeString,
				valueString,
				addPropertyString,
				typeIcon
			});
		}
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Type = 1;
		dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Dlc_FiveLoong_Jiao_Nurturance_Dialog_Title);
		dialogCmd.Content = contentString;
		dialogCmd.Yes = delegate()
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("DlcLoongJiaoDialog", "Confirm", true);
			TaiwuEventDomainMethod.Call.TriggerListener("DlcLoongJiaoDialog", true);
		};
		dialogCmd.No = delegate()
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("DlcLoongJiaoDialog", "Confirm", false);
			TaiwuEventDomainMethod.Call.TriggerListener("DlcLoongJiaoDialog", true);
		};
		DialogCmd cmd = dialogCmd;
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0004C651 File Offset: 0x0004A851
	private static void HandlerDisplayEvent_OpenSelectLegacy(RawDataPool dataPool, int argsOffset)
	{
		UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", true).Set("CrossArchive", true));
		UIManager.Instance.MaskUI(UIElement.Legacy);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0004C68A File Offset: 0x0004A88A
	private static void HandlerDisplayEvent_FuyuHiltGuidingFinish(RawDataPool dataPool, int argsOffset)
	{
		UIManager.Instance.ChangeToUI(UIElement.StateMainWorld);
		SingletonObject.getInstance<MapRenderSystem>().ReverseClearNegativeFilm(0.3f);
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0004C6B0 File Offset: 0x0004A8B0
	private static void HandlerDisplayEvent_FuyuHiltMove(RawDataPool dataPool, int argsOffset)
	{
		List<short> movePath = null;
		Serializer.Deserialize(dataPool, argsOffset, ref movePath);
		SingletonObject.getInstance<YieldHelper>().StartYield(DisplayEventHandler.WaitForFuyuHiltMove(movePath));
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0004C6DB File Offset: 0x0004A8DB
	private static IEnumerator WaitForFuyuHiltMove(List<short> movePath)
	{
		WaitForSeconds second = new WaitForSeconds(0.5f);
		WaitForSeconds second2 = new WaitForSeconds(0.6f);
		UIElement.BlockInteract.Show();
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		mapModel.FuyuHiltMovePath = movePath;
		Location taiwuVillageLocation = mapModel.GetTaiwuVillageBlock();
		yield return second;
		Location targetLocation = Location.Invalid;
		int num;
		for (int i = 1; i < movePath.Count; i = num + 1)
		{
			targetLocation = new Location(taiwuVillageLocation.AreaId, movePath[i]);
			CharacterDomainMethod.Call.MoveFuyuHiltLocation(targetLocation);
			GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", targetLocation).Set("doTween", true).Set("tweenTime", 0.4f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
			yield return second2;
			num = i;
		}
		CharacterDomainMethod.Call.MoveFuyuHiltLocation(targetLocation, true);
		yield return second;
		targetLocation = mapModel.PlayerAtBlock.GetLocation();
		GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", targetLocation).Set("doTween", true).Set("tweenTime", 0.4f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
		yield return second;
		TaiwuEventDomainMethod.Call.TriggerListener("FuyuMoveFinish", true);
		mapModel.FuyuHiltMovePath = null;
		UIElement.BlockInteract.Hide(false);
		yield break;
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0004C6EC File Offset: 0x0004A8EC
	private static void HandlerDisplayEvent_ShowEventConfirmWindow(RawDataPool dataPool, int argsOffset)
	{
		string title = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref title);
		string content = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref content);
		bool showBtnNo = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref showBtnNo);
		string extraString = string.Empty;
		Serializer.Deserialize(dataPool, argsOffset, ref extraString);
		LanguageKey languageKeyTitle;
		string titleStr = LocalStringManager.Get(Enum.TryParse<LanguageKey>(title, out languageKeyTitle) ? languageKeyTitle : LanguageKey.Invalid);
		LanguageKey languageKeyContent;
		string contentStr = LocalStringManager.Get(Enum.TryParse<LanguageKey>(content, out languageKeyContent) ? languageKeyContent : LanguageKey.Invalid);
		bool flag = !string.IsNullOrEmpty(extraString);
		if (flag)
		{
			titleStr = titleStr.GetFormat(extraString);
			contentStr = contentStr.GetFormat(extraString);
		}
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Title = titleStr;
		dialogCmd.Content = contentStr;
		dialogCmd.Type = (showBtnNo ? 1 : 2);
		dialogCmd.Yes = delegate()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("DialogChoiceMade", true);
		};
		dialogCmd.No = delegate()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("DialogChoiceMade", false);
		};
		dialogCmd.GroupYesText = LocalStringManager.Get(LanguageKey.LK_Yes);
		dialogCmd.GroupNoText = LocalStringManager.Get(LanguageKey.LK_No);
		DialogCmd cmd = dialogCmd;
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0004C84B File Offset: 0x0004AA4B
	private static void HandlerDisplayEvent_TaiwuCrossArchiveSpecialEffect(RawDataPool dataPool, int argsOffset)
	{
		SingletonObject.getInstance<YieldHelper>().StartYield(DisplayEventHandler.WaitForMapRenderAndSetNegative());
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0004C85E File Offset: 0x0004AA5E
	private static IEnumerator WaitForMapRenderAndSetNegative()
	{
		yield return new WaitUntil(() => GameApp.Instance != null && GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame);
		UIManager.Instance.ChangeToUI(UIElement.TaiwuCrossArchive);
		TaiwuEventDomainMethod.Call.UserLoadDreamBackArchive();
		yield return new WaitUntil(() => WorldMapModel.MapBlockRenderFinish);
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		List<Location> expectLocations = EasyPool.Get<List<Location>>();
		expectLocations.Clear();
		expectLocations.Add(worldMapModel.GetTaiwuVillageBlock());
		SingletonObject.getInstance<MapRenderSystem>().SetNegativeFilmInArea(expectLocations, 0.3f, true);
		yield break;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0004C868 File Offset: 0x0004AA68
	private static void HandlerDisplayEvent_TaiwuCrossArchive(RawDataPool dataPool, int argsOffset)
	{
		bool isCross = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isCross);
		sbyte worldIndex = -1;
		Serializer.Deserialize(dataPool, argsOffset, ref worldIndex);
		bool flag = isCross;
		if (flag)
		{
			GameApp.LoadArchive(worldIndex, true);
		}
		else
		{
			GameApp.ReturnToMainMenu(null, null, null);
		}
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0004C8AC File Offset: 0x0004AAAC
	private static void HandlerDisplayEvent_OpenCombatSkillSpecialBreak(RawDataPool dataPool, int argsOffset)
	{
		short charTemplateId = -1;
		bool isBaiyuan = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charTemplateId);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isBaiyuan);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterTemplateId", charTemplateId);
		argBox.Set("IsBaiyuan", isBaiyuan);
		UIElement.CombatSkillSpecialBreak.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CombatSkillSpecialBreak, true);
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0004C918 File Offset: 0x0004AB18
	private static void HandlerDisplayEvent_OpenExtraordinaryCricket(RawDataPool dataPool, int argsOffset)
	{
		ItemKey itemKey = ItemKey.Invalid;
		Serializer.Deserialize(dataPool, argsOffset, ref itemKey);
		UIElement.ExtraordinaryCricket.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set<ItemKey>("ItemKey", itemKey));
		UIManager.Instance.ShowUI(UIElement.ExtraordinaryCricket, true);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0004C964 File Offset: 0x0004AB64
	private static void HandleDisplayEvent_OpenCatchCricket()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("targetScale", 2f);
		argBox.Set("duration", 0.7f);
		argBox.SetObject("easeMode", Ease.OutQuad);
		argBox.SetObject("tweenType", EWorldmapScaleTweenType.IntervalPingPong);
		GEvent.OnEvent(UiEvents.DoWorldMapScaleTween, argBox);
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.7f, delegate
		{
			UIManager.Instance.ShowUI(UIElement.CatchCricket, true);
		});
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0004C9FE File Offset: 0x0004ABFE
	private static void HandleDisplayEvent_ExitAdventure()
	{
		GEvent.OnEvent(UiEvents.OnOpenAdventureResult, null);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0004CA14 File Offset: 0x0004AC14
	private static void HandleDisplayEvent_OpenCricketBattle(RawDataPool dataPool, int argsOffset)
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.CricketCombat);
		if (!flag)
		{
			int enemyId = 0;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref enemyId);
			Wager selfWager = default(Wager);
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref selfWager);
			CricketWagerData enemyWagerData = null;
			Serializer.Deserialize(dataPool, argsOffset, ref enemyWagerData);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("EnemyId", enemyId);
			argBox.Set<Wager>("SelfWager", selfWager);
			argBox.Set<CricketWagerData>("EnemyWagerData", enemyWagerData);
			UIElement.CricketCombat.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CricketCombat, true);
		}
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0004CAB4 File Offset: 0x0004ACB4
	private static void HandleDisplayEvent_OpenCricketBattleWithConfig(RawDataPool dataPool, int argsOffset)
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.CricketCombat);
		if (!flag)
		{
			int enemyId = 0;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref enemyId);
			Wager selfWager = default(Wager);
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref selfWager);
			CricketWagerData enemyWagerData = null;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref enemyWagerData);
			bool doubleDamage = false;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref doubleDamage);
			bool onlyNoInjuryCricket = false;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref onlyNoInjuryCricket);
			sbyte minGrade = 0;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref minGrade);
			sbyte maxGrade = 8;
			Serializer.Deserialize(dataPool, argsOffset, ref maxGrade);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("EnemyId", enemyId);
			argBox.Set<Wager>("SelfWager", selfWager);
			argBox.Set<CricketWagerData>("EnemyWagerData", enemyWagerData);
			argBox.Set("DoubleDamage", doubleDamage);
			argBox.Set("OnlyNoInjuryCricket", onlyNoInjuryCricket);
			argBox.Set("MinGrade", minGrade);
			argBox.Set("MaxGrade", maxGrade);
			argBox.Set("IsAutumnCricketMatch", true);
			UIElement.CricketCombat.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CricketCombat, true);
		}
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x0004CBE4 File Offset: 0x0004ADE4
	private static void HandleDisplayEvent_OpenShop(RawDataPool dataPool, int argsOffset)
	{
		OpenShopEventArguments openShopEventArguments = new OpenShopEventArguments();
		Serializer.Deserialize(dataPool, argsOffset, ref openShopEventArguments);
		bool isSettlementTreasury = openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			ViewSettlementShop.ShowOrUpdate(openShopEventArguments);
		}
		else
		{
			UIElement.NewShop.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("OpenShopEventArguments", openShopEventArguments));
			UIManager.Instance.ShowUI(UIElement.NewShop, true);
		}
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0004CC44 File Offset: 0x0004AE44
	private static void HandleDisplayEvent_OpenBookTrade(RawDataPool dataPool, int argsOffset)
	{
		int merchantId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref merchantId);
		bool isFavorExchange = false;
		Serializer.Deserialize(dataPool, argsOffset, ref isFavorExchange);
		UIElement.ExchangeBook.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", merchantId).Set("IsFavorExchange", isFavorExchange));
		UIManager.Instance.ShowUI(UIElement.ExchangeBook, true);
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x0004CCA4 File Offset: 0x0004AEA4
	private static void HandleDisplayEvent_StartCombat(RawDataPool dataPool, int argsOffset)
	{
		List<int> leftTeam = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref leftTeam);
		List<int> rightTeam = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref rightTeam);
		short combatConfigId = 0;
		Serializer.Deserialize(dataPool, argsOffset, ref combatConfigId);
		bool flag = GMFunc.IsOvercomeCombat(combatConfigId, rightTeam);
		if (flag)
		{
			GEvent.OnEvent(UiEvents.OnChangeTopUiInAdventure, null);
		}
		else
		{
			bool exist = UIElement.EventWindow.Exist;
			if (exist)
			{
				UIManager.Instance.HideUI(UIElement.EventWindow);
			}
			bool exist2 = UIElement.TextureShow.Exist;
			if (exist2)
			{
				UIManager.Instance.HideUI(UIElement.TextureShow);
			}
			bool exist3 = UIElement.CharacterMenu.Exist;
			if (exist3)
			{
				UIManager.Instance.HideUI(UIElement.CharacterMenu);
			}
			SingletonObject.getInstance<CombatModel>().SetEntryData(combatConfigId, leftTeam, rightTeam);
			UIManager.Instance.ShowUI(UIElement.CombatBegin, true);
			GEvent.OnEvent(UiEvents.OnChangeTopUiInAdventure, null);
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0004CD90 File Offset: 0x0004AF90
	private static void HandleDisplayEvent_ChangeTeammateCommandOnCombatBegin(RawDataPool dataPool, int argsOffset)
	{
		int charId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charId);
		List<sbyte> cmdTypes = null;
		Serializer.Deserialize(dataPool, argsOffset, ref cmdTypes);
		CombatModel combatModel = SingletonObject.getInstance<CombatModel>();
		combatModel.SetChangeData(charId, cmdTypes);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0004CDC8 File Offset: 0x0004AFC8
	private static void HandleDisplayEvent_CombatShowCommand(RawDataPool pool, int offset)
	{
		TeammateCommandDisplayData data = default(TeammateCommandDisplayData);
		Serializer.Deserialize(pool, offset, ref data);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("data", data);
		GEvent.OnEvent(UiEvents.ShowCombatTeammateCommand, argBox);
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0004CE10 File Offset: 0x0004B010
	private static void HandleDisplayEvent_CombatShowSpecialEffect(RawDataPool pool, int offset)
	{
		bool isAlly = false;
		offset += Serializer.Deserialize(pool, offset, ref isAlly);
		ShowSpecialEffectDisplayData data = default(ShowSpecialEffectDisplayData);
		Serializer.Deserialize(pool, offset, ref data);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("isAlly", isAlly).SetObject("data", data);
		GEvent.OnEvent(UiEvents.ShowCombatSpecialEffectTips, argBox);
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0004CE74 File Offset: 0x0004B074
	private static void HandleDisplayEvent_CombatShowSurrenderMark(RawDataPool pool, int offset)
	{
		List<int> showData = null;
		Serializer.Deserialize(pool, offset, ref showData);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("showData", showData);
		GEvent.OnEvent(UiEvents.ShowCombatSurrenderMark, argBox);
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0004CEB0 File Offset: 0x0004B0B0
	private static void HandleDisplayEvent_CombatShowFleeAnimation(RawDataPool pool, int offset)
	{
		int failCharId = 0;
		offset += Serializer.Deserialize(pool, offset, ref failCharId);
		string failAnimation = string.Empty;
		Serializer.Deserialize(pool, offset, ref failAnimation);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("failCharId", failCharId).Set("failAnimation", failAnimation);
		GEvent.OnEvent(UiEvents.ShowFleeAnimation, argBox);
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0004CF0C File Offset: 0x0004B10C
	private static void HandleDisplayEvent_CombatShowTianSuiBaoLu(RawDataPool pool, int offset)
	{
		bool isDirect = false;
		offset += Serializer.Deserialize(pool, offset, ref isDirect);
		int charId = 0;
		Serializer.Deserialize(pool, offset, ref charId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("isDirect", isDirect).Set("charId", charId);
		GEvent.OnEvent(UiEvents.ShowCombatTianSuiBaoLu, argBox);
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0004CF64 File Offset: 0x0004B164
	private static void HandleDisplayEvent_CombatShowAbsorbNeiliAllocation(RawDataPool pool, int offset)
	{
		int targetId = 0;
		offset += Serializer.Deserialize(pool, offset, ref targetId);
		int absorbCharId = 0;
		offset += Serializer.Deserialize(pool, offset, ref absorbCharId);
		byte type = 0;
		Serializer.Deserialize(pool, offset, ref type);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("targetId", targetId).Set("absorbCharId", absorbCharId).Set("neiliAllocationType", type);
		GEvent.OnEvent(UiEvents.ShowAbsorbNeiliAllocation, argBox);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0004CFD4 File Offset: 0x0004B1D4
	private static void HandleDisplayEvent_PerformCutscene(RawDataPool dataPool, int argsOffset)
	{
		short cutsceneId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref cutsceneId);
		EventCutsceneItem cutSceneConfig = EventCutscene.Instance.GetItem(cutsceneId);
		bool flag = cutSceneConfig != null;
		if (flag)
		{
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("CGName", Path.Combine("Cutscenes", cutSceneConfig.ResourceFormat));
			box.Set("Localized", true);
			box.Set("RenderMode", 2);
			box.Set("JumpOpen", cutSceneConfig.CanSkip);
			box.SetObject("OnVideoPlayStart", new Action(DisplayEventHandler.<HandleDisplayEvent_PerformCutscene>g__OnVideoPlayStart|45_1));
			bool flag2 = cutSceneConfig.CommandPanelOffset != null;
			if (flag2)
			{
				box.SetObject("CommandPanelOffset", new Vector2((float)cutSceneConfig.CommandPanelOffset[0], (float)cutSceneConfig.CommandPanelOffset[1]));
			}
			UIElement cgPlayer = UIElement.CgPlayer;
			cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, new Action(delegate()
			{
				AudioManager.Instance.ExitVideoMode();
				DisplayEventHandler.<HandleDisplayEvent_PerformCutscene>g__Finish|45_0();
				UIElement.BlockInteract.Hide(false);
			}));
			UIElement.CgPlayer.SetOnInitArgs(box);
			UIElement.CgPlayer.Show();
			UIElement.BlockInteract.Show();
		}
		else
		{
			DisplayEventHandler.<HandleDisplayEvent_PerformCutscene>g__Finish|45_0();
		}
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0004D120 File Offset: 0x0004B320
	private static void HandleDisplayEvent_OpenLifeSkillCombat(RawDataPool dataPool, int argsOffset)
	{
		int enemyId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref enemyId);
		sbyte lifeSkillType = -1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref lifeSkillType);
		bool hideTaiwuAudience = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref hideTaiwuAudience);
		bool flag = GMFunc.IsOvercomeLifeSkillCombat();
		if (flag)
		{
			GEvent.OnEvent(UiEvents.OnChangeTopUiInAdventure, null);
		}
		else
		{
			SingletonObject.getInstance<LifeSkillCombatModel>().StartLifeSkillCombat(enemyId, lifeSkillType, hideTaiwuAudience);
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x0004D188 File Offset: 0x0004B388
	private static void HandleDisplayEvent_BackToTutorialChapterMenu(RawDataPool dataPool, int argsOffset)
	{
		int chapterId = 0;
		Serializer.Deserialize(dataPool, argsOffset, ref chapterId);
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		bool flag = settings.CompletedChapters < chapterId;
		if (flag)
		{
			settings.CompletedChapters = chapterId;
		}
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("Tutorial", chapterId > 0);
		GameApp.ReturnToMainMenu(argBox, null, null);
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x0004D1DC File Offset: 0x0004B3DC
	private static void HandleDisplayEvent_RefreshCaravanData(RawDataPool dataPool, int argsOffset)
	{
		List<CaravanDisplayData> args = EasyPool.Get<List<CaravanDisplayData>>();
		Serializer.Deserialize(dataPool, argsOffset, ref args);
		SingletonObject.getInstance<WorldMapModel>().SetCaravanData(args);
		EasyPool.Free<List<CaravanDisplayData>>(args);
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x0004D210 File Offset: 0x0004B410
	private static void HandleDisplayEvent_ShowGmPanelMessage(RawDataPool dataPool, int argsOffset)
	{
		string message = null;
		Serializer.Deserialize(dataPool, argsOffset, ref message);
		UI_GMWindow instance = UI_GMWindow.Instance;
		if (instance != null)
		{
			instance.Log(message);
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0004D23C File Offset: 0x0004B43C
	private static void HandleDisplayEvent_StartShavingActionByEvent(RawDataPool dataPool, int argsOffset)
	{
		int cutterCharId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref cutterCharId);
		int targetCharId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref targetCharId);
		int time = -1;
		Serializer.Deserialize(dataPool, argsOffset, ref time);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharId", targetCharId);
		argBox.Set("NpcId", cutterCharId);
		argBox.Set("time", time);
		UIElement.CharacterShave.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.CharacterShave);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0004D2C0 File Offset: 0x0004B4C0
	private static void HandleDisplayEvent_OpenMonthNotifyForCricketContent()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		UIElement.MonthNotify.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0004D2F4 File Offset: 0x0004B4F4
	private static void HandleDisplayEvent_OpenMajorEventPrepare(RawDataPool dataPool, int argsOffset)
	{
		short areaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
		short blockId = SingletonObject.getInstance<WorldMapModel>().CurrentBlockId;
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		AdventureBlockCacheData data = model.TryGetLocationAdventureRemake(new Location(areaId, blockId));
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MajorEventId", data.MajorEventId).Set("EnterAreaId", areaId).Set("EnterBlockId", blockId);
		UIElement.AdventurePrepareRemake.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.AdventurePrepareRemake);
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0004D378 File Offset: 0x0004B578
	private static void HandleDisplayEvent_OpenAdventurePrepare(RawDataPool dataPool, int argsOffset)
	{
		short areaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
		short blockId = SingletonObject.getInstance<WorldMapModel>().CurrentBlockId;
		AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
		AdventureBlockCacheData data = model.TryGetLocationAdventureRemake(new Location(areaId, blockId));
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("AdventureId", data.AdventureId).Set("EnterAreaId", areaId).Set("EnterBlockId", blockId);
		UIElement.AdventurePrepareRemake.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.AdventurePrepareRemake);
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0004D3FC File Offset: 0x0004B5FC
	private static void HandleDisplayEvent_OperateBlackMask(RawDataPool dataPool, int argsOffset)
	{
		bool animToShowMask = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref animToShowMask);
		float animTime = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref animTime);
		bool hideAfterShow = false;
		Serializer.Deserialize(dataPool, argsOffset, ref hideAfterShow);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("AnimToShowMask", animToShowMask);
		argBox.Set("AnimTime", animTime);
		argBox.Set("HideAfterShow", hideAfterShow);
		bool flag = !UIElement.BlackMask.Exist;
		if (flag)
		{
			UIElement.BlackMask.SetOnInitArgs(argBox);
			UIElement.BlackMask.Show();
		}
		else
		{
			UIElement.BlackMask.UiBaseAs<UI_BlackMask>().Refresh(argBox, true);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0004D4A8 File Offset: 0x0004B6A8
	private static void HandleDisplayEvent_StartCreateSwordTomb(RawDataPool dataPool, int argsOffset)
	{
		List<Location> locationList = new List<Location>();
		Serializer.Deserialize(dataPool, argsOffset, ref locationList);
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		worldMap.StartCoroutine(worldMap.ShowSwordTombAppear(locationList));
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0004D4E0 File Offset: 0x0004B6E0
	private static void HandleDisplayEvent_RemoveSwordTomb(RawDataPool dataPool, int argsOffset)
	{
		MapBlockData rootMapBlock = new MapBlockData();
		Serializer.Deserialize(dataPool, argsOffset, ref rootMapBlock);
		BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
		bool isDreamBack = basicGameData.IsDreamBack;
		if (isDreamBack)
		{
			MapDomainMethod.Call.RemoveSwordTombFromLocation(rootMapBlock.GetLocation());
		}
		else
		{
			SingletonObject.getInstance<WorldMapModel>().RemoveSwordTombRootMapBlock = rootMapBlock;
		}
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0004D528 File Offset: 0x0004B728
	private static void HandleDisplayEvent_StartUnlockTaiwuStation(RawDataPool dataPool, int argsOffset)
	{
		Location location = Location.Invalid;
		Serializer.Deserialize(dataPool, argsOffset, ref location);
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		worldMap.StartCoroutine(worldMap.ShowUnlockTaiwuPostLocation(location));
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0004D560 File Offset: 0x0004B760
	private static void HandleDisplayEvent_OpenTextureShow(RawDataPool dataPool, int argsOffset)
	{
		string texturePath = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref texturePath);
		string textureType = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref textureType);
		string countDownActionName = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref countDownActionName);
		int intParam = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref intParam);
		float countDownTime = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref countDownTime);
		string modName = string.Empty;
		Serializer.Deserialize(dataPool, argsOffset, ref modName);
		bool flag = !UIElement.TextureShow.IsShowing && !UIElement.TextureShow.IsWaitShowing && string.IsNullOrEmpty(texturePath);
		if (!flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("TexturePath", texturePath);
			argBox.Set("TextureType", textureType);
			argBox.Set("CountDownActionName", countDownActionName);
			argBox.Set("IntParam", intParam);
			argBox.Set("CountDownTime", countDownTime);
			argBox.Set("ModName", modName);
			bool flag2 = UIManager.Instance.IsElementActive(UIElement.TextureShow);
			if (flag2)
			{
				GEvent.OnEvent(UiEvents.OnTextureShowChanged, argBox);
			}
			else
			{
				UIElement.TextureShow.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.TextureShow, true);
			}
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0004D6B0 File Offset: 0x0004B8B0
	private static void HandleDisplayEvent_OpenGetItem_Items(RawDataPool dataPool, int argsOffset)
	{
		List<ItemDisplayData> itemDisplayDataList = new List<ItemDisplayData>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref itemDisplayDataList);
		bool autoToWareHouse = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref autoToWareHouse);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("ItemList", itemDisplayDataList);
		argBox.Set("InWareHouse", autoToWareHouse);
		UIElement.GetItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.GetItem);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0004D71C File Offset: 0x0004B91C
	private static void HandleDisplayEvent_OpenGetItem_Chicken(RawDataPool dataPool, int argsOffset)
	{
		List<GameData.Domains.Building.Chicken> chicken = new List<GameData.Domains.Building.Chicken>();
		Serializer.Deserialize(dataPool, argsOffset, ref chicken);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("ChickenList", chicken);
		argBox.Set("Title", LocalStringManager.Get(LanguageKey.LK_Get_Item_GetChicken));
		UIElement.GetItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.GetItem);
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x0004D780 File Offset: 0x0004B980
	private static void HandleDisplayEvent_OpenGetItem_Characters(RawDataPool dataPool, int argsOffset)
	{
		List<int> charIdList = new List<int>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charIdList);
		sbyte type = 0;
		Serializer.Deserialize(dataPool, argsOffset, ref type);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("CharIdList", charIdList);
		argBox.Set("ObtainType", type);
		UIElement.GetItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.GetItem);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0004D7EC File Offset: 0x0004B9EC
	private static void HandleDisplayEvent_OpenGetItem_SecretInformation(RawDataPool dataPool, int argsOffset)
	{
		int metaSecretInformationId = -1;
		Serializer.Deserialize(dataPool, argsOffset, ref metaSecretInformationId);
		BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
		bool flag = basicGameData.AdvancingMonthState == 0;
		if (flag)
		{
			List<int> information = new List<int>();
			information.Add(metaSecretInformationId);
			InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackage(null, information, delegate(int offset, RawDataPool dataPool2)
			{
				SecretInformationDisplayPackage displayData = new SecretInformationDisplayPackage();
				Serializer.Deserialize(dataPool2, offset, ref displayData);
				ArgumentBox argBox = new ArgumentBox();
				argBox.Set("Title", LocalStringManager.Get(LanguageKey.LK_GetItem_SelectInformation));
				argBox.SetObject("SecretInformationDisplayPackage", displayData);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			});
		}
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0004D850 File Offset: 0x0004BA50
	private static void HandleDisplayEvent_OpenGetItem_Legacy(RawDataPool dataPool, int argsOffset)
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.Loading) || UIManager.Instance.IsElementActive(UIElement.AreaStoryScroll);
		if (!flag)
		{
			List<short> list = new List<short>();
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref list);
			sbyte type = 0;
			Serializer.Deserialize(dataPool, argsOffset, ref type);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("LegacyList", list);
			argBox.Set("ObtainType", type);
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0004D8E4 File Offset: 0x0004BAE4
	private static void HandleDisplayEvent_OpenGetItem_Feature(RawDataPool dataPool, int argsOffset)
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.Loading) || UIManager.Instance.IsElementActive(UIElement.AreaStoryScroll);
		if (!flag)
		{
			List<short> list = new List<short>();
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref list);
			sbyte type = 0;
			Serializer.Deserialize(dataPool, argsOffset, ref type);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("FeatureList", list);
			argBox.Set("ObtainType", type);
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
		}
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x0004D978 File Offset: 0x0004BB78
	private static void HandleDisplayEvent_ShowEventCgTexture(RawDataPool dataPool, int argsOffset)
	{
		string cgTexturePath = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref cgTexturePath);
		float tweenTime = 0f;
		Serializer.Deserialize(dataPool, argsOffset, ref tweenTime);
		SingletonObject.getInstance<EventModel>().SetEventCgData(cgTexturePath, tweenTime);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0004D9B6 File Offset: 0x0004BBB6
	private static void HandleDisplayEvent_OpenLegacyActivate()
	{
		UIManager.Instance.MaskUI(UIElement.LegacyActivate);
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0004D9CC File Offset: 0x0004BBCC
	private static void HandleDisplayEvent_MoveTaiwuAndRanChenZi(RawDataPool dataPool, int argsOffset)
	{
		Location curLocation = Location.Invalid;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref curLocation);
		Location targetLocation = Location.Invalid;
		Serializer.Deserialize(dataPool, argsOffset, ref targetLocation);
		bool exist = UIElement.WorldMap.Exist;
		if (exist)
		{
			UIManager.Instance.ShowUI(UIElement.BlockInteract, true);
			ViewWorldMap uiWorldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			uiWorldMap.MoveCameraTo(targetLocation, true, delegate()
			{
				TaiwuEventDomainMethod.Call.TriggerListener("MoveTaiwuAndRanChenZiComplete", true);
				UIManager.Instance.HideUI(UIElement.BlockInteract);
			}, 0.75f, Ease.Linear);
		}
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x0004DA58 File Offset: 0x0004BC58
	private static void HandleDisplayEvent_PlayAudioCommand(RawDataPool dataPool, int argsOffset)
	{
		string audioFileName = string.Empty;
		bool isMusic = false;
		bool isLoop = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref audioFileName);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isMusic);
		Serializer.Deserialize(dataPool, argsOffset, ref isLoop);
		bool flag = string.IsNullOrEmpty(audioFileName);
		if (flag)
		{
			bool flag2 = isMusic;
			if (flag2)
			{
				AudioManager.Instance.PlayMusic(AudioManager.DummyAudioName, 1f, 100, null);
			}
		}
		else
		{
			bool flag3 = isMusic;
			if (flag3)
			{
				AudioManager.Instance.PlayMusic(audioFileName, 1f, 100, null);
			}
			else
			{
				AudioManager.Instance.PlaySound(audioFileName, isLoop, false);
			}
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0004DAEC File Offset: 0x0004BCEC
	private static void HandleDisplayEvent_SetMainStoryBgm(RawDataPool dataPool, int argsOffset)
	{
		string audioFileName = string.Empty;
		Serializer.Deserialize(dataPool, argsOffset, ref audioFileName);
		SingletonObject.getInstance<WorldMapModel>().UpdateMainStoryBgm(audioFileName);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0004DB18 File Offset: 0x0004BD18
	private static void HandleDisplayEvent_PlayMediaCommand(RawDataPool dataPool, int argsOffset)
	{
		string mediaFileName = string.Empty;
		bool canJump = false;
		string finishAction = string.Empty;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref mediaFileName);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref canJump);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref finishAction);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("CGName", mediaFileName);
		box.Set("RenderMode", 0);
		box.Set("JumpOpen", canJump);
		UIElement cgPlayer = UIElement.CgPlayer;
		cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, new Action(delegate()
		{
			AudioManager.Instance.ExitVideoMode();
			string finishAction = finishAction;
			string a = finishAction;
			if (!(a == "ReturnToMainMenu"))
			{
				if (a == "LoadDreamBack")
				{
					sbyte worldIndex = -1;
					Serializer.Deserialize(dataPool, argsOffset, ref worldIndex);
					GameApp.LoadArchive(worldIndex, true);
				}
			}
			else
			{
				GameApp.ReturnToMainMenu(null, null, null);
			}
		}));
		box.SetObject("OnVideoPlayStart", new Action(DisplayEventHandler.<HandleDisplayEvent_PlayMediaCommand>g__OnVideoPlayStart|70_0));
		UIElement.CgPlayer.SetOnInitArgs(box);
		UIElement.CgPlayer.Show();
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0004DC34 File Offset: 0x0004BE34
	private static void HandleDisplayEvent_AreaTravelCommand(RawDataPool dataPool, int argsOffset)
	{
		string travelCommand = string.Empty;
		Serializer.Deserialize(dataPool, argsOffset, ref travelCommand);
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		string text = travelCommand;
		string a = text;
		if (!(a == "DeepValleyToSmallVillage"))
		{
			if (!(a == "SmallVillageToBrokenArea"))
			{
				if (!(a == "BrokenAreaToTaiwuVillageArea"))
				{
					if (!(a == "DeepValleyToTaiwuVillageArea"))
					{
						if (!(a == "TravelToPastTaiwuVillageArea"))
						{
							if (a == "BackFromPastTaiwuVillageArea")
							{
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
								argBox.Set("AnimToShowMask", true);
								argBox.Set("AnimTime", 1f);
								argBox.Set("HideAfterShow", false);
								UIElement.BlackMask.SetOnInitArgs(argBox);
								UIElement.BlackMask.Show();
								SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
								{
									SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId(), delegate(int progress)
									{
										bool flag2 = progress >= 100;
										if (flag2)
										{
											SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(3f, delegate
											{
												UIManager.Instance.ChangeToUI(UIElement.StateMainWorld);
												TaiwuEventDomainMethod.Call.TriggerListener("BackFromPastTaiwuVillage", true);
												SingletonObject.getInstance<MapRenderSystem>().ReverseClearNegativeFilm(0.3f);
												ArgumentBox argBox4 = EasyPool.Get<ArgumentBox>();
												argBox4.Set("AnimToShowMask", false);
												argBox4.Set("AnimTime", 3f);
												argBox4.Set("HideAfterShow", true);
												UIElement.BlackMask.SetOnInitArgs(argBox4);
												UIElement.BlackMask.UiBaseAs<UI_BlackMask>().Refresh(argBox4, true);
											});
										}
									}));
								});
							}
						}
						else
						{
							ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
							argBox2.Set("AnimToShowMask", true);
							argBox2.Set("AnimTime", 1f);
							argBox2.Set("HideAfterShow", false);
							UIElement.BlackMask.SetOnInitArgs(argBox2);
							UIElement.BlackMask.Show();
							SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
							{
								SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(139, delegate(int progress)
								{
									bool flag2 = progress >= 100;
									if (flag2)
									{
										SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(3f, delegate
										{
											UIManager.Instance.ChangeToUI(UIElement.PastTaiwuVillage);
											TaiwuEventDomainMethod.Call.TriggerListener("TravelToPastTaiwuVillage", true);
											ArgumentBox argBox4 = EasyPool.Get<ArgumentBox>();
											argBox4.Set("AnimToShowMask", false);
											argBox4.Set("AnimTime", 3f);
											argBox4.Set("HideAfterShow", true);
											UIElement.BlackMask.SetOnInitArgs(argBox4);
											UIElement.BlackMask.UiBaseAs<UI_BlackMask>().Refresh(argBox4, true);
										});
									}
								}));
							});
						}
					}
					else
					{
						int prevVsyncCount = QualitySettings.vSyncCount;
						ArgumentBox loadingArgBox = EasyPool.Get<ArgumentBox>();
						loadingArgBox.SetObject("OnLoadingStart", new Action(delegate
						{
							Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
							SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(taiwuVillageLocation.AreaId, delegate(int progress)
							{
								GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", progress));
							}));
						}));
						loadingArgBox.SetObject("OnLoadingFinish", new Action(delegate
						{
							TaiwuEventDomainMethod.Call.TriggerListener("MainStoryHeroicWords", true);
							GameApp.Instance.ChangeGameState(EGameState.InGame, null);
							QualitySettings.vSyncCount = prevVsyncCount;
						}));
						QualitySettings.vSyncCount = 0;
						GameApp.Instance.ChangeGameState(EGameState.Loading, loadingArgBox);
					}
				}
				else
				{
					int prevVsyncCount = QualitySettings.vSyncCount;
					ArgumentBox loadingArgBox2 = EasyPool.Get<ArgumentBox>();
					loadingArgBox2.SetObject("OnLoadingStart", new Action(delegate
					{
						Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
						SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(taiwuVillageLocation.AreaId, delegate(int progress)
						{
							GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", progress));
						}));
					}));
					loadingArgBox2.SetObject("OnLoadingFinish", new Action(delegate
					{
						TaiwuEventDomainMethod.Call.TriggerListener("MainStotyEnterTaiwuArea", true);
						GameApp.Instance.ChangeGameState(EGameState.InGame, null);
						QualitySettings.vSyncCount = prevVsyncCount;
					}));
					QualitySettings.vSyncCount = 0;
					GameApp.Instance.ChangeGameState(EGameState.Loading, loadingArgBox2);
				}
			}
			else
			{
				ArgumentBox argBox3 = EasyPool.Get<ArgumentBox>();
				argBox3.Set("AnimToShowMask", true);
				argBox3.Set("AnimTime", 1f);
				argBox3.Set("HideAfterShow", true);
				UIElement.BlackMask.SetOnInitArgs(argBox3);
				UIElement.BlackMask.Show();
				AudioManager.Instance.PlayAmbience("Ambience_map_8", 0.2f, 100);
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
				{
					SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(138, delegate(int progress)
					{
						bool flag2 = progress >= 100;
						if (flag2)
						{
							TaiwuEventDomainMethod.Call.TriggerListener("MainStoryHeroicWords", true);
							AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0.2f, 100);
						}
					}));
				});
			}
		}
		else
		{
			ArgumentBox loadingArgBox3 = EasyPool.Get<ArgumentBox>();
			loadingArgBox3.SetObject("OnLoadingStart", new Action(delegate
			{
				SingletonObject.getInstance<YieldHelper>().StartCoroutine(worldMap.QuickTravel(137, delegate(int progress)
				{
					GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", progress));
				}));
			}));
			loadingArgBox3.SetObject("OnLoadingFinish", new Action(delegate
			{
				TaiwuEventDomainMethod.Call.TriggerListener("MainstoryMeetMapRiver", true);
				GameApp.Instance.ChangeGameState(EGameState.InGame, null);
			}));
			GameApp.Instance.ChangeGameState(EGameState.Loading, loadingArgBox3);
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			bool flag = !settings.HasActivatedQuickBeginning;
			if (flag)
			{
				settings.SetQuickStartGame(true);
			}
		}
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0004DF84 File Offset: 0x0004C184
	private static void HandleDisplayEvent_LifeSkillCombatForceSilentResult(RawDataPool dataPool, int argsOffset)
	{
		sbyte silentType = -1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref silentType);
		bool result = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref result);
		sbyte behaviorType = -1;
		Serializer.Deserialize(dataPool, argsOffset, ref behaviorType);
		bool flag = silentType >= 0;
		if (flag)
		{
			SingletonObject.getInstance<LifeSkillCombatModel>().SetForceSilentResult(silentType, result, behaviorType);
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0004DFD5 File Offset: 0x0004C1D5
	private static void HandleDisplayEvent_StopAutoStartCombat()
	{
		GEvent.OnEvent(UiEvents.StopAutoStartCombat, null);
		SingletonObject.getInstance<TutorialChapterModel>().WaitOpenCharacterNeili = true;
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0004DFF4 File Offset: 0x0004C1F4
	private static void HandleDisplayEvent_StartSelectSoul(RawDataPool dataPool, int argsOffset)
	{
		List<int> charList = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charList);
		sbyte selectCount = 1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref selectCount);
		bool isDead = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isDead);
		SelectCharacterConfigHelper.ShowSelectCharacterInstantUI((int)selectCount, null, null, delegate(List<int> selectedList)
		{
			SelectCharacterConfigHelper.TriggerEvent(true, selectCount > 1 || selectCount == 0, selectedList);
		}, charList, true, true);
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0004E05B File Offset: 0x0004C25B
	private static void HandleDisplayEvent_OpenChangeWeaponTrick()
	{
		UIManager.Instance.ShowUI(UIElement.ChangeWeaponTrick, true);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0004E070 File Offset: 0x0004C270
	private static void HandleDisplayEvent_OpenInvestCaravan()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.Set("IsInvest", true);
		UIElement.MerchantInfo.SetOnInitArgs(args);
		UIManager.Instance.MaskUI(UIElement.MerchantInfo);
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x0004E0AD File Offset: 0x0004C2AD
	private static void HandleDisplayEvent_OpenMakeMedicine()
	{
		UIManager.Instance.ShowUI(UIElement.MakeMedicine, true);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0004E0C4 File Offset: 0x0004C2C4
	private static void HandleDisplayEvent_ConfirmProfessionSkillExecute(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass78_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass78_0();
		CS$<>8__locals1.professionSkillArg = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref CS$<>8__locals1.professionSkillArg);
		int num = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref num);
		bool skipConfirm = CS$<>8__locals1.professionSkillArg.SkipConfirm;
		if (skipConfirm)
		{
			ProfessionData profession = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(CS$<>8__locals1.professionSkillArg.ProfessionId);
			int skillIndex = profession.GetSkillIndex(CS$<>8__locals1.professionSkillArg.SkillId);
			ProfessionSkillController.StartShowSkillAnim(CS$<>8__locals1.professionSkillArg, skillIndex, new Action(CS$<>8__locals1.<HandleDisplayEvent_ConfirmProfessionSkillExecute>g__ConfirmExecuteSkill|0), num, CS$<>8__locals1.professionSkillArg.SkillId == 46, null);
		}
		else
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			argumentBox.SetObject("ProfessionSkillArg", CS$<>8__locals1.professionSkillArg);
			argumentBox.Set("BeggarMoneyCount", num);
			UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
			UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0004E1B0 File Offset: 0x0004C3B0
	private static void HandleDisPlayEvent_ConfirmSkillExecuteAndPlayAnim(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass79_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass79_0();
		CS$<>8__locals1.professionSkillArg = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref CS$<>8__locals1.professionSkillArg);
		CS$<>8__locals1.isFinished = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref CS$<>8__locals1.isFinished);
		ProfessionData professionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(CS$<>8__locals1.professionSkillArg.ProfessionId);
		int skillIndex = professionData.GetSkillIndex(CS$<>8__locals1.professionSkillArg.SkillId);
		ProfessionSkillController.StartShowSkillAnim(CS$<>8__locals1.professionSkillArg, skillIndex, new Action(CS$<>8__locals1.<HandleDisPlayEvent_ConfirmSkillExecuteAndPlayAnim>g__ConfirmExecuteSkill|0), 0, false, null);
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0004E23C File Offset: 0x0004C43C
	private static void HandlerDisplayEvent_UltimateSelectCharacterForDirectSamsaraMother(RawDataPool dataPool, int argsOffset)
	{
		List<CharacterDisplayDataForDirectSamsaraMother> displayData = new List<CharacterDisplayDataForDirectSamsaraMother>();
		Serializer.Deserialize(dataPool, argsOffset, ref displayData);
		List<ISelectCharacterData> selectList = (from item in displayData
		select new DirectSamsaraMotherCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
		CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
		config.Title = ProfessionSkill.Instance.GetItem(27).Name;
		config.InteractionMode = ESelectCharacterInteractionMode.Slot;
		config.SelectionMode = ESelectCharacterSelectionMode.Single;
		config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
		{
			ESelectCharacterFilterMenuId.Taiwu,
			ESelectCharacterFilterMenuId.Location
		};
		config.MinSelectionCount = 0;
		config.ConfirmInteractableChecker = ((IReadOnlyList<int> x) => x.Count > 0);
		UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(delegate(List<int> v)
		{
			int charId = v[0];
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg(" UltimateSelectCharOver", "MotherId", charId);
		})).SetObject("SelectCharacterCancelCallback", new Action(delegate
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg(" UltimateSelectCharOver", "CancelSelect", true);
		})));
		UIElement selectChar = UIElement.SelectChar;
		selectChar.OnHide = (Action)Delegate.Combine(selectChar.OnHide, new Action(delegate()
		{
			TaiwuEventDomainMethod.Call.TriggerListener(" UltimateSelectCharOver", true);
		}));
		UIManager.Instance.MaskUI(UIElement.SelectChar);
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0004E3C4 File Offset: 0x0004C5C4
	private static void HandlerDisplayEvent_OpenProfessionSkillUnlocked(RawDataPool dataPool, int argsOffset)
	{
		int skillTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref skillTemplateId);
		bool notInProcess = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref notInProcess);
		UI_ProfessionSkillUnlocked.ProfessionSkillUnlockedEnqueue(skillTemplateId);
		bool flag = !notInProcess;
		if (!flag)
		{
			bool flag2 = UIManager.Instance.IsFocusElement(UIElement.WorldMap);
			if (flag2)
			{
				GEvent.OnEvent(UiEvents.ProfessionSkillUnlock, null);
			}
			else
			{
				bool flag3 = UIManager.Instance.IsFocusElement(UIElement.Profession);
				if (flag3)
				{
					SingletonObject.getInstance<ProfessionModel>().TryOpenProfessionSkillUnlockedInProfessionView();
				}
			}
		}
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0004E448 File Offset: 0x0004C648
	private static void HandlerDisplayEvent_SetDisableMoving(RawDataPool dataPool, int argsOffset)
	{
		bool disableMove = false;
		Serializer.Deserialize(dataPool, argsOffset, ref disableMove);
		ViewWorldMap.SetDisableMoving(disableMove);
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x0004E46C File Offset: 0x0004C66C
	[Obsolete]
	private static void TryOpenProfessionSkillUnlocked(int skillId)
	{
		bool ignoreProfessionSkillUnlockAnimation = GMFunc.IgnoreProfessionSkillUnlockAnimation;
		if (!ignoreProfessionSkillUnlockAnimation)
		{
			UIElement waitingElement = null;
			bool exist = UIElement.ProfessionMask.Exist;
			if (exist)
			{
				waitingElement = UIElement.ProfessionMask;
			}
			else
			{
				bool exist2 = UIElement.CollectResource.Exist;
				if (exist2)
				{
					waitingElement = UIElement.CollectResource;
				}
				else
				{
					bool exist3 = UIElement.GetItem.Exist;
					if (exist3)
					{
						waitingElement = UIElement.GetItem;
					}
				}
			}
			bool flag = waitingElement != null;
			if (flag)
			{
				UIElement uielement = waitingElement;
				uielement.OnDeActive = (Action)Delegate.Combine(uielement.OnDeActive, new Action(delegate()
				{
					DisplayEventHandler.TryOpenProfessionSkillUnlocked(skillId);
				}));
			}
			else
			{
				bool exist4 = UIElement.ProfessionSkillUnlocked.Exist;
				if (!exist4)
				{
					UIElement.ProfessionSkillUnlocked.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("templateId", skillId));
					UIManager.Instance.MaskUI(UIElement.ProfessionSkillUnlocked);
				}
			}
		}
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x0004E554 File Offset: 0x0004C754
	private static void HandlerDisplayEvent_SetEventLockInputState(RawDataPool dataPool, int argsOffset)
	{
		bool state = false;
		Serializer.Deserialize(dataPool, argsOffset, ref state);
		SingletonObject.getInstance<EventModel>().SetLockInputState(state);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0004E57A File Offset: 0x0004C77A
	private static void HandlerDisplayEvent_EventHandleComplete()
	{
		GEvent.OnEvent(UiEvents.OnEventHandleComplete, null);
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x0004E58C File Offset: 0x0004C78C
	private static void HandlerDisplayEvent_SectMainStoryEndScroll(RawDataPool dataPool, int argsOffset)
	{
		short orgTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref orgTemplateId);
		bool isGoodEnd = false;
		Serializer.Deserialize(dataPool, argsOffset, ref isGoodEnd);
		UIElement.AreaStoryScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("orgTemplateId", (sbyte)orgTemplateId).Set("prosper", isGoodEnd).Set("showTip", true));
		UIManager.Instance.ShowUI(UIElement.AreaStoryScroll, true);
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x0004E5F8 File Offset: 0x0004C7F8
	private static void HandlerDisplayEvent_FindTreasureMaterial()
	{
		GEvent.OnEvent(UiEvents.OnBrokenMaterialEventInvoked, null);
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0004E60C File Offset: 0x0004C80C
	private static void HandlerDisplayEvent_CloseCharacterMenu()
	{
		UIManager.Instance.HideUI(UIElement.CharacterMenu);
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0004E61F File Offset: 0x0004C81F
	private static void HandlerDisplayEvent_OpenKongsangSelectMapArea()
	{
		UIElement.SelectKongsangArea.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SelectConfirmContent", LanguageKey.Event_SectStoryKongsang_SelectAreaTip.Tr()));
		UIManager.Instance.ShowUI(UIElement.SelectKongsangArea, true);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0004E658 File Offset: 0x0004C858
	private static void HandlerDisplayEvent_HideBuildingArea()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.BuildingArea);
		if (flag)
		{
			UIManager.Instance.StackBack(UIElement.EventWindow);
		}
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x0004E689 File Offset: 0x0004C889
	private static void HandlerDisplayEvent_WorldMapDoShakeByEvent()
	{
		GEvent.OnEvent(UiEvents.WorldMapDoShakeByEvent, null);
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0004E6A0 File Offset: 0x0004C8A0
	private static void HandlerDisplayEvent_WorldMapDoFocusByEvent(RawDataPool dataPool, int argsOffset)
	{
		Location location = default(Location);
		float duration = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref location);
		Serializer.Deserialize(dataPool, argsOffset, ref duration);
		GEvent.OnEvent(UiEvents.WorldMapDoFocusByEvent, EasyPool.Get<ArgumentBox>().Set<Location>("location", location).Set("duration", duration));
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0004E6FE File Offset: 0x0004C8FE
	private static void HandlerDisplayEvent_OpenSectShaolinDemonSlayer()
	{
		UIManager.Instance.ShowUI(UIElement.SectShaolinDemonSlayer, true);
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0004E712 File Offset: 0x0004C912
	private static void HandlerDisplayEvent_OpenModifyBook()
	{
		UIElement.ModifyBook.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
		UIManager.Instance.ShowUI(UIElement.ModifyBook, true);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0004E736 File Offset: 0x0004C936
	private static void HandlerDisplayEvent_OpenUpgradeTeammateCommand()
	{
		UIManager.Instance.ShowUI(UIElement.UpgradeTeammateCommand, true);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0004E74C File Offset: 0x0004C94C
	private static void HandlerDisplayEvent_OpenDefendHeavenlyTree(RawDataPool dataPool, int argsOffset)
	{
		Location location = Location.Invalid;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref location);
		bool includeGrownTree = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref includeGrownTree);
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set<Location>("Location", location).Set("IncludeGrownTree", includeGrownTree);
		UIElement.DefendHeavenlyTree.SetOnInitArgs(args);
		UIManager.Instance.ShowUI(UIElement.DefendHeavenlyTree, true);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0004E7B5 File Offset: 0x0004C9B5
	private static void HandlerDisplayEvent_ForceClearLifeRecordCache()
	{
		SingletonObject.getInstance<LifeRecordsController>().ClearAllCache();
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0004E7C4 File Offset: 0x0004C9C4
	private static void HandlerDisplayEvent_ShowDialogFromEvent(RawDataPool dataPool, int argsOffset)
	{
		Regex langRegex = new Regex("<Language Key( +)?=( +)?(?<langKey>[A-Z|a-z|0-9|_]+)( +)?/>");
		string title = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref title);
		bool flag = !string.IsNullOrEmpty(title) && langRegex.IsMatch(title);
		if (flag)
		{
			title = langRegex.Replace(title, (Match match) => LocalStringManager.Get(match.Groups["langKey"].Value));
		}
		string content = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref content);
		bool flag2 = !string.IsNullOrEmpty(content) && langRegex.IsMatch(content);
		if (flag2)
		{
			content = langRegex.Replace(content, (Match match) => LocalStringManager.Get(match.Groups["langKey"].Value));
		}
		sbyte type = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref type);
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Type = type;
		dialogCmd.Title = title;
		dialogCmd.Content = content.ColorReplace();
		dialogCmd.Yes = delegate()
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("DialogChoiceMade", "Confirm", true);
			TaiwuEventDomainMethod.Call.TriggerListener("DialogChoiceMade", true);
		};
		dialogCmd.No = delegate()
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("DialogChoiceMade", "Confirm", false);
			TaiwuEventDomainMethod.Call.TriggerListener("DialogChoiceMade", true);
		};
		DialogCmd cmd = dialogCmd;
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x0004E924 File Offset: 0x0004CB24
	private static void HandlerDisplayEvent_CheckSensitiveWord(RawDataPool dataPool, int argsOffset)
	{
		string checkStr = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref checkStr);
		List<SensitiveWordsMatchResult> list = SensitiveWordsSystem.Instance.TryMatch(checkStr, 10);
		bool result = list != null && list.Count > 0;
		TaiwuEventDomainMethod.Call.TriggerListener("CheckSensitiveWord", result);
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0004E96C File Offset: 0x0004CB6C
	private static void HandlerDisplayEvent_OpenCityPunishmentSeverityCustomizeUI(RawDataPool dataPool, int argsOffset)
	{
		sbyte stateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref stateId);
		bool isSect = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isSect);
		int allowRange = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref allowRange);
		int allowOverdriveCount = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref allowOverdriveCount);
		bool hasVillagerHead = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref hasVillagerHead);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("IsCustomizeSectLaw", true);
		argBox.Set("StateTemplateId", stateId);
		argBox.Set("IsSect", isSect);
		argBox.Set("AllowRange", allowRange);
		argBox.Set("AllowOverdriveCount", allowOverdriveCount);
		argBox.Set("HasVillagerHead", hasVillagerHead);
		UIElement.SectLaw.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SectLaw, true);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0004EA40 File Offset: 0x0004CC40
	private static void HandlerDisplayEvent_StartDoctorHeal(RawDataPool dataPool, int argsOffset)
	{
		int doctorCharId = 0;
		argsOffset += Serializer.Deserialize(dataPool2, argsOffset, ref doctorCharId);
		bool expensiveHeal = false;
		argsOffset += Serializer.Deserialize(dataPool2, argsOffset, ref expensiveHeal);
		bool flag = doctorCharId >= 0;
		if (flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			List<int> doctorList = new List<int>();
			List<int> patientList = new List<int>();
			doctorList.Add(doctorCharId);
			DisplayTriggerModel model = SingletonObject.getInstance<DisplayTriggerModel>();
			patientList.AddRange(model.GroupCharIds.GetCollection());
			argBox.SetObject("DoctorList", doctorList);
			argBox.SetObject("PatientList", patientList);
			argBox.Set("NeedPay", true);
			argBox.Set("ExpensiveHeal", expensiveHeal);
			CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				KidnappedCharacterList kidnappedCharacterList = null;
				Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
				bool flag2 = kidnappedCharacterList != null;
				if (flag2)
				{
					for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
					{
						patientList.Add(kidnappedCharacterList.Get(i).CharId);
					}
				}
				UIElement.Heal.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.Heal, true);
			});
		}
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0004EB38 File Offset: 0x0004CD38
	private static void HandlerDisplayEvent_StartHeal(RawDataPool dataPool, int argsOffset)
	{
		List<int> doctorList = new List<int>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref doctorList);
		List<int> patientList = new List<int>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref patientList);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("DoctorList", doctorList);
		argBox.SetObject("PatientList", patientList);
		argBox.Set("NeedPay", false);
		argBox.Set("ExpensiveHeal", false);
		UIElement.Heal.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.Heal, true);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0004EBC4 File Offset: 0x0004CDC4
	private static void HandlerDisplayEvent_SelectCombatSkill(RawDataPool dataPool, int argsOffset)
	{
		int charId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charId);
		List<short> skillList = new List<short>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref skillList);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharId", charId);
		argBox.Set("ShowCombatSkill", true);
		argBox.Set("CheckEquipRequirePracticeLevel", false);
		argBox.Set("ShowNone", false);
		argBox.SetObject("UnselectableCombatSkillList", new List<short>());
		argBox.SetObject("CombatSkillIdList", skillList);
		argBox.SetObject("Callback", new Action<sbyte, short>(delegate(sbyte _, short skillId)
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("CombatSkillId", skillId);
			GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
		}));
		argBox.Set("IsShowNeiLiFinish", false);
		UIElement.SelectSkill.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.SelectSkill);
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0004ECA0 File Offset: 0x0004CEA0
	private static void HandleDisplayEvent_ShowNormalDialog(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass104_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass104_0();
		CS$<>8__locals1.argsOffset = argsOffset;
		CS$<>8__locals1.dataPool = dataPool;
		bool flag = UIManager.Instance.IsElementActive(UIElement.Loading);
		if (flag)
		{
			DisplayEventHandler._pendingNormalDialogActions.Enqueue(new Action(CS$<>8__locals1.<HandleDisplayEvent_ShowNormalDialog>g__Action|0));
		}
		else
		{
			CS$<>8__locals1.<HandleDisplayEvent_ShowNormalDialog>g__Action|0();
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0004ECF8 File Offset: 0x0004CEF8
	private static void HandleDisplayEvent_OpenProfessionSkillSpecial(RawDataPool dataPool, int argsOffset)
	{
		int templateId = -1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref templateId);
		int num = templateId;
		int num2 = num;
		if (num2 <= 39)
		{
			if (num2 <= 27)
			{
				if (num2 != 3)
				{
					if (num2 == 27)
					{
						DisplayEventHandler.HandleDisplayEvent_BuddhistMonkSkill3(dataPool, argsOffset);
					}
				}
				else
				{
					List<ItemDisplayData> itemList = new List<ItemDisplayData>();
					argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref itemList);
					DisplayEventHandler.HandleDisplayEvent_OpenItemSelectForSavageSkill3(itemList);
				}
			}
			else if (num2 != 31)
			{
				if (num2 == 39)
				{
					UIManager.Instance.ShowUI(UIElement.BeggarSkill3, true);
				}
			}
			else
			{
				DisplayEventHandler.HandleDisplayEvent_OpenMultiSelectSkillBook(true);
			}
		}
		else if (num2 <= 47)
		{
			if (num2 != 46)
			{
				if (num2 == 47)
				{
					DisplayEventHandler.HandleDisplayEvent_OpenTravelerSkill3();
				}
			}
			else
			{
				DisplayEventHandler.HandleDisplayEvent_OpenTravelerSkill2();
			}
		}
		else if (num2 != 51)
		{
			if (num2 == 67)
			{
				DisplayEventHandler.HandleDisplayEvent_OpenMultiSelectSkillBook(false);
			}
		}
		else
		{
			DisplayEventHandler.HandleDisplayEvent_TravelingBuddhistMonkSkill3();
		}
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x0004EDCC File Offset: 0x0004CFCC
	private static void HandleDisplayEvent_TravelingBuddhistMonkSkill3()
	{
		List<short> featureList = new List<short>
		{
			252,
			253,
			254,
			255,
			256,
			257,
			258,
			259,
			260,
			261
		};
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("featureList", featureList);
		argBox.Set("texName", "tex_profession_travelingbuddhistmonk_1");
		string content = LocalStringManager.Get(LanguageKey.Event_ProfessionTravelingBuddhistMonkSkill3Text0);
		argBox.Set("desc", content);
		argBox.SetObject("callBack", new Action<short>(DisplayEventHandler.<HandleDisplayEvent_TravelingBuddhistMonkSkill3>g__OnConfirm|106_0));
		UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0004EECC File Offset: 0x0004D0CC
	private static void HandleDisplayEvent_BuddhistMonkSkill3(RawDataPool pool, int offset)
	{
		sbyte fameType = -1;
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		CharacterDomainMethod.AsyncCall.GetFameType(null, taiwuId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref fameType);
			List<short> featureList = new List<short>
			{
				242,
				243,
				244,
				245,
				246,
				247,
				248,
				249,
				250,
				251
			};
			for (int i = featureList.Count - 1; i >= 0; i--)
			{
				bool flag = !base.<HandleDisplayEvent_BuddhistMonkSkill3>g__CheckCanSelectFeature|2(featureList[i]);
				if (flag)
				{
					featureList.RemoveAt(i);
				}
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("texName", "tex_profession_buddhistmonk_1");
			argBox.SetObject("callBack", new Action<short>(DisplayEventHandler.<HandleDisplayEvent_BuddhistMonkSkill3>g__OnConfirm|107_1));
			argBox.SetObject("featureList", featureList);
			argBox.Set("canExit", false);
			string taiwuName = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
			string content = LocalStringManager.GetFormat(LanguageKey.Event_BuddhistMonkSkill3_SelectFeature, taiwuName, taiwuName, taiwuName);
			argBox.Set("desc", content);
			UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
		});
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0004EF08 File Offset: 0x0004D108
	private static void HandleDisplayEvent_OpenTravelerSkill2()
	{
		GEvent.OnEvent(UiEvents.ProfessionTravelerSkillTwoStart, null);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0004EF1C File Offset: 0x0004D11C
	private static void HandleDisplayEvent_OpenTravelerSkill3()
	{
		UIManager.Instance.MaskUI(UIElement.ProfessionTravelerStation);
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0004EF30 File Offset: 0x0004D130
	private static void HandleDisplayEvent_OpenMultiSelectSkillBook(bool isCombatSkillBook)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		SelectItemConfig config = SelectItemConfig.CreateMultipleSelectConfig(new SelectItemRules
		{
			ItemSubType = (isCombatSkillBook ? 1001 : 1000),
			OnlyFromInventory = false
		}, delegate(List<SelectedItemData> itemSelected)
		{
			DisplayEventHandler.OpenMultiSelectCharWindow(itemSelected, isCombatSkillBook);
		}, "", GlobalConfig.Instance.TeachSkillBookSelctMaxCount, 1, new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Value | ESelectItemColumnType.Weight | ESelectItemColumnType.Durability | ESelectItemColumnType.BookReadingInfo));
		config.ShowQuickButton = false;
		config.IsAllowSameTemplateIdItem = false;
		config.ShowQuickButton = true;
		UIElement.SelectItem.SetOnInitArgs(argBox.SetObject("SelectItemConfig", config));
		UIManager.Instance.MaskUI(UIElement.SelectItem);
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0004EFE4 File Offset: 0x0004D1E4
	private static void OpenMultiSelectCharWindow(List<SelectedItemData> selectedItems, bool isCombatSkillBook)
	{
		HashSet<int> charIdSet = new HashSet<int>();
		MapBlockData block = SingletonObject.getInstance<WorldMapModel>().CurrentBlockData;
		bool flag = block.CharacterSet != null;
		if (flag)
		{
			charIdSet.UnionWith(block.CharacterSet);
		}
		charIdSet.UnionWith(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds());
		List<int> selectionList = charIdSet.ToList<int>();
		selectionList.Remove(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		List<CharacterDisplayDataForTasterUltimate> displayData = new List<CharacterDisplayDataForTasterUltimate>();
		SelectCharacterCallback <>9__3;
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForTasterUltimate(null, (from x in selectedItems
		select x.ItemData.Key).ToList<ItemKey>(), delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref displayData);
			for (int i = displayData.Count - 1; i >= 0; i--)
			{
				bool flag2 = AgeGroup.GetAgeGroup(displayData[i].CharacterData.PhysiologicalAge) < 1;
				if (flag2)
				{
					displayData.RemoveAt(i);
				}
			}
			List<ISelectCharacterData> selectList = (from item in displayData
			select new TasterUltimateCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.TasterUltimateSkill);
			config.CustomColumnGenerator = new Dictionary<ESelectCharacterSubPage, Func<IEnumerable<ColumnDefinition>>>();
			config.CustomColumnGenerator[ESelectCharacterSubPage.TasterUltimateSkill] = new Func<IEnumerable<ColumnDefinition>>(DisplayEventHandler.GenerateColumns);
			config.InteractionMode = ESelectCharacterInteractionMode.Slot;
			config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
			config.TargetCount = GlobalConfig.Instance.TeachSkillCharacterMaxCount;
			config.MinSelectionCount = 1;
			config.InfoText = LanguageKey.LK_TeachCombatSkill_SelectChar_Notice.Tr();
			UIElement selectChar = UIElement.SelectChar;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList);
			string key = "SelectCharacterCallback";
			SelectCharacterCallback arg;
			if ((arg = <>9__3) == null)
			{
				arg = (<>9__3 = delegate(List<int> v)
				{
					v.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
					DisplayEventHandler.OpenProfessionSkillConfirm(selectedItems, v, isCombatSkillBook);
				});
			}
			selectChar.SetOnInitArgs(argumentBox.SetObject(key, arg));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		});
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0004F0A9 File Offset: 0x0004D2A9
	private static IEnumerable<ColumnDefinition> GenerateColumns()
	{
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(1));
		columnDefinition.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(0, new ReadProgressData(-1, null, null, false)));
		columnDefinition.SortId = -1;
		yield return columnDefinition;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition2.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(2));
		columnDefinition2.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(1, new ReadProgressData(-1, null, null, false)));
		columnDefinition2.SortId = -1;
		yield return columnDefinition2;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition3.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(3));
		columnDefinition3.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(2, new ReadProgressData(-1, null, null, false)));
		columnDefinition3.SortId = -1;
		yield return columnDefinition3;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition4.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(4));
		columnDefinition4.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(3, new ReadProgressData(-1, null, null, false)));
		columnDefinition4.SortId = -1;
		yield return columnDefinition4;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition5.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(5));
		columnDefinition5.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(4, new ReadProgressData(-1, null, null, false)));
		columnDefinition5.SortId = -1;
		yield return columnDefinition5;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition6.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(6));
		columnDefinition6.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(5, new ReadProgressData(-1, null, null, false)));
		columnDefinition6.SortId = -1;
		yield return columnDefinition6;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition7 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition7.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(7));
		columnDefinition7.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(6, new ReadProgressData(-1, null, null, false)));
		columnDefinition7.SortId = -1;
		yield return columnDefinition7;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition8 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition8.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(8));
		columnDefinition8.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(7, new ReadProgressData(-1, null, null, false)));
		columnDefinition8.SortId = -1;
		yield return columnDefinition8;
		ColumnDefinition<ISelectCharacterData, ReadProgressData> columnDefinition9 = new ColumnDefinition<ISelectCharacterData, ReadProgressData>();
		columnDefinition9.LayoutOption = new LayoutOption
		{
			MinWidth = 200f,
			FlexibleWidth = 1f,
			PreferredWidth = 200f,
			Priority = 1
		};
		columnDefinition9.TableHeadLabel = (() => LanguageKey.LK_Profession_Book.TrFormat(9));
		columnDefinition9.CellDataGenerator = ((ISelectCharacterData data) => (data as TasterUltimateCharacterDataAdapter).Data.ReadProgressList.GetOrDefault(8, new ReadProgressData(-1, null, null, false)));
		columnDefinition9.SortId = -1;
		yield return columnDefinition9;
		yield break;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0004F0B4 File Offset: 0x0004D2B4
	private static void OpenProfessionSkillConfirm(List<SelectedItemData> selectedItems, List<int> charIds, bool isCombatSkillBook)
	{
		List<int> bookIds = new List<int>();
		foreach (ItemKey bookIdItemKey in (from x in selectedItems
		select x.ItemData.Key).ToList<ItemKey>())
		{
			bookIds.Add(bookIdItemKey.Id);
		}
		if (isCombatSkillBook)
		{
			ViewTeachCombatSkillResultConfirm.SetBookItemKeys((from item in selectedItems
			where item != null
			select item.ItemData.RealKey).ToList<ItemKey>());
		}
		else
		{
			ViewTeachLifeSkillResultConfirm.SetBookItemKeys((from item in selectedItems
			where item != null
			select item.ItemData.RealKey).ToList<ItemKey>());
		}
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = (isCombatSkillBook ? 7 : 16),
			SkillId = (isCombatSkillBook ? 31 : 67),
			IsSuccess = true,
			CharIds = charIds,
			BookIds = bookIds
		};
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0004F264 File Offset: 0x0004D464
	private static void HandleDisplayEvent_OpenTravelingTaoistMonkSkill2(RawDataPool dataPool, int argsOffset)
	{
		int id = -1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref id);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("characterId", id);
		UIElement.TravelingTaoistMonkSkill2.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.TravelingTaoistMonkSkill2);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0004F2B0 File Offset: 0x0004D4B0
	private static void HandleDisplayEvent_OpenSelectSecretInformationLiteratiSkill2(RawDataPool dataPool, int argsOffset)
	{
		SecretInformationDisplayPackage displayPackage = new SecretInformationDisplayPackage();
		Serializer.Deserialize(dataPool, argsOffset, ref displayPackage);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("secretInformation", displayPackage);
		argBox.Set("SelectType", 2);
		argBox.SetObject("callback", new Action<SecretInformationDisplayData>(delegate(SecretInformationDisplayData secret)
		{
			DisplayEventHandler.<>c__DisplayClass115_1 CS$<>8__locals2 = new DisplayEventHandler.<>c__DisplayClass115_1();
			CS$<>8__locals2.secret = secret;
			bool flag = !CS$<>8__locals2.secret.SecretInformationId.Valid;
			if (!flag)
			{
				argBox.Clear();
				ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
				{
					ProfessionId = 4,
					SkillId = 18,
					CharId = -1,
					ItemKey = ItemKey.Invalid,
					IsSuccess = true
				};
				argBox.SetObject("ProfessionSkillArg", professionSkillArg);
				argBox.SetObject("OnConfirm", new Action(CS$<>8__locals2.<HandleDisplayEvent_OpenSelectSecretInformationLiteratiSkill2>g__OnProfessionSkillConfirm|1));
				UIElement.ProfessionSkillConfirm.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
			}
		}));
		UIElement.SelectSecretInformation.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.SelectSecretInformation);
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0004F344 File Offset: 0x0004D544
	private static void HandleDisplayEvent_OpenSelectNormalInformationLiteratiSkill3(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass116_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass116_0();
		CS$<>8__locals1.argBox = EasyPool.Get<ArgumentBox>();
		CS$<>8__locals1.argBox.SetObject("callback", new Action<NormalInformation>(CS$<>8__locals1.<HandleDisplayEvent_OpenSelectNormalInformationLiteratiSkill3>g__OnConfirmNormalInformation|0));
		CS$<>8__locals1.argBox.Set("IsLiteratiSkill3", true);
		UIElement.SelectInformation.SetOnInitArgs(CS$<>8__locals1.argBox);
		UIManager.Instance.MaskUI(UIElement.SelectInformation);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0004F3B4 File Offset: 0x0004D5B4
	private static void HandleDisplayEvent_OpenSelectCricketDukeSkill2(RawDataPool dataPool, int argsOffset)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		List<ItemDisplayData> cricketList = new List<ItemDisplayData>();
		Serializer.Deserialize(dataPool, argsOffset, ref cricketList);
		SelectItemConfig config = SelectItemConfig.CreateSingleSelectNoPreSelectConfig(new SelectItemRules
		{
			OnlyFromInventory = true
		}, delegate(List<SelectedItemData> itemSelected)
		{
			bool flag = itemSelected != null && itemSelected.Count > 0;
			if (flag)
			{
				SelectedItemData selectedItem = itemSelected[0];
				bool flag2 = !selectedItem.IsCancelled && selectedItem.ItemData != null;
				if (flag2)
				{
					base.<HandleDisplayEvent_OpenSelectCricketDukeSkill2>g__OnClickSelectCricket|1(selectedItem.ItemData.Key);
				}
			}
		}, ProfessionSkill.Instance.GetItem(70).Name, null);
		config.MaxSelectCount = 1;
		config.OperationMode = ESelectItemOperationMode.NoPreSelect;
		config.ExternalItems = cricketList;
		config.InitialSelectedItems = new List<SelectedItemData>();
		config.HideSourceToggles = true;
		argBox.SetObject("SelectItemConfig", config);
		UIElement.SelectItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.SelectItem);
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0004F47C File Offset: 0x0004D67C
	private static void HandleDisplayEvent_OpenItemSelectForSavageSkill3(List<ItemDisplayData> itemList)
	{
		SelectItemRules selectItemRules = new SelectItemRules();
		selectItemRules.OnlyFromInventory = true;
		SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(selectItemRules, delegate(List<SelectedItemData> itemSelected)
		{
			DisplayEventHandler.<HandleDisplayEvent_OpenItemSelectForSavageSkill3>g__OnSelectItem|118_1(itemSelected[0].ItemData.Key);
		}, "", null);
		config.MaxSelectCount = 1;
		config.InitialSelectedItems = new List<SelectedItemData>
		{
			new SelectedItemData(itemList[0], 1)
		};
		config.ExternalItems = itemList;
		config.ShowQuickButton = false;
		config.OperationMode = ESelectItemOperationMode.Slot;
		config.SelectItemMode = ESelectItemMode.ItemSelect;
		config.CanSelectLockedItem = true;
		config.HideSourceToggles = true;
		UIElement.SelectItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectItemConfig", config));
		UIManager.Instance.MaskUI(UIElement.SelectItem);
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0004F54C File Offset: 0x0004D74C
	private static void HandleDisplayEvent_OpenTasterUltimateResult(RawDataPool dataPool, int argsOffset)
	{
		TasterUltimateResult result = new TasterUltimateResult();
		bool isCombat = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isCombat);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref result);
		DisplayEventHandler.OpenTeachSkillResultConfirm(isCombat, result);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0004F584 File Offset: 0x0004D784
	public static void OpenTeachSkillResultConfirm(bool isCombat, TasterUltimateResult tasterUltimateResult)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.SetObject("TasterUltimateResult", tasterUltimateResult);
		if (isCombat)
		{
			UIElement.TeachCombatSkillResultConfirm.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.TeachCombatSkillResultConfirm);
		}
		else
		{
			UIElement.TeachLifeSkillResultConfirm.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.TeachLifeSkillResultConfirm);
		}
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0004F5E8 File Offset: 0x0004D7E8
	private static void HandleDisplayEvent_TaiwuBeKidnapped(RawDataPool dataPool, int argsOffset)
	{
		Location targetLocation = Location.Invalid;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref targetLocation);
		int hunterCharId = -1;
		Serializer.Deserialize(dataPool, argsOffset, ref hunterCharId);
		bool flag = UIManager.Instance.IsElementActive(UIElement.BuildingArea);
		if (flag)
		{
			UIManager.Instance.StackBack(UIElement.EventWindow);
		}
		bool needTravel = targetLocation.AreaId != SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
		bool flag2 = !UIElement.PartWorld.Exist && needTravel;
		if (flag2)
		{
			UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
		}
		MapDomainMethod.Call.TaiwuBeKidnapped(targetLocation, hunterCharId);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0004F67C File Offset: 0x0004D87C
	private static void HandleDisplayEvent_SelectDemonSlayerCharacterFeature(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass122_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass122_0();
		int levelTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref levelTemplateId);
		DemonSlayerTrialLevelItem levelConfig = DemonSlayerTrialLevel.Instance[levelTemplateId];
		CS$<>8__locals1.taiwuName = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("featureList", levelConfig.RewardFeatureOptions);
		argBox.Set("texName", "tex_sectstory_shaolin_40");
		argBox.Set("confirmNotHide", true);
		argBox.SetObject("callBack", new Action<short>(CS$<>8__locals1.<HandleDisplayEvent_SelectDemonSlayerCharacterFeature>g__OnConfirm|0));
		argBox.SetObject("cancelAction", new Action(CS$<>8__locals1.<HandleDisplayEvent_SelectDemonSlayerCharacterFeature>g__CancelAction|1));
		string content = LocalStringManager.GetFormat(LanguageKey.Event_SectShaolin_SelectFeature, Array.Empty<object>()).ColorReplace();
		argBox.Set("desc", content);
		UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0004F764 File Offset: 0x0004D964
	private static void HandleDisplayEvent_CommonSelectCharacterFeature(RawDataPool dataPool, int argsOffset)
	{
		DisplayEventHandler.<>c__DisplayClass123_0 CS$<>8__locals1 = new DisplayEventHandler.<>c__DisplayClass123_0();
		IntList intlist = IntList.Create();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref intlist);
		CS$<>8__locals1.saveKey = "";
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref CS$<>8__locals1.saveKey);
		string contentKey = "";
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref contentKey);
		string eventTexture = "";
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref eventTexture);
		List<short> featureList = new List<short>();
		foreach (int templateId in intlist.Items)
		{
			featureList.Add((short)templateId);
		}
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("featureList", featureList);
		argBox.Set("texName", eventTexture);
		argBox.SetObject("callBack", new Action<short>(CS$<>8__locals1.<HandleDisplayEvent_CommonSelectCharacterFeature>g__OnConfirm|0));
		argBox.SetObject("cancelAction", new Action(CS$<>8__locals1.<HandleDisplayEvent_CommonSelectCharacterFeature>g__CancelAction|1));
		string content = LocalStringManager.Get(contentKey);
		argBox.Set("desc", content);
		UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0004F8B0 File Offset: 0x0004DAB0
	private static void HandleDisplayEvent_ExtraProfessionSkillUnlocked(RawDataPool dataPool, int argsOffset)
	{
		UI_ProfessionSkillUnlocked.ExtraProfessionSkillUnlockedEnqueue();
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0004F8B9 File Offset: 0x0004DAB9
	private static void HandleDisplayEvent_HidePartWorldMap(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.HidePartWorldMap, null);
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0004F8CC File Offset: 0x0004DACC
	private static void HandleDisplayEvent_InteractCheckAnimation(RawDataPool dataPool, int argsOffset)
	{
		bool enableInteractCheckResultAnimation = SingletonObject.getInstance<GlobalSettings>().EnableInteractCheckResultAnimation;
		if (enableInteractCheckResultAnimation)
		{
			EventInteractCheckData eventInteractCheckData = new EventInteractCheckData();
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref eventInteractCheckData);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("InteractData", eventInteractCheckData);
			UIElement.InteractCheckResult.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.InteractCheckResult, true);
		}
		else
		{
			TaiwuEventDomainMethod.Call.EventSelectContinue();
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0004F937 File Offset: 0x0004DB37
	private static void HandlerDisplayEvent_UnlockLifeSkillCombatStrategy(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.CombatLifeSkillUnlockStrategy, null);
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0004F94C File Offset: 0x0004DB4C
	private static void HandlerDisplayEvent_ChangeLifeSkillCombatData(RawDataPool dataPool, int argsOffset)
	{
		DebateGame debateGame = null;
		Serializer.Deserialize(dataPool, argsOffset, ref debateGame);
		ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("DebateGame", debateGame);
		GEvent.OnEvent(UiEvents.ChangeLifeSkillCombatData, args);
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0004F988 File Offset: 0x0004DB88
	private static void HandlerDisplayEvent_PlayMapPickupEffect(RawDataPool dataPool, int argsOffset)
	{
		MapElementPickupDisplayData pickupDisplayData = null;
		Serializer.Deserialize(dataPool, argsOffset, ref pickupDisplayData);
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		EMapPickupsType type = pickupDisplayData.Pickup.Template.Type;
		EMapPickupsType emapPickupsType = type;
		if (emapPickupsType <= EMapPickupsType.Growth)
		{
			Queue<MapElementPickupDisplayData> queue;
			bool flag = !mapModel.MapPickupEffectQueueDict.TryGetValue(pickupDisplayData.Pickup.Location, out queue);
			if (flag)
			{
				queue = new Queue<MapElementPickupDisplayData>();
				mapModel.MapPickupEffectQueueDict[pickupDisplayData.Pickup.Location] = queue;
			}
			queue.Enqueue(pickupDisplayData);
		}
		GEvent.OnEvent(UiEvents.MapPickupEffectChanged, null);
		GEvent.OnEvent(UiEvents.OnTaiwuReadingBookProgressMayChange, null);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0004FA34 File Offset: 0x0004DC34
	private static void HandlerDisplayEvent_OpenCraftsmanPanelUI(RawDataPool dataPool, int argsOffset)
	{
		int charId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charId);
		sbyte craftsmanPanelType = 1;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref craftsmanPanelType);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("charId", charId);
		argBox.Set("craftsmanPanelType", craftsmanPanelType);
		UIElement.Craftsman.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.Craftsman, true);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0004FAA0 File Offset: 0x0004DCA0
	private static void HandlerDisplayEvent_SectMainStoryWudangStart(RawDataPool dataPool, int argsOffset)
	{
		short skillId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref skillId);
		bool flag = !UIElement.CombatBegin.Exist;
		if (flag)
		{
			TaiwuEventDomainMethod.Call.CloseUI("UI_SkillBreakPlate", false, (int)skillId);
		}
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x0004FAE0 File Offset: 0x0004DCE0
	private static void HandlerDisplayEvent_ShowYuanshanMiniGame(RawDataPool dataPool, int argsOffset)
	{
		int stage = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref stage);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref ViewYuanshanMiniGame.Finish);
		Debug.Log(string.Format("Enter Yuanshan mini game with stage {0} and finish {1}", stage, ViewYuanshanMiniGame.Finish));
		uint finish = ViewYuanshanMiniGame.Finish;
		bool flag = finish <= 0U || finish > 3U;
		if (flag)
		{
			ViewYuanshanMiniGame.Finish = 3U;
		}
		UIElement.YuanshanMiniGame.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set(SectMainStoryEventArgKey.DefValue.YuanshanMiniGameStage, stage));
		UIManager.Instance.MaskUI(UIElement.YuanshanMiniGame);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0004FB80 File Offset: 0x0004DD80
	private static void HandlerDisplayEvent_PlayVitalAnim(RawDataPool dataPool, int argsOffset)
	{
		int type = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref type);
		bool isInPrison = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isInPrison);
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("Type", type).Set("IsInPrison", isInPrison);
		GEvent.OnEvent(UiEvents.PlayVitalAnim, args);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0004FBDC File Offset: 0x0004DDDC
	private static void HandlerDisplayEvent_SwitchGuardedPageStatus(RawDataPool dataPool, int argsOffset)
	{
		Debug.Log("HandlerDisplayEvent_SwitchGuardedPageStatus called.");
		ViewSettlementPrison ui = UIElement.SettlementPrison.UiBaseAs<ViewSettlementPrison>();
		bool flag = ui == null;
		if (!flag)
		{
			Debug.Log("HandlerDisplayEvent_SwitchGuardedPageStatus found UI exists");
			sbyte currentPage = 0;
			argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref ui.EnterState);
			Serializer.Deserialize(dataPool, argsOffset, ref currentPage);
			ui.CurrPage = currentPage;
			Debug.Log(string.Format("HandlerDisplayEvent_SwitchGuardedPageStatus returned with ui.EnterState = {0} and currPage = {1}, ui.CurrPage = {2}", ui.EnterState, currentPage, ui.CurrPage));
		}
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0004FC6C File Offset: 0x0004DE6C
	public static void HandlePendingNormalDialogActions()
	{
		while (DisplayEventHandler._pendingNormalDialogActions.Count > 0)
		{
			DisplayEventHandler._pendingNormalDialogActions.Dequeue()();
		}
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0004FCA0 File Offset: 0x0004DEA0
	private static void HandlerDisplayEvent_ShowSectMainStoryUnlock(RawDataPool dataPool, int argsOffset)
	{
		sbyte orgTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref orgTemplateId);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("OrgTemplateId", orgTemplateId);
		UIElement.SectMainStoryUnlock.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.SectMainStoryUnlock, true);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0004FCEC File Offset: 0x0004DEEC
	private static void HandlerDisplayEvent_ShowSectMainStorySpecialInteract(RawDataPool dataPool, int argsOffset)
	{
		sbyte orgTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref orgTemplateId);
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		sbyte b = orgTemplateId;
		sbyte b2 = b;
		if (b2 != 10)
		{
			if (b2 == 15)
			{
				UIElement.Jixi.SetOnInitArgs(args);
				UIManager.Instance.MaskUI(UIElement.Jixi);
			}
		}
		else
		{
			UIElement.KongsangSpecialInteract.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.KongsangSpecialInteract, true);
		}
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0004FD60 File Offset: 0x0004DF60
	private static void HandlerDisplayEvent_ChangeMusicStatus(RawDataPool dataPool, int argsOffset)
	{
		bool isPause = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isPause);
		bool flag = isPause;
		if (flag)
		{
			AudioManager.Instance.MusicPlayer.Pause();
		}
		else
		{
			AudioManager.Instance.MusicPlayer.Resume();
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0004FDA4 File Offset: 0x0004DFA4
	private static void HandlerDisplayEvent_ChangeSoundStatus(RawDataPool dataPool, int argsOffset)
	{
		bool isPause = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isPause);
		bool flag = isPause;
		if (flag)
		{
			AudioManager.Instance.PauseAllSound();
		}
		else
		{
			AudioManager.Instance.ResumeAllSound();
		}
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0004FDE4 File Offset: 0x0004DFE4
	private static void HandlerDisplayEvent_ShowAdventureFinish(RawDataPool dataPool, int argsOffset)
	{
		bool success = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref success);
		bool skipFinishAnim = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref skipFinishAnim);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("Success", success).Set("SkipFinishAnim", skipFinishAnim);
		GEvent.OnEvent(UiEvents.AdventureRemakeFinish, argumentBox);
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0004FE40 File Offset: 0x0004E040
	private static void HandlerDisplayEvent_AdventureElementAlertAnim(RawDataPool dataPool, int argsOffset)
	{
		int elementId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref elementId);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("ElementId", elementId);
		GEvent.OnEvent(UiEvents.AdventureElementAlertAnim, argumentBox);
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0004FE80 File Offset: 0x0004E080
	private static void HandlerDisplayEvent_AdventureBlockChangeIcon(RawDataPool dataPool, int argsOffset)
	{
		List<AdventureBlockIndexForSerialize> blockIndexList = new List<AdventureBlockIndexForSerialize>();
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref blockIndexList);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("BlockIndexList", blockIndexList);
		GEvent.OnEvent(UiEvents.AdventureBlockChangeIcon, argumentBox);
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0004FEC4 File Offset: 0x0004E0C4
	private static void HandlerDisplayEvent_AdventureElementShowHideEffect(RawDataPool dataPool, int argsOffset)
	{
		int elementId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref elementId);
		bool isBlockElement = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isBlockElement);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("ElementId", elementId).Set("IsBlockElement", isBlockElement);
		GEvent.OnEvent(UiEvents.AdventureElementShowHideEffect, argumentBox);
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0004FF1D File Offset: 0x0004E11D
	private static void HandlerDisplayEvent_AdventureRefreshBlockEffect(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.AdventureRefreshBlockEffect, null);
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0004FF31 File Offset: 0x0004E131
	private static void HandlerDisplayEvent_AdventureRefreshGlobalEffect(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.AdventureRefreshGlobalEffect, null);
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0004FF48 File Offset: 0x0004E148
	private static void HandlerDisplayEvent_AdventureTaiwuShowDialog(RawDataPool dataPool, int argsOffset)
	{
		string key = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref key);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("Key", key);
		GEvent.OnEvent(UiEvents.AdventureTaiwuShowDialog, argumentBox);
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0004FF87 File Offset: 0x0004E187
	private static void HandlerDisplayEvent_AdventureEventHandled(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.AdventureEventHandled, null);
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x0004FF9C File Offset: 0x0004E19C
	private static void HandlerDisplayEvent_AdventureElementShowDialog(RawDataPool dataPool, int argsOffset)
	{
		int elementId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref elementId);
		string key = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref key);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("Key", key).Set("ElementId", elementId);
		GEvent.OnEvent(UiEvents.AdventureElementShowDialog, argumentBox);
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0004FFF8 File Offset: 0x0004E1F8
	private static void HandlerDisplayEvent_AdventureElementDeleteAnim(RawDataPool dataPool, int argsOffset)
	{
		int elementCoreId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref elementCoreId);
		int visibleIndex = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref visibleIndex);
		AdventureBlockIndexForSerialize index = default(AdventureBlockIndexForSerialize);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref index);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("ElementCoreId", elementCoreId).Set("VisibleIndex", visibleIndex).SetObject("BlockIndex", index);
		GEvent.OnEvent(UiEvents.AdventureElementDeleteAnim, argumentBox);
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x0005007C File Offset: 0x0004E27C
	private static void HandlerDisplayEvent_AdventureCameraMoveToBlock(RawDataPool dataPool, int argsOffset)
	{
		AdventureBlockIndexForSerialize blockIndex = default(AdventureBlockIndexForSerialize);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref blockIndex);
		bool reset = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref reset);
		float oneWayDuration = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref oneWayDuration);
		float stayDuration = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref stayDuration);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("OneWayDuration", oneWayDuration).Set("StayDuration", stayDuration).Set("CameraReset", reset).SetObject("BlockIndex", blockIndex);
		GEvent.OnEvent(UiEvents.AdventureCameraMoveToBlock, argumentBox);
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x00050124 File Offset: 0x0004E324
	private static void HandlerDisplayEvent_AdventureDelayAction(RawDataPool dataPool, int argsOffset)
	{
		float delayDuration = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref delayDuration);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("DelayDuration", delayDuration);
		GEvent.OnEvent(UiEvents.AdventureDelayAction, argumentBox);
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00050168 File Offset: 0x0004E368
	private static void HandleDisplayEvent_AdventureStartSelectElement(RawDataPool dataPool, int argsOffset)
	{
		AdventureBlockIndexForSerialize blockIndex = default(AdventureBlockIndexForSerialize);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref blockIndex);
		int range = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref range);
		string saveKey = "";
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref saveKey);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("CenterIndex", blockIndex).Set("Range", range).Set("SaveKey", saveKey);
		GEvent.OnEvent(UiEvents.AdventureStartSelectElement, argumentBox);
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x000501F0 File Offset: 0x0004E3F0
	private static void HandlerDisplayEvent_SectStoryPopUpToggle(RawDataPool dataPool, int argsOffset)
	{
		sbyte orgTemplateId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref orgTemplateId);
		int status = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref status);
		ViewSectStoryPopUpToggle viewSectStoryPopUpToggle = UIElement.SectStoryPopUpToggle.UiBaseAs<ViewSectStoryPopUpToggle>();
		bool flag = viewSectStoryPopUpToggle != null && viewSectStoryPopUpToggle.IsTriggeredThisMonth(orgTemplateId);
		if (flag)
		{
			TaiwuEventDomainMethod.Call.TriggerListener("SectStoryPopUpToggle", true);
		}
		else
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.Set("TemplateId", orgTemplateId + 25 - 1).Set("ConditionStatus", status).Set("IsShowConfirmButton", true);
			UIElement.SectStoryPopUpToggle.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.SectStoryPopUpToggle);
		}
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00050294 File Offset: 0x0004E494
	private static void HandlerDisplayEvent_PlayAudioCommandWithFade(RawDataPool dataPool, int argsOffset)
	{
		string audioName = null;
		bool isAmbience = false;
		bool isFadeOut = false;
		float fadeTime = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref audioName);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isAmbience);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isFadeOut);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref fadeTime);
		AudioManager audioManager = AudioManager.Instance;
		bool flag = audioManager == null;
		if (!flag)
		{
			bool flag2 = isAmbience;
			if (flag2)
			{
				bool flag3 = isFadeOut;
				if (flag3)
				{
					audioManager.StopAmbienceWithFade(fadeTime);
				}
				else
				{
					audioManager.PlayAmbience(audioName, fadeTime, 100);
				}
			}
			else
			{
				bool flag4 = isFadeOut;
				if (flag4)
				{
					audioManager.StopMusicWithFade(fadeTime);
				}
				else
				{
					audioManager.PlayMusic(audioName, fadeTime, 100, null);
				}
			}
		}
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0005034C File Offset: 0x0004E54C
	private static void HandlerDisplayEvent_ShowExchangePanel(RawDataPool dataPool, int argsOffset)
	{
		int charId = -1;
		string toEvent = "";
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charId);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref toEvent);
		UIElement.Exchange.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).Set("ShouldTriggerEvent", !string.IsNullOrEmpty(toEvent)));
		UIManager.Instance.ShowUI(UIElement.Exchange, true);
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x000503BB File Offset: 0x0004E5BB
	private static void HandlerDisplayEvent_ShowCreateMirrorCharacter(RawDataPool dataPool, int argsOffset)
	{
		UIManager.Instance.ShowUI(UIElement.CreateMirrorCharacter, true);
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x000503CF File Offset: 0x0004E5CF
	private static void HandleDisplayEvent_BackToMainMenu(RawDataPool dataPool, int argsOffset)
	{
		GameApp.ReturnToMainMenu(null, null, null);
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x000503DB File Offset: 0x0004E5DB
	private static void HandleDisplayEvent_TriggerCricketCatch(RawDataPool dataPool, int argsOffset)
	{
		GEvent.OnEvent(UiEvents.EventTriggerCricketCatch, null);
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x000503F0 File Offset: 0x0004E5F0
	private static void HandleDisplayEvent_ChangeMusicVolume(RawDataPool dataPool, int argsOffset)
	{
		int volume = 0;
		float fadeTime = 0f;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref volume);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref fadeTime);
		AudioManager.Instance.EnableMusicVolumeRate((float)volume / 100f);
		AudioManager.Instance.SetMusicVolumeWithFade(fadeTime, 0f);
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00050444 File Offset: 0x0004E644
	private static void HandleDisplayEvent_PlayMusicForCount(RawDataPool dataPool, int argsOffset)
	{
		string musicName = null;
		int count = 0;
		string nextMusic = null;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref musicName);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref count);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref nextMusic);
		AudioManager audioManager = AudioManager.Instance;
		bool flag = audioManager == null || string.IsNullOrEmpty(musicName) || count <= 0;
		if (!flag)
		{
			DisplayEventHandler.PlayMusicWithCount(audioManager, musicName, count, nextMusic);
		}
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x000504B4 File Offset: 0x0004E6B4
	private static void PlayMusicWithCount(AudioManager audioManager, string musicName, int remainingCount, string nextMusic)
	{
		bool flag = remainingCount <= 0;
		if (flag)
		{
			bool flag2 = !string.IsNullOrEmpty(nextMusic);
			if (flag2)
			{
				audioManager.PlayMusic(nextMusic, 1f, 100, null);
			}
			else
			{
				audioManager.StopMusicWithFade(1f);
			}
		}
		else
		{
			audioManager.PlayMusicForPlayer(musicName, 1f, 100, delegate(string _)
			{
				DisplayEventHandler.PlayMusicWithCount(audioManager, musicName, remainingCount - 1, nextMusic);
			}, 0f);
		}
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00050564 File Offset: 0x0004E764
	private static void HandleDisplayEvent_ReadLifeRecord(RawDataPool dataPool, int argsOffset)
	{
		int charId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref charId);
		ViewLifeRecords.Show(charId);
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00050588 File Offset: 0x0004E788
	private static void HandleDisplayEvent_ShowUnlockSkillSlotAnim(RawDataPool dataPool, int argsOffset)
	{
		int combatSkillEquipType = 0;
		int slotCount = 0;
		int neiliCount = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref combatSkillEquipType);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref slotCount);
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref neiliCount);
		UIElement.GameLineAnim.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("DisplayEventType", 145).Set("CombatSkillEquipType", combatSkillEquipType).Set("SlotCount", slotCount).Set("NeiliCount", neiliCount));
		UIManager.Instance.MaskUI(UIElement.GameLineAnim);
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00050614 File Offset: 0x0004E814
	private static void HandleDisplayEvent_CricketPolymorphEffect(RawDataPool dataPool, int argsOffset)
	{
		int cricketItemId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref cricketItemId);
		int colorId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref colorId);
		int partId = 0;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref partId);
		int charId = 0;
		Serializer.Deserialize(dataPool, argsOffset, ref charId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CricketItemId", cricketItemId);
		argBox.Set("ColorId", colorId);
		argBox.Set("PartId", partId);
		argBox.Set("CharId", charId);
		UIElement.CricketPolymorphEffect.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CricketPolymorphEffect, true);
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x000506B8 File Offset: 0x0004E8B8
	private static void HandlerDisplayEvent_SwitchDate(RawDataPool dataPool, int argsOffset)
	{
		bool isUnknown = false;
		argsOffset += Serializer.Deserialize(dataPool, argsOffset, ref isUnknown);
		UIElement.SwitchDate.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("IsUnknown", isUnknown));
		UIManager.Instance.ShowUI(UIElement.SwitchDate, true);
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00050701 File Offset: 0x0004E901
	private static void HandleDisplayEvent_MoveTaiwuVillageAreaOnMonthNotifyClosed()
	{
		UIElement monthNotify = UIElement.MonthNotify;
		monthNotify.OnHide = (Action)Delegate.Combine(monthNotify.OnHide, new Action(delegate()
		{
			UIElement.Bottom.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("KeepAnim", false));
			UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
			GEvent.OnEvent(UiEvents.PartWorldMapDirectReturn, null);
		}));
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0005074F File Offset: 0x0004E94F
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_PerformCutscene>g__OnVideoPlayStart|45_1()
	{
		AudioManager.Instance.EnterVideoMode();
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0005075D File Offset: 0x0004E95D
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_PerformCutscene>g__Finish|45_0()
	{
		TaiwuEventDomainMethod.Call.TriggerListener("PerformCutsceneComplete", true);
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0005076C File Offset: 0x0004E96C
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_PlayMediaCommand>g__OnVideoPlayStart|70_0()
	{
		AudioManager.Instance.EnterVideoMode();
		UIManager.Instance.StackToUI(UIElement.BlockInteract);
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0005078C File Offset: 0x0004E98C
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_TravelingBuddhistMonkSkill3>g__OnConfirm|106_0(short id)
	{
		bool flag = id != -1;
		if (flag)
		{
			ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
			{
				ProfessionId = 12,
				SkillId = 51,
				EffectId = id,
				IsSuccess = true
			};
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("ProfessionSkillArg", professionSkillArg);
			UIElement.ProfessionSkillConfirm.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x000507FC File Offset: 0x0004E9FC
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_BuddhistMonkSkill3>g__OnConfirm|107_1(short id)
	{
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = 6,
			SkillId = 27,
			EffectId = id,
			IsSuccess = true
		};
		ExtraDomainMethod.Call.ConfirmExecuteSkill(professionSkillArg, true);
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x00050838 File Offset: 0x0004EA38
	[CompilerGenerated]
	internal static void <HandleDisplayEvent_OpenItemSelectForSavageSkill3>g__OnSelectItem|118_1(ItemKey selectedItem)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = 0,
			SkillId = 3,
			IsSuccess = true,
			ItemKey = selectedItem
		};
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x04000DA2 RID: 3490
	private static Queue<Action> _pendingNormalDialogActions = new Queue<Action>();

	// Token: 0x04000DA3 RID: 3491
	private static bool AudioSettingSeon = false;
}

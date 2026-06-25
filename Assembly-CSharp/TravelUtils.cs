using System;
using System.Collections;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000147 RID: 327
public static class TravelUtils
{
	// Token: 0x0600118E RID: 4494 RVA: 0x0006A3A0 File Offset: 0x000685A0
	public static string GetStationName(short areaId)
	{
		return MapBlock.Instance[(areaId < 45) ? 37 : 38].Name;
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0006A3BC File Offset: 0x000685BC
	public static void SetDialogTravelAuthorityNotEnough(DialogCmd cmd, int target, bool open = false)
	{
		cmd = (cmd ?? new DialogCmd());
		cmd.Type = 2;
		cmd.Title = LocalStringManager.Get(LanguageKey.UI_Travel_Authority_Not_Enough_Title);
		cmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_Travel_Authority_Not_Enough_Tip, target).ColorReplace();
		if (open)
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0006A43C File Offset: 0x0006863C
	public static void SetDialogAsUnlockStation(DialogCmd dialogCmd, int costAuthority, short areaId, WorldMapModel mapModel, Action onConfirm = null)
	{
		dialogCmd.Type = 1;
		dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Unlock_Station);
		dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Unlock_Station_Confirm, costAuthority, mapModel.Areas[(int)areaId].GetConfig().Name, TravelUtils.GetStationName(areaId)).ColorReplace();
		dialogCmd.Yes = delegate()
		{
			MapDomainMethod.Call.UnlockStation(areaId);
			Action onConfirm2 = onConfirm;
			if (onConfirm2 != null)
			{
				onConfirm2();
			}
		};
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0006A4C5 File Offset: 0x000686C5
	public static void ShowTravelConfirmDialog(CrossAreaMoveInfo travelInfo, Action confirm, Action cancel = null)
	{
		SingletonObject.getInstance<YieldHelper>().StartYield(TravelUtils.CoShowTravelConfirmDialog(travelInfo, confirm, cancel));
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0006A4DB File Offset: 0x000686DB
	public static void ShowDirectTravelConfirmDialog(CrossAreaMoveInfo travelInfo, Action confirm, Action cancel = null)
	{
		SingletonObject.getInstance<YieldHelper>().StartYield(TravelUtils.CoShowDirectTravelConfirmDialog(travelInfo, confirm, cancel));
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0006A4F4 File Offset: 0x000686F4
	public static void GetTravelCostAutoConfirm(short srcArea, short srcBlock, short dstArea, Action<CrossAreaMoveInfo> onGet)
	{
		MapDomainMethod.AsyncCall.GetTravelCost(null, srcArea, srcBlock, dstArea, delegate(int offset, RawDataPool dataPool)
		{
			CrossAreaMoveInfo travelInfo = new CrossAreaMoveInfo();
			Serializer.Deserialize(dataPool, offset, ref travelInfo);
			Action<CrossAreaMoveInfo> onGet2 = onGet;
			if (onGet2 != null)
			{
				onGet2(travelInfo);
			}
		});
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0006A525 File Offset: 0x00068725
	private static IEnumerator CoShowTravelConfirmDialog(CrossAreaMoveInfo travelInfo, Action confirm, Action cancel)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		ResourceMonitor resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(taiwuCharId, false);
		yield return new WaitUntil(() => resourceMonitor.Init);
		DialogCmd dialogCmd = new DialogCmd();
		bool flag = TravelUtils.SetAsFailed(dialogCmd, travelInfo, resourceMonitor);
		if (flag)
		{
			dialogCmd.Type = 2;
			dialogCmd.Yes = cancel;
			dialogCmd.No = cancel;
		}
		else
		{
			bool flag2 = travelInfo.AuthorityCost > 0;
			if (flag2)
			{
				TravelUtils.SetAsUnlock(dialogCmd, travelInfo, cancel);
				dialogCmd.No = cancel;
			}
			else
			{
				TravelUtils.SetAsSuccess(dialogCmd, travelInfo, resourceMonitor);
				dialogCmd.Type = 1;
				dialogCmd.Yes = confirm;
				dialogCmd.No = cancel;
			}
		}
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
		yield break;
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0006A542 File Offset: 0x00068742
	private static IEnumerator CoShowDirectTravelConfirmDialog(CrossAreaMoveInfo travelInfo, Action confirm, Action cancel)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		ResourceMonitor resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(taiwuCharId, false);
		yield return new WaitUntil(() => resourceMonitor.Init);
		DialogCmd dialogCmd = new DialogCmd();
		TravelUtils.SetAsSuccess(dialogCmd, travelInfo, resourceMonitor);
		string contentPrefix = LocalStringManager.Get(LanguageKey.UI_DirectTravelToTaiwuVillage_Prefix);
		dialogCmd.Content = contentPrefix + dialogCmd.Content;
		dialogCmd.Type = 1;
		dialogCmd.Yes = confirm;
		dialogCmd.No = cancel;
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
		yield break;
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0006A560 File Offset: 0x00068760
	private static int CountLockedStation(CrossAreaMoveInfo travelInfo, WorldMapModel mapModel)
	{
		int count = 0;
		bool flag = travelInfo.AuthorityCost > 0;
		if (flag)
		{
			foreach (short areaId in travelInfo.Route.AreaList)
			{
				bool flag2 = !mapModel.Areas[(int)areaId].StationUnlocked;
				if (flag2)
				{
					count++;
				}
			}
		}
		return count;
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0006A5E4 File Offset: 0x000687E4
	private static string ParseFirstLockedStationAreaName(CrossAreaMoveInfo travelInfo, WorldMapModel mapModel)
	{
		foreach (short areaId in travelInfo.Route.AreaList)
		{
			bool flag = !mapModel.Areas[(int)areaId].StationUnlocked;
			if (flag)
			{
				return mapModel.Areas[(int)areaId].GetConfig().Name;
			}
		}
		PredefinedLog.Show(11, "ParseFirstLockedStationAreaName with all station unlocked");
		return mapModel.Areas[(int)travelInfo.ToAreaId].GetConfig().Name;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x0006A68C File Offset: 0x0006888C
	private static string ParseStationList(CrossAreaMoveInfo travelInfo, WorldMapModel mapModel)
	{
		StringBuilder stationListStrBuilder = EasyPool.Get<StringBuilder>();
		bool flag = travelInfo.AuthorityCost > 0;
		if (flag)
		{
			bool firstUnlockArea = true;
			stationListStrBuilder.Clear();
			stationListStrBuilder.Append(LocalStringManager.Get(LanguageKey.UI_Dialog_First_TravelConfirm_Need_Unlock));
			for (int i = 0; i < travelInfo.Route.AreaList.Count; i++)
			{
				short areaId = travelInfo.Route.AreaList[i];
				bool flag2 = !mapModel.Areas[(int)areaId].StationUnlocked;
				if (flag2)
				{
					bool flag3 = firstUnlockArea;
					if (flag3)
					{
						firstUnlockArea = false;
					}
					else
					{
						stationListStrBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					stationListStrBuilder.Append(mapModel.Areas[(int)areaId].GetConfig().Name.SetColor("darkbrown"));
				}
			}
		}
		string ret = stationListStrBuilder.ToString();
		EasyPool.Free<StringBuilder>(stationListStrBuilder);
		return ret;
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0006A778 File Offset: 0x00068978
	private static bool SetAsFailed(DialogCmd dialogCmd, CrossAreaMoveInfo travelInfo, ResourceMonitor resourceMonitor)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		bool flag = travelInfo.FromAreaId < 135 && !mapModel.Areas[(int)travelInfo.FromAreaId].StationUnlocked;
		bool result;
		if (flag)
		{
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_Station_Not_Found_Title);
			dialogCmd.Content = LocalStringManager.Get(LanguageKey.UI_Station_Not_Found_Tip).ColorReplace() + "\n" + TravelUtils.ParseStationList(travelInfo, mapModel);
			result = true;
		}
		else
		{
			bool flag2 = travelInfo.AuthorityCost > resourceMonitor.Resources[7];
			if (flag2)
			{
				TravelUtils.SetDialogTravelAuthorityNotEnough(dialogCmd, travelInfo.AuthorityCost, false);
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0006A81C File Offset: 0x00068A1C
	private static void SetAsSuccess(DialogCmd dialogCmd, CrossAreaMoveInfo travelInfo, ResourceMonitor resourceMonitor)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		MapAreaItem areaConfig = mapModel.Areas[(int)travelInfo.ToAreaId].GetConfig();
		MapStateItem stateConfig = MapState.Instance[areaConfig.StateID];
		bool flag = travelInfo.MoneyCost > resourceMonitor.Resources[6];
		if (flag)
		{
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_Travel_Money_Not_Enough_Title);
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_Travel_Money_Not_Enough_Tip, SharedConstValue.FreeTravelCostTimeRate).ColorReplace();
		}
		else
		{
			bool flag2 = travelInfo.AuthorityCost <= 0;
			if (flag2)
			{
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_Dialog_TravelConfirm_Title);
				dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_Dialog_TravelConfirm_Content, new object[]
				{
					travelInfo.Route.GetTotalTimeCost(),
					travelInfo.MoneyCost,
					stateConfig.Name + areaConfig.Name,
					TravelUtils.GetStationName(travelInfo.ToAreaId)
				}).ColorReplace();
			}
			else
			{
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_Dialog_First_TravelConfirm_Title);
				dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_Dialog_First_TravelConfirm_Content, new object[]
				{
					travelInfo.Route.GetTotalTimeCost(),
					travelInfo.MoneyCost,
					travelInfo.AuthorityCost,
					stateConfig.Name + areaConfig.Name,
					TravelUtils.GetStationName(travelInfo.ToAreaId)
				}).ColorReplace() + "\n" + TravelUtils.ParseStationList(travelInfo, mapModel);
			}
		}
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0006A9B4 File Offset: 0x00068BB4
	public static void SetAsUnlock(DialogCmd dialogCmd, CrossAreaMoveInfo travelInfo, Action onConfirm)
	{
		TravelUtils.<>c__DisplayClass13_0 CS$<>8__locals1 = new TravelUtils.<>c__DisplayClass13_0();
		CS$<>8__locals1.travelInfo = travelInfo;
		CS$<>8__locals1.onConfirm = onConfirm;
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		MapAreaItem areaConfig = mapModel.Areas[(int)CS$<>8__locals1.travelInfo.ToAreaId].GetConfig();
		MapStateItem stateConfig = MapState.Instance[areaConfig.StateID];
		dialogCmd.Type = 1;
		dialogCmd.Yes = new Action(CS$<>8__locals1.<SetAsUnlock>g__DoUnlock|0);
		dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Unlock_Station);
		bool flag = TravelUtils.CountLockedStation(CS$<>8__locals1.travelInfo, mapModel) == 1;
		if (flag)
		{
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Unlock_Station_Confirm, CS$<>8__locals1.travelInfo.AuthorityCost, TravelUtils.ParseFirstLockedStationAreaName(CS$<>8__locals1.travelInfo, mapModel), TravelUtils.GetStationName(CS$<>8__locals1.travelInfo.ToAreaId)).ColorReplace();
		}
		else
		{
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Unlock_Station_Confirm, CS$<>8__locals1.travelInfo.AuthorityCost, stateConfig.Name + areaConfig.Name, TravelUtils.GetStationName(CS$<>8__locals1.travelInfo.ToAreaId)).ColorReplace() + "\n" + TravelUtils.ParseStationList(CS$<>8__locals1.travelInfo, mapModel);
		}
	}
}

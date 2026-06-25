using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
public class MapElementStationBtn : MapElementBase
{
	// Token: 0x06003CE1 RID: 15585 RVA: 0x001EA39C File Offset: 0x001E859C
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MapAreaData areaData = MapElementBase.MapModel.Areas[(int)location.AreaId];
			bool flag2 = areaData == null;
			result = (!flag2 && areaData.StationBlockId == location.BlockId);
		}
		return result;
	}

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x06003CE2 RID: 15586 RVA: 0x001EA404 File Offset: 0x001E8604
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.SettlementAndStation;
		}
	}

	// Token: 0x06003CE3 RID: 15587 RVA: 0x001EA408 File Offset: 0x001E8608
	public override void Scale(float wheel)
	{
		float factor = Mathf.Pow(1f / wheel, 1.5f) * wheel;
		Vector3 reverseScale = Vector3.one * factor;
		PointerScaleAnim buildingScale = this.btnTravelToStation.GetComponent<PointerScaleAnim>();
		PointerScaleAnim infoScale = this.btnUnlockStation.GetComponent<PointerScaleAnim>();
		buildingScale.BaseScale = reverseScale;
		buildingScale.TargetScale = reverseScale * 1.1f;
		this.btnTravelToStation.transform.localScale = reverseScale;
		infoScale.BaseScale = reverseScale;
		infoScale.TargetScale = reverseScale * 1.1f;
		this.btnUnlockStation.transform.localScale = reverseScale;
		base.transform.localScale = Vector3.one * (Mathf.Pow(1f / wheel, 1.375f) * wheel);
	}

	// Token: 0x06003CE4 RID: 15588 RVA: 0x001EA4CB File Offset: 0x001E86CB
	protected override void OnCreate()
	{
		this.btnTravelToStation.ClearAndAddListener(new Action(this.OnClickTravel));
		this.btnUnlockStation.ClearAndAddListener(new Action(this.OnClickUnlockStation));
	}

	// Token: 0x06003CE5 RID: 15589 RVA: 0x001EA500 File Offset: 0x001E8700
	protected override void OnRefresh()
	{
		bool visible = base.BlockLocation.AreaId == MapElementBase.MapModel.ShowingAreaId && base.BlockLocation.AreaId != MapElementBase.MapModel.CurrentAreaId && MapElementBase.MapModel.Areas[(int)base.BlockLocation.AreaId].Discovered;
		base.gameObject.SetActive(visible);
		bool flag = !visible;
		if (!flag)
		{
			this.btnTravelToStation.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
			this.btnUnlockStation.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
			bool unlocked = MapElementBase.MapModel.Areas[(int)base.BlockLocation.AreaId].StationUnlocked;
			this.btnTravelToStation.gameObject.SetActive(unlocked);
			this.btnUnlockStation.gameObject.SetActive(!unlocked);
			bool flag2 = MapElementBase.MapModel.IsAreaHasDangerTips(base.BlockLocation.AreaId);
			if (flag2)
			{
				TooltipInvoker component = this.btnTravelToStation.GetComponent<TooltipInvoker>();
				bool flag3 = component == null;
				if (flag3)
				{
					component = this.btnTravelToStation.gameObject.AddComponent<TooltipInvoker>();
					component.Type = TipType.SingleDesc;
				}
				component.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Danger_Area_Simple_Tips)
				};
			}
			else
			{
				TooltipInvoker component2 = this.btnTravelToStation.GetComponent<TooltipInvoker>();
				bool flag4 = component2 != null;
				if (flag4)
				{
					Object.Destroy(component2);
				}
			}
		}
	}

	// Token: 0x06003CE6 RID: 15590 RVA: 0x001EA69F File Offset: 0x001E889F
	protected override void OnCollect()
	{
	}

	// Token: 0x06003CE7 RID: 15591 RVA: 0x001EA6A4 File Offset: 0x001E88A4
	private void OnClickTravel()
	{
		FunctionLockManager funcLockManager = SingletonObject.getInstance<FunctionLockManager>();
		bool flag = !funcLockManager.IsFunctionUnlock(3);
		if (!flag)
		{
			WorldMapModel mapModel = MapElementBase.MapModel;
			TravelUtils.GetTravelCostAutoConfirm(mapModel.CurrentAreaId, mapModel.CurrentBlockId, base.BlockLocation.AreaId, delegate(CrossAreaMoveInfo travelInfo)
			{
				MapElementStationBtn.<>c__DisplayClass9_0 CS$<>8__locals1 = new MapElementStationBtn.<>c__DisplayClass9_0();
				CS$<>8__locals1.travelInfo = travelInfo;
				TravelUtils.ShowTravelConfirmDialog(CS$<>8__locals1.travelInfo, new Action(CS$<>8__locals1.<OnClickTravel>g__ConfirmTravel|1), null);
			});
		}
	}

	// Token: 0x06003CE8 RID: 15592 RVA: 0x001EA70C File Offset: 0x001E890C
	private void OnClickUnlockStation()
	{
		int foundAreasCount = 0;
		for (short i = 0; i < 135; i += 1)
		{
			bool stationUnlocked = MapElementBase.MapModel.Areas[(int)i].StationUnlocked;
			if (stationUnlocked)
			{
				foundAreasCount++;
			}
		}
		int costAuthority = Mathf.Max((int)GlobalConfig.Instance.MapAreaOpenPrestige * (foundAreasCount - (int)(9 * GlobalConfig.Instance.MapInitUnlockStationStateCount) + 1), 0);
		DialogCmd dialogCmd = new DialogCmd();
		bool flag = MapElementBase.MapModel.TaiwuResources.Resources[7] >= costAuthority;
		if (flag)
		{
			TravelUtils.SetDialogAsUnlockStation(dialogCmd, costAuthority, base.BlockLocation.AreaId, MapElementBase.MapModel, delegate
			{
				this.btnTravelToStation.gameObject.SetActive(true);
				this.btnUnlockStation.gameObject.SetActive(false);
			});
		}
		else
		{
			TravelUtils.SetDialogTravelAuthorityNotEnough(dialogCmd, costAuthority, false);
		}
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x04002BA6 RID: 11174
	[SerializeField]
	private CButton btnTravelToStation;

	// Token: 0x04002BA7 RID: 11175
	[SerializeField]
	private CButton btnUnlockStation;
}

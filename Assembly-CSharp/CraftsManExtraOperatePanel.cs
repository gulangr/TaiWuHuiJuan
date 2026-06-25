using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class CraftsManExtraOperatePanel : Refers
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06001848 RID: 6216 RVA: 0x00094EB5 File Offset: 0x000930B5
	private GameObject _addResourceHints
	{
		get
		{
			return base.CGet<GameObject>("AddResourceHint");
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06001849 RID: 6217 RVA: 0x00094EC2 File Offset: 0x000930C2
	public CraftsManAddResourcePanel CraftsManAddResourcePanel
	{
		get
		{
			return this._craftsManAddResourcePanel;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x0600184A RID: 6218 RVA: 0x00094ECA File Offset: 0x000930CA
	public Dictionary<sbyte, List<ItemKey>> PutResourceDic
	{
		get
		{
			return this._craftsManAddResourcePanel.PutResourceDic;
		}
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x00094ED8 File Offset: 0x000930D8
	public void Init(IAsyncMethodRequestHandler asyncMethodRequestHandler, Action onClickBtnProductType, Action onClickBtnAddAttainments, Action<int> onChangeStock, int makeItemType, Action addResource, Action<Transform> actionOnEnterFocusMode)
	{
		this._actionOnEnterFocusMode = actionOnEnterFocusMode;
		this._btnProductType = base.CGet<CButtonObsolete>("BtnProductType");
		this._btnProductType.ClearAndAddListener(onClickBtnProductType);
		this._btnAddAttainments = base.CGet<CButtonObsolete>("BtnAddAttainments");
		this._btnAddAttainments.ClearAndAddListener(onClickBtnAddAttainments);
		this.storeToDropdown.Init(asyncMethodRequestHandler, makeItemType, onChangeStock);
		this._btnAddResources = base.CGet<CButtonObsolete>("BtnAddResources");
		this._btnAddResources.ClearAndAddListener(new Action(this.OnClickBtnAddResources));
		this._addResource = addResource;
		base.CGet<TextMeshProUGUI>("ProgressContentMonth").text = "/" + LocalStringManager.Get(LanguageKey.LK_Month);
		this._craftsManAddResourcePanel = base.CGet<CraftsManAddResourcePanel>("CraftsManAddResourcePanel");
		this._craftsManAddResourcePanel.Init(new Action(this.OnCancelAddResource), new Action<int>(this.SetPredictProductProgress), this);
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x00094FC7 File Offset: 0x000931C7
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnPutResourcePreview, new GEvent.Callback(this._craftsManAddResourcePanel.OnPutResourcePreview));
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x00094FEB File Offset: 0x000931EB
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnPutResourcePreview, new GEvent.Callback(this._craftsManAddResourcePanel.OnPutResourcePreview));
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x00095010 File Offset: 0x00093210
	public void Refresh(ArtisanOrder artisanOrder, short liftSkillType, int resourceCount, EPanelMode panelMode, ECraftType currentCraftType, bool isMatchVillagerRole, int leaderId)
	{
		bool flag = artisanOrder != null;
		if (flag)
		{
			this.storeToDropdown.ForceRefreshStorageType(artisanOrder.StorageType);
			bool isInvalid = artisanOrder.BuildingBlockKey.IsInvalid;
			if (isInvalid)
			{
				this.storeToDropdown.MakeItemMethod = (int)(-32 + artisanOrder.LifeSkillType);
			}
			else
			{
				this.storeToDropdown.MakeItemMethod = (int)artisanOrder.BuildingBlockKey.BuildingBlockIndex;
			}
		}
		sbyte orderLifeSkillType = (artisanOrder != null) ? artisanOrder.LifeSkillType : -1;
		bool isSameResourceType = liftSkillType == (short)orderLifeSkillType;
		int addProgress = isSameResourceType ? ((artisanOrder != null) ? artisanOrder.ProgressDelta : 0) : 0;
		int addBaseProgress = isSameResourceType ? ((artisanOrder != null) ? artisanOrder.ProgressBaseDelta : 0) : 0;
		this._progressAddValue = addProgress;
		this._progressAddBaseValue = addBaseProgress;
		this.SetProductAddProgress(this._progressAddValue, this._progressAddBaseValue);
		base.CGet<TooltipInvoker>("MouseTip").enabled = (panelMode == EPanelMode.Building);
		this.SetPredictProductProgress(0);
		int progress = isSameResourceType ? ((artisanOrder != null) ? artisanOrder.Progress : 0) : 0;
		this._progressValue = 100 * progress / SharedMethods.MaxProductionProgress(this._buildingCraftPanel);
		this.SetProductProgress(this._progressValue);
		bool isTaiwuOrder = panelMode.HasFlag(EPanelMode.TaiwuOrdered);
		bool isBuildingOrder = panelMode.HasFlag(EPanelMode.Building) && isMatchVillagerRole && leaderId > 0;
		bool isProgressFull = progress >= SharedMethods.MaxProductionProgress(this._buildingCraftPanel);
		bool isTeaOrWine = currentCraftType == ECraftType.Tea || currentCraftType == ECraftType.Wine;
		bool isFood = currentCraftType == ECraftType.Cooking;
		this._btnProductType.interactable = ((isBuildingOrder || isTaiwuOrder) && isSameResourceType && !isTeaOrWine && !isFood);
		bool storeBtnInteractable = (isBuildingOrder || isTaiwuOrder) && isSameResourceType;
		this._btnAddAttainments.interactable = (storeBtnInteractable && !isTeaOrWine);
		this._btnAddResources.interactable = (storeBtnInteractable && !isProgressFull);
		this.storeToDropdown.Active = storeBtnInteractable;
		sbyte productResourceType = UI_CraftsmanPanel.GetResourceTypeByCraftType(currentCraftType);
		this._craftsManAddResourcePanel.Refresh(productResourceType, progress, resourceCount);
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x0009520B File Offset: 0x0009340B
	private void OnCancelAddResource()
	{
		this.SetProductAddProgress(this._progressAddValue, this._progressAddBaseValue);
		this.SetProductProgress(this._progressValue);
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x00095230 File Offset: 0x00093430
	private void OnClickBtnAddResources()
	{
		bool focusMode = this._craftsManAddResourcePanel.FocusMode;
		if (focusMode)
		{
			bool flag = this._craftsManAddResourcePanel.PutResourceDic.Count > 0;
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Building_AddCraftResource_ConfirmTitle);
				string content = LocalStringManager.Get(LanguageKey.LK_Building_AddCraftResource_ConfirmContent);
				CommonUtils.ShowConfirmDialog(title, content, new Action(this.<OnClickBtnAddResources>g__PutResource|22_0), null, EDialogType.None);
			}
			else
			{
				this.<OnClickBtnAddResources>g__PutResource|22_0();
			}
		}
		else
		{
			this._craftsManAddResourcePanel.PutResourceDic.Clear();
			Transform targetTrans = this._craftsManAddResourcePanel.SetAddResourceFocusMode(true);
			Action<Transform> actionOnEnterFocusMode = this._actionOnEnterFocusMode;
			if (actionOnEnterFocusMode != null)
			{
				actionOnEnterFocusMode(targetTrans);
			}
		}
		this._addResourceHints.SetActive(this._craftsManAddResourcePanel.FocusMode);
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x000952EE File Offset: 0x000934EE
	public void CloseFocusModeUI()
	{
		this._craftsManAddResourcePanel.SetAddResourceFocusMode(false);
		this._addResourceHints.SetActive(false);
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x0009530C File Offset: 0x0009350C
	public void SetProductType(short itemSubType)
	{
		Refers refers = this._btnProductType.GetComponent<Refers>();
		string icon;
		string name;
		UI_SelectProductType.GetMakeItemTypeInfo(itemSubType, out icon, out name);
		string disableIcon = icon.Replace("_0", "_1");
		refers.CGet<CImage>("NormalIcon").SetSprite(icon, false, null);
		refers.CGet<CImage>("DisableIcon").SetSprite(disableIcon, false, null);
		refers.CGet<TextMeshProUGUI>("NormalLabel").text = name;
		refers.CGet<TextMeshProUGUI>("DisableLabel").text = name;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x00095390 File Offset: 0x00093590
	private void SetPredictProductProgress(int predictValue)
	{
		float predictValuePercent = (float)predictValue / 100f;
		base.CGet<PredictableProgressBar>("LeftProgressBar").SetPredictProgress(predictValuePercent);
		base.CGet<PredictableProgressBar>("RightProgressBar").SetPredictProgress(predictValuePercent);
		string content = string.Format("{0}%", predictValue).SetColor("brightblue");
		base.CGet<TextMeshProUGUI>("ProgressContent").text = content;
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x000953F8 File Offset: 0x000935F8
	private void SetProductProgress(int value)
	{
		float valuePercent = (float)value / 100f;
		base.CGet<PredictableProgressBar>("LeftProgressBar").SetProgressValue(valuePercent);
		base.CGet<PredictableProgressBar>("RightProgressBar").SetProgressValue(valuePercent);
		string content = string.Format("{0}%", value).SetColor("white");
		base.CGet<TextMeshProUGUI>("ProgressContent").text = content;
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x00095460 File Offset: 0x00093660
	private void SetProductAddProgress(int addValue, int addBaseValue)
	{
		int showAddValue = this.GetShowValue(addValue);
		int showAddBaseValue = this.GetShowValue(addBaseValue);
		string content = string.Format("{0}%", Math.Min(100, showAddValue)).SetColor("brightblue");
		base.CGet<TextMeshProUGUI>("PredictProgressContent").text = content;
		TooltipInvoker mouseTip = base.CGet<TooltipInvoker>("MouseTip");
		int extraValue = showAddValue - showAddBaseValue;
		GeneralLineData desc = new GeneralLineData
		{
			Type = 3,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerCraftPreviewPanel_Tip_Normal)
			}
		};
		GeneralLineData title = new GeneralLineData
		{
			Type = 3,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Craftsman_CraftProgress))
			}
		};
		GeneralLineData contentNormal = new GeneralLineData
		{
			Type = 5,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_CraftProgress_Tip, showAddBaseValue.ToString() + "%")
			},
			ExtraArgs = new List<object>
			{
				20
			}
		};
		GeneralLineData contentExtra = new GeneralLineData
		{
			Type = 5,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_CraftProgress_ExtraTip, extraValue.ToString() + "%")
			},
			ExtraArgs = new List<object>
			{
				20
			}
		};
		int lineCount = 3;
		TooltipInvoker tooltipInvoker = mouseTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		mouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Craftsman_CraftProgress)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentNormal);
		bool flag = extraValue > 0;
		if (flag)
		{
			lineCount++;
			mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentExtra);
		}
		mouseTip.RuntimeParam.Set("LineCount", lineCount);
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x0009566C File Offset: 0x0009386C
	private int GetShowValue(int value)
	{
		return 100 * value / SharedMethods.MaxProductionProgress(this._buildingCraftPanel);
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x00095697 File Offset: 0x00093897
	[CompilerGenerated]
	private void <OnClickBtnAddResources>g__PutResource|22_0()
	{
		this._addResource();
		this._craftsManAddResourcePanel.SetAddResourceFocusMode(false);
		GEvent.OnEvent(UiEvents.OnAddCraftsmanResource, null);
	}

	// Token: 0x04001385 RID: 4997
	private CButtonObsolete _btnProductType;

	// Token: 0x04001386 RID: 4998
	private CButtonObsolete _btnAddAttainments;

	// Token: 0x04001387 RID: 4999
	private CButtonObsolete _btnAddResources;

	// Token: 0x04001388 RID: 5000
	[SerializeField]
	internal BuildingStoreToDropdown storeToDropdown;

	// Token: 0x04001389 RID: 5001
	private CraftsManAddResourcePanel _craftsManAddResourcePanel;

	// Token: 0x0400138A RID: 5002
	private Action _addResource;

	// Token: 0x0400138B RID: 5003
	private Action<Transform> _actionOnEnterFocusMode;

	// Token: 0x0400138C RID: 5004
	private int _progressValue;

	// Token: 0x0400138D RID: 5005
	private int _progressAddValue;

	// Token: 0x0400138E RID: 5006
	private int _progressAddBaseValue;

	// Token: 0x0400138F RID: 5007
	internal bool _buildingCraftPanel;
}

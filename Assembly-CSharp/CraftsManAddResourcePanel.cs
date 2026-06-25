using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Item;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001AC RID: 428
public class CraftsManAddResourcePanel : Refers
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06001837 RID: 6199 RVA: 0x00094A42 File Offset: 0x00092C42
	// (set) Token: 0x06001838 RID: 6200 RVA: 0x00094A4A File Offset: 0x00092C4A
	public bool FocusMode { get; private set; }

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06001839 RID: 6201 RVA: 0x00094A53 File Offset: 0x00092C53
	// (set) Token: 0x0600183A RID: 6202 RVA: 0x00094A5B File Offset: 0x00092C5B
	public int ProductResourceAdded { get; private set; }

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x0600183B RID: 6203 RVA: 0x00094A64 File Offset: 0x00092C64
	public Dictionary<sbyte, List<ItemKey>> PutResourceDic
	{
		get
		{
			return this._putResourceDic;
		}
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x00094A6C File Offset: 0x00092C6C
	public void Init(Action onCancelAddResource, Action<int> setPredictProductProgress, CraftsManExtraOperatePanel craftsManExtraOperatePanel)
	{
		this._onCancelAddResource = onCancelAddResource;
		this._setPredictProductProgress = setPredictProductProgress;
		this._craftsManExtraOperatePanel = craftsManExtraOperatePanel;
		this._btnCancelFocusMode = base.CGet<CButtonObsolete>("BtnCancelFocusMode");
		this._btnCancelFocusMode.ClearAndAddListener(delegate
		{
			this.SetAddResourceFocusMode(false);
		});
		this._addResourceSlider = base.CGet<CSliderLegacy>("Slider_FreeValue");
		this._addResourceSlider.onValueChanged.RemoveAllListeners();
		this._addResourceSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnAddResourceScrollValueChange));
		Refers setSelectResourceAmountRefer = base.CGet<Refers>("SetSelectResourceAmountRefer");
		this._buttonLess = setSelectResourceAmountRefer.CGet<CButtonObsolete>("ButtonLess");
		this._buttonLess.ClearAndAddListener(new Action(this.OnClickButtonLess));
		this._buttonMore = setSelectResourceAmountRefer.CGet<CButtonObsolete>("ButtonMore");
		this._buttonMore.ClearAndAddListener(new Action(this.OnClickButtonMore));
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00094B58 File Offset: 0x00092D58
	public void Refresh(sbyte productResourceType, int progress, int resourceCount)
	{
		this._productResourceType = productResourceType;
		this._progress = progress;
		this._resourceCount = resourceCount;
		this._addResourceSlider.value = 0f;
		bool hasResource = resourceCount > 0;
		this._addResourceSlider.interactable = hasResource;
		this.SetAddResourceCostLabel(0);
		this.RefreshButton();
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00094BB0 File Offset: 0x00092DB0
	public void OnPutResourcePreview(ArgumentBox argBox)
	{
		long value;
		argBox.Get("Value", out value);
		this.ProductResourceAdded = (int)value;
		argBox.Get<Dictionary<sbyte, List<ItemKey>>>("KeyDic", out this._putResourceDic);
		int maxProgress = SharedMethods.MaxProductionProgress(this._craftsManExtraOperatePanel._buildingCraftPanel);
		long predictProgress = Math.Clamp((long)this._progress + value, 0L, (long)maxProgress);
		long predictValue = 100L * predictProgress / (long)maxProgress;
		this._setPredictProductProgress((int)predictValue);
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x00094C24 File Offset: 0x00092E24
	private void OnAddResourceScrollValueChange(float newValue)
	{
		int maxProgress = SharedMethods.MaxProductionProgress(this._craftsManExtraOperatePanel._buildingCraftPanel);
		int need = maxProgress - this._progress;
		int full = Math.Min(need, this._resourceCount);
		this.ProductResourceAdded = Math.Clamp((int)(newValue * (float)full), 0, maxProgress);
		this.SetAddResourceCostLabel(this.ProductResourceAdded);
		int predictProgress = Math.Clamp(this._progress + this.ProductResourceAdded, 0, maxProgress);
		int predictValue = 100 * predictProgress / maxProgress;
		this._setPredictProductProgress(predictValue);
		this.RefreshButton();
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x00094CAC File Offset: 0x00092EAC
	private void RefreshButton()
	{
		bool isFull = this._progress + this.ProductResourceAdded >= SharedMethods.MaxProductionProgress(this._craftsManExtraOperatePanel._buildingCraftPanel);
		bool hasResource = this._resourceCount > 0;
		this._buttonMore.interactable = (hasResource && !isFull);
		this._buttonLess.interactable = (hasResource && this.ProductResourceAdded > 0);
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x00094D18 File Offset: 0x00092F18
	private void OnClickButtonLess()
	{
		this.ProductResourceAdded -= ItemTemplateHelper.GetResourceCountUnit();
		this.ClampResourceAdded();
	}

	// Token: 0x06001842 RID: 6210 RVA: 0x00094D35 File Offset: 0x00092F35
	private void OnClickButtonMore()
	{
		this.ProductResourceAdded += ItemTemplateHelper.GetResourceCountUnit();
		this.ClampResourceAdded();
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x00094D54 File Offset: 0x00092F54
	private void ClampResourceAdded()
	{
		int diff = SharedMethods.MaxProductionProgress(this._craftsManExtraOperatePanel._buildingCraftPanel) - this._progress;
		int resourceCount = this._resourceCount;
		int max = Math.Min(diff, resourceCount);
		this.ProductResourceAdded = Math.Clamp(this.ProductResourceAdded, 0, max);
		float value = (max == 0) ? 0f : ((float)this.ProductResourceAdded / (float)max);
		this._addResourceSlider.SetValueWithoutNotify(value);
		this.RefreshButton();
		this.SetAddResourceCostLabel(this.ProductResourceAdded);
		int predictProgress = this._progress + this.ProductResourceAdded;
		int predictValue = 100 * predictProgress / SharedMethods.MaxProductionProgress(this._craftsManExtraOperatePanel._buildingCraftPanel);
		this._setPredictProductProgress(predictValue);
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x00094E08 File Offset: 0x00093008
	private void SetAddResourceCostLabel(int added)
	{
		base.CGet<CostResource>("CostResource").SetInfo(this._productResourceType, added, this._resourceCount);
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x00094E2C File Offset: 0x0009302C
	public Transform SetAddResourceFocusMode(bool isEnter)
	{
		this.FocusMode = isEnter;
		Transform targetTrans = this._craftsManExtraOperatePanel.CGet<GameObject>("FocusModeElements").transform;
		bool flag = !isEnter;
		if (flag)
		{
			this._addResourceSlider.value = 0f;
			this._onCancelAddResource();
			targetTrans.SetParent(this._craftsManExtraOperatePanel.transform, true);
		}
		return targetTrans;
	}

	// Token: 0x04001378 RID: 4984
	private CButtonObsolete _btnCancelFocusMode;

	// Token: 0x04001379 RID: 4985
	private CSliderLegacy _addResourceSlider;

	// Token: 0x0400137A RID: 4986
	private CButtonObsolete _buttonLess;

	// Token: 0x0400137B RID: 4987
	private CButtonObsolete _buttonMore;

	// Token: 0x0400137E RID: 4990
	private sbyte _productResourceType;

	// Token: 0x0400137F RID: 4991
	private int _progress;

	// Token: 0x04001380 RID: 4992
	private int _resourceCount;

	// Token: 0x04001381 RID: 4993
	private Action _onCancelAddResource;

	// Token: 0x04001382 RID: 4994
	private Action<int> _setPredictProductProgress;

	// Token: 0x04001383 RID: 4995
	private CraftsManExtraOperatePanel _craftsManExtraOperatePanel;

	// Token: 0x04001384 RID: 4996
	public Dictionary<sbyte, List<ItemKey>> _putResourceDic = new Dictionary<sbyte, List<ItemKey>>();
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200038D RID: 909
public class UI_MerchantCaravanDetail : UIBase
{
	// Token: 0x170005A6 RID: 1446
	// (get) Token: 0x060035EE RID: 13806 RVA: 0x001B1FC2 File Offset: 0x001B01C2
	private Graphic TravelPathPassedGraphic
	{
		get
		{
			return this.PartWorldView.PathHolder.GetComponent<CRawImage>();
		}
	}

	// Token: 0x170005A7 RID: 1447
	// (get) Token: 0x060035EF RID: 13807 RVA: 0x001B1FD4 File Offset: 0x001B01D4
	private Graphic TravelPathRemainGraphic
	{
		get
		{
			return this.PartWorldView.CGet<Line2DGenerator>("PathHolder_Remain").GetComponent<CRawImage>();
		}
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x001B1FEC File Offset: 0x001B01EC
	public override void OnInit(ArgumentBox argsBox)
	{
		if (argsBox != null)
		{
			argsBox.Get<MerchantInfoCaravanData>("CaravanData", out this._caravanData);
		}
		this._finalSelectAreaId = -1;
		this._worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		this.PartWorldView.ScaleAndMoveRoot.transform.localScale = Vector3.one;
		this._merchantAvatarOffset = new Dictionary<sbyte, Vector2>();
		this._merchantAvatarOffset[0] = new Vector2(-26f, 0f);
		this._merchantAvatarOffset[1] = new Vector2(-20f, 0f);
		this._merchantAvatarOffset[2] = new Vector2(-35f, 0f);
		this._merchantAvatarOffset[3] = new Vector2(15f, 0f);
		this._merchantAvatarOffset[4] = new Vector2(-12f, 0f);
		this._merchantAvatarOffset[5] = new Vector2(5f, 0f);
		this._merchantAvatarOffset[6] = new Vector2(0f, 0f);
		this.PartWorldView.CurrPosPointer.gameObject.SetActive(false);
		this._merchantMapIcon = base.CGet<CaravanDetailMerchant>("MerchantMapIcon");
		this.PartWorldView.OnMouseEnterArea = new Action<short>(this.OnMouseEnterArea);
		this.PartWorldView.OnMouseExitArea = new Action<short>(this.OnMousExitArea);
	}

	// Token: 0x060035F1 RID: 13809 RVA: 0x001B2163 File Offset: 0x001B0363
	private void OnMouseEnterArea(short areaId)
	{
		this.OnMouseEnterState((int)this._worldMapModel.GetStateId(areaId));
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x001B2179 File Offset: 0x001B0379
	private void OnMousExitArea(short areaId)
	{
		this.OnMouseExitState((int)this._worldMapModel.GetStateId(areaId));
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x001B2190 File Offset: 0x001B0390
	public void OnMouseEnterState(int stateId)
	{
		bool flag = (int)this._currSelectedStateId == stateId || !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this._currSelectedStateId = (sbyte)stateId;
			Transform stateMask = this.PartWorldView.StateMaskHolder.GetChild(stateId);
			bool enabled = stateMask.GetComponent<IrregularClickableImage>().enabled;
			if (enabled)
			{
				stateMask.GetChild(0).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060035F4 RID: 13812 RVA: 0x001B2200 File Offset: 0x001B0400
	public void OnMouseExitState(int stateId)
	{
		bool flag = this._currSelectedStateId < 0 || (int)this._currSelectedStateId != stateId || !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this._currSelectedStateId = -1;
			this.PartWorldView.StateMaskHolder.GetChild(stateId).GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x060035F5 RID: 13813 RVA: 0x001B2268 File Offset: 0x001B0468
	private void Awake()
	{
		this.PartWorldView.InitAreas();
		this.PartWorldView.PathHolder.GetComponent<Line2DGenerator>().OverrideVertices = this._travelPathVerticesVisited;
		this.PartWorldView.CGet<Line2DGenerator>("PathHolder_Remain").OverrideVertices = this._travelPathVerticesNotVisited;
		MouseWheelScale scaleAndMoveRoot = this.PartWorldView.ScaleAndMoveRoot;
		scaleAndMoveRoot.OnScale = (Action<Vector3>)Delegate.Combine(scaleAndMoveRoot.OnScale, new Action<Vector3>(this.SetHighlighScale));
		this._currentHighlight.ActionPointerEnter = delegate()
		{
			this.OnMouseEnterArea((short)this._currentAreaId);
		};
		this._currentHighlight.ActionPointerExit = delegate()
		{
			this.OnMousExitArea((short)this._currentAreaId);
		};
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x001B2312 File Offset: 0x001B0512
	private void SetHighlighScale(Vector3 obj)
	{
		this._startHighlight.AdjustScaleByParentScale(obj);
		this._endHighlight.AdjustScaleByParentScale(obj);
		this._currentHighlight.AdjustScaleByParentScale(obj);
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x001B233C File Offset: 0x001B053C
	private void OnEnable()
	{
		this._caravanPathTemplateIdList = new List<short>();
		this._caravanPathStateIdSet = new HashSet<short>();
		HashSet<short> tempAreaSet = new HashSet<short>();
		this._settlementDisplayDataDic = new Dictionary<int, List<SettlementDisplayData>>();
		bool flag = this._caravanData.RemainSettlementInfoList != null;
		if (flag)
		{
			foreach (SettlementDisplayData settlementInfo in this._caravanData.RemainSettlementInfoList)
			{
				bool flag2 = !this._settlementDisplayDataDic.ContainsKey((int)settlementInfo.AreaTemplateId);
				if (flag2)
				{
					this._settlementDisplayDataDic[(int)settlementInfo.AreaTemplateId] = new List<SettlementDisplayData>();
				}
				this._settlementDisplayDataDic[(int)settlementInfo.AreaTemplateId].Add(settlementInfo);
			}
		}
		foreach (Location pathNode in this._caravanData.CaravanPath.FullPath)
		{
			MapAreaItem item = this._worldMapModel.Areas[(int)pathNode.AreaId].GetConfig();
			bool flag3 = tempAreaSet.Add(pathNode.AreaId);
			if (flag3)
			{
				this._caravanPathTemplateIdList.Add(item.TemplateId);
			}
			this._caravanPathStateIdSet.Add((short)item.StateID);
		}
		this.SetupDetailInfo();
		this.SetupMapInfo();
		this.FillAllMapStateNames();
		this.SetMapStateActive();
		this.ShowRoute();
		this.ScrollMapToMinimum();
		this.InitSelectableAreas();
	}

	// Token: 0x060035F8 RID: 13816 RVA: 0x001B24EC File Offset: 0x001B06EC
	private void ScrollMapToMinimum()
	{
		this.PartWorldView.ScaleAndMoveRoot.SetScaleProcess(0f);
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x001B2505 File Offset: 0x001B0705
	private void GetAreaDisplayData()
	{
		CommandManager.AddCommandMethodCall(EPriority.CallGetAreaDisplayData, 2, 35, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._areaDisplayData);
			this.UpdateAreaInfo();
		}, null);
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x001B2520 File Offset: 0x001B0720
	private void UpdateAreaInfo()
	{
		for (short areaId = 0; areaId < 135; areaId += 1)
		{
			AreaDisplayData displayData = this._areaDisplayData[(int)areaId];
			Refers areaRefers = this.PartWorldView.AreaHolder.GetChild((int)areaId).GetComponent<Refers>();
			areaRefers.CGet<AreaName>("AreaName").SetBroken(displayData.IsBroken);
			areaRefers.CGet<AreaName>("AreaName").ShowFiveLoongIcon(false);
			areaRefers.CGet<AreaBroken>("AreaBroken").SetBroken(displayData.IsBroken, displayData.BrokenLevel);
		}
		base.GetComponent<CanvasGroup>().alpha = 1f;
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x060035FB RID: 13819 RVA: 0x001B25CC File Offset: 0x001B07CC
	private void ShowRoute()
	{
		this._travelPathVerticesVisited.Clear();
		this._travelPathVerticesNotVisited.Clear();
		ByteCoordinate previousPos = new ByteCoordinate(0, 0);
		bool reachCurrentPos = false;
		for (int i = 0; i < this._caravanPathTemplateIdList.Count; i++)
		{
			ByteCoordinate currentPos = UI_MerchantCaravanDetail.<ShowRoute>g__ConvertPos|38_1(this._caravanPathTemplateIdList[i]);
			bool flag = i > 0 && previousPos.X != currentPos.X && previousPos.Y != currentPos.Y;
			if (flag)
			{
				this.<ShowRoute>g__InsertPos|38_0(new ByteCoordinate(currentPos.X, previousPos.Y), reachCurrentPos ? this._travelPathVerticesNotVisited : this._travelPathVerticesVisited);
			}
			bool flag2 = this._caravanPathTemplateIdList[i] == this._caravanData.CurrentAreaTemplateId;
			if (flag2)
			{
				reachCurrentPos = true;
				this.<ShowRoute>g__InsertPos|38_0(currentPos, this._travelPathVerticesVisited);
			}
			previousPos = currentPos;
			this.<ShowRoute>g__InsertPos|38_0(currentPos, reachCurrentPos ? this._travelPathVerticesNotVisited : this._travelPathVerticesVisited);
		}
		this.TravelPathPassedGraphic.SetVerticesDirty();
		this.TravelPathRemainGraphic.SetVerticesDirty();
		this.PartWorldView.PathHolder.gameObject.SetActive(true);
		this.PartWorldView.CGet<Line2DGenerator>("PathHolder_Remain").gameObject.SetActive(true);
	}

	// Token: 0x060035FC RID: 13820 RVA: 0x001B2724 File Offset: 0x001B0924
	private void SetupDetailInfo()
	{
		MerchantItem merchantItem = Merchant.Instance[(int)this._caravanData.MerchantTemplateId];
		sbyte typeId = merchantItem.MerchantType;
		MerchantTypeItem merchantType = Config.MerchantType.Instance.GetItem(typeId);
		string avatarIconPath = Path.Combine(this._avatarBasePath, merchantType.CaravanAvatar);
		CRawImage avatarIconComp = base.CGet<CRawImage>("merchantAvatar");
		Vector2 offset = this.GetMerchantAvatarOffset(merchantType.TemplateId);
		ResLoader.LoadModOrGameResource<Texture2D>(avatarIconPath ?? "", delegate(Texture2D texture)
		{
			avatarIconComp.texture = texture;
			avatarIconComp.rectTransform.anchoredPosition = this._baseAnchoredPosition + offset;
		}, null);
		base.CGet<CImage>("merchantIcon").SetSprite(merchantType.Icon, false, null);
		string dotStr = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		TextMeshProUGUI txt_MerchantName = base.CGet<TextMeshProUGUI>("txt_MerchantName");
		string levelStr = CommonUtils.GetMerchantLevel(merchantItem.Level + 1);
		txt_MerchantName.text = string.Concat(new string[]
		{
			merchantType.Name,
			" ",
			dotStr,
			" ",
			levelStr
		});
		MapAreaItem mapAreaItem = MapArea.Instance[this._caravanData.CurrentAreaTemplateId];
		MapStateItem mapStateItem = MapState.Instance[mapAreaItem.StateID];
		base.CGet<TextMeshProUGUI>("txt_CurrentPosition").text = mapStateItem.Name + dotStr + mapAreaItem.Name;
		mapAreaItem = MapArea.Instance[this._caravanData.TargetAreaTemplateId];
		mapStateItem = MapState.Instance[mapAreaItem.StateID];
		base.CGet<TextMeshProUGUI>("txt_Destination").text = mapStateItem.Name + dotStr + mapAreaItem.Name;
		base.CGet<TextMeshProUGUI>("txt_PassThrough").text = LocalStringManager.GetFormat(LanguageKey.LK_UIMerchantCaravanDetail_CaravanPassthrough, this._caravanData.RemainSettlementCount);
		base.CGet<TextMeshProUGUI>("txt_TimeCost").text = LocalStringManager.GetFormat(LanguageKey.LK_UIMerchantCaravanDetail_TimeCostMonth, this._caravanData.RemainNodeCount);
		int rate = GameData.Domains.Merchant.SharedMethods.GetCaravanRobbedRate((int)this._caravanData.ExtraData.RobbedRate, this._caravanData.IsInBrokenArea) / 10;
		string color = this._caravanData.IsInBrokenArea ? "brightred" : "grey";
		string valueText = string.Format("{0}%", rate).SetColor(color);
		base.CGet<TextMeshProUGUI>("txt_EncounterRobberyChance").text = valueText;
		int incomeBonus = (int)(this._caravanData.ExtraData.IncomeBonus / 10);
		color = ((incomeBonus > 100) ? "brightblue" : ((incomeBonus < 100) ? "brightred" : "grey"));
		valueText = string.Format("{0}%", incomeBonus).SetColor(color);
		base.CGet<TextMeshProUGUI>("txt_Profit").text = valueText;
		bool isInvested = this._caravanData.ExtraData.IsInvested;
		if (isInvested)
		{
			color = ((this._caravanData.ExtraData.IncomeBonus > 1000) ? "brightblue" : ((this._caravanData.ExtraData.IncomeBonus < 1000) ? "brightred" : "grey"));
			int income = this._caravanData.GetInvestIncome();
			valueText = CommonUtils.GetDisplayStringForNum(income, 100000).SetColor(color);
		}
		else
		{
			valueText = 0.ToString();
		}
		base.CGet<TextMeshProUGUI>("txt_InvestProfit").text = valueText;
		int criticalRate = (int)(this._caravanData.ExtraData.IncomeCriticalRate / 10);
		float criticalResult = (float)this._caravanData.ExtraData.IncomeCriticalResult / 100f;
		criticalResult = (float)Math.Round((double)criticalResult, 1);
		valueText = LocalStringManager.GetFormat(LanguageKey.LK_MerchantInfo_Invest_Critial, criticalRate, criticalResult);
		base.CGet<TextMeshProUGUI>("txt_Windfall").text = valueText;
	}

	// Token: 0x060035FD RID: 13821 RVA: 0x001B2B04 File Offset: 0x001B0D04
	private void SetupMapInfo()
	{
		MerchantItem merchantItem = Merchant.Instance[(int)this._caravanData.MerchantTemplateId];
		sbyte typeId = merchantItem.MerchantType;
		this._merchantMapIcon.SetMerchantType(typeId);
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x001B2B3C File Offset: 0x001B0D3C
	private Vector2 GetMerchantAvatarOffset(sbyte merchantTypeTemplateId)
	{
		return this._merchantAvatarOffset[merchantTypeTemplateId];
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x001B2B5C File Offset: 0x001B0D5C
	private void OnDisable()
	{
		this.PartWorldView.ScaleAndMoveRoot.Reset();
		TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SelectMapAreaOver", "SelectedAreaId", (int)this._finalSelectAreaId);
		TaiwuEventDomainMethod.Call.TriggerListener("SelectMapAreaOver", true);
		for (int i = 0; i < this.PartWorldView.StateMaskHolder.childCount; i++)
		{
			Transform stateMask = this.PartWorldView.StateMaskHolder.GetChild(i);
			bool enabled = stateMask.GetComponent<IrregularClickableImage>().enabled;
			if (enabled)
			{
				stateMask.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x001B2BF4 File Offset: 0x001B0DF4
	private void FillAllMapStateNames()
	{
		Transform stateNamesRoot = base.CGet<GameObject>("StateNameHolder").transform;
		MapState.Instance.Iterate(delegate(MapStateItem e)
		{
			bool flag = e.TemplateId <= 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Transform child = stateNamesRoot.GetChild((int)(e.TemplateId - 1));
				bool flag2 = null == child;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this._caravanPathStateIdSet.Contains((short)e.TemplateId);
					if (flag3)
					{
						child.gameObject.SetActive(false);
						result = true;
					}
					else
					{
						child.gameObject.SetActive(true);
						child.GetComponent<TextMeshProUGUI>().text = e.Name;
						result = true;
					}
				}
			}
			return result;
		});
	}

	// Token: 0x06003601 RID: 13825 RVA: 0x001B2C3C File Offset: 0x001B0E3C
	private void SetMapStateActive()
	{
		Transform stateMaskRoot = this.PartWorldView.StateMaskHolder;
		MapState.Instance.Iterate(delegate(MapStateItem e)
		{
			bool flag = e.TemplateId <= 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Transform child = stateMaskRoot.GetChild((int)(e.TemplateId - 1));
				bool flag2 = null == child;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this._caravanPathStateIdSet.Contains((short)e.TemplateId);
					if (flag3)
					{
						child.gameObject.SetActive(false);
						result = true;
					}
					else
					{
						child.gameObject.SetActive(true);
						result = true;
					}
				}
			}
			return result;
		});
	}

	// Token: 0x06003602 RID: 13826 RVA: 0x001B2C80 File Offset: 0x001B0E80
	private void InitSelectableAreas()
	{
		this._stateAreasMap = new Dictionary<sbyte, List<Refers>>();
		Refers[] areaRefers = this.PartWorldView.transform.Find("Background/AreaHolder").GetComponentsInTopChildren(true);
		int i = 0;
		int max = areaRefers.Length;
		while (i < max)
		{
			Refers refers = areaRefers[i];
			MapAreaItem config = this._worldMapModel.Areas[refers.UserInt].GetConfig();
			bool canSelect = this._caravanPathTemplateIdList.Contains(config.TemplateId);
			MapAreaItem areaConfig = MapArea.Instance.GetItem(config.TemplateId);
			MapStateItem stateConfig = MapState.Instance.GetItem(config.StateID);
			CImage highlightComp = refers.CGet<CImage>("HighlightImage");
			PointerTrigger highlightPointerTrigger = refers.GetComponent<PointerTrigger>();
			highlightComp.enabled = (config.TemplateId != this._caravanData.StartAreaTemplateId && config.TemplateId != this._caravanData.TargetAreaTemplateId && config.TemplateId != this._caravanData.CurrentAreaTemplateId);
			bool flag = config.TemplateId == this._caravanData.StartAreaTemplateId;
			if (flag)
			{
				this._startHighlight.transform.position = highlightComp.transform.position;
				this._startHighlight.transform.localScale = Vector3.one;
				this._startHighlight.gameObject.SetActive(true);
				this._startAreaId = refers.UserInt;
			}
			bool flag2 = config.TemplateId == this._caravanData.TargetAreaTemplateId;
			if (flag2)
			{
				this._endHighlight.transform.position = highlightComp.transform.position;
				this._endHighlight.transform.localScale = Vector3.one;
				this._endHighlight.gameObject.SetActive(true);
				this._targetAreaId = refers.UserInt;
			}
			bool flag3 = config.TemplateId == this._caravanData.CurrentAreaTemplateId;
			if (flag3)
			{
				this._currentHighlight.transform.position = highlightComp.transform.position;
				this._currentHighlight.transform.localScale = Vector3.one;
				this._currentHighlight.gameObject.SetActive(true);
				this._currentAreaId = refers.UserInt;
			}
			bool flag4 = !this._caravanPathStateIdSet.Contains((short)areaConfig.StateID);
			if (flag4)
			{
				refers.gameObject.SetActive(false);
			}
			else
			{
				refers.gameObject.SetActive(true);
				TooltipInvoker tipComp = refers.CGet<TooltipInvoker>("MouseTipDisplay");
				tipComp.Type = TipType.CaravanPathDetail;
				TooltipInvoker tooltipInvoker = tipComp;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tipComp.RuntimeParam.Clear();
				List<SettlementDisplayData> areaSettlementData;
				this._settlementDisplayDataDic.TryGetValue((int)config.TemplateId, out areaSettlementData);
				bool flag5 = areaSettlementData != null && areaSettlementData.Count > 0;
				if (flag5)
				{
					tipComp.enabled = true;
					tipComp.RuntimeParam.SetObject("DataList", areaSettlementData);
					tipComp.RuntimeParam.Set("AreaName", config.Name);
					tipComp.RuntimeParam.Set("StateName", stateConfig.Name);
				}
				else
				{
					tipComp.enabled = false;
				}
				refers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!canSelect, false);
				refers.GetComponent<CButtonObsolete>().interactable = canSelect;
				bool flag6 = refers.UserInt == (int)this._worldMapModel.CurrentAreaId;
				if (flag6)
				{
					this.PartWorldView.CurrPosPointer.position = refers.transform.position;
				}
				else
				{
					bool flag7 = !this._stateAreasMap.ContainsKey(config.StateID);
					if (flag7)
					{
						this._stateAreasMap.Add(config.StateID, new List<Refers>());
					}
					this._stateAreasMap[config.StateID].Add(refers);
				}
			}
			i++;
		}
		this.GetAreaDisplayData();
	}

	// Token: 0x06003603 RID: 13827 RVA: 0x001B3090 File Offset: 0x001B1290
	protected override void OnClick(Transform btn)
	{
		base.OnClick(btn);
		bool flag = btn.name == "Close";
		if (flag)
		{
			UIManager.Instance.HideUI(UIElement.MerchantCaravanDetail);
		}
		else
		{
			bool flag2 = btn.name == "CurrentButton";
			if (flag2)
			{
				this.PartWorldView.ResetScaleAndFocusArea((short)this._currentAreaId);
				this.SetHighlighScale(Vector3.one);
			}
			else
			{
				bool flag3 = btn.name == "DestinationButton";
				if (flag3)
				{
					this.PartWorldView.ResetScaleAndFocusArea((short)this._targetAreaId);
					this.SetHighlighScale(Vector3.one);
				}
			}
		}
	}

	// Token: 0x06003608 RID: 13832 RVA: 0x001B31DC File Offset: 0x001B13DC
	[CompilerGenerated]
	private void <ShowRoute>g__InsertPos|38_0(ByteCoordinate pos, List<Vector2> verticesList)
	{
		Vector2 anchoredPos = this.PartWorldView.GetAnchoredPos((int)pos.X, (int)pos.Y);
		anchoredPos += this.PartWorldView.PathHolder.offsetMin;
		int index = verticesList.IndexOf(anchoredPos);
		bool flag = index >= 0;
		if (flag)
		{
			Vector2 tempAnchoredPos = verticesList[verticesList.Count - 1] + this._repeatOffset;
			verticesList.Add(tempAnchoredPos);
		}
		verticesList.Add(anchoredPos);
	}

	// Token: 0x06003609 RID: 13833 RVA: 0x001B3258 File Offset: 0x001B1458
	[CompilerGenerated]
	internal static ByteCoordinate <ShowRoute>g__ConvertPos|38_1(short templateOd)
	{
		sbyte[] configPos = MapArea.Instance.GetItem(templateOd).WorldMapPos;
		return new ByteCoordinate((byte)configPos[0], (byte)configPos[1]);
	}

	// Token: 0x0400271C RID: 10012
	public PartWorldView PartWorldView;

	// Token: 0x0400271D RID: 10013
	private CaravanDetailMerchant _merchantMapIcon;

	// Token: 0x0400271E RID: 10014
	private MerchantInfoCaravanData _caravanData;

	// Token: 0x0400271F RID: 10015
	private WorldMapModel _worldMapModel;

	// Token: 0x04002720 RID: 10016
	private Dictionary<sbyte, List<Refers>> _stateAreasMap;

	// Token: 0x04002721 RID: 10017
	private Dictionary<sbyte, Vector2> _merchantAvatarOffset;

	// Token: 0x04002722 RID: 10018
	private Dictionary<int, List<SettlementDisplayData>> _settlementDisplayDataDic;

	// Token: 0x04002723 RID: 10019
	private short _finalSelectAreaId;

	// Token: 0x04002724 RID: 10020
	private readonly string _avatarBasePath = "NpcFace/BigFace";

	// Token: 0x04002725 RID: 10021
	private Vector2 _baseAnchoredPosition = new Vector2(-214.71f, 12f);

	// Token: 0x04002726 RID: 10022
	private List<short> _caravanPathTemplateIdList;

	// Token: 0x04002727 RID: 10023
	private HashSet<short> _caravanPathStateIdSet;

	// Token: 0x04002728 RID: 10024
	[SerializeField]
	private UIFixedScaleChildrenManual _startHighlight;

	// Token: 0x04002729 RID: 10025
	[SerializeField]
	private UIFixedScaleChildrenManual _endHighlight;

	// Token: 0x0400272A RID: 10026
	[SerializeField]
	private CaravanDetailMerchant _currentHighlight;

	// Token: 0x0400272B RID: 10027
	private readonly List<Vector2> _travelPathVerticesVisited = new List<Vector2>();

	// Token: 0x0400272C RID: 10028
	private readonly List<Vector2> _travelPathVerticesNotVisited = new List<Vector2>();

	// Token: 0x0400272D RID: 10029
	private int _currentAreaId;

	// Token: 0x0400272E RID: 10030
	private int _targetAreaId;

	// Token: 0x0400272F RID: 10031
	private int _startAreaId;

	// Token: 0x04002730 RID: 10032
	private sbyte _currSelectedStateId = -1;

	// Token: 0x04002731 RID: 10033
	private AreaDisplayData[] _areaDisplayData;

	// Token: 0x04002732 RID: 10034
	public Vector2 _repeatOffset = new Vector2(1f, 1f);
}

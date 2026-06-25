using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000730 RID: 1840
	public class ViewMerchantCaravanDetail : UIBase
	{
		// Token: 0x060057F1 RID: 22513 RVA: 0x0028CE80 File Offset: 0x0028B080
		private void Awake()
		{
			this.scale.OnScale = delegate(Vector3 xyz)
			{
				this.areaMap.OnScale(1f / xyz.x, -1f);
			};
			this.currLoc.onClick.ResetListener(delegate()
			{
				this.areaMap.LookAtTemplate(this._caravanData.CurrentAreaTemplateId, 0f, default(Vector2));
			});
			this.fromLoc.onClick.ResetListener(delegate()
			{
				this.areaMap.LookAtTemplate(this._caravanData.StartAreaTemplateId, 0f, default(Vector2));
			});
			this.toLoc.onClick.ResetListener(delegate()
			{
				this.areaMap.LookAtTemplate(this._caravanData.TargetAreaTemplateId, 0f, default(Vector2));
			});
			this.areaMap.PostRender = delegate(Area area, AreaDisplayData data)
			{
				TooltipInvoker tipComp = area.Displayer;
				tipComp.Type = TipType.CaravanPathDetail;
				TooltipInvoker tooltipInvoker = tipComp;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				List<SettlementDisplayData> areaSettlementData;
				bool flag = this._settlementDisplayDataDic.TryGetValue((int)area.Config.TemplateId, out areaSettlementData) && areaSettlementData != null && areaSettlementData.Count > 0;
				if (flag)
				{
					tipComp.enabled = true;
					tipComp.RuntimeParam.SetObject("DataList", areaSettlementData);
					tipComp.RuntimeParam.Set("AreaName", area.Config.Name);
					tipComp.RuntimeParam.Set("StateName", MapState.Instance[area.Config.StateID].Name);
				}
				else
				{
					tipComp.enabled = false;
				}
			};
		}

		// Token: 0x060057F2 RID: 22514 RVA: 0x0028CF13 File Offset: 0x0028B113
		public override void QuickHide()
		{
			AreaStateItemController.Checker.ForceHide = false;
			base.QuickHide();
		}

		// Token: 0x060057F3 RID: 22515 RVA: 0x0028CF28 File Offset: 0x0028B128
		public override void OnInit(ArgumentBox argsBox)
		{
			ViewMerchantCaravanDetail.<>c__DisplayClass31_0 CS$<>8__locals1 = new ViewMerchantCaravanDetail.<>c__DisplayClass31_0();
			CS$<>8__locals1.<>4__this = this;
			AreaStateItemController.Checker.ForceHide = true;
			if (this._travelRouteHelper == null)
			{
				this._travelRouteHelper = new TravelRouteHelper<Vector3>(delegate(short index)
				{
					float[] pos = MapArea.Instance[index].RoadPos;
					return new Vector3(pos[0], pos[1], (float)(CS$<>8__locals1.<>4__this._targets.Contains(CS$<>8__locals1.<>4__this._travelRouteHelper.FromId) ? -1 : 1));
				}, (float[] pos) => new Vector3(pos[0], pos[1], (float)(CS$<>8__locals1.<>4__this._targets.Contains(CS$<>8__locals1.<>4__this._travelRouteHelper.FromId) ? -1 : 1)));
			}
			if (argsBox != null)
			{
				argsBox.Get<MerchantInfoCaravanData>("CaravanData", out this._caravanData);
			}
			this._settlementDisplayDataDic.Clear();
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
			MerchantItem merchant = Merchant.Instance[(int)this._caravanData.MerchantTemplateId];
			this._worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			this.areaMap.transform.localScale = Vector3.one;
			this.merchantMapIcon.sprite = this.merchantMapIcons[(int)merchant.MerchantType];
			this.merchantName.text = merchant.UiName;
			CS$<>8__locals1.dotStr = LanguageKey.LK_Dot_Symbol.Tr();
			this.reward.text = LanguageKey.LK_ViewMerchantCaravanDetail_ProfitPercentage.TrFormat((float)this._caravanData.ExtraData.IncomeBonus * 0.1f).ColorReplace();
			this.rewardRatio.text = LanguageKey.LK_ViewMerchantCaravanDetail_InvestProfit.TrFormat(this._caravanData.ExtraData.IsInvested ? CommonUtils.GetDisplayStringForNum(this._caravanData.GetInvestIncome(), 100000).SetColor((this._caravanData.ExtraData.IncomeBonus > 1000) ? "brightblue" : ((this._caravanData.ExtraData.IncomeBonus < 1000) ? "brightred" : "grey")) : "0").ColorReplace();
			this.luck.text = LanguageKey.LK_ViewMerchantCaravanDetail_Windfall.TrFormat((int)(this._caravanData.ExtraData.IncomeCriticalRate / 10), (float)this._caravanData.ExtraData.IncomeCriticalResult / 100f).ColorReplace();
			this.settlementRemain.text = LanguageKey.LK_ViewMerchantCaravanDetail_CaravanRouteTitle.TrFormat(this._caravanData.RemainSettlementCount).ColorReplace();
			this.arriveCost.text = LanguageKey.LK_ViewMerchantCaravanDetail_TimeCost.TrFormat(this._caravanData.RemainNodeCount).ColorReplace();
			this.robProb.text = LanguageKey.LK_ViewMerchantCaravanDetail_RobbedPercentage.TrFormat(string.Format("{0}%", (float)GameData.Domains.Merchant.SharedMethods.GetCaravanRobbedRate((int)this._caravanData.ExtraData.RobbedRate, this._caravanData.IsInBrokenArea) * 0.1f).SetColor(this._caravanData.IsInBrokenArea ? "brightred" : "brightyellow")).ColorReplace();
			this.currLocation.text = LanguageKey.LK_ViewMerchantCaravanDetail_CurrentLocation.TrFormat(CS$<>8__locals1.<OnInit>g__GetLoc|5(this._caravanData.CurrentAreaTemplateId)).ColorReplace();
			this.fromLocation.text = LanguageKey.LK_ViewMerchantCaravanDetail_FromLocation.TrFormat(CS$<>8__locals1.<OnInit>g__GetLoc|5(this._caravanData.StartAreaTemplateId)).ColorReplace();
			this.toLocation.text = LanguageKey.LK_ViewMerchantCaravanDetail_ToLocation.TrFormat(CS$<>8__locals1.<OnInit>g__GetLoc|5(this._caravanData.TargetAreaTemplateId)).ColorReplace();
			this.areaMap.Init(true, this, false);
			this.areaMap.generator.Vertices = Array.Empty<Vector3>();
			this._passed.Clear();
			this._targets.Clear();
			HashSet<short> tempAreaSet = EasyPool.Get<HashSet<short>>();
			bool currMet = false;
			foreach (short pathNode in from x in this._caravanData.CaravanPath.FullPath
			select x.AreaId)
			{
				bool flag3 = tempAreaSet.Add(pathNode);
				if (flag3)
				{
					short id = this._worldMapModel.Areas[(int)pathNode].GetTemplateId();
					(currMet ? this._targets : this._passed).Add(id);
					bool flag4 = id == this._caravanData.CurrentAreaTemplateId;
					if (flag4)
					{
						currMet = true;
						this._targets.Add(id);
					}
				}
			}
			this.start.Target = this.areaMap.GetTransform(this._worldMapModel.Areas[(int)this._caravanData.CaravanPath.FullPath[0].AreaId].GetTemplateId());
			this.taiwu.Target = this.areaMap.GetTransform(this._worldMapModel.Areas[(int)this._caravanData.CaravanPath.GetCurrLocation().AreaId].GetTemplateId());
			PositionFollower positionFollower = this.end;
			AreaMap areaMap = this.areaMap;
			MapAreaData[] areas = this._worldMapModel.Areas;
			List<Location> fullPath = this._caravanData.CaravanPath.FullPath;
			positionFollower.Target = areaMap.GetTransform(areas[(int)fullPath[fullPath.Count - 1].AreaId].GetTemplateId());
			this.areaMap.generator.Vertices = this._passed.Concat(this._targets).Zip(this._passed.Skip(1).Concat(this._targets), (short fromId, short toId) => CS$<>8__locals1.<>4__this._travelRouteHelper.GetRoute(fromId, toId)).SelectMany((IEnumerable<Vector3> item) => item).ToArray<Vector3>();
			ViewMerchantCaravanDetail.<>c__DisplayClass31_0 CS$<>8__locals2 = CS$<>8__locals1;
			int version = this._version + 1;
			this._version = version;
			CS$<>8__locals2.version = version;
			ResLoader.LoadModOrGameResource<Texture2D>("NpcFace/NormalFace/" + Config.MerchantType.Instance[merchant.MerchantType].CaravanAvatar, delegate(Texture2D texture)
			{
				bool flag5 = CS$<>8__locals1.<>4__this._version != CS$<>8__locals1.version;
				if (!flag5)
				{
					CS$<>8__locals1.<>4__this.avatar.Refresh(texture);
					CS$<>8__locals1.<>4__this.avatar.gameObject.SetActive(true);
				}
			}, null);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060057F4 RID: 22516 RVA: 0x0028D5E8 File Offset: 0x0028B7E8
		public void RequestData()
		{
			AreaMap.RequestData(this, delegate(AreaDisplayData[] data)
			{
				this.areaMap.Refresh(data);
				this.areaMap.LookAtTemplate(this._caravanData.CurrentAreaTemplateId, 0f, default(Vector2));
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x04003C5E RID: 15454
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003C5F RID: 15455
		[SerializeField]
		private AreaMap areaMap;

		// Token: 0x04003C60 RID: 15456
		[SerializeField]
		private PositionFollower start;

		// Token: 0x04003C61 RID: 15457
		[SerializeField]
		private PositionFollower taiwu;

		// Token: 0x04003C62 RID: 15458
		[SerializeField]
		private PositionFollower end;

		// Token: 0x04003C63 RID: 15459
		[SerializeField]
		private CButton currLoc;

		// Token: 0x04003C64 RID: 15460
		[SerializeField]
		private CButton fromLoc;

		// Token: 0x04003C65 RID: 15461
		[SerializeField]
		private CButton toLoc;

		// Token: 0x04003C66 RID: 15462
		[SerializeField]
		private CImage merchantMapIcon;

		// Token: 0x04003C67 RID: 15463
		[SerializeField]
		private Sprite[] merchantMapIcons;

		// Token: 0x04003C68 RID: 15464
		[SerializeField]
		private MouseWheelScale scale;

		// Token: 0x04003C69 RID: 15465
		[SerializeField]
		private TMP_Text merchantName;

		// Token: 0x04003C6A RID: 15466
		[SerializeField]
		private TMP_Text reward;

		// Token: 0x04003C6B RID: 15467
		[SerializeField]
		private TMP_Text rewardRatio;

		// Token: 0x04003C6C RID: 15468
		[SerializeField]
		private TMP_Text luck;

		// Token: 0x04003C6D RID: 15469
		[SerializeField]
		private TMP_Text settlementRemain;

		// Token: 0x04003C6E RID: 15470
		[SerializeField]
		private TMP_Text robProb;

		// Token: 0x04003C6F RID: 15471
		[SerializeField]
		private TMP_Text arriveCost;

		// Token: 0x04003C70 RID: 15472
		[SerializeField]
		private TMP_Text currLocation;

		// Token: 0x04003C71 RID: 15473
		[SerializeField]
		private TMP_Text fromLocation;

		// Token: 0x04003C72 RID: 15474
		[SerializeField]
		private TMP_Text toLocation;

		// Token: 0x04003C73 RID: 15475
		private AreaDisplayData[] _areaDisplayData;

		// Token: 0x04003C74 RID: 15476
		private MerchantInfoCaravanData _caravanData;

		// Token: 0x04003C75 RID: 15477
		private WorldMapModel _worldMapModel;

		// Token: 0x04003C76 RID: 15478
		private TravelRouteHelper<Vector3> _travelRouteHelper;

		// Token: 0x04003C77 RID: 15479
		private Dictionary<int, List<SettlementDisplayData>> _settlementDisplayDataDic = new Dictionary<int, List<SettlementDisplayData>>();

		// Token: 0x04003C78 RID: 15480
		private int _version;

		// Token: 0x04003C79 RID: 15481
		private readonly List<short> _passed = new List<short>();

		// Token: 0x04003C7A RID: 15482
		private readonly List<short> _targets = new List<short>();
	}
}

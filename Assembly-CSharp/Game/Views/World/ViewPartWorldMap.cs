using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.LineGenerator;
using Game.Components.SkeletonAnim;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000731 RID: 1841
	public class ViewPartWorldMap : UIBase
	{
		// Token: 0x060057FC RID: 22524 RVA: 0x0028D80C File Offset: 0x0028BA0C
		public static void LookAtTaiwu()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapLookAt, EasyPool.Get<ArgumentBox>().Set("AreaId", SingletonObject.getInstance<WorldMapModel>().CurrentAreaId));
		}

		// Token: 0x060057FD RID: 22525 RVA: 0x0028D834 File Offset: 0x0028BA34
		public static void LookAtTaiwuVillage()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapLookAt, EasyPool.Get<ArgumentBox>().Set("AreaId", SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId()));
		}

		// Token: 0x060057FE RID: 22526 RVA: 0x0028D85C File Offset: 0x0028BA5C
		public static void TravelToTaiwuVillage()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapDirectReturn, null);
		}

		// Token: 0x060057FF RID: 22527 RVA: 0x0028D86C File Offset: 0x0028BA6C
		public static void Hide()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapHide, EasyPool.Get<ArgumentBox>().Set("AreaId", SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId()));
		}

		// Token: 0x06005800 RID: 22528 RVA: 0x0028D894 File Offset: 0x0028BA94
		public static void RefreshData()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapDataChanged, null);
		}

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x06005801 RID: 22529 RVA: 0x0028D8A4 File Offset: 0x0028BAA4
		public static bool IsBlockInteraction
		{
			get
			{
				ViewPartWorldMap map = UIElement.PartWorld.UiBase as ViewPartWorldMap;
				bool flag = map == null;
				return !flag && (map._startTravel && !map._isTravelAutoPaused) && !map.pauseToggle.isOn;
			}
		}

		// Token: 0x06005802 RID: 22530 RVA: 0x0028D8F6 File Offset: 0x0028BAF6
		public static void HighlightArea(short areaId)
		{
			GEvent.OnEvent(UiEvents.PartWorldMapHighlight, EasyPool.Get<ArgumentBox>().Set("AreaId", areaId));
		}

		// Token: 0x06005803 RID: 22531 RVA: 0x0028D915 File Offset: 0x0028BB15
		public static void ClearHighlight()
		{
			GEvent.OnEvent(UiEvents.PartWorldMapHighlight, EasyPool.Get<ArgumentBox>().Set("AreaId", -1));
		}

		// Token: 0x06005804 RID: 22532 RVA: 0x0028D934 File Offset: 0x0028BB34
		public static TravelSkeletonItem GetSkeleton(short carrierId)
		{
			return (carrierId < 0) ? Config.TravelSkeleton.Instance[55] : Config.TravelSkeleton.Instance[Carrier.Instance[carrierId].TravelSkeleton];
		}

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06005805 RID: 22533 RVA: 0x0028D962 File Offset: 0x0028BB62
		private float StepAnimationDurationWithoutLimit
		{
			get
			{
				return 0.15f * (float)this._nextAreaCostDays;
			}
		}

		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x06005806 RID: 22534 RVA: 0x0028D971 File Offset: 0x0028BB71
		private float StepWaitDuration
		{
			get
			{
				return Math.Max(this.StepAnimationDurationWithoutLimit, 1.5f);
			}
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x06005807 RID: 22535 RVA: 0x0028D983 File Offset: 0x0028BB83
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x06005808 RID: 22536 RVA: 0x0028D98F File Offset: 0x0028BB8F
		private bool IsWorldTravelUnlocked
		{
			get
			{
				return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(4);
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x06005809 RID: 22537 RVA: 0x0028D99C File Offset: 0x0028BB9C
		private bool CanTravelInteractable
		{
			get
			{
				CrossAreaMoveInfo moveInfo = this._moveInfo;
				return moveInfo != null && moveInfo.Traveling && this._moveInfo.CurrentAreaId != this._moveInfo.ToAreaId;
			}
		}

		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x0600580A RID: 22538 RVA: 0x0028D9D9 File Offset: 0x0028BBD9
		private bool TaiwuIsKid
		{
			get
			{
				return this._taiwuDisplayData.AvatarRelatedData.DisplayAge < 16;
			}
		}

		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x0600580B RID: 22539 RVA: 0x0028D9EF File Offset: 0x0028BBEF
		private TravelSkeletonItem TravelSkeleton
		{
			get
			{
				return ViewPartWorldMap.GetSkeleton(this._kidnappedTravelData.Valid ? 56 : this._mapModel.TaiwuCarrier);
			}
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x0028DA14 File Offset: 0x0028BC14
		public float GetZoomValueFromLevel(int level)
		{
			if (!true)
			{
			}
			float result;
			if (!this._startTravel)
			{
				if (level >= 1)
				{
					switch (level)
					{
					case 1:
						result = 1f;
						break;
					case 2:
						result = 0.9f;
						break;
					case 3:
						result = 0.8f;
						break;
					case 4:
						result = 0.7f;
						break;
					case 5:
						result = 0.6f;
						break;
					case 6:
						result = 0.5f;
						break;
					case 7:
						result = 0.4f;
						break;
					case 8:
						result = 0.35f;
						break;
					case 9:
						result = 0.3f;
						break;
					default:
						result = 0.2f;
						break;
					}
				}
				else
				{
					result = 1.25f;
				}
			}
			else
			{
				result = 1.25f;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x0600580D RID: 22541 RVA: 0x0028DACA File Offset: 0x0028BCCA
		// (set) Token: 0x0600580E RID: 22542 RVA: 0x0028DAD4 File Offset: 0x0028BCD4
		public int ZoomLevel
		{
			get
			{
				return this.zoomLevel;
			}
			set
			{
				bool startTravel = this._startTravel;
				if (startTravel)
				{
					value = 0;
				}
				bool flag = Mathf.RoundToInt(this.zoom.value) != (this.zoomLevel = value);
				if (flag)
				{
					this.zoom.SetValueWithoutNotify((float)Mathf.RoundToInt((float)value));
				}
				float zoomValue = this.GetZoomValueFromLevel(this.zoomLevel);
				this.map.BeforeScale(this._centerZoom);
				this.map.transform.localScale = Vector3.one * zoomValue;
				float nameScaleFactor = TargetScaleAlign.GetAlignScale(zoomValue, new Vector2(0.5f, 1f), new Vector2(1.5f, 1f), false);
				this.map.OnScale(1f / zoomValue, nameScaleFactor);
				foreach (PositionFollower item in this.twelveFollower)
				{
					item.transform.localScale = Vector3.one * nameScaleFactor;
				}
				this.anim.transform.localScale = Vector3.one * nameScaleFactor * 0.1f;
				bool flag2 = this._towards < 0f;
				if (flag2)
				{
					this.anim.transform.localScale = new Vector3(-this.anim.transform.localScale.x, this.anim.transform.localScale.y, this.anim.transform.localScale.z);
				}
				this.map.dragMove.AdjustOffsetAfterScale();
				Action afterClampCallback = this.map.dragMove.AfterClampCallback;
				if (afterClampCallback != null)
				{
					afterClampCallback();
				}
				bool zoomMode = this.zoomLevel < this.zoomThreshold;
				this.map.SetStateHover(true);
				this.map.RefreshShowAreaState(zoomMode);
			}
		}

		// Token: 0x0600580F RID: 22543 RVA: 0x0028DCC0 File Offset: 0x0028BEC0
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 163, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 164, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 15, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 204, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(2, 56, ulong.MaxValue, null));
		}

		// Token: 0x06005810 RID: 22544 RVA: 0x0028DD50 File Offset: 0x0028BF50
		public void OnClickState(short stateTemplateId)
		{
			MapStateItem state = MapState.Instance[(int)stateTemplateId];
			State stateObject;
			bool flag;
			if (this.zoomLevel > 5)
			{
				stateObject = this.map.states.FirstOrDefault((State item) => (short)item.Config.TemplateId == stateTemplateId);
				flag = (stateObject == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				this.clickBlocker.SetActive(true);
				DOTween.To(() => this.map.transform.localScale.x, delegate(float value)
				{
					this.map.transform.localScale = Vector3.one * value;
				}, this.GetZoomValueFromLevel(5), 1f).OnComplete(delegate
				{
					this.ZoomLevel = 5;
					this.clickBlocker.SetActive(false);
				});
				this.map.LookAt(stateObject.transform.localPosition, 1f, this.GetZoomValueFromLevel(5), default(Vector2));
				this.map.SelectedAreaTemplateId = -1;
			}
		}

		// Token: 0x06005811 RID: 22545 RVA: 0x0028DE44 File Offset: 0x0028C044
		private void OnClickArea(short areaId)
		{
			bool flag = areaId == this._mapModel.CurrentAreaId || this._travelDestAreaId >= 0 || this._moveInfo.Traveling || this._travelIsFinish || this._currSelectedAreaMoveInfo == null;
			if (flag)
			{
				this.map.SelectedAreaTemplateId = -1;
			}
			else
			{
				this._travelDestAreaId = areaId;
				this.ShowTravelTipsDialog(this._currSelectedAreaMoveInfo);
			}
		}

		// Token: 0x06005812 RID: 22546 RVA: 0x0028DEB2 File Offset: 0x0028C0B2
		private void ShowTravelTipsDialog(CrossAreaMoveInfo moveInfo)
		{
			TravelUtils.ShowTravelConfirmDialog(moveInfo, new Action(this.ConfirmTravel), new Action(this.CancelTravel));
			this.EnableStateMask(false, false);
		}

		// Token: 0x06005813 RID: 22547 RVA: 0x0028DEDD File Offset: 0x0028C0DD
		private void ConfirmTravel()
		{
			this.EnableStateMask(true, true);
			this.map.CancelSelect();
			MapDomainMethod.Call.StartTravel(this._travelDestAreaId);
		}

		// Token: 0x06005814 RID: 22548 RVA: 0x0028DF04 File Offset: 0x0028C104
		private void ConfirmDirectReturn(ArgumentBox _)
		{
			this._travelDestAreaId = this._mapModel.GetTaiwuVillageAreaId();
			CommandManager.AddCommandMethodCall<short, short, short>(EPriority.CallMethodNormal, 2, 11, this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, this._mapModel.GetTaiwuVillageAreaId(), delegate(int offset, RawDataPool pool)
			{
				CrossAreaMoveInfo moveInfo = new CrossAreaMoveInfo();
				Serializer.Deserialize(pool, offset, ref moveInfo);
				moveInfo.AuthorityCost = 0;
				TravelUtils.ShowDirectTravelConfirmDialog(moveInfo, new Action(MapDomainMethod.Call.DirectTravelToTaiwuVillage), delegate
				{
					this._travelDestAreaId = -1;
				});
			}, null);
		}

		// Token: 0x06005815 RID: 22549 RVA: 0x0028DF5B File Offset: 0x0028C15B
		private void CancelTravel()
		{
			this._travelDestAreaId = -1;
			this.map.CancelSelect();
			this.ClearTravelPath();
			this.EnableStateMask(true, true);
		}

		// Token: 0x06005816 RID: 22550 RVA: 0x0028DF81 File Offset: 0x0028C181
		private void SetZoomLevel(float val)
		{
			this._centerZoom = true;
			this.zoom.SetValueWithoutNotify((float)Mathf.RoundToInt(val));
			this.ZoomLevel = Mathf.RoundToInt(val);
			this._centerZoom = false;
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x0028DFB4 File Offset: 0x0028C1B4
		private void Awake()
		{
			PoolManager.SetSrcObject("ViewPartWorldMapAreaEffectPrefabKey", this.areaEffectPrefab.gameObject);
			this.pauseToggle.onValueChanged.ResetListener(new Action<bool>(this.RefreshPauseToggle));
			this.locateTaiwuVillage.ResetListener(new Action(ViewPartWorldMap.LookAtTaiwuVillage));
			this._travelRouteHelper = new TravelRouteHelper<Vector3>(delegate(short index)
			{
				short fromId = this._mapModel.GetAreaIdByAreaTemplateId(this._travelRouteHelper.FromId);
				short toId = this._mapModel.GetAreaIdByAreaTemplateId(this._travelRouteHelper.ToId);
				AreaDisplayData[] areaDisplayData = this._areaDisplayData;
				bool? flag = (areaDisplayData != null) ? new bool?(areaDisplayData.CheckIndex((int)fromId)) : null;
				AreaDisplayData[] areaDisplayData2 = this._areaDisplayData;
				bool? flag2 = (areaDisplayData2 != null) ? new bool?(areaDisplayData2.CheckIndex((int)toId)) : null;
				int unlockFlag = ((flag ?? false) && flag2 != null && flag2.GetValueOrDefault() && this._areaDisplayData[(int)fromId].IsUnlocked && this._areaDisplayData[(int)toId].IsUnlocked) ? 1 : -1;
				float[] pos = MapArea.Instance[index].RoadPos;
				return new Vector3(pos[0], pos[1], (float)unlockFlag);
			}, delegate(float[] pos)
			{
				short fromId = this._mapModel.GetAreaIdByAreaTemplateId(this._travelRouteHelper.FromId);
				short toId = this._mapModel.GetAreaIdByAreaTemplateId(this._travelRouteHelper.ToId);
				float x = pos[0];
				float y = pos[1];
				AreaDisplayData[] areaDisplayData = this._areaDisplayData;
				bool? flag = (areaDisplayData != null) ? new bool?(areaDisplayData.CheckIndex((int)fromId)) : null;
				AreaDisplayData[] areaDisplayData2 = this._areaDisplayData;
				bool? flag2 = (areaDisplayData2 != null) ? new bool?(areaDisplayData2.CheckIndex((int)toId)) : null;
				return new Vector3(x, y, (float)(((flag ?? false) && flag2 != null && flag2.GetValueOrDefault() && this._areaDisplayData[(int)fromId].IsUnlocked && this._areaDisplayData[(int)toId].IsUnlocked) ? 1 : -1));
			});
			this.map.ExtraLocationDict[139] = this.chaishan.transform.localPosition;
			this.map.RegisterMouseHoverEvent(new Action<short>(this.OnMouseEnterAreaTemplateId), new Action<short>(this.OnMouseExitAreaTemplateId));
			this.map.PostRenderStart = delegate(AreaDisplayData[] _)
			{
				foreach (PositionFollower item in this.twelveFollower)
				{
					item.gameObject.SetActive(false);
				}
			};
			this.map.PostRender = delegate(Area area, AreaDisplayData data)
			{
				TooltipInvoker tip = area.Displayer;
				tip.Type = TipType.MapArea;
				TooltipInvoker tooltipInvoker = tip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("areaId", this._mapModel.GetAreaIdByAreaTemplateId(area.Config.TemplateId)).SetObject("displayData", data);
				tip.enabled = true;
				bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
				if (flag)
				{
					AreaEffect areaEffect = area.EffectRect.GetComponentInChildren<AreaEffect>(true);
					bool flag2 = areaEffect == null && data.AnyLoong;
					if (flag2)
					{
						areaEffect = PoolManager.GetObject<AreaEffect>("ViewPartWorldMapAreaEffectPrefabKey");
						RectTransform rectTrans = areaEffect.transform as RectTransform;
						rectTrans.SetParent(area.EffectRect, false);
						rectTrans.localPosition = Vector3.zero;
						rectTrans.anchoredPosition = Vector2.zero;
						rectTrans.pivot = new Vector2(0.5f, 0.5f);
						areaEffect.Refresh(data.LoongStatus);
					}
					else
					{
						bool flag3 = null != areaEffect && !data.AnyLoong;
						if (flag3)
						{
							areaEffect.transform.SetParent(null);
							PoolManager.Destroy("ViewPartWorldMapAreaEffectPrefabKey", areaEffect.gameObject);
						}
					}
				}
				area.SetTwelveImmortals(data.TwelveImmortal, this.twelveFollower);
			};
			this.map.PostRenderEnd = delegate(AreaDisplayData[] received)
			{
				bool flag = this.map.MapModel.AtChaishan || (SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(34) && this.map.MapModel.GetCurrentAreaTemplateId() == 79);
				if (flag)
				{
					AreaDisplayData data = received[140];
					data.IsBroken = false;
					bool flag2 = this.map.selector.CanAddToggle(this.chaishan.toggle);
					if (flag2)
					{
						this.map.selector.Add(this.chaishan.toggle);
					}
					this.chaishan.Set(data, true);
					this.chaishan.Set(this.chaishan.Config, false);
					(this.chaishan.map = this.map).PostRender(this.chaishan, data);
					this.chaishan.gameObject.SetActive(true);
				}
				else
				{
					this.chaishan.gameObject.SetActive(false);
				}
			};
			this.info.RegisterMouseHoverEvent(new Action<short>(this.OnMouseEnterAreaTemplateIdWithLock), new Action<short>(this.OnMouseExitAreaTemplateIdWithLock));
			this.map.OnSelectArea = (this.info.OnSelectArea = new Action<short>(this.OnClickArea));
			this.map.dragMove.AfterClampCallback = delegate()
			{
				bool flag = !this._startTravel;
				if (flag)
				{
					this.locateTaiwuVillage.UpdateTaiwuVillage(this.map.GetTransform(this.map.MapModel.Areas[(int)this.map.MapModel.GetTaiwuVillageAreaId()].GetTemplateId()));
				}
				else
				{
					this.locateTaiwuVillage.gameObject.SetActive(false);
				}
			};
			this.zoom.onValueChanged.ResetListener(new Action<float>(this.SetZoomLevel));
			GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
			this.cancelButton.onClick.ResetListener(delegate()
			{
				CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.Dialog, EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Type = 1,
					Yes = new Action(this.StopTravel),
					No = delegate()
					{
						this.EnableStateMask(true, true);
					},
					Title = LanguageKey.UI_Stop_Travel.Tr(),
					Content = LanguageKey.UI_Stop_Travel_Confirm.Tr()
				}));
			});
			this.search.onEndEdit.ResetListener(new Action<string>(this.<Awake>g__Search|100_7));
			this.search.onValueChanged.ResetListener(new Action<string>(this.<Awake>g__Search|100_7));
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x0028E1B7 File Offset: 0x0028C3B7
		private void OnDestroy()
		{
			GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
			PoolManager.RemoveData("ViewPartWorldMapAreaEffectPrefabKey");
		}

		// Token: 0x06005819 RID: 22553 RVA: 0x0028E1E0 File Offset: 0x0028C3E0
		private void OnEnable()
		{
			GEvent.Add(UiEvents.PartWorldMapDirectReturn, new GEvent.Callback(this.ConfirmDirectReturn));
			GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
			GEvent.Add(UiEvents.HidePartWorldMap, new GEvent.Callback(this.HidePartWorldMap));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyComplete));
			GEvent.Add(UiEvents.OnTravelPathUnlocked, new GEvent.Callback(this.OnTravelPathUnlocked));
			GEvent.Add(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
			GEvent.Add(UiEvents.PartWorldMapLookAt, new GEvent.Callback(this.MoveCameraToTaiwuVillage));
			GEvent.Add(UiEvents.PartWorldMapHide, new GEvent.Callback(this.QuickHideImpl));
			GEvent.Add(UiEvents.PartWorldMapDataChanged, new GEvent.Callback(this.RequestData));
			GEvent.Add(UiEvents.PartWorldMapHighlight, new GEvent.Callback(this.OnHighlightArea));
			this._isQuickHiding = false;
			ConchShipCursor.Instance.AddWheelProgress(-1f);
			ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OnMouseWheelProgressFull));
			this.moveTarget.gameObject.SetActive(false);
		}

		// Token: 0x0600581A RID: 22554 RVA: 0x0028E36C File Offset: 0x0028C56C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.PartWorldMapDirectReturn, new GEvent.Callback(this.ConfirmDirectReturn));
			GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
			GEvent.Remove(UiEvents.HidePartWorldMap, new GEvent.Callback(this.HidePartWorldMap));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyComplete));
			GEvent.Remove(UiEvents.OnTravelPathUnlocked, new GEvent.Callback(this.OnTravelPathUnlocked));
			GEvent.Remove(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
			GEvent.Remove(UiEvents.PartWorldMapLookAt, new GEvent.Callback(this.MoveCameraToTaiwuVillage));
			GEvent.Remove(UiEvents.PartWorldMapHide, new GEvent.Callback(this.QuickHideImpl));
			GEvent.Remove(UiEvents.PartWorldMapDataChanged, new GEvent.Callback(this.RequestData));
			GEvent.Remove(UiEvents.PartWorldMapHighlight, new GEvent.Callback(this.OnHighlightArea));
			ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OnMouseWheelProgressFull));
			bool flag = !this._isQuickHiding;
			if (flag)
			{
				AudioManager.Instance.PlaySound(this.seEnter, false, 100);
			}
			this.ProcessCachedGEvents();
			this._currSelectedStateId = -1;
			this.ClearGainsGauge();
			this.ClearTravelPath();
			this.map.ClearHighlight();
			this.anim.transform.DOKill(false);
			this.anim.gameObject.SetActive(false);
		}

		// Token: 0x0600581B RID: 22555 RVA: 0x0028E52F File Offset: 0x0028C72F
		private void QuickHideImpl(ArgumentBox argBox)
		{
			this.QuickHide();
		}

		// Token: 0x0600581C RID: 22556 RVA: 0x0028E538 File Offset: 0x0028C738
		private void MoveCameraToTaiwuVillage(ArgumentBox argBox)
		{
			bool startTravel = this._startTravel;
			if (!startTravel)
			{
				this.ZoomLevel = 1;
				short areaId;
				bool flag = argBox == null || !argBox.Get("AreaId", out areaId);
				if (flag)
				{
					this.map.LookAtTaiwuVillage();
				}
				else
				{
					this.map.LookAt(areaId, 0f, default(Vector2));
				}
			}
		}

		// Token: 0x0600581D RID: 22557 RVA: 0x0028E5A0 File Offset: 0x0028C7A0
		private void OnHighlightArea(ArgumentBox argBox)
		{
			short areaId;
			bool flag = argBox == null || !argBox.Get("AreaId", out areaId) || areaId < 0;
			if (flag)
			{
				this.map.ClearHighlight();
			}
			else
			{
				bool flag2 = !this._mapModel.Areas.CheckIndex((int)areaId);
				if (!flag2)
				{
					this.map.HighlightedAreaTemplateId = this._mapModel.Areas[(int)areaId].GetTemplateId();
					this.ZoomLevel = 1;
					this.map.LookAt(areaId, 0f, default(Vector2));
				}
			}
		}

		// Token: 0x0600581E RID: 22558 RVA: 0x0028E635 File Offset: 0x0028C835
		private void OnMouseWheelProgressFull()
		{
			ConchShipCursor.Instance.AddWheelProgress(-1f);
			this.QuickHide();
		}

		// Token: 0x0600581F RID: 22559 RVA: 0x0028E64F File Offset: 0x0028C84F
		private void OnTopUiChanged(ArgumentBox argumentBox)
		{
			this._disableCheckClose = true;
		}

		// Token: 0x06005820 RID: 22560 RVA: 0x0028E65C File Offset: 0x0028C85C
		private void OnGameStateChange(ArgumentBox argBox)
		{
			Enum state;
			argBox.Get("newState", out state);
			bool flag = (EGameState)state != EGameState.Loading;
			if (!flag)
			{
				this.Element.OnHide = null;
				GEvent.ClearEvent(UiEvents.OnTravelCheckPointProcessed);
			}
		}

		// Token: 0x06005821 RID: 22561 RVA: 0x0028E6A4 File Offset: 0x0028C8A4
		private void OnConfirmQuitGameState(ArgumentBox argBox)
		{
			bool traveling = WorldMapModel.Traveling;
			if (traveling)
			{
				bool show;
				argBox.Get("ShowState", out show);
				Time.timeScale = (float)(show ? 0 : 1);
				CommandKitBase.SetDisable(!show);
			}
		}

		// Token: 0x06005822 RID: 22562 RVA: 0x0028E6E4 File Offset: 0x0028C8E4
		private void OnWorldMapInit(ArgumentBox argbox)
		{
			bool travelIsFinish = this._travelIsFinish;
			if (travelIsFinish)
			{
				this.FinishAndHideSelf();
			}
		}

		// Token: 0x06005823 RID: 22563 RVA: 0x0028E704 File Offset: 0x0028C904
		private void OnTaiwuCharIdChange(ArgumentBox argBox)
		{
			base.CGet<CImage>("Arrow").SetSprite(CommonUtils.GetTaiwuSpriteName(), true, null);
			this.ProcessCachedGEvents();
			this.FinishAndHideSelf();
			GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", false));
		}

		// Token: 0x06005824 RID: 22564 RVA: 0x0028E755 File Offset: 0x0028C955
		private void HidePartWorldMap(ArgumentBox argbox)
		{
			this.HideSelf();
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x0028E75F File Offset: 0x0028C95F
		private void OnMonthNotifyComplete(ArgumentBox argBox)
		{
			MapDomainMethod.Call.GetAllAreaDisplayData(this.Element.GameDataListenerId);
		}

		// Token: 0x06005826 RID: 22566 RVA: 0x0028E773 File Offset: 0x0028C973
		private void OnTravelPathUnlocked(ArgumentBox argbox)
		{
			CommandManager.AddCommandMethodCall(EPriority.CallGetAreaDisplayData, 2, 35, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._areaDisplayData);
				this.HandlerMethodGetAllAreaDisplayData();
			}, null);
		}

		// Token: 0x06005827 RID: 22567 RVA: 0x0028E78D File Offset: 0x0028C98D
		private void OnCharacterTaiwuCarrierChanged(ArgumentBox _)
		{
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, this._taiwuDisplayData.CharacterId, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
				Serializer.Deserialize(pool, offset, ref equipments);
				this.anim.gameObject.SetActive(true);
				this.anim.Set(this._taiwuDisplayData, equipments, this._kidnappedTravelData.Valid ? short.MaxValue : this._mapModel.TaiwuCarrier, true);
				this.anim.SetScale(this.anim.TravelSkeleton.Size, this.anim.TravelSkeleton.DeltaX, this.anim.TravelSkeleton.DeltaY);
				this.anim.PlayCarrierAnim(this._startTravel && !this._isTravelAutoPaused && !this.pauseToggle.isOn);
				EasyPool.Free<List<ItemDisplayData>>(equipments);
			});
		}

		// Token: 0x06005828 RID: 22568 RVA: 0x0028E7B0 File Offset: 0x0028C9B0
		private void Update()
		{
			this.UpdateButtons();
			bool flag = this.UpdateCheckPause();
			if (!flag)
			{
				this.UpdateTravelAnimIdleState();
				this.UpdateCheckContinue();
				bool traveling = this._moveInfo.Traveling;
				if (traveling)
				{
					bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
					if (flag2)
					{
						UIManager.Instance.MaskUI(UIElement.SystemOption);
					}
				}
				else
				{
					bool flag3 = CommandKitBase.GetDisable() || !UIManager.Instance.IsFocusElement(this.Element);
					if (!flag3)
					{
						float scrollValue = this._scrollLock ? 0f : Input.GetAxis("Mouse ScrollWheel");
						float num = scrollValue;
						float num2 = num;
						if (num2 <= 0f)
						{
							if (num2 < 0f)
							{
								int num3 = this.zoomLevel;
								if (num3 > 0 && num3 < 10)
								{
									num3 = this.ZoomLevel;
									this.ZoomLevel = num3 + 1;
								}
							}
						}
						else if (this.zoomLevel > 1)
						{
							int num3 = this.ZoomLevel;
							this.ZoomLevel = num3 - 1;
						}
						bool flag4 = this.ZoomLevel == 1 && scrollValue > 0f;
						if (flag4)
						{
							ConchShipCursor.Instance.AddWheelProgress(scrollValue * 2f);
						}
						else
						{
							bool flag5 = scrollValue < 0f;
							if (flag5)
							{
								ConchShipCursor.Instance.AddWheelProgress(-1f);
							}
						}
						bool disableCheckClose = this._disableCheckClose;
						if (disableCheckClose)
						{
							this._disableCheckClose = false;
						}
						else
						{
							this.UpdateCheckHide();
						}
					}
				}
			}
		}

		// Token: 0x06005829 RID: 22569 RVA: 0x0028E94C File Offset: 0x0028CB4C
		private void UpdateButtons()
		{
			this.cancelImage.sprite = ((this.cancelButton.interactable = (SingletonObject.getInstance<BasicGameData>().AdvancingMonthState == 0 && !this._isDirectTraveling)) ? this.cancelSprite : this.disableCancelSprite);
		}

		// Token: 0x0600582A RID: 22570 RVA: 0x0028E9A0 File Offset: 0x0028CBA0
		private bool UpdateCheckPause()
		{
			bool interactable = this.pauseToggle.interactable;
			if (interactable)
			{
				bool flag = MainInterfaceFunctionCommandKit.PartWorldMapPause.Check(this.Element, false, false, false, true, false) && WorldMapModel.Traveling;
				if (flag)
				{
					CToggle ctoggle = this.pauseToggle;
					ctoggle.isOn = !ctoggle.isOn;
					return true;
				}
				bool isFocus = UIManager.Instance.IsFocusElement(this.Element);
				bool isPause = this.pauseToggle.isOn;
				bool flag2 = !isFocus && !isPause;
				if (flag2)
				{
					this.pauseToggle.isOn = true;
					this._isTravelAutoPaused = true;
					return true;
				}
				bool flag3 = isFocus && isPause && this._isTravelAutoPaused;
				if (flag3)
				{
					this.pauseToggle.isOn = false;
					this._isTravelAutoPaused = false;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600582B RID: 22571 RVA: 0x0028EA7C File Offset: 0x0028CC7C
		private void UpdateTravelAnimIdleState()
		{
			CrossAreaMoveInfo moveInfo = this._moveInfo;
			bool flag = moveInfo == null || !moveInfo.Traveling || !this.anim.gameObject.activeSelf;
			if (!flag)
			{
				bool paused = this._isTravelAutoPaused || this.pauseToggle.isOn;
				bool flag2 = this._travelAnimLastIdleState == null || this._travelAnimLastIdleState.Value != paused;
				if (flag2)
				{
					this.anim.PlayCarrierAnim(true);
				}
				this._travelAnimLastIdleState = new bool?(this._isTravelAutoPaused || this.pauseToggle.isOn);
			}
		}

		// Token: 0x0600582C RID: 22572 RVA: 0x0028EB24 File Offset: 0x0028CD24
		private void UpdateCheckContinue()
		{
			bool flag = this._continueWaitingSeconds < 0f;
			if (!flag)
			{
				bool flag2 = !this._moveInfo.Traveling;
				if (flag2)
				{
					this.ClearWait();
				}
				else
				{
					this._continueWaitingSeconds -= Time.deltaTime;
					bool flag3 = this._continueWaitingSeconds < 0f && !this._waitForAnimation;
					if (flag3)
					{
						this.ContinueTravelWithCheck();
					}
					else
					{
						bool flag4 = this._continueWaitingSeconds < 0f && this._waitForAnimation;
						if (flag4)
						{
							this._continueWaitingSeconds = 0f;
						}
					}
				}
			}
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x0028EBC0 File Offset: 0x0028CDC0
		private void ContinueTravelWithCheck()
		{
			UIElement conflictElement = null;
			bool exist = UIElement.Loading.Exist;
			if (exist)
			{
				conflictElement = UIElement.Loading;
			}
			bool exist2 = UIElement.MonthNotify.Exist;
			if (exist2)
			{
				conflictElement = UIElement.MonthNotify;
			}
			bool exist3 = UIElement.CombatBegin.Exist;
			if (exist3)
			{
				conflictElement = UIElement.CombatBegin;
			}
			bool exist4 = UIElement.Combat.Exist;
			if (exist4)
			{
				conflictElement = UIElement.Combat;
			}
			bool exist5 = UIElement.EventWindow.Exist;
			if (exist5)
			{
				conflictElement = UIElement.EventWindow;
			}
			bool inConflict = conflictElement != null || SingletonObject.getInstance<EventModel>().HasListeningEvent;
			BasicGameData basic = SingletonObject.getInstance<BasicGameData>();
			bool inAdvance = basic.AdvancingMonthState != 0 || basic.SavingWorld;
			bool flag = inConflict || inAdvance || this.pauseToggle.isOn || CommandManager.IsRunning;
			if (flag)
			{
				this.ResetWait();
			}
			else
			{
				this.ContinueTravel();
			}
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x0028EC97 File Offset: 0x0028CE97
		private void ResetWait()
		{
			this.ResetWait(1f);
		}

		// Token: 0x0600582F RID: 22575 RVA: 0x0028ECA5 File Offset: 0x0028CEA5
		private void ResetWait(float ratio)
		{
			this._continueWaitingSeconds = Math.Max(this.StepWaitDuration * ratio, this._lastTravelAnimDuration);
		}

		// Token: 0x06005830 RID: 22576 RVA: 0x0028ECC4 File Offset: 0x0028CEC4
		private void ContinueTravel()
		{
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
			WorldMapModel.MapBlockLoadFinish = false;
			WorldMapModel.MapBlockUiLoadFinish = false;
			bool flag = GMFunc.AvoidTravelEvent || this._kidnappedTravelData.Valid;
			if (flag)
			{
				CommandManager.AddCommandMethodCall(EPriority.CallContinueTravel, 2, 13, new CallMethodRespHandler(this.HandlerMethodContinueTravel), new CallMethodSkipHandler(this.ResetWait));
			}
			else
			{
				CommandManager.AddCommandMethodCall(EPriority.CallContinueTravel, 2, 31, new CallMethodRespHandler(this.HandlerMethodContinueTravelWithDetectTravelingEvent), new CallMethodSkipHandler(this.ResetWait));
			}
		}

		// Token: 0x06005831 RID: 22577 RVA: 0x0028ED54 File Offset: 0x0028CF54
		private void HandlerMethodContinueTravelWithDetectTravelingEvent(int offset, RawDataPool pool)
		{
			short travelingEventId = -1;
			Serializer.Deserialize(pool, offset, ref travelingEventId);
			this.HandlerMethodContinueTravelWithDetectTravelingEvent(travelingEventId);
		}

		// Token: 0x06005832 RID: 22578 RVA: 0x0028ED76 File Offset: 0x0028CF76
		private void HandlerMethodContinueTravel(int offset, RawDataPool pool)
		{
			this.HandlerMethodContinueTravelWithDetectTravelingEvent(-1);
		}

		// Token: 0x06005833 RID: 22579 RVA: 0x0028ED84 File Offset: 0x0028CF84
		private void HandlerMethodContinueTravelWithDetectTravelingEvent(short travelingEventId)
		{
			this.ChangeTravelInteractable();
			bool flag = travelingEventId < 0;
			if (!flag)
			{
				TravelingEventItem config = TravelingEvent.Instance[travelingEventId];
				bool flag2 = string.IsNullOrEmpty(config.Event);
				if (!flag2)
				{
					AdaptableLog.Info(string.Format("HandlerMethodContinueTravelWithDetectTravelingEvent wait for {0}", travelingEventId));
					this.ChangeTravelInteractable(false, true);
					short destAreaId = this._travelDestAreaId;
					GEvent.AddOneShot(UiEvents.OnTravelCheckPointProcessed, delegate(ArgumentBox _)
					{
						AdaptableLog.Info(string.Format("HandlerMethodContinueTravelWithDetectTravelingEvent waited {0}", travelingEventId));
						this._travelDestAreaId = destAreaId;
						this.ResetWait(0.33f);
						this.ChangeTravelInteractable(this.CanTravelInteractable, false);
					});
				}
			}
		}

		// Token: 0x06005834 RID: 22580 RVA: 0x0028EE28 File Offset: 0x0028D028
		private void UpdateCheckHide()
		{
			bool flag = this._travelDestAreaId < 0 && this.Element.Ready;
			if (flag)
			{
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					this.QuickHide();
				}
			}
			bool flag3 = this._travelIsFinish && UIManager.Instance.IsFocusElement(this.Element) && WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockUiLoadFinish;
			if (flag3)
			{
				this.FinishAndHideSelf();
			}
		}

		// Token: 0x06005835 RID: 22581 RVA: 0x0028EEC0 File Offset: 0x0028D0C0
		private void HandlerDataModification(Notification notification, NotificationWrapper wrapper)
		{
			DataUid uid = notification.Uid;
			ushort domainId = uid.DomainId;
			ushort num = domainId;
			if (num != 2)
			{
				if (num == 19)
				{
					if (uid.DataId != 15)
					{
						if (uid.DataId != 163)
						{
							if (uid.DataId != 164)
							{
								if (uid.DataId == 204)
								{
									Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._taiwuVisitedAreas);
									base.RemoveMonitorFieldId(19, 204);
								}
							}
							else
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._kidnappedTravelData);
								this.OnCharacterTaiwuCarrierChanged(null);
							}
						}
						else
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._isDirectTraveling);
							this.ChangeTravelInteractable();
						}
					}
					else
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._gainsInTravel);
						this.HandlerDataGainsInTravel();
					}
				}
			}
			else if (uid.DataId == 56)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._newMoveInfo);
				this.HandlerDataTravelInfo();
			}
		}

		// Token: 0x06005836 RID: 22582 RVA: 0x0028EFEC File Offset: 0x0028D1EC
		private void HandlerMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort num = domainId;
			if (num != 2)
			{
				if (num == 4)
				{
					if (notification.MethodId == 48)
					{
						List<CharacterDisplayData> displayDataList = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
						this.HandlerMethodGetCharacterDisplayDataList(displayDataList);
						this.OnCharacterTaiwuCarrierChanged(null);
					}
				}
			}
			else if (notification.MethodId != 11)
			{
				if (notification.MethodId == 35)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._areaDisplayData);
					this.HandlerMethodGetAllAreaDisplayData();
				}
			}
			else
			{
				CrossAreaMoveInfo travelInfo = new CrossAreaMoveInfo();
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref travelInfo);
				this.HandlerMethodGetTravelCost(travelInfo);
			}
		}

		// Token: 0x06005837 RID: 22583 RVA: 0x0028F0A8 File Offset: 0x0028D2A8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
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
						this.HandlerMethodReturn(notification, wrapper);
					}
				}
				else
				{
					this.HandlerDataModification(notification, wrapper);
				}
			}
		}

		// Token: 0x06005838 RID: 22584 RVA: 0x0028F128 File Offset: 0x0028D328
		private void UpdateTravelInfo(CrossAreaMoveInfo moveInfo)
		{
			this.SetTravelButtonsVisibility(true);
			this.ChangeTravelInteractable(true);
			this.OnCharacterTaiwuCarrierChanged(null);
			this.moveTarget.gameObject.SetActive(true);
			this.moveTarget.Target = this.map.GetTransform(this._mapModel.Areas[(int)moveInfo.ToAreaId].GetTemplateId());
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x0028F190 File Offset: 0x0028D390
		private void HandlerDataTravelInfo()
		{
			AdaptableLog.Info(string.Format("HandlerDataTravelInfo {0} {1} {2}", this._newMoveInfo.Traveling, this._newMoveInfo.FromAreaId, this._newMoveInfo.ToAreaId));
			bool flag = !this._moveInfo.Traveling && this._newMoveInfo.Traveling;
			if (flag)
			{
				this.StartTravel(this._newMoveInfo);
			}
			else
			{
				bool traveling = this._moveInfo.Traveling;
				if (traveling)
				{
					bool flag2 = !this._newMoveInfo.Traveling;
					if (flag2)
					{
						this._startTravel = false;
						this._travelIsFinish = true;
						this.moveTarget.gameObject.SetActive(false);
						this.moveTarget.Target = null;
						bool flag3 = !this._guidingSectExam && this._areaDisplayData.CheckIndex((int)this._moveInfo.CurrentAreaId) && this._areaDisplayData[(int)this._moveInfo.CurrentAreaId].HasSectExam;
						if (flag3)
						{
							this._guidingSectExam = true;
							GlobalDomainMethod.Call.InvokeGuidingTrigger(61);
						}
					}
					else
					{
						bool flag4 = !this._startTravel;
						if (flag4)
						{
							this._startTravel = true;
							this.SetTravelFacing(this._newMoveInfo.FromAreaId, this._newMoveInfo.ToAreaId);
							this.UpdateTravelInfo(this._newMoveInfo);
						}
					}
				}
			}
			this.SyncRouteCostDays();
			this._nextAreaCostDays = this._moveInfo.NextCostDays;
			short srcArea = this._moveInfo.CurrentAreaId;
			short dstArea = this._newMoveInfo.CurrentAreaId;
			bool flag5 = srcArea == -1 && this._newMoveInfo.CurrentAreaId != -1;
			if (flag5)
			{
				this.UpdatePlayerPos(srcArea = this._newMoveInfo.CurrentAreaId, false);
			}
			bool flag6 = srcArea != dstArea && this._moveInfo.Traveling && this._newMoveInfo.Traveling;
			if (flag6)
			{
				this.PlayTravelAnim(srcArea, dstArea);
			}
			this.ResetWait();
			CrossAreaMoveInfo newMoveInfo = this._newMoveInfo;
			CrossAreaMoveInfo moveInfo = this._moveInfo;
			this._moveInfo = newMoveInfo;
			this._newMoveInfo = moveInfo;
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x0028F3BC File Offset: 0x0028D5BC
		private void PlayTravelAnim(short srcArea, short dstArea)
		{
			bool flag = srcArea == 140 || dstArea == 140;
			if (flag)
			{
				this._waitForAnimation = true;
				CrossAreaMoveInfo newInfo = this._newMoveInfo;
				Transform fromTrans = (srcArea == 140) ? this.chaishan.transform : this.map.GetTransform(this.map.MapModel.Areas[(int)srcArea].GetTemplateId());
				Transform toTrans = (dstArea == 140) ? this.chaishan.transform : this.map.GetTransform(this.map.MapModel.Areas[(int)dstArea].GetTemplateId());
				DOTween.Sequence().AppendCallback(delegate
				{
					this.hidePosition.Target = fromTrans;
					this.hideEffect.gameObject.SetActive(true);
					this.hideEffect.Play();
				}).AppendInterval(1f).Append(this.anim.transform.DOScale(0f, 0.5f)).AppendInterval(0.5f).AppendCallback(delegate
				{
					this.hideEffect.gameObject.SetActive(false);
					this.map.LookAt(this.anim.transform.localPosition = this.map.GetLocation(this.map.MapModel.Areas[(int)dstArea].GetTemplateId()), 0.2f, -1f, default(Vector2));
				}).AppendInterval(0.2f).AppendCallback(delegate
				{
					this.UpdateTravelPath(newInfo, 1);
					this.showPosition.Target = toTrans;
					this.showEffect.gameObject.SetActive(true);
					this.showEffect.Play();
				}).AppendInterval(1f).Append(this.anim.transform.DOScale(0.1f, 0.5f)).AppendInterval(0.5f).AppendCallback(delegate
				{
					this.showEffect.gameObject.SetActive(false);
					this._waitForAnimation = false;
				}).Play<Sequence>();
			}
			else
			{
				this.map.LookAt(dstArea, 0.3f, default(Vector2));
				this.UpdateTravelPath(this._newMoveInfo, 1);
			}
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x0028F5A8 File Offset: 0x0028D7A8
		private void UpdateTravelPath(CrossAreaMoveInfo travelInfo, int delta = 0)
		{
			int x = travelInfo.RouteIndex + delta;
			bool flag = x == 0;
			if (flag)
			{
				this.RenderRoute(travelInfo.Route.AreaList.Zip(travelInfo.Route.AreaList.Prepend(this._mapModel.CurrentAreaId), (short s, short s1) => new ValueTuple<short, short>(this._mapModel.Areas[(int)s].GetTemplateId(), this._mapModel.Areas[(int)s1].GetTemplateId())));
			}
			else
			{
				this.RenderRoute(travelInfo.Route.AreaList.Skip(x).Zip(travelInfo.Route.AreaList.Skip(x - 1), (short s, short s1) => new ValueTuple<short, short>(this._mapModel.Areas[(int)s].GetTemplateId(), this._mapModel.Areas[(int)s1].GetTemplateId())));
			}
			bool traveling = travelInfo.Traveling;
			if (traveling)
			{
				this.UpdatePlayerPos((travelInfo.RouteIndex == 0) ? this._mapModel.CurrentAreaId : travelInfo.Route.AreaList[travelInfo.RouteIndex - 1], travelInfo.Route.AreaList[travelInfo.RouteIndex]);
				AudioManager.Instance.PlaySound(this.seCursor.name, false, false);
			}
			sbyte currStateId = this._mapModel.GetStateId(this._mapModel.CurrentAreaId);
			string bgmName = (currStateId >= 0 && this._mapModel.CurrentAreaId != this._mapModel.GetTaiwuVillageBlock().AreaId) ? MapState.Instance[(int)(currStateId + 1)].Bgm : "main_fushixun";
			MusicPlayerModel musicPlayerModel = SingletonObject.getInstance<MusicPlayerModel>();
			bool flag2 = AudioManager.Instance.GetPlayingMusic() != bgmName && musicPlayerModel.IsEnabled && !musicPlayerModel.IsPlaying;
			if (flag2)
			{
				AudioManager.Instance.PlayMusic(bgmName, 3f, 100, null);
			}
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x0028F750 File Offset: 0x0028D950
		private void HandlerDataGainsInTravel()
		{
		}

		// Token: 0x0600583D RID: 22589 RVA: 0x0028F754 File Offset: 0x0028D954
		private void HandlerMethodGetTravelCost(CrossAreaMoveInfo travelInfo)
		{
			bool flag = travelInfo.ToAreaId != this._currSelectedAreaId;
			if (!flag)
			{
				this._currSelectedAreaMoveInfo = travelInfo;
				this.UpdateTravelPath(travelInfo, 0);
			}
		}

		// Token: 0x0600583E RID: 22590 RVA: 0x0028F78C File Offset: 0x0028D98C
		private void HandlerMethodGetAllAreaDisplayData()
		{
			this.map.Refresh(this._areaDisplayData);
			this.info.Refresh(this._areaDisplayData);
			foreach (State state in this.info.states)
			{
				state.Sort();
			}
			this.postStationCount.text = LanguageKey.UI_MiniMap_PartWorld_PostStationCount.TrFormat(this._areaDisplayData.Count((AreaDisplayData x) => x.IsUnlocked), 135).ColorReplace();
			this.RefreshZoom();
		}

		// Token: 0x0600583F RID: 22591 RVA: 0x0028F840 File Offset: 0x0028DA40
		private void HandlerMethodGetCharacterDisplayDataList(List<CharacterDisplayData> displayDataList)
		{
			this._taiwuDisplayData = displayDataList[0];
			bool traveling = this._moveInfo.Traveling;
			if (traveling)
			{
				this.anim.PlayCarrierAnim(!this._isTravelAutoPaused && !this.pauseToggle.isOn);
			}
		}

		// Token: 0x06005840 RID: 22592 RVA: 0x0028F890 File Offset: 0x0028DA90
		private void SyncRouteCostDays()
		{
			bool flag = this._moveInfo.Route.CostList.Count != this._newMoveInfo.Route.CostList.Count;
			if (!flag)
			{
				this._moveInfo.Route.CostList.Clear();
				this._moveInfo.Route.CostList.AddRange(this._newMoveInfo.Route.CostList);
			}
		}

		// Token: 0x06005841 RID: 22593 RVA: 0x0028F90F File Offset: 0x0028DB0F
		private void RefreshZoom()
		{
			this.ZoomLevel = this.zoomLevel;
		}

		// Token: 0x06005842 RID: 22594 RVA: 0x0028F920 File Offset: 0x0028DB20
		private void StartTravel(CrossAreaMoveInfo moveInfo)
		{
			this._startTravel = true;
			this.zoom.interactable = (this.zoomBtn.interactable = false);
			this.RefreshZoom();
			short curTemplateId = this._mapModel.Areas[(int)moveInfo.CurrentAreaId].GetTemplateId();
			this.RenderRoute(moveInfo.Route.AreaList.Zip(moveInfo.Route.AreaList.Prepend(this._mapModel.CurrentAreaId), (short s, short s1) => new ValueTuple<short, short>(this._mapModel.Areas[(int)s].GetTemplateId(), this._mapModel.Areas[(int)s1].GetTemplateId())).SkipWhile((ValueTuple<short, short> x) => x.Item2 != curTemplateId));
			this.InvokeFirstEvent();
			short area = moveInfo.CurrentAreaId;
			this.map.LookAt(area, 0f, default(Vector2));
			this.SetTravelFacing(moveInfo.FromAreaId, moveInfo.ToAreaId);
			this.UpdateTravelInfo(moveInfo);
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x0028FA18 File Offset: 0x0028DC18
		private void SetTravelFacing(short fromAreaId, short toAreaId)
		{
			bool flag = !this._mapModel.Areas.CheckIndex((int)fromAreaId) || !this._mapModel.Areas.CheckIndex((int)toAreaId);
			if (!flag)
			{
				Vector2 fromLoc = this.map.GetLocation(this._mapModel.Areas[(int)fromAreaId].GetTemplateId());
				Vector2 destLoc = this.map.GetLocation(this._mapModel.Areas[(int)toAreaId].GetTemplateId());
				this._towards = (float)((destLoc.x < fromLoc.x) ? -1 : 1);
				this.ApplyFacingScale();
			}
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x0028FAB2 File Offset: 0x0028DCB2
		private void ApplyFacingScale()
		{
			this.anim.SetDirection(this._towards > 0f);
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x0028FAD0 File Offset: 0x0028DCD0
		private void UpdatePlayerPos(short areaId, bool isTween = false)
		{
			bool flag = !this._mapModel.Areas.CheckIndex((int)areaId);
			if (!flag)
			{
				Vector2 loc = (areaId == 140) ? this.chaishan.transform.localPosition : this.map.GetLocation(this._mapModel.Areas[(int)areaId].GetTemplateId());
				bool flag2 = !this._moveInfo.Traveling && isTween;
				if (flag2)
				{
					this._towards = (float)((loc.x < this.anim.transform.localPosition.x) ? -1 : 1);
				}
				this.ApplyFacingScale();
				if (isTween)
				{
					this.anim.transform.DOKill(false);
					this._lastTravelAnimDuration = Vector2.Distance(this.anim.transform.localPosition, loc) / 1000f;
					this.anim.transform.DOLocalMove(loc, this._lastTravelAnimDuration, false).SetEase(Ease.Linear).SetTarget(this.anim.transform);
				}
				else
				{
					this.anim.transform.localPosition = loc;
				}
			}
		}

		// Token: 0x06005846 RID: 22598 RVA: 0x0028FC08 File Offset: 0x0028DE08
		private void UpdatePlayerPos(short fromAreaId, short toAreaId)
		{
			bool flag = !this._mapModel.Areas.CheckIndex((int)fromAreaId) || !this._mapModel.Areas.CheckIndex((int)toAreaId);
			if (flag)
			{
				this.UpdatePlayerPos(toAreaId, true);
			}
			else
			{
				short fromTemplateId = this._mapModel.Areas[(int)fromAreaId].GetTemplateId();
				short toTemplateId = this._mapModel.Areas[(int)toAreaId].GetTemplateId();
				this.ApplyFacingScale();
				Vector3[] routePoints = this._travelRouteHelper.GetRoute(fromTemplateId, toTemplateId).ToArray<Vector3>();
				bool flag2 = routePoints.Length < 2;
				if (flag2)
				{
					this.UpdatePlayerPos(toAreaId, true);
				}
				else
				{
					this._sc = new SmoothCurve(routePoints);
					this.anim.transform.DOKill(false);
					float totalLength = this._sc.TotalLength;
					bool flag3 = totalLength <= 0f;
					if (flag3)
					{
						Transform transform = this.anim.transform;
						Vector3[] array = routePoints;
						transform.localPosition = array[array.Length - 1];
					}
					else
					{
						this._lastTravelAnimDuration = totalLength / 1000f;
						DOVirtual.Float(0f, totalLength, this._lastTravelAnimDuration, delegate(float distance)
						{
							this.anim.transform.localPosition = this._sc.GetPoint(distance);
						}).SetEase(Ease.Linear).SetTarget(this.anim.transform);
					}
				}
			}
		}

		// Token: 0x06005847 RID: 22599 RVA: 0x0028FD4B File Offset: 0x0028DF4B
		private void InvokeFirstEvent()
		{
			GEvent.OnEvent(UiEvents.OnTravelStart, null);
			GEvent.AddOneShot(UiEvents.WorldMapInited, new GEvent.Callback(this.OnWorldMapInit));
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x0028FD75 File Offset: 0x0028DF75
		public void ClearGainsGauge()
		{
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x0028FD78 File Offset: 0x0028DF78
		public override void OnInit(ArgumentBox argsBox)
		{
			this.search.text = "";
			this.zoom.interactable = (this.zoomBtn.interactable = (this.NeedDataListenerId = true));
			this._waitForAnimation = (this._travelIsFinish = (this._startTravel = (this.pauseToggle.isOn = false)));
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._moveInfo = new CrossAreaMoveInfo();
			this._newMoveInfo = new CrossAreaMoveInfo();
			this._travelDestAreaId = -1;
			this._travelAnimLastIdleState = null;
			this.anim.gameObject.SetActive(false);
			this.canvasGroup.alpha = 0f;
			this.areaStateItemController.OnInit();
			this.locateTaiwuVillage.gameObject.SetActive(true);
			this.map.Init(true, this, false);
			this.info.Init(true, this, false);
			State[] states = this.map.states;
			for (int i = 0; i < states.Length; i++)
			{
				State state = states[i];
				bool flag = state.stateMask;
				if (flag)
				{
					state.stateMask.button.onClick.ResetListener(delegate()
					{
						this.OnClickState((short)state.Config.TemplateId);
					});
				}
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			this.mapLegendBtn.ClearAndAddListener(new Action(this.OnClickMapLegendButton));
		}

		// Token: 0x0600584A RID: 22602 RVA: 0x0028FF2C File Offset: 0x0028E12C
		private void SetTravelButtonsVisibility(bool value)
		{
			this.pauseToggle.interactable = value;
			this.pauseToggle.gameObject.SetActive(value);
			this.cancelButton.gameObject.SetActive(value);
			if (value)
			{
				this.pauseToggle.isOn = false;
				this.RefreshPauseToggle(this.pauseToggle.isOn);
			}
		}

		// Token: 0x0600584B RID: 22603 RVA: 0x0028FF91 File Offset: 0x0028E191
		private void RequestData(ArgumentBox _)
		{
			AreaMap.RequestData(this, delegate(AreaDisplayData[] data)
			{
				this._areaDisplayData = data;
				this.HandlerMethodGetAllAreaDisplayData();
			});
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x0028FFA8 File Offset: 0x0028E1A8
		private void RequestData()
		{
			this.SetTravelButtonsVisibility(false);
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
			{
				this.TaiwuCharId
			});
			MapDomainMethod.Call.GetAllAreaDisplayData(this.Element.GameDataListenerId);
			this.map.LookAt(this._mapModel.CurrentAreaId, 0f, default(Vector2));
			this.info.LookAt(this._mapModel.CurrentAreaId, 0f, default(Vector2));
			AreaMap.RequestData(this, delegate(AreaDisplayData[] data)
			{
				this._areaDisplayData = data;
				this.HandlerMethodGetAllAreaDisplayData();
				this.ZoomLevel = 1;
				bool flag2 = !this._startTravel;
				if (flag2)
				{
					this.UpdatePlayerPos(this._mapModel.CurrentAreaId, false);
				}
				this.map.LookAt(this._mapModel.CurrentAreaId, 0f, default(Vector2));
				this.info.LookAt(this._mapModel.CurrentAreaId, 0f, default(Vector2));
				this.Element.ShowAfterRefresh();
				this.canvasGroup.alpha = 1f;
			});
			AudioManager.Instance.PlaySound(this.seEnter, false, 100);
			bool flag = !this._guidingEnter;
			if (flag)
			{
				this._guidingEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(66);
			}
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x00290081 File Offset: 0x0028E281
		public void SetCanScroll(bool canScroll)
		{
			this._scrollLock = canScroll;
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x0029008A File Offset: 0x0028E28A
		private void OnMouseEnterAreaTemplateIdWithLock(short areaTemplateId)
		{
			this.OnMouseEnterAreaTemplateId(areaTemplateId);
		}

		// Token: 0x0600584F RID: 22607 RVA: 0x00290095 File Offset: 0x0028E295
		private void OnMouseExitAreaTemplateIdWithLock(short areaTemplateId)
		{
			this.OnMouseExitAreaTemplateId(areaTemplateId);
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x002900A0 File Offset: 0x0028E2A0
		private void OnMouseEnterAreaTemplateId(short areaTemplateId)
		{
			ViewPartWorldMap.<>c__DisplayClass166_0 CS$<>8__locals1 = new ViewPartWorldMap.<>c__DisplayClass166_0();
			CS$<>8__locals1.<>4__this = this;
			bool traveling = this._moveInfo.Traveling;
			if (!traveling)
			{
				short areaId = this._mapModel.GetAreaIdByAreaTemplateId(areaTemplateId);
				this._currSelectedAreaId = areaId;
				ViewPartWorldMap.<>c__DisplayClass166_0 CS$<>8__locals2 = CS$<>8__locals1;
				int version = this._version + 1;
				this._version = version;
				CS$<>8__locals2.version = version;
				this._currSelectedAreaMoveInfo = null;
				bool flag = areaId != this._mapModel.CurrentAreaId;
				if (flag)
				{
					MapDomainMethod.AsyncCall.GetTravelCost(this, this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, areaId, delegate(int offset, RawDataPool pool)
					{
						bool flag2 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
						if (!flag2)
						{
							Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._currSelectedAreaMoveInfo);
							ViewPartWorldMap <>4__this = CS$<>8__locals1.<>4__this;
							IEnumerable<short> areaList = CS$<>8__locals1.<>4__this._currSelectedAreaMoveInfo.Route.AreaList;
							IEnumerable<short> second = CS$<>8__locals1.<>4__this._currSelectedAreaMoveInfo.Route.AreaList.Prepend(CS$<>8__locals1.<>4__this._mapModel.CurrentAreaId);
							Func<short, short, ValueTuple<short, short>> resultSelector;
							if ((resultSelector = CS$<>8__locals1.<>9__1) == null)
							{
								resultSelector = (CS$<>8__locals1.<>9__1 = ((short s, short s1) => new ValueTuple<short, short>(CS$<>8__locals1.<>4__this._mapModel.Areas[(int)s].GetTemplateId(), CS$<>8__locals1.<>4__this._mapModel.Areas[(int)s1].GetTemplateId())));
							}
							<>4__this.RenderRoute(areaList.Zip(second, resultSelector));
						}
					});
				}
			}
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x0029013F File Offset: 0x0028E33F
		private void RenderRoute([TupleElementNames(new string[]
		{
			"toId",
			"fromId"
		})] IEnumerable<ValueTuple<short, short>> path)
		{
			this.map.generator.Vertices = path.SelectMany(([TupleElementNames(new string[]
			{
				"toId",
				"fromId"
			})] ValueTuple<short, short> item) => this._travelRouteHelper.GetRoute(item.Item2, item.Item1)).ToArray<Vector3>();
		}

		// Token: 0x06005852 RID: 22610 RVA: 0x0029016C File Offset: 0x0028E36C
		private void OnMouseExitAreaTemplateId(short areaTemplateId)
		{
			bool traveling = this._moveInfo.Traveling;
			if (!traveling)
			{
				bool flag = this._currSelectedAreaMoveInfo != null;
				if (flag)
				{
					this.ClearTravelPath();
				}
				else
				{
					this._version++;
				}
				this._currSelectedAreaId = -1;
				this._currSelectedAreaMoveInfo = null;
			}
		}

		// Token: 0x06005853 RID: 22611 RVA: 0x002901BD File Offset: 0x0028E3BD
		private void ClearTravelPath()
		{
			this._travelPathVertices.Clear();
			this.map.generator.Vertices = Array.Empty<Vector3>();
		}

		// Token: 0x06005854 RID: 22612 RVA: 0x002901E4 File Offset: 0x0028E3E4
		private void OnHide()
		{
			bool flag = new Location(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId).IsValid();
			if (flag)
			{
				bool isVisited = false;
				bool flag2 = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId() == this._mapModel.CurrentAreaId;
				if (flag2)
				{
					isVisited = true;
				}
				else
				{
					this._taiwuVisitedAreas.TryGetValue(this._mapModel.CurrentAreaId, out isVisited);
				}
				ViewNewAreaNotify.Setup(SingletonObject.getInstance<WorldMapModel>().Areas[(int)this._mapModel.CurrentAreaId].GetConfig().TemplateId, isVisited);
			}
		}

		// Token: 0x06005855 RID: 22613 RVA: 0x00290280 File Offset: 0x0028E480
		public void FinishAndHideSelf()
		{
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(this.OnHide));
			GEvent.ClearEvent(UiEvents.OnTravelCheckPointProcessed);
			GEvent.OnEvent(UiEvents.OnTravelFinish, null);
			this.HideSelf();
			this._isTravelAutoPaused = false;
		}

		// Token: 0x06005856 RID: 22614 RVA: 0x002902DE File Offset: 0x0028E4DE
		public void HideSelf()
		{
			CommandKitBase.SetDisable(false);
			UIManager.Instance.HideUI(UIElement.StatePartWorldMap);
		}

		// Token: 0x06005857 RID: 22615 RVA: 0x002902F8 File Offset: 0x0028E4F8
		public override void QuickHide()
		{
			bool flag = this._startTravel && !this._travelIsFinish;
			if (!flag)
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				this.OnMouseExitAreaTemplateId((short)this._currSelectedStateId);
				this.areaStateItemController.gameObject.SetActive(false);
				bool flag2 = this._travelDestAreaId < 0;
				if (flag2)
				{
					this.HideSelf();
				}
			}
		}

		// Token: 0x06005858 RID: 22616 RVA: 0x00290368 File Offset: 0x0028E568
		private void ProcessCachedGEvents()
		{
			foreach (ValueTuple<Enum, ArgumentBox> eventPair in this.GEventsOnExit)
			{
				GEvent.OnEvent(eventPair.Item1, eventPair.Item2);
			}
			this.GEventsOnExit.Clear();
		}

		// Token: 0x06005859 RID: 22617 RVA: 0x002903D8 File Offset: 0x0028E5D8
		private void ClearWait()
		{
			this._continueWaitingSeconds = -1f;
		}

		// Token: 0x0600585A RID: 22618 RVA: 0x002903E8 File Offset: 0x0028E5E8
		private void StopTravel()
		{
			this.ClearWait();
			MapDomainMethod.Call.StopTravel();
			bool flag = this._moveInfo.CurrentAreaId == this._moveInfo.FromAreaId;
			if (flag)
			{
				this.HideSelf();
			}
			this.ProcessCachedGEvents();
			this.EnableStateMask(true, true);
			this.ChangeTravelInteractable(false);
			this._isTravelAutoPaused = false;
		}

		// Token: 0x0600585B RID: 22619 RVA: 0x00290448 File Offset: 0x0028E648
		private void EnableStateMask(bool enable, bool callMouseExit = true)
		{
			if (callMouseExit)
			{
				this.OnMouseExitAreaTemplateId((short)this._currSelectedStateId);
			}
			this.map.SetStateHover(enable);
		}

		// Token: 0x0600585C RID: 22620 RVA: 0x00290475 File Offset: 0x0028E675
		private void ChangeTravelInteractable(bool interactable, bool isPause)
		{
			this.pauseToggle.isOn = isPause;
			this.ChangeTravelInteractable(interactable);
		}

		// Token: 0x0600585D RID: 22621 RVA: 0x0029048D File Offset: 0x0028E68D
		private void ChangeTravelInteractable(bool interactable)
		{
			this.pauseToggle.interactable = interactable;
		}

		// Token: 0x0600585E RID: 22622 RVA: 0x0029049D File Offset: 0x0028E69D
		private void ChangeTravelInteractable()
		{
			this.ChangeTravelInteractable(this.CanTravelInteractable);
		}

		// Token: 0x0600585F RID: 22623 RVA: 0x002904AC File Offset: 0x0028E6AC
		private void RefreshPauseToggle(bool isOn)
		{
			GEvent.OnEvent(UiEvents.OnTravelPauseStatusChanged, null);
			this.pauseImage.sprite = (this.pauseToggle.interactable ? (isOn ? this.continueTravel : this.pauseTravel) : this.cannotPauseTravel);
			this.pauseText.text = (isOn ? LanguageKey.UI_MiniMap_PartWorld_Continue.Tr() : LanguageKey.UI_MiniMap_PartWorld_Pause.Tr());
			this.OnCharacterTaiwuCarrierChanged(null);
		}

		// Token: 0x06005860 RID: 22624 RVA: 0x00290527 File Offset: 0x0028E727
		private void OnClickMapLegendButton()
		{
			this.areaStateItemController.gameObject.SetActive(!this.areaStateItemController.gameObject.activeSelf);
		}

		// Token: 0x0600586C RID: 22636 RVA: 0x00290B8C File Offset: 0x0028ED8C
		[CompilerGenerated]
		private void <Awake>g__Search|100_7(string str)
		{
			bool flag = string.IsNullOrWhiteSpace(str);
			if (flag)
			{
				this.info.RefreshShowArea((short _) => true);
			}
			else
			{
				this.info.RefreshShowArea(delegate(short template)
				{
					sbyte item = MapArea.Instance[template].StateID;
					return (item >= 0 && MapState.Instance[item].Name.Contains(str)) || MapArea.Instance[template].Name.Contains(str);
				});
			}
		}

		// Token: 0x04003C7B RID: 15483
		private const float TravelProgressAniSpeed = 0.15f;

		// Token: 0x04003C7C RID: 15484
		private const float TravelMoveSpeed = 1000f;

		// Token: 0x04003C7D RID: 15485
		private float _lastTravelAnimDuration;

		// Token: 0x04003C7E RID: 15486
		private WorldMapModel _mapModel;

		// Token: 0x04003C7F RID: 15487
		private bool _isDirectTraveling;

		// Token: 0x04003C80 RID: 15488
		private KidnappedTravelData _kidnappedTravelData;

		// Token: 0x04003C81 RID: 15489
		private CrossAreaMoveInfo _moveInfo;

		// Token: 0x04003C82 RID: 15490
		private CrossAreaMoveInfo _newMoveInfo;

		// Token: 0x04003C83 RID: 15491
		private CharacterDisplayData _taiwuDisplayData;

		// Token: 0x04003C84 RID: 15492
		private List<ItemKey> _gainsInTravel;

		// Token: 0x04003C85 RID: 15493
		private readonly Dictionary<short, bool> _taiwuVisitedAreas = new Dictionary<short, bool>();

		// Token: 0x04003C86 RID: 15494
		private AreaDisplayData[] _areaDisplayData;

		// Token: 0x04003C87 RID: 15495
		private CrossAreaMoveInfo _currSelectedAreaMoveInfo;

		// Token: 0x04003C88 RID: 15496
		private TravelRouteHelper<Vector3> _travelRouteHelper;

		// Token: 0x04003C89 RID: 15497
		private bool _travelIsFinish;

		// Token: 0x04003C8A RID: 15498
		private float _continueWaitingSeconds;

		// Token: 0x04003C8B RID: 15499
		private short _travelDestAreaId;

		// Token: 0x04003C8C RID: 15500
		private int _nextAreaCostDays;

		// Token: 0x04003C8D RID: 15501
		private sbyte _currSelectedStateId = -1;

		// Token: 0x04003C8E RID: 15502
		private short _currSelectedAreaId = -1;

		// Token: 0x04003C8F RID: 15503
		private RectTransform _focusTaiwuVillage;

		// Token: 0x04003C90 RID: 15504
		private Vector2 _taiwuVillagePos;

		// Token: 0x04003C91 RID: 15505
		private bool _isTravelAutoPaused;

		// Token: 0x04003C92 RID: 15506
		private bool _isQuickHiding;

		// Token: 0x04003C93 RID: 15507
		private bool _isAreaStatusActive;

		// Token: 0x04003C94 RID: 15508
		private bool? _travelAnimLastIdleState;

		// Token: 0x04003C95 RID: 15509
		private readonly List<Vector2> _travelPathVertices = new List<Vector2>();

		// Token: 0x04003C96 RID: 15510
		[NonSerialized]
		public readonly List<ValueTuple<Enum, ArgumentBox>> GEventsOnExit = new List<ValueTuple<Enum, ArgumentBox>>();

		// Token: 0x04003C97 RID: 15511
		private const string AreaEffectPrefabKey = "ViewPartWorldMapAreaEffectPrefabKey";

		// Token: 0x04003C98 RID: 15512
		[SerializeField]
		private AreaEffect areaEffectPrefab;

		// Token: 0x04003C99 RID: 15513
		[SerializeField]
		private TMP_InputField search;

		// Token: 0x04003C9A RID: 15514
		[SerializeField]
		private AreaMap map;

		// Token: 0x04003C9B RID: 15515
		[SerializeField]
		private AreaMap info;

		// Token: 0x04003C9C RID: 15516
		[SerializeField]
		private CSlider zoom;

		// Token: 0x04003C9D RID: 15517
		[SerializeField]
		private CButton zoomBtn;

		// Token: 0x04003C9E RID: 15518
		[SerializeField]
		private AudioClip seEnter;

		// Token: 0x04003C9F RID: 15519
		[SerializeField]
		private AudioClip seCursor;

		// Token: 0x04003CA0 RID: 15520
		[SerializeField]
		private CToggle pauseToggle;

		// Token: 0x04003CA1 RID: 15521
		[SerializeField]
		private CImage pauseImage;

		// Token: 0x04003CA2 RID: 15522
		[SerializeField]
		private CImage cancelImage;

		// Token: 0x04003CA3 RID: 15523
		[SerializeField]
		private TMP_Text pauseText;

		// Token: 0x04003CA4 RID: 15524
		[SerializeField]
		private TMP_Text postStationCount;

		// Token: 0x04003CA5 RID: 15525
		[SerializeField]
		private Sprite continueTravel;

		// Token: 0x04003CA6 RID: 15526
		[SerializeField]
		private Sprite pauseTravel;

		// Token: 0x04003CA7 RID: 15527
		[SerializeField]
		private Sprite cannotPauseTravel;

		// Token: 0x04003CA8 RID: 15528
		[SerializeField]
		private Sprite cancelSprite;

		// Token: 0x04003CA9 RID: 15529
		[SerializeField]
		private Sprite disableCancelSprite;

		// Token: 0x04003CAA RID: 15530
		[SerializeField]
		private CButton cancelButton;

		// Token: 0x04003CAB RID: 15531
		[SerializeField]
		private TravelAnimation anim;

		// Token: 0x04003CAC RID: 15532
		[SerializeField]
		private AreaStateItemController areaStateItemController;

		// Token: 0x04003CAD RID: 15533
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04003CAE RID: 15534
		[SerializeField]
		private PositionFollower moveTarget;

		// Token: 0x04003CAF RID: 15535
		[SerializeField]
		private GameObject clickBlocker;

		// Token: 0x04003CB0 RID: 15536
		[SerializeField]
		private int zoomLevel = 1;

		// Token: 0x04003CB1 RID: 15537
		[SerializeField]
		private int zoomThreshold = 8;

		// Token: 0x04003CB2 RID: 15538
		[SerializeField]
		private CButton mapLegendBtn;

		// Token: 0x04003CB3 RID: 15539
		[SerializeField]
		private Area chaishan;

		// Token: 0x04003CB4 RID: 15540
		[SerializeField]
		private UIParticle hideEffect;

		// Token: 0x04003CB5 RID: 15541
		[SerializeField]
		private UIParticle showEffect;

		// Token: 0x04003CB6 RID: 15542
		[SerializeField]
		private PositionFollower hidePosition;

		// Token: 0x04003CB7 RID: 15543
		[SerializeField]
		private PositionFollower showPosition;

		// Token: 0x04003CB8 RID: 15544
		[SerializeField]
		private PositionFollower[] twelveFollower;

		// Token: 0x04003CB9 RID: 15545
		[SerializeField]
		private TargetIndicator locateTaiwuVillage;

		// Token: 0x04003CBA RID: 15546
		private bool _centerZoom = false;

		// Token: 0x04003CBB RID: 15547
		private bool _disableCheckClose;

		// Token: 0x04003CBC RID: 15548
		private bool animState = false;

		// Token: 0x04003CBD RID: 15549
		private bool _waitForAnimation;

		// Token: 0x04003CBE RID: 15550
		private bool _startTravel;

		// Token: 0x04003CBF RID: 15551
		private float _towards = 1f;

		// Token: 0x04003CC0 RID: 15552
		private SmoothCurve _sc;

		// Token: 0x04003CC1 RID: 15553
		private bool _guidingEnter;

		// Token: 0x04003CC2 RID: 15554
		private bool _guidingSectExam;

		// Token: 0x04003CC3 RID: 15555
		private bool _scrollLock;

		// Token: 0x04003CC4 RID: 15556
		private int _version;
	}
}

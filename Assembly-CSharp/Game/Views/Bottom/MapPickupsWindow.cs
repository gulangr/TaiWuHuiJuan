using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Map;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using Map.RenderSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3E RID: 3134
	public class MapPickupsWindow : MonoBehaviour
	{
		// Token: 0x170010CA RID: 4298
		// (get) Token: 0x06009F28 RID: 40744 RVA: 0x004A6117 File Offset: 0x004A4317
		public WorldMapModel MapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x170010CB RID: 4299
		// (get) Token: 0x06009F29 RID: 40745 RVA: 0x004A611E File Offset: 0x004A431E
		public MapBlockData PlayerAtBlock
		{
			get
			{
				return this.MapModel.PlayerAtBlock;
			}
		}

		// Token: 0x170010CC RID: 4300
		// (get) Token: 0x06009F2A RID: 40746 RVA: 0x004A612B File Offset: 0x004A432B
		public MapBlockData SelectedBlock
		{
			get
			{
				return this.MapModel.SelectedBlock;
			}
		}

		// Token: 0x170010CD RID: 4301
		// (get) Token: 0x06009F2B RID: 40747 RVA: 0x004A6138 File Offset: 0x004A4338
		private ViewWorldMap WorldMap
		{
			get
			{
				UIElement worldMap = UIElement.WorldMap;
				return (worldMap != null) ? worldMap.UiBaseAs<ViewWorldMap>() : null;
			}
		}

		// Token: 0x06009F2C RID: 40748 RVA: 0x004A614C File Offset: 0x004A434C
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.rect = base.GetComponent<RectTransform>();
				this._btnPickupAll.onClick.AddListener(delegate()
				{
					bool flowControl = this.TryPickupAll();
					bool flag2 = !flowControl;
					if (flag2)
					{
					}
				});
				this._btnPickupAll.interactable = true;
				PointerTrigger pointerTrigger = this._btnPickupAll.GetComponent<PointerTrigger>();
				bool flag = pointerTrigger != null;
				if (flag)
				{
					PointerTrigger pointerTrigger2 = pointerTrigger;
					if (pointerTrigger2.EnterEvent == null)
					{
						pointerTrigger2.EnterEvent = new UnityEvent();
					}
					pointerTrigger2 = pointerTrigger;
					if (pointerTrigger2.ExitEvent == null)
					{
						pointerTrigger2.ExitEvent = new UnityEvent();
					}
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						this.SetPickupItemHighlight(true);
					});
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						this.SetPickupItemHighlight(false);
					});
				}
			}
		}

		// Token: 0x06009F2D RID: 40749 RVA: 0x004A621C File Offset: 0x004A441C
		public bool TryPickupAll()
		{
			bool flag = this._currentBlockData == null || this.WorldMap == null;
			bool result;
			if (flag)
			{
				Debug.LogWarning(string.Format("[MapPickupsWindow] TryPickupAll aborted: _currentBlockData={0}, _worldMap={1}", this._currentBlockData, this.WorldMap));
				result = false;
			}
			else
			{
				Vector2 targetLocation = MapRenderSystem.GetBlockLocalPos(this._currentBlockData.GetLocation());
				Debug.Log(string.Format("test :{0} -- {1}", targetLocation, this.WorldMap.LastCameraMoveTarget));
				bool flag2 = targetLocation != this.WorldMap.LastCameraMoveTarget;
				if (flag2)
				{
					GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", this._currentBlockData.GetLocation()).Set("doTween", false).Set("tweenTime", 0f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
					result = false;
				}
				else
				{
					bool flag3 = this.SelectedBlock != this.PlayerAtBlock;
					if (flag3)
					{
						this._pathFinding = true;
						this._lockedLocation = new Location?(this._currentBlockData.GetLocation());
						GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", this.SelectedBlock.GetLocation()));
						this._currentPickupCoor = this.SelectedBlock.GetBlockPos();
						this._actionWhenMoveFinish = delegate()
						{
							bool flag4 = this.SelectedBlock == this.PlayerAtBlock;
							if (flag4)
							{
								this.PickupAll();
							}
						};
						result = false;
					}
					else
					{
						this.PickupAll();
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06009F2E RID: 40750 RVA: 0x004A63A8 File Offset: 0x004A45A8
		private void Update()
		{
			bool flag = MapCommandKit.PickupMapItems.Check(UIElement.WorldMap, false, false, false, true, false) && this.IsOnThisBlock();
			if (flag)
			{
				Button.ButtonClickedEvent onClick = this._btnPickupAll.onClick;
				if (onClick != null)
				{
					onClick.Invoke();
				}
			}
			bool flag2 = !this._pathFinding;
			if (!flag2)
			{
				bool flag3 = this.MapModel.TaiwuMoveState == WorldMapModel.MoveState.WaitEventShow;
				if (flag3)
				{
					this._pathFinding = false;
					this._lockedLocation = null;
					this._actionWhenMoveFinish = null;
				}
				else
				{
					ViewWorldMap worldMap = this.WorldMap;
					bool flag4 = worldMap != null && !worldMap.IsMoving && this._currentPickupCoor == this.PlayerAtBlock.GetBlockPos();
					if (flag4)
					{
						GlobalSettings setting = SingletonObject.getInstance<GlobalSettings>();
						bool enableAutoTriggerNormalMapPickup = setting.EnableAutoTriggerNormalMapPickup;
						if (enableAutoTriggerNormalMapPickup)
						{
							this._pathFinding = false;
							this._lockedLocation = null;
						}
						else
						{
							Action actionWhenMoveFinish = this._actionWhenMoveFinish;
							if (actionWhenMoveFinish != null)
							{
								actionWhenMoveFinish();
							}
							this._pathFinding = false;
							this._lockedLocation = null;
						}
					}
				}
			}
		}

		// Token: 0x06009F2F RID: 40751 RVA: 0x004A64BC File Offset: 0x004A46BC
		private bool IsOnThisBlock()
		{
			WorldMapModel mapModel = this.MapModel;
			return mapModel.CurrentAreaId == this._currentBlockData.AreaId && mapModel.CurrentBlockId == this._currentBlockData.BlockId;
		}

		// Token: 0x06009F30 RID: 40752 RVA: 0x004A6500 File Offset: 0x004A4700
		public void SetPickupItemHighlight(bool isHighlight)
		{
			foreach (PickUpItem item in this._currentPickupItems)
			{
				item.SetHighlight(isHighlight);
			}
		}

		// Token: 0x06009F31 RID: 40753 RVA: 0x004A655C File Offset: 0x004A475C
		private void PickupAll()
		{
			TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
			{
				Location = this._currentBlockData.GetLocation(),
				PickAll = true
			});
		}

		// Token: 0x06009F32 RID: 40754 RVA: 0x004A6594 File Offset: 0x004A4794
		public void RefreshCarrierInfo(int exploreBonusRate)
		{
			this._carrierBonusRate.transform.gameObject.SetActive(exploreBonusRate > 0);
			bool flag = exploreBonusRate <= 0;
			if (!flag)
			{
				this._carrierBonusRate.text = string.Format("{0}%", exploreBonusRate);
			}
		}

		// Token: 0x06009F33 RID: 40755 RVA: 0x004A65E8 File Offset: 0x004A47E8
		public void RefreshPickupItemInfos(MapBlockData blockData = null)
		{
			bool flag = this._pathFinding && this._lockedLocation != null;
			if (flag)
			{
				bool flag2 = blockData == null || blockData.GetLocation() != this._lockedLocation.Value;
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = blockData == null || UIManager.Instance.IsElementActive(UIElement.PartWorld) || UIManager.Instance.IsElementActive(UIElement.AdventureMajorEvent) || UIManager.Instance.IsElementActive(UIElement.BuildingArea) || UIManager.Instance.IsElementActive(UIElement.AdventureRemake);
			if (flag3)
			{
				this.ClearDisplay();
				base.gameObject.SetActive(false);
			}
			else
			{
				this._currentBlockData = blockData;
				bool flag4 = !SingletonObject.getInstance<WorldMapModel>().IsLocationShouldInSight(blockData.GetLocation());
				if (flag4)
				{
					this.RefreshItemsDisplay(null);
				}
				else
				{
					List<MapElementPickupDisplayData> pickupDisplayInfo;
					SingletonObject.getInstance<WorldMapModel>().VisibleMapPickupDict.TryGetValue(blockData.GetLocation(), out pickupDisplayInfo);
					this.RefreshItemsDisplay(pickupDisplayInfo);
				}
			}
		}

		// Token: 0x06009F34 RID: 40756 RVA: 0x004A66E6 File Offset: 0x004A48E6
		public void RefreshPickupItemInfos()
		{
			this.RefreshPickupItemInfos(this._currentBlockData);
		}

		// Token: 0x06009F35 RID: 40757 RVA: 0x004A66F8 File Offset: 0x004A48F8
		private void ClearDisplay()
		{
			CommonUtils.PrepareEnoughChildren(this._container, this._pickUpItemPrefab.gameObject, 0, null);
		}

		// Token: 0x06009F36 RID: 40758 RVA: 0x004A6728 File Offset: 0x004A4928
		private void RefreshItemsDisplay(List<MapElementPickupDisplayData> pickupDisplayInfo)
		{
			bool flag = pickupDisplayInfo == null || pickupDisplayInfo.Count <= 0 || this.rect == null;
			if (flag)
			{
				this.ClearDisplay();
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
				this._btnPickupAll.gameObject.SetActive(pickupDisplayInfo.Count >= 2);
				this._currentPickupItems.Clear();
				CommonUtils.PrepareEnoughChildren(this._container, this._pickUpItemPrefab.gameObject, pickupDisplayInfo.Count, null);
				for (int i = 0; i < pickupDisplayInfo.Count; i++)
				{
					PickUpItem item = this._container.GetChild(i).GetComponent<PickUpItem>();
					this._currentPickupItems.Add(item);
					MapPickup mapPickup = pickupDisplayInfo[i].Pickup;
					bool flag2 = mapPickup.ItemType >= 0;
					if (flag2)
					{
						ItemDisplayData itemData = new ItemDisplayData(mapPickup.ItemType, mapPickup.ItemTemplateId);
					}
					else
					{
						bool flag3 = mapPickup.Type == MapPickup.EMapPickupType.Resource;
						if (flag3)
						{
							ItemDisplayData itemData = new ItemDisplayData(12, (short)mapPickup.ResourceType);
						}
						else
						{
							ItemDisplayData itemData = new ItemDisplayData(ItemKey.Invalid, -1);
						}
					}
					int tempIndex = i;
					item.Set(mapPickup.Template.Icon, mapPickup.Template.TipsContent, this.GetPickAmount(mapPickup), mapPickup.HasXiangshuMinion, pickupDisplayInfo[i].CanAutoBeatXiangshuMinion, pickupDisplayInfo[i].BanReason, mapPickup.Type, delegate
					{
						this.PickUpSingle(tempIndex);
					});
				}
				RectTransform containerParent = this._container.parent as RectTransform;
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					containerParent.sizeDelta = containerParent.sizeDelta.SetY(Math.Min(this._container.sizeDelta.y, 200f));
					containerParent.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
					containerParent.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
				});
			}
		}

		// Token: 0x06009F37 RID: 40759 RVA: 0x004A6928 File Offset: 0x004A4B28
		private void PickUpSingle(int targetIndex)
		{
			bool flag = this._currentBlockData == null || this.WorldMap == null;
			if (flag)
			{
				Debug.LogWarning(string.Format("[MapPickupsWindow] PickUpSingle aborted: targetIndex={0}, _currentBlockData={1}, _worldMap={2}", targetIndex, this._currentBlockData, this.WorldMap));
			}
			else
			{
				Vector2 targetLocation = MapRenderSystem.GetBlockLocalPos(this._currentBlockData.GetLocation());
				Debug.Log(string.Format("test :{0} -- {1}", targetLocation, this.WorldMap.LastCameraMoveTarget));
				bool flag2 = targetLocation != this.WorldMap.LastCameraMoveTarget;
				if (flag2)
				{
					GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", this._currentBlockData.GetLocation()).Set("doTween", false).Set("tweenTime", 0f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
				}
				else
				{
					bool flag3 = this.SelectedBlock != this.PlayerAtBlock;
					if (flag3)
					{
						this._pathFinding = true;
						this._lockedLocation = new Location?(this._currentBlockData.GetLocation());
						GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", this.SelectedBlock.GetLocation()));
						this._currentPickupCoor = this.SelectedBlock.GetBlockPos();
						this._actionWhenMoveFinish = delegate()
						{
							bool flag4 = this.SelectedBlock == this.PlayerAtBlock;
							if (flag4)
							{
								TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
								{
									Location = this._currentBlockData.GetLocation(),
									PickAll = false,
									PickupIndex = targetIndex
								});
							}
						};
					}
					else
					{
						TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
						{
							Location = this._currentBlockData.GetLocation(),
							PickAll = false,
							PickupIndex = targetIndex
						});
					}
				}
			}
		}

		// Token: 0x06009F38 RID: 40760 RVA: 0x004A6AF8 File Offset: 0x004A4CF8
		private string GetPickAmount(MapPickup mapPickup)
		{
			switch (mapPickup.Type)
			{
			case MapPickup.EMapPickupType.Resource:
				return mapPickup.ResourceCount.ToString();
			case MapPickup.EMapPickupType.Item:
				return "1";
			case MapPickup.EMapPickupType.ExpBonus:
				return mapPickup.ExpCount.ToString();
			case MapPickup.EMapPickupType.DebtBonus:
				return mapPickup.DebtCount.ToString();
			}
			return "1";
		}

		// Token: 0x04007B14 RID: 31508
		[SerializeField]
		private PickUpItem _pickUpItemPrefab;

		// Token: 0x04007B15 RID: 31509
		[SerializeField]
		private TextMeshProUGUI _carrierBonusRate;

		// Token: 0x04007B16 RID: 31510
		[SerializeField]
		private CButton _btnPickupAll;

		// Token: 0x04007B17 RID: 31511
		[SerializeField]
		private RectTransform _container;

		// Token: 0x04007B18 RID: 31512
		public float basicHeight;

		// Token: 0x04007B19 RID: 31513
		public float elementHeight;

		// Token: 0x04007B1A RID: 31514
		private RectTransform rect;

		// Token: 0x04007B1B RID: 31515
		private MapBlockData _currentBlockData;

		// Token: 0x04007B1C RID: 31516
		private readonly List<PickUpItem> _currentPickupItems = new List<PickUpItem>();

		// Token: 0x04007B1D RID: 31517
		private ByteCoordinate _currentPickupCoor;

		// Token: 0x04007B1E RID: 31518
		private bool _pathFinding = false;

		// Token: 0x04007B1F RID: 31519
		private Location? _lockedLocation = null;

		// Token: 0x04007B20 RID: 31520
		private Action _actionWhenMoveFinish;

		// Token: 0x04007B21 RID: 31521
		private bool _inited = false;
	}
}
